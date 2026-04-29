using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Reports;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Report.Prefix)]
    [Authorize(Roles = AppConstants.Groups.Faculty)]
    public class ReportController(
        IReportRepository repository,
        IFacultyRepository facultyRepository,
        IClassRepository classRepository,
        ISubjectRepository subjectRepository) : Controller
    {
        private OkObjectResult ConvertDataTableToJson(System.Data.DataTable dt)
        {
            var list = new List<Dictionary<string, object?>>();
            foreach (System.Data.DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object?>();
                foreach (System.Data.DataColumn col in dt.Columns)
                {
                    string mappedKey = ReportConstants.ColumnMap.GetValueOrDefault(col.ColumnName, col.ColumnName);
                    dict[mappedKey] = row[col] == DBNull.Value ? null : row[col];
                }
                list.Add(dict);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route(RouteConstants.Report.Index)]
        public async Task<IActionResult> Index()
        {
            ViewBag.Faculties = await facultyRepository.GetFacultiesAsync();
            ViewBag.Classes = await classRepository.GetClassListAsync();
            ViewBag.Subjects = await subjectRepository.GetSubjectListAsync();
            ViewBag.SchoolYears = await repository.GetSchoolYearsAsync();
            return View();
        }

        [HttpGet]
        [Route(RouteConstants.Report.GetGradesReport)]
        public async Task<IActionResult> GetGradesReport(string studentId, [FromQuery] bool asJson = false)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest("Mã sinh viên không được để trống.");
            }

            var studentScores = await repository.GetGradesReportAsync(studentId);

            if (asJson) return ConvertDataTableToJson(studentScores);

            var report = new BangDiemReport(studentId)
            {
                DataSource = studentScores
            };

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"BangDiem_{studentId}.pdf\"");
            return File(ms.ToArray(), "application/pdf");
        }

        [HttpGet]
        [Route(RouteConstants.Report.GetCreditClassList)]
        public async Task<IActionResult> GetCreditClassList(string facultyName, string facultyId, string schoolYear, int semester, [FromQuery] bool asJson = false)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrEmpty(facultyId) || string.IsNullOrEmpty(schoolYear))
            {
                return BadRequest("Thiếu thông tin tra cứu (Mã Khoa, Niên Khóa).");
            }

            var ds = await repository.GetCreditClassListAsync(schoolYear, semester, facultyId);

            if (asJson) return ConvertDataTableToJson(ds);

            var report = new DanhSachLopTinChiReport(facultyName, schoolYear, semester)
            {
                DataSource = ds
            };

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"DS_LTC_{facultyId}_{schoolYear}_HK{semester}.pdf\"");
            return File(ms.ToArray(), "application/pdf");
        }

        [HttpGet]
        [Route(RouteConstants.Report.GetRegisteredStudentsList)]
        public async Task<IActionResult> GetRegisteredStudentsList(string facultyName, string schoolYear, int semester, string subjectId, string subjectName, int group, [FromQuery] bool asJson = false)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrEmpty(schoolYear) || string.IsNullOrEmpty(subjectId))
            {
                return BadRequest("Thiếu thông tin tra cứu báo cáo (Niên khóa, Mã Môn học).");
            }

            var ds = await repository.GetRegisteredStudentsListAsync(schoolYear, semester, subjectId, group);

            if (asJson) return ConvertDataTableToJson(ds);

            var report = new DanhSachSVDangKyLTCReport(facultyName, schoolYear, semester, subjectName, group)
            {
                DataSource = ds
            };

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"DS_DK_LTC_{subjectId}_{schoolYear}_HK{semester}_NHOM{group}.pdf\"");
            return File(ms.ToArray(), "application/pdf");
        }

        [HttpGet]
        [Route(RouteConstants.Report.GetSubjectGrades)]
        public async Task<IActionResult> GetSubjectGrades(string facultyName, string schoolYear, int semester, string subjectId, string subjectName, int group, [FromQuery] bool asJson = false)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrEmpty(schoolYear) || string.IsNullOrEmpty(subjectId))
            {
                return BadRequest("Thiếu thông tin tra cứu báo cáo (Niên khóa, Mã Môn học).");
            }

            var ds = await repository.GetSubjectGradesAsync(schoolYear, semester, subjectId, group);

            if (asJson) return ConvertDataTableToJson(ds);

            var report = new BangDiemMonHocLTCReport(facultyName, schoolYear, semester, subjectName, group)
            {
                DataSource = ds
            };

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"DIEM_LTC_{subjectId}_{schoolYear}_HK{semester}_NHOM{group}.pdf\"");
            return File(ms.ToArray(), "application/pdf");
        }

        [HttpGet]
        [Route(RouteConstants.Report.GetClassGradesSummary)]
        public async Task<IActionResult> GetClassGradesSummary(string classId, [FromQuery] bool asJson = false)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrEmpty(classId))
            {
                return BadRequest("Thiếu thông tin tra cứu (Mã Lớp).");
            }

            var ds = await repository.GetClassGradesSummaryAsync(classId);

            if (asJson) return ConvertDataTableToJson(ds);

            var report = new BangDiemTongKetLopReport(classId, ds); // DataSource passed via constructor for dynamic layout

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"DIEM_TONGKET_{classId}.pdf\"");
            return File(ms.ToArray(), "application/pdf");
        }
    }
}
