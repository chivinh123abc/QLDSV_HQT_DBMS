using DevExpress.XtraReports.UI;
using System.Drawing;

namespace QLDSV_HTC.Web.Reports
{
    public class DanhSachSVDangKyLTCReport : XtraReport
    {
        public DanhSachSVDangKyLTCReport(string facultyName, string nienKhoa, int hocKy, string subjectName, int nhom)
        {
            PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            Landscape = false;
            Margins = new(50, 50, 50, 50);

            var detailBand = new DetailBand() { HeightF = 30f };
            var reportHeaderBand = new ReportHeaderBand() { HeightF = 180f };
            var pageHeaderBand = new PageHeaderBand() { HeightF = 40f };

            Bands.AddRange([reportHeaderBand, pageHeaderBand, detailBand]);

            // ------- REPORT HEADER -------
            XRLabel lblKHOA = new()
            {
                Text = $"KHOA: {facultyName.ToUpper()}",
                Font = new("Times New Roman", 14, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(727f, 30f),
                LocationF = new(0f, 20f)
            };

            XRLabel lblTitle = new()
            {
                Text = "DANH SÁCH SINH VIÊN ĐĂNG KÝ LỚP TÍN CHỈ",
                Font = new("Times New Roman", 16, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(727f, 40f),
                LocationF = new(0f, 60f)
            };

            XRLabel lblNKHK = new()
            {
                Text = $"Niên khóa: {nienKhoa} - Học kỳ: {hocKy}",
                Font = new("Times New Roman", 13, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(727f, 30f),
                LocationF = new(0f, 100f)
            };

            XRLabel lblMhNhom = new()
            {
                Text = $"Môn học: {subjectName} - Nhóm: {nhom}",
                Font = new("Times New Roman", 13, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(727f, 30f),
                LocationF = new(0f, 130f)
            };

            reportHeaderBand.Controls.AddRange([lblKHOA, lblTitle, lblNKHK, lblMhNhom]);

            // ------- PAGE HEADER -------
            const DevExpress.XtraPrinting.BorderSide borderSideAll = DevExpress.XtraPrinting.BorderSide.All;
            var headerFont = new DevExpress.Drawing.DXFont("Times New Roman", 12, DevExpress.Drawing.DXFontStyle.Bold);

            XRTable headerTable = new()
            {
                SizeF = new(727f, 40f),
                Borders = borderSideAll,
                Font = headerFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                BackColor = Color.LightGray
            };

            XRTableRow headerRow = new();
            headerRow.Cells.Add(new XRTableCell() { WidthF = 50f, Text = "STT" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 150f, Text = "MÃ SV" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 150f, Text = "HỌ" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 100f, Text = "TÊN" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 77f, Text = "PHÁI" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 200f, Text = "MÃ LỚP" });

            headerTable.Rows.Add(headerRow);
            pageHeaderBand.Controls.Add(headerTable);

            // ------- DETAIL BAND -------
            var detailFont = new DevExpress.Drawing.DXFont("Times New Roman", 12, DevExpress.Drawing.DXFontStyle.Regular);

            XRTable detailTable = new()
            {
                SizeF = new(727f, 30f),
                Borders = borderSideAll,
                Font = detailFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
            };
            XRTableRow detailRow = new();

            XRTableCell cellStt = new()
            {
                WidthF = 50f,
                Summary = new() { Func = SummaryFunc.RecordNumber, Running = SummaryRunning.Report }
            };
            cellStt.ExpressionBindings.Add(new("BeforePrint", "Text", "sumRecordNumber()"));

            XRTableCell cellMaSV = new() { WidthF = 150f };
            cellMaSV.ExpressionBindings.Add(new("BeforePrint", "Text", "[MASV]"));

            XRTableCell cellHo = new() { WidthF = 150f, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft, Padding = new(5, 0, 0, 0) };
            cellHo.ExpressionBindings.Add(new("BeforePrint", "Text", "[HO]"));

            XRTableCell cellTen = new() { WidthF = 100f, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft, Padding = new(5, 0, 0, 0) };
            cellTen.ExpressionBindings.Add(new("BeforePrint", "Text", "[TEN]"));

            XRTableCell cellPhai = new() { WidthF = 77f };
            cellPhai.ExpressionBindings.Add(new("BeforePrint", "Text", "[PHAI]"));

            XRTableCell cellMaLop = new() { WidthF = 200f };
            cellMaLop.ExpressionBindings.Add(new("BeforePrint", "Text", "[MALOP]"));

            detailRow.Cells.AddRange([cellStt, cellMaSV, cellHo, cellTen, cellPhai, cellMaLop]);
            detailTable.Rows.Add(detailRow);
            detailBand.Controls.Add(detailTable);
        }
    }
}
