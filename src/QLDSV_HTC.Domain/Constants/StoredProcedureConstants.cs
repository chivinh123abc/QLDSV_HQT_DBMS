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

        public static class StudentCrud
        {
            public const string OldStudentId = "@MASV_OLD";
            public const string StudentId = "@MASV";
            public const string FirstName = "@HO";
            public const string LastName = "@TEN";
            public const string Gender = "@PHAI";
            public const string Address = "@DIACHI";
            public const string Dob = "@NGAYSINH";
            public const string ClassId = "@MALOP";
            public const string OnLeave = "@DANGHIHOC";
            public const string Password = "@PASSWORD";
        }

        public static class LecturerCrud
        {
            public const string OldLecturerId = "@MAGV_OLD";
            public const string LecturerId = "@MAGV";
            public const string FacultyId = "@MAKHOA";
            public const string FirstName = "@HO";
            public const string LastName = "@TEN";
            public const string Degree = "@HOCVI";
            public const string AcademicRank = "@HOCHAM";
            public const string Specialty = "@CHUYENMON";
        }

        public static class Faculty
        {
            public const string LecturerId = "@MAGV";
        }

        public static class FacultyCrud
        {
            public const string OldFacultyId = "@MAKHOA_OLD";
            public const string FacultyId = "@MAKHOA";
            public const string FacultyName = "@TENKHOA";
        }

        public static class AccountCrud
        {
            public const string LoginName = "@LGNAME";
            public const string Password = "@PASS";
            public const string UserName = "@USERNAME";
            public const string Role = "@ROLE";
            public const string OldLoginName = "@OLD_LGNAME";
            public const string NewLoginName = "@NEW_LGNAME";
            public const string NewPassword = "@NEW_PASS";
            public const string NewUserName = "@NEW_USERNAME";
            public const string NewRole = "@NEW_ROLE";
        }

        public static class Pagination
        {
            public const string SelectCols = "@SelectCols";
            public const string TableName = "@TableName";
            public const string JoinClause = "@JoinClause";
            public const string WhereClause = "@WhereClause";
            public const string OrderBy = "@OrderBy";
            public const string PageNumber = "@PageNumber";
            public const string PageSize = "@PageSize";
            public const string TotalCount = "TotalCount";
        }
    }
}
