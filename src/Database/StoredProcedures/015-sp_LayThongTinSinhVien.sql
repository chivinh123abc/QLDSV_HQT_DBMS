USE [QLDSV_HTC]
GO

-- =============================================
-- Lấy thông tin cơ bản của 1 Sinh Viên theo MASV
-- (Dùng cho trang đăng ký tín chỉ, SP chạy trong context dbo → có thể JOIN LOP)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayThongTinSinhVien]
    @MASV NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        sv.MASV,
        sv.HO,
        sv.TEN,
        sv.MALOP,
        ISNULL(l.TENLOP, sv.MALOP) AS TENLOP
    FROM SINHVIEN sv
    LEFT JOIN LOP l ON l.MALOP = sv.MALOP
    WHERE sv.MASV = @MASV;
END
GO