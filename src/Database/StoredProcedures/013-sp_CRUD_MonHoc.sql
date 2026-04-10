USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Thêm mới môn học
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ThemMonHoc]
    @MAMH      NVARCHAR(10),
    @TENMH     NVARCHAR(50),
    @SOTIET_LT INT,
    @SOTIET_TH INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM MONHOC WHERE MAMH = @MAMH)
    BEGIN
        RAISERROR(N'Mã môn học đã tồn tại.', 16, 1);
        RETURN;
    END

    INSERT INTO MONHOC (MAMH, TENMH, SOTIET_LT, SOTIET_TH)
    VALUES (@MAMH, @TENMH, @SOTIET_LT, @SOTIET_TH);
END
GO

-- =============================================
-- Description: Lấy danh sách Môn học (toàn trường)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayDanhSachMonHoc]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MAMH, TENMH, SOTIET_LT, SOTIET_TH
    FROM MONHOC
    ORDER BY TENMH;
END
GO

-- =============================================
-- Description: Cập nhật thông tin môn học (đổi mã môn học)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SuaMonHoc]
    @MAMH_OLD  NVARCHAR(10),
    @MAMH      NVARCHAR(10),
    @TENMH     NVARCHAR(50),
    @SOTIET_LT INT,
    @SOTIET_TH INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM MONHOC WHERE MAMH = @MAMH_OLD)
    BEGIN
        RAISERROR(N'Mã môn học gốc không tồn tại.', 16, 1);
        RETURN;
    END

    IF @MAMH != @MAMH_OLD AND EXISTS (SELECT 1 FROM MONHOC WHERE MAMH = @MAMH)
    BEGIN
        RAISERROR(N'Mã môn học mới đã tồn tại ở bản ghi khác.', 16, 1);
        RETURN;
    END

    UPDATE MONHOC
    SET MAMH = @MAMH,
        TENMH = @TENMH,
        SOTIET_LT = @SOTIET_LT,
        SOTIET_TH = @SOTIET_TH
    WHERE MAMH = @MAMH_OLD;
END
GO

-- =============================================
-- Description: Xoá môn học
-- Ngăn xóa nếu đã có trong loptinchi
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_XoaMonHoc]
    @MAMH NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM MONHOC WHERE MAMH = @MAMH)
    BEGIN
        RAISERROR(N'Mã môn học không tồn tại.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM LOPTINCHI WHERE MAMH = @MAMH)
    BEGIN
        RAISERROR(N'Không thể xóa môn học vì đã có Lớp Tín Chỉ mở môn này.', 16, 1);
        RETURN;
    END

    DELETE FROM MONHOC WHERE MAMH = @MAMH;
END
GO
