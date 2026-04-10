USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Lấy danh sách giảng viên
-- Cấp quyền: PGV (Tất cả), KHOA (Chỉ giảng viên thuộc khoa)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayDanhSachGiangVien]
    @MAKHOA NCHAR(10) = NULL  -- NULL = tất cả, có giá trị = lọc theo khoa
AS
BEGIN
    SET NOCOUNT ON;

    -- PGV/dbo: xem tất cả giảng viên (có thể lọc theo khoa)
    IF IS_MEMBER('PGV') = 1 OR USER_NAME() = 'dbo'
    BEGIN
        SELECT 
            gv.MAGV, gv.HO, gv.TEN,
            gv.HOCVI, gv.HOCHAM, gv.CHUYENMON,
            gv.MAKHOA,
            ISNULL(k.TENKHOA, gv.MAKHOA) AS TENKHOA
        FROM GIANGVIEN gv
        LEFT JOIN KHOA k ON k.MAKHOA = gv.MAKHOA
        WHERE @MAKHOA IS NULL OR gv.MAKHOA = @MAKHOA
        ORDER BY gv.MAKHOA, gv.MAGV;
    END
    ELSE IF IS_MEMBER('KHOA') = 1
    BEGIN
        -- Xác định khoa của giảng viên hiện tại
        DECLARE @MAKHOA_CURRENT NVARCHAR(10);
        SELECT @MAKHOA_CURRENT = MAKHOA FROM GIANGVIEN WHERE MAGV = USER_NAME();

        SELECT 
            gv.MAGV, gv.HO, gv.TEN,
            gv.HOCVI, gv.HOCHAM, gv.CHUYENMON,
            gv.MAKHOA,
            ISNULL(k.TENKHOA, gv.MAKHOA) AS TENKHOA
        FROM GIANGVIEN gv
        LEFT JOIN KHOA k ON k.MAKHOA = gv.MAKHOA
        WHERE gv.MAKHOA = @MAKHOA_CURRENT
        ORDER BY gv.MAGV;
    END
END
GO


-- =============================================
-- Description: Thêm mới giảng viên
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ThemGiangVien]
    @MAGV       NCHAR(10),
    @MAKHOA     NCHAR(10),
    @HO         NVARCHAR(50),
    @TEN        NVARCHAR(10),
    @HOCVI      NVARCHAR(20) = NULL,
    @HOCHAM     NVARCHAR(20) = NULL,
    @CHUYENMON  NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra MAGV trùng (EXISTS tối ưu hơn COUNT)
    IF EXISTS (SELECT 1 FROM GIANGVIEN WHERE MAGV = @MAGV)
    BEGIN
        RAISERROR(N'Mã giảng viên đã tồn tại.', 16, 1);
        RETURN;
    END

    -- Kiểm tra MAKHOA hợp lệ (khử phép nối: dùng EXISTS thay vì JOIN)
    IF NOT EXISTS (SELECT 1 FROM KHOA WHERE MAKHOA = @MAKHOA)
    BEGIN
        RAISERROR(N'Mã khoa không tồn tại.', 16, 1);
        RETURN;
    END

    INSERT INTO GIANGVIEN (MAGV, MAKHOA, HO, TEN, HOCVI, HOCHAM, CHUYENMON)
    VALUES (@MAGV, @MAKHOA, @HO, @TEN, @HOCVI, @HOCHAM, @CHUYENMON);
END
GO


-- =============================================
-- Description: Cập nhật thông tin giảng viên
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SuaGiangVien]
    @MAGV_OLD   NCHAR(10),
    @MAGV       NCHAR(10),
    @MAKHOA     NCHAR(10),
    @HO         NVARCHAR(50),
    @TEN        NVARCHAR(10),
    @HOCVI      NVARCHAR(20) = NULL,
    @HOCHAM     NVARCHAR(20) = NULL,
    @CHUYENMON  NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM GIANGVIEN WHERE MAGV = @MAGV_OLD)
    BEGIN
        RAISERROR(N'Mã giảng viên gốc không tồn tại.', 16, 1);
        RETURN;
    END

    -- Nếu đổi MAGV, kiểm tra mã mới có trùng không
    IF @MAGV != @MAGV_OLD AND EXISTS (SELECT 1 FROM GIANGVIEN WHERE MAGV = @MAGV)
    BEGIN
        RAISERROR(N'Mã giảng viên mới đã tồn tại ở bản ghi khác.', 16, 1);
        RETURN;
    END

    -- Kiểm tra MAKHOA hợp lệ
    IF NOT EXISTS (SELECT 1 FROM KHOA WHERE MAKHOA = @MAKHOA)
    BEGIN
        RAISERROR(N'Mã khoa không tồn tại.', 16, 1);
        RETURN;
    END

    UPDATE GIANGVIEN
    SET MAGV       = @MAGV,
        MAKHOA     = @MAKHOA,
        HO         = @HO,
        TEN        = @TEN,
        HOCVI      = @HOCVI,
        HOCHAM     = @HOCHAM,
        CHUYENMON  = @CHUYENMON
    WHERE MAGV = @MAGV_OLD;
END
GO


-- =============================================
-- Description: Xóa giảng viên
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_XoaGiangVien]
    @MAGV NCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM GIANGVIEN WHERE MAGV = @MAGV)
    BEGIN
        RAISERROR(N'Mã giảng viên không tồn tại.', 16, 1);
        RETURN;
    END

    -- Kiểm tra ràng buộc: giảng viên có đang phụ trách lớp tín chỉ không
    IF EXISTS (SELECT 1 FROM LOPTINCHI WHERE MAGV = @MAGV)
    BEGIN
        RAISERROR(N'Không thể xóa giảng viên vì đang phụ trách lớp tín chỉ.', 16, 1);
        RETURN;
    END

    DELETE FROM GIANGVIEN WHERE MAGV = @MAGV;
END
GO
