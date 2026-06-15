USE [QLDSV_HTC]
GO

IF NOT EXISTS (SELECT 1 FROM sys.types WHERE name = 'GradeEntryType' AND is_table_type = 1)
BEGIN
    CREATE TYPE dbo.GradeEntryType AS TABLE (
        MALTC INT,
        MASV NVARCHAR(15),
        DIEM_CC FLOAT,
        DIEM_GK FLOAT,
        DIEM_CK FLOAT
    );
END
GO

-- =============================================
-- Description: Cập nhật điểm sinh viên hàng loạt
-- =============================================
CREATE OR ALTER PROCEDURE sp_CapNhatDiem
    @Grades dbo.GradeEntryType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Verify that all students are validly registered
        IF EXISTS (
            SELECT 1 
            FROM @Grades g
            LEFT JOIN DANGKY dk ON dk.MALTC = g.MALTC AND dk.MASV = g.MASV AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
            WHERE dk.MASV IS NULL
        )
        BEGIN
            THROW 50000, N'Có sinh viên không đăng ký lớp tín chỉ này hoặc lớp không tồn tại.', 1;
        END

        -- Perform bulk update
        UPDATE dk
        SET dk.DIEM_CC = g.DIEM_CC,
            dk.DIEM_GK = g.DIEM_GK,
            dk.DIEM_CK = g.DIEM_CK
        FROM DANGKY dk
        JOIN @Grades g ON dk.MALTC = g.MALTC AND dk.MASV = g.MASV;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
