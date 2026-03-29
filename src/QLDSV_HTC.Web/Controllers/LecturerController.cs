using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Lecturer.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class LecturerController : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Lecturer.Index)]
        public IActionResult Index() => View();
    }
}
