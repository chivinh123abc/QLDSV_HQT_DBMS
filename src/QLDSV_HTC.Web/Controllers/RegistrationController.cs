using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Registration.Prefix)]
    public class RegistrationController : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Registration.Index)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
