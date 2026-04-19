using DevExpress.XtraReports.UI;
using System.Data;
using System.Drawing;

namespace QLDSV_HTC.Web.Reports
{
    public class BangDiemTongKetLopReport : XtraReport
    {
        public BangDiemTongKetLopReport(string tenLop, DataTable dt)
        {
            PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A3; // A3 since there can be many subjects
            Landscape = true;
            Margins = new(30, 30, 50, 50);

            var detailBand = new DetailBand() { HeightF = 30f };
            var reportHeaderBand = new ReportHeaderBand() { HeightF = 120f };
            var pageHeaderBand = new PageHeaderBand() { HeightF = 60f };

            Bands.AddRange([reportHeaderBand, pageHeaderBand, detailBand]);

            // ------- REPORT HEADER -------
            XRLabel lblTitle = new()
            {
                Text = "BẢNG ĐIỂM TỔNG KẾT CUỐI KHÓA",
                Font = new("Times New Roman", 18, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(1050f, 40f), // A3 Landscape width approximate
                LocationF = new(0f, 20f)
            };

            XRLabel lblLop = new()
            {
                Text = $"LỚP: {tenLop.ToUpper()}",
                Font = new("Times New Roman", 14, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(1050f, 30f),
                LocationF = new(0f, 70f)
            };

            reportHeaderBand.Controls.AddRange([lblTitle, lblLop]);

            // ------- PAGE HEADER DYNAMIC -------
            const DevExpress.XtraPrinting.BorderSide borderSideAll = DevExpress.XtraPrinting.BorderSide.All;
            var headerFont = new DevExpress.Drawing.DXFont("Times New Roman", 11, DevExpress.Drawing.DXFontStyle.Bold);

            XRTable headerTable = new()
            {
                SizeF = new(1050f, 60f),
                Borders = borderSideAll,
                Font = headerFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                BackColor = Color.LightGray
            };

            XRTableRow headerRow = new();

            const float totalWidth = 1050f;
            const float sttWidth = 50f;
            const float hoTenWidth = 220f;
            int subjectColumnsCount = dt.Columns.Count - 1; // excluding MASV_HOTEN
            float subjectWidth = subjectColumnsCount > 0 ? (totalWidth - sttWidth - hoTenWidth) / subjectColumnsCount : 100f;

            // STT Column
            headerRow.Cells.Add(new XRTableCell() { WidthF = sttWidth, Text = "STT" });

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName == "MASV_HOTEN")
                {
                    headerRow.Cells.Add(new XRTableCell() { WidthF = hoTenWidth, Text = "MÃ SV - HỌ TÊN" });
                }
                else
                {
                    headerRow.Cells.Add(new XRTableCell() { WidthF = subjectWidth, Text = col.ColumnName });
                }
            }

            headerTable.Rows.Add(headerRow);
            pageHeaderBand.Controls.Add(headerTable);

            // ------- DETAIL BAND DYNAMIC -------
            var detailFont = new DevExpress.Drawing.DXFont("Times New Roman", 11, DevExpress.Drawing.DXFontStyle.Regular);

            XRTable detailTable = new()
            {
                SizeF = new(1050f, 30f),
                Borders = borderSideAll,
                Font = detailFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
            };
            XRTableRow detailRow = new();

            // STT Expression
            XRTableCell cellStt = new()
            {
                WidthF = sttWidth,
                Summary = new() { Func = SummaryFunc.RecordNumber, Running = SummaryRunning.Report }
            };
            cellStt.ExpressionBindings.Add(new("BeforePrint", "Text", "sumRecordNumber()"));
            detailRow.Cells.Add(cellStt);

            foreach (DataColumn col in dt.Columns)
            {
                XRTableCell detailCell = new();
                if (col.ColumnName == "MASV_HOTEN")
                {
                    detailCell.WidthF = hoTenWidth;
                    detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                    detailCell.Padding = new(5, 0, 0, 0);
                    detailCell.ExpressionBindings.Add(new("BeforePrint", "Text", $"[{col.ColumnName}]"));
                }
                else
                {
                    detailCell.WidthF = subjectWidth;
                    // Format diem limit to 1 decimal place or display null safely
                    detailCell.ExpressionBindings.Add(new("BeforePrint", "Text", $"[{col.ColumnName}]"));
                    detailCell.TextFormatString = "{0:n1}";
                }
                detailRow.Cells.Add(detailCell);
            }

            detailTable.Rows.Add(detailRow);
            detailBand.Controls.Add(detailTable);

            // Set data source directly since we need it for constructor layout anyway
            this.DataSource = dt;
        }
    }
}
