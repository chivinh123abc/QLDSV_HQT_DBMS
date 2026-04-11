namespace QLDSV_HTC.Application.DTOs
{
    public class ClassDto
    {
        public string? OldClassId { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SchoolYear { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
    }

    public class FacultyDto
    {
        public string? OldFacultyId { get; set; }
        public string FacultyId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public int LecturerCount { get; set; }
    }
}
