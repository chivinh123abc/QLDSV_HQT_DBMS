using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Home.Prefix)]
    public class HomeController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Home.Index)]
        public IActionResult Index()
        {
            var group = User.FindFirst(AppConstants.SessionKeys.Group)?.Value;
            if (string.Equals(group, AppConstants.Groups.SV, StringComparison.OrdinalIgnoreCase))
            {
                return View("StudentDashboard");
            }
            if (User.Identity?.IsAuthenticated == true)
            {
                return View("AdminDashboard");
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Home.Error)]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Home.NotFound)]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Home.ComingSoon)]
        public IActionResult ComingSoon()
        {
            return View();
        }
    }
}
