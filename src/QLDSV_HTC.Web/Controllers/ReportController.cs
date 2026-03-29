using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Reports;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Report.Prefix)]
    public class ReportController(IReportRepository repository) : Controller
    {
        [HttpGet]
        [Route(RouteConstants.Report.Index)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route(RouteConstants.Report.GetGradesReport)]
        public async Task<IActionResult> GetGradesReport(string studentId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest("Mã sinh viên không được để trống.");
            }

            var studentScores = await repository.GetGradesReportAsync(studentId);
            var report = new BangDiemReport(studentId)
            {
                DataSource = studentScores
            };

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            return File(ms.ToArray(), "application/pdf", $"BangDiem_{studentId}.pdf");
        }

        [HttpGet]
        [Route(RouteConstants.Report.GetCreditClassList)]
        public async Task<IActionResult> GetCreditClassList(string facultyName, string facultyId, string schoolYear, int semester)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrEmpty(facultyId) || string.IsNullOrEmpty(schoolYear))
            {
                return BadRequest("Thiếu thông tin tra cứu (Mã Khoa, Niên Khóa).");
            }

            var ds = await repository.GetCreditClassListAsync(schoolYear, semester, facultyId);

            var report = new DanhSachLopTinChiReport(facultyName, schoolYear, semester)
            {
                DataSource = ds
            };

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            return File(ms.ToArray(), "application/pdf", $"DS_LTC_{facultyId}_{schoolYear}_HK{semester}.pdf");
        }
    }
}
