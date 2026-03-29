import { useState } from "react";
import { useNavigate } from "react-router";
import { Mail, ArrowLeft, CheckCircle, GraduationCap } from "lucide-react";

type Step = "email" | "otp" | "success";

export function ForgotPasswordPage() {
  const navigate = useNavigate();
  const [currentStep, setCurrentStep] = useState<Step>("email");
  const [email, setEmail] = useState("");
  const [otp, setOtp] = useState(["", "", "", "", "", ""]);

  const handleEmailSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (email) {
      setCurrentStep("otp");
    }
  };

  const handleOtpSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const otpValue = otp.join("");
    if (otpValue.length === 6) {
      setCurrentStep("success");
    }
  };

  const handleOtpChange = (index: number, value: string) => {
    if (value.length <= 1 && /^\d*$/.test(value)) {
      const newOtp = [...otp];
      newOtp[index] = value;
      setOtp(newOtp);

      // Auto-focus next input
      if (value && index < 5) {
        const nextInput = document.getElementById(`otp-${index + 1}`);
        nextInput?.focus();
      }
    }
  };

  const handleOtpKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === "Backspace" && !otp[index] && index > 0) {
      const prevInput = document.getElementById(`otp-${index - 1}`);
      prevInput?.focus();
    }
  };

  const handleResendOtp = () => {
    setOtp(["", "", "", "", "", ""]);
    // Simulate resending OTP
    alert("Mã OTP mới đã được gửi đến email của bạn!");
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
            Khôi phục mật khẩu
          </h2>
          <p className="mt-2 text-muted-foreground text-center">
            Đừng lo lắng, chúng tôi sẽ giúp bạn lấy lại tài khoản
          </p>
        </div>

        {/* Form Side */}
        <div className="flex-1 max-w-md">
          <div className="bg-card rounded-2xl shadow-xl border border-border p-8">
            {/* Header */}
            <div className="text-center mb-8">
              <h1 className="text-2xl font-bold text-primary mb-2">Quên mật khẩu</h1>
              <p className="text-sm text-muted-foreground">
                {currentStep === "email" && "Nhập email để nhận mã xác thực"}
                {currentStep === "otp" && "Nhập mã OTP đã được gửi đến email"}
                {currentStep === "success" && "Mật khẩu mới đã được tạo"}
              </p>
            </div>

            {/* Step 1: Email Input */}
            {currentStep === "email" && (
              <form onSubmit={handleEmailSubmit} className="space-y-6">
                <div>
                  <label className="block text-sm font-medium mb-2">
                    Email hoặc Mã sinh viên
                  </label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                    <input
                      type="text"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      className="w-full pl-10 pr-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="example@student.edu.vn hoặc 2051052001"
                      required
                    />
                  </div>
                </div>

                <button
                  type="submit"
                  className="w-full bg-primary text-primary-foreground py-2.5 rounded-lg font-medium hover:bg-primary/90 transition-colors"
                >
                  Gửi mã xác thực
                </button>
              </form>
            )}

            {/* Step 2: OTP Input */}
            {currentStep === "otp" && (
              <form onSubmit={handleOtpSubmit} className="space-y-6">
                <div>
                  <label className="block text-sm font-medium mb-4 text-center">
                    Nhập mã OTP (6 số)
                  </label>
                  <div className="flex gap-2 justify-center">
                    {otp.map((digit, index) => (
                      <input
                        key={index}
                        id={`otp-${index}`}
                        type="text"
                        maxLength={1}
                        value={digit}
                        onChange={(e) => handleOtpChange(index, e.target.value)}
                        onKeyDown={(e) => handleOtpKeyDown(index, e)}
                        className="w-12 h-12 text-center text-xl font-semibold border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      />
                    ))}
                  </div>
                  <p className="text-xs text-muted-foreground mt-3 text-center">
                    Mã OTP đã được gửi đến <span className="font-medium">{email}</span>
                  </p>
                </div>

                <button
                  type="submit"
                  className="w-full bg-primary text-primary-foreground py-2.5 rounded-lg font-medium hover:bg-primary/90 transition-colors"
                >
                  Xác nhận
                </button>

                <button
                  type="button"
                  onClick={handleResendOtp}
                  className="w-full text-sm text-primary hover:underline"
                >
                  Không nhận được mã? Gửi lại
                </button>
              </form>
            )}

            {/* Step 3: Success */}
            {currentStep === "success" && (
              <div className="text-center space-y-6">
                <div className="inline-flex items-center justify-center w-20 h-20 rounded-full bg-accent/10">
                  <CheckCircle className="w-12 h-12 text-accent" />
                </div>
                <div>
                  <h3 className="text-xl font-semibold text-foreground mb-2">
                    Khôi phục thành công!
                  </h3>
                  <p className="text-sm text-muted-foreground">
                    Mật khẩu mới đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư và đăng nhập lại.
                  </p>
                </div>
                <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                  <p className="text-sm text-blue-900">
                    <span className="font-semibold">Lưu ý:</span> Vui lòng đổi mật khẩu ngay sau khi đăng nhập để bảo mật tài khoản.
                  </p>
                </div>
                <button
                  onClick={() => navigate("/login")}
                  className="w-full bg-primary text-primary-foreground py-2.5 rounded-lg font-medium hover:bg-primary/90 transition-colors"
                >
                  Đăng nhập ngay
                </button>
              </div>
            )}

            {/* Back to Login */}
            {currentStep !== "success" && (
              <div className="mt-6">
                <button
                  onClick={() => navigate("/login")}
                  className="w-full flex items-center justify-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
                >
                  <ArrowLeft className="w-4 h-4" />
                  Quay lại đăng nhập
                </button>
              </div>
            )}
          </div>

          <p className="mt-6 text-center text-xs text-muted-foreground">
            © 2026 QLDSV_HTC - Quản lý điểm sinh viên theo hệ tín chỉ
          </p>
        </div>
      </div>
    </div>
  );
}
