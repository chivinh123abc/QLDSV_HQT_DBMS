using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Infrastructure.Repositories;
using QLDSV_HTC.Web.Services;
using QLDSV_HTC.Domain.Constants;

// Load environment variables from .env file (looked for in one directory up relative to the project root, 
// but DotNetEnv.Load() with search traverse can find it)
Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "../../.env"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. (SOLID - Dependency Injection)
builder.Services.AddControllersWithViews();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(AppConstants.Routes.Error);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Important: must be placed before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
