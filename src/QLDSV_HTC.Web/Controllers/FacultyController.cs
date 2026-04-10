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
    [Route(RouteConstants.Faculty.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class FacultyController(IFacultyRepository facultyRepository) : Controller
    {
        // ────────────────────────────────────────────────
        // GET /faculty  — Trang danh sách khoa
        // ────────────────────────────────────────────────
        [HttpGet]
        [Route(RouteConstants.Faculty.Index)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var paginationQuery = new PaginationQuery
            {
                PageNumber = page,
                PageSize = pageSize
            };

            var pagedResult = await facultyRepository.GetPagedFacultyListAsync(paginationQuery);
            var faculties = pagedResult.Items.ToList();

            var vm = new FacultyManagementViewModel
            {
                Faculties = faculties.Select(f => new FacultyViewModel
                {
                    Id = f.FacultyId,
                    Name = f.FacultyName,
                    LecturerCount = f.LecturerCount
                }),
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
        // POST /faculty/add  — Thêm khoa mới
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Faculty.Add)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Add([FromBody] FacultyInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await facultyRepository.AddFacultyAsync(new FacultyDto
                {
                    FacultyId = input.FacultyId.Trim(),
                    FacultyName = input.FacultyName.Trim(),
                });
                return Ok(new { success = true, message = "Thêm khoa thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────
        // POST /faculty/edit  — Cập nhật khoa
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Faculty.Edit)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Edit([FromBody] FacultyInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await facultyRepository.UpdateFacultyAsync(new FacultyDto
                {
                    OldFacultyId = input.OldFacultyId?.Trim(),
                    FacultyId = input.FacultyId.Trim(),
                    FacultyName = input.FacultyName.Trim(),
                });
                return Ok(new { success = true, message = "Cập nhật khoa thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────
        // POST /faculty/delete  — Xoá khoa
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Faculty.Delete)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Delete([FromBody] FacultyDeleteModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            if (string.IsNullOrWhiteSpace(input?.FacultyId))
                return BadRequest(new { success = false, message = "Mã khoa không hợp lệ." });

            try
            {
                await facultyRepository.DeleteFacultyAsync(input.FacultyId.Trim());
                return Ok(new { success = true, message = "Xoá khoa thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
