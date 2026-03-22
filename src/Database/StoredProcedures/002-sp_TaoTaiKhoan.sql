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

        -- Bước 4: Nếu là Admin, gán thêm quyền cấp Server (Ví dụ: securityadmin)
        IF (@ROLE = 'Admin')
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

EXEC sp_TaoTaiKhoan 'pgv', '123', 'user_pgv', 'pgv';
EXEC sp_TaoTaiKhoan 'khoa', '123', 'user_khoa', 'khoa';
EXEC sp_TaoTaiKhoan 'sv', 'sv', 'user_sv', 'sv';