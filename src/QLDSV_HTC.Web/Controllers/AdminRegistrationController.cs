using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.Helpers;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Models;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.AdminRegistration.Prefix)]
    [Authorize(Roles = AppConstants.Groups.PGV)]
    public class AdminRegistrationController(
        IStudentRepository studentRepository,
        IRegistrationRepository registrationRepository) : Controller
    {
        // ── GET /admin-registration ───────────────────────────────────
        [HttpGet]
        [Route(RouteConstants.AdminRegistration.Index)]
        public IActionResult Index()
        {
            var vm = new AdminRegistrationViewModel
            {
                Years = DateTimeHelper.GetSchoolYears(),
                Semesters = DateTimeHelper.GetSemesters(),
                CurrentYear = DateTimeHelper.GetCurrentSchoolYear(),
                CurrentSemester = DateTimeHelper.GetCurrentSemester(),
            };

            return View(vm);
        }

        // ── GET /admin-registration/search-students?q=... → JSON ─────
        [HttpGet(RouteConstants.AdminRegistration.SearchStudents)]
        public async Task<IActionResult> SearchStudents(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
                return Ok(new { success = true, data = Array.Empty<object>() });

            var students = await studentRepository.GetStudentListAsync();
            var keyword = q.Trim().ToLower();
            var result = students
                .Where(s =>
                    s.StudentId.ToLower().Contains(keyword) ||
                    $"{s.FirstName} {s.LastName}".ToLower().Contains(keyword))
                .Take(20)
                .Select(s => new
                {
                    id = s.StudentId,
                    name = $"{s.FirstName} {s.LastName}".Trim(),
                    classId = s.ClassId
                });

            return Ok(new { success = true, data = result });
        }

        // ── GET /admin-registration/filter?year=...&semester=...&studentId=... → JSON ─────
        [HttpGet(RouteConstants.AdminRegistration.Filter)]
        public async Task<IActionResult> Filter(string year, int? semester, string studentId)
        {
            if (string.IsNullOrWhiteSpace(year) || !semester.HasValue || semester.Value is < 1 or > 3)
                return BadRequest(new { success = false, message = "Niên khóa hoặc Học kỳ không hợp lệ." });

            if (string.IsNullOrWhiteSpace(studentId))
                return BadRequest(new { success = false, message = "Vui lòng chọn sinh viên." });

            try
            {
                var classes = await registrationRepository.GetAvailableClassesAsync(year, semester.Value, studentId.Trim());
                var result = classes.Select(c => new
                {
                    id = c.Id,
                    subjectCode = c.SubjectId,
                    subjectName = c.SubjectName,
                    group = c.Group,
                    lecturer = c.LecturerName,
                    registered = c.RegisteredCount,
                    minStudents = c.MinStudents,
                    isRegistered = c.IsRegistered,
                    hasGrades = c.HasGrades
                });
                return Ok(new { success = true, data = result });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ── POST /admin-registration/register ─────────────────────────
        [HttpPost(RouteConstants.AdminRegistration.Register)]
        public async Task<IActionResult> Register([FromBody] AdminRegistrationInputModel input)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(input?.StudentId))
                return BadRequest(new { success = false, message = "Dữ liệu đăng ký không hợp lệ." });

            if (input.CreditClassId <= 0)
                return BadRequest(new { success = false, message = "Mã lớp tín chỉ không hợp lệ." });

            try
            {
                await registrationRepository.RegisterAsync(input.StudentId.Trim(), input.CreditClassId);
                return Ok(new { success = true, message = "Đăng ký thành công!" });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ── POST /admin-registration/unregister ───────────────────────
        [HttpPost(RouteConstants.AdminRegistration.Unregister)]
        public async Task<IActionResult> Unregister([FromBody] AdminRegistrationInputModel input)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(input?.StudentId))
                return BadRequest(new { success = false, message = "Dữ liệu hủy đăng ký không hợp lệ." });

            if (input.CreditClassId <= 0)
                return BadRequest(new { success = false, message = "Mã lớp tín chỉ không hợp lệ." });

            try
            {
                await registrationRepository.UnregisterAsync(input.StudentId.Trim(), input.CreditClassId);
                return Ok(new { success = true, message = "Hủy đăng ký thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class AdminRegistrationInputModel
    {
        [Required(ErrorMessage = "Mã sinh viên là bắt buộc")]
        public string StudentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã lớp tín chỉ là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Mã lớp tín chỉ không hợp lệ")]
        public int CreditClassId { get; set; }
    }
}
