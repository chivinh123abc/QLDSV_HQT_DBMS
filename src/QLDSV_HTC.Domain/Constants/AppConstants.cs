namespace QLDSV_HTC.Domain.Constants
{
    public static class AppConstants
    {
        public static class SpNames
        {
            public const string Login = "sp_DangNhap";
            public const string StudentLogin = "sp_DangNhap_SinhVien";
            public const string GetGradesReport = "sp_LayPhieuDiem";

            public const string GetClassList = "sp_LayDanhSachLop";
            public const string AddClass = "sp_ThemLop";
            public const string UpdateClass = "sp_SuaLop";
            public const string DeleteClass = "sp_XoaLop";

            public const string GetStudentList = "sp_LayDanhSachSinhVien";
            public const string GetStudentById = "sp_LayThongTinSinhVien";
            public const string GetCreditClassListForSV = "sp_LayDanhSachLopTinChi_SinhVien";
            public const string RegisterCreditClass = "sp_DangKyLopTinChi";
            public const string UnregisterCreditClass = "sp_HuyDangKyLopTinChi";
            public const string AddStudent = "sp_ThemSinhVien";
            public const string UpdateStudent = "sp_SuaSinhVien";
            public const string DeleteStudent = "sp_XoaSinhVien";

            public const string GetSubjectList = "sp_LayDanhSachMonHoc";
            public const string AddSubject = "sp_ThemMonHoc";
            public const string UpdateSubject = "sp_SuaMonHoc";
            public const string DeleteSubject = "sp_XoaMonHoc";

            public const string GetLecturerList = "sp_LayDanhSachGiangVien";
            public const string AddLecturer = "sp_ThemGiangVien";
            public const string UpdateLecturer = "sp_SuaGiangVien";
            public const string DeleteLecturer = "sp_XoaGiangVien";

            public const string GetCreditClassList = "sp_LayDanhSachLopTinChi";
            public const string AddCreditClass = "sp_ThemLopTinChi";
            public const string UpdateCreditClass = "sp_SuaLopTinChi";
            public const string DeleteCreditClass = "sp_XoaLopTinChi";
            public const string GetAccountList = "sp_LayDanhSachTaiKhoan";
            public const string CreateAccount = "sp_TaoTaiKhoan";
            public const string UpdateAccount = "sp_SuaTaiKhoan";
            public const string DeleteAccount = "sp_XoaTaiKhoan";

            public const string DynamicPagination = "sp_PhanTrangDong";
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

        public static class Formats
        {
            public const string IsoDate = "yyyy-MM-dd";
        }
    }
}
