using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Models;
using QLDSV_HTC.Domain.Models;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Class.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class ClassController(IClassRepository classRepository, IFacultyRepository facultyRepository) : Controller
    {
        // ────────────────────────────────────────────────
        // GET /class  — Trang danh sách lớp
        // ────────────────────────────────────────────────
        [HttpGet]
        [Route(RouteConstants.Class.Index)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            // Lấy mã khoa từ Session
            // Tuy nhiên, nếu là PGV thì ta xem như không có "Khoa hiện tại" để view hiển thị "Toàn trường"
            var facultyId = string.Empty;
            if (!User.IsInRole(AppConstants.Groups.PGV))
            {
                facultyId = User.FindFirst(AppConstants.SessionKeys.FacultyId)?.Value ?? string.Empty;
            }

            var paginationQuery = new PaginationQuery
            {
                PageNumber = page,
                PageSize = pageSize
            };

            var pagedResult = await classRepository.GetPagedClassListAsync(paginationQuery);
            var classes = pagedResult.Items.ToList();
            var faculties = (await facultyRepository.GetFacultiesAsync()).ToList();

            var vm = new ClassManagementViewModel
            {
                Classes = classes.Select(c => new ClassViewModel
                {
                    Id = c.ClassId,
                    Name = c.ClassName,
                    Year = c.SchoolYear,
                    FacultyId = c.FacultyId,
                    FacultyName = c.FacultyName,
                    StudentCount = c.StudentCount
                }),
                Faculties = faculties.Select(f => new FacultyViewModel
                {
                    Id = f.FacultyId,
                    Name = f.FacultyName,
                }),
                CurrentFacultyId = facultyId,
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize,
                    TotalCount = pagedResult.TotalCount,
                    TotalPages = pagedResult.TotalPages
                }
            };

            return View(vm);
        }

        // ────────────────────────────────────────────────
        // POST /class/add  — Thêm lớp mới
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Class.Add)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
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
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Edit([FromBody] ClassInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await classRepository.UpdateClassAsync(new ClassDto
                {
                    OldClassId = input.OldClassId?.Trim(),
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
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Delete([FromBody] ClassDeleteModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

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
    }
}
