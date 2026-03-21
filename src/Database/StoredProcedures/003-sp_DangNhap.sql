USE [QLDSV_HTC]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: SP đăng nhập dành cho Giảng viên/Quản trị
-- Trả về chính xác 2 cột: UserName (Index 0) và GroupName (Index 1)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_DangNhap]
    @TENLOGIN NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TENUSER NVARCHAR(100);
    
    -- 1. Lấy Mã GV (Tên User trong Database)
    SELECT @TENUSER = u.name 
    FROM sys.server_principals l
    JOIN sys.database_principals u ON l.sid = u.sid
    WHERE l.name = @TENLOGIN;
    
    -- 2. Trả về đúng 3 cột: [Mã GV] (Index 0), [Họ Tên] (Index 1), [Nhóm Quyền] (Index 2)
    -- Giống hệt cấu trúc của sp_DangNhap_SinhVien!
    SELECT 
        @TENUSER AS UserName, 
        
        -- Dò tên Giảng viên. Nếu là tài khoản Admin không có trong bảng GV thì để chữ 'Quản trị viên'
        ISNULL((SELECT HO + ' ' + TEN FROM GIANGVIEN WHERE MAGV = @TENUSER), N'Quản trị viên') AS FullName, 
        
        CAST(g.name AS NVARCHAR(100)) AS GroupName
    FROM sys.database_principals u
    JOIN sys.database_role_members rm ON rm.member_principal_id = u.principal_id
    JOIN sys.database_principals g ON g.principal_id = rm.role_principal_id
    WHERE u.name = @TENUSER;
END
GO

-- Cấp quyền cho tất cả các user được phép chạy SP này để xác thực
GRANT EXECUTE ON OBJECT::dbo.sp_DangNhap TO public;
GO