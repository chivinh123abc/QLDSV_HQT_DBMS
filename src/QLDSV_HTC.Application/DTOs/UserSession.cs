namespace QLDSV_HTC.Application.DTOs
{
    public class UserSession
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string LoginName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
    }
}
