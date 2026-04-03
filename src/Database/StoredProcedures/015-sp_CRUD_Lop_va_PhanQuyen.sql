USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Thêm mới một lớp
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ThemLop]
    @MALOP    NVARCHAR(20),
    @TENLOP   NVARCHAR(100),
    @KHOAHOC  NVARCHAR(10),
    @MAKHOA   NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM LOP WHERE MALOP = @MALOP)
    BEGIN
        RAISERROR(N'Mã lớp đã tồn tại.', 16, 1);
        RETURN;
    END

    INSERT INTO LOP (MALOP, TENLOP, KHOAHOC, MAKHOA)
    VALUES (@MALOP, @TENLOP, @KHOAHOC, @MAKHOA);
END
GO

-- =============================================
-- Description: Cập nhật thông tin lớp
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SuaLop]
    @MALOP    NVARCHAR(20),
    @TENLOP   NVARCHAR(100),
    @KHOAHOC  NVARCHAR(10),
    @MAKHOA   NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM LOP WHERE MALOP = @MALOP)
    BEGIN
        RAISERROR(N'Mã lớp không tồn tại.', 16, 1);
        RETURN;
    END

    UPDATE LOP
    SET TENLOP  = @TENLOP,
        KHOAHOC = @KHOAHOC,
        MAKHOA  = @MAKHOA
    WHERE MALOP = @MALOP;
END
GO

-- =============================================
-- Description: Xoá lớp theo mã lớp
--              Ngăn xóa nếu còn sinh viên thuộc lớp
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_XoaLop]
    @MALOP NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM LOP WHERE MALOP = @MALOP)
    BEGIN
        RAISERROR(N'Mã lớp không tồn tại.', 16, 1);
        RETURN;
    END

    -- Kiểm tra còn sinh viên thuộc lớp này không
    IF EXISTS (SELECT 1 FROM SINHVIEN WHERE MALOP = @MALOP)
    BEGIN
        RAISERROR(N'Không thể xóa lớp vì còn sinh viên thuộc lớp này.', 16, 1);
        RETURN;
    END

    DELETE FROM LOP WHERE MALOP = @MALOP;
END
GO

-- =============================================
-- Phân quyền: PGV là db_owner nên tự có toàn quyền.
-- Cấp thêm EXECUTE cho KHOA để có thể GỌI SP (chỉ đọc danh sách lớp).
-- KHOA vẫn bị DENY INSERT/UPDATE/DELETE trên bảng LOP (xem 001-PhanQuyen.sql)
-- nên dù gọi sp_ThemLop/sp_SuaLop/sp_XoaLop cũng sẽ bị từ chối ở tầng bảng.
-- =============================================
GRANT EXECUTE ON OBJECT::dbo.sp_LayDanhSachLop TO [KHOA];
GRANT EXECUTE ON OBJECT::dbo.sp_ThemLop        TO [KHOA];
GRANT EXECUTE ON OBJECT::dbo.sp_SuaLop         TO [KHOA];
GRANT EXECUTE ON OBJECT::dbo.sp_XoaLop         TO [KHOA];
GO

PRINT N'=== sp_ThemLop, sp_SuaLop, sp_XoaLop đã được tạo/cập nhật thành công. ===';
GO
