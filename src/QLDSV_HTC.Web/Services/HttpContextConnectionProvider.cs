using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Services
{
    public class HttpContextConnectionProvider : IDbConnectionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextConnectionProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetConnectionString()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                // Retrieve the full ConnectionString from Claims if available
                var userConnectionString = user.FindFirst("UserConnectionString")?.Value;
                if (!string.IsNullOrEmpty(userConnectionString))
                {
                    return userConnectionString;
                }
            }

            // Fallback for non-authenticated actions
            var server = Environment.GetEnvironmentVariable("DB_SERVER") ?? AppConstants.Configs.ServerName;
            var database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? AppConstants.Configs.DatabaseName;
            var trustCert = Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERTIFICATE") ?? "True";
            var userId = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
            var pwd = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";

            return $"Server={server};Database={database};User Id={userId};Password={pwd};TrustServerCertificate={trustCert}";
        }
    }
}
