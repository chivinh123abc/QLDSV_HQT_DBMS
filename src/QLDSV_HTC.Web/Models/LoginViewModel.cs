using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Models
{
    public class LoginViewModel
    {
        [Required]
        public string LoginName { get; set; } = string.Empty;
        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự.")]
        public string? Password { get; set; }
        public bool IsStudent { get; set; } = false;
    }
}
