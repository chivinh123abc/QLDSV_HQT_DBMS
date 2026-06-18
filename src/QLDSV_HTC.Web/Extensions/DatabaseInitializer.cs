using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.Helpers;

namespace QLDSV_HTC.Web.Extensions
{
    public static class DatabaseInitializer
    {
        public static async Task SetupDatabaseAsync(string contentRootPath)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================");
            Console.WriteLine("🚀 BẮT ĐẦU KHỞI TẠO CƠ SỞ DỮ LIỆU QLDSV_HTC");
            Console.WriteLine("==================================================");
            Console.ResetColor();

            var connectionString = SqlConfigHelper.GetConnectionString();

            // 1. Làm sạch và tạo mới Database (kết nối master)
            try
            {
                var masterConnString = ReplaceDatabaseInConnectionString(connectionString, "master");
                await using var conn = new SqlConnection(masterConnString);
                await conn.OpenAsync();

                var recreateDbSql = @"
                    IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'QLDSV_HTC')
                    BEGIN
                        ALTER DATABASE QLDSV_HTC SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        DROP DATABASE QLDSV_HTC;
                    END
                    CREATE DATABASE QLDSV_HTC;";
                await using var cmd = new SqlCommand(recreateDbSql, conn);
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine("✅ Đã làm sạch và tạo mới Cơ sở dữ liệu QLDSV_HTC.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Lỗi tạo Database QLDSV_HTC: {ex.Message}");
                Console.ResetColor();
                throw;
            }

            // 2. Định nghĩa các đường dẫn tệp tin
            var dbFolderPath = Path.GetFullPath(Path.Combine(contentRootPath, "..", "Database"));
            var tablesFilePath = Path.Combine(dbFolderPath, "Tables", "QLDSV_HTC.sql");
            var scriptsFolderPath = Path.Combine(dbFolderPath, "Scripts");
            var spFolderPath = Path.Combine(dbFolderPath, "StoredProcedures");

            // 3. Thực thi file Tables/QLDSV_HTC.sql
            if (File.Exists(tablesFilePath))
            {
                Console.WriteLine($"\n⚙️ Đang thực thi tệp cấu trúc bảng: {Path.GetFileName(tablesFilePath)}...");
                await ExecuteSqlFileAsync(connectionString, tablesFilePath);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠️ Không tìm thấy tệp Tables tại {tablesFilePath}");
                Console.ResetColor();
            }

            // 4. Thực thi thư mục Scripts (Phân quyền, Indexes, Seeds)
            string? phanQuyenFilePath = null;
            if (Directory.Exists(scriptsFolderPath))
            {
                var scriptFiles = Directory.GetFiles(scriptsFolderPath, "*.sql")
                    .OrderBy(f => f)
                    .ToList();

                Console.WriteLine("\n⚙️ Đang thực thi các tệp Scripts (Indexes, Seeds)...");
                foreach (var file in scriptFiles)
                {
                    if (Path.GetFileName(file).Equals("001-PhanQuyen.sql", StringComparison.OrdinalIgnoreCase))
                    {
                        phanQuyenFilePath = file;
                        continue;
                    }
                    Console.WriteLine($" 👉 Thực thi: {Path.GetFileName(file)}");
                    await ExecuteSqlFileAsync(connectionString, file);
                }
            }

            // 5. Thực thi thư mục StoredProcedures
            if (Directory.Exists(spFolderPath))
            {
                var spFiles = Directory.GetFiles(spFolderPath, "*.sql")
                    .OrderBy(f => f)
                    .ToList();

                Console.WriteLine("\n⚙️ Đang thực thi các tệp Stored Procedures...");
                foreach (var file in spFiles)
                {
                    Console.WriteLine($" 👉 Thực thi: {Path.GetFileName(file)}");
                    await ExecuteSqlFileAsync(connectionString, file);
                }
            }

            // 6. Thực thi tệp Phân quyền sau cùng khi đã đầy đủ stored procedures và vai trò (roles)
            if (phanQuyenFilePath != null && File.Exists(phanQuyenFilePath))
            {
                Console.WriteLine($"\n⚙️ Đang thực thi tệp Phân quyền: {Path.GetFileName(phanQuyenFilePath)}...");
                await ExecuteSqlFileAsync(connectionString, phanQuyenFilePath);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n==================================================");
            Console.WriteLine("✅ HOÀN THÀNH KHỞI TẠO CƠ SỞ DỮ LIỆU THÀNH CÔNG!");
            Console.WriteLine("==================================================");
            Console.ResetColor();
        }

        private static async Task ExecuteSqlFileAsync(string connectionString, string filePath)
        {
            var content = await File.ReadAllTextAsync(filePath);

            // Loại bỏ các lệnh USE [DatabaseName] để tránh lỗi lô lệnh (batch) và đảm bảo chạy đúng DB cấu hình
            content = Regex.Replace(content, @"^\s*USE\s+[^;\r\n]+;?\s*$", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            // Tách các lô lệnh ngăn cách bởi từ khóa GO đứng độc lập trên dòng mới (Regex multiline)
            var batches = Regex.Split(content, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            foreach (var batch in batches)
            {
                var sql = batch.Trim();
                if (string.IsNullOrWhiteSpace(sql)) continue;

                try
                {
                    await using var cmd = new SqlCommand(sql, conn)
                    {
                        CommandTimeout = 180 // Tăng timeout hỗ trợ nạp seeds lớn
                    };
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"   ❌ Lỗi SQL khi chạy lô lệnh trong tệp {Path.GetFileName(filePath)}:");
                    Console.WriteLine($"   Nội dung lệnh lỗi: {(sql.Length > 200 ? sql.Substring(0, 200) + "..." : sql)}");
                    Console.WriteLine($"   Chi tiết lỗi: {ex.Message}");
                    Console.ResetColor();
                    throw;
                }
            }
        }

        private static string ReplaceDatabaseInConnectionString(string connectionString, string newDatabase)
        {
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = newDatabase
            };
            return builder.ConnectionString;
        }
    }
}
