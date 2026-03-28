USE [QLDSV_HTC]
GO

-- ========================================================
-- 1. NHÓM PGV: TOÀN QUYỀN TRÊN DATABASE
-- ========================================================
ALTER ROLE [db_owner] ADD MEMBER [PGV];
GO

-- ========================================================
-- 2. NHÓM KHOA: QUYỀN HẠN CHẾ
-- ========================================================
-- Cấp quyền Đọc/Ghi dữ liệu chung và chạy tất cả SP
ALTER ROLE [db_datareader] ADD MEMBER [KHOA];
ALTER ROLE [db_datawriter] ADD MEMBER [KHOA];
GRANT EXECUTE TO [KHOA];
GO

-- "Trói tay" không cho sửa đổi danh mục (DENY đè lên mọi quyền khác)
DENY INSERT, UPDATE, DELETE ON OBJECT::dbo.KHOA TO [KHOA];
DENY INSERT, UPDATE, DELETE ON OBJECT::dbo.LOP TO [KHOA];
DENY INSERT, UPDATE, DELETE ON OBJECT::dbo.GIANGVIEN TO [KHOA];
DENY INSERT, UPDATE, DELETE ON OBJECT::dbo.SINHVIEN TO [KHOA];
DENY INSERT, UPDATE, DELETE ON OBJECT::dbo.LOPTINCHI TO [KHOA];
GO

-- ========================================================
-- 3. NHÓM SV: QUYỀN TỐI THIỂU (LEAST PRIVILEGE)
-- ========================================================
-- Chỉ cấp quyền chạy đúng những SP dành riêng cho sinh viên
GRANT EXECUTE ON OBJECT::dbo.sp_DangNhap_SinhVien TO [SV];
GRANT EXECUTE ON OBJECT::dbo.sp_LayPhieuDiem TO [SV];
-- (Bổ sung sp_DangKyLopTC, sp_HuyDangKyLopTC... vào đây nếu có)