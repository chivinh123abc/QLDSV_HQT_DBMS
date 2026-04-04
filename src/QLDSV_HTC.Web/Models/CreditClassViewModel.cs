namespace QLDSV_HTC.Web.Models
{
    public class CreditClassViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string Subject { get; set; } = string.Empty;
        public int Group { get; set; }
        public string Lecturer { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public int MinStudents { get; set; }
        public bool Cancelled { get; set; }
    }

    public class CreditClassManagementViewModel
    {
        public IEnumerable<CreditClassViewModel> CreditClasses { get; set; } = [];
        public IEnumerable<string> Years { get; set; } = [];
        public IEnumerable<int> Semesters { get; set; } = [];
        public IEnumerable<string> Faculties { get; set; } = [];
        public IEnumerable<string> Subjects { get; set; } = [];
        public IEnumerable<string> Lecturers { get; set; } = [];

        public string FilterYear { get; set; } = "all";
        public string FilterSemester { get; set; } = "all";
        public string FilterFaculty { get; set; } = "all";

        public CreditClassViewModel? SelectedCreditClass { get; set; }
    }
}
