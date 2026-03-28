using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Web.Reports;

namespace QLDSV_HTC.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _repository;

        public ReportController(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> ExportPdf(string masv)
        {
            if (string.IsNullOrEmpty(masv))
            {
                return BadRequest("Mã sinh viên không được để trống.");
            }

            var report = new BangDiemReport();

            var studentScores = await _repository.LayDiemSinhVienAsync(masv);

            report.DataSource = studentScores;

            using (var ms = new MemoryStream())
            {
                await report.ExportToPdfAsync(ms);
                return File(ms.ToArray(), "application/pdf", $"BangDiem_{masv}.pdf");
            }
        }
    }
}
