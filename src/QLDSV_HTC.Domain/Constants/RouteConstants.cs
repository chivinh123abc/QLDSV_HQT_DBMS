namespace QLDSV_HTC.Domain.Constants
{
    public static class RouteConstants
    {
        public static class Home
        {
            public const string Prefix = "/";
            public const string Index = "";
            public const string Error = "error";
            public const string NotFound = "404";
            public const string ComingSoon = "coming-soon";
            public const string AccessDenied = "access-denied";

            public const string StudentDashboard = "StudentDashboard";
            public const string AdminDashboard = "AdminDashboard";

            public const string HomePath = Prefix;
            public const string ErrorPath = $"{Prefix}{Error}";
            public const string NotFoundPath = $"{Prefix}{NotFound}";
            public const string ComingSoonPath = $"{Prefix}{ComingSoon}";
            public const string AccessDeniedPath = $"{Prefix}{AccessDenied}";
        }

        public static class Account
        {
            public const string Prefix = "/account";
            public const string Login = "login";
            public const string Logout = "logout";
            public const string Management = "management";

            public const string LoginPath = $"{Prefix}/{Login}";
            public const string LogoutPath = $"{Prefix}/{Logout}";
            public const string ManagementPath = $"{Prefix}/{Management}";
        }

        public static class Report
        {
            public const string Prefix = "/report";
            public const string Index = "";
            public const string GetGradesReport = "grades";
            public const string GetCreditClassList = "credit-class-list";

            public const string ReportPath = Prefix;
            public const string GetGradesReportPath = $"{Prefix}/{GetGradesReport}";
            public const string GetCreditClassListPath = $"{Prefix}/{GetCreditClassList}";
        }

        public static class Class
        {
            public const string Prefix = "/class";
            public const string Index = "";
            public const string ClassPath = Prefix;
        }

        public static class Student
        {
            public const string Prefix = "/student";
            public const string Index = "";
            public const string Schedule = "schedule";
            public const string Grades = "grades";

            public const string StudentPath = Prefix;
            public const string SchedulePath = $"{Prefix}/{Schedule}";
            public const string GradesPath = $"{Prefix}/{Grades}";
        }

        public static class Subject
        {
            public const string Prefix = "/subject";
            public const string Index = "";

            public const string SubjectPath = Prefix;
        }

        public static class Lecturer
        {
            public const string Prefix = "/lecturer";
            public const string Index = "";

            public const string LecturerPath = Prefix;
        }

        public static class CreditClass
        {
            public const string Prefix = "/credit-class";
            public const string Index = "";

            public const string CreditClassPath = Prefix;
        }

        public static class Registration
        {
            public const string Prefix = "/registration";
            public const string Index = "";

            public const string RegistrationPath = Prefix;
        }

        public static class Grade
        {
            public const string Prefix = "/grade";
            public const string Index = "";

            public const string GradePath = Prefix;
        }
    }
}
