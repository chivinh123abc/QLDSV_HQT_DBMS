using System.ComponentModel.DataAnnotations;

namespace QLDSV_HTC.Web.Models
{
    public class CreditClassViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Group { get; set; }
        public string LecturerId { get; set; } = string.Empty;
        public string LecturerName { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public int MinStudents { get; set; }
        public bool Cancelled { get; set; }
        public int RegisteredCount { get; set; }
    }

    public class CreditClassManagementViewModel
    {
        public IEnumerable<CreditClassViewModel> CreditClasses { get; set; } = [];
        public IEnumerable<string> Years { get; set; } = [];
        public IEnumerable<int> Semesters { get; set; } = [];
        public IEnumerable<FacultyViewModel> Faculties { get; set; } = [];
        public IEnumerable<SubjectViewModel> Subjects { get; set; } = [];
        public IEnumerable<LecturerViewModel> Lecturers { get; set; } = [];

        public string FilterYear { get; set; } = string.Empty;
        public int? FilterSemester { get; set; }
        public string FilterFaculty { get; set; } = string.Empty;

        public CreditClassViewModel? SelectedCreditClass { get; set; }
        public string CurrentFacultyId { get; set; } = string.Empty;
    }

    public class CreditClassInputModel
    {
        public int? OldCreditClassId { get; set; }

        [Required]
        public int? CreditClassId { get; set; }

        [Required]
        public string Year { get; set; } = string.Empty;

        [Required]
        public int? Semester { get; set; }

        [Required]
        public string SubjectId { get; set; } = string.Empty;

        [Required]
        public int? Group { get; set; }

        [Required]
        public string LecturerId { get; set; } = string.Empty;

        [Required]
        public string FacultyId { get; set; } = string.Empty;

        [Required]
        public int? MinStudents { get; set; }

        [Required]
        public bool? IsCancelled { get; set; }
    }

    public class CreditClassDeleteModel
    {
        [Required]
        public int? CreditClassId { get; set; }
    }
}
