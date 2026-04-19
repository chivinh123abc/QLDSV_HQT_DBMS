USE [QLDSV_HTC]
GO
-- =============================================
-- PGV/SV đăng ký một Lớp Tín Chỉ
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_DangKyLopTinChi]
    @MASV   NVARCHAR(50),
    @MALTC  INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra LTC tồn tại và chưa bị hủy
    IF NOT EXISTS (SELECT 1 FROM LOPTINCHI WHERE MALTC = @MALTC AND (HUYLOP = 0 OR HUYLOP IS NULL))
    BEGIN
        RAISERROR(N'Lớp tín chỉ không tồn tại hoặc đã bị hủy.', 16, 1);
        RETURN;
    END

    -- Kiểm tra sinh viên có bị nghỉ học không
    IF EXISTS (SELECT 1 FROM SINHVIEN WHERE MASV = @MASV AND DANGHIHOC = 1)
    BEGIN
        RAISERROR(N'Sinh viên này đã nghỉ học, không thể đăng ký lớp tín chỉ.', 16, 1);
        RETURN;
    END

    -- Lấy thông tin LTC đang muốn đăng ký
    DECLARE @MAMH_NEW NVARCHAR(10), @NIENKHOA_NEW NVARCHAR(9), @HOCKY_NEW INT;
    SELECT @MAMH_NEW = MAMH, @NIENKHOA_NEW = NIENKHOA, @HOCKY_NEW = HOCKY 
    FROM LOPTINCHI WHERE MALTC = @MALTC;

    -- Chặn đăng ký lớp tín chỉ trong quá khứ (Không ai được bypass)
    DECLARE @NOW DATE = GETDATE();
    DECLARE @CURRENT_MONTH INT = MONTH(@NOW);
    DECLARE @CURRENT_YEAR INT = YEAR(@NOW);
    DECLARE @CUR_NIENKHOA_START INT, @CUR_HOCKY INT;

    -- Xác định niên khóa hiện tại: HK bắt đầu tháng 9
    IF @CURRENT_MONTH >= 9
        SET @CUR_NIENKHOA_START = @CURRENT_YEAR;
    ELSE
        SET @CUR_NIENKHOA_START = @CURRENT_YEAR - 1;

    -- Xác định học kỳ hiện tại: HK1 (T9-T1), HK2 (T2-T6), HK3 (T7-T8)
    IF @CURRENT_MONTH >= 9 OR @CURRENT_MONTH = 1
        SET @CUR_HOCKY = 1;
    ELSE IF @CURRENT_MONTH >= 2 AND @CURRENT_MONTH <= 6
        SET @CUR_HOCKY = 2;
    ELSE
        SET @CUR_HOCKY = 3;

    DECLARE @CUR_NK NVARCHAR(9) = CAST(@CUR_NIENKHOA_START AS NVARCHAR) + '-' + CAST(@CUR_NIENKHOA_START + 1 AS NVARCHAR);

    -- So sánh: nếu LTC thuộc quá khứ → chặn
    IF @NIENKHOA_NEW < @CUR_NK OR (@NIENKHOA_NEW = @CUR_NK AND @HOCKY_NEW < @CUR_HOCKY)
    BEGIN
        RAISERROR(N'Không thể thao tác trên lớp tín chỉ trong quá khứ.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        -- Sử dụng SERIALIZABLE để tránh Race Condition (Đăng ký trùng môn cùng lúc)
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
        BEGIN TRAN;

    -- Nếu đã đăng ký lớp này rồi (chưa hủy), báo lỗi
    IF EXISTS (SELECT 1 FROM DANGKY WHERE MASV = @MASV AND MALTC = @MALTC AND (HUYDANGKY = 0 OR HUYDANGKY IS NULL))
    BEGIN
        RAISERROR(N'Bạn đã đăng ký lớp tín chỉ này rồi.', 16, 1);
        RETURN;
    END

    -- 1. Chặn đăng ký trùng môn học trong cùng học kỳ
    IF EXISTS (
        SELECT 1 
        FROM DANGKY dk
        JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
        WHERE dk.MASV = @MASV
          AND ltc.MAMH = @MAMH_NEW
          AND ltc.NIENKHOA = @NIENKHOA_NEW
          AND ltc.HOCKY = @HOCKY_NEW
          AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
          AND dk.MALTC <> @MALTC
    )
    BEGIN
        RAISERROR(N'Bạn đã đăng ký một lớp khác cho môn học này trong cùng học kỳ.', 16, 1);
        RETURN;
    END

    -- 2. Chặn đăng ký nếu đã có điểm DIEM_GK cho môn này
    --    Nếu ĐÃ CÓ điểm GK → kiểm tra đậu/rớt:
    --      - Đậu (DIEM_GK >= 5): chặn hoàn toàn (đã hoàn thành môn)
    --      - Rớt (DIEM_GK < 5): chỉ được đăng ký lại ở HK SAU, không được ở HK hiện tại
    IF EXISTS (
        SELECT 1
        FROM DANGKY dk
        JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
        WHERE dk.MASV = @MASV
          AND ltc.MAMH = @MAMH_NEW
          AND dk.DIEM_GK IS NOT NULL
          AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
    )
    BEGIN
        -- Kiểm tra có bản ghi nào ĐẬU không
        IF EXISTS (
            SELECT 1
            FROM DANGKY dk
            JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
            WHERE dk.MASV = @MASV
              AND ltc.MAMH = @MAMH_NEW
              AND dk.DIEM_GK >= 5
              AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
        )
        BEGIN
            RAISERROR(N'Bạn đã hoàn thành và đạt môn học này rồi, không thể đăng ký lại.', 16, 1);
            RETURN;
        END

        -- Rớt (DIEM_GK < 5) đã được xử lý tự nhiên: Sinh viên rớt ở HK trước 
        -- sẽ không bị chặn bởi bất kỳ điều kiện nào ở đây và được phép đăng ký lại.
    END

    -- Nếu trước đó đã đăng ký nhưng đã hủy → kích hoạt lại
    IF EXISTS (SELECT 1 FROM DANGKY WHERE MASV = @MASV AND MALTC = @MALTC AND HUYDANGKY = 1)
    BEGIN
        UPDATE DANGKY SET HUYDANGKY = 0 WHERE MASV = @MASV AND MALTC = @MALTC;
        COMMIT TRAN;
        RETURN;
    END

    -- Đăng ký mới
    INSERT INTO DANGKY (MASV, MALTC, HUYDANGKY) VALUES (@MASV, @MALTC, 0);
    COMMIT TRAN;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- PGV/SV hủy đăng ký một Lớp Tín Chỉ
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_HuyDangKyLopTinChi]
    @MASV   NVARCHAR(50),
    @MALTC  INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM DANGKY WHERE MASV = @MASV AND MALTC = @MALTC AND (HUYDANGKY = 0 OR HUYDANGKY IS NULL))
    BEGIN
        RAISERROR(N'Bạn chưa đăng ký lớp tín chỉ này.', 16, 1);
        RETURN;
    END

    -- Lấy thông tin LTC đang muốn hủy đăng ký
    DECLARE @NIENKHOA_NEW NVARCHAR(9), @HOCKY_NEW INT;
    SELECT @NIENKHOA_NEW = NIENKHOA, @HOCKY_NEW = HOCKY 
    FROM LOPTINCHI WHERE MALTC = @MALTC;

    -- Chặn hủy đăng ký lớp tín chỉ trong quá khứ (Không ai được bypass)
    DECLARE @NOW DATE = GETDATE();
    DECLARE @CURRENT_MONTH INT = MONTH(@NOW);
    DECLARE @CURRENT_YEAR INT = YEAR(@NOW);
    DECLARE @CUR_NIENKHOA_START INT, @CUR_HOCKY INT;

    -- Xác định niên khóa hiện tại: HK bắt đầu tháng 9
    IF @CURRENT_MONTH >= 9
        SET @CUR_NIENKHOA_START = @CURRENT_YEAR;
    ELSE
        SET @CUR_NIENKHOA_START = @CURRENT_YEAR - 1;

    -- Xác định học kỳ hiện tại: HK1 (T9-T1), HK2 (T2-T6), HK3 (T7-T8)
    IF @CURRENT_MONTH >= 9 OR @CURRENT_MONTH = 1
        SET @CUR_HOCKY = 1;
    ELSE IF @CURRENT_MONTH >= 2 AND @CURRENT_MONTH <= 6
        SET @CUR_HOCKY = 2;
    ELSE
        SET @CUR_HOCKY = 3;

    DECLARE @CUR_NK NVARCHAR(9) = CAST(@CUR_NIENKHOA_START AS NVARCHAR) + '-' + CAST(@CUR_NIENKHOA_START + 1 AS NVARCHAR);

    -- So sánh: nếu LTC thuộc quá khứ → chặn
    IF @NIENKHOA_NEW < @CUR_NK OR (@NIENKHOA_NEW = @CUR_NK AND @HOCKY_NEW < @CUR_HOCKY)
    BEGIN
        RAISERROR(N'Không thể thao tác trên lớp tín chỉ trong quá khứ.', 16, 1);
        RETURN;
    END

    -- Kiểm tra sinh viên đã có điểm chưa (Chặn hủy đăng ký nếu đã có điểm)
    IF EXISTS (
        SELECT 1 
        FROM DANGKY 
        WHERE MASV = @MASV 
          AND MALTC = @MALTC 
          AND (DIEM_GK IS NOT NULL OR DIEM_CK IS NOT NULL)
    )
    BEGIN
        RAISERROR(N'Sinh viên đã có điểm, không thể hủy đăng ký lớp tín chỉ này.', 16, 1);
        RETURN;
    END

    -- Thực hiện hủy đăng ký
    BEGIN TRY
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
        BEGIN TRAN;

        UPDATE DANGKY SET HUYDANGKY = 1 WHERE MASV = @MASV AND MALTC = @MALTC;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
