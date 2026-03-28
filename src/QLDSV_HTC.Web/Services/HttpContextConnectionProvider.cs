using QLDSV_HTC.Application.Helpers;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Services
{
    public class HttpContextConnectionProvider(IHttpContextAccessor httpContextAccessor) : IDbConnectionProvider
    {

        public string GetConnectionString()
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                // Retrieve the full ConnectionString from Claims if available
                var userConnectionString = user.FindFirst(AppConstants.SessionKeys.UserConnectionString)?.Value;
                if (!string.IsNullOrEmpty(userConnectionString))
                {
                    return userConnectionString;
                }
            }

            // Fallback for non-authenticated actions
            return SqlConfigHelper.GetConnectionString();
        }
    }
}
