namespace QLDSV_HTC.Application.DTOs
{
    public class AccountDto
    {
        public string LoginName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string? LecturerId { get; set; }
        public string? LecturerFullName { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class CreateAccountDto
    {
        public string LoginName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateAccountDto
    {
        public string OldLoginName { get; set; } = string.Empty;
        public string? NewLoginName { get; set; }
        public string? NewPassword { get; set; }
        public string? NewUserName { get; set; }
        public string? NewRole { get; set; }
    }
}
