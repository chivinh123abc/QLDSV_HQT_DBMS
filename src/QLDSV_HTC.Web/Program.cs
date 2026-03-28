using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Infrastructure.Repositories;
using QLDSV_HTC.Web.Services;
using QLDSV_HTC.Domain.Constants;
using DevExpress.AspNetCore;
using QLDSV_HTC.Web.Extensions;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddDevExpressControls();
builder.Services.AddMvc();

builder.Services.AddHttpContextAccessor();

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = RouteConstants.Account.LoginPath;
        options.LogoutPath = RouteConstants.Account.LogoutPath;
        options.AccessDeniedPath = RouteConstants.Home.AccessDeniedPath;
    });

// Register Custom Services
// We are migrating from IDbConnectionFactory to IDbConnectionProvider (Scoped per request)
builder.Services.AddScoped<IDbConnectionProvider, HttpContextConnectionProvider>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(RouteConstants.Home.ErrorPath);
}

// Automatically handle 404 and other status codes
app.UseStatusCodePagesWithReExecute(RouteConstants.Home.NotFoundPath);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDevExpressControls();

app.UseRouting();

app.UseAuthentication(); // Important: must be placed before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

try
{
    await app.ValidateDatabaseFromScriptsAsync();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("✅ Database check passed! All required Stored Procedures exist from scripts folder.");
    Console.ResetColor();
}
catch (Microsoft.Data.SqlClient.SqlException ex)
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
