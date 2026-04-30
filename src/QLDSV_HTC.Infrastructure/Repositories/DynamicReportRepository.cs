using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Enums;

namespace QLDSV_HTC.Infrastructure.Repositories;

public class DynamicReportRepository(IDbConnectionProvider connectionProvider)
    : BaseSqlRepository(connectionProvider), IDynamicReportRepository
{
    public async Task<IEnumerable<string>> GetAllowedTablesAsync()
    {
        const string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE IN ('BASE TABLE', 'VIEW') AND TABLE_NAME NOT LIKE 'SPRING_%' AND TABLE_NAME NOT LIKE 'sys%' ORDER BY TABLE_NAME";
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

    private static (string Sql, SqlParameter[] Parameters) BuildDynamicQuery(DynamicQueryRequestDto request, bool isPagination)
    {
        var parameters = new List<SqlParameter>();
        string safeTableName = request.TableName.Replace("]", "]]");

        // 1. Build Select Clause
        string columns = "*";
        var groupByColumns = new List<string>(); // Columns that need to be in GROUP BY
        bool hasAggregates = request.AdvancedSelectColumns?.Any(c =>
            !string.IsNullOrEmpty(c.Aggregate) && !c.Aggregate.Equals("None", StringComparison.OrdinalIgnoreCase)) == true;

        if (request.AdvancedSelectColumns?.Count > 0)
        {
            var uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var selectedColumns = new List<string>();
            foreach (var c in request.AdvancedSelectColumns)
            {
                string colName = c.ColumnName.Replace("]", "]]");
                string safeColTable = c.TableName.Replace("]", "]]");
                string qualifiedCol = $"[{safeColTable}].[{colName}]";

                bool isRaw = string.IsNullOrEmpty(c.Aggregate) || c.Aggregate.Equals("None", StringComparison.OrdinalIgnoreCase);
                string aggFunc = isRaw ? "NONE" : c.Aggregate.ToUpper();
                
                string aggAlias;
                if (!string.IsNullOrWhiteSpace(c.AliasName))
                {
                    aggAlias = c.AliasName.Replace("]", "]]");
                }
                else
                {
                    aggAlias = isRaw ? colName : $"{aggFunc}_{colName}";
                }

                // Unique key includes table, column, function, and alias to allow multiple different aggregations
                string uniqueKey = $"{qualifiedCol}_{aggFunc}_{aggAlias}";
                if (!uniqueKeys.Add(uniqueKey)) continue;

                if (isRaw)
                {
                    selectedColumns.Add($"{qualifiedCol} AS [{aggAlias}]");
                    if (hasAggregates)
                        groupByColumns.Add(qualifiedCol);
                }
                else
                {
                    selectedColumns.Add($"{aggFunc}({qualifiedCol}) AS [{aggAlias}]");
                }
            }
            columns = string.Join(", ", selectedColumns);
        }
        else if (request.SelectColumns?.Count > 0)
        {
            columns = string.Join(", ", request.SelectColumns.Select(c => $"[{safeTableName}].[{c.Replace("]", "]]")}]"));
        }

        // 2. Build FROM & JOINs
        var sqlBuilder = new System.Text.StringBuilder();
        sqlBuilder.Append($"SELECT {columns} FROM [dbo].[{safeTableName}]");

        if (request.Joins?.Count > 0)
        {
            var joinedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { safeTableName };

            foreach (var join in request.Joins)
            {
                string joinSafeTable = join.JoinTable.Replace("]", "]]");

                // Find a valid join condition from any already joined table to this new table
                string? joinCondition = null;
                foreach (var existingTable in joinedTables)
                {
                    joinCondition = QLDSV_HTC.Domain.Constants.TableRelationRegistry.GetJoinCondition(existingTable, joinSafeTable);
                    if (joinCondition == null)
                        joinCondition = QLDSV_HTC.Domain.Constants.TableRelationRegistry.GetJoinCondition(joinSafeTable, existingTable);

                    if (joinCondition != null) break;
                }

                if (joinCondition != null)
                {
                    sqlBuilder.Append($" {join.JoinType} [dbo].[{joinSafeTable}] ON {joinCondition}");
                    joinedTables.Add(joinSafeTable);
                }
            }
        }

        // 3. Build Where Clause
        if (request.Filters?.Count > 0)
        {
            var whereClauses = new List<string>();
            for (int i = 0; i < request.Filters.Count; i++)
            {
                var filter = request.Filters[i];
                string paramName = $"@p{i}";
                string clause = "";

                string safeColName = filter.ColumnName.Replace("]", "]]");
                string safeTableForFilter = string.IsNullOrEmpty(filter.TableName) ? safeTableName : filter.TableName.Replace("]", "]]");
                string fullyQualifiedCol = $"[{safeTableForFilter}].[{safeColName}]";

                switch (filter.Operator)
                {
                    case FilterOperator.Equals:
                        clause = $"{fullyQualifiedCol} = {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                    case FilterOperator.NotEquals:
                        clause = $"{fullyQualifiedCol} <> {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                    case FilterOperator.Contains:
                        clause = $"CAST({fullyQualifiedCol} AS NVARCHAR(MAX)) LIKE {paramName}";
                        parameters.Add(new SqlParameter(paramName, $"%{filter.Value}%"));
                        break;
                    case FilterOperator.StartsWith:
                        clause = $"CAST({fullyQualifiedCol} AS NVARCHAR(MAX)) LIKE {paramName}";
                        parameters.Add(new SqlParameter(paramName, $"{filter.Value}%"));
                        break;
                    case FilterOperator.EndsWith:
                        clause = $"CAST({fullyQualifiedCol} AS NVARCHAR(MAX)) LIKE {paramName}";
                        parameters.Add(new SqlParameter(paramName, $"%{filter.Value}"));
                        break;
                    case FilterOperator.GreaterThan:
                        clause = $"{fullyQualifiedCol} > {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                    case FilterOperator.GreaterThanOrEqual:
                        clause = $"{fullyQualifiedCol} >= {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                    case FilterOperator.LessThan:
                        clause = $"{fullyQualifiedCol} < {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                    case FilterOperator.LessThanOrEqual:
                        clause = $"{fullyQualifiedCol} <= {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                }

                if (!string.IsNullOrEmpty(clause))
                {
                    whereClauses.Add(clause);
                }
            }

            if (whereClauses.Count > 0)
            {
                sqlBuilder.Append(" WHERE ").Append(string.Join(" AND ", whereClauses));
            }
        }

        // 4. Build GROUP BY Clause (if aggregates are used)
        if (groupByColumns.Count > 0)
        {
            sqlBuilder.Append(" GROUP BY ").Append(string.Join(", ", groupByColumns));

            // 4.5 Build HAVING Clause (Integrated into AdvancedSelectColumns)
            var havingClauses = new List<string>();
            int hpIndex = 0;
            foreach (var c in request.AdvancedSelectColumns)
            {
                if (!c.HavingOperator.HasValue || string.IsNullOrEmpty(c.HavingValue)) continue;
                if (string.IsNullOrEmpty(c.Aggregate) || c.Aggregate.Equals("None", StringComparison.OrdinalIgnoreCase)) continue;

                string paramName = $"@hp{hpIndex++}";
                string safeColName = c.ColumnName.Replace("]", "]]");
                string safeTable = c.TableName.Replace("]", "]]");
                string aggFunc = c.Aggregate.ToUpper();
                string functionalCol = $"{aggFunc}([{safeTable}].[{safeColName}])";

                string clause = c.HavingOperator.Value switch
                {
                    FilterOperator.Equals => $"{functionalCol} = {paramName}",
                    FilterOperator.NotEquals => $"{functionalCol} <> {paramName}",
                    FilterOperator.GreaterThan => $"{functionalCol} > {paramName}",
                    FilterOperator.GreaterThanOrEqual => $"{functionalCol} >= {paramName}",
                    FilterOperator.LessThan => $"{functionalCol} < {paramName}",
                    FilterOperator.LessThanOrEqual => $"{functionalCol} <= {paramName}",
                    FilterOperator.Contains => $"CAST({functionalCol} AS NVARCHAR(MAX)) LIKE {paramName}",
                    FilterOperator.StartsWith => $"CAST({functionalCol} AS NVARCHAR(MAX)) LIKE {paramName}",
                    FilterOperator.EndsWith => $"CAST({functionalCol} AS NVARCHAR(MAX)) LIKE {paramName}",
                    _ => ""
                };

                if (!string.IsNullOrEmpty(clause))
                {
                    havingClauses.Add(clause);
                    object val = c.HavingValue;
                    if (c.HavingOperator == FilterOperator.Contains) val = $"%{c.HavingValue}%";
                    else if (c.HavingOperator == FilterOperator.StartsWith) val = $"{c.HavingValue}%";
                    else if (c.HavingOperator == FilterOperator.EndsWith) val = $"%{c.HavingValue}";

                    parameters.Add(new SqlParameter(paramName, val));
                }
            }

            if (havingClauses.Count > 0)
            {
                sqlBuilder.Append(" HAVING ").Append(string.Join(" AND ", havingClauses));
            }
        }

        // 5. Build Pagination Clause
        if (isPagination)
        {
            // Note: SQL Server OFFSET/FETCH requires an ORDER BY clause. 
            // We use ORDER BY 1 (the first column in SELECT) as a generic fallback.
            int offset = Math.Max(0, request.PageNumber - 1) * Math.Max(1, request.PageSize);
            sqlBuilder.Append($" ORDER BY 1 OFFSET {offset} ROWS FETCH NEXT {Math.Max(1, request.PageSize)} ROWS ONLY");
        }

        return (sqlBuilder.ToString(), parameters.ToArray());
    }

    public async Task<DataTable> GetPreviewDataAsync(DynamicQueryRequestDto request)
    {
        var (sql, parameters) = BuildDynamicQuery(request, isPagination: true);
        return await ExecuteQueryAsync(sql, CommandType.Text, parameters);
    }

    public async Task<DataTable> GetReportDataAsync(DynamicQueryRequestDto request)
    {
        var (sql, parameters) = BuildDynamicQuery(request, isPagination: false);
        return await ExecuteQueryAsync(sql, CommandType.Text, parameters);
    }
}
