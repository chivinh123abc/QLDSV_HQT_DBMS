USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Tạo tài khoản mới (Login + User + Role)
-- Tối ưu: Kiểm tra giá trị NULL hoặc rỗng trước khi thực hiện để không tốn tài nguyên
-- =============================================
CREATE OR ALTER PROCEDURE sp_TaoTaiKhoan 
    @LGNAME NVARCHAR(50), 
    @PASS NVARCHAR(50), 
    @USERNAME NVARCHAR(50), 
    @ROLE NVARCHAR(50) 
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @stmt NVARCHAR(MAX);

    -- 1. Kiểm tra trước (Pre-validation) để tránh văng lỗi hệ thống
    IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @LGNAME)
    BEGIN
        RAISERROR ('Login name đã tồn tại trên Server!', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @USERNAME)
    BEGIN
        RAISERROR ('User name đã tồn tại trong Database!', 16, 1);
        RETURN;
    END

    -- 2. Bắt đầu Giao dịch (Transaction) để đảm bảo tính toàn vẹn
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Bước 1: Tạo Login (Cấp Server)
        -- Lưu ý: Phải replace dấu nháy đơn trong Password để chống SQL Injection khi dùng SQL động
        SET @stmt = 'CREATE LOGIN [' + @LGNAME + '] WITH PASSWORD = ''' + REPLACE(@PASS, '''', '''''') + ''', DEFAULT_DATABASE = [QLDSV_HTC]';
        EXEC sp_executesql @stmt;

        -- Bước 2: Tạo User (Cấp Database) và Map vào Login vừa tạo
        SET @stmt = 'CREATE USER [' + @USERNAME + '] FOR LOGIN [' + @LGNAME + ']';
        EXEC sp_executesql @stmt;

        -- Bước 3: Gán Role cho User trong Database
        SET @stmt = 'ALTER ROLE [' + @ROLE + '] ADD MEMBER [' + @USERNAME + ']';
        EXEC sp_executesql @stmt;

        -- Bước 4: Gán thêm quyền Server (Ví dụ: securityadmin) để user này tạo tài khoản được cho người khác
        IF (@ROLE IN (CAST(UPPER('PGV') AS NVARCHAR(10)), CAST(UPPER('KHOA') AS NVARCHAR(10))))
        BEGIN
            SET @stmt = 'ALTER SERVER ROLE [securityadmin] ADD MEMBER [' + @LGNAME + ']';
            EXEC sp_executesql @stmt;
        END

        -- Nếu mọi thứ thành công, lưu thay đổi
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Nếu có bất kỳ lỗi gì ở giữa chừng, Rollback (hoàn tác) lại toàn bộ
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Báo lỗi chi tiết ra cho C# (Web) bắt được
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH
END
GO


-- =============================================
-- Description: Lấy danh sách tài khoản (login) trong hệ thống
-- Tối ưu:
--   1. Phép chọn trước: Lọc loại login (S=SQL Login) và loại user trước khi JOIN
--   2. Phép chiếu trước: CTE chỉ SELECT cột cần thiết
--   3. Khử phép nối: Dùng CTE tách biệt, chỉ JOIN GIANGVIEN 1 lần cuối
--   4. AND: điều kiện loại trừ (l.name NOT IN) có xác suất sai cao → đặt đầu
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayDanhSachTaiKhoan]
AS
BEGIN
    SET NOCOUNT ON;

    -- CTE 1: Phép chọn trên database_principals (loại user hệ thống)
    ;WITH UserRole AS (
        SELECT
            u.sid       AS UserSid,
            u.name      AS UserName,
            UPPER(CAST(g.name AS NVARCHAR(100))) AS GroupName,
            SUSER_SNAME(u.sid) AS LoginName     -- Khử phép nối: lấy tên Login trực tiếp từ SID
        FROM sys.database_principals u
        JOIN sys.database_role_members rm ON rm.member_principal_id = u.principal_id
        JOIN sys.database_principals g   ON g.principal_id = rm.role_principal_id
        WHERE u.type IN ('S', 'U')                  -- SQL user hoặc Windows user
          AND u.name NOT IN ('dbo', 'guest', 'INFORMATION_SCHEMA', 'sys', 'user_sv')
    )
    -- Phép kết cuối cùng: JOIN CTE với GIANGVIEN
    SELECT 
        ur.LoginName,
        ur.UserName,
        ur.GroupName,
        gv.MAGV     AS LecturerId,
        CASE 
            WHEN gv.MAGV IS NOT NULL THEN gv.HO + N' ' + gv.TEN
            ELSE NULL
        END         AS LecturerFullName,
        CONVERT(BIT, 0) AS IsDisabled -- Bỏ qua cờ disable để tối ưu vì ta không dùng
    FROM UserRole ur
    LEFT JOIN GIANGVIEN gv ON gv.MAGV = ur.UserName  -- Khử phép nối: chỉ 1 LEFT JOIN
    WHERE ur.LoginName IS NOT NULL
    ORDER BY ur.GroupName, ur.LoginName;
