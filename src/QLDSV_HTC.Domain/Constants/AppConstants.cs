namespace QLDSV_HTC.Domain.Constants
{
    public static class AppConstants
    {
        public static class SpNames
        {
            public const string Login = "sp_DangNhap";
            public const string StudentLogin = "sp_DangNhap_SinhVien";
            public const string GetGradesReport = "sp_LayPhieuDiem";
            public const string GetCreditClassList = "sp_LayDanhSachLopTinChi";
            public const string GetClassList = "sp_LayDanhSachLop";
            public const string AddClass = "sp_ThemLop";
            public const string UpdateClass = "sp_SuaLop";
            public const string DeleteClass = "sp_XoaLop";
        }

        public static class Configs
        {
            public const string ServerName = "DB_SERVER";
            public const string DatabaseName = "DB_DATABASE";
            public const string Username = "DB_USER";
            public const string Password = "DB_PASSWORD";
            public const string TrustServerCertificate = "DB_TRUST_SERVER_CERTIFICATE";
            public const string StudentUsername = "DB_STUDENT_USER";
            public const string StudentPassword = "DB_STUDENT_PASSWORD";
        }

        public static class SessionKeys
        {
            public const string Username = "Username";
            public const string FullName = "FullName";
            public const string Group = "Group";
            public const string FacultyId = "FacultyId";
            public const string UserConnectionString = "UserConnectionString";
        }

        public static class Groups
        {
            public const string PGV = "PGV";
            public const string KHOA = "KHOA";
            public const string SV = "SV";
            public const string Faculty = PGV + "," + KHOA;
        }
    }
}
