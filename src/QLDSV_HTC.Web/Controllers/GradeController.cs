using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Infrastructure.Helpers;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Grade.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class GradeController(
        IGradeRepository gradeRepository,
        IReportRepository reportRepository,
        ISubjectRepository subjectRepository,
        IClassRepository classRepository,
        ICreditClassRepository creditClassRepository) : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Grade.Index)]
        public async Task<IActionResult> Index()
        {
            ViewBag.SchoolYears = await reportRepository.GetSchoolYearsAsync();
            ViewBag.Subjects = await subjectRepository.GetSubjectListAsync();

            // Lấy danh sách lớp — SP tự lọc theo khoa của GV nếu đăng nhập KHOA
            var classes = await classRepository.GetClassListAsync();
            var currentYear = DateTime.Now.Year;
            ViewBag.Classes = classes.Where(cls =>
            {
                if (string.IsNullOrEmpty(cls.SchoolYear)) return false;
                var parts = cls.SchoolYear.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out int endYear))
                {
                    return endYear >= currentYear;
                }
                return false;
            });

            // Truyền thông tin nhóm/khoa xuống view
            ViewBag.CurrentGroup = User.FindFirst(AppConstants.SessionKeys.Group)?.Value ?? "";
            ViewBag.CurrentFacultyId = User.FindFirst(AppConstants.SessionKeys.FacultyId)?.Value ?? "";

            return View();
        }

        /// <summary>
        /// API: Lấy danh sách lớp tín chỉ theo năm và học kỳ (để populate dropdown chọn LTC).
        /// Nếu đăng nhập KHOA, tự động lọc theo khoa của GV đó.
        /// </summary>
        [HttpGet]
        [Route(RouteConstants.Grade.GetCreditClasses)]
        public async Task<IActionResult> GetCreditClasses(string? year, int? semester)
        {
            // Lấy FacultyId nếu là KHOA để lọc theo khoa
            string? facultyId = null;
            var currentGroup = User.FindFirst(AppConstants.SessionKeys.Group)?.Value ?? "";
            if (currentGroup == AppConstants.Groups.KHOA)
            {
                facultyId = User.FindFirst(AppConstants.SessionKeys.FacultyId)?.Value;
            }

            var creditClasses = await creditClassRepository.GetListAsync(year, semester, facultyId);

            // Trả về dữ liệu gọn cho dropdown
            var result = creditClasses
                .Where(c => !c.IsCancelled)
                .Select(c => new
                {
                    id = c.Id,
                    label = $"[{c.Id}] {c.SubjectName} — Nhóm {c.Group} ({c.LecturerName})",
                    subjectId = c.SubjectId,
                    subjectName = c.SubjectName,
                    group = c.Group,
                    lecturerId = c.LecturerId,
                    lecturerName = c.LecturerName,
                    facultyId = c.FacultyId,
                    year = c.Year,
                    semester = c.Semester,
                    registeredCount = c.RegisteredCount
                });

            return Ok(result);
        }

        /// <summary>
        /// API: Lấy danh sách sinh viên và điểm của một lớp tín chỉ cụ thể.
        /// Nhận creditClassId (MALTC) hoặc các filter cũ (year/semester/subjectId/group).
        /// </summary>
        [HttpGet]
        [Route(RouteConstants.Grade.GetGrades)]
        public async Task<IActionResult> GetGrades(
            string? year, int semester, string? subjectId, int group,
            string? studentId, string? studentName, string? classId)
        {
            // Nếu đăng nhập KHOA, tự động lọc sinh viên thuộc khoa của GV đó
            string? facultyId = null;
            var currentGroup = User.FindFirst(AppConstants.SessionKeys.Group)?.Value ?? "";
            if (currentGroup == AppConstants.Groups.KHOA)
            {
                facultyId = User.FindFirst(AppConstants.SessionKeys.FacultyId)?.Value;
            }

            var grades = await gradeRepository.GetGradesAsync(
                year ?? "", semester, subjectId ?? "", group,
                studentId, studentName, classId, facultyId);

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
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = SqlErrorHelper.GetFriendlyMessage(ex) });
            }
        }
    }
}
