namespace QLDSV_HTC.Web.Models
{
    public class StudentViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Dob { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public bool OnLeave { get; set; }
        public string? Password { get; set; }
    }

    public class StudentManagementViewModel
    {
        public IEnumerable<StudentViewModel> Students { get; set; } = [];
        public IEnumerable<string> Classes { get; set; } = [];
        public IEnumerable<string> Faculties { get; set; } = [];

        public string SelectedClass { get; set; } = "all";
        public string SelectedFaculty { get; set; } = "all";
        public string SearchTerm { get; set; } = string.Empty;

        public StudentViewModel? SelectedStudent { get; set; }
    }
}
