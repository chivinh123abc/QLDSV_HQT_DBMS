import { useState } from "react";
import { Plus, Trash2, Save, RotateCcw, X, Search } from "lucide-react";

const mockSubjects = [
  { id: "MH001", name: "Cơ sở dữ liệu", theoryHours: 30, practiceHours: 30 },
  { id: "MH002", name: "Lập trình hướng đối tượng", theoryHours: 30, practiceHours: 30 },
  { id: "MH003", name: "Cấu trúc dữ liệu và giải thuật", theoryHours: 30, practiceHours: 30 },
  { id: "MH004", name: "Mạng máy tính", theoryHours: 30, practiceHours: 15 },
  { id: "MH005", name: "Hệ điều hành", theoryHours: 30, practiceHours: 15 },
  { id: "MH006", name: "Kỹ thuật phần mềm", theoryHours: 30, practiceHours: 15 },
  { id: "MH007", name: "Lập trình Web", theoryHours: 30, practiceHours: 30 },
  { id: "MH008", name: "Trí tuệ nhân tạo", theoryHours: 30, practiceHours: 15 },
];

export function SubjectManagementPage() {
  const [selectedSubject, setSelectedSubject] = useState<any>(null);
  const [searchTerm, setSearchTerm] = useState("");

  const filteredSubjects = mockSubjects.filter(
    (subject) =>
      subject.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
      subject.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Quản lý môn học</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Quản lý thông tin các môn học
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
              placeholder="Tìm kiếm theo mã môn học hoặc tên môn học..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </div>

        <div className="grid grid-cols-2 gap-6">
          {/* Table */}
          <div className="overflow-hidden border border-border rounded-lg">
            <table className="w-full">
              <thead className="bg-muted">
                <tr>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Mã môn học
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Tên môn học
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Số tiết LT
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Số tiết TH
                  </th>
                </tr>
              </thead>
              <tbody>
                {filteredSubjects.map((subject) => (
                  <tr
                    key={subject.id}
                    onClick={() => setSelectedSubject(subject)}
                    className={`border-t border-border cursor-pointer hover:bg-muted/50 transition-colors ${
                      selectedSubject?.id === subject.id ? "bg-primary/5" : ""
                    }`}
                  >
                    <td className="px-4 py-3 text-sm text-foreground">{subject.id}</td>
                    <td className="px-4 py-3 text-sm text-foreground">{subject.name}</td>
                    <td className="px-4 py-3 text-sm text-foreground">{subject.theoryHours}</td>
                    <td className="px-4 py-3 text-sm text-foreground">{subject.practiceHours}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Form */}
          <div className="border border-border rounded-lg p-6 bg-muted/20">
            <h3 className="text-base font-semibold mb-4">Thông tin môn học</h3>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-2">Mã môn học</label>
                <input
                  type="text"
                  value={selectedSubject?.id || ""}
                  className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  placeholder="Nhập mã môn học"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Tên môn học</label>
                <input
                  type="text"
                  value={selectedSubject?.name || ""}
                  className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  placeholder="Nhập tên môn học"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Số tiết lý thuyết</label>
                <input
                  type="number"
                  value={selectedSubject?.theoryHours || ""}
                  className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  placeholder="Nhập số tiết lý thuyết"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Số tiết thực hành</label>
                <input
                  type="number"
                  value={selectedSubject?.practiceHours || ""}
                  className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  placeholder="Nhập số tiết thực hành"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
