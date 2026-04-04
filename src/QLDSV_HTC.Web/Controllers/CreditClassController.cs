using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.CreditClass.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class CreditClassController : Controller
    {
        [HttpGet]
        [Route(RouteConstants.CreditClass.Index)]
        public IActionResult Index() => View();
    }
}
