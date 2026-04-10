namespace QLDSV_HTC.Application.DTOs
{
    public class SubjectDto
    {
        public string? OldSubjectId { get; set; }
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int TheoryHours { get; set; }
        public int PracticeHours { get; set; }
    }
}
