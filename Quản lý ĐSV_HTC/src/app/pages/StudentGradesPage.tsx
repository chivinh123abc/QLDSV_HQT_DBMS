import { BookOpen, TrendingUp, Award, FileText } from "lucide-react";
import { useState } from "react";

const semesterData = [
  {
    semester: "Học kỳ 1 - 2025-2026",
    courses: [
      {
        id: 1,
        code: "IT301",
        name: "Cơ sở dữ liệu",
        credits: 3,
        midterm: 8.5,
        final: 7.5,
        average: 7.8,
        letterGrade: "B+",
        status: "Đã có điểm",
      },
      {
        id: 2,
        code: "IT302",
        name: "Lập trình Web",
        credits: 3,
        midterm: 9.0,
        final: 8.5,
        average: 8.7,
        letterGrade: "A",
        status: "Đã có điểm",
      },
      {
        id: 3,
        code: "IT303",
        name: "Mạng máy tính",
        credits: 3,
        midterm: 7.0,
        final: null,
        average: null,
        letterGrade: null,
        status: "Chưa có điểm",
      },
      {
        id: 4,
        code: "IT304",
        name: "Công nghệ phần mềm",
        credits: 3,
        midterm: 8.0,
        final: null,
        average: null,
        letterGrade: null,
        status: "Chưa có điểm",
      },
      {
        id: 5,
        code: "IT305",
        name: "Trí tuệ nhân tạo",
        credits: 3,
        midterm: 9.5,
        final: null,
        average: null,
        letterGrade: null,
        status: "Chưa có điểm",
      },
      {
        id: 6,
        code: "EN201",
        name: "Anh văn chuyên ngành",
        credits: 3,
        midterm: 7.5,
        final: null,
        average: null,
        letterGrade: null,
        status: "Chưa có điểm",
      },
    ],
    gpa: 8.25,
    totalCredits: 18,
  },
  {
    semester: "Học kỳ 3 - 2024-2025",
    courses: [
      {
        id: 7,
        code: "IT201",
        name: "Cấu trúc dữ liệu và giải thuật",
        credits: 3,
        midterm: 8.0,
        final: 7.5,
        average: 7.7,
        letterGrade: "B+",
        status: "Đã có điểm",
      },
      {
        id: 8,
        code: "IT202",
        name: "Lập trình hướng đối tượng",
        credits: 3,
        midterm: 9.0,
        final: 8.0,
        average: 8.4,
        letterGrade: "A",
        status: "Đã có điểm",
      },
      {
        id: 9,
        code: "IT203",
        name: "Hệ điều hành",
        credits: 3,
        midterm: 7.5,
        final: 7.0,
        average: 7.2,
        letterGrade: "B",
        status: "Đã có điểm",
      },
    ],
    gpa: 7.76,
    totalCredits: 9,
  },
];

const cumulativeStats = {
  totalCredits: 87,
  completedCredits: 69,
  cumulativeGPA: 3.45,
  ranking: "Khá",
};

