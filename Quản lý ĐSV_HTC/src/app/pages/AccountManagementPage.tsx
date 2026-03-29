import { useState } from "react";
import { Users, UserPlus, Shield, Lock, CheckCircle, XCircle } from "lucide-react";

const mockAccounts = [
  { id: 1, login: "admin01", name: "Nguyễn Văn A", role: "PGV", department: "Phòng Giáo Vụ", status: "active" },
  { id: 2, login: "gv001", name: "Trần Thị B", role: "KHOA", department: "CNTT", status: "active" },
  { id: 3, login: "gv002", name: "Lê Văn C", role: "KHOA", department: "CNTT", status: "active" },
  { id: 4, login: "sv001", name: "Phạm Thị D", role: "SV", department: "CNTT", status: "active" },
  { id: 5, login: "sv002", name: "Hoàng Văn E", role: "SV", department: "CNTT", status: "inactive" },
];

const permissions = {
  PGV: ["Dashboard", "Lớp", "Sinh viên", "Môn học", "Giảng viên", "Lớp tín chỉ", "Đăng ký tín chỉ", "Nhập điểm", "Báo cáo", "Tài khoản"],
  KHOA: ["Dashboard", "Sinh viên", "Môn học", "Giảng viên", "Lớp tín chỉ", "Nhập điểm", "Báo cáo"],
  SV: ["Dashboard", "Đăng ký tín chỉ", "Xem điểm"],
};

