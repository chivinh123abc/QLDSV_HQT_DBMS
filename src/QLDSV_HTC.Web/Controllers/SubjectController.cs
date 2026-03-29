using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Subject.Prefix)]
    public class SubjectController : Controller
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
        [Route(RouteConstants.Subject.Index)]
        public IActionResult Index() => AdminOnlyView();
    }
}
