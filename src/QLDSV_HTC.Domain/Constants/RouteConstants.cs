namespace QLDSV_HTC.Domain.Constants
{
    public static class RouteConstants
    {
        public static class Account
        {
            public const string Prefix = "/account";
            public const string Login = "login";
            public const string Logout = "logout";

            public const string LoginPath = $"{Prefix}/{Login}";
            public const string LogoutPath = $"{Prefix}/{Logout}";
        }

        public static class Home
        {
            public const string Prefix = "/";
            public const string Index = "";
            public const string Error = "error";
            public const string AccessDenied = "access-denied";

            public const string HomePath = Prefix;
            public const string ErrorPath = $"{Prefix}/{Error}";
            public const string AccessDeniedPath = $"{Prefix}/{AccessDenied}";
        }

        public static class Report
        {
            public const string Prefix = "/report";
            public const string LayPhieuDiem = "lay-phieu-diem";
            public const string LayDanhSachLopTinChi = "lay-danh-sach-lop-tin-chi";

            public const string LayPhieuDiemPath = $"{Prefix}/{LayPhieuDiem}";
            public const string LayDanhSachLopTinChiPath = $"{Prefix}/{LayDanhSachLopTinChi}";
        }
    }
}
