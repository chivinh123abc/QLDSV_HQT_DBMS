using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QLDSV_HTC.Web.Extensions
{
    public static class DatabaseValidator
    {
        public static async Task ValidateDatabaseFromScriptsAsync(this WebApplication app)
        {
            var dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? ".";
            var dbDatabase = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "QLDSV_HTC";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "123";
            var connectionString = $"Server={dbServer};Database={dbDatabase};User Id={dbUser};Password={dbPassword};TrustServerCertificate=True;";
            
            var spFolderPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "Database", "StoredProcedures"));
            var requiredSps = new List<string>();

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
            using var conn = new SqlConnection(connectionString);
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
            var existingSps = new HashSet<string>();
            var query = "SELECT name FROM sys.procedures";
            
            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();
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
