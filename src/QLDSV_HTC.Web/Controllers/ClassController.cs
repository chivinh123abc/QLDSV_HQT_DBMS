using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Models;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Class.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class ClassController(IClassRepository classRepository) : Controller
    {
        // ────────────────────────────────────────────────
        // GET /class  — Trang danh sách lớp
        // ────────────────────────────────────────────────
        [HttpGet]
        [Route(RouteConstants.Class.Index)]
        public async Task<IActionResult> Index()
        {
            // PGV toàn trường: truyền "" để lấy tất cả
            // KHOA: lấy theo mã khoa của tài khoản đang đăng nhập
            var facultyId = await GetCurrentFacultyIdAsync();

            var classes = (await classRepository.GetClassListAsync(facultyId)).ToList();
            var departments = (await classRepository.GetDepartmentsAsync()).ToList();

            var vm = new ClassManagementViewModel
            {
                Classes = classes.Select(c => new ClassViewModel
                {
                    Id = c.ClassId,
                    Name = c.ClassName,
                    Year = c.SchoolYear,
                    Department = c.FacultyId,
                    DepartmentName = c.FacultyName,
                }),
                Departments = departments.Select(d => new DepartmentViewModel
                {
                    Id = d.FacultyId,
                    Name = d.FacultyName,
                }),
                CurrentFacultyId = facultyId,
            };

            return View(vm);
        }

        // ────────────────────────────────────────────────
        // POST /class/add  — Thêm lớp mới
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Class.Add)]
        public async Task<IActionResult> Add([FromBody] ClassInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await classRepository.AddClassAsync(new ClassDto
                {
                    ClassId = input.ClassId.Trim(),
                    ClassName = input.ClassName.Trim(),
                    SchoolYear = input.SchoolYear.Trim(),
                    FacultyId = input.FacultyId.Trim(),
                });
                return Ok(new { success = true, message = "Thêm lớp thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────
        // POST /class/edit  — Cập nhật lớp
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Class.Edit)]
        public async Task<IActionResult> Edit([FromBody] ClassInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await classRepository.UpdateClassAsync(new ClassDto
                {
                    ClassId = input.ClassId.Trim(),
                    ClassName = input.ClassName.Trim(),
                    SchoolYear = input.SchoolYear.Trim(),
                    FacultyId = input.FacultyId.Trim(),
                });
                return Ok(new { success = true, message = "Cập nhật lớp thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────
        // POST /class/delete  — Xoá lớp
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Class.Delete)]
        public async Task<IActionResult> Delete([FromBody] ClassDeleteModel input)
        {
            if (string.IsNullOrWhiteSpace(input?.ClassId))
                return BadRequest(new { success = false, message = "Mã lớp không hợp lệ." });

            try
            {
                await classRepository.DeleteClassAsync(input.ClassId.Trim());
                return Ok(new { success = true, message = "Xoá lớp thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────
        // GET /class/departments  — Lấy danh sách khoa (JSON)
        // ────────────────────────────────────────────────
        [HttpGet]
        [Route(RouteConstants.Class.GetDepartments)]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await classRepository.GetDepartmentsAsync();
            return Ok(departments.Select(d => new { id = d.FacultyId, name = d.FacultyName }));
        }

        // ────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────

        /// <summary>
        /// Lấy mã khoa của người đang đăng nhập.
        /// - PGV toàn trường → trả về ""
        /// - Tài khoản KHOA → query GIANGVIEN lấy MAKHOA theo DB username
        /// </summary>
        private async Task<string> GetCurrentFacultyIdAsync()
        {
            // Nếu là PGV thì không cần lọc theo khoa
            if (User.IsInRole(AppConstants.Groups.PGV))
                return string.Empty;

            // Lấy DB username từ claim (có thể là MAGV hoặc tên login)
            var dbUsername = User.FindFirst(AppConstants.SessionKeys.Username)?.Value
                          ?? User.Identity?.Name
                          ?? string.Empty;

            return await classRepository.GetFacultyByUsernameAsync(dbUsername);
        }
    }
}
