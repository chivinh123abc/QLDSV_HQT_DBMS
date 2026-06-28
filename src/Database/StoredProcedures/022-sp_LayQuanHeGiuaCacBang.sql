USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Lấy danh sách các quan hệ khóa ngoại (foreign keys) giữa các bảng
-- Cấp quyền: Hệ thống sử dụng connection string SA để truy vấn metadata
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayQuanHeGiuaCacBang]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        parent_tab.name AS ParentTable,
        parent_col.name AS ParentColumn,
        ref_tab.name AS ReferencedTable,
        ref_col.name AS ReferencedColumn
    FROM sys.foreign_keys AS fk
    INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
    INNER JOIN sys.tables AS parent_tab ON fk.parent_object_id = parent_tab.object_id
    INNER JOIN sys.columns AS parent_col ON fkc.parent_object_id = parent_col.object_id AND fkc.parent_column_id = parent_col.column_id
    INNER JOIN sys.tables AS ref_tab ON fk.referenced_object_id = ref_tab.object_id
    INNER JOIN sys.columns AS ref_col ON fkc.referenced_object_id = ref_col.object_id AND fkc.referenced_column_id = ref_col.column_id;
END
GO
