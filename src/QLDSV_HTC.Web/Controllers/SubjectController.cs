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
    [Route(RouteConstants.Subject.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class SubjectController(ISubjectRepository subjectRepository) : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Subject.Index)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var isPGV = User.IsInRole(AppConstants.Groups.PGV);

            var paginationQuery = new PaginationQuery
            {
                PageNumber = page,
                PageSize = pageSize
            };

            var pagedResult = await subjectRepository.GetPagedSubjectListAsync(paginationQuery);
            var subjects = pagedResult.Items.ToList();

            var vm = new SubjectManagementViewModel
            {
                Subjects = subjects.Select(s => new SubjectViewModel
                {
                    Id = s.SubjectId,
                    Name = s.SubjectName,
                    TheoryHours = s.TheoryHours,
                    PracticeHours = s.PracticeHours
                }),
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize,
                    TotalCount = pagedResult.TotalCount,
                    TotalPages = pagedResult.TotalPages
                }
            };

            // PGV là role DUY NHẤT được sửa / xoá / thêm môn học (do data dùng chung toàn trường)
            ViewData["isPGV"] = isPGV;

            return View(vm);
        }

        [HttpPost]
        [Route(RouteConstants.Subject.Add)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Add([FromBody] SubjectInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await subjectRepository.AddSubjectAsync(new SubjectDto
                {
                    SubjectId = input.SubjectId.Trim(),
                    SubjectName = input.SubjectName.Trim(),
                    TheoryHours = input.TheoryHours!.Value,
                    PracticeHours = input.PracticeHours!.Value,
                });
                return Ok(new { success = true, message = "Thêm môn học thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route(RouteConstants.Subject.Edit)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Edit([FromBody] SubjectInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await subjectRepository.UpdateSubjectAsync(new SubjectDto
                {
                    OldSubjectId = input.OldSubjectId?.Trim(),
                    SubjectId = input.SubjectId.Trim(),
                    SubjectName = input.SubjectName.Trim(),
                    TheoryHours = input.TheoryHours!.Value,
                    PracticeHours = input.PracticeHours!.Value,
                });
                return Ok(new { success = true, message = "Cập nhật môn học thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route(RouteConstants.Subject.Delete)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Delete([FromBody] SubjectDeleteModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            if (string.IsNullOrWhiteSpace(input?.SubjectId))
                return BadRequest(new { success = false, message = "Mã môn học không hợp lệ." });

            try
            {
                await subjectRepository.DeleteSubjectAsync(input.SubjectId.Trim());
                return Ok(new { success = true, message = "Xoá môn học thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
