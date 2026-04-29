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
        const string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";
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

    private (string Sql, SqlParameter[] Parameters) BuildDynamicQuery(DynamicQueryRequestDto request, bool isPagination)
    {
        var parameters = new List<SqlParameter>();

        // Build Select Clause
        string columns = "*";
        if (request.SelectColumns != null && request.SelectColumns.Count > 0)
        {
            columns = string.Join(", ", request.SelectColumns.Select(c => $"[{c}]"));
        }

        // Build base SQL
        string sql = $"SELECT {columns} FROM [dbo].[{request.TableName}]";

        // Build Where Clause
        if (request.Filters != null && request.Filters.Count > 0)
        {
            var whereClauses = new List<string>();
            for (int i = 0; i < request.Filters.Count; i++)
            {
                var filter = request.Filters[i];
                string paramName = $"@p{i}";
                string clause = "";

                switch (filter.Operator)
                {
                    case FilterOperator.Equals:
                        clause = $"[{filter.ColumnName}] = {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                    case FilterOperator.NotEquals:
                        clause = $"[{filter.ColumnName}] <> {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                    case FilterOperator.Contains:
                        clause = $"[{filter.ColumnName}] LIKE {paramName}";
                        parameters.Add(new SqlParameter(paramName, $"%{filter.Value}%"));
                        break;
                    case FilterOperator.StartsWith:
                        clause = $"[{filter.ColumnName}] LIKE {paramName}";
                        parameters.Add(new SqlParameter(paramName, $"{filter.Value}%"));
                        break;
                    case FilterOperator.EndsWith:
                        clause = $"[{filter.ColumnName}] LIKE {paramName}";
                        parameters.Add(new SqlParameter(paramName, $"%{filter.Value}"));
                        break;
                    case FilterOperator.GreaterThan:
                        clause = $"[{filter.ColumnName}] > {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                    case FilterOperator.GreaterThanOrEqual:
                        clause = $"[{filter.ColumnName}] >= {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                    case FilterOperator.LessThan:
                        clause = $"[{filter.ColumnName}] < {paramName}";
                        parameters.Add(new SqlParameter(paramName, filter.Value));
                        break;
                    case FilterOperator.LessThanOrEqual:
                        clause = $"[{filter.ColumnName}] <= {paramName}";
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
                sql += " WHERE " + string.Join(" AND ", whereClauses);
            }
        }

        // Build Pagination Clause
        if (isPagination)
        {
            // Note: SQL Server OFFSET/FETCH requires an ORDER BY clause. 
            // We use ORDER BY 1 (the first column in SELECT) as a generic fallback.
            sql += $" ORDER BY 1 OFFSET {(request.PageNumber - 1) * request.PageSize} ROWS FETCH NEXT {request.PageSize} ROWS ONLY";
        }

        return (sql, parameters.ToArray());
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
