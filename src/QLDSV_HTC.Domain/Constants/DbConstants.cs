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
        }
    }
}
