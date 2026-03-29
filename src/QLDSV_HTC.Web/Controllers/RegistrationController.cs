using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Registration.Prefix)]
    [Authorize(Roles = AppConstants.Groups.SV + "," + AppConstants.Groups.PGV)]
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
