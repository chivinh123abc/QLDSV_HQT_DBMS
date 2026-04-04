using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Models;
using System.Security.Claims;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Student.Prefix)]
    public class StudentController(
        IStudentRepository studentRepository,
        IClassRepository classRepository,
        IFacultyRepository facultyRepository) : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Student.Index)]
        [Authorize(Roles = AppConstants.Groups.Faculty)]
        public async Task<IActionResult> Index()
        {
            var classes = await classRepository.GetClassListAsync();
            var faculties = await facultyRepository.GetFacultiesAsync();

            var userGroup = User.FindFirstValue(AppConstants.SessionKeys.Group);
            var currentFacultyId = User.FindFirstValue(AppConstants.SessionKeys.FacultyId);

            var viewModel = new StudentManagementViewModel
            {
                Classes = classes.Select(c => new ClassViewModel
                {
                    Id = c.ClassId,
                    Name = c.ClassName,
                    Year = c.SchoolYear,
                    FacultyId = c.FacultyId,
                    FacultyName = c.FacultyName
                }),
                Faculties = faculties.Select(f => new FacultyViewModel
                {
                    Id = f.FacultyId,
                    Name = f.FacultyName
                }),
                CurrentFacultyId = userGroup == AppConstants.Groups.PGV ? string.Empty : (currentFacultyId ?? string.Empty)
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route(RouteConstants.Student.List)]
        [Authorize(Roles = AppConstants.Groups.Faculty)]
        public async Task<IActionResult> List(string classId)
        {
            try
            {
                // In Khoa role, the SP itself ensures only their students are retrieved 
                // Alternatively it can be sent via parameter, but our SP fetches KHOA automatically
                var students = await studentRepository.GetStudentListAsync(classId);
                var viewModels = students.Select(s => new StudentViewModel
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Gender = s.Gender,
                    Dob = s.Dob?.ToString(AppConstants.Formats.IsoDate),
                    Address = s.Address,
                    ClassId = s.ClassId,
                    ClassName = s.ClassName,
                    OnLeave = s.OnLeave,
                    Password = s.Password
                });

                return Json(new { success = true, data = viewModels });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route(RouteConstants.Student.Add)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromBody] StudentInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            try
            {
                var dto = new StudentDto
                {
                    StudentId = model.StudentId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    Address = model.Address,
                    Dob = string.IsNullOrEmpty(model.Dob) ? null : DateTime.Parse(model.Dob, System.Globalization.CultureInfo.InvariantCulture),
                    ClassId = model.ClassId,
                    OnLeave = model.OnLeave,
                    Password = model.Password
                };

                await studentRepository.AddStudentAsync(dto);
                return Json(new { success = true, message = "Thêm sinh viên thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route(RouteConstants.Student.Edit)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] StudentInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            try
            {
                var dto = new StudentDto
                {
                    OldStudentId = model.OldStudentId,
                    StudentId = model.StudentId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    Address = model.Address,
                    Dob = string.IsNullOrEmpty(model.Dob) ? null : DateTime.Parse(model.Dob, System.Globalization.CultureInfo.InvariantCulture),
                    ClassId = model.ClassId,
                    OnLeave = model.OnLeave,
                    Password = string.IsNullOrEmpty(model.Password) ? null : model.Password
                };

                await studentRepository.UpdateStudentAsync(dto);
                return Json(new { success = true, message = "Cập nhật sinh viên thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route(RouteConstants.Student.Delete)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] StudentDeleteModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            if (string.IsNullOrEmpty(model.StudentId))
            {
                return Json(new { success = false, message = "Mã sinh viên không hợp lệ." });
            }

            try
            {
                await studentRepository.DeleteStudentAsync(model.StudentId);
                return Json(new { success = true, message = "Xóa sinh viên thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route(RouteConstants.Student.Schedule)]
        [Authorize(Roles = AppConstants.Groups.SV)]
        public IActionResult Schedule() => View();

        [HttpGet]
        [Route(RouteConstants.Student.Grades)]
        [Authorize(Roles = AppConstants.Groups.SV)]
        public IActionResult Grades() => View();
    }
}
