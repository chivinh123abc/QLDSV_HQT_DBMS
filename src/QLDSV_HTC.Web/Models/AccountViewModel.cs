namespace QLDSV_HTC.Web.Models
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class CreateAccountViewModel
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

    public class AccountManagementViewModel
    {
        public IEnumerable<AccountViewModel> Accounts { get; set; } = [];
        public CreateAccountViewModel NewAccount { get; set; } = new();

        public string ActiveTab { get; set; } = "list";
    }
}
