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
        [Route(RouteConstants.Report.LayPhieuDiem)]
        public async Task<IActionResult> LayBangDiem(string maSV)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrEmpty(maSV))
            {
                return BadRequest("Mã sinh viên không được để trống.");
            }

            var studentScores = await repository.LayPhieuDiemAsync(maSV);
            var report = new BangDiemReport(maSV)
            {
                DataSource = studentScores
            };

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            return File(ms.ToArray(), "application/pdf", $"BangDiem_{maSV}.pdf");
        }

        [HttpGet]
        [Route(RouteConstants.Report.LayDanhSachLopTinChi)]
        public async Task<IActionResult> ExportDanhSachLopTinChi(string khoaName, string maKhoa, string nienKhoa, int hocKy)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrEmpty(maKhoa) || string.IsNullOrEmpty(nienKhoa))
            {
                return BadRequest("Thiếu thông tin tra cứu (Mã Khoa, Niên Khóa).");
            }

            var ds = await repository.LayDanhSachLopTinChiAsync(nienKhoa, hocKy, maKhoa);

            var report = new DanhSachLopTinChiReport(khoaName, nienKhoa, hocKy)
            {
                DataSource = ds
            };

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            return File(ms.ToArray(), "application/pdf", $"DS_LTC_{maKhoa}_{nienKhoa}_HK{hocKy}.pdf");
        }
    }
}
