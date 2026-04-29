namespace QLDSV_HTC.Domain.Constants
{
    public static class ReportConstants
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
