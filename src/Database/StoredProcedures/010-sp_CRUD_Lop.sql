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
-- Description: Lấy danh sách Lớp theo Mã Khoa
-- Nếu @MAKHOA = '' thì trả về tất cả (dành cho admin PGV không thuộc khoa)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayDanhSachLop]
AS
BEGIN
    SET NOCOUNT ON;

    IF IS_MEMBER('PGV') = 1 OR USER_NAME() = 'dbo'
    BEGIN
        -- Trả về tất cả lớp (kết hợp tên khoa)
        SELECT 
            l.MALOP,
            l.TENLOP,
            l.KHOAHOC,
            l.MAKHOA,
            ISNULL(k.TENKHOA, l.MAKHOA) AS TENKHOA
        FROM LOP l
        LEFT JOIN KHOA k ON k.MAKHOA = l.MAKHOA
        ORDER BY l.MAKHOA, l.MALOP;
    END
    ELSE IF IS_MEMBER('KHOA') = 1
    BEGIN
        -- Trả về lớp thuộc khoa của giảng viên
        DECLARE @MAKHOA NVARCHAR(10);
        SELECT @MAKHOA = MAKHOA FROM GIANGVIEN WHERE MAGV = USER_NAME();

        SELECT 
            l.MALOP,
            l.TENLOP,
            l.KHOAHOC,
            l.MAKHOA,
            ISNULL(k.TENKHOA, l.MAKHOA) AS TENKHOA
        FROM LOP l
        LEFT JOIN KHOA k ON k.MAKHOA = l.MAKHOA
        WHERE l.MAKHOA = @MAKHOA
        ORDER BY l.MALOP;
    END
END
GO

-- =============================================
-- Description: Cập nhật thông tin lớp (Cho phép đổi cả Mã Lớp)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SuaLop]
    @MALOP_OLD NVARCHAR(20),
    @MALOP     NVARCHAR(20),
    @TENLOP    NVARCHAR(100),
    @KHOAHOC   NVARCHAR(10),
    @MAKHOA    NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM LOP WHERE MALOP = @MALOP_OLD)
    BEGIN
        RAISERROR(N'Mã lớp gốc không tồn tại.', 16, 1);
        RETURN;
    END

    -- Nếu đổi mã lớp mới, kiểm tra xem mã mới có trùng không
    IF @MALOP != @MALOP_OLD AND EXISTS (SELECT 1 FROM LOP WHERE MALOP = @MALOP)
    BEGIN
        RAISERROR(N'Mã lớp mới đã tồn tại ở bản ghi khác.', 16, 1);
        RETURN;
    END

    UPDATE LOP
    SET MALOP   = @MALOP,
        TENLOP  = @TENLOP,
        KHOAHOC = @KHOAHOC,
        MAKHOA  = @MAKHOA
    WHERE MALOP = @MALOP_OLD;
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
