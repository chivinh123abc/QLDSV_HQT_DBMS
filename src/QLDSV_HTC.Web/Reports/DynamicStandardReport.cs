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
            string title = BuildTitle(request);
            bool groupedPageBreak = request.PrintByGroup
                && request.PageBreakPerGroup
                && !string.IsNullOrWhiteSpace(request.GroupByColumn);

            XRLabel lblTitle = new()
            {
                Text = title.ToUpper(),
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

            if (groupedPageBreak)
            {
                pageHeaderBand.HeightF = 0f;
                pageHeaderBand.Visible = false;
            }
            else
            {
                pageHeaderBand.Controls.Add(headerTable);
            }

            // ------- DETAIL BAND DYNAMIC -------
            var detailFont = new DevExpress.Drawing.DXFont("Arial", 11, DevExpress.Drawing.DXFontStyle.Regular);
            const DevExpress.XtraPrinting.BorderSide borderSideDetail =
                DevExpress.XtraPrinting.BorderSide.Left |
                DevExpress.XtraPrinting.BorderSide.Right |
                DevExpress.XtraPrinting.BorderSide.Bottom;

            XRTable detailTable = new()
            {
                SizeF = new(totalWidth, 30f),
                Borders = borderSideDetail,
                Font = detailFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
            };
            XRTableRow detailRow = new();

            // STT Expression - reset về 1 mỗi nhóm nếu có gom nhóm
            SummaryRunning runningMode = request.PrintByGroup ? SummaryRunning.Group : SummaryRunning.Report;
            XRTableCell cellStt = new()
            {
                WidthF = sttWidth,
                Summary = new() { Func = SummaryFunc.RecordNumber, Running = runningMode }
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

            // ------- F5: PRINT BY GROUP -------
            if (request.PrintByGroup && !string.IsNullOrWhiteSpace(request.GroupByColumn))
            {
                string rawGroupCol = request.GroupByColumn;
                string colPart = rawGroupCol.Contains('.') ? rawGroupCol.Split('.').Last() : rawGroupCol;

                string? matchedCol = null;
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.Equals(colPart, StringComparison.OrdinalIgnoreCase)
                        || col.ColumnName.Equals(rawGroupCol, StringComparison.OrdinalIgnoreCase))
                    {
                        matchedCol = col.ColumnName;
                        break;
                    }
                }

                if (matchedCol != null)
                {
                    request.GroupByColumn = matchedCol;
                    ApplyGroupedLayout(request, totalWidth, title, headerTable);

                    if (groupedPageBreak)
                    {
                        reportHeaderBand.HeightF = 0f;
                        reportHeaderBand.Visible = false;
                    }
                }
            }

            // ------- F6: PAGE FOOTER WITH PAGE NUMBERS -------
            var pageFooterBand = new PageFooterBand() { HeightF = 30f };
            var lblPageNumber = new XRPageInfo()
            {
                Format = "Trang {0} / {1}",
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight,
                SizeF = new SizeF(totalWidth / 2, 20f),
                LocationF = new PointF(totalWidth / 2, 5f),
                Font = new DevExpress.Drawing.DXFont("Arial", 9, DevExpress.Drawing.DXFontStyle.Italic)
            };
            pageFooterBand.Controls.Add(lblPageNumber);
            Bands.Add(pageFooterBand);

            this.DataSource = dt;
        }

        private static string BuildTitle(DynamicQueryRequestDto request)
        {
            string title = !string.IsNullOrWhiteSpace(request.ReportTitle)
                ? request.ReportTitle
                : $"BÁO CÁO TÙY CHỈNH: {request.TableName}";

            if (request.Filters == null || request.Filters.Count == 0) return title;

            foreach (var filter in request.Filters)
            {
                if (string.IsNullOrWhiteSpace(filter.Value)) continue;
                string placeholder = $"@{filter.ColumnName.Trim().ToUpper()}";
                if (title.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
                {
                    title = title.Replace(placeholder, filter.Value.Trim(), StringComparison.OrdinalIgnoreCase);
                }
            }

            return title;
        }

        private void ApplyGroupedLayout(DynamicQueryRequestDto request, float totalWidth, string reportTitle, XRTable headerTable)
        {
            string groupCol = request.GroupByColumn!;

            // Tính chiều cao GroupHeader: 
            // - Sang trang: tiêu đề (40f) + table header (40f) + margins = 90f (để khít với detailTable)
            // - Không sang trang: chỉ có group label màu xanh (30f) + margins = 35f
            float headerHeight = request.PageBreakPerGroup ? 90f : 35f;

            var groupHeader = new GroupHeaderBand()
            {
                HeightF = headerHeight,
                GroupFields = { new GroupField(groupCol, XRColumnSortOrder.Ascending) },
                RepeatEveryPage = true
            };

            if (request.PageBreakPerGroup)
            {
                groupHeader.PageBreak = PageBreak.BeforeBand;

                var lblGroupTitle = new XRLabel()
                {
                    Font = new DevExpress.Drawing.DXFont("Arial", 18, DevExpress.Drawing.DXFontStyle.Bold),
                    TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                    SizeF = new SizeF(totalWidth, 40f),
                    LocationF = new PointF(0f, 5f)
                };
                string escapedTitle = reportTitle.ToUpper().Replace("'", "''");
                lblGroupTitle.ExpressionBindings.Add(
                    new("BeforePrint", "Text", $"'{escapedTitle} ' + [{groupCol}]"));
                groupHeader.Controls.Add(lblGroupTitle);

                headerTable.LocationF = new PointF(0f, 50f);
                groupHeader.Controls.Add(headerTable);
            }
            else
            {
                var lblGroupHeader = new XRLabel()
                {
                    Font = new DevExpress.Drawing.DXFont("Arial", 12, DevExpress.Drawing.DXFontStyle.Bold),
                    TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft,
                    SizeF = new SizeF(totalWidth, 30f),
                    LocationF = new PointF(0f, 2f),
                    BackColor = Color.FromArgb(230, 240, 255),
                    Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 0, 0, 0)
                };
                lblGroupHeader.ExpressionBindings.Add(new("BeforePrint", "Text", $"'► ' + [{groupCol}]"));
                groupHeader.Controls.Add(lblGroupHeader);
            }

            var groupFooter = new GroupFooterBand() { HeightF = 25f };
            var lblGroupCount = new XRLabel()
            {
                Font = new DevExpress.Drawing.DXFont("Arial", 9, DevExpress.Drawing.DXFontStyle.Italic),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight,
                SizeF = new SizeF(totalWidth, 20f),
                LocationF = new PointF(0f, 2f),
                ForeColor = Color.Gray,
                Summary = new XRSummary()
                {
                    Running = SummaryRunning.Group,
                    Func = SummaryFunc.RecordNumber,
                    FormatString = "Tổng cộng nhóm: {0} dòng"
                }
            };
            groupFooter.Controls.Add(lblGroupCount);

            Bands.Add(groupHeader);
            Bands.Add(groupFooter);
        }
    }
}
