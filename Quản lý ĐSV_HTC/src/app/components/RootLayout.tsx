import { Outlet, NavLink, useNavigate } from "react-router";
import {
  LayoutDashboard,
  Users,
  BookOpen,
  UserCircle,
  GraduationCap,
  ClipboardList,
  FileText,
  FileBarChart,
  Settings,
  LogOut,
  Package,
  Calendar,
  Award,
  Bell,
  User,
  Lock,
} from "lucide-react";
import { useAuth } from "../contexts/AuthContext";

// Navigation for admin roles (PGV, KHOA)
const adminNavigationItems = [
  { path: "/", label: "Dashboard", icon: LayoutDashboard },
  { path: "/classes", label: "Lớp", icon: Users },
  { path: "/students", label: "Sinh viên", icon: GraduationCap },
  { path: "/subjects", label: "Môn học", icon: BookOpen },
  { path: "/lecturers", label: "Giảng viên", icon: UserCircle },
  { path: "/credit-classes", label: "Lớp tín chỉ", icon: ClipboardList },
  { path: "/registration", label: "Đăng ký tín chỉ", icon: FileText },
  { path: "/grades", label: "Nhập điểm", icon: FileBarChart },
  { path: "/reports", label: "Báo cáo / In ấn", icon: FileBarChart },
  { path: "/accounts", label: "Tài khoản / Phân quyền", icon: Settings },
  { path: "/components", label: "Component Library", icon: Package },
];

// Navigation for student role (SV)
const studentNavigationItems = [
  { path: "/", label: "Dashboard", icon: LayoutDashboard },
  { path: "/schedule", label: "Lịch học", icon: Calendar },
  { path: "/student-grades", label: "Điểm", icon: Award },
  { path: "/registration", label: "Đăng ký học phần", icon: FileText },
  { path: "/notifications", label: "Thông báo", icon: Bell },
];

const roles = [
  { id: "PGV", label: "PGV", color: "bg-purple-500" },
  { id: "KHOA", label: "KHOA", color: "bg-blue-500" },
  { id: "SV", label: "SV", color: "bg-green-500" },
];

export function RootLayout() {
  const navigate = useNavigate();
  const { currentRole, currentUser, logout } = useAuth();

  // Redirect to login if not authenticated
  if (!currentUser || !currentRole) {
    navigate("/login");
    return null;
  }

  // Select navigation items based on role
  const navigationItems =
    currentRole === "SV" ? studentNavigationItems : adminNavigationItems;

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="flex h-screen bg-background">
      {/* Sidebar */}
      <aside className="w-64 bg-sidebar border-r border-sidebar-border flex flex-col">
        <div className="p-6 border-b border-sidebar-border">
          <h2 className="text-lg font-bold text-primary">QLDSV_HTC</h2>
          <p className="text-xs text-muted-foreground mt-1">
            {currentRole === "SV"
              ? "Cổng thông tin sinh viên"
              : "Quản lý điểm sinh viên"}
          </p>
        </div>

        <nav className="flex-1 p-4 overflow-y-auto">
          <ul className="space-y-1">
            {navigationItems.map((item) => {
              const Icon = item.icon;
              return (
                <li key={item.path}>
                  <NavLink
                    to={item.path}
                    end={item.path === "/"}
                    className={({ isActive }) =>
                      `flex items-center gap-3 px-3 py-2.5 rounded-lg transition-colors ${
                        isActive
                          ? "bg-sidebar-primary text-sidebar-primary-foreground"
                          : "text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground"
                      }`
                    }
                  >
                    <Icon className="w-5 h-5" />
                    <span className="text-sm">{item.label}</span>
                  </NavLink>
                </li>
              );
            })}
          </ul>
        </nav>

        <div className="p-4 border-t border-sidebar-border">
          <div className="flex items-center gap-2 mb-3">
            {roles.map((role) => (
              <span
                key={role.id}
                className={`px-2 py-1 text-xs font-semibold text-white rounded ${role.color} ${
                  currentRole === role.id ? "ring-2 ring-offset-2 ring-primary" : "opacity-50"
                }`}
              >
                {role.label}
              </span>
            ))}
          </div>
          <button
            onClick={handleLogout}
            className="w-full flex items-center gap-2 px-3 py-2 text-sm text-destructive hover:bg-destructive/10 rounded-lg transition-colors"
          >
            <LogOut className="w-4 h-4" />
            Đăng xuất
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Header */}
        <header className="bg-card border-b border-border px-8 py-4 flex items-center justify-between">
          <div>
            <h1 className="text-xl font-semibold text-foreground">
              {currentRole === "SV"
                ? "Cổng thông tin sinh viên"
                : "Hệ thống quản lý điểm sinh viên theo hệ tín chỉ"}
            </h1>
          </div>
          <div className="flex items-center gap-4">
            {/* Quick Actions */}
            <NavLink
              to="/notifications"
              className={({ isActive }) =>
                `relative p-2 rounded-lg transition-colors ${
                  isActive
                    ? "bg-primary/10 text-primary"
                    : "text-muted-foreground hover:bg-muted hover:text-foreground"
                }`
              }
            >
              <Bell className="w-5 h-5" />
              {/* Notification Badge */}
              <span className="absolute top-1 right-1 w-2 h-2 bg-destructive rounded-full" />
            </NavLink>
            
            <NavLink
              to="/profile"
              className={({ isActive }) =>
                `flex items-center gap-3 p-2 rounded-lg transition-colors ${
                  isActive
                    ? "bg-primary/10"
                    : "hover:bg-muted"
                }`
              }
            >
              <div className="text-right">
                <p className="text-sm font-medium text-foreground">
                  {currentUser.name}
                </p>
                <p className="text-xs text-muted-foreground">
                  {currentRole === "SV"
                    ? `${currentUser.studentId} - ${currentUser.department}`
                    : `${currentUser.role} - ${currentUser.department}`}
                </p>
              </div>
              <div className="w-10 h-10 rounded-full bg-primary text-primary-foreground flex items-center justify-center font-semibold">
                {currentUser.name.charAt(0)}
              </div>
            </NavLink>
          </div>
        </header>

        {/* Page Content */}
        <main className="flex-1 overflow-y-auto p-8">
          <Outlet />
        </main>
      </div>
    </div>
  );
}