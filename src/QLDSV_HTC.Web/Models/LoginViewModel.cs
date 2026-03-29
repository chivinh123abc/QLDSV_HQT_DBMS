using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Models
{
    public class LoginViewModel
    {
        [Required]
        public string LoginName { get; set; } = string.Empty;
        public string? Password { get; set; }
        public bool? IsStudent { get; set; }
    }
}
