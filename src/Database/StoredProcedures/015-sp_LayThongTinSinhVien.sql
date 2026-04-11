USE [QLDSV_HTC]

-- =============================================
-- Lấy thông tin cơ bản của 1 Sinh Viên theo MASV
-- (Dùng cho trang đăng ký tín chỉ, SP chạy trong context dbo → có thể JOIN LOP)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayThongTinSinhVien]
    @MASV NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        sv.MASV,
        sv.HO,
        sv.TEN,
        sv.MALOP,
        ISNULL(l.TENLOP, sv.MALOP) AS TENLOP
    FROM SINHVIEN sv
    LEFT JOIN LOP l ON l.MALOP = sv.MALOP
    WHERE sv.MASV = @MASV;
END
GO

-- =============================================
-- Lấy danh sách Lớp Tín Chỉ đang mở theo Niên khóa & Học kỳ
-- Dành cho Sinh Viên: chỉ trả về các LTC chưa bị hủy, kèm trạng thái đã DK hay chưa
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayDanhSachLopTinChi_SinhVien]
    @NIENKHOA   NVARCHAR(9),
    @HOCKY      INT,
    @MASV       NVARCHAR(50)   -- để biết SV đã đăng ký LTC nào rồi
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ltc.MALTC,
        ltc.MAMH,
        mh.TENMH,
        ltc.NHOM,
        gv.HO + N' ' + gv.TEN AS HOTEN_GV,
        ltc.SOSVTOITHIEU,
        (
            SELECT COUNT(*)
            FROM DANGKY dk
            WHERE dk.MALTC = ltc.MALTC
              AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
        ) AS SOSV_DANGKY,
        CASE
            WHEN EXISTS (
                SELECT 1 FROM DANGKY dk2
                WHERE dk2.MALTC = ltc.MALTC
                  AND dk2.MASV  = @MASV
                  AND (dk2.HUYDANGKY = 0 OR dk2.HUYDANGKY IS NULL)
            ) THEN 1
            ELSE 0
        END AS DA_DANGKY
    FROM LOPTINCHI ltc
    INNER JOIN MONHOC mh ON mh.MAMH = ltc.MAMH
    INNER JOIN GIANGVIEN gv ON gv.MAGV = ltc.MAGV
    WHERE ltc.NIENKHOA = @NIENKHOA
      AND ltc.HOCKY    = @HOCKY
      AND (ltc.HUYLOP  = 0 OR ltc.HUYLOP IS NULL)
    ORDER BY mh.TENMH, ltc.NHOM;
END
GO

-- =============================================
-- Sinh viên đăng ký vào một Lớp Tín Chỉ
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

    -- Nếu đã đăng ký rồi (chưa hủy), báo lỗi
    IF EXISTS (SELECT 1 FROM DANGKY WHERE MASV = @MASV AND MALTC = @MALTC AND (HUYDANGKY = 0 OR HUYDANGKY IS NULL))
    BEGIN
        RAISERROR(N'Bạn đã đăng ký lớp tín chỉ này rồi.', 16, 1);
        RETURN;
    END

    -- Nếu trước đó đã đăng ký nhưng đã hủy → kích hoạt lại
    IF EXISTS (SELECT 1 FROM DANGKY WHERE MASV = @MASV AND MALTC = @MALTC AND HUYDANGKY = 1)
    BEGIN
        UPDATE DANGKY SET HUYDANGKY = 0 WHERE MASV = @MASV AND MALTC = @MALTC;
        RETURN;
    END

    -- Đăng ký mới
    INSERT INTO DANGKY (MASV, MALTC, HUYDANGKY) VALUES (@MASV, @MALTC, 0);
END
GO

-- =============================================
-- Sinh viên hủy đăng ký một Lớp Tín Chỉ
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

    UPDATE DANGKY SET HUYDANGKY = 1 WHERE MASV = @MASV AND MALTC = @MALTC;
END
GO

-- =============================================
-- Phân quyền cho nhóm [SV]
-- =============================================

GO