END
GO


-- =============================================
-- Description: Cập nhật thông tin tài khoản (Tên đăng nhập, Mật khẩu, Mã GV, Quyền)
-- Tối ưu: Kiểm tra giá trị NULL hoặc rỗng trước khi thực hiện để không tốn tài nguyên Update
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SuaTaiKhoan]
    @OLD_LGNAME NVARCHAR(50),
    @NEW_LGNAME NVARCHAR(50) = NULL,
    @NEW_PASS   NVARCHAR(50) = NULL,
    @NEW_USERNAME NVARCHAR(50) = NULL,
    @NEW_ROLE   NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @stmt NVARCHAR(MAX);

    -- Kiểm tra login cũ tồn tại
    IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @OLD_LGNAME AND type = 'S')
    BEGIN
        RAISERROR (N'Login [%s] không tồn tại trên Server!', 16, 1, @OLD_LGNAME);
        RETURN;
    END

    -- Kiểm tra xem tài khoản đang sửa có đang đăng nhập (hoạt động) hay không
    IF EXISTS (SELECT 1 FROM sys.dm_exec_sessions WHERE login_name = @OLD_LGNAME AND session_id <> @@SPID)
    BEGIN
        RAISERROR (N'Tài khoản [%s] đang có phiên đăng nhập hoạt động. Không thể sửa lúc này!', 16, 1, @OLD_LGNAME);
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Cập nhật tên đăng nhập (Login Name)
        IF @NEW_LGNAME IS NOT NULL AND @NEW_LGNAME <> '' AND @NEW_LGNAME <> @OLD_LGNAME
        BEGIN
            IF EXISTS(SELECT 1 FROM sys.server_principals WHERE name = @NEW_LGNAME)
            BEGIN
                RAISERROR(N'Tên đăng nhập mới [%s] đã tồn tại!', 16, 1, @NEW_LGNAME);
                ROLLBACK TRANSACTION;
                RETURN;
            END
            SET @stmt = 'ALTER LOGIN [' + @OLD_LGNAME + '] WITH NAME = [' + @NEW_LGNAME + ']';
            EXEC sp_executesql @stmt;
            -- Cập nhật lại chuỗi để các bước sau lấy tên mới
            SET @OLD_LGNAME = @NEW_LGNAME;
        END

        -- 2. Cập nhật mật khẩu
        IF @NEW_PASS IS NOT NULL AND @NEW_PASS <> ''
        BEGIN
            SET @stmt = 'ALTER LOGIN [' + @OLD_LGNAME + '] WITH PASSWORD = ''' + REPLACE(@NEW_PASS, '''', '''''') + '''';
            EXEC sp_executesql @stmt;
        END

        -- 3. Cập nhật mã giảng viên / tên người dùng trong Database (User Name)
        DECLARE @OLD_USERNAME NVARCHAR(50);
        SELECT @OLD_USERNAME = u.name 
        FROM sys.database_principals u 
        WHERE u.sid = SUSER_SID(@OLD_LGNAME);

        IF @OLD_USERNAME IS NOT NULL AND @NEW_USERNAME IS NOT NULL AND @NEW_USERNAME <> '' AND @NEW_USERNAME <> @OLD_USERNAME
        BEGIN
            IF EXISTS(SELECT 1 FROM sys.database_principals WHERE name = @NEW_USERNAME)
            BEGIN
                RAISERROR(N'Mã giảng viên mới [%s] đã có tài khoản!', 16, 1, @NEW_USERNAME);
                ROLLBACK TRANSACTION;
                RETURN;
            END
            SET @stmt = 'ALTER USER [' + @OLD_USERNAME + '] WITH NAME = [' + @NEW_USERNAME + ']';
            EXEC sp_executesql @stmt;
            SET @OLD_USERNAME = @NEW_USERNAME;
        END

        -- 4. Cập nhật nhóm quyền (Role)
        IF @OLD_USERNAME IS NOT NULL AND @NEW_ROLE IS NOT NULL AND @NEW_ROLE <> ''
        BEGIN
            DECLARE @OLD_ROLE NVARCHAR(50);
            SELECT @OLD_ROLE = g.name 
            FROM sys.database_role_members rm
            JOIN sys.database_principals g ON rm.role_principal_id = g.principal_id
            JOIN sys.database_principals u ON rm.member_principal_id = u.principal_id
            WHERE u.name = @OLD_USERNAME;
            
            IF @OLD_ROLE IS NOT NULL AND @OLD_ROLE <> @NEW_ROLE
            BEGIN
                -- Bỏ role cũ, thêm role mới
                SET @stmt = 'ALTER ROLE [' + @OLD_ROLE + '] DROP MEMBER [' + @OLD_USERNAME + ']';
                EXEC sp_executesql @stmt;
                
                SET @stmt = 'ALTER ROLE [' + @NEW_ROLE + '] ADD MEMBER [' + @OLD_USERNAME + ']';
                EXEC sp_executesql @stmt;
                
                -- Phân quyền quản trị ở cấp Server (chỉ Khoa, PGV)
                IF @OLD_ROLE IN ('PGV', 'KHOA') AND @NEW_ROLE NOT IN ('PGV', 'KHOA')
                BEGIN
                    BEGIN TRY
                        SET @stmt = 'ALTER SERVER ROLE [securityadmin] DROP MEMBER [' + @OLD_LGNAME + ']';
                        EXEC sp_executesql @stmt;
                    END TRY BEGIN CATCH END CATCH
                END
                ELSE IF @OLD_ROLE NOT IN ('PGV', 'KHOA') AND @NEW_ROLE IN ('PGV', 'KHOA')
                BEGIN
                    BEGIN TRY
                        SET @stmt = 'ALTER SERVER ROLE [securityadmin] ADD MEMBER [' + @OLD_LGNAME + ']';
                        EXEC sp_executesql @stmt;
                    END TRY BEGIN CATCH END CATCH
                END
            END
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH
END
GO


-- =============================================
-- Description: Xóa tài khoản (User + Login)
-- Tối ưu:
--   1. Phép chọn trước: kiểm tra tồn tại login + user trước khi DROP
--   2. Transaction đảm bảo tính toàn vẹn (xóa user trước, login sau)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_XoaTaiKhoan]
    @LGNAME NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @stmt NVARCHAR(MAX);
    DECLARE @USERNAME NVARCHAR(100);

    -- 1. Phép chọn trước: Kiểm tra login tồn tại
    IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @LGNAME AND type = 'S')
    BEGIN
        RAISERROR (N'Login [%s] không tồn tại trên Server!', 16, 1, @LGNAME);
        RETURN;
    END

    -- Kiểm tra xem tài khoản đang xóa có đang đăng nhập (hoạt động) hay không
    IF EXISTS (SELECT 1 FROM sys.dm_exec_sessions WHERE login_name = @LGNAME AND session_id <> @@SPID)
    BEGIN
        RAISERROR (N'Tài khoản [%s] đang có phiên đăng nhập hoạt động. Không thể xóa lúc này!', 16, 1, @LGNAME);
        RETURN;
    END

    -- 2. Phép chiếu trước: Lấy tên user (chỉ cột name)
    SELECT @USERNAME = u.name
    FROM sys.server_principals l
    JOIN sys.database_principals u ON u.sid = l.sid
    WHERE l.name = @LGNAME;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Bước 1: Xóa User khỏi Database (nếu tồn tại)
        IF @USERNAME IS NOT NULL
        BEGIN
            SET @stmt = 'DROP USER [' + @USERNAME + ']';
            EXEC sp_executesql @stmt;
        END

        -- Bước 2: Xóa Login khỏi Server
        SET @stmt = 'DROP LOGIN [' + @LGNAME + ']';
        EXEC sp_executesql @stmt;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- Nếu trước đó chưa tạo role securityadmin thì chạy lệnh này
-- ALTER SERVER ROLE [securityadmin] ADD MEMBER [login_name];