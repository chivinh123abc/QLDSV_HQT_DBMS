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

        public string SearchTerm { get; set; } = string.Empty;
        public SubjectViewModel? SelectedSubject { get; set; }
    }
}
