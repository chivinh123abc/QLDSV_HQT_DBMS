namespace QLDSV_HTC.Application.DTOs
{
    public class AvailableCreditClassDto
    {
        public int Id { get; set; }
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Group { get; set; }
        public string LecturerName { get; set; } = string.Empty;
        public int MinStudents { get; set; }
        public int RegisteredCount { get; set; }
        public bool IsRegistered { get; set; }
    }
}
