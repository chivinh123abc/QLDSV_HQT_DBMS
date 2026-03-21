CREATE OR ALTER PROCEDURE sp_TaoVaiTro
    @ROLENAME NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @stmt NVARCHAR(MAX);

    -- 1. Kiểm tra xem Role đã tồn tại trong Database chưa
    IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @ROLENAME AND type = 'R')
    BEGIN
        RAISERROR ('Role [%s] đã tồn tại trong cơ sở dữ liệu!', 16, 1, @ROLENAME);
        RETURN;
    END

    -- 2. Thực thi lệnh tạo Role an toàn
    BEGIN TRY
        -- Sử dụng ngoặc vuông [] để phòng trường hợp tên Role có dấu cách hoặc trùng từ khóa
        SET @stmt = 'CREATE ROLE [' + @ROLENAME + ']';
        EXEC sp_executesql @stmt;

        PRINT 'Đã tạo thành công vai trò: ' + @ROLENAME;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH
END
GO