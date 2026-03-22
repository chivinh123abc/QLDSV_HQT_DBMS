using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using QLDSV_HTC.Application.Interfaces;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public abstract class BaseSqlRepository
    {
        private readonly IDbConnectionProvider _connectionProvider;

        protected BaseSqlRepository(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        // Equivalent logic for ADO.NET query execution
        protected async Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                using (var cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    if (parameters != null) cmd.Parameters.AddRange(parameters);

                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // Equivalent logic for fetching DataTables via ExecSqlDataTable
        protected async Task<DataTable> ExecuteQueryAsync(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                using (var cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    if (parameters != null) cmd.Parameters.AddRange(parameters);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        return dt;
                    }
                }
            }
        }
    }
}
