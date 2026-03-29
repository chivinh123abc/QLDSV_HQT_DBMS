using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.Helpers;
using System.Text.RegularExpressions;

namespace QLDSV_HTC.Web.Extensions
{
    public static class DatabaseValidator
    {
        public static async Task ValidateDatabaseFromScriptsAsync(this WebApplication app)
        {
            var connectionString = SqlConfigHelper.GetConnectionString();

            var spFolderPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "Database", "StoredProcedures"));
            List<string> requiredSps = [];

            if (Directory.Exists(spFolderPath))
            {
                foreach (var file in Directory.GetFiles(spFolderPath, "*.sql"))
                {
                    var content = await File.ReadAllTextAsync(file);
                    var match = Regex.Match(content, @"CREATE\s+PROC(?:EDURE)?\s+([a-zA-Z0-9_]+)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        requiredSps.Add(match.Groups[1].Value);
                    }
                }
            }

            if (!requiredSps.Any())
            {
                throw new InvalidOperationException($"Không tìm thấy script Procedure nào trong mục {spFolderPath}");
            }

            await EnsureStoredProceduresExistAsync(connectionString, requiredSps.ToArray());
        }

        private static async Task EnsureStoredProceduresExistAsync(string connectionString, string[] requiredSps)
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            var existingSps = await FetchExistingStoredProceduresAsync(conn);

            var missingSps = requiredSps
                .Where(sp => !existingSps.Contains(sp.ToLower()))
                .ToList();

            if (missingSps.Any())
            {
                ThrowMissingProceduresException(missingSps);
            }
        }

        private static async Task<HashSet<string>> FetchExistingStoredProceduresAsync(SqlConnection conn)
        {
            HashSet<string> existingSps = [];
            var query = "SELECT name FROM sys.procedures";

            await using var cmd = new SqlCommand(query, conn);
            cmd.CommandTimeout = 60; // Extend timeout for slower initial connections
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                existingSps.Add(reader.GetString(0).ToLower());
            }

            return existingSps;
        }

        private static void ThrowMissingProceduresException(List<string> missingSps)
        {
            var errorMessage = $"[CRITICAL ERROR] Database thiếu các Stored Procedure sau:\n" +
                               string.Join("\n", missingSps.Select(sp => $"- {sp}")) +
                               "\n=> Vui lòng chạy các script SQL còn thiếu để bổ sung trước khi khởi động Web!";

            throw new InvalidOperationException(errorMessage);
        }
    }
}
