namespace QLDSV_HTC.Domain.Constants
{
    public static class AppConstants
    {
        public static class SpNames
        {
            public const string DangNhap = "sp_DangNhap";
            public const string DangNhapSinhVien = "sp_DangNhap_SinhVien";
            public const string LayPhieuDiem = "sp_LayPhieuDiem";
            public const string LayDanhSachLopTinChi = "sp_LayDanhSachLopTinChi";
        }

        public static class Configs
        {
            public const string ServerName = "DB_SERVER";
            public const string DatabaseName = "DB_DATABASE";
            public const string Username = "DB_USER";
            public const string Password = "DB_PASSWORD";
            public const string TrustServerCertificate = "DB_TRUST_SERVER_CERTIFICATE";
        }

        public static class SessionKeys
        {
            public const string Username = "Username";
            public const string FullName = "FullName";
            public const string Group = "Group";
            public const string UserConnectionString = "UserConnectionString";
        }
    }
}
