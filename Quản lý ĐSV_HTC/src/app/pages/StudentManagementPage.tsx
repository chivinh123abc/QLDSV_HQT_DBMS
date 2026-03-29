import { useState } from "react";
import { Plus, Trash2, Save, RotateCcw, X, Search } from "lucide-react";

const mockStudents = [
  { id: "SV001", firstName: "Nguyễn Văn", lastName: "An", gender: "Nam", dob: "2002-05-15", address: "123 Lê Lợi, Q1, TP.HCM", class: "DH20CNTT01", onLeave: false },
  { id: "SV002", firstName: "Trần Thị", lastName: "Bình", gender: "Nữ", dob: "2002-08-20", address: "456 Nguyễn Huệ, Q1, TP.HCM", class: "DH20CNTT01", onLeave: false },
  { id: "SV003", firstName: "Lê Văn", lastName: "Cường", gender: "Nam", dob: "2002-03-10", address: "789 Trần Hưng Đạo, Q5, TP.HCM", class: "DH20CNTT02", onLeave: false },
  { id: "SV004", firstName: "Phạm Thị", lastName: "Dung", gender: "Nữ", dob: "2002-11-25", address: "321 Võ Văn Tần, Q3, TP.HCM", class: "DH21CNTT01", onLeave: true },
  { id: "SV005", firstName: "Hoàng Văn", lastName: "Em", gender: "Nam", dob: "2003-01-30", address: "654 Hai Bà Trưng, Q3, TP.HCM", class: "DH21CNTT01", onLeave: false },
];

const classes = ["DH20CNTT01", "DH20CNTT02", "DH21CNTT01", "DH21KT01", "DH21QTKD01"];
const departments = ["CNTT", "KT", "QTKD", "MARKETING", "NGOẠI NGỮ"];

export function StudentManagementPage() {
  const [selectedStudent, setSelectedStudent] = useState<any>(null);
  const [selectedClass, setSelectedClass] = useState("all");
  const [selectedDept, setSelectedDept] = useState("all");
  const [searchTerm, setSearchTerm] = useState("");

  const filteredStudents = mockStudents.filter(
    (student) =>
      (selectedClass === "all" || student.class === selectedClass) &&
      (student.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
        student.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        student.lastName.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Quản lý sinh viên</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Quản lý thông tin sinh viên theo lớp
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

        {/* Filters */}
        <div className="flex gap-4 mb-6">
          <select
            value={selectedDept}
            onChange={(e) => setSelectedDept(e.target.value)}
            className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option value="all">Tất cả khoa</option>
            {departments.map((dept) => (
              <option key={dept} value={dept}>
                {dept}
              </option>
            ))}
          </select>
          <select
            value={selectedClass}
            onChange={(e) => setSelectedClass(e.target.value)}
            className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option value="all">Tất cả lớp</option>
            {classes.map((cls) => (
              <option key={cls} value={cls}>
                {cls}
              </option>
            ))}
          </select>
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <input
              type="text"
              placeholder="Tìm kiếm theo mã SV, họ, tên..."
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
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Mã SV</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Họ</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Tên</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Phái</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Ngày sinh</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Địa chỉ</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Lớp</th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">Nghỉ học</th>
              </tr>
            </thead>
            <tbody>
              {filteredStudents.map((student) => (
                <tr
                  key={student.id}
                  onClick={() => setSelectedStudent(student)}
                  className={`border-t border-border cursor-pointer hover:bg-muted/50 transition-colors ${
                    selectedStudent?.id === student.id ? "bg-primary/5" : ""
                  }`}
                >
                  <td className="px-4 py-3 text-sm text-foreground">{student.id}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{student.firstName}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{student.lastName}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{student.gender}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{student.dob}</td>
                  <td className="px-4 py-3 text-sm text-foreground truncate max-w-xs">{student.address}</td>
                  <td className="px-4 py-3 text-sm text-foreground">{student.class}</td>
                  <td className="px-4 py-3 text-sm">
                    {student.onLeave ? (
                      <span className="px-2 py-1 bg-destructive/10 text-destructive text-xs rounded-full">
                        Có
                      </span>
                    ) : (
                      <span className="px-2 py-1 bg-accent/10 text-accent text-xs rounded-full">
                        Không
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
          <h3 className="text-base font-semibold mb-4">Thông tin sinh viên</h3>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-2">Mã sinh viên</label>
              <input
                type="text"
                value={selectedStudent?.id || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập mã sinh viên"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Lớp</label>
              <select
                value={selectedStudent?.class || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Chọn lớp</option>
                {classes.map((cls) => (
                  <option key={cls} value={cls}>
                    {cls}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Họ</label>
              <input
                type="text"
                value={selectedStudent?.firstName || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập họ"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Tên</label>
              <input
                type="text"
                value={selectedStudent?.lastName || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập tên"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Phái</label>
              <div className="flex gap-4">
                <label className="flex items-center gap-2">
                  <input
                    type="radio"
                    name="gender"
                    value="Nam"
                    checked={selectedStudent?.gender === "Nam"}
                    className="w-4 h-4 text-primary"
                  />
                  <span className="text-sm">Nam</span>
                </label>
                <label className="flex items-center gap-2">
                  <input
                    type="radio"
                    name="gender"
                    value="Nữ"
                    checked={selectedStudent?.gender === "Nữ"}
                    className="w-4 h-4 text-primary"
                  />
                  <span className="text-sm">Nữ</span>
                </label>
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Ngày sinh</label>
              <input
                type="date"
                value={selectedStudent?.dob || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
            <div className="col-span-2">
              <label className="block text-sm font-medium mb-2">Địa chỉ</label>
              <input
                type="text"
                value={selectedStudent?.address || ""}
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập địa chỉ"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Password</label>
              <input
                type="password"
                className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Nhập mật khẩu"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Trạng thái</label>
              <label className="flex items-center gap-2">
                <input
                  type="checkbox"
                  checked={selectedStudent?.onLeave || false}
                  className="w-4 h-4 text-primary rounded"
                />
                <span className="text-sm">Đang nghỉ học</span>
              </label>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
