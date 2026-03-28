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
            Margins = new DevExpress.Drawing.DXMargins(50, 50, 50, 50);

            var detailBand = new DetailBand() { HeightF = 30f };
            var reportHeaderBand = new ReportHeaderBand() { HeightF = 130f };
            var pageHeaderBand = new PageHeaderBand() { HeightF = 40f };

            Bands.AddRange(new Band[] { reportHeaderBand, pageHeaderBand, detailBand });

            // ------- REPORT HEADER -------
            XRLabel lblTitle = new XRLabel
            {
                Text = "PHIẾU ĐIỂM SINH VIÊN",
                Font = new DevExpress.Drawing.DXFont("Times New Roman", 18, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new SizeF(727f, 40f),
                LocationF = new PointF(0f, 40f)
            };

            XRLabel lblMaSV = new XRLabel
            {
                Text = $"Mã sinh viên: {maSV}",
                Font = new DevExpress.Drawing.DXFont("Times New Roman", 13, DevExpress.Drawing.DXFontStyle.Regular),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new SizeF(727f, 30f),
                LocationF = new PointF(0f, 80f)
            };

            reportHeaderBand.Controls.AddRange(new XRControl[] { lblTitle, lblMaSV });

            // ------- PAGE HEADER (Headers of the Table) -------
            var borderSideAll = DevExpress.XtraPrinting.BorderSide.All;
            var headerFont = new DevExpress.Drawing.DXFont("Times New Roman", 12, DevExpress.Drawing.DXFontStyle.Bold);

            XRTable headerTable = new XRTable()
            {
                SizeF = new SizeF(727f, 40f),
                Borders = borderSideAll,
                Font = headerFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                BackColor = Color.LightGray
            };

            XRTableRow headerRow = new XRTableRow();

            headerRow.Cells.Add(new XRTableCell() { WidthF = 60f, Text = "STT" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 467f, Text = "TÊN MÔN HỌC" });
            headerRow.Cells.Add(new XRTableCell() { WidthF = 200f, Text = "ĐIỂM" });

            headerTable.Rows.Add(headerRow);
            pageHeaderBand.Controls.Add(headerTable);

            // ------- DETAIL BAND (Data source bindings) -------
            var detailFont = new DevExpress.Drawing.DXFont("Times New Roman", 12, DevExpress.Drawing.DXFontStyle.Regular);

            XRTable detailTable = new XRTable()
            {
                SizeF = new SizeF(727f, 30f),
                Borders = borderSideAll,
                Font = detailFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
            };
            XRTableRow detailRow = new XRTableRow();

            // Setup STT auto-increment using sumRecordNumber()
            XRTableCell cellStt = new XRTableCell() { WidthF = 60f };
            cellStt.Summary = new XRSummary { Func = SummaryFunc.RecordNumber, Running = SummaryRunning.Report };
            cellStt.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "sumRecordNumber()"));

            XRTableCell cellTenMh = new XRTableCell()
            {
                WidthF = 467f,
                Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 5, 0, 0),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
            };
            cellTenMh.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[TENMH]"));

            XRTableCell cellDiem = new XRTableCell() { WidthF = 200f };
            cellDiem.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[DIEM]"));

            detailRow.Cells.AddRange(new XRTableCell[] { cellStt, cellTenMh, cellDiem });
            detailTable.Rows.Add(detailRow);
            detailBand.Controls.Add(detailTable);
        }
    }
}
