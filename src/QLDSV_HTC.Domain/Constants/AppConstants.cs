namespace QLDSV_HTC.Domain.Constants
{
    public static class AppConstants
    {
        public static class Routes
        {
            public const string Login = "/account/login";
            public const string Logout = "/account/logout";
            public const string Home = "/home/index";
            public const string Error = "/home/error";
            public const string AccessDenied = "/home/accessdenied";
        }

        public static class SpNames
        {
            public const string DangNhap = "sp_dangnhap";
            public const string DangNhapSinhVien = "sp_DangNhap_SinhVien";
        }

        public static class Roles
        {
            public const string PGV = "PGV";
            public const string KHOA = "KHOA";
            public const string KETOAN = "KETOAN";
            public const string USER = "USER";
        }

        public static class Configs
        {
            public const string ServerName = "localhost";
            public const string DatabaseName = "QLDSV_HTC";
        }

        public static class SessionKeys
        {
            public const string Username = "Username";
            public const string FullName = "FullName";
            public const string Group = "Group";
        }
    }
}
