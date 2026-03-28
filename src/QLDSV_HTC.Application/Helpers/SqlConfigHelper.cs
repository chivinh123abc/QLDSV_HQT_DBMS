using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Application.Helpers
{
    public static class SqlConfigHelper
    {
        public static string GetConnectionString(string? userId = null, string? password = null)
        {
            var server = Environment.GetEnvironmentVariable(AppConstants.Configs.ServerName) ?? "localhost";
            var database = Environment.GetEnvironmentVariable(AppConstants.Configs.DatabaseName) ?? "QLDSV_HTC";
            var baseUser = Environment.GetEnvironmentVariable(AppConstants.Configs.Username) ?? "sa";
            var basePass = Environment.GetEnvironmentVariable(AppConstants.Configs.Password) ?? "123";
            var trustCert = Environment.GetEnvironmentVariable(AppConstants.Configs.TrustServerCertificate) ?? "True";

            string finalUser = !string.IsNullOrEmpty(userId) ? userId : baseUser;
            string finalPwd = password ?? basePass;

            return $"Data Source={server};Initial Catalog={database};User ID={finalUser};Password={finalPwd};TrustServerCertificate={trustCert}";
        }
    }
}
