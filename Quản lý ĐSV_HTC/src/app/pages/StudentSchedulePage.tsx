import { Calendar, Clock, MapPin, User, BookOpen } from "lucide-react";
import { useState } from "react";

const weekDays = ["Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN"];

const scheduleData = [
  {
    day: "Thứ 2",
    classes: [
      {
        id: 1,
        subject: "Cơ sở dữ liệu",
        code: "IT301",
        room: "H301",
        time: "07:00 - 09:00",
        lecturer: "TS. Nguyễn Văn A",
        type: "Lý thuyết",
      },
      {
        id: 2,
        subject: "Lập trình Web",
        code: "IT302",
        room: "Lab 1",
        time: "13:30 - 15:30",
        lecturer: "ThS. Trần Thị B",
        type: "Thực hành",
      },
    ],
  },
  {
    day: "Thứ 3",
    classes: [
      {
        id: 3,
        subject: "Mạng máy tính",
        code: "IT303",
        room: "H205",
        time: "07:00 - 09:00",
        lecturer: "PGS.TS. Lê Văn C",
        type: "Lý thuyết",
      },
    ],
  },
  {
    day: "Thứ 4",
    classes: [
      {
        id: 4,
        subject: "Công nghệ phần mềm",
        code: "IT304",
        room: "H102",
        time: "09:30 - 11:30",
        lecturer: "ThS. Phạm Văn D",
        type: "Lý thuyết",
      },
      {
        id: 5,
        subject: "Lập trình Web",
        code: "IT302",
        room: "Lab 1",
        time: "13:30 - 15:30",
        lecturer: "ThS. Trần Thị B",
        type: "Thực hành",
      },
    ],
  },
  {
    day: "Thứ 5",
    classes: [
      {
        id: 6,
        subject: "Trí tuệ nhân tạo",
        code: "IT305",
        room: "H401",
        time: "07:00 - 09:00",
        lecturer: "PGS.TS. Hoàng Văn E",
        type: "Lý thuyết",
      },
      {
        id: 7,
        subject: "Mạng máy tính",
        code: "IT303",
        room: "Lab 2",
        time: "13:30 - 15:30",
        lecturer: "PGS.TS. Lê Văn C",
        type: "Thực hành",
      },
    ],
  },
  {
    day: "Thứ 6",
    classes: [
      {
        id: 8,
        subject: "Anh văn chuyên ngành",
        code: "EN201",
        room: "H203",
        time: "07:00 - 09:00",
        lecturer: "ThS. Võ Thị F",
        type: "Lý thuyết",
      },
    ],
  },
  {
    day: "Thứ 7",
    classes: [],
  },
  {
    day: "CN",
    classes: [],
  },
];

export function StudentSchedulePage() {
  const [viewMode, setViewMode] = useState<"week" | "list">("week");

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-foreground">Lịch học</h2>
          <p className="text-sm text-muted-foreground mt-1">
            Học kỳ 1 - Năm học 2025-2026
          </p>
        </div>
        <div className="flex items-center gap-3">
          <button
            onClick={() => setViewMode("week")}
            className={`px-4 py-2 rounded-lg font-medium transition-colors ${
              viewMode === "week"
                ? "bg-primary text-primary-foreground"
                : "bg-card border border-border text-foreground hover:bg-accent"
            }`}
          >
            Xem theo tuần
          </button>
          <button
            onClick={() => setViewMode("list")}
            className={`px-4 py-2 rounded-lg font-medium transition-colors ${
              viewMode === "list"
                ? "bg-primary text-primary-foreground"
                : "bg-card border border-border text-foreground hover:bg-accent"
            }`}
          >
            Xem danh sách
          </button>
        </div>
      </div>

      {/* Week View */}
      {viewMode === "week" && (
        <div className="grid grid-cols-7 gap-4">
          {scheduleData.map((dayData, index) => (
            <div
              key={index}
              className="bg-card border border-border rounded-xl overflow-hidden"
            >
              <div className="bg-primary/10 px-3 py-2 border-b border-border">
                <p className="text-sm font-semibold text-center text-foreground">
                  {dayData.day}
                </p>
              </div>
              <div className="p-3 space-y-2 min-h-[400px]">
                {dayData.classes.map((classItem) => (
                  <div
                    key={classItem.id}
                    className="bg-accent/50 border border-border rounded-lg p-2 cursor-pointer hover:bg-accent transition-colors"
                  >
                    <div className="flex items-start justify-between mb-1">
                      <h4 className="font-medium text-xs text-foreground leading-tight">
                        {classItem.subject}
                      </h4>
                      <span
                        className={`px-1.5 py-0.5 text-[10px] rounded flex-shrink-0 ml-1 ${
                          classItem.type === "Lý thuyết"
                            ? "bg-blue-100 text-blue-700"
                            : "bg-green-100 text-green-700"
                        }`}
                      >
                        {classItem.type === "Lý thuyết" ? "LT" : "TH"}
                      </span>
                    </div>
                    <p className="text-[10px] text-muted-foreground mb-1">
                      {classItem.code}
                    </p>
                    <div className="space-y-0.5">
                      <div className="flex items-center gap-1 text-[10px] text-muted-foreground">
                        <Clock className="w-3 h-3" />
                        <span>{classItem.time}</span>
                      </div>
                      <div className="flex items-center gap-1 text-[10px] text-muted-foreground">
                        <MapPin className="w-3 h-3" />
                        <span>{classItem.room}</span>
                      </div>
                    </div>
                  </div>
                ))}
                {dayData.classes.length === 0 && (
                  <div className="flex items-center justify-center h-32 text-muted-foreground">
                    <p className="text-xs">Không có lịch</p>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* List View */}
      {viewMode === "list" && (
        <div className="space-y-4">
          {scheduleData.map((dayData, index) => (
            <div key={index} className="bg-card border border-border rounded-xl p-6">
              <h3 className="text-lg font-semibold mb-4 flex items-center gap-2">
                <Calendar className="w-5 h-5 text-primary" />
                {dayData.day}
              </h3>
              {dayData.classes.length > 0 ? (
                <div className="space-y-3">
                  {dayData.classes.map((classItem) => (
                    <div
                      key={classItem.id}
                      className="border border-border rounded-lg p-4 hover:bg-accent/50 transition-colors"
                    >
                      <div className="flex items-start justify-between mb-2">
                        <div>
                          <h4 className="font-semibold text-foreground">
                            {classItem.subject}
                          </h4>
                          <p className="text-sm text-muted-foreground">
                            {classItem.code}
                          </p>
                        </div>
                        <span
                          className={`px-2 py-1 text-xs rounded-md ${
                            classItem.type === "Lý thuyết"
                              ? "bg-blue-100 text-blue-700"
                              : "bg-green-100 text-green-700"
                          }`}
                        >
                          {classItem.type}
                        </span>
                      </div>

                      <div className="grid grid-cols-3 gap-4 mt-3">
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <Clock className="w-4 h-4" />
                          <span>{classItem.time}</span>
                        </div>
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <MapPin className="w-4 h-4" />
                          <span>Phòng {classItem.room}</span>
                        </div>
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <User className="w-4 h-4" />
                          <span>{classItem.lecturer}</span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="text-center py-8 text-muted-foreground">
                  <BookOpen className="w-12 h-12 mx-auto mb-2 opacity-50" />
                  <p>Không có lịch học</p>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
