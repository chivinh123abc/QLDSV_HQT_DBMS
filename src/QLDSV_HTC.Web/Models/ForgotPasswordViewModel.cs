using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập hoặc mã sinh viên.")]
        public string LoginName { get; set; } = string.Empty;

        public bool IsStudent { get; set; } = false;

        public string? Email { get; set; }

        public string? Otp { get; set; }

        [MinLength(8, ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự.")]
        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }

        public int Step { get; set; } = 1; // 1 = Input LoginName, 2 = Verify OTP & Reset Password

        public bool ResendOtp { get; set; } = false;
    }
}
