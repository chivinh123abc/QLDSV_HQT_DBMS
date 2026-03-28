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
            Margins = new DevExpress.Drawing.DXMargins(50, 50, 50, 50);

            var detailBand = new DetailBand() { HeightF = 30f };
            var reportHeaderBand = new ReportHeaderBand() { HeightF = 130f };
            var pageHeaderBand = new PageHeaderBand() { HeightF = 40f };

            Bands.AddRange(new Band[] { reportHeaderBand, pageHeaderBand, detailBand });

            // ------- REPORT HEADER -------
            XRLabel lblKhoa = new XRLabel
            {
                Text = $"KHOA: {khoaName.ToUpper()}",
                Font = new DevExpress.Drawing.DXFont("Times New Roman", 15, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new SizeF(1069f, 30f),
                LocationF = new PointF(0f, 0f)
            };

            XRLabel lblTitle = new XRLabel
            {
                Text = "DANH SÁCH LỚP TÍN CHỈ",
                Font = new DevExpress.Drawing.DXFont("Times New Roman", 18, DevExpress.Drawing.DXFontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new SizeF(1069f, 40f),
                LocationF = new PointF(0f, 40f)
            };

            XRLabel lblNienKhoaHocKy = new XRLabel
            {
                Text = $"Niên khóa: {nienKhoa}  -  Học kỳ: {hocKy}",
                Font = new DevExpress.Drawing.DXFont("Times New Roman", 13, DevExpress.Drawing.DXFontStyle.Regular),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                SizeF = new SizeF(1069f, 30f),
                LocationF = new PointF(0f, 80f)
            };

            reportHeaderBand.Controls.AddRange(new XRControl[] { lblKhoa, lblTitle, lblNienKhoaHocKy });

            // ------- PAGE HEADER (Headers of the Table) -------
            var borderSideAll = DevExpress.XtraPrinting.BorderSide.All;
            var headerFont = new DevExpress.Drawing.DXFont("Times New Roman", 12, DevExpress.Drawing.DXFontStyle.Bold);

            XRTable headerTable = new XRTable()
            {
                SizeF = new SizeF(1069f, 40f),
                Borders = borderSideAll,
                Font = headerFont,
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                BackColor = Color.LightGray
            };

            XRTableRow headerRow = new XRTableRow();

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

            XRTable detailTable = new XRTable()
            {
                SizeF = new SizeF(1069f, 30f),
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
                WidthF = 400f,
                Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 5, 0, 0),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
            };
            cellTenMh.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[TENMH]"));

            XRTableCell cellNhom = new XRTableCell() { WidthF = 80f };
            cellNhom.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[NHOM]"));

            XRTableCell cellGiangVien = new XRTableCell()
            {
                WidthF = 250f,
                Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 5, 0, 0),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
            };
            cellGiangVien.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[HOTEN_GV]"));

            XRTableCell cellSvToiThieu = new XRTableCell() { WidthF = 139f };
            cellSvToiThieu.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[SOSVTOITHIEU]"));

            XRTableCell cellSvDangKy = new XRTableCell() { WidthF = 140f };
            cellSvDangKy.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[SOSV_DANGKY]"));

            detailRow.Cells.AddRange(new XRTableCell[] { cellStt, cellTenMh, cellNhom, cellGiangVien, cellSvToiThieu, cellSvDangKy });
            detailTable.Rows.Add(detailRow);
            detailBand.Controls.Add(detailTable);
        }
    }
}
