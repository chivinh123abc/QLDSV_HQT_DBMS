namespace QLDSV_HTC.Application.DTOs
{
    public class GradeEntryDto
    {
        public int MaLtc { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public float? AttendanceGrade { get; set; }
        public float? MidtermGrade { get; set; }
        public float? FinalGrade { get; set; }
        public float? TotalGrade { get; set; }
    }
}
