USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Lấy danh sách Lớp theo Mã Khoa
-- Nếu @MAKHOA = '' thì trả về tất cả (dành cho admin PGV không thuộc khoa)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayDanhSachLop]
    @MAKHOA NVARCHAR(10) = ''
AS
BEGIN
    SET NOCOUNT ON;

    IF @MAKHOA = '' OR @MAKHOA IS NULL
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
    ELSE
    BEGIN
        -- Trả về lớp thuộc khoa đó
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
