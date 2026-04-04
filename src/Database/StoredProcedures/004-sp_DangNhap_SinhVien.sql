USE [QLDSV_HTC]
GO
-- =============================================
-- Description: SP đăng nhập dành cho Sinh Viên (Application-level Security)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_DangNhap_SinhVien]
    @MASV NVARCHAR(50),
    @PASSWORD NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    -- Lấy GroupName từ thông tin của tài khoản SQL gọi procedure này (VD: user_sv)
    DECLARE @GroupName NVARCHAR(100);
    SELECT TOP 1 @GroupName = UPPER(CAST(g.name AS NVARCHAR(100)))
    FROM sys.database_principals u
    JOIN sys.database_role_members rm ON rm.member_principal_id = u.principal_id
    JOIN sys.database_principals g ON g.principal_id = rm.role_principal_id
    WHERE u.name = USER_NAME();

    -- Kết quả trả về
    SELECT 
        MASV AS LoginName, 
        HO + ' ' + TEN AS UserName, 
        ISNULL(@GroupName, 'SV') AS GroupName
    FROM SINHVIEN 
    WHERE MASV = @MASV AND PASSWORD = @PASSWORD
END