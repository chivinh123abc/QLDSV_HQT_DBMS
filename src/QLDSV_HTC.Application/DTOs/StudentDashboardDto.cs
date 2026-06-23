namespace QLDSV_HTC.Application.DTOs
{
    public class StudentDashboardDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SchoolYear { get; set; } = string.Empty; // Niên khóa (KHOAHOC từ LOP)
        public bool OnLeave { get; set; }

        // Học kỳ hiện tại (lấy HK & NK mới nhất có đăng ký)
        public string CurrentYear { get; set; } = string.Empty;
        public int CurrentSemester { get; set; }
        public int RegisteredCourses { get; set; }

        // Danh sách môn đăng ký HK hiện tại
        public IEnumerable<RegisteredCourseDto> Courses { get; set; } = [];
    }

    public class RegisteredCourseDto
    {
        public int CreditClassId { get; set; }
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string LecturerName { get; set; } = string.Empty;
        public int TheoryHours { get; set; }
        public int PracticeHours { get; set; }
        public float? AttendanceGrade { get; set; }
        public float? MidtermGrade { get; set; }
        public float? FinalGrade { get; set; }
    }
}
