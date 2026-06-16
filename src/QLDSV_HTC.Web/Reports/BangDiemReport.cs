using DevExpress.XtraReports.UI;
using System.Drawing;

namespace QLDSV_HTC.Web.Reports
{
    public class BangDiemReport : XtraReport
    {
        public BangDiemReport(string maSV)
        {
            PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            Landscape = false; // Portrait size
            Margins = new(50, 50, 50, 50);

            var detailBand = new DetailBand() { HeightF = 30f };
            var reportHeaderBand = new ReportHeaderBand() { HeightF = 130f };
            var groupHeaderBand = new GroupHeaderBand()
            {
                HeightF = 75f,
                RepeatEveryPage = true
            };
            groupHeaderBand.GroupFields.Add(new GroupField("NIENKHOA"));
            groupHeaderBand.GroupFields.Add(new GroupField("HOCKY"));

            Bands.AddRange([reportHeaderBand, groupHeaderBand, detailBand]);

            // ------- REPORT HEADER -------
            XRLabel lblTitle = new()
            {
                Text = "PHIẾU ĐIỂM SINH VIÊN",
                Font = new("Times New Roman", 18, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(727f, 40f),
                LocationF = new(0f, 40f)
            };

            XRLabel lblMaSV = new()
            {
                Text = $"Mã sinh viên: {maSV}",
                Font = new("Times New Roman", 13, DevExpress.Drawing.DXFontStyle.Regular),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(727f, 30f),
                LocationF = new(0f, 80f)
            };

            reportHeaderBand.Controls.AddRange([lblTitle, lblMaSV]);

            // ------- GROUP HEADER (Semester Title and Table Headers) -------
            XRLabel lblSemester = new()
            {
                Font = new("Times New Roman", 12, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft,
                SizeF = new(727f, 30f),
                LocationF = new(0f, 5f)
            };
            lblSemester.ExpressionBindings.Add(new("BeforePrint", "Text", "'Học kỳ ' + [HOCKY] + ' - Niên khóa: ' + [NIENKHOA]"));
            groupHeaderBand.Controls.Add(lblSemester);

            const DevExpress.XtraPrinting.BorderSide borderSideAll = DevExpress.XtraPrinting.BorderSide.All;
            var headerFont = new DevExpress.Drawing.DXFont("Times New Roman", 12, DevExpress.Drawing.DXFontStyle.Bold);

            XRTable headerTable = new()
            {
                SizeF = new(727f, 40f),
                LocationF = new(0f, 35f),
                Borders = borderSideAll,
                Font = headerFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                BackColor = Color.LightGray
            };

            XRTableRow headerRow = new();

            headerRow.Cells.Add(new XRTableCell() { WidthF = 50f, Text = "STT" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 250f, Text = "TÊN MÔN HỌC" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 85f, Text = "ĐIỂM CC" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 85f, Text = "ĐIỂM GK" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 85f, Text = "ĐIỂM CK" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 172f, Text = "ĐIỂM TỔNG" });

            headerTable.Rows.Add(headerRow);
            groupHeaderBand.Controls.Add(headerTable);

            // ------- DETAIL BAND (Data source bindings) -------
            var detailFont = new DevExpress.Drawing.DXFont("Times New Roman", 12, DevExpress.Drawing.DXFontStyle.Regular);

            XRTable detailTable = new()
            {
                SizeF = new(727f, 30f),
                Borders = borderSideAll,
                Font = detailFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
            };
            XRTableRow detailRow = new();

            // Setup STT auto-increment using sumRecordNumber() reset by Group
            XRTableCell cellStt = new()
            {
                WidthF = 50f,
                Summary = new() { Func = SummaryFunc.RecordNumber, Running = SummaryRunning.Group }
            };
            cellStt.ExpressionBindings.Add(new("BeforePrint", "Text", "sumRecordNumber()"));

            XRTableCell cellTenMh = new()
            {
                WidthF = 250f,
                Padding = new(10, 5, 0, 0),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
            };
            cellTenMh.ExpressionBindings.Add(new("BeforePrint", "Text", "[TENMH]"));

            XRTableCell cellDiemCC = new() { WidthF = 85f };
            cellDiemCC.ExpressionBindings.Add(new("BeforePrint", "Text", "[DIEM_CC]"));

            XRTableCell cellDiemGK = new() { WidthF = 85f };
            cellDiemGK.ExpressionBindings.Add(new("BeforePrint", "Text", "[DIEM_GK]"));

            XRTableCell cellDiemCK = new() { WidthF = 85f };
            cellDiemCK.ExpressionBindings.Add(new("BeforePrint", "Text", "[DIEM_CK]"));

            XRTableCell cellDiem = new() { WidthF = 172f };
            cellDiem.ExpressionBindings.Add(new("BeforePrint", "Text", "[DIEM]"));

            detailRow.Cells.AddRange([cellStt, cellTenMh, cellDiemCC, cellDiemGK, cellDiemCK, cellDiem]);
            detailTable.Rows.Add(detailRow);
            detailBand.Controls.Add(detailTable);
        }
    }
}
