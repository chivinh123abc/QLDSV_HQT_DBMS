using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Student.Prefix)]
    public class StudentController : Controller
    {
        private IActionResult StudentOnlyView()
        {
            var group = User.FindFirst(AppConstants.SessionKeys.Group)?.Value;
            if (!string.Equals(group, AppConstants.Groups.SV, StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(RouteConstants.Home.AccessDeniedPath);
            }
            return View();
        }

        private IActionResult AdminOnlyView()
        {
            var group = User.FindFirst(AppConstants.SessionKeys.Group)?.Value;
            if (string.Equals(group, AppConstants.Groups.SV, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(group))
            {
                return Redirect(RouteConstants.Home.AccessDeniedPath);
            }
            return View();
        }

        [HttpGet]
        [Route(RouteConstants.Student.Index)]
        public IActionResult Index() => AdminOnlyView();

        [HttpGet]
        [Route(RouteConstants.Student.Schedule)]
        public IActionResult Schedule() => StudentOnlyView();

        [HttpGet]
        [Route(RouteConstants.Student.Grades)]
        public IActionResult Grades() => StudentOnlyView();
    }
}
