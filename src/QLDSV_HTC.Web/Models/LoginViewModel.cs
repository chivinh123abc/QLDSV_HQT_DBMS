using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        public string LoginName { get; set; } = string.Empty;
        [Required]
        public string? Password { get; set; }
        public bool IsStudent { get; set; } = false;
    }
}
