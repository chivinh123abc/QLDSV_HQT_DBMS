using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Class.Prefix)]
    public class ClassController : Controller
    {
        private IActionResult PgvOnlyView()
        {
            var group = User.FindFirst(AppConstants.SessionKeys.Group)?.Value;
            if (!string.Equals(group, AppConstants.Groups.PGV, StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(RouteConstants.Home.AccessDeniedPath);
            }
            return View();
        }

        [HttpGet]
        [Route(RouteConstants.Class.Index)]
        public IActionResult Index() => PgvOnlyView();
    }
}
