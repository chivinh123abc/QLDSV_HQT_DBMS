using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Web.Models
{
    public class ClassViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class DepartmentViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class ClassManagementViewModel
    {
        public IEnumerable<ClassViewModel> Classes { get; set; } = [];
        public IEnumerable<DepartmentViewModel> Departments { get; set; } = [];
        public string CurrentFacultyId { get; set; } = string.Empty;
    }

    // DTO nhận từ form (JSON body)
    public class ClassInputModel
    {
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
