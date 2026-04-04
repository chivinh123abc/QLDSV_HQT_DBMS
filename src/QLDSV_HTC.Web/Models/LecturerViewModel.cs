namespace QLDSV_HTC.Web.Models
{
    public class LecturerViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Rank { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
    }

    public class LecturerManagementViewModel
    {
        public IEnumerable<LecturerViewModel> Lecturers { get; set; } = [];
        public IEnumerable<string> Faculties { get; set; } = [];
        public IEnumerable<string> Degrees { get; set; } = [];
        public IEnumerable<string> Ranks { get; set; } = [];

        public string SearchTerm { get; set; } = string.Empty;
        public LecturerViewModel? SelectedLecturer { get; set; }
    }
}
