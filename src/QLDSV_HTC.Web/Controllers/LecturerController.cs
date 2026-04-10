using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Domain.Models;
using QLDSV_HTC.Web.Models;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Lecturer.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class LecturerController(
        ILecturerRepository lecturerRepository,
        IFacultyRepository facultyRepository) : Controller
    {
        // ────────────────────────────────────────────────
        // GET /lecturer  — Trang danh sách giảng viên
        // ────────────────────────────────────────────────
        [HttpGet]
        [Route(RouteConstants.Lecturer.Index)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

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

            var pagedResult = await lecturerRepository.GetPagedLecturerListAsync(paginationQuery);
            var lecturers = pagedResult.Items.ToList();
            var faculties = (await facultyRepository.GetFacultiesAsync()).ToList();

            var vm = new LecturerManagementViewModel
            {
                Lecturers = lecturers.Select(l => new LecturerViewModel
                {
                    Id = l.LecturerId,
                    FirstName = l.FirstName,
                    LastName = l.LastName,
                    Degree = l.Degree,
                    Rank = l.AcademicRank,
                    Specialty = l.Specialty,
                    FacultyId = l.FacultyId,
                    FacultyName = l.FacultyName,
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
                    TotalPages = pagedResult.TotalPages,
                }
            };

            return View(vm);
        }

        // ────────────────────────────────────────────────
        // POST /lecturer/add  — Thêm giảng viên mới
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Lecturer.Add)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Add([FromBody] LecturerInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await lecturerRepository.AddLecturerAsync(new LecturerDto
                {
                    LecturerId = input.LecturerId.Trim(),
                    FacultyId = input.FacultyId.Trim(),
                    FirstName = input.FirstName.Trim(),
                    LastName = input.LastName.Trim(),
                    Degree = input.Degree?.Trim(),
                    AcademicRank = input.AcademicRank?.Trim(),
                    Specialty = input.Specialty?.Trim(),
                });
                return Ok(new { success = true, message = "Thêm giảng viên thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────
        // POST /lecturer/edit  — Cập nhật giảng viên
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Lecturer.Edit)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Edit([FromBody] LecturerInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await lecturerRepository.UpdateLecturerAsync(new LecturerDto
                {
                    OldLecturerId = input.OldLecturerId?.Trim(),
                    LecturerId = input.LecturerId.Trim(),
                    FacultyId = input.FacultyId.Trim(),
                    FirstName = input.FirstName.Trim(),
                    LastName = input.LastName.Trim(),
                    Degree = input.Degree?.Trim(),
                    AcademicRank = input.AcademicRank?.Trim(),
                    Specialty = input.Specialty?.Trim(),
                });
                return Ok(new { success = true, message = "Cập nhật giảng viên thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────
        // POST /lecturer/delete  — Xoá giảng viên
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Lecturer.Delete)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Delete([FromBody] LecturerDeleteModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            if (string.IsNullOrWhiteSpace(input?.LecturerId))
                return BadRequest(new { success = false, message = "Mã giảng viên không hợp lệ." });

            try
            {
                await lecturerRepository.DeleteLecturerAsync(input.LecturerId.Trim());
                return Ok(new { success = true, message = "Xoá giảng viên thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
