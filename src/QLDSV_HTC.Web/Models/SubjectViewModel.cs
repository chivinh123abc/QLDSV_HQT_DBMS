using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Web.Models
{
    public class SubjectViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int TheoryHours { get; set; }
        public int PracticeHours { get; set; }
    }

    public class SubjectManagementViewModel
    {
        public IEnumerable<SubjectViewModel> Subjects { get; set; } = [];
        public PaginationViewModel Pagination { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class SubjectInputModel
    {
        public string? OldSubjectId { get; set; }

        [Required]
        public string SubjectId { get; set; } = string.Empty;

        [Required]
        public string SubjectName { get; set; } = string.Empty;

        [Required]
        public int? TheoryHours { get; set; }

        [Required]
        public int? PracticeHours { get; set; }
    }

    public class SubjectDeleteModel
    {
        [Required]
        public string SubjectId { get; set; } = string.Empty;
    }
}
