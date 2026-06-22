namespace QLDSV_HTC.Web.Models
{
    public class AdminRegistrationViewModel
    {
        public IEnumerable<string> Years { get; set; } = [];
        public IEnumerable<int> Semesters { get; set; } = [];
        public string CurrentYear { get; set; } = string.Empty;
        public int CurrentSemester { get; set; }
    }
}
