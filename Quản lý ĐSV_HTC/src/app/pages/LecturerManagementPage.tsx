import { useState } from "react";
import { Plus, Trash2, Save, RotateCcw, X, Search } from "lucide-react";

const mockLecturers = [
  { id: "GV001", firstName: "Nguyễn Văn", lastName: "Anh", degree: "Tiến sĩ", rank: "Phó giáo sư", specialty: "Cơ sở dữ liệu", department: "CNTT" },
  { id: "GV002", firstName: "Trần Thị", lastName: "Bảo", degree: "Tiến sĩ", rank: "Giảng viên", specialty: "Lập trình Web", department: "CNTT" },
  { id: "GV003", firstName: "Lê Văn", lastName: "Cường", degree: "Thạc sĩ", rank: "Giảng viên", specialty: "Mạng máy tính", department: "CNTT" },
  { id: "GV004", firstName: "Phạm Thị", lastName: "Dung", degree: "Tiến sĩ", rank: "Giáo sư", specialty: "Trí tuệ nhân tạo", department: "CNTT" },
  { id: "GV005", firstName: "Hoàng Văn", lastName: "Em", degree: "Thạc sĩ", rank: "Giảng viên", specialty: "Kế toán quản trị", department: "KT" },
];

const departments = ["CNTT", "KT", "QTKD", "MARKETING", "NGOẠI NGỮ"];
const degrees = ["Tiến sĩ", "Thạc sĩ", "Cử nhân"];
const ranks = ["Giáo sư", "Phó giáo sư", "Giảng viên"];

export function LecturerManagementPage() {
  const [selectedLecturer, setSelectedLecturer] = useState<any>(null);
  const [searchTerm, setSearchTerm] = useState("");

  const filteredLecturers = mockLecturers.filter(
    (lecturer) =>
      lecturer.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
      lecturer.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      lecturer.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      lecturer.specialty.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Quản lý giảng viên</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Quản lý thông tin giảng viên
        </p>
      </div>

      <div className="bg-card border border-border rounded-xl p-6">
        {/* Action Buttons */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex gap-2">
            <button className="flex items-center gap-2 bg-primary text-primary-foreground px-4 py-2 rounded-lg hover:bg-primary/90 transition-colors">
              <Plus className="w-4 h-4" />
              Thêm
            </button>
            <button className="flex items-center gap-2 bg-destructive text-destructive-foreground px-4 py-2 rounded-lg hover:bg-destructive/90 transition-colors">
              <Trash2 className="w-4 h-4" />
              Xóa
            </button>
            <button className="flex items-center gap-2 bg-accent text-accent-foreground px-4 py-2 rounded-lg hover:bg-accent/90 transition-colors">
              <Save className="w-4 h-4" />
              Ghi
            </button>
            <button className="flex items-center gap-2 bg-secondary text-secondary-foreground px-4 py-2 rounded-lg hover:bg-secondary/80 transition-colors">
              <RotateCcw className="w-4 h-4" />
              Phục hồi
            </button>
            <button className="flex items-center gap-2 bg-secondary text-secondary-foreground px-4 py-2 rounded-lg hover:bg-secondary/80 transition-colors">
              <X className="w-4 h-4" />
              Thoát
            </button>
          </div>
        </div>

        {/* Search */}
        <div className="mb-6">
          <div className="relative max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <input
              type="text"
              placeholder="Tìm kiếm giảng viên..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </div>

        {/* Table */}
        <div className="mb-6 overflow-hidden border border-border rounded-lg">
          <table className="w-full">
            <thead className="bg-muted">
              <tr>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Mã GV</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Họ</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Tên</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Học vị</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Học hàm</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Chuyên môn</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Khoa</th>
              </tr>
            </thead>
            <tbody>
              {filteredLecturers.map((lecturer) => (
                <tr
                  key={lecturer.id}
                  onClick={() => setSelectedLecturer(lecturer)}
                  className={`border-t border-border cursor-pointer hover:bg-muted/50 transition-colors ${
                    selectedLecturer?.id === lecturer.id ? "bg-primary/5" : ""
                  }`}
                >
                  <td className="px-4 py-3 text-sm text-foreground">{lecturer.id}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{lecturer.firstName}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{lecturer.lastName}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{lecturer.degree}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{lecturer.rank}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{lecturer.specialty}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{lecturer.department}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Form */}
        <div className="border border-border rounded-lg p-6 bg-muted/20">
          <h3 className="text-base font-semibold mb-4">Thông tin giảng viên</h3>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-2">Mã giảng viên</label>
              <input
                type="text"
                value={selectedLecturer?.id || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập mã giảng viên"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Khoa</label>
              <select
                value={selectedLecturer?.department || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Chọn khoa</option>
                {departments.map((dept) => (
                  <option key={dept} value={dept}>
                    {dept}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Họ</label>
              <input
                type="text"
                value={selectedLecturer?.firstName || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập họ"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Tên</label>
              <input
                type="text"
                value={selectedLecturer?.lastName || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập tên"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Học vị</label>
              <select
                value={selectedLecturer?.degree || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Chọn học vị</option>
                {degrees.map((degree) => (
                  <option key={degree} value={degree}>
                    {degree}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Học hàm</label>
              <select
                value={selectedLecturer?.rank || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Chọn học hàm</option>
                {ranks.map((rank) => (
                  <option key={rank} value={rank}>
                    {rank}
                  </option>
                ))}
              </select>
            </div>
            <div className="col-span-2">
              <label className="block text-sm font-medium mb-2">Chuyên môn</label>
              <input
                type="text"
                value={selectedLecturer?.specialty || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập chuyên môn"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
