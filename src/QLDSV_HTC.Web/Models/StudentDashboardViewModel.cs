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

    public class UpcomingClassViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Lecturer { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
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
        public string DepartmentName { get; set; } = string.Empty;

        public CurrentSemesterInfoViewModel CurrentSemester { get; set; } = new();
        public IEnumerable<UpcomingClassViewModel> UpcomingClasses { get; set; } = [];
        public IEnumerable<AnnouncementViewModel> Announcements { get; set; } = [];
    }
}
