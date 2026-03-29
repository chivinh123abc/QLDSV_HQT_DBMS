USE [QLDSV_HTC]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

-- =============================================
-- Author:      Antigravity
-- Create date: 2026-03-21
-- Description: SP đăng nhập dành cho Sinh Viên (Application-level Security)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_DangNhap_SinhVien]
    @MASV NVARCHAR(50),
    @PASSWORD NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra thông tin trong bảng SINHVIEN
    -- Lưu ý: Cột PASSWORD trong thực tế nên được mã hóa (MD5/SHA) 
    -- nhưng ở đây dùng plaintext theo cấu trúc mẫu của đề bài.
    SELECT 
        MASV AS LoginName, 
        HO + ' ' + TEN AS UserName, 
        UPPER(CAST('SV') AS NVARCHAR(100)) AS GroupName
    FROM SINHVIEN 
    WHERE MASV = @MASV AND PASSWORD = @PASSWORD
END