using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Infrastructure.Repositories;
using QLDSV_HTC.Web.Services;
using QLDSV_HTC.Domain.Constants;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using QLDSV_HTC.Web.Extensions;

// Load environment variables from .env file (looked for in one directory up relative to the project root, 
// but DotNetEnv.Load() with search traverse can find it)
Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "../../.env"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. (SOLID - Dependency Injection)
builder.Services.AddControllersWithViews();
builder.Services.AddDevExpressControls();
builder.Services.AddMvc();

builder.Services.AddHttpContextAccessor();

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = AppConstants.Routes.Login;
        options.LogoutPath = AppConstants.Routes.Logout;
        options.AccessDeniedPath = AppConstants.Routes.AccessDenied;
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
    app.UseExceptionHandler(AppConstants.Routes.Error);
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
catch (System.Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
    Console.ResetColor();
    return; 
}

await app.RunAsync();
