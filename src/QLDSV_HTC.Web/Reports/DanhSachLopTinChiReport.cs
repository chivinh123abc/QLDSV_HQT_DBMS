using DevExpress.XtraReports.UI;
using System.Drawing;

namespace QLDSV_HTC.Web.Reports
{
    public class DanhSachLopTinChiReport : XtraReport
    {
        public DanhSachLopTinChiReport(string khoaName, string nienKhoa, int hocKy)
        {
            PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            Landscape = true;
            Margins = new(50, 50, 50, 50);

            var detailBand = new DetailBand() { HeightF = 30f };
            var reportHeaderBand = new ReportHeaderBand() { HeightF = 130f };
            var pageHeaderBand = new PageHeaderBand() { HeightF = 40f };

            Bands.AddRange([reportHeaderBand, pageHeaderBand, detailBand]);

            // ------- REPORT HEADER -------
            XRLabel lblKhoa = new()
            {
                Text = $"KHOA: {khoaName.ToUpper()}",
                Font = new("Times New Roman", 15, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(1069f, 30f),
                LocationF = new(0f, 0f)
            };

            XRLabel lblTitle = new()
            {
                Text = "DANH SÁCH LỚP TÍN CHỈ",
                Font = new("Times New Roman", 18, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(1069f, 40f),
                LocationF = new(0f, 40f)
            };

            XRLabel lblNienKhoaHocKy = new()
            {
                Text = $"Niên khóa: {nienKhoa}  -  Học kỳ: {hocKy}",
                Font = new("Times New Roman", 13, DevExpress.Drawing.DXFontStyle.Regular),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(1069f, 30f),
                LocationF = new(0f, 80f)
            };

            reportHeaderBand.Controls.AddRange([lblKhoa, lblTitle, lblNienKhoaHocKy]);

            // ------- PAGE HEADER (Headers of the Table) -------
            const DevExpress.XtraPrinting.BorderSide borderSideAll = DevExpress.XtraPrinting.BorderSide.All;
            var headerFont = new DevExpress.Drawing.DXFont("Times New Roman", 12, DevExpress.Drawing.DXFontStyle.Bold);

            XRTable headerTable = new()
            {
                SizeF = new(1069f, 40f),
                Borders = borderSideAll,
                Font = headerFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                BackColor = Color.LightGray
            };

            XRTableRow headerRow = new();

            headerRow.Cells.Add(new XRTableCell() { WidthF = 60f, Text = "STT" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 400f, Text = "TÊN MÔN HỌC" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 80f, Text = "NHÓM" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 250f, Text = "HỌ TÊN GIẢNG VIÊN" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 139f, Text = "SỐ SV TỐI THIỂU" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 140f, Text = "SỐ SV ĐĂNG KÝ" });

            headerTable.Rows.Add(headerRow);
            pageHeaderBand.Controls.Add(headerTable);

            // ------- DETAIL BAND (Data source bindings) -------
            var detailFont = new DevExpress.Drawing.DXFont("Times New Roman", 12, DevExpress.Drawing.DXFontStyle.Regular);

            XRTable detailTable = new()
            {
                SizeF = new(1069f, 30f),
                Borders = borderSideAll,
                Font = detailFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
            };
            XRTableRow detailRow = new();

            // Setup STT auto-increment using sumRecordNumber()
            XRTableCell cellStt = new()
            {
                WidthF = 60f,
                Summary = new() { Func = SummaryFunc.RecordNumber, Running = SummaryRunning.Report }
            };
            cellStt.ExpressionBindings.Add(new("BeforePrint", "Text", "sumRecordNumber()"));

            XRTableCell cellTenMh = new()
            {
                WidthF = 400f,
                Padding = new(10, 5, 0, 0),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
            };
            cellTenMh.ExpressionBindings.Add(new("BeforePrint", "Text", "[TENMH]"));

            XRTableCell cellNhom = new() { WidthF = 80f };
            cellNhom.ExpressionBindings.Add(new("BeforePrint", "Text", "[NHOM]"));

            XRTableCell cellGiangVien = new()
            {
                WidthF = 250f,
                Padding = new(10, 5, 0, 0),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
            };
            cellGiangVien.ExpressionBindings.Add(new("BeforePrint", "Text", "[HOTEN_GV]"));

            XRTableCell cellSvToiThieu = new() { WidthF = 139f };
            cellSvToiThieu.ExpressionBindings.Add(new("BeforePrint", "Text", "[SOSVTOITHIEU]"));

            XRTableCell cellSvDangKy = new() { WidthF = 140f };
            cellSvDangKy.ExpressionBindings.Add(new("BeforePrint", "Text", "[SOSV_DANGKY]"));

            detailRow.Cells.AddRange([cellStt, cellTenMh, cellNhom, cellGiangVien, cellSvToiThieu, cellSvDangKy]);
            detailTable.Rows.Add(detailRow);
            detailBand.Controls.Add(detailTable);
        }
    }
}
