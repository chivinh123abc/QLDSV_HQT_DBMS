using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Faculty.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class FacultyController(IFacultyRepository facultyRepository) : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Faculty.Index)]
        public async Task<IActionResult> GetFaculties()
        {
            var faculties = await facultyRepository.GetFacultiesAsync();
            return Ok(faculties.Select(f => new
            {
                id = f.FacultyId,
                name = f.FacultyName
            }));
        }
    }
}
