using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QLDSV_HTC.Application.Interfaces;

namespace QLDSV_HTC.Infrastructure.Data
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection(string? username = null, string? password = null)
        {
            var server = Environment.GetEnvironmentVariable("DB_SERVER") ?? ".";
            var database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "QLDSV_HTC";
            var baseUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
            var basePass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";
            var trustCert = Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERTIFICATE") ?? "True";

            // If username/password provided (e.g., from login), use those. Otherwise use base config.
            string userId = !string.IsNullOrEmpty(username) ? username : baseUser;
            string pwd = !string.IsNullOrEmpty(password) ? password : basePass;

            var connectionString = $"Server={server};Database={database};User Id={userId};Password={pwd};TrustServerCertificate={trustCert}";

            return new SqlConnection(connectionString);
        }
    }
}
