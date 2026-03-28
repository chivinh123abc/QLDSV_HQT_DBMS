namespace QLDSV_HTC.Web.Models
{
    public class ReportTypeViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconClassName { get; set; } = string.Empty;
    }

    public class ReportFilterViewModel
    {
        public string Year { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
    }

    public class ReportDataViewModel
    {
        // Headers and Rows for generic table generation on the client
        public IEnumerable<string> Headers { get; set; } = [];
        public IEnumerable<IEnumerable<string>> Rows { get; set; } = [];
        public string ReportTitle { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
    }

    public class ReportsPageViewModel
    {
        public IEnumerable<ReportTypeViewModel> ReportTypes { get; set; } = [];
        public ReportFilterViewModel Filters { get; set; } = new();
        public IEnumerable<string> AvailableYears { get; set; } = [];
        public IEnumerable<string> AvailableSemesters { get; set; } = [];
        public IEnumerable<string> AvailableSubjects { get; set; } = [];
        public IEnumerable<string> AvailableClasses { get; set; } = [];

        public string SelectedReportId { get; set; } = string.Empty;
        public bool ShowPreview { get; set; }

        public ReportDataViewModel? PreviewData { get; set; }
    }
}
