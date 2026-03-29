using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Lecturer.Prefix)]
    public class LecturerController : Controller
    {
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
        [Route(RouteConstants.Lecturer.Index)]
        public IActionResult Index() => AdminOnlyView();
    }
}
