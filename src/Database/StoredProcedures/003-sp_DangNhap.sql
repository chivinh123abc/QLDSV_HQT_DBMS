USE [QLDSV_HTC]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

-- =============================================
-- Description: SP đăng nhập dành cho Giảng viên/Quản trị
-- Trả về chính xác 3 cột: UserName (Index 0), FullName (Index 1), GroupName (Index 2)
--
-- [TỐI ƯU]
--   1. Khử phép nối: Thay subquery inline (correlated subquery chạy mỗi dòng)
--      bằng biến cục bộ @HOTEN — chỉ SELECT 1 lần trên bảng GIANGVIEN.
--   2. Kết quả đăng nhập luôn trả về 1 dòng duy nhất → biến cục bộ là tối ưu nhất,
--      nhẹ hơn cả LEFT JOIN (không cần engine thực hiện phép nối).
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_DangNhap]
    @TENLOGIN NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TENUSER NVARCHAR(100);
    DECLARE @HOTEN   NVARCHAR(200);

    -- 1. Lấy Mã GV (Tên User trong Database)
    SELECT @TENUSER = u.name 
    FROM sys.server_principals l
    JOIN sys.database_principals u ON l.sid = u.sid
    WHERE l.name = @TENLOGIN;

    -- 2. Lấy Họ Tên Giảng viên trước (phép chiếu trước, tránh subquery inline)
    --    Nếu không tìm thấy (tài khoản Admin), @HOTEN vẫn = NULL
    SELECT @HOTEN = HO + N' ' + TEN
    FROM GIANGVIEN
    WHERE MAGV = @TENUSER;

    -- 3. Trả về kết quả — không còn subquery inline
    SELECT 
        @TENUSER AS UserName, 
        ISNULL(@HOTEN, N'Quản trị viên') AS FullName, 
        UPPER(CAST(g.name AS NVARCHAR(100))) AS GroupName
    FROM sys.database_principals u
    JOIN sys.database_role_members rm ON rm.member_principal_id = u.principal_id
    JOIN sys.database_principals g ON g.principal_id = rm.role_principal_id
    WHERE u.name = @TENUSER;
END