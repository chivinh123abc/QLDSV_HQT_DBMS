import { useState } from "react";
import { Search, CheckCircle, XCircle } from "lucide-react";

const mockAvailableClasses = [
  { id: "LTC001", code: "MH001", name: "Cơ sở dữ liệu", group: 1, lecturer: "GV001 - Nguyễn Văn Anh", registered: 28, max: 40, isRegistered: false },
  { id: "LTC002", code: "MH002", name: "Lập trình hướng đối tượng", group: 1, lecturer: "GV002 - Trần Thị Bảo", registered: 35, max: 40, isRegistered: true },
  { id: "LTC003", code: "MH003", name: "Cấu trúc dữ liệu và giải thuật", group: 1, lecturer: "GV003 - Lê Văn Cường", registered: 40, max: 40, isRegistered: false },
  { id: "LTC004", code: "MH004", name: "Mạng máy tính", group: 1, lecturer: "GV003 - Lê Văn Cường", registered: 22, max: 35, isRegistered: false },
  { id: "LTC005", code: "MH007", name: "Lập trình Web", group: 1, lecturer: "GV002 - Trần Thị Bảo", registered: 30, max: 40, isRegistered: true },
  { id: "LTC006", code: "MH008", name: "Trí tuệ nhân tạo", group: 1, lecturer: "GV004 - Phạm Thị Dung", registered: 18, max: 30, isRegistered: false },
];

const studentInfo = {
  id: "SV001",
  name: "Nguyễn Văn An",
  class: "DH20CNTT01",
};

export function CourseRegistrationPage() {
  const [selectedYear, setSelectedYear] = useState("2023-2024");
  const [selectedSemester, setSelectedSemester] = useState("1");
  const [searchTerm, setSearchTerm] = useState("");
  const [classes, setClasses] = useState(mockAvailableClasses);

  const registeredClasses = classes.filter((c) => c.isRegistered);

  const handleToggleRegistration = (classId: string) => {
    setClasses(
      classes.map((c) =>
        c.id === classId ? { ...c, isRegistered: !c.isRegistered } : c
      )
    );
  };

  const filteredClasses = classes.filter(
    (cls) =>
      cls.code.toLowerCase().includes(searchTerm.toLowerCase()) ||
      cls.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Đăng ký tín chỉ</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Đăng ký các lớp tín chỉ trong học kỳ
        </p>
      </div>

      {/* Student Info Card */}
      <div className="bg-gradient-to-r from-blue-500 to-blue-600 rounded-xl p-6 text-white">
        <div className="flex items-center gap-8">
          <div>
            <p className="text-blue-100 text-sm">Mã sinh viên</p>
            <p className="font-semibold">{studentInfo.id}</p>
          </div>
          <div>
            <p className="text-blue-100 text-sm">Họ tên</p>
            <p className="font-semibold">{studentInfo.name}</p>
          </div>
          <div>
            <p className="text-blue-100 text-sm">Lớp</p>
            <p className="font-semibold">{studentInfo.class}</p>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-3 gap-6">
        {/* Main Registration Area */}
        <div className="col-span-2 bg-card border border-border rounded-xl p-6">
          {/* Filters */}
          <div className="flex gap-4 mb-6">
            <select
              value={selectedYear}
              onChange={(e) => setSelectedYear(e.target.value)}
              className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="2023-2024">Niên khóa 2023-2024</option>
              <option value="2024-2025">Niên khóa 2024-2025</option>
            </select>
            <select
              value={selectedSemester}
              onChange={(e) => setSelectedSemester(e.target.value)}
              className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="1">Học kỳ 1</option>
              <option value="2">Học kỳ 2</option>
              <option value="3">Học kỳ 3</option>
            </select>
            <div className="flex-1 relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
              <input
                type="text"
                placeholder="Tìm môn học..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
          </div>

          {/* Classes Table */}
          <div className="overflow-hidden border border-border rounded-lg">
            <table className="w-full">
              <thead className="bg-muted">
                <tr>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Mã MH
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Tên môn học
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Nhóm
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Giảng viên
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    SV đã ĐK
                  </th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody>
                {filteredClasses.map((cls) => {
                  const isFull = cls.registered >= cls.max;
                  return (
                    <tr
                      key={cls.id}
                      className="border-t border-border hover:bg-muted/50 transition-colors"
                    >
                      <td className="px-4 py-3 text-sm text-foreground">{cls.code}</td>
                      <td className="px-4 py-3 text-sm text-foreground">{cls.name}</td>
                      <td className="px-4 py-3 text-sm text-foreground">{cls.group}</td>
                      <td className="px-4 py-3 text-sm text-foreground">{cls.lecturer}</td>
                      <td className="px-4 py-3 text-sm">
                        <span
                          className={`${
                            isFull ? "text-destructive" : "text-foreground"
                          }`}
                        >
                          {cls.registered}/{cls.max}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-sm">
                        {cls.isRegistered ? (
                          <button
                            onClick={() => handleToggleRegistration(cls.id)}
                            className="flex items-center gap-1 px-3 py-1.5 bg-destructive/10 text-destructive rounded-lg hover:bg-destructive/20 transition-colors"
                          >
                            <XCircle className="w-4 h-4" />
                            Hủy ĐK
                          </button>
                        ) : (
                          <button
                            onClick={() => handleToggleRegistration(cls.id)}
                            disabled={isFull}
                            className={`flex items-center gap-1 px-3 py-1.5 rounded-lg transition-colors ${
                              isFull
                                ? "bg-muted text-muted-foreground cursor-not-allowed"
                                : "bg-accent text-accent-foreground hover:bg-accent/90"
                            }`}
                          >
                            <CheckCircle className="w-4 h-4" />
                            {isFull ? "Đã đầy" : "Đăng ký"}
                          </button>
                        )}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>

        {/* Registered Classes Panel */}
        <div className="bg-card border border-border rounded-xl p-6">
          <h3 className="text-base font-semibold mb-4">Môn đã chọn</h3>
          <div className="space-y-3">
            {registeredClasses.length > 0 ? (
              registeredClasses.map((cls) => (
                <div
                  key={cls.id}
                  className="p-4 bg-accent/10 border border-accent/20 rounded-lg"
                >
                  <p className="font-medium text-sm text-foreground">{cls.name}</p>
                  <p className="text-xs text-muted-foreground mt-1">
                    Nhóm {cls.group} - {cls.code}
                  </p>
                  <p className="text-xs text-muted-foreground">{cls.lecturer}</p>
                </div>
              ))
            ) : (
              <p className="text-sm text-muted-foreground text-center py-8">
                Chưa đăng ký môn nào
              </p>
            )}
          </div>

          <div className="mt-6 pt-4 border-t border-border">
            <div className="flex items-center justify-between mb-2">
              <span className="text-sm text-muted-foreground">Tổng số môn</span>
              <span className="font-semibold text-foreground">
                {registeredClasses.length}
              </span>
            </div>
            <button className="w-full mt-4 bg-primary text-primary-foreground py-2.5 rounded-lg font-medium hover:bg-primary/90 transition-colors">
              Xác nhận đăng ký
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
