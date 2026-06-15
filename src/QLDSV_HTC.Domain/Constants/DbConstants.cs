namespace QLDSV_HTC.Domain.Constants
{
    public static class DbConstants
    {
        public static class Tables
        {
            public const string Class = "LOP";
            public const string Faculty = "KHOA";
            public const string Lecturer = "GIANGVIEN";
            public const string Student = "SINHVIEN";
            public const string Subject = "MONHOC";
            public const string CreditClass = "LOPTINCHI";
            public const string Grade = "DANGKY";
        }

        public static class Columns
        {
            public static class Class
            {
                public const string Id = "MALOP";
                public const string Name = "TENLOP";
                public const string SchoolYear = "KHOAHOC";
                public const string FacultyId = "MAKHOA";
                public const string FacultyName = "TENKHOA";
                public const string StudentCount = "StudentCount";
            }

            public static class Subject
            {
                public const string Id = "MAMH";
                public const string Name = "TENMH";
                public const string TheoryHours = "SOTIET_LT";
                public const string PracticeHours = "SOTIET_TH";
            }

            public static class Faculty
            {
                public const string Id = "MAKHOA";
                public const string Name = "TENKHOA";
                public const string LecturerCount = "SOLUONGGV";
            }

            public static class Lecturer
            {
                public const string Id = "MAGV";
                public const string FacultyId = "MAKHOA";
                public const string FirstName = "HO";
                public const string LastName = "TEN";
                public const string Degree = "HOCVI";
                public const string AcademicRank = "HOCHAM";
                public const string Specialty = "CHUYENMON";
                public const string FacultyName = "TENKHOA";
            }

            public static class Student
            {
                public const string Id = "MASV";
                public const string FirstName = "HO";
                public const string LastName = "TEN";
                public const string Gender = "PHAI";
                public const string Address = "DIACHI";
                public const string Dob = "NGAYSINH";
                public const string ClassId = "MALOP";
                public const string ClassName = "TENLOP";
                public const string OnLeave = "DANGHIHOC";
                public const string HasDependencies = "HasDependencies";
            }

            public static class Account
            {
                public const string LoginName = "LoginName";
                public const string UserName = "UserName";
                public const string GroupName = "GroupName";
                public const string LecturerId = "LecturerId";
                public const string LecturerFullName = "LecturerFullName";
                public const string IsDisabled = "IsDisabled";
            }

            public static class CreditClass
            {
                public const string Id = "MALTC";
                public const string Year = "NIENKHOA";
                public const string Semester = "HOCKY";
                public const string SubjectId = "MAMH";
                public const string SubjectName = "TENMH";
                public const string Group = "NHOM";
                public const string LecturerId = "MAGV";
                public const string LecturerName = "HOTEN_GV";
                public const string FacultyId = "MAKHOA";
                public const string MinStudents = "SOSVTOITHIEU";
                public const string IsCancelled = "HUYLOP";
                public const string RegisteredCount = "SOSV_DANGKY";
                public const string IsRegistered = "DA_DANGKY";
            }
        }
        public static class SqlKeywords
        {
            public const string InnerJoin = "INNER JOIN";
            public const string LeftJoin = "LEFT JOIN";
            public const string RightJoin = "RIGHT JOIN";
            public const string FullOuterJoin = "FULL OUTER JOIN";
            public const string CrossJoin = "CROSS JOIN";
            public const string Select = "SELECT";
            public const string From = "FROM";
            public const string Where = "WHERE";
            public const string GroupBy = "GROUP BY";
            public const string OrderBy = "ORDER BY";
            public const string Having = "HAVING";
            public const string And = "AND";
            public const string Or = "OR";
            public const string Asc = "ASC";
            public const string Desc = "DESC";
            public const string Offset = "OFFSET";
            public const string FetchNext = "FETCH NEXT";
            public const string RowsOnly = "ROWS ONLY";
            public const string In = "IN";
            public const string NotIn = "NOT IN";
            public const string Between = "BETWEEN";
            public const string IsNull = "IS NULL";
            public const string IsNotNull = "IS NOT NULL";
            public const string Like = "LIKE";
            public const string NotLike = "NOT LIKE";
            public const string Cast = "CAST";
            public const string As = "AS";
        }
    }
}
