import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { GraduationCap, User, Lock, AlertCircle } from "lucide-react";
import { useAuth } from "../contexts/AuthContext";

export function LoginPage() {
  const navigate = useNavigate();
  const { login, isAuthenticated } = useAuth();
  const [loginType, setLoginType] = useState<"admin" | "student">("admin");
  const [formData, setFormData] = useState({
    username: "",
    password: "",
    remember: false,
  });
  const [error, setError] = useState("");

  // Redirect if already authenticated
  useEffect(() => {
    if (isAuthenticated) {
      navigate("/");
    }
  }, [isAuthenticated, navigate]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    // Validate input
    if (!formData.username || !formData.password) {
      setError("Vui lòng nhập đầy đủ thông tin đăng nhập");
      return;
    }

    // Attempt login
    const success = login(formData.username, formData.password);

    if (success) {
      navigate("/");
    } else {
      setError("Tên đăng nhập hoặc mật khẩu không đúng");
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-blue-50 flex items-center justify-center p-4">
      <div className="w-full max-w-5xl flex items-center gap-12">
        {/* Illustration Side */}
        <div className="hidden lg:flex flex-1 flex-col items-center justify-center">
          <div className="w-80 h-80 bg-blue-100 rounded-full flex items-center justify-center">
            <GraduationCap className="w-40 h-40 text-primary" />
          </div>
          <h2 className="mt-8 text-2xl font-bold text-foreground text-center">
            Hệ thống quản lý điểm sinh viên
          </h2>
          <p className="mt-2 text-muted-foreground text-center">
            Theo hệ tín chỉ - Hiện đại và chuyên nghiệp
          </p>
        </div>

        {/* Login Form */}
        <div className="flex-1 max-w-md">
          <div className="bg-card rounded-2xl shadow-xl border border-border p-8">
            <div className="text-center mb-8">
              <h1 className="text-2xl font-bold text-primary mb-2">
                QLDSV_HTC
              </h1>
              <p className="text-sm text-muted-foreground">
                QUẢN LÝ ĐIỂM SINH VIÊN THEO HỆ TÍN CHỈ
              </p>
            </div>

            {/* Role Selection */}
            <div className="flex gap-2 mb-6">
              <button
                type="button"
                onClick={() => setLoginType("admin")}
                className={`flex-1 py-2.5 px-4 rounded-lg text-sm font-medium transition-colors ${
                  loginType === "admin"
                    ? "bg-primary text-primary-foreground"
                    : "bg-secondary text-secondary-foreground hover:bg-secondary/80"
                }`}
              >
                Giảng viên / Khoa / PGV
              </button>
              <button
                type="button"
                onClick={() => setLoginType("student")}
                className={`flex-1 py-2.5 px-4 rounded-lg text-sm font-medium transition-colors ${
                  loginType === "student"
                    ? "bg-primary text-primary-foreground"
                    : "bg-secondary text-secondary-foreground hover:bg-secondary/80"
                }`}
              >
                Sinh viên
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
              {/* Error Message */}
              {error && (
                <div className="p-3 bg-destructive/10 border border-destructive/20 rounded-lg flex items-start gap-2">
                  <AlertCircle className="w-5 h-5 text-destructive flex-shrink-0 mt-0.5" />
                  <p className="text-sm text-destructive">{error}</p>
                </div>
              )}

              <div>
                <label className="block text-sm font-medium mb-2">
                  {loginType === "admin" ? "Login" : "Mã sinh viên"}
                </label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type="text"
                    value={formData.username}
                    onChange={(e) =>
                      setFormData({ ...formData, username: e.target.value })
                    }
                    className="w-full pl-10 pr-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                    placeholder={
                      loginType === "admin" ? "Nhập tên đăng nhập" : "Nhập mã sinh viên"
                    }
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">
                  Mật khẩu
                </label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type="password"
                    value={formData.password}
                    onChange={(e) =>
                      setFormData({ ...formData, password: e.target.value })
                    }
                    className="w-full pl-10 pr-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                    placeholder="Nhập mật khẩu"
                  />
                </div>
              </div>

              <div className="flex items-center">
                <input
                  type="checkbox"
                  id="remember"
                  checked={formData.remember}
                  onChange={(e) =>
                    setFormData({ ...formData, remember: e.target.checked })
                  }
                  className="w-4 h-4 text-primary border-input rounded focus:ring-2 focus:ring-primary"
                />
                <label htmlFor="remember" className="ml-2 text-sm text-foreground">
                  Ghi nhớ đăng nhập
                </label>
              </div>

              <button
                type="submit"
                className="w-full bg-primary text-primary-foreground py-2.5 rounded-lg font-medium hover:bg-primary/90 transition-colors"
              >
                Đăng nhập
              </button>
            </form>

            <div className="mt-6 p-4 bg-blue-50 border border-blue-200 rounded-lg">
              <p className="text-xs font-semibold text-blue-900 mb-2">Tài khoản test:</p>
              <div className="space-y-1 text-xs text-blue-700">
                <p>• Sinh viên: <span className="font-mono font-semibold">sv / sv</span></p>
                <p>• Phòng Giáo Vụ: <span className="font-mono font-semibold">pgv / pgv</span></p>
                <p>• Khoa: <span className="font-mono font-semibold">khoa / khoa</span></p>
              </div>
            </div>

            <div className="mt-6 text-center">
              <a href="#" className="text-sm text-primary hover:underline">
                Quên mật khẩu?
              </a>
            </div>
          </div>

          <p className="mt-6 text-center text-xs text-muted-foreground">
            © 2026 QLDSV_HTC - Quản lý điểm sinh viên theo hệ tín chỉ
          </p>
        </div>
      </div>
    </div>
  );
}