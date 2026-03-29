import { Calendar, Bell, BookOpen, Clock, MapPin, User, FileText } from "lucide-react";

const upcomingClasses = [
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
    time: "09:30 - 11:30",
    lecturer: "ThS. Trần Thị B",
    type: "Thực hành",
  },
  {
    id: 3,
    subject: "Mạng máy tính",
    code: "IT303",
    room: "H205",
    time: "13:30 - 15:30",
    lecturer: "PGS.TS. Lê Văn C",
    type: "Lý thuyết",
  },
];

const announcements = [
  {
    id: 1,
    title: "Thông báo lịch thi giữa kỳ học kỳ 1 năm 2026",
    date: "28/03/2026",
    category: "Thi cử",
    excerpt: "Nhà trường thông báo lịch thi giữa kỳ cho các lớp tín chỉ sẽ diễn ra từ ngày 05/04/2026..."
  },
  {
    id: 2,
    title: "Mở đăng ký học phần học kỳ 2 năm học 2025-2026",
    date: "27/03/2026",
    category: "Đăng ký",
    excerpt: "Phòng Đào tạo thông báo sinh viên đăng ký học phần từ 01/04 đến 10/04/2026..."
  },
  {
    id: 3,
    title: "Hướng dẫn đóng học phí học kỳ 1",
    date: "26/03/2026",
    category: "Học phí",
    excerpt: "Sinh viên có thể đóng học phí qua hệ thống ngân hàng hoặc tại phòng Kế toán..."
  },
  {
    id: 4,
    title: "Lịch học bù môn Cơ sở dữ liệu",
    date: "25/03/2026",
    category: "Học tập",
    excerpt: "Lớp IT301 sẽ có buổi học bù vào thứ 7 ngày 30/03/2026 tại phòng H301..."
  },
];

const currentSemesterInfo = {
  semester: "Học kỳ 1",
  year: "2025-2026",
  totalCredits: 18,
  registeredCourses: 6,
  gpa: "3.45",
};

export function StudentDashboardPage() {
  return (
    <div className="space-y-8">
      {/* Welcome Card */}
      <div className="bg-gradient-to-r from-blue-500 to-blue-600 rounded-2xl p-8 text-white">
        <h2 className="text-2xl font-bold mb-2">Chào mừng trở lại!</h2>
        <div className="flex items-center gap-8 mt-4">
          <div>
            <p className="text-blue-100 text-sm">Họ và tên</p>
            <p className="font-semibold">Nguyễn Văn Minh</p>
          </div>
          <div>
            <p className="text-blue-100 text-sm">Mã sinh viên</p>
            <p className="font-semibold">2051052001</p>
          </div>
          <div>
            <p className="text-blue-100 text-sm">Lớp</p>
            <p className="font-semibold">DH20CNTT01</p>
          </div>
          <div>
            <p className="text-blue-100 text-sm">Khoa</p>
            <p className="font-semibold">Công Nghệ Thông Tin</p>
          </div>
        </div>
      </div>

      {/* Current Semester Info */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-3">
            <div className="bg-blue-500 w-12 h-12 rounded-lg flex items-center justify-center">
              <Calendar className="w-6 h-6 text-white" />
            </div>
            <div>
              <p className="text-2xl font-bold text-foreground">{currentSemesterInfo.semester}</p>
              <p className="text-sm text-muted-foreground">{currentSemesterInfo.year}</p>
            </div>
          </div>
        </div>

        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-3">
            <div className="bg-green-500 w-12 h-12 rounded-lg flex items-center justify-center">
              <BookOpen className="w-6 h-6 text-white" />
            </div>
            <div>
              <p className="text-2xl font-bold text-foreground">{currentSemesterInfo.registeredCourses}</p>
              <p className="text-sm text-muted-foreground">Môn đang học</p>
            </div>
          </div>
        </div>

        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-3">
            <div className="bg-purple-500 w-12 h-12 rounded-lg flex items-center justify-center">
              <FileText className="w-6 h-6 text-white" />
            </div>
            <div>
              <p className="text-2xl font-bold text-foreground">{currentSemesterInfo.totalCredits}</p>
              <p className="text-sm text-muted-foreground">Tổng tín chỉ</p>
            </div>
          </div>
        </div>

        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-3">
            <div className="bg-orange-500 w-12 h-12 rounded-lg flex items-center justify-center">
              <User className="w-6 h-6 text-white" />
            </div>
            <div>
              <p className="text-2xl font-bold text-foreground">{currentSemesterInfo.gpa}</p>
              <p className="text-sm text-muted-foreground">GPA tích lũy</p>
            </div>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Upcoming Classes Today */}
        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-lg font-semibold flex items-center gap-2">
              <Clock className="w-5 h-5 text-primary" />
              Lịch học hôm nay
            </h3>
            <span className="text-xs text-muted-foreground">Thứ 7, 28/03/2026</span>
          </div>
          
          <div className="space-y-4">
            {upcomingClasses.map((classItem) => (
              <div
                key={classItem.id}
                className="border border-border rounded-lg p-4 hover:bg-accent/50 transition-colors"
              >
                <div className="flex items-start justify-between mb-2">
                  <div>
                    <h4 className="font-semibold text-foreground">{classItem.subject}</h4>
                    <p className="text-sm text-muted-foreground">{classItem.code}</p>
                  </div>
                  <span className={`px-2 py-1 text-xs rounded-md ${
                    classItem.type === "Lý thuyết" 
                      ? "bg-blue-100 text-blue-700" 
                      : "bg-green-100 text-green-700"
                  }`}>
                    {classItem.type}
                  </span>
                </div>
                
                <div className="space-y-1.5 mt-3">
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

          {upcomingClasses.length === 0 && (
            <div className="text-center py-8 text-muted-foreground">
              <Calendar className="w-12 h-12 mx-auto mb-2 opacity-50" />
              <p>Không có lịch học hôm nay</p>
            </div>
          )}
        </div>

        {/* Recent Announcements */}
        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-lg font-semibold flex items-center gap-2">
              <Bell className="w-5 h-5 text-primary" />
              Thông báo mới
            </h3>
            <button className="text-xs text-primary hover:underline">
              Xem tất cả
            </button>
          </div>
          
          <div className="space-y-3 max-h-[500px] overflow-y-auto">
            {announcements.map((announcement) => (
              <div
                key={announcement.id}
                className="border border-border rounded-lg p-4 hover:bg-accent/50 transition-colors cursor-pointer"
              >
                <div className="flex items-start gap-3">
                  <div className="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                    <Bell className="w-5 h-5 text-primary" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-2 mb-1">
                      <h4 className="font-medium text-sm text-foreground leading-tight">
                        {announcement.title}
                      </h4>
                      <span className="px-2 py-0.5 text-xs rounded-md bg-blue-100 text-blue-700 flex-shrink-0">
                        {announcement.category}
                      </span>
                    </div>
                    <p className="text-xs text-muted-foreground line-clamp-2 mb-2">
                      {announcement.excerpt}
                    </p>
                    <p className="text-xs text-muted-foreground">
                      {announcement.date}
                    </p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
