namespace QLDSV_HTC.Web.Models
{
    public class StatItemViewModel
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string IconClassName { get; set; } = string.Empty;
        public string BackgroundColorClass { get; set; } = string.Empty;
    }

    public class QuickActionViewModel
    {
        public string Label { get; set; } = string.Empty;
        public string BackgroundColorClass { get; set; } = string.Empty;
        public string ActionUrl { get; set; } = string.Empty;
    }

    public class RegistrationChartData
    {
        public string Name { get; set; } = string.Empty;
        public int Students { get; set; }
    }

    public class RecentActivityViewModel
    {
        public string User { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }

    public class DashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;

        public IEnumerable<StatItemViewModel> Stats { get; set; } = [];
        public IEnumerable<QuickActionViewModel> QuickActions { get; set; } = [];
        public IEnumerable<RegistrationChartData> ChartData { get; set; } = [];
        public IEnumerable<RecentActivityViewModel> RecentActivities { get; set; } = [];
    }
}
