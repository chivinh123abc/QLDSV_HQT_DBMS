using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Application.Interfaces;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Grade.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class GradeController(
        IGradeRepository gradeRepository,
        IReportRepository reportRepository,
        ISubjectRepository subjectRepository) : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Grade.Index)]
        public async Task<IActionResult> Index()
        {
            ViewBag.SchoolYears = await reportRepository.GetSchoolYearsAsync();
            ViewBag.Subjects = await subjectRepository.GetSubjectListAsync();
            return View();
        }

        [HttpGet]
        [Route("/api/grades/students")]
        public async Task<IActionResult> GetGrades(string? year, int semester, string? subjectId, int group, string? studentId, string? studentName)
        {
            var grades = await gradeRepository.GetGradesAsync(year ?? "", semester, subjectId ?? "", group, studentId, studentName);
            return Ok(grades);
        }

        [HttpPost]
        [Route("/api/grades/save")]
        public async Task<IActionResult> SaveGrades([FromBody] IEnumerable<QLDSV_HTC.Application.DTOs.GradeEntryDto> grades)
        {
            if (grades == null || !grades.Any())
            {
                return BadRequest("Không có dữ liệu điểm để lưu.");
            }

            try
            {
                await gradeRepository.UpdateGradesAsync(grades);
                return Ok(new { success = true, message = "Cập nhật điểm thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
