namespace QLDSV_HTC.Application.DTOs
{
    public class CreditClassDto
    {
        public int Id { get; set; }
        public string Year { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Group { get; set; }
        public string LecturerId { get; set; } = string.Empty;
        public string LecturerName { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public int MinStudents { get; set; }
        public bool IsCancelled { get; set; }
        public int RegisteredCount { get; set; }
    }
}
