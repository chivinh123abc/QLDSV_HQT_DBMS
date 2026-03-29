import { useState } from "react";
import { FileText, Eye, Printer, FileSpreadsheet, Filter } from "lucide-react";

const reportTypes = [
  {
    id: "credit-class-list",
    title: "Danh sách lớp tín chỉ",
    description: "In danh sách các lớp tín chỉ theo niên khóa và học kỳ",
    icon: FileText,
  },
  {
    id: "registered-students",
    title: "Danh sách sinh viên đăng ký lớp tín chỉ",
    description: "In danh sách sinh viên đã đăng ký từng lớp tín chỉ",
    icon: FileText,
  },
  {
    id: "subject-grades",
    title: "Bảng điểm môn học",
    description: "In bảng điểm của một môn học theo lớp tín chỉ",
    icon: FileText,
  },
  {
    id: "student-transcript",
    title: "Phiếu điểm sinh viên",
    description: "In phiếu điểm cá nhân của sinh viên",
    icon: FileText,
  },
  {
    id: "final-transcript",
    title: "Bảng điểm tổng kết cuối khóa",
    description: "In bảng điểm tổng kết toàn khóa học của sinh viên",
    icon: FileText,
  },
];

export function ReportsPage() {
  const [selectedReport, setSelectedReport] = useState<string | null>(null);
  const [showPreview, setShowPreview] = useState(false);
  const [filters, setFilters] = useState({
    year: "2023-2024",
    semester: "1",
    subject: "",
    group: "",
    class: "",
    studentId: "",
  });

  const handlePreview = () => {
    setShowPreview(true);
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Báo cáo / In ấn</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Xem và in các báo cáo hệ thống
        </p>
      </div>

      <div className="grid grid-cols-3 gap-6">
        {/* Filter Panel */}
        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-2 mb-4">
            <Filter className="w-5 h-5 text-primary" />
            <h3 className="text-base font-semibold">Bộ lọc</h3>
          </div>

          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-2">Niên khóa</label>
              <select
                value={filters.year}
                onChange={(e) => setFilters({ ...filters, year: e.target.value })}
                className="w-full px-3 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="2023-2024">2023-2024</option>
                <option value="2024-2025">2024-2025</option>
                <option value="2025-2026">2025-2026</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium mb-2">Học kỳ</label>
              <select
                value={filters.semester}
                onChange={(e) => setFilters({ ...filters, semester: e.target.value })}
                className="w-full px-3 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="1">Học kỳ 1</option>
                <option value="2">Học kỳ 2</option>
                <option value="3">Học kỳ 3</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium mb-2">Môn học</label>
              <select
                value={filters.subject}
                onChange={(e) => setFilters({ ...filters, subject: e.target.value })}
                className="w-full px-3 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Tất cả</option>
                <option value="MH001">MH001 - Cơ sở dữ liệu</option>
                <option value="MH002">MH002 - Lập trình hướng đối tượng</option>
                <option value="MH003">MH003 - Cấu trúc dữ liệu</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium mb-2">Nhóm</label>
              <select
                value={filters.group}
                onChange={(e) => setFilters({ ...filters, group: e.target.value })}
                className="w-full px-3 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Tất cả</option>
                <option value="1">Nhóm 1</option>
                <option value="2">Nhóm 2</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium mb-2">Mã lớp</label>
              <select
                value={filters.class}
                onChange={(e) => setFilters({ ...filters, class: e.target.value })}
                className="w-full px-3 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Tất cả</option>
                <option value="DH20CNTT01">DH20CNTT01</option>
                <option value="DH20CNTT02">DH20CNTT02</option>
                <option value="DH21CNTT01">DH21CNTT01</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium mb-2">Mã sinh viên</label>
              <input
                type="text"
                value={filters.studentId}
                onChange={(e) => setFilters({ ...filters, studentId: e.target.value })}
                placeholder="Nhập mã sinh viên"
                className="w-full px-3 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
          </div>
        </div>

        {/* Report Cards */}
        <div className="col-span-2 space-y-4">
          {reportTypes.map((report) => {
            const Icon = report.icon;
            return (
              <div
                key={report.id}
                className={`bg-card border rounded-xl p-6 cursor-pointer transition-all hover:shadow-md ${
                  selectedReport === report.id
                    ? "border-primary ring-2 ring-primary/20"
                    : "border-border"
                }`}
                onClick={() => setSelectedReport(report.id)}
              >
                <div className="flex items-start gap-4">
                  <div className="w-12 h-12 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                    <Icon className="w-6 h-6 text-primary" />
                  </div>
                  <div className="flex-1">
                    <h4 className="font-semibold text-foreground">{report.title}</h4>
                    <p className="text-sm text-muted-foreground mt-1">
                      {report.description}
                    </p>
                    <div className="flex gap-2 mt-4">
                      <button
                        onClick={handlePreview}
                        className="flex items-center gap-2 bg-primary text-primary-foreground px-4 py-2 rounded-lg text-sm hover:bg-primary/90 transition-colors"
                      >
                        <Eye className="w-4 h-4" />
                        Xem trước
                      </button>
                      <button className="flex items-center gap-2 bg-secondary text-secondary-foreground px-4 py-2 rounded-lg text-sm hover:bg-secondary/80 transition-colors">
                        <Printer className="w-4 h-4" />
                        In PDF
                      </button>
                      <button className="flex items-center gap-2 bg-accent text-accent-foreground px-4 py-2 rounded-lg text-sm hover:bg-accent/90 transition-colors">
                        <FileSpreadsheet className="w-4 h-4" />
                        Xuất Excel
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {/* Preview Panel */}
      {showPreview && (
        <div className="bg-card border border-border rounded-xl p-8">
          <div className="max-w-4xl mx-auto">
            {/* Report Header */}
            <div className="text-center mb-8">
              <h3 className="text-lg font-bold text-foreground">
                TRƯỜNG ĐẠI HỌC ABC
              </h3>
              <h4 className="text-base font-semibold text-foreground mt-2">
                BẢNG ĐIỂM MÔN HỌC
              </h4>
              <p className="text-sm text-muted-foreground mt-4">
                Niên khóa: {filters.year} - Học kỳ: {filters.semester}
              </p>
              <p className="text-sm text-muted-foreground">
                Môn: Cơ sở dữ liệu - Nhóm: 1
              </p>
            </div>

            {/* Sample Table */}
            <div className="overflow-hidden border border-border rounded-lg">
              <table className="w-full">
                <thead className="bg-muted">
                  <tr>
                    <th className="px-4 py-3 text-left text-sm font-semibold">STT</th>
                    <th className="px-4 py-3 text-left text-sm font-semibold">Mã SV</th>
                    <th className="px-4 py-3 text-left text-sm font-semibold">Họ tên</th>
                    <th className="px-4 py-3 text-center text-sm font-semibold">ĐCC</th>
                    <th className="px-4 py-3 text-center text-sm font-semibold">ĐGK</th>
                    <th className="px-4 py-3 text-center text-sm font-semibold">ĐCK</th>
                    <th className="px-4 py-3 text-center text-sm font-semibold">ĐTBM</th>
                  </tr>
                </thead>
                <tbody>
                  <tr className="border-t border-border">
                    <td className="px-4 py-2 text-sm">1</td>
                    <td className="px-4 py-2 text-sm">SV001</td>
                    <td className="px-4 py-2 text-sm">Nguyễn Văn An</td>
                    <td className="px-4 py-2 text-sm text-center">8.0</td>
                    <td className="px-4 py-2 text-sm text-center">7.5</td>
                    <td className="px-4 py-2 text-sm text-center">8.0</td>
                    <td className="px-4 py-2 text-sm text-center font-semibold">7.85</td>
                  </tr>
                  <tr className="border-t border-border">
                    <td className="px-4 py-2 text-sm">2</td>
                    <td className="px-4 py-2 text-sm">SV002</td>
                    <td className="px-4 py-2 text-sm">Trần Thị Bình</td>
                    <td className="px-4 py-2 text-sm text-center">9.0</td>
                    <td className="px-4 py-2 text-sm text-center">8.0</td>
                    <td className="px-4 py-2 text-sm text-center">8.5</td>
                    <td className="px-4 py-2 text-sm text-center font-semibold">8.40</td>
                  </tr>
                  <tr className="border-t border-border">
                    <td className="px-4 py-2 text-sm">3</td>
                    <td className="px-4 py-2 text-sm">SV003</td>
                    <td className="px-4 py-2 text-sm">Lê Văn Cường</td>
                    <td className="px-4 py-2 text-sm text-center">7.0</td>
                    <td className="px-4 py-2 text-sm text-center">6.5</td>
                    <td className="px-4 py-2 text-sm text-center">7.0</td>
                    <td className="px-4 py-2 text-sm text-center font-semibold">6.85</td>
                  </tr>
                </tbody>
              </table>
            </div>

            {/* Footer */}
            <div className="mt-8 flex justify-between">
              <div className="text-sm text-muted-foreground">
                <p>Ngày in: {new Date().toLocaleDateString("vi-VN")}</p>
              </div>
              <div className="text-center">
                <p className="text-sm font-semibold">Giảng viên</p>
                <p className="text-sm text-muted-foreground mt-8">(Ký và ghi rõ họ tên)</p>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
