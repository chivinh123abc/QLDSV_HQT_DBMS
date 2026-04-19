namespace QLDSV_HTC.Domain.Constants
{
    public static class AppConstants
    {
        public static class SpNames
        {
            public const string Login = "sp_DangNhap";
            public const string StudentLogin = "sp_DangNhap_SinhVien";
            public const string GetGradesReport = "sp_LayPhieuDiem";
            public const string GetRegisteredStudentsList = "sp_LayDanhSachSinhVienDangKyLopTinChi";
            public const string GetSubjectGrades = "sp_LayBangDiemMonHocCuaMotLopTinChi";
            public const string GetClassGradesSummary = "sp_LayBangDiemTongKet";

            public const string GetClassList = "sp_LayDanhSachLop";
            public const string AddClass = "sp_ThemLop";
            public const string UpdateClass = "sp_SuaLop";
            public const string DeleteClass = "sp_XoaLop";

            public const string GetFacultyList = "sp_LayDanhSachKhoa";
            public const string AddFaculty = "sp_ThemKhoa";
            public const string UpdateFaculty = "sp_SuaKhoa";
            public const string DeleteFaculty = "sp_XoaKhoa";

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

        public static class Reports
        {
            public static readonly IReadOnlyDictionary<string, string> ColumnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "MALTC", "Mã Lớp Tín Chỉ" },
                { "NIENKHOA", "Niên Khóa" },
                { "HOCKY", "Học Kỳ" },
                { "MAMH", "Mã Môn" },
                { "TENMH", "Tên Môn Học" },
                { "NHOM", "Nhóm" },
                { "MAGV", "Mã GV" },
                { "HOTEN_GV", "Giảng Viên" },
                { "MAKHOA", "Mã Khoa" },
                { "SOSVTOITHIEU", "SV Tối Thiểu" },
                { "HUYLOP", "Đã Hủy" },
                { "SOSV_DANGKY", "Lượng ĐK" },
                { "MASV", "Mã Sinh Viên" },
                { "HO", "Họ" },
                { "TEN", "Tên" },
                { "PHAI", "Giới Tính" },
                { "MALOP", "Mã Lớp" },
                { "DIEM_CC", "Chuyên Cần" },
                { "DIEM_GK", "Giữa Kỳ" },
                { "DIEM_CK", "Cuối Kỳ" },
                { "DIEM_HET_MON", "Điểm Khóa" },
                { "DIEM", "Điểm" }
            };
        }
    }
}
