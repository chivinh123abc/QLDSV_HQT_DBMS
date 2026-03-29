using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Subject.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class SubjectController : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Subject.Index)]
        public IActionResult Index() => View();
    }
}
