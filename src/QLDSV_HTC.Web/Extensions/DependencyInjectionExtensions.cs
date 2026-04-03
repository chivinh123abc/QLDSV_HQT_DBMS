using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Infrastructure.Repositories;
using QLDSV_HTC.Web.Services;
using QLDSV_HTC.Domain.Constants;
using DevExpress.AspNetCore;

namespace QLDSV_HTC.Web.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddAppModule(this IServiceCollection services)
        {
            // Controller & Global Filters
            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddDevExpressControls();
            services.AddMvc();
            services.AddHttpContextAccessor();

            // Authentication Configuration
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = RouteConstants.Account.LoginPath;
                    options.LogoutPath = RouteConstants.Account.LogoutPath;
                    options.AccessDeniedPath = RouteConstants.Home.AccessDeniedPath;
                });

            // Register Custom Repositories & Services
            services.AddScoped<IDbConnectionProvider, HttpContextConnectionProvider>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddSingleton<ISidebarService, SidebarService>();

            return services;
        }
    }
}
