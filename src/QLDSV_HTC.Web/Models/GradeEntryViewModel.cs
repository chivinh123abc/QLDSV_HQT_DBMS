namespace QLDSV_HTC.Web.Models
{
    public class GradeEntryViewModel
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public float AttendanceGrade { get; set; }
        public float MidtermGrade { get; set; }
        public float FinalGrade { get; set; }

        // Calculated locally or returned from DB
        public float TotalGrade => (float)Math.Round(AttendanceGrade * 0.1 + MidtermGrade * 0.3 + FinalGrade * 0.6, 2);
    }

    public class GradeManagementViewModel
    {
        public IEnumerable<GradeEntryViewModel> Students { get; set; } = [];

        public IEnumerable<string> Years { get; set; } = [];
        public IEnumerable<int> Semesters { get; set; } = [];
        public IEnumerable<string> Subjects { get; set; } = [];
        public IEnumerable<int> Groups { get; set; } = [];

        public string SelectedYear { get; set; } = string.Empty;
        public int SelectedSemester { get; set; } = 0;
        public string SelectedSubject { get; set; } = string.Empty;
        public int SelectedGroup { get; set; } = 0;

        public bool IsStarted { get; set; }
    }
}
