import { useState } from "react";
import { Save, RefreshCw, FileDown, AlertCircle } from "lucide-react";

const mockStudentGrades = [
  { id: "SV001", name: "Nguyễn Văn An", attendance: 8, midterm: 7.5, final: 8, total: 0 },
  { id: "SV002", name: "Trần Thị Bình", attendance: 9, midterm: 8, final: 8.5, total: 0 },
  { id: "SV003", name: "Lê Văn Cường", attendance: 7, midterm: 6.5, final: 7, total: 0 },
  { id: "SV004", name: "Phạm Thị Dung", attendance: 10, midterm: 9, final: 9.5, total: 0 },
  { id: "SV005", name: "Hoàng Văn Em", attendance: 8, midterm: 7, final: 7.5, total: 0 },
];

const calculateTotal = (attendance: number, midterm: number, final: number) => {
  return (attendance * 0.1 + midterm * 0.3 + final * 0.6).toFixed(2);
};

export function GradeEntryPage() {
  const [selectedYear, setSelectedYear] = useState("");
  const [selectedSemester, setSelectedSemester] = useState("");
  const [selectedSubject, setSelectedSubject] = useState("");
  const [selectedGroup, setSelectedGroup] = useState("");
  const [started, setStarted] = useState(false);
  const [students, setStudents] = useState(mockStudentGrades);

  const handleStart = () => {
    if (selectedYear && selectedSemester && selectedSubject && selectedGroup) {
      setStarted(true);
      // Calculate totals
      const updatedStudents = students.map((s) => ({
        ...s,
        total: parseFloat(calculateTotal(s.attendance, s.midterm, s.final)),
      }));
      setStudents(updatedStudents);
    }
  };

  const handleGradeChange = (studentId: string, field: string, value: number) => {
    setStudents(
      students.map((s) =>
        s.id === studentId ? { ...s, [field]: value } : s
      )
    );
  };

  const handleSave = () => {
    // Calculate all totals before saving
    const updatedStudents = students.map((s) => ({
      ...s,
      total: parseFloat(calculateTotal(s.attendance, s.midterm, s.final)),
    }));
    setStudents(updatedStudents);
    alert("Đã lưu điểm thành công!");
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Nhập điểm</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Nhập điểm cho sinh viên theo lớp tín chỉ
        </p>
      </div>

      <div className="bg-card border border-border rounded-xl p-6">
        {/* Filter Section */}
        <div className="mb-6">
          <h3 className="text-base font-semibold mb-4">Chọn lớp tín chỉ</h3>
          <div className="grid grid-cols-5 gap-4">
            <select
              value={selectedYear}
              onChange={(e) => setSelectedYear(e.target.value)}
              className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="">Niên khóa</option>
              <option value="2023-2024">2023-2024</option>
              <option value="2024-2025">2024-2025</option>
            </select>
            <select
              value={selectedSemester}
              onChange={(e) => setSelectedSemester(e.target.value)}
              className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="">Học kỳ</option>
              <option value="1">Học kỳ 1</option>
              <option value="2">Học kỳ 2</option>
              <option value="3">Học kỳ 3</option>
            </select>
            <select
              value={selectedSubject}
              onChange={(e) => setSelectedSubject(e.target.value)}
              className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="">Môn học</option>
              <option value="MH001">MH001 - Cơ sở dữ liệu</option>
              <option value="MH002">MH002 - Lập trình hướng đối tượng</option>
              <option value="MH003">MH003 - Cấu trúc dữ liệu</option>
            </select>
            <select
              value={selectedGroup}
              onChange={(e) => setSelectedGroup(e.target.value)}
              className="px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="">Nhóm</option>
              <option value="1">Nhóm 1</option>
              <option value="2">Nhóm 2</option>
            </select>
            <button
              onClick={handleStart}
              disabled={!selectedYear || !selectedSemester || !selectedSubject || !selectedGroup}
              className="bg-primary text-primary-foreground px-6 py-2 rounded-lg hover:bg-primary/90 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Bắt đầu
            </button>
          </div>
        </div>

        {started && (
          <>
            {/* Formula Note */}
            <div className="mb-6 p-4 bg-blue-50 border border-blue-200 rounded-lg flex items-start gap-3">
              <AlertCircle className="w-5 h-5 text-blue-600 flex-shrink-0 mt-0.5" />
              <div>
                <p className="text-sm font-medium text-blue-900">
                  Công thức tính điểm hết môn:
                </p>
                <p className="text-sm text-blue-700 mt-1">
                  Điểm hết môn = Điểm chuyên cần × 0.1 + Điểm giữa kỳ × 0.3 + Điểm cuối kỳ × 0.6
                </p>
              </div>
            </div>

            {/* Grades Table */}
            <div className="overflow-hidden border border-border rounded-lg mb-6">
              <table className="w-full">
                <thead className="bg-muted">
                  <tr>
                    <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                      Mã SV
                    </th>
                    <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                      Họ tên SV
                    </th>
                    <th className="px-4 py-3 text-center text-sm font-semibold text-foreground">
                      Điểm chuyên cần
                    </th>
                    <th className="px-4 py-3 text-center text-sm font-semibold text-foreground">
                      Điểm giữa kỳ
                    </th>
                    <th className="px-4 py-3 text-center text-sm font-semibold text-foreground">
                      Điểm cuối kỳ
                    </th>
                    <th className="px-4 py-3 text-center text-sm font-semibold text-foreground">
                      Điểm hết môn
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {students.map((student) => (
                    <tr key={student.id} className="border-t border-border hover:bg-muted/30">
                      <td className="px-4 py-3 text-sm text-foreground bg-muted/50">
                        {student.id}
                      </td>
                      <td className="px-4 py-3 text-sm text-foreground bg-muted/50">
                        {student.name}
                      </td>
                      <td className="px-4 py-3">
                        <input
                          type="number"
                          min="0"
                          max="10"
                          step="0.5"
                          value={student.attendance}
                          onChange={(e) =>
                            handleGradeChange(student.id, "attendance", parseFloat(e.target.value) || 0)
                          }
                          className="w-20 px-3 py-1.5 text-center border border-input bg-input-background rounded focus:outline-none focus:ring-2 focus:ring-primary"
                        />
                      </td>
                      <td className="px-4 py-3">
                        <input
                          type="number"
                          min="0"
                          max="10"
                          step="0.5"
                          value={student.midterm}
                          onChange={(e) =>
                            handleGradeChange(student.id, "midterm", parseFloat(e.target.value) || 0)
                          }
                          className="w-20 px-3 py-1.5 text-center border border-input bg-input-background rounded focus:outline-none focus:ring-2 focus:ring-primary"
                        />
                      </td>
                      <td className="px-4 py-3">
                        <input
                          type="number"
                          min="0"
                          max="10"
                          step="0.5"
                          value={student.final}
                          onChange={(e) =>
                            handleGradeChange(student.id, "final", parseFloat(e.target.value) || 0)
                          }
                          className="w-20 px-3 py-1.5 text-center border border-input bg-input-background rounded focus:outline-none focus:ring-2 focus:ring-primary"
                        />
                      </td>
                      <td className="px-4 py-3 text-center text-sm font-semibold text-foreground bg-muted/50">
                        {calculateTotal(student.attendance, student.midterm, student.final)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Action Buttons */}
            <div className="flex gap-3">
              <button
                onClick={handleSave}
                className="flex items-center gap-2 bg-accent text-accent-foreground px-6 py-2.5 rounded-lg hover:bg-accent/90 transition-colors"
              >
                <Save className="w-4 h-4" />
                Ghi điểm
              </button>
              <button
                onClick={() => setStudents(mockStudentGrades)}
                className="flex items-center gap-2 bg-secondary text-secondary-foreground px-6 py-2.5 rounded-lg hover:bg-secondary/80 transition-colors"
              >
                <RefreshCw className="w-4 h-4" />
                Làm mới
              </button>
              <button className="flex items-center gap-2 bg-primary text-primary-foreground px-6 py-2.5 rounded-lg hover:bg-primary/90 transition-colors">
                <FileDown className="w-4 h-4" />
                Xuất file
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
