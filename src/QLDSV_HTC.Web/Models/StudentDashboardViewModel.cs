namespace QLDSV_HTC.Web.Models
{
    public class CurrentSemesterInfoViewModel
    {
        public string Semester { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public int TotalCredits { get; set; }
        public int RegisteredCourses { get; set; }
        public string Gpa { get; set; } = string.Empty;
    }

    public class RegisteredCourseViewModel
    {
        public string SubjectName { get; set; } = string.Empty;
        public string LecturerName { get; set; } = string.Empty;
        public bool IsTheory { get; set; }
        public bool IsPractice { get; set; }
    }

    public class AnnouncementViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
    }

    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentClass { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string SchoolYear { get; set; } = string.Empty;
        public bool OnLeave { get; set; }

        public CurrentSemesterInfoViewModel CurrentSemester { get; set; } = new();
        public IEnumerable<RegisteredCourseViewModel> RegisteredCourses { get; set; } = [];
        public IEnumerable<AnnouncementViewModel> Announcements { get; set; } = [];
    }
}
