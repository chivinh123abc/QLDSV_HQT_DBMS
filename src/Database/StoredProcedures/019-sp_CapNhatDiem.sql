USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Cập nhật điểm sinh viên
-- =============================================
CREATE OR ALTER PROCEDURE sp_CapNhatDiem
    @MALTC INT,
    @MASV NVARCHAR(15),
    @DIEM_CC FLOAT = NULL,
    @DIEM_GK FLOAT = NULL,
    @DIEM_CK FLOAT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM DANGKY WHERE MALTC = @MALTC AND MASV = @MASV)
        BEGIN
            THROW 50000, N'Sinh viên không đăng ký lớp tín chỉ này hoặc lớp không tồn tại.', 1;
        END

        UPDATE DANGKY
        SET DIEM_CC = @DIEM_CC,
            DIEM_GK = @DIEM_GK,
            DIEM_CK = @DIEM_CK
        WHERE MALTC = @MALTC AND MASV = @MASV AND (HUYDANGKY = 0 OR HUYDANGKY IS NULL);
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
