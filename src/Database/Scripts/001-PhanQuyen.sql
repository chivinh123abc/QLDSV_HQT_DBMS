USE [QLDSV_HTC]
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
GRANT VIEW DEFINITION TO [KHOA]; 

-- (D) Đảm bảo KHOA không có quyền CUD (Xóa/Sửa/Thêm) Lớp và Sinh viên
-- Mặc dù REVOKE ở trên đã làm việc này, nhưng DENY sẽ chặn tuyệt đối kể cả khi có GRANT rời rạc.
DENY EXECUTE ON OBJECT::dbo.sp_ThemLop TO [KHOA];
DENY EXECUTE ON OBJECT::dbo.sp_SuaLop TO [KHOA];
DENY EXECUTE ON OBJECT::dbo.sp_XoaLop TO [KHOA];

DENY EXECUTE ON OBJECT::dbo.sp_ThemSinhVien TO [KHOA];
DENY EXECUTE ON OBJECT::dbo.sp_SuaSinhVien TO [KHOA];
DENY EXECUTE ON OBJECT::dbo.sp_XoaSinhVien TO [KHOA];
GO

-- ========================================================
-- 3. NHÓM SV: QUYỀN TỐI THIỂU
-- ========================================================
REVOKE EXECUTE ON schema::dbo FROM [SV];
GO

GRANT EXECUTE ON OBJECT::dbo.sp_DangNhap_SinhVien TO [SV];
GRANT EXECUTE ON OBJECT::dbo.sp_LayPhieuDiem TO [SV];
GO