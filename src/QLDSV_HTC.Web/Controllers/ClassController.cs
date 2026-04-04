using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Class.Prefix)]
    [Authorize(Roles = AppConstants.Groups.PGV)]
    public class ClassController : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Class.Index)]
        public IActionResult Index() => View();
    }
}
