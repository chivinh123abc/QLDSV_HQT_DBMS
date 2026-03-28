using DevExpress.XtraReports.UI;
using System.Drawing;

namespace QLDSV_HTC.Web.Reports
{
    public class BangDiemReport : XtraReport
    {
        public BangDiemReport()
        {
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            
            var detail = new DetailBand() { HeightF = 30f };
            var reportHeader = new ReportHeaderBand() { HeightF = 100f };
            this.Bands.AddRange(new Band[] { reportHeader, detail });

            XRLabel lblTitle = new XRLabel {
                Text = "PHIẾU ĐIỂM SINH VIÊN",
                Font = new DevExpress.Drawing.DXFont("Arial", 18, DevExpress.Drawing.DXFontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new SizeF(650f, 50f)
            };
            reportHeader.Controls.Add(lblTitle);

            XRTable reportTable = new XRTable() { SizeF = new SizeF(650f, 30f) };
            XRTableRow detailRow = new XRTableRow();

            XRTableCell subjectCell = new XRTableCell() { WidthF = 150f };
            subjectCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[TENMH]"));
            
            XRTableCell scoreCell = new XRTableCell() { WidthF = 100f };
            scoreCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[DIEM]"));

            detailRow.Cells.AddRange(new XRTableCell[] { subjectCell, scoreCell });
            reportTable.Rows.Add(detailRow);
            detail.Controls.Add(reportTable);
        }
    }
}
