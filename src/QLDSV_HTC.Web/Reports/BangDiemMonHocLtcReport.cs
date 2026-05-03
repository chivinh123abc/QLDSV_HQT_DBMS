using DevExpress.XtraReports.UI;
using System.Drawing;

namespace QLDSV_HTC.Web.Reports
{
    public class BangDiemMonHocLtcReport : XtraReport
    {
        public BangDiemMonHocLtcReport(string facultyName, string nienKhoa, int hocKy, string subjectName, int nhom)
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
                Text = "BẢNG ĐIỂM HẾT MÔN",
                Font = new("Times New Roman", 18, DevExpress.Drawing.DXFontStyle.Bold),
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
            headerRow.Cells.Add(new XRTableCell() { WidthF = 120f, Text = "MÃ SV" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 120f, Text = "HỌ" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 80f, Text = "TÊN" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 85f, Text = "ĐIỂM CC" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 85f, Text = "ĐIỂM GK" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 85f, Text = "ĐIỂM CK" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 102f, Text = "ĐIỂM HM" });

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

            XRTableCell cellMaSV = new() { WidthF = 120f };
            cellMaSV.ExpressionBindings.Add(new("BeforePrint", "Text", "[MASV]"));

            XRTableCell cellHo = new() { WidthF = 120f, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft, Padding = new(5, 0, 0, 0) };
            cellHo.ExpressionBindings.Add(new("BeforePrint", "Text", "[HO]"));

            XRTableCell cellTen = new() { WidthF = 80f, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft, Padding = new(5, 0, 0, 0) };
            cellTen.ExpressionBindings.Add(new("BeforePrint", "Text", "[TEN]"));

            XRTableCell cellDiemCC = new() { WidthF = 85f };
            cellDiemCC.ExpressionBindings.Add(new("BeforePrint", "Text", "[DIEM_CC]"));

            XRTableCell cellDiemGK = new() { WidthF = 85f };
            cellDiemGK.ExpressionBindings.Add(new("BeforePrint", "Text", "[DIEM_GK]"));

            XRTableCell cellDiemCK = new() { WidthF = 85f };
            cellDiemCK.ExpressionBindings.Add(new("BeforePrint", "Text", "[DIEM_CK]"));

            XRTableCell cellDiemHM = new() { WidthF = 102f };
            cellDiemHM.ExpressionBindings.Add(new("BeforePrint", "Text", "[DIEM_HET_MON]"));

            detailRow.Cells.AddRange([cellStt, cellMaSV, cellHo, cellTen, cellDiemCC, cellDiemGK, cellDiemCK, cellDiemHM]);
            detailTable.Rows.Add(detailRow);
            detailBand.Controls.Add(detailTable);
        }
    }
}
