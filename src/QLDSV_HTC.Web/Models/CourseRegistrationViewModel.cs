namespace QLDSV_HTC.Web.Models
{
    public class AvailableClassViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Group { get; set; }
        public string Lecturer { get; set; } = string.Empty;
        public int Registered { get; set; }
        public int MaxStudents { get; set; }
        public bool IsRegistered { get; set; }

        public bool IsFull => Registered >= MaxStudents;
    }

    public class CourseRegistrationViewModel
    {
        public IEnumerable<AvailableClassViewModel> AvailableClasses { get; set; } = [];
        public IEnumerable<AvailableClassViewModel> RegisteredClasses => AvailableClasses.Where(c => c.IsRegistered);

        public string StudentId { get; set; } = string.Empty;
        public string StudentFullName { get; set; } = string.Empty;
        public string StudentClass { get; set; } = string.Empty;

        public IEnumerable<string> Years { get; set; } = [];
        public IEnumerable<int> Semesters { get; set; } = [];

        public string CurrentYear { get; set; } = string.Empty;
        public int CurrentSemester { get; set; }

        public string SelectedYear { get; set; } = string.Empty;
        public int SelectedSemester { get; set; } = 0;
        public string SearchTerm { get; set; } = string.Empty;
    }
}
