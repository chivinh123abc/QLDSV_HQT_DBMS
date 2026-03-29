import { useState } from "react";
import { Plus, Trash2, Save, RotateCcw } from "lucide-react";

const mockCreditClasses = [
  { id: "LTC001", year: "2023-2024", semester: 1, subject: "MH001 - Cơ sở dữ liệu", group: 1, lecturer: "GV001 - Nguyễn Văn Anh", department: "CNTT", minStudents: 30, cancelled: false },
  { id: "LTC002", year: "2023-2024", semester: 1, subject: "MH002 - Lập trình hướng đối tượng", group: 1, lecturer: "GV002 - Trần Thị Bảo", department: "CNTT", minStudents: 30, cancelled: false },
  { id: "LTC003", year: "2023-2024", semester: 1, subject: "MH003 - Cấu trúc dữ liệu", group: 1, lecturer: "GV003 - Lê Văn Cường", department: "CNTT", minStudents: 30, cancelled: false },
  { id: "LTC004", year: "2023-2024", semester: 1, subject: "MH004 - Mạng máy tính", group: 1, lecturer: "GV003 - Lê Văn Cường", department: "CNTT", minStudents: 25, cancelled: false },
  { id: "LTC005", year: "2023-2024", semester: 2, subject: "MH007 - Lập trình Web", group: 1, lecturer: "GV002 - Trần Thị Bảo", department: "CNTT", minStudents: 30, cancelled: false },
  { id: "LTC006", year: "2023-2024", semester: 2, subject: "MH008 - Trí tuệ nhân tạo", group: 1, lecturer: "GV004 - Phạm Thị Dung", department: "CNTT", minStudents: 25, cancelled: true },
];

const years = ["2023-2024", "2024-2025", "2025-2026"];
const semesters = [1, 2, 3];
const departments = ["CNTT", "KT", "QTKD", "MARKETING", "NGOẠI NGỮ"];
const subjects = ["MH001 - Cơ sở dữ liệu", "MH002 - Lập trình hướng đối tượng", "MH003 - Cấu trúc dữ liệu", "MH004 - Mạng máy tính", "MH007 - Lập trình Web", "MH008 - Trí tuệ nhân tạo"];
const lecturers = ["GV001 - Nguyễn Văn Anh", "GV002 - Trần Thị Bảo", "GV003 - Lê Văn Cường", "GV004 - Phạm Thị Dung"];

export function CreditClassPage() {
  const [selectedClass, setSelectedClass] = useState<any>(null);
  const [filterYear, setFilterYear] = useState("all");
  const [filterSemester, setFilterSemester] = useState("all");
  const [filterDept, setFilterDept] = useState("all");

  const filteredClasses = mockCreditClasses.filter(
    (cls) =>
      (filterYear === "all" || cls.year === filterYear) &&
      (filterSemester === "all" || cls.semester.toString() === filterSemester) &&
      (filterDept === "all" || cls.department === filterDept)
  );

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Mở lớp tín chỉ</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Quản lý các lớp tín chỉ theo niên khóa và học kỳ
        </p>
      </div>

      <div className="bg-card border border-border rounded-xl p-6">
        {/* Filters */}
        <div className="flex gap-4 mb-6">
          <select
            value={filterYear}
            onChange={(e) => setFilterYear(e.target.value)}
            className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option value="all">Tất cả niên khóa</option>
            {years.map((year) => (
              <option key={year} value={year}>
                {year}
              </option>
            ))}
          </select>
          <select
            value={filterSemester}
            onChange={(e) => setFilterSemester(e.target.value)}
            className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option value="all">Tất cả học kỳ</option>
            {semesters.map((sem) => (
              <option key={sem} value={sem.toString()}>
                Học kỳ {sem}
              </option>
            ))}
          </select>
          <select
            value={filterDept}
            onChange={(e) => setFilterDept(e.target.value)}
            className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option value="all">Tất cả khoa</option>
            {departments.map((dept) => (
              <option key={dept} value={dept}>
                {dept}
              </option>
            ))}
          </select>
        </div>

        {/* Action Buttons */}
        <div className="flex gap-2 mb-6">
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
        </div>

        {/* Table */}
        <div className="mb-6 overflow-hidden border border-border rounded-lg">
          <table className="w-full">
            <thead className="bg-muted">
              <tr>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Mã LTC</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Niên khóa</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Học kỳ</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Môn học</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Nhóm</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Giảng viên</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Khoa</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">SV tối thiểu</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Trạng thái</th>
              </tr>
            </thead>
            <tbody>
              {filteredClasses.map((cls) => (
                <tr
                  key={cls.id}
                  onClick={() => setSelectedClass(cls)}
                  className={`border-t border-border cursor-pointer hover:bg-muted/50 transition-colors ${
                    selectedClass?.id === cls.id ? "bg-primary/5" : ""
                  }`}
                >
                  <td className="px-4 py-3 text-sm text-foreground">{cls.id}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{cls.year}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{cls.semester}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{cls.subject}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{cls.group}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{cls.lecturer}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{cls.department}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{cls.minStudents}</td>
                  <td className="px-4 py-3 text-sm">
                    {cls.cancelled ? (
                      <span className="px-2 py-1 bg-destructive/10 text-destructive text-xs rounded-full font-medium">
                        Đã hủy
                      </span>
                    ) : (
                      <span className="px-2 py-1 bg-accent/10 text-accent text-xs rounded-full font-medium">
                        Đang mở
                      </span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Form */}
        <div className="border border-border rounded-lg p-6 bg-muted/20">
          <h3 className="text-base font-semibold mb-4">Thông tin lớp tín chỉ</h3>
          <div className="grid grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium mb-2">Niên khóa</label>
              <select
                value={selectedClass?.year || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Chọn niên khóa</option>
                {years.map((year) => (
                  <option key={year} value={year}>
                    {year}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Học kỳ</label>
              <select
                value={selectedClass?.semester || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Chọn học kỳ</option>
                {semesters.map((sem) => (
                  <option key={sem} value={sem}>
                    Học kỳ {sem}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Nhóm</label>
              <input
                type="number"
                value={selectedClass?.group || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập nhóm"
              />
            </div>
            <div className="col-span-2">
              <label className="block text-sm font-medium mb-2">Môn học</label>
              <select
                value={selectedClass?.subject || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Chọn môn học</option>
                {subjects.map((subj) => (
                  <option key={subj} value={subj}>
                    {subj}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Khoa</label>
              <select
                value={selectedClass?.department || ""}
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
            <div className="col-span-2">
              <label className="block text-sm font-medium mb-2">Giảng viên</label>
              <select
                value={selectedClass?.lecturer || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Chọn giảng viên</option>
                {lecturers.map((lec) => (
                  <option key={lec} value={lec}>
                    {lec}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Số SV tối thiểu</label>
              <input
                type="number"
                value={selectedClass?.minStudents || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập số SV tối thiểu"
              />
            </div>
            <div className="col-span-3">
              <label className="flex items-center gap-2">
                <input
                  type="checkbox"
                  checked={selectedClass?.cancelled || false}
                  className="w-4 h-4 text-primary rounded"
                />
                <span className="text-sm font-medium">Hủy lớp</span>
              </label>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
