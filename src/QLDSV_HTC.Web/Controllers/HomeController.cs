using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Controllers
{
    [Route(RouteConstants.Home.Prefix)]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Home.Index)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route(RouteConstants.Home.Error)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
