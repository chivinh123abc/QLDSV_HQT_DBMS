namespace QLDSV_HTC.Web.Models
{
    public class MenuItem
    {
        public string Title { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string[] Roles { get; set; } = [];
    }
}
