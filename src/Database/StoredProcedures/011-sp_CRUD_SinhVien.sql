USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Lặp danh sách sinh viên
-- Cấp quyền: PGV (Tất cả), KHOA (Chỉ sinh viên thuộc khoa)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayDanhSachSinhVien]
    @MALOP NCHAR(10) = NULL  -- NULL = tất cả, có giá trị = lọc theo lớp
AS
BEGIN
    SET NOCOUNT ON;

    -- PGV/dbo: xem tất cả sinh viên (có thể lọc theo lớp)
    IF IS_MEMBER('PGV') = 1 OR USER_NAME() = 'dbo'
    BEGIN
        -- Chiếu trước: chỉ lấy cột cần thiết
        -- Chọn trước: WHERE trên bảng SINHVIEN trước khi JOIN
        SELECT 
            sv.MASV, sv.HO, sv.TEN, sv.PHAI,
            sv.DIACHI, sv.NGAYSINH, sv.MALOP,
            sv.DANGHIHOC,
            ISNULL(l.TENLOP, sv.MALOP) AS TENLOP
        FROM SINHVIEN sv
        LEFT JOIN LOP l ON l.MALOP = sv.MALOP
        WHERE @MALOP IS NULL OR sv.MALOP = @MALOP
        ORDER BY sv.MALOP, sv.MASV;
    END
    ELSE IF IS_MEMBER('KHOA') = 1
    BEGIN
        -- Xác định khoa của giảng viên hiện tại
        DECLARE @MAKHOA NVARCHAR(10);
        SELECT @MAKHOA = MAKHOA FROM GIANGVIEN WHERE MAGV = USER_NAME();

        -- Chọn trước: lọc lớp thuộc khoa → rồi mới JOIN sinh viên
        -- AND: MALOP (xác suất sai cao) đặt trước
        SELECT 
            sv.MASV, sv.HO, sv.TEN, sv.PHAI,
            sv.DIACHI, sv.NGAYSINH, sv.MALOP,
            sv.DANGHIHOC,
            ISNULL(l.TENLOP, sv.MALOP) AS TENLOP
        FROM SINHVIEN sv
        INNER JOIN LOP l ON l.MALOP = sv.MALOP
        WHERE l.MAKHOA = @MAKHOA
          AND (@MALOP IS NULL OR sv.MALOP = @MALOP)
        ORDER BY sv.MASV;
    END
END
GO


-- =============================================
-- Description: Thêm mới sinh viên
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ThemSinhVien]
    @MASV      NCHAR(10),
    @HO        NVARCHAR(50),
    @TEN       NVARCHAR(10),
    @PHAI      BIT,
    @DIACHI    NVARCHAR(100) = NULL,
    @NGAYSINH  DATE = NULL,
    @MALOP     NCHAR(10),
    @DANGHIHOC BIT = 0,
    @PASSWORD  NVARCHAR(40) = '123456'
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra MASV trùng (EXISTS tối ưu hơn COUNT)
    IF EXISTS (SELECT 1 FROM SINHVIEN WHERE MASV = @MASV)
    BEGIN
        RAISERROR(N'Mã sinh viên đã tồn tại.', 16, 1);
        RETURN;
    END

    -- Validate tuổi (Age >= 16)
    IF @NGAYSINH IS NOT NULL AND DATEDIFF(YEAR, @NGAYSINH, GETDATE()) < 16
    BEGIN
        RAISERROR(N'Sinh viên phải từ 16 tuổi trở lên.', 16, 1);
        RETURN;
    END

    -- Kiểm tra MALOP hợp lệ (khử phép nối: dùng EXISTS thay vì JOIN)
    IF NOT EXISTS (SELECT 1 FROM LOP WHERE MALOP = @MALOP)
    BEGIN
        RAISERROR(N'Mã lớp không tồn tại.', 16, 1);
        RETURN;
    END

    INSERT INTO SINHVIEN (MASV, HO, TEN, PHAI, DIACHI, NGAYSINH, MALOP, DANGHIHOC, PASSWORD)
    VALUES (@MASV, @HO, @TEN, @PHAI, @DIACHI, @NGAYSINH, @MALOP, @DANGHIHOC, @PASSWORD);
END
GO


-- =============================================
-- Description: Cập nhật thông tin sinh viên
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SuaSinhVien]
    @MASV_OLD  NCHAR(10),
    @MASV      NCHAR(10),
    @HO        NVARCHAR(50),
    @TEN       NVARCHAR(10),
    @PHAI      BIT,
    @DIACHI    NVARCHAR(100) = NULL,
    @NGAYSINH  DATE = NULL,
    @MALOP     NCHAR(10),
    @DANGHIHOC BIT = 0,
    @PASSWORD  NVARCHAR(40) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM SINHVIEN WHERE MASV = @MASV_OLD)
    BEGIN
        RAISERROR(N'Mã sinh viên gốc không tồn tại.', 16, 1);
        RETURN;
    END

    -- Nếu đổi MASV, kiểm tra mã mới có trùng không
    IF @MASV != @MASV_OLD AND EXISTS (SELECT 1 FROM SINHVIEN WHERE MASV = @MASV)
    BEGIN
        RAISERROR(N'Mã sinh viên mới đã tồn tại ở bản ghi khác.', 16, 1);
        RETURN;
    END

    -- Validate tuổi (Age >= 16)
    IF @NGAYSINH IS NOT NULL AND DATEDIFF(YEAR, @NGAYSINH, GETDATE()) < 16
    BEGIN
        RAISERROR(N'Sinh viên phải từ 16 tuổi trở lên.', 16, 1);
        RETURN;
    END

    -- Kiểm tra MALOP hợp lệ
    IF NOT EXISTS (SELECT 1 FROM LOP WHERE MALOP = @MALOP)
    BEGIN
        RAISERROR(N'Mã lớp không tồn tại.', 16, 1);
        RETURN;
    END

    UPDATE SINHVIEN
    SET MASV      = @MASV,
        HO        = @HO,
        TEN       = @TEN,
        PHAI      = @PHAI,
        DIACHI    = @DIACHI,
        NGAYSINH  = @NGAYSINH,
        MALOP     = @MALOP,
        DANGHIHOC = @DANGHIHOC,
        PASSWORD  = ISNULL(@PASSWORD, PASSWORD)  -- NULL = giữ nguyên password cũ
    WHERE MASV = @MASV_OLD;
END
GO


-- =============================================
-- Description: Xóa sinh viên
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_XoaSinhVien]
    @MASV NCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM SINHVIEN WHERE MASV = @MASV)
    BEGIN
        RAISERROR(N'Mã sinh viên không tồn tại.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        DELETE FROM SINHVIEN WHERE MASV = @MASV;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 547
            RAISERROR(N'Không thể xóa sinh viên do dữ liệu đang bị ràng buộc (ví dụ: đã đăng ký môn học hoặc có điểm).', 16, 1);
        ELSE
        BEGIN
            DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
            RAISERROR(@Err, 16, 1);
        END
    END CATCH
END
GO
