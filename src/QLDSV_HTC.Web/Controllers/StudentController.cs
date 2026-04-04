using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Student.Prefix)]
    public class StudentController : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Student.Index)]
        [Authorize(Roles = AppConstants.Groups.Faculty)]
        public IActionResult Index() => View();

        [HttpGet]
        [Route(RouteConstants.Student.Schedule)]
        [Authorize(Roles = AppConstants.Groups.SV)]
        public IActionResult Schedule() => View();

        [HttpGet]
        [Route(RouteConstants.Student.Grades)]
        [Authorize(Roles = AppConstants.Groups.SV)]
        public IActionResult Grades() => View();
    }
}
