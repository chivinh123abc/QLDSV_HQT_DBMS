namespace QLDSV_HTC.Web.Models
{
    public class ClassViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

    public class ClassManagementViewModel
    {
        public IEnumerable<ClassViewModel> Classes { get; set; } = [];
        public IEnumerable<string> Departments { get; set; } = [];

        public string SelectedDepartment { get; set; } = "all";
        public string SearchTerm { get; set; } = string.Empty;

        public ClassViewModel? SelectedClass { get; set; }
    }
}
