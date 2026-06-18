using DotNetEnv;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Web.Extensions;

// Bootstrap Configuration (Similar to main.ts setup)
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// CLI Database Setup runner
if (args.Contains("--setup-db"))
{
    try
    {
        await DatabaseInitializer.SetupDatabaseAsync(builder.Environment.ContentRootPath);
        return;
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ Setup Database thất bại: {ex.Message}");
        Console.ResetColor();
        Environment.Exit(1);
    }
}

// Register "App Module" (Services, Providers, Auth)
builder.Services.AddAppModule();

var app = builder.Build();

// Use "App Pipeline" (Middleware, Routing)
app.UseAppPipeline();

// Database Validation (Startup checks)
try
{
    await app.ValidateDatabaseFromScriptsAsync();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("✅ Database check passed! All required Stored Procedures exist from scripts folder.");
    Console.ResetColor();
}
catch (SqlException ex)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"[WARNING] Lỗi kết nối Database (Timeout hoặc SQL Error): {ex.Message}");
    Console.WriteLine("=> Ứng dụng sẽ tiếp tục khởi động, nhưng vui lòng kiểm tra SQL Server.");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[ERROR] Lỗi khởi động: {ex.Message}");
    Console.ResetColor();
}

await app.RunAsync();
