using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.Interfaces;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public abstract class BaseSqlRepository(IDbConnectionProvider connectionProvider)
    {
        // Tương đương ExecuteNonQuery của ADO.NET
        protected async Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            await using var conn = new SqlConnection(connectionProvider.GetConnectionString());
            await using var cmd = new SqlCommand(commandText, conn);

            cmd.CommandType = commandType;
            if (parameters != null) cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        // Tương đương ExecSqlDataTable
        protected async Task<DataTable> ExecuteQueryAsync(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            await using var conn = new SqlConnection(connectionProvider.GetConnectionString());
            await using var cmd = new SqlCommand(commandText, conn);

            cmd.CommandType = commandType;
            if (parameters != null) cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            var dt = new DataTable();
            dt.Load(reader);

            return dt;
        }
    }
}
