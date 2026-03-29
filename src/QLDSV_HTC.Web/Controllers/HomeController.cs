using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Extensions;

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
            if (User.IsStudent())
            {
                return View(RouteConstants.Home.StudentDashboard);
            }
            if (User.Identity?.IsAuthenticated == true)
            {
                return View(RouteConstants.Home.AdminDashboard);
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

        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Home.AccessDenied)]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
