using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Models;

namespace QLDSV_HTC.Web.Services
{
    public class SidebarService : ISidebarService
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            return
            [
                new() { Title = "Trang chủ", Path = RouteConstants.Home.HomePath, Icon = "bi-speedometer2", Roles = [AppConstants.Groups.SV, AppConstants.Groups.PGV, AppConstants.Groups.KHOA] },
               // Items for SV only
                // new() { Title = "Lịch học", Path = RouteConstants.Student.SchedulePath, Icon = "bi-calendar-week", Roles = [AppConstants.Groups.SV] },
                new() { Title = "Xem điểm", Path = RouteConstants.Student.GradesPath, Icon = "bi-award", Roles = [AppConstants.Groups.SV] },
                new() { Title = "Đăng ký lớp tín chỉ", Path = RouteConstants.Registration.RegistrationPath, Icon = "bi-file-earmark-text", Roles = [AppConstants.Groups.SV] },
                // new() { Title = "Đăng ký học phần", Path = RouteConstants.Registration.RegistrationPath, Icon = "bi-file-earmark-text", Roles = [AppConstants.Groups.SV] },
               // Items for PGV/KHOA
                new() { Title = "Lớp", Path = RouteConstants.Class.ClassPath, Icon = "bi-people", Roles = [AppConstants.Groups.PGV, AppConstants.Groups.KHOA] },
                new() { Title = "Sinh viên", Path = RouteConstants.Student.StudentPath, Icon = "bi-mortarboard", Roles = [AppConstants.Groups.PGV, AppConstants.Groups.KHOA] },
                new() { Title = "Môn học", Path = RouteConstants.Subject.SubjectPath, Icon = "bi-book", Roles = [AppConstants.Groups.PGV, AppConstants.Groups.KHOA] },
                new() { Title = "Giảng viên", Path = RouteConstants.Lecturer.LecturerPath, Icon = "bi-person-badge", Roles = [AppConstants.Groups.PGV, AppConstants.Groups.KHOA] },
                new() { Title = "Lớp tín chỉ", Path = RouteConstants.CreditClass.CreditClassPath, Icon = "bi-clipboard-check", Roles = [AppConstants.Groups.PGV, AppConstants.Groups.KHOA] },
                new() { Title = "Nhập điểm", Path = RouteConstants.Grade.GradePath, Icon = "bi-pencil-square", Roles = [AppConstants.Groups.PGV, AppConstants.Groups.KHOA] },
                new() { Title = "Báo cáo / In ấn", Path = RouteConstants.Report.ReportPath, Icon = "bi-bar-chart-line", Roles = [AppConstants.Groups.PGV, AppConstants.Groups.KHOA] },
                new() { Title = "Báo cáo tuỳ chỉnh", Path = RouteConstants.DynamicReport.DynamicReportPath, Icon = "bi-sliders", Roles = [AppConstants.Groups.PGV, AppConstants.Groups.KHOA] },
                new() { Title = "Khoa", Path = RouteConstants.Faculty.FacultyPath, Icon = "bi-clipboard-check", Roles = [AppConstants.Groups.PGV, AppConstants.Groups.KHOA] },
                new() { Title = "Tài khoản / Phân quyền", Path = RouteConstants.Account.ManagementPath, Icon = "bi-gear", Roles = [AppConstants.Groups.PGV, AppConstants.Groups.KHOA] }
            ];
        }
    }
}
