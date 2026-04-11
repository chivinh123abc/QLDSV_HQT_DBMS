USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Thêm mới một khoa
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ThemKhoa]
    @MAKHOA    NCHAR(10),
    @TENKHOA   NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM KHOA WHERE MAKHOA = @MAKHOA)
    BEGIN
        RAISERROR(N'Mã khoa đã tồn tại.', 16, 1);
        RETURN;
    END

    INSERT INTO KHOA (MAKHOA, TENKHOA)
    VALUES (@MAKHOA, @TENKHOA);
END
GO

-- =============================================
-- Description: Lấy danh sách toàn bộ các Khoa (không giới hạn quyền truy cập theo role)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayDanhSachKhoa]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        MAKHOA,
        TENKHOA,
        (SELECT COUNT(*) FROM GIANGVIEN WHERE MAKHOA = KHOA.MAKHOA) AS SOLUONGGV
    FROM KHOA
    ORDER BY MAKHOA;
END
GO

-- =============================================
-- Description: Cập nhật thông tin khoa (Cho phép đổi cả Mã Khoa)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SuaKhoa]
    @MAKHOA_OLD NCHAR(10),
    @MAKHOA     NCHAR(10),
    @TENKHOA    NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM KHOA WHERE MAKHOA = @MAKHOA_OLD)
    BEGIN
        RAISERROR(N'Mã khoa gốc không tồn tại.', 16, 1);
        RETURN;
    END

    -- Nếu đổi mã khoa mới, kiểm tra xem mã mới có trùng không
    IF @MAKHOA != @MAKHOA_OLD AND EXISTS (SELECT 1 FROM KHOA WHERE MAKHOA = @MAKHOA)
    BEGIN
        RAISERROR(N'Mã khoa mới đã tồn tại ở bản ghi khác.', 16, 1);
        RETURN;
    END

    UPDATE KHOA
    SET MAKHOA  = @MAKHOA,
        TENKHOA = @TENKHOA
    WHERE MAKHOA = @MAKHOA_OLD;
END
GO

-- =============================================
-- Description: Xoá khoa theo mã khoa
--              Ngăn xóa nếu còn Lớp hoặc Giảng Viên thuộc khoa
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_XoaKhoa]
    @MAKHOA NCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM KHOA WHERE MAKHOA = @MAKHOA)
    BEGIN
        RAISERROR(N'Mã khoa không tồn tại.', 16, 1);
        RETURN;
    END

    -- Kiểm tra còn Lớp thuộc khoa này không
    IF EXISTS (SELECT 1 FROM LOP WHERE MAKHOA = @MAKHOA)
    BEGIN
        RAISERROR(N'Không thể xóa khoa vì còn lớp thuộc khoa này.', 16, 1);
        RETURN;
    END

    -- Kiểm tra còn giảng viên thuộc khoa này không
    IF EXISTS (SELECT 1 FROM GIANGVIEN WHERE MAKHOA = @MAKHOA)
    BEGIN
        RAISERROR(N'Không thể xóa khoa vì còn giảng viên thuộc khoa này.', 16, 1);
        RETURN;
    END

    DELETE FROM KHOA WHERE MAKHOA = @MAKHOA;
END
GO
