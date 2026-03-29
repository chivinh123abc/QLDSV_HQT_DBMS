import { CheckCircle, XCircle, AlertCircle, Info, Users, BookOpen, Calendar } from "lucide-react";
import { useAuth } from "../contexts/AuthContext";

export function ComponentLibraryPage() {
  const { currentUser, currentRole } = useAuth();

  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Component Library & Testing Guide</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Thư viện các component UI và hướng dẫn test các role khác nhau
        </p>
      </div>

      {/* Current User Info */}
      {currentUser && currentRole && (
        <section className="bg-gradient-to-r from-green-50 to-emerald-50 border-2 border-green-200 rounded-xl p-6">
          <h3 className="text-lg font-semibold mb-3 text-green-900">
            ✅ Đang đăng nhập với tài khoản:
          </h3>
          <div className="bg-white rounded-lg p-4 border border-green-200">
            <div className="flex items-center gap-4">
              <div
                className={`px-3 py-1.5 text-sm font-semibold text-white rounded ${
                  currentRole === "SV"
                    ? "bg-green-500"
                    : currentRole === "PGV"
                    ? "bg-purple-500"
                    : "bg-blue-500"
                }`}
              >
                {currentRole}
              </div>
              <div>
                <p className="font-semibold text-foreground">{currentUser.name}</p>
                <p className="text-sm text-muted-foreground">
                  {currentUser.studentId
                    ? `Mã SV: ${currentUser.studentId} - ${currentUser.department}`
                    : currentUser.department}
                </p>
              </div>
            </div>
          </div>
        </section>
      )}

      {/* Role Testing Guide */}
      <section className="bg-gradient-to-br from-blue-50 to-indigo-50 border-2 border-primary/20 rounded-xl p-6">
        <h3 className="text-lg font-semibold mb-4 flex items-center gap-2">
          <Info className="w-5 h-5 text-primary" />
          Hướng dẫn test các Role trong hệ thống
        </h3>
        <div className="space-y-4">
          <div className="bg-white rounded-lg p-4 border border-border">
            <p className="text-sm font-semibold text-foreground mb-3">
              Tài khoản test (dùng để đăng nhập):
            </p>
            
            <div className="space-y-3">
              <div className="flex items-start gap-3 bg-green-50 p-3 rounded-lg">
                <div className="bg-green-500 px-2 py-1 rounded text-white text-xs font-semibold flex-shrink-0">
                  SV
                </div>
                <div className="flex-1">
                  <p className="font-medium text-sm">Sinh viên (Student Portal)</p>
                  <p className="text-xs text-muted-foreground mb-2">
                    Dashboard sinh viên, Lịch học, Điểm, Đăng ký học phần
                  </p>
                  <div className="bg-white p-2 rounded border border-green-200">
                    <p className="text-xs">
                      <span className="font-semibold">Username:</span> <code className="font-mono bg-gray-100 px-1.5 py-0.5 rounded">sv</code>
                      {" | "}
                      <span className="font-semibold">Password:</span> <code className="font-mono bg-gray-100 px-1.5 py-0.5 rounded">sv</code>
                    </p>
                  </div>
                </div>
              </div>
              
              <div className="flex items-start gap-3 bg-purple-50 p-3 rounded-lg">
                <div className="bg-purple-500 px-2 py-1 rounded text-white text-xs font-semibold flex-shrink-0">
                  PGV
                </div>
                <div className="flex-1">
                  <p className="font-medium text-sm">Phòng Giáo Vụ (Academic Affairs Admin)</p>
                  <p className="text-xs text-muted-foreground mb-2">
                    Full quản lý: Lớp, Sinh viên, Môn học, Giảng viên, Lớp tín chỉ, Nhập điểm, Báo cáo
                  </p>
                  <div className="bg-white p-2 rounded border border-purple-200">
                    <p className="text-xs">
                      <span className="font-semibold">Username:</span> <code className="font-mono bg-gray-100 px-1.5 py-0.5 rounded">pgv</code>
                      {" | "}
                      <span className="font-semibold">Password:</span> <code className="font-mono bg-gray-100 px-1.5 py-0.5 rounded">pgv</code>
                    </p>
                  </div>
                </div>
              </div>
              
              <div className="flex items-start gap-3 bg-blue-50 p-3 rounded-lg">
                <div className="bg-blue-500 px-2 py-1 rounded text-white text-xs font-semibold flex-shrink-0">
                  KHOA
                </div>
                <div className="flex-1">
                  <p className="font-medium text-sm">Khoa (Faculty Admin)</p>
                  <p className="text-xs text-muted-foreground mb-2">
                    Full quản lý: Lớp, Sinh viên, Môn học, Giảng viên, Lớp tín chỉ, Nhập điểm, Báo cáo
                  </p>
                  <div className="bg-white p-2 rounded border border-blue-200">
                    <p className="text-xs">
                      <span className="font-semibold">Username:</span> <code className="font-mono bg-gray-100 px-1.5 py-0.5 rounded">khoa</code>
                      {" | "}
                      <span className="font-semibold">Password:</span> <code className="font-mono bg-gray-100 px-1.5 py-0.5 rounded">khoa</code>
                    </p>
                  </div>
                </div>
              </div>
            </div>
            
            <div className="mt-4 pt-4 border-t border-gray-200">
              <p className="text-xs text-muted-foreground">
                💡 Để test: Đăng xuất khỏi hệ thống và đăng nhập lại với các tài khoản khác nhau để xem giao diện tương ứng với từng role.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Buttons */}
      <section className="bg-card border border-border rounded-xl p-6">
        <h3 className="text-lg font-semibold mb-4">Buttons</h3>
        <div className="flex flex-wrap gap-3">
          <button className="bg-primary text-primary-foreground px-4 py-2 rounded-lg hover:bg-primary/90 transition-colors">
            Primary
          </button>
          <button className="bg-secondary text-secondary-foreground px-4 py-2 rounded-lg hover:bg-secondary/80 transition-colors">
            Secondary
          </button>
          <button className="bg-accent text-accent-foreground px-4 py-2 rounded-lg hover:bg-accent/90 transition-colors">
            Success
          </button>
          <button className="bg-destructive text-destructive-foreground px-4 py-2 rounded-lg hover:bg-destructive/90 transition-colors">
            Danger
          </button>
          <button className="bg-warning text-warning-foreground px-4 py-2 rounded-lg hover:bg-warning/90 transition-colors">
            Warning
          </button>
          <button className="bg-muted text-muted-foreground px-4 py-2 rounded-lg cursor-not-allowed" disabled>
            Disabled
          </button>
        </div>
      </section>

      {/* Inputs */}
      <section className="bg-card border border-border rounded-xl p-6">
        <h3 className="text-lg font-semibold mb-4">Inputs</h3>
        <div className="grid grid-cols-2 gap-4 max-w-2xl">
          <div>
            <label className="block text-sm font-medium mb-2">Text Input</label>
            <input
              type="text"
              placeholder="Enter text..."
              className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Password Input</label>
            <input
              type="password"
              placeholder="Enter password..."
              className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Dropdown</label>
            <select className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary">
              <option>Option 1</option>
              <option>Option 2</option>
              <option>Option 3</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Date Picker</label>
            <input
              type="date"
              className="w-full px-4 py-2 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </div>
        <div className="mt-4 space-y-3 max-w-md">
          <label className="flex items-center gap-2">
            <input type="checkbox" className="w-4 h-4 text-primary rounded" />
            <span className="text-sm">Checkbox option</span>
          </label>
          <div className="flex gap-4">
            <label className="flex items-center gap-2">
              <input type="radio" name="radio" className="w-4 h-4 text-primary" />
              <span className="text-sm">Radio option 1</span>
            </label>
            <label className="flex items-center gap-2">
              <input type="radio" name="radio" className="w-4 h-4 text-primary" />
              <span className="text-sm">Radio option 2</span>
            </label>
          </div>
        </div>
      </section>

      {/* Status Badges */}
      <section className="bg-card border border-border rounded-xl p-6">
        <h3 className="text-lg font-semibold mb-4">Status Badges</h3>
        <div className="flex flex-wrap gap-3">
          <span className="px-3 py-1.5 bg-accent/10 text-accent border border-accent/20 rounded-full text-sm font-medium">
            Active
          </span>
          <span className="px-3 py-1.5 bg-destructive/10 text-destructive border border-destructive/20 rounded-full text-sm font-medium">
            Cancelled
          </span>
          <span className="px-3 py-1.5 bg-warning/10 text-warning border border-warning/20 rounded-full text-sm font-medium">
            Pending
          </span>
          <span className="px-3 py-1.5 bg-muted text-muted-foreground border border-border rounded-full text-sm font-medium">
            Disabled
          </span>
          <span className="px-3 py-1.5 bg-blue-100 text-blue-700 border border-blue-200 rounded-full text-sm font-medium">
            In Progress
          </span>
        </div>
      </section>

      {/* Table States */}
      <section className="bg-card border border-border rounded-xl p-6">
        <h3 className="text-lg font-semibold mb-4">Table Row States</h3>
        <div className="overflow-hidden border border-border rounded-lg">
          <table className="w-full">
            <thead className="bg-muted">
              <tr>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                  Column 1
                </th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                  Column 2
                </th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-foreground">
                  Column 3
                </th>
              </tr>
            </thead>
            <tbody>
              <tr className="border-t border-border">
                <td className="px-4 py-3 text-sm text-foreground">Default row</td>
                <td className="px-4 py-3 text-sm text-foreground">Normal state</td>
                <td className="px-4 py-3 text-sm text-foreground">Data</td>
              </tr>
              <tr className="border-t border-border bg-muted/50">
                <td className="px-4 py-3 text-sm text-foreground">Hover state</td>
                <td className="px-4 py-3 text-sm text-foreground">On mouse over</td>
                <td className="px-4 py-3 text-sm text-foreground">Data</td>
              </tr>
              <tr className="border-t border-border bg-primary/5">
                <td className="px-4 py-3 text-sm text-foreground">Selected row</td>
                <td className="px-4 py-3 text-sm text-foreground">Currently selected</td>
                <td className="px-4 py-3 text-sm text-foreground">Data</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      {/* Alerts */}
      <section className="bg-card border border-border rounded-xl p-6">
        <h3 className="text-lg font-semibold mb-4">Alerts / Notifications</h3>
        <div className="space-y-3 max-w-2xl">
          <div className="p-4 bg-accent/10 border border-accent/20 rounded-lg flex items-start gap-3">
            <CheckCircle className="w-5 h-5 text-accent flex-shrink-0 mt-0.5" />
            <div>
              <p className="font-medium text-accent">Success</p>
              <p className="text-sm text-foreground mt-1">
                Thao tác đã được thực hiện thành công!
              </p>
            </div>
          </div>
          <div className="p-4 bg-destructive/10 border border-destructive/20 rounded-lg flex items-start gap-3">
            <XCircle className="w-5 h-5 text-destructive flex-shrink-0 mt-0.5" />
            <div>
              <p className="font-medium text-destructive">Error</p>
              <p className="text-sm text-foreground mt-1">
                Đã có lỗi xảy ra. Vui lòng thử lại!
              </p>
            </div>
          </div>
          <div className="p-4 bg-warning/10 border border-warning/20 rounded-lg flex items-start gap-3">
            <AlertCircle className="w-5 h-5 text-warning flex-shrink-0 mt-0.5" />
            <div>
              <p className="font-medium text-warning">Warning</p>
              <p className="text-sm text-foreground mt-1">
                Thông tin này cần được kiểm tra lại!
              </p>
            </div>
          </div>
          <div className="p-4 bg-blue-50 border border-blue-200 rounded-lg flex items-start gap-3">
            <Info className="w-5 h-5 text-blue-600 flex-shrink-0 mt-0.5" />
            <div>
              <p className="font-medium text-blue-900">Info</p>
              <p className="text-sm text-foreground mt-1">
                Đây là thông tin hướng dẫn cho người dùng.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Cards */}
      <section className="bg-card border border-border rounded-xl p-6">
        <h3 className="text-lg font-semibold mb-4">Cards</h3>
        <div className="grid grid-cols-3 gap-4">
          <div className="bg-card border border-border rounded-xl p-6 hover:shadow-md transition-shadow">
            <h4 className="font-semibold text-foreground mb-2">Basic Card</h4>
            <p className="text-sm text-muted-foreground">
              This is a basic card component with border and hover effect.
            </p>
          </div>
          <div className="bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl p-6 text-white">
            <h4 className="font-semibold mb-2">Gradient Card</h4>
            <p className="text-sm text-blue-100">
              Card with gradient background for highlighting.
            </p>
          </div>
          <div className="bg-muted/20 border border-border rounded-xl p-6">
            <h4 className="font-semibold text-foreground mb-2">Muted Card</h4>
            <p className="text-sm text-muted-foreground">
              Card with subtle background color.
            </p>
          </div>
        </div>
      </section>

      {/* Role Badges */}
      <section className="bg-card border border-border rounded-xl p-6">
        <h3 className="text-lg font-semibold mb-4">Role Badges</h3>
        <div className="flex gap-3">
          <span className="px-3 py-1.5 text-sm font-semibold text-white bg-purple-500 rounded">
            PGV
          </span>
          <span className="px-3 py-1.5 text-sm font-semibold text-white bg-blue-500 rounded">
            KHOA
          </span>
          <span className="px-3 py-1.5 text-sm font-semibold text-white bg-green-500 rounded">
            SV
          </span>
        </div>
      </section>
    </div>
  );
}