USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Base Pagination Dynamically
-- Cấp quyền: PGV (Tất cả), KHOA (Tất cả) 
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

    -- Gộp tính TotalCount qua Window Function luôn vào kết quả, giúp tiết kiệm lần đọc thứ hai.
    SET @Sql = N'
        SELECT ' + @SelectCols + ', COUNT(1) OVER() AS TotalCount
        FROM ' + @TableName + ' ' + @JoinClause + @Where + 
        @OrderByClause + N' 
        OFFSET ' + CAST((@PageNumber - 1) * @PageSize AS NVARCHAR(20)) + N' ROWS 
        FETCH NEXT ' + CAST(@PageSize AS NVARCHAR(20)) + N' ROWS ONLY;';

    EXEC sp_executesql @Sql;
END
GO
