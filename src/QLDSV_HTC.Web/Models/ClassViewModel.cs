using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Web.Models
{
    public class ClassViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
    }

    public class ClassManagementViewModel
    {
        public IEnumerable<ClassViewModel> Classes { get; set; } = [];
        public IEnumerable<FacultyViewModel> Faculties { get; set; } = [];
        public string CurrentFacultyId { get; set; } = string.Empty;
        public PaginationViewModel Pagination { get; set; } = new();
    }

    public class ClassInputModel
    {
        public string? OldClassId { get; set; }

        [Required]
        public string ClassId { get; set; } = string.Empty;

        [Required]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        public string SchoolYear { get; set; } = string.Empty;

        [Required]
        public string FacultyId { get; set; } = string.Empty;
    }

    public class ClassDeleteModel
    {
        [Required]
        public string ClassId { get; set; } = string.Empty;
    }
}
