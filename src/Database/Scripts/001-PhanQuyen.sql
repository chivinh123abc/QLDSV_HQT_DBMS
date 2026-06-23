USE [QLDSV_HTC]

-- ========================================================
-- 0. TẠO LOGIN VÀ USER CHO SINH VIÊN (DÙNG ĐỂ ĐĂNG NHẬP CHUNG)
-- ========================================================
-- Tạo Login 'sv' ở cấp Server nếu chưa có
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'sv')
BEGIN
    CREATE LOGIN [sv] WITH PASSWORD = 'sv12345678', DEFAULT_DATABASE = [QLDSV_HTC];
END
ELSE
BEGIN
    ALTER LOGIN [sv] WITH PASSWORD = 'sv12345678';
END
GO

-- Tạo User 'user_sv' trong Database QLDSV_HTC nếu chưa có
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'user_sv')
BEGIN
    CREATE USER [user_sv] FOR LOGIN [sv];
END
GO

-- Gán User 'user_sv' vào Role 'SV' để thừa hưởng quyền hạn chế
ALTER ROLE [SV] ADD MEMBER [user_sv];
GO

-- ========================================================
-- 0.5 TẠO LOGIN VÀ USER CHO LNKT (PGV) VÀ HTT (KHOA)
-- ========================================================
-- (A) Tài khoản lnkt - Nhóm PGV (Toàn quyền)
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'lnkt')
BEGIN
    CREATE LOGIN [lnkt] WITH PASSWORD = '12345678', DEFAULT_DATABASE = [QLDSV_HTC];
END
ELSE
BEGIN
    ALTER LOGIN [lnkt] WITH PASSWORD = '12345678';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'lnkt')
BEGIN
    CREATE USER [lnkt] FOR LOGIN [lnkt];
END
GO
ALTER ROLE [PGV] ADD MEMBER [lnkt];
-- Cho phép lnkt tạo tài khoản khác
IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'lnkt')
BEGIN
    ALTER SERVER ROLE [securityadmin] ADD MEMBER [lnkt];
END
GO

-- (B) Tài khoản htt - Nhóm KHOA (Quyền hạn chế)
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'htt')
BEGIN
    CREATE LOGIN [htt] WITH PASSWORD = '12345678', DEFAULT_DATABASE = [QLDSV_HTC];
END
ELSE
BEGIN
    ALTER LOGIN [htt] WITH PASSWORD = '12345678';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'htt')
BEGIN
    CREATE USER [htt] FOR LOGIN [htt];
END
GO
ALTER ROLE [KHOA] ADD MEMBER [htt];
-- Cho phép htt tạo tài khoản khác
IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'htt')
BEGIN
    ALTER SERVER ROLE [securityadmin] ADD MEMBER [htt];
END
GO

-- ========================================================
-- 1. NHÓM PGV: TOÀN QUYỀN TRÊN DATABASE
-- ========================================================
-- PGV được add vào db_owner, có quyền execute tất cả SP
ALTER ROLE [db_owner] ADD MEMBER [PGV];
GO

-- ========================================================
-- 2. NHÓM KHOA: QUYỀN HẠN CHẾ (LEAST PRIVILEGE)
-- ========================================================
-- (A) Thu hồi sạch các quyền Execute cũ (để tránh rò rỉ quyền từ script cũ)
REVOKE EXECUTE ON schema::dbo FROM [KHOA];
GO

-- (B) Cấp quyền Đọc dữ liệu (Chỉ SELECT)
ALTER ROLE [db_datareader] ADD MEMBER [KHOA];
GO

-- (C) Cấp quyền thực thi các SP được phép
GRANT EXECUTE ON OBJECT::dbo.sp_DangNhap TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_LayDanhSachLop TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_LayDanhSachSinhVien TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_PhanTrangDong TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_CapNhatDiem TO [KHOA];

GRANT EXECUTE, REFERENCES ON TYPE::dbo.GradeEntryType TO [KHOA];

-- Xem danh sách các module (Khoa, Giảng viên, Môn học, Lớp tín chỉ)
GRANT EXECUTE ON OBJECT::dbo.sp_LayDanhSachKhoa TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_LayDanhSachGiangVien TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_LayDanhSachMonHoc TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_LayDanhSachLopTinChi TO [KHOA];

-- Báo cáo điểm
GRANT EXECUTE ON OBJECT::dbo.sp_LayDanhSachSinhVienDangKyLopTinChi TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_LayBangDiemMonHocCuaMotLopTinChi TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_LayBangDiemTongKet TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_LayPhieuDiem TO [KHOA];

-- Quản lý tài khoản (KHOA chỉ tạo/sửa tài khoản nhóm KHOA)
GRANT EXECUTE ON OBJECT::dbo.sp_LayDanhSachTaiKhoan TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_TaoTaiKhoan TO [KHOA];

GRANT EXECUTE ON OBJECT::dbo.sp_SuaTaiKhoan TO [KHOA];

GRANT VIEW DEFINITION TO [KHOA];

-- (D) Đảm bảo KHOA không có quyền CUD (Xóa/Sửa/Thêm) Lớp và Sinh viên
-- Mặc dù REVOKE ở trên đã làm việc này, nhưng DENY sẽ chặn tuyệt đối kể cả khi có GRANT rời rạc.

-- Lớp
DENY EXECUTE ON OBJECT::dbo.sp_ThemLop TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_SuaLop TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_XoaLop TO [KHOA];

-- Sinh viên
DENY EXECUTE ON OBJECT::dbo.sp_ThemSinhVien TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_SuaSinhVien TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_XoaSinhVien TO [KHOA];

-- Giảng viên
DENY EXECUTE ON OBJECT::dbo.sp_ThemGiangVien TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_SuaGiangVien TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_XoaGiangVien TO [KHOA];

-- Môn học
DENY EXECUTE ON OBJECT::dbo.sp_ThemMonHoc TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_SuaMonHoc TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_XoaMonHoc TO [KHOA];

-- Lớp tín chỉ
DENY EXECUTE ON OBJECT::dbo.sp_ThemLopTinChi TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_SuaLopTinChi TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_XoaLopTinChi TO [KHOA];

-- Đăng ký/hủy đăng ký lớp tín chỉ
DENY EXECUTE ON OBJECT::dbo.sp_DangKyLopTinChi TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_HuyDangKyLopTinChi TO [KHOA];
GO

-- ========================================================
-- 3. NHÓM SV: QUYỀN TỐI THIỂU
-- ========================================================
REVOKE EXECUTE ON schema::dbo FROM [SV];
GO

GRANT EXECUTE ON OBJECT::dbo.sp_DangNhap_SinhVien TO [SV];

GRANT EXECUTE ON OBJECT::dbo.sp_LayPhieuDiem TO [SV];

GRANT EXECUTE ON OBJECT::dbo.sp_LayThongTinSinhVien TO [SV];

GRANT EXECUTE ON OBJECT::dbo.sp_LayDanhSachLopTinChi_SinhVien TO [SV];

GRANT EXECUTE ON OBJECT::dbo.sp_DangKyLopTinChi TO [SV];

GRANT EXECUTE ON OBJECT::dbo.sp_HuyDangKyLopTinChi TO [SV];
GO