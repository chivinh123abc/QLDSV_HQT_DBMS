USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Đặt lại mật khẩu cho Sinh Viên (Plaintext)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ResetPasswordSinhVien]
    @MASV NVARCHAR(50),
    @PASSWORD NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra sinh viên tồn tại
    IF NOT EXISTS (SELECT 1 FROM SINHVIEN WHERE MASV = @MASV)
    BEGIN
        RAISERROR(N'Mã sinh viên không tồn tại.', 16, 1);
        RETURN;
    END

    -- Kiểm tra độ dài mật khẩu tối thiểu
    IF LEN(@PASSWORD) < 8
    BEGIN
        RAISERROR(N'Mật khẩu mới phải chứa ít nhất 8 ký tự.', 16, 1);
        RETURN;
    END

    -- Cập nhật mật khẩu plaintext
    UPDATE SINHVIEN
    SET PASSWORD = @PASSWORD
    WHERE MASV = @MASV;
END
GO
