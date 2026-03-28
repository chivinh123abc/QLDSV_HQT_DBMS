using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Infrastructure.Repositories;
using QLDSV_HTC.Web.Services;
using QLDSV_HTC.Domain.Constants;
using DevExpress.AspNetCore;
using QLDSV_HTC.Web.Extensions;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
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
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
    Console.ResetColor();
    return;
}

await app.RunAsync();
