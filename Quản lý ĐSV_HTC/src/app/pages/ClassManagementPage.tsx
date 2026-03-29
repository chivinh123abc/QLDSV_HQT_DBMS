import { useState } from "react";
import { Plus, Trash2, Save, RotateCcw, X, Search } from "lucide-react";

const mockClasses = [
  { id: "DH20CNTT01", name: "Công nghệ thông tin 1", year: "2020", department: "CNTT" },
  { id: "DH20CNTT02", name: "Công nghệ thông tin 2", year: "2020", department: "CNTT" },
  { id: "DH21CNTT01", name: "Công nghệ thông tin 1", year: "2021", department: "CNTT" },
  { id: "DH21KT01", name: "Kế toán 1", year: "2021", department: "KT" },
  { id: "DH21QTKD01", name: "Quản trị kinh doanh 1", year: "2021", department: "QTKD" },
];

const departments = ["CNTT", "KT", "QTKD", "MARKETING", "NGOẠI NGỮ"];

export function ClassManagementPage() {
  const [selectedClass, setSelectedClass] = useState<any>(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedDept, setSelectedDept] = useState("all");

  const filteredClasses = mockClasses.filter(
    (cls) =>
      (selectedDept === "all" || cls.department === selectedDept) &&
      (cls.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
        cls.name.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Quản lý lớp</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Quản lý thông tin lớp học
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
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <input
              type="text"
              placeholder="Tìm kiếm theo mã lớp hoặc tên lớp..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
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
        </div>

        <div className="grid grid-cols-2 gap-6">
          {/* Table */}
          <div className="overflow-hidden border border-border rounded-lg">
            <table className="w-full">
              <thead className="bg-muted">
                <tr>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Mã lớp
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Tên lớp
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Khóa học
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Mã khoa
                  </th>
                </tr>
              </thead>
              <tbody>
                {filteredClasses.map((cls, index) => (
                  <tr
                    key={cls.id}
                    onClick={() => setSelectedClass(cls)}
                    className={`border-t border-border cursor-pointer hover:bg-muted/50 transition-colors ${
                      selectedClass?.id === cls.id ? "bg-primary/5" : ""
                    }`}
                  >
                    <td className="px-4 py-3 text-sm text-foreground">{cls.id}</td>
                    <td className="px-4 py-3 text-sm text-foreground">{cls.name}</td>
                    <td className="px-4 py-3 text-sm text-foreground">{cls.year}</td>
                    <td className="px-4 py-3 text-sm text-foreground">{cls.department}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Form */}
          <div className="border border-border rounded-lg p-6 bg-muted/20">
            <h3 className="text-base font-semibold mb-4">Thông tin lớp</h3>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-2">Mã lớp</label>
                <input
                  type="text"
                  value={selectedClass?.id || ""}
                  className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  placeholder="Nhập mã lớp"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Tên lớp</label>
                <input
                  type="text"
                  value={selectedClass?.name || ""}
                  className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  placeholder="Nhập tên lớp"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Khóa học</label>
                <input
                  type="text"
                  value={selectedClass?.year || ""}
                  className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  placeholder="Nhập khóa học"
                />
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
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
