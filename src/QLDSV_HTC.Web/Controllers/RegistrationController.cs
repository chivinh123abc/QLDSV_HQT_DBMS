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
    [Route(RouteConstants.Registration.Prefix)]
    [Authorize(Roles = AppConstants.Groups.SV)]
    public class RegistrationController(
        IStudentRepository studentRepository,
        IRegistrationRepository registrationRepository) : Controller
    {
        // ── GET /registration ──────────────────────────────────────────
        [HttpGet]
        [Route(RouteConstants.Registration.Index)]
        public async Task<IActionResult> Index()
        {
            var studentId = GetStudentId();
            var student = await studentRepository.GetStudentByIdAsync(studentId);

            var vm = new CourseRegistrationViewModel
            {
                StudentId = student?.StudentId ?? studentId,
                StudentFullName = student != null ? $"{student.FirstName} {student.LastName}".Trim() : string.Empty,
                StudentClass = student?.ClassId ?? string.Empty,
                Years = DateTimeHelper.GetSchoolYears(),
                Semesters = DateTimeHelper.GetSemesters(),
                CurrentYear = DateTimeHelper.GetCurrentSchoolYear(),
                CurrentSemester = DateTimeHelper.GetCurrentSemester(),
                SelectedYear = DateTimeHelper.GetCurrentSchoolYear(),
                SelectedSemester = DateTimeHelper.GetCurrentSemester(),
            };

            return View(vm);
        }

        // ── GET /registration/filter?year=...&semester=... → JSON ──────
        [HttpGet(RouteConstants.Registration.Filter)]
        public async Task<IActionResult> Filter(string year, int? semester)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu lọc không hợp lệ." });

            if (string.IsNullOrWhiteSpace(year) || !semester.HasValue || semester.Value is < 1 or > 3)
                return BadRequest(new { success = false, message = "Niên khóa hoặc Học kỳ không hợp lệ." });

            var studentId = GetStudentId();

            try
            {
                var classes = await registrationRepository.GetAvailableClassesAsync(year, semester.Value, studentId);
                var result = classes.Select(c => new
                {
                    id = c.Id,
                    subjectCode = c.SubjectId,
                    subjectName = c.SubjectName,
                    group = c.Group,
                    lecturer = c.LecturerName,
                    registered = c.RegisteredCount,
                    minStudents = c.MinStudents,
                    isRegistered = c.IsRegistered
                });
                return Ok(new { success = true, data = result });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ── POST /registration/register ────────────────────────────────
        [HttpPost(RouteConstants.Registration.Register)]
        public async Task<IActionResult> Register([FromBody] RegistrationInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu đăng ký không hợp lệ." });

            if (input?.CreditClassId <= 0)
                return BadRequest(new { success = false, message = "Mã lớp tín chỉ không hợp lệ." });

            var studentId = GetStudentId();
            try
            {
                await registrationRepository.RegisterAsync(studentId, input!.CreditClassId);
                return Ok(new { success = true, message = "Đăng ký thành công!" });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ── POST /registration/unregister ──────────────────────────────
        [HttpPost(RouteConstants.Registration.Unregister)]
        public async Task<IActionResult> Unregister([FromBody] RegistrationInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu hủy đăng ký không hợp lệ." });

            if (input?.CreditClassId <= 0)
                return BadRequest(new { success = false, message = "Mã lớp tín chỉ không hợp lệ." });

            var studentId = GetStudentId();
            try
            {
                await registrationRepository.UnregisterAsync(studentId, input!.CreditClassId);
                return Ok(new { success = true, message = "Hủy đăng ký thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ── Helpers ────────────────────────────────────────────────────
        private string GetStudentId() =>
            User.FindFirst(AppConstants.SessionKeys.Username)?.Value?.Trim() ?? string.Empty;
    }

    public class RegistrationInputModel
    {
        [Required(ErrorMessage = "Mã lớp tín chỉ là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Mã lớp tín chỉ không hợp lệ")]
        public int CreditClassId { get; set; }
    }
}
