using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Web.Models
{
    public class LecturerViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Degree { get; set; }
        public string? Rank { get; set; }
        public string? Specialty { get; set; }
        public string FacultyId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
    }

    public class LecturerManagementViewModel
    {
        public IEnumerable<LecturerViewModel> Lecturers { get; set; } = [];
        public IEnumerable<FacultyViewModel> Faculties { get; set; } = [];
        public string CurrentFacultyId { get; set; } = string.Empty;
        public PaginationViewModel Pagination { get; set; } = new();
    }

    public class LecturerInputModel
    {
        public string? OldLecturerId { get; set; }

        [Required]
        public string LecturerId { get; set; } = string.Empty;

        [Required]
        public string FacultyId { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? Degree { get; set; }
        public string? AcademicRank { get; set; }
        public string? Specialty { get; set; }
    }

    public class LecturerDeleteModel
    {
        [Required]
        public string LecturerId { get; set; } = string.Empty;
    }
}
