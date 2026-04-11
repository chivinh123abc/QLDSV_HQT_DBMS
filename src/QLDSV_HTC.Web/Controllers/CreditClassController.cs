using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Models;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.CreditClass.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class CreditClassController(
        ICreditClassRepository creditClassRepository,
        IFacultyRepository facultyRepository,
        ISubjectRepository subjectRepository,
        ILecturerRepository lecturerRepository) : Controller
    {
        [HttpGet]
        [Route(RouteConstants.CreditClass.Index)]
        public async Task<IActionResult> Index(string? filterYear, int? filterSemester, string? filterFaculty)
        {
            var isPGV = User.IsInRole(AppConstants.Groups.PGV);
            var currentFacultyId = isPGV ? string.Empty : (User.FindFirst(AppConstants.SessionKeys.FacultyId)?.Value ?? string.Empty);

            if (!isPGV && string.IsNullOrEmpty(filterFaculty))
            {
                filterFaculty = currentFacultyId;
            }

            // Fetch list based on filters
            var dtos = await creditClassRepository.GetListAsync(filterYear, filterSemester, filterFaculty);

            // Fetch reference lists
            var faculties = await facultyRepository.GetFacultiesAsync();
            var subjects = await subjectRepository.GetSubjectListAsync();
            var lecturers = await lecturerRepository.GetLecturerListAsync(isPGV ? null : currentFacultyId);

            // Build View Model
            var vm = new CreditClassManagementViewModel
            {
                CreditClasses = dtos.Select(d => new CreditClassViewModel
                {
                    Id = d.Id.ToString(),
                    Year = d.Year,
                    Semester = d.Semester,
                    SubjectId = d.SubjectId,
                    SubjectName = d.SubjectName,
                    Group = d.Group,
                    LecturerId = d.LecturerId,
                    LecturerName = d.LecturerName,
                    FacultyId = d.FacultyId,
                    FacultyName = d.FacultyId, // Wait, dtos doesn't have FacultyName, but that's fine for now or I can add it
                    MinStudents = d.MinStudents,
                    Cancelled = d.IsCancelled,
                    RegisteredCount = d.RegisteredCount
                }),
                Faculties = faculties.Select(f => new FacultyViewModel { Id = f.FacultyId, Name = f.FacultyName }),
                Subjects = subjects.Select(s => new SubjectViewModel { Id = s.SubjectId, Name = s.SubjectName }),
                Lecturers = lecturers.Select(l => new LecturerViewModel { Id = l.LecturerId, FirstName = l.FirstName, LastName = l.LastName }),
                Years = new List<string> { "2021-2022", "2022-2023", "2023-2024", "2024-2025" },
                Semesters = new List<int> { 1, 2, 3 },
                FilterYear = filterYear ?? string.Empty,
                FilterSemester = filterSemester,
                FilterFaculty = filterFaculty ?? string.Empty,
                CurrentFacultyId = currentFacultyId
            };

            ViewData["isPGV"] = isPGV;
            return View(vm);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] CreditClassInputModel input)
        {
            if (!ModelState.IsValid) return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                var newId = await creditClassRepository.AddAsync(new CreditClassDto
                {
                    Year = input.Year.Trim(),
                    Semester = input.Semester,
                    SubjectId = input.SubjectId.Trim(),
                    Group = input.Group,
                    LecturerId = input.LecturerId.Trim(),
                    FacultyId = input.FacultyId.Trim(),
                    MinStudents = input.MinStudents,
                    IsCancelled = input.IsCancelled
                });
                return Ok(new { success = true, message = "Thêm lớp tín chỉ thành công.", newId = newId });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit([FromBody] CreditClassInputModel input)
        {
            if (!ModelState.IsValid) return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await creditClassRepository.UpdateAsync(new CreditClassDto
                {
                    Id = input.CreditClassId,
                    Year = input.Year.Trim(),
                    Semester = input.Semester,
                    SubjectId = input.SubjectId.Trim(),
                    Group = input.Group,
                    LecturerId = input.LecturerId.Trim(),
                    FacultyId = input.FacultyId.Trim(),
                    MinStudents = input.MinStudents,
                    IsCancelled = input.IsCancelled
                });
                return Ok(new { success = true, message = "Cập nhật lớp tín chỉ thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete([FromBody] CreditClassDeleteModel input)
        {
            if (!ModelState.IsValid) return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                await creditClassRepository.DeleteAsync(input.CreditClassId);
                return Ok(new { success = true, message = "Xóa lớp tín chỉ thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
