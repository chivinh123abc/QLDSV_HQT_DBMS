namespace QLDSV_HTC.Web.Models
{
    public class StudentViewModel
    {
        public string StudentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool Gender { get; set; }
        public string? Dob { get; set; }
        public string? Address { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public bool OnLeave { get; set; }
        public string? Password { get; set; }
    }

    public class StudentManagementViewModel
    {
        public IEnumerable<ClassViewModel> Classes { get; set; } = [];
        public IEnumerable<FacultyViewModel> Faculties { get; set; } = [];
        public string CurrentFacultyId { get; set; } = string.Empty;
    }

    public class StudentInputModel
    {
        public string? OldStudentId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool? Gender { get; set; }
        public string? Address { get; set; }
        public string? Dob { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public bool? OnLeave { get; set; }
        public string? Password { get; set; }
    }

    public class StudentDeleteModel
    {
        public string StudentId { get; set; } = string.Empty;
    }
}
