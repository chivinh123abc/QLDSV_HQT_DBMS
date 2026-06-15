using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Application.DTOs;

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
        [Route(RouteConstants.Grade.GetGrades)]
        public async Task<IActionResult> GetGrades(string? year, int semester, string? subjectId, int group, string? studentId, string? studentName)
        {
            var grades = await gradeRepository.GetGradesAsync(year ?? "", semester, subjectId ?? "", group, studentId, studentName);
            return Ok(grades);
        }

        [HttpPost]
        [Route(RouteConstants.Grade.SaveGrades)]
        public async Task<IActionResult> SaveGrades([FromBody] IEnumerable<GradeEntryDto> grades)
        {
            if (grades?.Any() != true)
            {
                return BadRequest("Không có dữ liệu điểm để lưu.");
            }

            foreach (var grade in grades)
            {
                if ((grade.AttendanceGrade.HasValue && (grade.AttendanceGrade < 0 || grade.AttendanceGrade > 10)) ||
                    (grade.MidtermGrade.HasValue && (grade.MidtermGrade < 0 || grade.MidtermGrade > 10)) ||
                    (grade.FinalGrade.HasValue && (grade.FinalGrade < 0 || grade.FinalGrade > 10)))
                {
                    return BadRequest(new { success = false, message = "Điểm của sinh viên phải nằm trong khoảng từ 0 đến 10." });
                }
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
