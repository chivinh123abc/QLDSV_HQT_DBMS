namespace QLDSV_HTC.Application.DTOs
{
    public class StudentDto
    {
        public string? OldStudentId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? Dob { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public bool OnLeave { get; set; }
        public string? Password { get; set; }
        public bool HasDependencies { get; set; }
    }
}
