using DotNetEnv;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Infrastructure.Data;
using QLDSV_HTC.Infrastructure.Repositories;
// Load environment variables from .env file (looked for in one directory up relative to the project root, 
// but DotNetEnv.Load() with search traverse can find it)
Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "../../.env"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. (SOLID - Dependency Injection)
builder.Services.AddControllersWithViews();

// Register Custom Services
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
