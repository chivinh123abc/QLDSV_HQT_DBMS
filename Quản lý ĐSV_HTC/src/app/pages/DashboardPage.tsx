import { Users, BookOpen, GraduationCap, ClipboardList, UserCheck, Clock } from "lucide-react";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts";
import { useAuth } from "../contexts/AuthContext";
import { StudentDashboardPage } from "./StudentDashboardPage";

const stats = [
  { label: "Tổng số sinh viên", value: "2,450", icon: GraduationCap, color: "bg-blue-500" },
  { label: "Tổng số lớp", value: "85", icon: Users, color: "bg-purple-500" },
  { label: "Tổng số môn học", value: "156", icon: BookOpen, color: "bg-green-500" },
  { label: "Tổng số lớp tín chỉ", value: "245", icon: ClipboardList, color: "bg-orange-500" },
  { label: "Lớp tín chỉ đang mở", value: "42", icon: Clock, color: "bg-teal-500" },
  { label: "Sinh viên đang đăng ký", value: "1,823", icon: UserCheck, color: "bg-pink-500" },
];

const quickActions = [
  { label: "Thêm lớp", color: "bg-blue-500", path: "/classes" },
  { label: "Thêm sinh viên", color: "bg-green-500", path: "/students" },
  { label: "Mở lớp tín chỉ", color: "bg-purple-500", path: "/credit-classes" },
  { label: "Nhập điểm", color: "bg-orange-500", path: "/grades" },
  { label: "Tạo tài khoản", color: "bg-teal-500", path: "/accounts" },
];

const chartData = [
  { name: "HK1", students: 1850 },
  { name: "HK2", students: 1920 },
  { name: "HK3", students: 1650 },
  { name: "HK1", students: 1980 },
  { name: "HK2", students: 2100 },
  { name: "HK3", students: 1823 },
];

const recentActivities = [
  {
    user: "Nguyễn Văn A",
    action: "đã nhập điểm cho lớp",
    target: "LTC001 - Cơ sở dữ liệu",
    time: "5 phút trước",
  },
  {
    user: "Trần Thị B",
    action: "đã đăng ký lớp tín chỉ",
    target: "LTC045 - Lập trình Web",
    time: "15 phút trước",
  },
  {
    user: "Lê Văn C",
    action: "đã tạo lớp mới",
    target: "DH20CNTT01",
    time: "1 giờ trước",
  },
  {
    user: "Phạm Thị D",
    action: "đã cập nhật thông tin sinh viên",
    target: "SV001 - Nguyễn Văn E",
    time: "2 giờ trước",
  },
];

export function DashboardPage() {
  const { currentRole, currentUser } = useAuth();

  // If student role, show student dashboard
  if (currentRole === "SV") {
    return <StudentDashboardPage />;
  }

  // If no user authenticated, return null (will be redirected by RootLayout)
  if (!currentUser || !currentRole) {
    return null;
  }

  // Otherwise show admin dashboard
  return (
    <div className="space-y-8">
      {/* Welcome Card */}
      <div className="bg-gradient-to-r from-blue-500 to-blue-600 rounded-2xl p-8 text-white">
        <h2 className="text-2xl font-bold mb-2">Chào mừng trở lại!</h2>
        <div className="flex items-center gap-8 mt-4">
          <div>
            <p className="text-blue-100 text-sm">Họ và tên</p>
            <p className="font-semibold">{currentUser.name}</p>
          </div>
          <div>
            <p className="text-blue-100 text-sm">Vai trò</p>
            <p className="font-semibold">
              {currentRole === "PGV" ? "Phòng Giáo Vụ (PGV)" : "Khoa (KHOA)"}
            </p>
          </div>
          <div>
            <p className="text-blue-100 text-sm">Khoa</p>
            <p className="font-semibold">{currentUser.department}</p>
          </div>
        </div>
      </div>

      {/* Statistics */}
      <div>
        <h3 className="text-lg font-semibold mb-4">Thống kê tổng quan</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {stats.map((stat, index) => {
            const Icon = stat.icon;
            return (
              <div
                key={index}
                className="bg-card border border-border rounded-xl p-6 flex items-center gap-4 hover:shadow-md transition-shadow"
              >
                <div className={`${stat.color} w-14 h-14 rounded-lg flex items-center justify-center`}>
                  <Icon className="w-7 h-7 text-white" />
                </div>
                <div>
                  <p className="text-2xl font-bold text-foreground">{stat.value}</p>
                  <p className="text-sm text-muted-foreground">{stat.label}</p>
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {/* Quick Actions */}
      <div>
        <h3 className="text-lg font-semibold mb-4">Thao tác nhanh</h3>
        <div className="flex flex-wrap gap-3">
          {quickActions.map((action, index) => (
            <button
              key={index}
              className={`${action.color} text-white px-6 py-3 rounded-lg font-medium hover:opacity-90 transition-opacity`}
            >
              {action.label}
            </button>
          ))}
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Chart */}
        <div className="bg-card border border-border rounded-xl p-6">
          <h3 className="text-lg font-semibold mb-4">Thống kê đăng ký theo học kỳ</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={chartData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#E2E8F0" />
              <XAxis dataKey="name" stroke="#64748B" />
              <YAxis stroke="#64748B" />
              <Tooltip />
              <Bar dataKey="students" fill="#2563EB" radius={[8, 8, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Recent Activities */}
        <div className="bg-card border border-border rounded-xl p-6">
          <h3 className="text-lg font-semibold mb-4">Hoạt động gần đây</h3>
          <div className="space-y-4">
            {recentActivities.map((activity, index) => (
              <div key={index} className="flex items-start gap-3 pb-4 border-b border-border last:border-0">
                <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                  <span className="text-xs font-semibold text-primary">
                    {activity.user.charAt(0)}
                  </span>
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm text-foreground">
                    <span className="font-medium">{activity.user}</span>{" "}
                    <span className="text-muted-foreground">{activity.action}</span>{" "}
                    <span className="font-medium">{activity.target}</span>
                  </p>
                  <p className="text-xs text-muted-foreground mt-1">{activity.time}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}