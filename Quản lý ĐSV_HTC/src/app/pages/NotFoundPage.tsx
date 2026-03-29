import { useNavigate } from "react-router";
import { Search, Home, ArrowLeft, FileQuestion } from "lucide-react";

export function NotFoundPage() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-blue-50 flex items-center justify-center p-4">
      <div className="max-w-2xl w-full text-center">
        {/* 404 Illustration */}
        <div className="mb-8">
          <div className="inline-flex items-center justify-center w-32 h-32 rounded-full bg-blue-100 mb-6">
            <FileQuestion className="w-16 h-16 text-primary" />
          </div>
          <h1 className="text-9xl font-bold text-primary mb-2">404</h1>
          <h2 className="text-3xl font-semibold text-foreground mb-3">
            Không tìm thấy trang
          </h2>
          <p className="text-lg text-muted-foreground max-w-md mx-auto">
            Xin lỗi, trang bạn đang tìm kiếm không tồn tại hoặc đã được di chuyển.
          </p>
        </div>

        {/* Search Box */}
        <div className="bg-card border border-border rounded-xl p-6 mb-6">
          <p className="text-sm font-medium text-foreground mb-3">Tìm kiếm trang bạn cần:</p>
          <div className="relative">
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
            <input
              type="text"
              placeholder="Nhập từ khóa tìm kiếm..."
              className="w-full pl-12 pr-4 py-3 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </div>

        {/* Action Buttons */}
        <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
          <button
            onClick={() => navigate(-1)}
            className="w-full sm:w-auto px-6 py-3 bg-secondary text-secondary-foreground rounded-lg font-medium hover:bg-secondary/80 transition-colors flex items-center justify-center gap-2"
          >
            <ArrowLeft className="w-5 h-5" />
            Quay lại trang trước
          </button>
          <button
            onClick={() => navigate("/")}
            className="w-full sm:w-auto px-6 py-3 bg-primary text-primary-foreground rounded-lg font-medium hover:bg-primary/90 transition-colors flex items-center justify-center gap-2"
          >
            <Home className="w-5 h-5" />
            Về trang chủ
          </button>
        </div>

        {/* Quick Links */}
        <div className="mt-12 pt-8 border-t border-border">
          <p className="text-sm font-medium text-foreground mb-4">Các trang phổ biến:</p>
          <div className="flex flex-wrap justify-center gap-3">
            <button
              onClick={() => navigate("/")}
              className="px-4 py-2 bg-blue-50 text-primary rounded-lg text-sm hover:bg-blue-100 transition-colors"
            >
              Dashboard
            </button>
            <button
              onClick={() => navigate("/schedule")}
              className="px-4 py-2 bg-blue-50 text-primary rounded-lg text-sm hover:bg-blue-100 transition-colors"
            >
              Lịch học
            </button>
            <button
              onClick={() => navigate("/student-grades")}
              className="px-4 py-2 bg-blue-50 text-primary rounded-lg text-sm hover:bg-blue-100 transition-colors"
            >
              Điểm
            </button>
            <button
              onClick={() => navigate("/registration")}
              className="px-4 py-2 bg-blue-50 text-primary rounded-lg text-sm hover:bg-blue-100 transition-colors"
            >
              Đăng ký
            </button>
            <button
              onClick={() => navigate("/profile")}
              className="px-4 py-2 bg-blue-50 text-primary rounded-lg text-sm hover:bg-blue-100 transition-colors"
            >
              Thông tin cá nhân
            </button>
          </div>
        </div>

        {/* Help Text */}
        <p className="mt-8 text-sm text-muted-foreground">
          Nếu bạn cho rằng đây là lỗi, vui lòng{" "}
          <a href="#" className="text-primary hover:underline font-medium">
            liên hệ bộ phận hỗ trợ
          </a>
        </p>
      </div>
    </div>
  );
}
