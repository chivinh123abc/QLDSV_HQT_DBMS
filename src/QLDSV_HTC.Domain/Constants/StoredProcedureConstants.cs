namespace QLDSV_HTC.Domain.Constants
{
    public static class StoredProcedureConstants
    {
        public static class GetGradesReport
        {
            public const string StudentId = "@MASV";
        }

        public static class GetCreditClassList
        {
            public const string SchoolYear = "@NIENKHOA";
            public const string Semester = "@HOCKY";
            public const string FacultyId = "@MAKHOA";
        }

        public static class Login
        {
            public const string LoginName = "@TENLOGIN";
        }

        public static class StudentLogin
        {
            public const string StudentId = "@MASV";
            public const string Password = "@PASSWORD";
        }

        public static class GetClassList
        {
            public const string FacultyId = "@MAKHOA";
        }

        public static class ClassCrud
        {
            public const string OldClassId = "@MALOP_OLD";
            public const string ClassId = "@MALOP";
            public const string ClassName = "@TENLOP";
            public const string SchoolYear = "@KHOAHOC";
            public const string FacultyId = "@MAKHOA";
        }

        public static class Faculty
        {
            public const string LecturerId = "@MAGV";
        }
    }
}
