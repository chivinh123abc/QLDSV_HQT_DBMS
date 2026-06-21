using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Web.Models
{
    public class AccountViewModel
    {
        public string LoginName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string? LecturerId { get; set; }
        public string? LecturerFullName { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class AccountManagementViewModel
    {
        public IEnumerable<AccountViewModel> Accounts { get; set; } = [];
        public IEnumerable<LecturerViewModel> Lecturers { get; set; } = [];
        public string CurrentUserGroup { get; set; } = string.Empty;
    }

    public class AccountInputModel
    {
        [Required]
        public string LoginName { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class AccountUpdateInputModel
    {
        [Required]
        public string OldLoginName { get; set; } = string.Empty;

        public string? NewLoginName { get; set; }
        [MinLength(8, ErrorMessage = "Mật khẩu mới phải chứa ít nhất 8 ký tự.")]
        public string? NewPassword { get; set; }
        public string? NewUserName { get; set; }
        public string? NewRole { get; set; }
    }

    public class AccountDeleteModel
    {
        [Required]
        public string LoginName { get; set; } = string.Empty;
    }
}
