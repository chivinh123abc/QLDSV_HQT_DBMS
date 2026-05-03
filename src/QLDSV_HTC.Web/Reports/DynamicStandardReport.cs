using DevExpress.XtraReports.UI;
using System.Data;
using System.Drawing;
using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Web.Reports
{
    public class DynamicStandardReport : XtraReport
    {
        const int LandscapeThreshold = 6;
        public DynamicStandardReport(DataTable dt, DynamicQueryRequestDto request)
        {
            if (dt.Columns.Count > LandscapeThreshold)
            {
                PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A3;
                Landscape = true;
            }
            else
            {
                PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
                Landscape = false;
            }

            Margins = new(30, 30, 50, 50);

            var detailBand = new DetailBand() { HeightF = 30f };
            var reportHeaderBand = new ReportHeaderBand() { HeightF = 100f };
            var pageHeaderBand = new PageHeaderBand() { HeightF = 40f };

            Bands.AddRange([reportHeaderBand, pageHeaderBand, detailBand]);

            float totalWidth = PageWidth - Margins.Left - Margins.Right;

            // ------- REPORT HEADER -------
            XRLabel lblTitle = new()
            {
                Text = !string.IsNullOrWhiteSpace(request.ReportTitle) 
                    ? request.ReportTitle.ToUpper()
                    : $"BÁO CÁO TÙY CHỈNH: {request.TableName.ToUpper()}",
                Font = new("Arial", 18, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new(totalWidth, 40f),
                LocationF = new(0f, 20f)
            };

            reportHeaderBand.Controls.Add(lblTitle);

            // ------- PAGE HEADER DYNAMIC -------
            const DevExpress.XtraPrinting.BorderSide borderSideAll = DevExpress.XtraPrinting.BorderSide.All;
            var headerFont = new DevExpress.Drawing.DXFont("Arial", 11, DevExpress.Drawing.DXFontStyle.Bold);

            XRTable headerTable = new()
            {
                SizeF = new(totalWidth, 40f),
                Borders = borderSideAll,
                Font = headerFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                BackColor = Color.LightGray
            };

            XRTableRow headerRow = new();

            const float sttWidth = 50f;
            int dataColumnsCount = dt.Columns.Count;
            float columnWidth = dataColumnsCount > 0 ? (totalWidth - sttWidth) / dataColumnsCount : 100f;

            // STT Column
            headerRow.Cells.Add(new XRTableCell() { WidthF = sttWidth, Text = "STT" });

            foreach (DataColumn col in dt.Columns)
            {
                headerRow.Cells.Add(new XRTableCell() { WidthF = columnWidth, Text = col.ColumnName });
            }

            headerTable.Rows.Add(headerRow);
            pageHeaderBand.Controls.Add(headerTable);

            // ------- DETAIL BAND DYNAMIC -------
            var detailFont = new DevExpress.Drawing.DXFont("Arial", 11, DevExpress.Drawing.DXFontStyle.Regular);

            XRTable detailTable = new()
            {
                SizeF = new(totalWidth, 30f),
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
                XRTableCell detailCell = new()
                {
                    WidthF = columnWidth,
                    Padding = new(5, 5, 0, 0)
                };

                if (col.DataType == typeof(int) || col.DataType == typeof(double) || col.DataType == typeof(decimal) || col.DataType == typeof(float))
                {
                    detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
                }
                else if (col.DataType == typeof(DateTime))
                {
                    detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                    detailCell.TextFormatString = "{0:dd/MM/yyyy}";
                }
                else
                {
                    detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                }

                detailCell.ExpressionBindings.Add(new("BeforePrint", "Text", $"[{col.ColumnName}]"));
                detailRow.Cells.Add(detailCell);
            }

            detailTable.Rows.Add(detailRow);
            detailBand.Controls.Add(detailTable);

            this.DataSource = dt;
        }
    }
}
