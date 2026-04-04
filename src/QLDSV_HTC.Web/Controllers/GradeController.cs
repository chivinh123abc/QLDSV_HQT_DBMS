using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Grade.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class GradeController : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Grade.Index)]
        public IActionResult Index() => View();
    }
}
