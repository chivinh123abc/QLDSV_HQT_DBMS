using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Domain.Enums;

namespace QLDSV_HTC.Infrastructure.Repositories;

public class DynamicReportRepository(IDbConnectionProvider connectionProvider)
    : BaseSqlRepository(connectionProvider), IDynamicReportRepository
{
    public async Task<IEnumerable<string>> GetAllowedTablesAsync()
    {
        const string query = @"
            SELECT TABLE_NAME 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_TYPE IN ('BASE TABLE', 'VIEW') 
              AND TABLE_NAME NOT LIKE 'SPRING_%' 
              AND TABLE_NAME NOT LIKE 'sys%'
              AND HAS_PERMS_BY_NAME('[dbo].[' + TABLE_NAME + ']', 'OBJECT', 'SELECT') = 1
            ORDER BY TABLE_NAME";
        var dt = await ExecuteQueryAsync(query, CommandType.Text);

        var tables = new List<string>();
        foreach (DataRow row in dt.Rows)
        {
            tables.Add(row["TABLE_NAME"].ToString()!);
        }

        return tables;
    }

    public async Task<IEnumerable<string>> GetTableColumnsAsync(string tableName)
    {
        const string query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION";
        var dt = await ExecuteQueryAsync(query, CommandType.Text, new SqlParameter("@TableName", tableName));

        var columns = new List<string>();
        foreach (DataRow row in dt.Rows)
        {
            columns.Add(row["COLUMN_NAME"].ToString()!);
        }

        return columns;
    }

    public async Task<(DataTable Data, string Sql, int TotalCount)> GetPreviewDataAsync(DynamicQueryRequestDto request)
    {
        var (sql, parameters) = BuildDynamicQuery(request, isPagination: true);
        var dataTable = await ExecuteQueryAsync(sql, CommandType.Text, parameters);

        // Build count query (reuse same WHERE/JOIN/GROUP BY but SELECT COUNT)
        var (countSql, countParams) = BuildCountQuery(request);
        var countDt = await ExecuteQueryAsync(countSql, CommandType.Text, countParams);
        int totalCount = countDt.Rows.Count > 0 ? Convert.ToInt32(countDt.Rows[0][0]) : 0;

        return (dataTable, sql, totalCount);
    }

    public Task<string> GetSqlPreviewAsync(DynamicQueryRequestDto request)
    {
        var (sql, _) = BuildDynamicQuery(request, isPagination: true);
        return Task.FromResult(sql);
    }

    public async Task<DataTable> GetReportDataAsync(DynamicQueryRequestDto request)
    {
        var (sql, parameters) = BuildDynamicQuery(request, isPagination: false);
        return await ExecuteQueryAsync(sql, CommandType.Text, parameters);
    }

    #region Query Builder Logic

    private static (string Sql, SqlParameter[] Parameters) BuildDynamicQuery(DynamicQueryRequestDto request, bool isPagination)
    {
        var parameters = new List<SqlParameter>();
        string safeTableName = request.TableName.Replace("]", "]]");

        // 1. Build Select Clause
        string selectClause = BuildSelectClause(request, out var groupByColumns, out var fallbackOrderByColumn);

        // 2. Build FROM & JOINs
        var sqlBuilder = new StringBuilder();
        sqlBuilder.Append(DbConstants.SqlKeywords.Select).Append(' ').Append(selectClause)
                  .Append(" \n").Append(DbConstants.SqlKeywords.From).Append(" [dbo].[").Append(safeTableName).Append(']');
        ApplyJoins(sqlBuilder, request, safeTableName);

        // 3. Build Where Clause
        ApplyFilters(sqlBuilder, request, safeTableName, parameters);

        // 4. Build GROUP BY & HAVING
        ApplyGroupingAndHaving(sqlBuilder, request, groupByColumns, parameters);

        // 5. Build ORDER BY + Pagination
        ApplyOrderByAndPagination(sqlBuilder, request, fallbackOrderByColumn, isPagination);

        return (sqlBuilder.ToString(), parameters.ToArray());
    }

    private static (string Sql, SqlParameter[] Parameters) BuildCountQuery(DynamicQueryRequestDto request)
    {
        var parameters = new List<SqlParameter>();
        string safeTableName = request.TableName.Replace("]", "]]");

        string selectClause = BuildSelectClause(request, out var groupByColumns, out _);

        var sqlBuilder = new StringBuilder();

        if (groupByColumns.Count > 0)
        {
            // When GROUP BY exists, count the number of groups
            sqlBuilder.Append("SELECT COUNT(*) FROM (SELECT ")
                      .Append(selectClause)
                      .Append(" \n").Append(DbConstants.SqlKeywords.From).Append(" [dbo].[").Append(safeTableName).Append(']');
        }
        else
        {
            sqlBuilder.Append("SELECT COUNT(*) ")
                      .Append(" \n").Append(DbConstants.SqlKeywords.From).Append(" [dbo].[").Append(safeTableName).Append(']');
        }

        ApplyJoins(sqlBuilder, request, safeTableName);
        ApplyFilters(sqlBuilder, request, safeTableName, parameters);
        ApplyGroupingAndHaving(sqlBuilder, request, groupByColumns, parameters);

        if (groupByColumns.Count > 0)
        {
            sqlBuilder.Append(") AS CountQuery");
        }

        return (sqlBuilder.ToString(), parameters.ToArray());
    }

    private static string BuildSelectClause(DynamicQueryRequestDto request, out List<string> groupByColumns, out string fallbackOrderByColumn)
    {
        groupByColumns = [];
        fallbackOrderByColumn = "(SELECT NULL)";
        string safeTableName = request.TableName.Replace("]", "]]");

        if (request.AdvancedSelectColumns?.Count > 0)
        {
            var uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var selectedColumns = new List<string>();
            bool hasAggregates = request.AdvancedSelectColumns.Any(c => c.Aggregate != AggregateType.None);
            bool hasRawColumns = request.AdvancedSelectColumns.Any(c => c.Aggregate == AggregateType.None);

            foreach (var c in request.AdvancedSelectColumns)
            {
                string colName = c.ColumnName.Replace("]", "]]");
                string safeColTable = c.TableName.Replace("]", "]]");
                string qualifiedCol = string.IsNullOrWhiteSpace(c.Expression)
                    ? $"[{safeColTable}].[{colName}]"
                    : c.Expression;

                bool isRaw = c.Aggregate == AggregateType.None;
                string aggFunc = isRaw ? "NONE" : c.Aggregate.ToString().ToUpper();
                string aggAlias;
                if (!string.IsNullOrWhiteSpace(c.AliasName))
                {
                    aggAlias = c.AliasName.Replace("]", "]]");
                }
                else
                {
                    aggAlias = isRaw ? colName : $"{aggFunc}_{colName}";
                }

                if (!string.IsNullOrWhiteSpace(c.Expression) && string.IsNullOrWhiteSpace(c.AliasName))
                {
                    aggAlias = "ComputedColumn";
                }

                string uniqueKey = $"{qualifiedCol}_{aggFunc}_{aggAlias}";
                if (!uniqueKeys.Add(uniqueKey)) continue;

                if (isRaw)
                {
                    selectedColumns.Add($"{qualifiedCol} AS [{aggAlias}]");
                    // F1: Only add to GROUP BY if there are ALSO aggregate columns AND raw columns coexist
                    if (hasAggregates && hasRawColumns)
                    {
                        groupByColumns.Add(qualifiedCol);
                    }
                }
                else
                {
                    selectedColumns.Add($"{aggFunc}({qualifiedCol}) AS [{aggAlias}]");
                }

                if (fallbackOrderByColumn == "(SELECT NULL)") fallbackOrderByColumn = isRaw ? qualifiedCol : $"[{aggAlias}]";
            }
            return string.Join(", ", selectedColumns);
        }

        if (request.SelectColumns?.Count > 0)
        {
            fallbackOrderByColumn = $"[{safeTableName}].[{request.SelectColumns[0].Replace("]", "]]")}]";
            return string.Join(", ", request.SelectColumns.Select(c => $"[{safeTableName}].[{c.Replace("]", "]]")}]"));
        }

        return "*";
    }

    private static void ApplyJoins(StringBuilder sqlBuilder, DynamicQueryRequestDto request, string mainTable)
    {
        if (request.Joins == null || request.Joins.Count == 0) return;

        var joinedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { mainTable };
        foreach (var join in request.Joins)
        {
            string joinSafeTable = join.JoinTable.Replace("]", "]]");
            string safeJoinType = join.JoinType.Trim().ToUpperInvariant();

            if (safeJoinType != DbConstants.SqlKeywords.InnerJoin &&
                safeJoinType != DbConstants.SqlKeywords.LeftJoin &&
                safeJoinType != DbConstants.SqlKeywords.RightJoin &&
                safeJoinType != DbConstants.SqlKeywords.FullOuterJoin)
            {
                throw new ArgumentException($"Invalid JoinType: {safeJoinType}");
            }

            string? joinCondition = null;
            foreach (var existingTable in joinedTables)
            {
                joinCondition = TableRelationRegistry.GetJoinCondition(existingTable, joinSafeTable)
                              ?? TableRelationRegistry.GetJoinCondition(joinSafeTable, existingTable);
                if (joinCondition != null) break;
            }

            if (joinCondition != null)
            {
                sqlBuilder.Append('\n').Append(safeJoinType).Append(" [dbo].[").Append(joinSafeTable).Append("] ON ").Append(joinCondition);
                joinedTables.Add(joinSafeTable);
            }
        }
    }

    private static void ApplyFilters(StringBuilder sqlBuilder, DynamicQueryRequestDto request, string mainTable, List<SqlParameter> parameters)
    {
        if (request.Filters == null || request.Filters.Count == 0) return;

        var whereClauses = new List<string>();
        for (int i = 0; i < request.Filters.Count; i++)
        {
            var filter = request.Filters[i];
            string paramName = $"@p{i}";
            string safeColName = filter.ColumnName.Replace("]", "]]");
            string safeTable = string.IsNullOrEmpty(filter.TableName) ? mainTable : filter.TableName.Replace("]", "]]");
            string fullyQualifiedCol = $"[{safeTable}].[{safeColName}]";

            string clause = filter.Operator switch
            {
                FilterOperator.Equals => $"{fullyQualifiedCol} = {paramName}",
                FilterOperator.NotEquals => $"{fullyQualifiedCol} <> {paramName}",
                FilterOperator.Contains => $"{DbConstants.SqlKeywords.Cast}({fullyQualifiedCol} {DbConstants.SqlKeywords.As} NVARCHAR(MAX)) {DbConstants.SqlKeywords.Like} {paramName}",
                FilterOperator.StartsWith => $"{DbConstants.SqlKeywords.Cast}({fullyQualifiedCol} {DbConstants.SqlKeywords.As} NVARCHAR(MAX)) {DbConstants.SqlKeywords.Like} {paramName}",
                FilterOperator.EndsWith => $"{DbConstants.SqlKeywords.Cast}({fullyQualifiedCol} {DbConstants.SqlKeywords.As} NVARCHAR(MAX)) {DbConstants.SqlKeywords.Like} {paramName}",
                FilterOperator.GreaterThan => $"{fullyQualifiedCol} > {paramName}",
                FilterOperator.GreaterThanOrEqual => $"{fullyQualifiedCol} >= {paramName}",
                FilterOperator.LessThan => $"{fullyQualifiedCol} < {paramName}",
                FilterOperator.LessThanOrEqual => $"{fullyQualifiedCol} <= {paramName}",
                FilterOperator.IsNull => $"{fullyQualifiedCol} {DbConstants.SqlKeywords.IsNull}",
                FilterOperator.IsNotNull => $"{fullyQualifiedCol} {DbConstants.SqlKeywords.IsNotNull}",
                FilterOperator.NotLike => $"{DbConstants.SqlKeywords.Cast}({fullyQualifiedCol} {DbConstants.SqlKeywords.As} NVARCHAR(MAX)) {DbConstants.SqlKeywords.NotLike} {paramName}",
                FilterOperator.In => BuildInClause(fullyQualifiedCol, paramName, filter.Value, parameters, false),
                FilterOperator.NotIn => BuildInClause(fullyQualifiedCol, paramName, filter.Value, parameters, true),
                FilterOperator.Between => BuildBetweenClause(fullyQualifiedCol, paramName, filter.Value, parameters),
                _ => ""
            };

            if (!string.IsNullOrEmpty(clause))
            {
                whereClauses.Add(clause);
                if (filter.Operator != FilterOperator.In && filter.Operator != FilterOperator.NotIn &&
                    filter.Operator != FilterOperator.Between && filter.Operator != FilterOperator.IsNull &&
                    filter.Operator != FilterOperator.IsNotNull)
                {
                    object val = filter.Value;
                    if (filter.Operator == FilterOperator.Contains || filter.Operator == FilterOperator.NotLike) val = $"%{filter.Value}%";
                    else if (filter.Operator == FilterOperator.StartsWith) val = $"{filter.Value}%";
                    else if (filter.Operator == FilterOperator.EndsWith) val = $"%{filter.Value}";
                    parameters.Add(new SqlParameter(paramName, val));
                }
            }
        }

        if (whereClauses.Count > 0)
            sqlBuilder.Append('\n').Append(DbConstants.SqlKeywords.Where).Append(' ').AppendJoin($"\n  {DbConstants.SqlKeywords.And} ", whereClauses);
    }

    private static string BuildInClause(string column, string paramBase, string value, List<SqlParameter> parameters, bool isNot)
    {
        var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToArray();
        var paramNames = new List<string>();
        for (int j = 0; j < values.Length; j++)
        {
            string pName = $"{paramBase}_i{j}";
            paramNames.Add(pName);
            parameters.Add(new SqlParameter(pName, values[j]));
        }
        return $"{column} {(isNot ? DbConstants.SqlKeywords.NotIn : DbConstants.SqlKeywords.In)} ({string.Join(", ", paramNames)})";
    }

    private static string BuildBetweenClause(string column, string paramBase, string value, List<SqlParameter> parameters)
    {
        var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToArray();
        if (values.Length < 2) return "";
        string p1 = $"{paramBase}_b1", p2 = $"{paramBase}_b2";
        parameters.Add(new SqlParameter(p1, values[0]));
        parameters.Add(new SqlParameter(p2, values[1]));
        return $"{column} {DbConstants.SqlKeywords.Between} {p1} {DbConstants.SqlKeywords.And} {p2}";
    }

    private static void ApplyGroupingAndHaving(StringBuilder sqlBuilder, DynamicQueryRequestDto request, List<string> groupByColumns, List<SqlParameter> parameters)
    {
        if (groupByColumns.Count == 0) return;

        sqlBuilder.Append('\n').Append(DbConstants.SqlKeywords.GroupBy).Append(' ').AppendJoin(", ", groupByColumns);

        if (request.AdvancedSelectColumns == null) return;

        var havingClauses = new List<string>();
        int hpIndex = 0;
        foreach (var c in request.AdvancedSelectColumns)
        {
            if (!c.HavingOperator.HasValue || string.IsNullOrEmpty(c.HavingValue) || c.Aggregate == AggregateType.None) continue;

            string paramName = $"@hp{hpIndex++}";
            string functionalCol = $"{c.Aggregate.ToString().ToUpper()}([{c.TableName.Replace("]", "]]")}].[{c.ColumnName.Replace("]", "]]")}])";

            string clause = c.HavingOperator.Value switch
            {
                FilterOperator.Equals => $"{functionalCol} = {paramName}",
                FilterOperator.NotEquals => $"{functionalCol} <> {paramName}",
                FilterOperator.GreaterThan => $"{functionalCol} > {paramName}",
                FilterOperator.GreaterThanOrEqual => $"{functionalCol} >= {paramName}",
                FilterOperator.LessThan => $"{functionalCol} < {paramName}",
                FilterOperator.LessThanOrEqual => $"{functionalCol} <= {paramName}",
                FilterOperator.IsNull => $"{functionalCol} {DbConstants.SqlKeywords.IsNull}",
                FilterOperator.IsNotNull => $"{functionalCol} {DbConstants.SqlKeywords.IsNotNull}",
                _ => ""
            };

            if (!string.IsNullOrEmpty(clause))
            {
                havingClauses.Add(clause);
                if (c.HavingOperator != FilterOperator.IsNull && c.HavingOperator != FilterOperator.IsNotNull)
                    parameters.Add(new SqlParameter(paramName, c.HavingValue));
            }
        }

        if (havingClauses.Count > 0)
        {
            // F3: Support AND/OR logic for HAVING
            string havingLogic = request.HavingLogic?.Trim().ToUpperInvariant() switch
            {
                "OR" => DbConstants.SqlKeywords.Or,
                _ => DbConstants.SqlKeywords.And // Default to AND
            };
            sqlBuilder.Append('\n').Append(DbConstants.SqlKeywords.Having).Append(' ').AppendJoin($"\n   {havingLogic} ", havingClauses);
        }
    }

    private static void ApplyOrderByAndPagination(StringBuilder sqlBuilder, DynamicQueryRequestDto request, string fallbackOrderByColumn, bool isPagination)
    {
        // F2: Build ORDER BY from user-defined columns, fallback to default
        string orderByClause;
        if (request.OrderByColumns?.Count > 0)
        {
            var orderParts = new List<string>();
            foreach (var ob in request.OrderByColumns)
            {
                string safeTable = ob.TableName.Replace("]", "]]");
                string safeCol = ob.ColumnName.Replace("]", "]]");
                string direction = ob.Descending ? "DESC" : "ASC";
                orderParts.Add($"[{safeTable}].[{safeCol}] {direction}");
            }
            orderByClause = string.Join(", ", orderParts);
        }
        else
        {
            orderByClause = fallbackOrderByColumn;
        }

        // F5: When PrintByGroup, ensure group column is FIRST in ORDER BY
        if (request.PrintByGroup && !string.IsNullOrWhiteSpace(request.GroupByColumn))
        {
            string groupColRaw = request.GroupByColumn;
            string groupOrderExpr;
            if (groupColRaw.Contains('.'))
            {
                var parts = groupColRaw.Split('.');
                groupOrderExpr = $"[{parts[0].Replace("]", "]]")}].[{parts[1].Replace("]", "]]")}]";
            }
            else
            {
                groupOrderExpr = $"[{groupColRaw.Replace("]", "]]")}]";
            }

            // Prepend group column if not already at the start
            if (!orderByClause.StartsWith(groupOrderExpr, StringComparison.OrdinalIgnoreCase))
            {
                orderByClause = $"{groupOrderExpr} ASC, {orderByClause}";
            }
        }

        // Always apply ORDER BY for report (sorted output), pagination needs it for OFFSET
        sqlBuilder.Append('\n').Append(DbConstants.SqlKeywords.OrderBy).Append(' ').Append(orderByClause);

        if (isPagination)
        {
            int offset = Math.Max(0, request.PageNumber - 1) * Math.Max(1, request.PageSize);
            sqlBuilder.Append(" \n").Append(DbConstants.SqlKeywords.Offset).Append(' ').Append(offset)
                      .Append(" ROWS \n").Append(DbConstants.SqlKeywords.FetchNext).Append(' ').Append(Math.Max(1, request.PageSize))
                      .Append(' ').Append(DbConstants.SqlKeywords.RowsOnly);
        }
    }

    #endregion
}
