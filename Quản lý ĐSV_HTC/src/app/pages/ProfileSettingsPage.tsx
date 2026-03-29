import { useState } from "react";
import { User, Mail, Phone, MapPin, Calendar, Building, Save, Camera } from "lucide-react";
import { useAuth } from "../contexts/AuthContext";
import { useNavigate } from "react-router";

export function ProfileSettingsPage() {
  const { currentUser, currentRole } = useAuth();
  const navigate = useNavigate();
  const [isEditing, setIsEditing] = useState(false);
  const [showSuccessMessage, setShowSuccessMessage] = useState(false);

  const [formData, setFormData] = useState({
    name: currentUser?.name || "",
    email: currentUser?.studentId ? `${currentUser.studentId}@student.edu.vn` : "admin@university.edu.vn",
    phone: "0123456789",
    address: "123 Nguyễn Văn Linh, Q.7, TP.HCM",
    dateOfBirth: "2000-01-15",
    gender: "Nam",
    department: currentUser?.department || "",
    studentId: currentUser?.studentId || "",
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Simulate save
    setIsEditing(false);
    setShowSuccessMessage(true);
    setTimeout(() => setShowSuccessMessage(false), 3000);
  };

  const handleCancel = () => {
    setIsEditing(false);
    // Reset form data
    setFormData({
      name: currentUser?.name || "",
      email: currentUser?.studentId ? `${currentUser.studentId}@student.edu.vn` : "admin@university.edu.vn",
      phone: "0123456789",
      address: "123 Nguyễn Văn Linh, Q.7, TP.HCM",
      dateOfBirth: "2000-01-15",
      gender: "Nam",
      department: currentUser?.department || "",
      studentId: currentUser?.studentId || "",
    });
  };

  return (
    <div className="max-w-5xl space-y-8">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Thông tin cá nhân</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Quản lý thông tin tài khoản và cài đặt cá nhân của bạn
        </p>
      </div>

      {/* Success Message */}
      {showSuccessMessage && (
        <div className="bg-accent/10 border border-accent/20 rounded-xl p-4 flex items-center gap-3">
          <div className="w-8 h-8 rounded-full bg-accent flex items-center justify-center">
            <Save className="w-4 h-4 text-white" />
          </div>
          <div>
            <p className="font-medium text-accent">Lưu thành công!</p>
            <p className="text-sm text-muted-foreground">Thông tin của bạn đã được cập nhật.</p>
          </div>
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Avatar Section */}
        <div className="lg:col-span-1">
          <div className="bg-card border border-border rounded-xl p-6 sticky top-8">
            <div className="text-center">
              <div className="relative inline-block">
                <div className="w-32 h-32 rounded-full bg-gradient-to-br from-primary to-blue-600 flex items-center justify-center text-white text-4xl font-bold mx-auto">
                  {currentUser?.name.charAt(0)}
                </div>
                <button className="absolute bottom-0 right-0 w-10 h-10 bg-primary text-white rounded-full flex items-center justify-center hover:bg-primary/90 transition-colors border-4 border-card">
                  <Camera className="w-5 h-5" />
                </button>
              </div>
              <h3 className="text-xl font-semibold text-foreground mt-4">{currentUser?.name}</h3>
              <div className="mt-2">
                <span
                  className={`inline-block px-3 py-1 text-sm font-semibold text-white rounded ${
                    currentRole === "SV"
                      ? "bg-green-500"
                      : currentRole === "PGV"
                      ? "bg-purple-500"
                      : "bg-blue-500"
                  }`}
                >
                  {currentRole === "SV" ? "Sinh viên" : currentRole === "PGV" ? "Phòng Giáo Vụ" : "Khoa"}
                </span>
              </div>
              {currentUser?.studentId && (
                <p className="text-sm text-muted-foreground mt-2">MSSV: {currentUser.studentId}</p>
              )}
              <p className="text-sm text-muted-foreground mt-1">{currentUser?.department}</p>
            </div>

            <div className="mt-6 pt-6 border-t border-border space-y-3">
              <button className="w-full bg-primary text-primary-foreground py-2.5 rounded-lg font-medium hover:bg-primary/90 transition-colors">
                Đổi ảnh đại diện
              </button>
              <button
                onClick={() => navigate("/change-password")}
                className="w-full bg-secondary text-secondary-foreground py-2.5 rounded-lg font-medium hover:bg-secondary/80 transition-colors"
              >
                Đổi mật khẩu
              </button>
            </div>
          </div>
        </div>

        {/* Form Section */}
        <div className="lg:col-span-2">
          <form onSubmit={handleSubmit} className="bg-card border border-border rounded-xl p-6 space-y-6">
            <div className="flex items-center justify-between pb-4 border-b border-border">
              <h3 className="text-lg font-semibold text-foreground">Thông tin chi tiết</h3>
              {!isEditing && (
                <button
                  type="button"
                  onClick={() => setIsEditing(true)}
                  className="px-4 py-2 bg-primary text-primary-foreground rounded-lg text-sm font-medium hover:bg-primary/90 transition-colors"
                >
                  Chỉnh sửa
                </button>
              )}
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Full Name */}
              <div>
                <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                  <User className="w-4 h-4 text-muted-foreground" />
                  Họ và tên
                </label>
                <input
                  type="text"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  disabled={!isEditing}
                  className="w-full px-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 disabled:cursor-not-allowed"
                />
              </div>

              {/* Email */}
              <div>
                <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                  <Mail className="w-4 h-4 text-muted-foreground" />
                  Email
                </label>
                <input
                  type="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  disabled={!isEditing}
                  className="w-full px-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 disabled:cursor-not-allowed"
                />
              </div>

              {/* Phone */}
              <div>
                <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                  <Phone className="w-4 h-4 text-muted-foreground" />
                  Số điện thoại
                </label>
                <input
                  type="tel"
                  value={formData.phone}
                  onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                  disabled={!isEditing}
                  className="w-full px-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 disabled:cursor-not-allowed"
                />
              </div>

              {/* Date of Birth */}
              <div>
                <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                  <Calendar className="w-4 h-4 text-muted-foreground" />
                  Ngày sinh
                </label>
                <input
                  type="date"
                  value={formData.dateOfBirth}
                  onChange={(e) => setFormData({ ...formData, dateOfBirth: e.target.value })}
                  disabled={!isEditing}
                  className="w-full px-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 disabled:cursor-not-allowed"
                />
              </div>

              {/* Gender */}
              <div>
                <label className="block text-sm font-medium mb-2">Giới tính</label>
                <select
                  value={formData.gender}
                  onChange={(e) => setFormData({ ...formData, gender: e.target.value })}
                  disabled={!isEditing}
                  className="w-full px-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 disabled:cursor-not-allowed"
                >
                  <option>Nam</option>
                  <option>Nữ</option>
                  <option>Khác</option>
                </select>
              </div>

              {/* Department */}
              <div>
                <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                  <Building className="w-4 h-4 text-muted-foreground" />
                  Khoa
                </label>
                <input
                  type="text"
                  value={formData.department}
                  disabled
                  className="w-full px-4 py-2.5 border border-input bg-muted/50 rounded-lg opacity-60 cursor-not-allowed"
                />
              </div>

              {/* Student ID (if student) */}
              {currentRole === "SV" && (
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium mb-2">Mã sinh viên</label>
                  <input
                    type="text"
                    value={formData.studentId}
                    disabled
                    className="w-full px-4 py-2.5 border border-input bg-muted/50 rounded-lg opacity-60 cursor-not-allowed"
                  />
                </div>
              )}

              {/* Address */}
              <div className="md:col-span-2">
                <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                  <MapPin className="w-4 h-4 text-muted-foreground" />
                  Địa chỉ
                </label>
                <textarea
                  value={formData.address}
                  onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                  disabled={!isEditing}
                  rows={3}
                  className="w-full px-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 disabled:cursor-not-allowed resize-none"
                />
              </div>
            </div>

            {/* Action Buttons */}
            {isEditing && (
              <div className="flex items-center gap-3 pt-4 border-t border-border">
                <button
                  type="submit"
                  className="px-6 py-2.5 bg-primary text-primary-foreground rounded-lg font-medium hover:bg-primary/90 transition-colors flex items-center gap-2"
                >
                  <Save className="w-4 h-4" />
                  Lưu thay đổi
                </button>
                <button
                  type="button"
                  onClick={handleCancel}
                  className="px-6 py-2.5 bg-secondary text-secondary-foreground rounded-lg font-medium hover:bg-secondary/80 transition-colors"
                >
                  Hủy
                </button>
              </div>
            )}
          </form>

          {/* Additional Settings */}
          <div className="bg-card border border-border rounded-xl p-6 mt-6 space-y-4">
            <h3 className="text-lg font-semibold text-foreground pb-4 border-b border-border">
              Cài đặt thông báo
            </h3>
            <div className="space-y-4">
              <label className="flex items-center justify-between">
                <div>
                  <p className="font-medium text-foreground">Thông báo qua Email</p>
                  <p className="text-sm text-muted-foreground">Nhận thông báo quan trọng qua email</p>
                </div>
                <input type="checkbox" defaultChecked className="w-5 h-5 text-primary rounded" />
              </label>
              <label className="flex items-center justify-between">
                <div>
                  <p className="font-medium text-foreground">Thông báo điểm thi</p>
                  <p className="text-sm text-muted-foreground">Thông báo khi có điểm mới</p>
                </div>
                <input type="checkbox" defaultChecked className="w-5 h-5 text-primary rounded" />
              </label>
              <label className="flex items-center justify-between">
                <div>
                  <p className="font-medium text-foreground">Thông báo lịch học</p>
                  <p className="text-sm text-muted-foreground">Nhắc nhở về lịch học và thời khóa biểu</p>
                </div>
                <input type="checkbox" defaultChecked className="w-5 h-5 text-primary rounded" />
              </label>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}