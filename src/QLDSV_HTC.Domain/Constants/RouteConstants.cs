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
            public const string Add = "management/add";
            public const string Edit = "management/edit";
            public const string Delete = "management/delete";

            public const string LoginPath = $"{Prefix}/{Login}";
            public const string LogoutPath = $"{Prefix}/{Logout}";
            public const string ManagementPath = $"{Prefix}/{Management}";
            public const string AddPath = $"{Prefix}/{Add}";
            public const string EditPath = $"{Prefix}/{Edit}";
            public const string DeletePath = $"{Prefix}/{Delete}";
        }

        public static class Report
        {
            public const string Prefix = "/report";
            public const string Index = "";
            public const string GetGradesReport = "grades";
            public const string GetCreditClassList = "credit-class-list";
            public const string GetRegisteredStudentsList = "registered-students-list";
            public const string GetSubjectGrades = "subject-grades";
            public const string GetClassGradesSummary = "class-grades-summary";

            public const string ReportPath = Prefix;
            public const string GetGradesReportPath = $"{Prefix}/{GetGradesReport}";
            public const string GetCreditClassListPath = $"{Prefix}/{GetCreditClassList}";
            public const string GetRegisteredStudentsListPath = $"{Prefix}/{GetRegisteredStudentsList}";
            public const string GetSubjectGradesPath = $"{Prefix}/{GetSubjectGrades}";
            public const string GetClassGradesSummaryPath = $"{Prefix}/{GetClassGradesSummary}";
        }

        public static class Class
        {
            public const string Prefix = "/class";
            public const string Index = "";
            public const string Add = "add";
            public const string Edit = "edit";
            public const string Delete = "delete";

            public const string ClassPath = Prefix;
            public const string AddPath = $"{Prefix}/{Add}";
            public const string EditPath = $"{Prefix}/{Edit}";
            public const string DeletePath = $"{Prefix}/{Delete}";
        }

        public static class Faculty
        {
            public const string Prefix = "/faculty";
            public const string Index = "";
            public const string Add = "add";
            public const string Edit = "edit";
            public const string Delete = "delete";

            public const string FacultyPath = Prefix;
            public const string AddPath = $"{Prefix}/{Add}";
            public const string EditPath = $"{Prefix}/{Edit}";
            public const string DeletePath = $"{Prefix}/{Delete}";
        }

        public static class Student
        {
            public const string Prefix = "/student";
            public const string Index = "";
            public const string Add = "add";
            public const string Edit = "edit";
            public const string Delete = "delete";
            public const string List = "list";
            public const string Schedule = "schedule";
            public const string Grades = "grades";

            public const string StudentPath = Prefix;
            public const string AddPath = $"{Prefix}/{Add}";
            public const string EditPath = $"{Prefix}/{Edit}";
            public const string DeletePath = $"{Prefix}/{Delete}";
            public const string ListPath = $"{Prefix}/{List}";
            public const string SchedulePath = $"{Prefix}/{Schedule}";
            public const string GradesPath = $"{Prefix}/{Grades}";
        }

        public static class Subject
        {
            public const string Prefix = "/subject";
            public const string Index = "";
            public const string Add = "add";
            public const string Edit = "edit";
            public const string Delete = "delete";

            public const string SubjectPath = Prefix;
            public const string AddPath = $"{Prefix}/{Add}";
            public const string EditPath = $"{Prefix}/{Edit}";
            public const string DeletePath = $"{Prefix}/{Delete}";
        }

        public static class Lecturer
        {
            public const string Prefix = "/lecturer";
            public const string Index = "";
            public const string Add = "add";
            public const string Edit = "edit";
            public const string Delete = "delete";

            public const string LecturerPath = Prefix;
            public const string AddPath = $"{Prefix}/{Add}";
            public const string EditPath = $"{Prefix}/{Edit}";
            public const string DeletePath = $"{Prefix}/{Delete}";
        }

        public static class CreditClass
        {
            public const string Prefix = "/credit-class";
            public const string Index = "";
            public const string Add = "add";
            public const string Edit = "edit";
            public const string Delete = "delete";

            public const string CreditClassPath = Prefix;
            public const string AddPath = $"{Prefix}/{Add}";
            public const string EditPath = $"{Prefix}/{Edit}";
            public const string DeletePath = $"{Prefix}/{Delete}";
        }

        public static class Registration
        {
            public const string Prefix = "/registration";
            public const string Index = "";
            public const string Filter = "filter";
            public const string Register = "register";
            public const string Unregister = "unregister";

            public const string RegistrationPath = Prefix;
            public const string FilterPath = $"{Prefix}/{Filter}";
            public const string RegisterPath = $"{Prefix}/{Register}";
            public const string UnregisterPath = $"{Prefix}/{Unregister}";
        }

        public static class Grade
        {
            public const string Prefix = "/grade";
            public const string Index = "";

            public const string GradePath = Prefix;
        }
        public static class DynamicReport
        {
            public const string Prefix = "/dynamic-report";
            public const string Index = "";
            public const string GetTables = "tables";
            public const string GetColumns = "columns";
            public const string Preview = "preview";
            public const string Generate = "generate";

            public const string DynamicReportPath = Prefix;
            public const string GetTablesPath = $"{Prefix}/{GetTables}";
            public const string GetColumnsPath = $"{Prefix}/{GetColumns}";
            public const string PreviewPath = $"{Prefix}/{Preview}";
            public const string GeneratePath = $"{Prefix}/{Generate}";
        }
    }
}
