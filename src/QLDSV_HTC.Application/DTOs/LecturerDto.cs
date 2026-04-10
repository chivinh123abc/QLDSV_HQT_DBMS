namespace QLDSV_HTC.Application.DTOs
{
    public class LecturerDto
    {
        public string? OldLecturerId { get; set; }
        public string LecturerId { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Degree { get; set; }
        public string? AcademicRank { get; set; }
        public string? Specialty { get; set; }
    }
}
