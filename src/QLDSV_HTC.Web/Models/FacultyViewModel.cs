using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Web.Models
{
    public class FacultyViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int LecturerCount { get; set; }
    }

    public class FacultyManagementViewModel
    {
        public IEnumerable<FacultyViewModel> Faculties { get; set; } = [];
        public PaginationViewModel Pagination { get; set; } = new();
    }

    public class FacultyInputModel
    {
        public string? OldFacultyId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã khoa")]
        public string FacultyId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên khoa")]
        public string FacultyName { get; set; } = string.Empty;
    }

    public class FacultyDeleteModel
    {
        [Required]
        public string FacultyId { get; set; } = string.Empty;
    }
}
