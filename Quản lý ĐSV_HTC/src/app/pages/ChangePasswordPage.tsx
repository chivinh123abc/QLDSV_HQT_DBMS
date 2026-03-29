import { useState } from "react";
import { Lock, Eye, EyeOff, CheckCircle, XCircle } from "lucide-react";

export function ChangePasswordPage() {
  const [showPassword, setShowPassword] = useState({
    current: false,
    new: false,
    confirm: false,
  });

  const [formData, setFormData] = useState({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  });

  const [passwordStrength, setPasswordStrength] = useState(0);
  const [errors, setErrors] = useState<string[]>([]);
  const [success, setSuccess] = useState(false);

  // Password strength checker
  const checkPasswordStrength = (password: string) => {
    let strength = 0;
    if (password.length >= 8) strength++;
    if (password.length >= 12) strength++;
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
    if (/\d/.test(password)) strength++;
    if (/[!@#$%^&*(),.?":{}|<>]/.test(password)) strength++;
    setPasswordStrength(strength);
  };

  const handleNewPasswordChange = (value: string) => {
    setFormData({ ...formData, newPassword: value });
    checkPasswordStrength(value);
  };

  const validateForm = () => {
    const newErrors: string[] = [];

    if (!formData.currentPassword) {
      newErrors.push("Vui lòng nhập mật khẩu hiện tại");
    }

    if (!formData.newPassword) {
      newErrors.push("Vui lòng nhập mật khẩu mới");
    } else {
      if (formData.newPassword.length < 8) {
        newErrors.push("Mật khẩu mới phải có ít nhất 8 ký tự");
      }
      if (formData.newPassword === formData.currentPassword) {
        newErrors.push("Mật khẩu mới không được trùng với mật khẩu hiện tại");
      }
    }

    if (!formData.confirmPassword) {
      newErrors.push("Vui lòng xác nhận mật khẩu mới");
    } else if (formData.newPassword !== formData.confirmPassword) {
      newErrors.push("Mật khẩu xác nhận không khớp");
    }

    setErrors(newErrors);
    return newErrors.length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validateForm()) {
      // Simulate password change
      setSuccess(true);
      setFormData({
        currentPassword: "",
        newPassword: "",
        confirmPassword: "",
      });
      setPasswordStrength(0);
      setErrors([]);
      
      setTimeout(() => setSuccess(false), 5000);
    }
  };

  const getStrengthColor = () => {
    if (passwordStrength <= 1) return "bg-destructive";
    if (passwordStrength <= 3) return "bg-warning";
    return "bg-accent";
  };

  const getStrengthText = () => {
    if (passwordStrength <= 1) return "Yếu";
    if (passwordStrength <= 3) return "Trung bình";
    return "Mạnh";
  };

  return (
    <div className="max-w-2xl mx-auto space-y-8">
      <div>
        <h2 className="text-2xl font-semibold text-foreground">Đổi mật khẩu</h2>
        <p className="text-sm text-muted-foreground mt-1">
          Cập nhật mật khẩu của bạn để bảo mật tài khoản
        </p>
      </div>

      {/* Success Message */}
      {success && (
        <div className="bg-accent/10 border border-accent/20 rounded-xl p-4 flex items-start gap-3">
          <CheckCircle className="w-5 h-5 text-accent flex-shrink-0 mt-0.5" />
          <div>
            <p className="font-medium text-accent">Đổi mật khẩu thành công!</p>
            <p className="text-sm text-muted-foreground mt-1">
              Mật khẩu của bạn đã được cập nhật. Vui lòng sử dụng mật khẩu mới cho lần đăng nhập tiếp theo.
            </p>
          </div>
        </div>
      )}

      {/* Error Messages */}
      {errors.length > 0 && (
        <div className="bg-destructive/10 border border-destructive/20 rounded-xl p-4">
          <div className="flex items-start gap-3">
            <XCircle className="w-5 h-5 text-destructive flex-shrink-0 mt-0.5" />
            <div className="flex-1">
              <p className="font-medium text-destructive mb-2">Vui lòng kiểm tra lại:</p>
              <ul className="space-y-1">
                {errors.map((error, index) => (
                  <li key={index} className="text-sm text-destructive flex items-center gap-2">
                    <span className="w-1 h-1 rounded-full bg-destructive" />
                    {error}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      )}

      {/* Form */}
      <form onSubmit={handleSubmit} className="bg-card border border-border rounded-xl p-6 space-y-6">
        {/* Current Password */}
        <div>
          <label className="block text-sm font-medium mb-2">Mật khẩu hiện tại</label>
          <div className="relative">
            <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
            <input
              type={showPassword.current ? "text" : "password"}
              value={formData.currentPassword}
              onChange={(e) => setFormData({ ...formData, currentPassword: e.target.value })}
              className="w-full pl-10 pr-12 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              placeholder="Nhập mật khẩu hiện tại"
            />
            <button
              type="button"
              onClick={() => setShowPassword({ ...showPassword, current: !showPassword.current })}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
            >
              {showPassword.current ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
            </button>
          </div>
          <p className="text-xs text-muted-foreground mt-2">
            <a href="#" className="text-primary hover:underline">Quên mật khẩu?</a>
          </p>
        </div>

        <div className="border-t border-border" />

        {/* New Password */}
        <div>
          <label className="block text-sm font-medium mb-2">Mật khẩu mới</label>
          <div className="relative">
            <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
            <input
              type={showPassword.new ? "text" : "password"}
              value={formData.newPassword}
              onChange={(e) => handleNewPasswordChange(e.target.value)}
              className="w-full pl-10 pr-12 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              placeholder="Nhập mật khẩu mới"
            />
            <button
              type="button"
              onClick={() => setShowPassword({ ...showPassword, new: !showPassword.new })}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
            >
              {showPassword.new ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
            </button>
          </div>
          
          {/* Password Strength Indicator */}
          {formData.newPassword && (
            <div className="mt-3">
              <div className="flex items-center justify-between mb-2">
                <span className="text-xs text-muted-foreground">Độ mạnh mật khẩu:</span>
                <span className={`text-xs font-semibold ${
                  passwordStrength <= 1 ? "text-destructive" : 
                  passwordStrength <= 3 ? "text-warning" : 
                  "text-accent"
                }`}>
                  {getStrengthText()}
                </span>
              </div>
              <div className="h-2 bg-muted rounded-full overflow-hidden">
                <div
                  className={`h-full ${getStrengthColor()} transition-all duration-300`}
                  style={{ width: `${(passwordStrength / 5) * 100}%` }}
                />
              </div>
            </div>
          )}

          <div className="mt-3 space-y-1">
            <p className="text-xs text-muted-foreground">Mật khẩu nên có:</p>
            <ul className="space-y-1 text-xs">
              <li className={`flex items-center gap-2 ${formData.newPassword.length >= 8 ? "text-accent" : "text-muted-foreground"}`}>
                {formData.newPassword.length >= 8 ? <CheckCircle className="w-3 h-3" /> : <span className="w-3 h-3 rounded-full border border-current" />}
                Ít nhất 8 ký tự
              </li>
              <li className={`flex items-center gap-2 ${/[A-Z]/.test(formData.newPassword) && /[a-z]/.test(formData.newPassword) ? "text-accent" : "text-muted-foreground"}`}>
                {/[A-Z]/.test(formData.newPassword) && /[a-z]/.test(formData.newPassword) ? <CheckCircle className="w-3 h-3" /> : <span className="w-3 h-3 rounded-full border border-current" />}
                Chữ hoa và chữ thường
              </li>
              <li className={`flex items-center gap-2 ${/\d/.test(formData.newPassword) ? "text-accent" : "text-muted-foreground"}`}>
                {/\d/.test(formData.newPassword) ? <CheckCircle className="w-3 h-3" /> : <span className="w-3 h-3 rounded-full border border-current" />}
                Ít nhất 1 số
              </li>
              <li className={`flex items-center gap-2 ${/[!@#$%^&*(),.?":{}|<>]/.test(formData.newPassword) ? "text-accent" : "text-muted-foreground"}`}>
                {/[!@#$%^&*(),.?":{}|<>]/.test(formData.newPassword) ? <CheckCircle className="w-3 h-3" /> : <span className="w-3 h-3 rounded-full border border-current" />}
                Ít nhất 1 ký tự đặc biệt
              </li>
            </ul>
          </div>
        </div>

        {/* Confirm Password */}
        <div>
          <label className="block text-sm font-medium mb-2">Xác nhận mật khẩu mới</label>
          <div className="relative">
            <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
            <input
              type={showPassword.confirm ? "text" : "password"}
              value={formData.confirmPassword}
              onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
              className="w-full pl-10 pr-12 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              placeholder="Nhập lại mật khẩu mới"
            />
            <button
              type="button"
              onClick={() => setShowPassword({ ...showPassword, confirm: !showPassword.confirm })}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
            >
              {showPassword.confirm ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
            </button>
          </div>
        </div>

        {/* Submit Button */}
        <div className="pt-4 border-t border-border flex gap-3">
          <button
            type="submit"
            className="px-6 py-2.5 bg-primary text-primary-foreground rounded-lg font-medium hover:bg-primary/90 transition-colors"
          >
            Đổi mật khẩu
          </button>
          <button
            type="button"
            onClick={() => {
              setFormData({ currentPassword: "", newPassword: "", confirmPassword: "" });
              setPasswordStrength(0);
              setErrors([]);
            }}
            className="px-6 py-2.5 bg-secondary text-secondary-foreground rounded-lg font-medium hover:bg-secondary/80 transition-colors"
          >
            Hủy
          </button>
        </div>
      </form>

      {/* Security Tips */}
      <div className="bg-blue-50 border border-blue-200 rounded-xl p-6">
        <h3 className="font-semibold text-blue-900 mb-3 flex items-center gap-2">
          <Lock className="w-5 h-5" />
          Mẹo bảo mật
        </h3>
        <ul className="space-y-2 text-sm text-blue-800">
          <li className="flex items-start gap-2">
            <span className="w-1.5 h-1.5 rounded-full bg-blue-600 mt-1.5 flex-shrink-0" />
            Không sử dụng mật khẩu giống với các tài khoản khác
          </li>
          <li className="flex items-start gap-2">
            <span className="w-1.5 h-1.5 rounded-full bg-blue-600 mt-1.5 flex-shrink-0" />
            Thay đổi mật khẩu định kỳ (3-6 tháng)
          </li>
          <li className="flex items-start gap-2">
            <span className="w-1.5 h-1.5 rounded-full bg-blue-600 mt-1.5 flex-shrink-0" />
            Không chia sẻ mật khẩu với bất kỳ ai
          </li>
          <li className="flex items-start gap-2">
            <span className="w-1.5 h-1.5 rounded-full bg-blue-600 mt-1.5 flex-shrink-0" />
            Sử dụng trình quản lý mật khẩu để lưu trữ an toàn
          </li>
        </ul>
      </div>
    </div>
  );
}