export function AccountManagementPage() {
  const [activeTab, setActiveTab] = useState<"list" | "create" | "permissions">("list");
  const [formData, setFormData] = useState({
    login: "",
    password: "",
    confirmPassword: "",
    role: "SV",
    department: "",
  });

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Quản trị tài khoản</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Quản lý tài khoản và phân quyền người dùng
        </p>
      </div>

      {/* Tabs */}
      <div className="bg-card border border-border rounded-xl overflow-hidden">
        <div className="flex border-b border-border">
          <button
            onClick={() => setActiveTab("list")}
            className={`flex items-center gap-2 px-6 py-4 font-medium transition-colors ${
              activeTab === "list"
                ? "bg-primary text-primary-foreground"
                : "text-muted-foreground hover:text-foreground hover:bg-muted/50"
            }`}
          >
            <Users className="w-5 h-5" />
            Danh sách tài khoản
          </button>
          <button
            onClick={() => setActiveTab("create")}
            className={`flex items-center gap-2 px-6 py-4 font-medium transition-colors ${
              activeTab === "create"
                ? "bg-primary text-primary-foreground"
                : "text-muted-foreground hover:text-foreground hover:bg-muted/50"
            }`}
          >
            <UserPlus className="w-5 h-5" />
            Tạo tài khoản
          </button>
          <button
            onClick={() => setActiveTab("permissions")}
            className={`flex items-center gap-2 px-6 py-4 font-medium transition-colors ${
              activeTab === "permissions"
                ? "bg-primary text-primary-foreground"
                : "text-muted-foreground hover:text-foreground hover:bg-muted/50"
            }`}
          >
            <Shield className="w-5 h-5" />
            Phân quyền
          </button>
        </div>

        <div className="p-6">
          {/* Account List Tab */}
          {activeTab === "list" && (
            <div className="overflow-hidden border border-border rounded-lg">
              <table className="w-full">
                <thead className="bg-muted">
                  <tr>
                    <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                      Login
                    </th>
                    <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                      Họ tên
                    </th>
                    <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                      Vai trò
                    </th>
                    <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                      Khoa/Phòng
                    </th>
                    <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                      Trạng thái
                    </th>
                    <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                      Thao tác
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {mockAccounts.map((account) => (
                    <tr
                      key={account.id}
                      className="border-t border-border hover:bg-muted/50 transition-colors"
                    >
                      <td className="px-4 py-3 text-sm text-foreground">{account.login}</td>
                      <td className="px-4 py-3 text-sm text-foreground">{account.name}</td>
                      <td className="px-4 py-3 text-sm">
                        <span
                          className={`px-2 py-1 text-xs font-semibold rounded ${
                            account.role === "PGV"
                              ? "bg-purple-100 text-purple-700"
                              : account.role === "KHOA"
                              ? "bg-blue-100 text-blue-700"
                              : "bg-green-100 text-green-700"
                          }`}
                        >
                          {account.role}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-sm text-foreground">{account.department}</td>
                      <td className="px-4 py-3 text-sm">
                        {account.status === "active" ? (
                          <span className="flex items-center gap-1 text-accent">
                            <CheckCircle className="w-4 h-4" />
                            Hoạt động
                          </span>
                        ) : (
                          <span className="flex items-center gap-1 text-destructive">
                            <XCircle className="w-4 h-4" />
                            Khóa
                          </span>
                        )}
                      </td>
                      <td className="px-4 py-3 text-sm">
                        <button className="text-primary hover:underline">Sửa</button>
                        <span className="mx-2">|</span>
                        <button className="text-destructive hover:underline">Xóa</button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {/* Create Account Tab */}
          {activeTab === "create" && (
            <div className="max-w-2xl">
              <h3 className="text-base font-semibold mb-6">Tạo tài khoản mới</h3>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium mb-2">Login</label>
                  <input
                    type="text"
                    value={formData.login}
                    onChange={(e) => setFormData({ ...formData, login: e.target.value })}
                    className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                    placeholder="Nhập tên đăng nhập"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium mb-2">Mật khẩu</label>
                  <div className="relative">
                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                    <input
                      type="password"
                      value={formData.password}
                      onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                      className="w-full pl-10 pr-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Nhập mật khẩu"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium mb-2">Xác nhận mật khẩu</label>
                  <div className="relative">
                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                    <input
                      type="password"
                      value={formData.confirmPassword}
                      onChange={(e) =>
                        setFormData({ ...formData, confirmPassword: e.target.value })
                      }
                      className="w-full pl-10 pr-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Nhập lại mật khẩu"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium mb-2">Vai trò</label>
                  <select
                    value={formData.role}
                    onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                    className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  >
                    <option value="PGV">PGV - Phòng Giáo Vụ</option>
                    <option value="KHOA">KHOA - Giảng viên Khoa</option>
                    <option value="SV">SV - Sinh viên</option>
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium mb-2">Khoa/Phòng</label>
                  <select
                    value={formData.department}
                    onChange={(e) => setFormData({ ...formData, department: e.target.value })}
                    className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  >
                    <option value="">Chọn khoa/phòng</option>
                    <option value="PGV">Phòng Giáo Vụ</option>
                    <option value="CNTT">Công nghệ thông tin</option>
                    <option value="KT">Kế toán</option>
                    <option value="QTKD">Quản trị kinh doanh</option>
                    <option value="MARKETING">Marketing</option>
                  </select>
                </div>

                <button className="w-full bg-primary text-primary-foreground py-2.5 rounded-lg font-medium hover:bg-primary/90 transition-colors mt-6">
                  Tạo tài khoản
                </button>
              </div>
            </div>
          )}

          {/* Permissions Tab */}
          {activeTab === "permissions" && (
            <div>
              <h3 className="text-base font-semibold mb-6">Ma trận phân quyền</h3>
              <div className="space-y-6">
                {Object.entries(permissions).map(([role, perms]) => (
                  <div key={role} className="border border-border rounded-lg p-6">
                    <div className="flex items-center gap-3 mb-4">
                      <Shield className="w-5 h-5 text-primary" />
                      <h4 className="font-semibold text-foreground">{role}</h4>
                      <span
                        className={`ml-auto px-3 py-1 text-xs font-semibold rounded ${
                          role === "PGV"
                            ? "bg-purple-100 text-purple-700"
                            : role === "KHOA"
                            ? "bg-blue-100 text-blue-700"
                            : "bg-green-100 text-green-700"
                        }`}
                      >
                        {perms.length} quyền
                      </span>
                    </div>
                    <div className="flex flex-wrap gap-2">
                      {perms.map((perm) => (
                        <span
                          key={perm}
                          className="px-3 py-1.5 bg-accent/10 text-accent border border-accent/20 rounded-lg text-sm"
                        >
                          {perm}
                        </span>
                      ))}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
