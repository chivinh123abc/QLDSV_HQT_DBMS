USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Base Pagination Dynamically
-- Cấp quyền: PGV (Tất cả), KHOA (Tất cả) 
-- [TỐI ƯU] P0 Security: QUOTENAME + table validation + parameterized OFFSET/FETCH
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_PhanTrangDong]
    @SelectCols NVARCHAR(MAX) = '*',
    @TableName NVARCHAR(255),
    @JoinClause NVARCHAR(MAX) = '',
    @WhereClause NVARCHAR(MAX) = '',
    @OrderBy NVARCHAR(255) = '',
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    -- P0: Validate table name exists (chống SQL injection qua tên bảng giả)
    IF OBJECT_ID(@TableName, 'U') IS NULL AND OBJECT_ID(@TableName, 'V') IS NULL
    BEGIN
        RAISERROR(N'Tên bảng không hợp lệ: %s', 16, 1, @TableName);
        RETURN;
    END

    DECLARE @Sql NVARCHAR(MAX);
    
    DECLARE @Where NVARCHAR(MAX) = '';
    IF LTRIM(RTRIM(@WhereClause)) <> ''
    BEGIN
        SET @Where = ' WHERE ' + @WhereClause;
    END

    DECLARE @OrderByClause NVARCHAR(255) = '';
    IF LTRIM(RTRIM(@OrderBy)) <> ''
    BEGIN
        SET @OrderByClause = ' ORDER BY ' + @OrderBy;
    END
    ELSE
    BEGIN
        SET @OrderByClause = ' ORDER BY (SELECT NULL)';
    END

    -- P0: Parameterize OFFSET/FETCH thay vì string concat
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Gộp tính TotalCount qua Window Function luôn vào kết quả, giúp tiết kiệm lần đọc thứ hai.
    -- P0: Dùng QUOTENAME cho @TableName
    SET @Sql = N'
        SELECT ' + @SelectCols + ', COUNT(1) OVER() AS TotalCount
        FROM ' + QUOTENAME(@TableName) + ' ' + @JoinClause + @Where + 
        @OrderByClause + N' 
        OFFSET @pOffset ROWS 
        FETCH NEXT @pPageSize ROWS ONLY;';

    EXEC sp_executesql @Sql, 
        N'@pOffset INT, @pPageSize INT', 
        @pOffset = @Offset, 
        @pPageSize = @PageSize;
END
GO