export function StudentGradesPage() {
  const [selectedSemester, setSelectedSemester] = useState(0);

  const getGradeColor = (letterGrade: string | null) => {
    if (!letterGrade) return "bg-gray-100 text-gray-700";
    if (letterGrade === "A" || letterGrade === "A+") return "bg-green-100 text-green-700";
    if (letterGrade === "B+" || letterGrade === "B") return "bg-blue-100 text-blue-700";
    if (letterGrade === "C+" || letterGrade === "C") return "bg-yellow-100 text-yellow-700";
    return "bg-red-100 text-red-700";
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-foreground">Kết quả học tập</h2>
          <p className="text-sm text-muted-foreground mt-1">
            Xem điểm các môn học và kết quả tích lũy
          </p>
        </div>
        <button className="bg-primary text-primary-foreground px-4 py-2 rounded-lg font-medium hover:opacity-90 transition-opacity flex items-center gap-2">
          <FileText className="w-4 h-4" />
          Xuất bảng điểm
        </button>
      </div>

      {/* Cumulative Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-3">
            <div className="bg-blue-500 w-12 h-12 rounded-lg flex items-center justify-center">
              <BookOpen className="w-6 h-6 text-white" />
            </div>
            <div>
              <p className="text-2xl font-bold text-foreground">
                {cumulativeStats.completedCredits}/{cumulativeStats.totalCredits}
              </p>
              <p className="text-sm text-muted-foreground">Tín chỉ tích lũy</p>
            </div>
          </div>
        </div>

        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-3">
            <div className="bg-green-500 w-12 h-12 rounded-lg flex items-center justify-center">
              <TrendingUp className="w-6 h-6 text-white" />
            </div>
            <div>
              <p className="text-2xl font-bold text-foreground">
                {cumulativeStats.cumulativeGPA}
              </p>
              <p className="text-sm text-muted-foreground">GPA tích lũy</p>
            </div>
          </div>
        </div>

        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-3">
            <div className="bg-purple-500 w-12 h-12 rounded-lg flex items-center justify-center">
              <Award className="w-6 h-6 text-white" />
            </div>
            <div>
              <p className="text-2xl font-bold text-foreground">
                {cumulativeStats.ranking}
              </p>
              <p className="text-sm text-muted-foreground">Xếp loại</p>
            </div>
          </div>
        </div>

        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-3">
            <div className="bg-orange-500 w-12 h-12 rounded-lg flex items-center justify-center">
              <FileText className="w-6 h-6 text-white" />
            </div>
            <div>
              <p className="text-2xl font-bold text-foreground">
                {semesterData.length}
              </p>
              <p className="text-sm text-muted-foreground">Học kỳ đã học</p>
            </div>
          </div>
        </div>
      </div>

      {/* Semester Tabs */}
      <div className="flex gap-2 overflow-x-auto pb-2">
        {semesterData.map((sem, index) => (
          <button
            key={index}
            onClick={() => setSelectedSemester(index)}
            className={`px-4 py-2 rounded-lg font-medium transition-colors whitespace-nowrap ${
              selectedSemester === index
                ? "bg-primary text-primary-foreground"
                : "bg-card border border-border text-foreground hover:bg-accent"
            }`}
          >
            {sem.semester}
          </button>
        ))}
      </div>

      {/* Semester Summary */}
      <div className="bg-gradient-to-r from-blue-500 to-blue-600 rounded-xl p-6 text-white">
        <h3 className="text-lg font-semibold mb-4">
          Tóm tắt {semesterData[selectedSemester].semester}
        </h3>
        <div className="grid grid-cols-3 gap-8">
          <div>
            <p className="text-blue-100 text-sm">Số môn học</p>
            <p className="text-2xl font-bold mt-1">
              {semesterData[selectedSemester].courses.length}
            </p>
          </div>
          <div>
            <p className="text-blue-100 text-sm">Tổng tín chỉ</p>
            <p className="text-2xl font-bold mt-1">
              {semesterData[selectedSemester].totalCredits}
            </p>
          </div>
          <div>
            <p className="text-blue-100 text-sm">GPA học kỳ</p>
            <p className="text-2xl font-bold mt-1">
              {semesterData[selectedSemester].gpa}
            </p>
          </div>
        </div>
      </div>

      {/* Grades Table */}
      <div className="bg-card border border-border rounded-xl overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-muted/50 border-b border-border">
              <tr>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">
                  Mã MH
                </th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">
                  Tên môn học
                </th>
                <th className="px-6 py-4 text-center text-sm font-semibold text-foreground">
                  Tín chỉ
                </th>
                <th className="px-6 py-4 text-center text-sm font-semibold text-foreground">
                  Điểm GK
                </th>
                <th className="px-6 py-4 text-center text-sm font-semibold text-foreground">
                  Điểm CK
                </th>
                <th className="px-6 py-4 text-center text-sm font-semibold text-foreground">
                  Điểm TB
                </th>
                <th className="px-6 py-4 text-center text-sm font-semibold text-foreground">
                  Điểm chữ
                </th>
                <th className="px-6 py-4 text-center text-sm font-semibold text-foreground">
                  Trạng thái
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-border">
              {semesterData[selectedSemester].courses.map((course) => (
                <tr key={course.id} className="hover:bg-muted/30 transition-colors">
                  <td className="px-6 py-4 text-sm font-medium text-foreground">
                    {course.code}
                  </td>
                  <td className="px-6 py-4 text-sm text-foreground">
                    {course.name}
                  </td>
                  <td className="px-6 py-4 text-sm text-center text-foreground">
                    {course.credits}
                  </td>
                  <td className="px-6 py-4 text-sm text-center text-foreground">
                    {course.midterm !== null ? course.midterm : "-"}
                  </td>
                  <td className="px-6 py-4 text-sm text-center text-foreground">
                    {course.final !== null ? course.final : "-"}
                  </td>
                  <td className="px-6 py-4 text-sm text-center">
                    <span className="font-semibold text-foreground">
                      {course.average !== null ? course.average : "-"}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-center">
                    {course.letterGrade ? (
                      <span
                        className={`px-2 py-1 text-xs font-semibold rounded-md ${getGradeColor(
                          course.letterGrade
                        )}`}
                      >
                        {course.letterGrade}
                      </span>
                    ) : (
                      "-"
                    )}
                  </td>
                  <td className="px-6 py-4 text-center">
                    <span
                      className={`px-2 py-1 text-xs font-medium rounded-md ${
                        course.status === "Đã có điểm"
                          ? "bg-green-100 text-green-700"
                          : "bg-yellow-100 text-yellow-700"
                      }`}
                    >
                      {course.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Notes */}
      <div className="bg-muted/50 border border-border rounded-xl p-6">
        <h4 className="font-semibold text-foreground mb-3">Ghi chú:</h4>
        <ul className="space-y-2 text-sm text-muted-foreground">
          <li>• Điểm GK: Điểm giữa kỳ</li>
          <li>• Điểm CK: Điểm cuối kỳ</li>
          <li>• Điểm TB: Điểm trung bình (GK × 0.3 + CK × 0.7)</li>
          <li>
            • Thang điểm chữ: A+ (9.0-10), A (8.5-8.9), B+ (8.0-8.4), B (7.0-7.9), C+
            (6.5-6.9), C (5.5-6.4), D+ (5.0-5.4), D (4.0-4.9), F (&lt;4.0)
          </li>
        </ul>
      </div>
    </div>
  );
}
