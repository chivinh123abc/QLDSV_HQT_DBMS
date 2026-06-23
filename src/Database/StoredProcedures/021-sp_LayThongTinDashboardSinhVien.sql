USE [QLDSV_HTC]
GO

IF OBJECT_ID('sp_LayThongTinDashboardSinhVien', 'P') IS NOT NULL
    DROP PROCEDURE sp_LayThongTinDashboardSinhVien;
GO

CREATE PROCEDURE sp_LayThongTinDashboardSinhVien
    @MASV NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- [TỐI ƯU] P3: Factor out điều kiện lặp
    -- EXISTS check (dùng 2 lần ở L22, L43 cũ) → biến @HAS_DK
    -- TOP 1 subquery (dùng 2 lần ở L53-54 cũ) → biến @CUR_NK, @CUR_HK
    DECLARE @HAS_DK BIT = 0;
    DECLARE @CUR_NK NVARCHAR(9), @CUR_HK INT;

    IF EXISTS (SELECT 1 FROM DANGKY WHERE MASV = @MASV AND HUYDANGKY = 0)
    BEGIN
        SET @HAS_DK = 1;
        -- Truy vấn 1 lần duy nhất, lưu NK/HK mới nhất
        SELECT TOP 1 @CUR_NK = ltc.NIENKHOA, @CUR_HK = ltc.HOCKY
        FROM DANGKY dk
        INNER JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
        WHERE dk.MASV = @MASV AND dk.HUYDANGKY = 0
        ORDER BY ltc.NIENKHOA DESC, ltc.HOCKY DESC;
    END

    -- 1. Thông tin sinh viên + niên khóa lớp
    SELECT sv.MASV, sv.HO, sv.TEN, sv.MALOP, l.TENLOP, l.KHOAHOC, sv.DANGHIHOC
    FROM SINHVIEN sv
    LEFT JOIN LOP l ON l.MALOP = sv.MALOP
    WHERE sv.MASV = @MASV;

    -- 2. Học kỳ hiện tại
    IF @HAS_DK = 1
    BEGIN
        SELECT @CUR_NK AS NIENKHOA, @CUR_HK AS HOCKY,
               (SELECT COUNT(*) FROM DANGKY dk2
                INNER JOIN LOPTINCHI ltc2 ON ltc2.MALTC = dk2.MALTC
                WHERE dk2.MASV = @MASV AND dk2.HUYDANGKY = 0 
                  AND ltc2.NIENKHOA = @CUR_NK AND ltc2.HOCKY = @CUR_HK) AS SoMon;
    END
    ELSE
    BEGIN
        -- SV chưa đăng ký môn nào → lấy HK mới nhất từ LOPTINCHI
        SELECT TOP 1 NIENKHOA, HOCKY, 0 AS SoMon
        FROM LOPTINCHI
        ORDER BY NIENKHOA DESC, HOCKY DESC;
    END

    -- 3. Danh sách môn đã đăng ký ở HK hiện tại
    IF @HAS_DK = 1
    BEGIN
        SELECT ltc.MALTC, mh.MAMH, mh.TENMH, mh.SOTIET_LT, mh.SOTIET_TH,
               ISNULL(gv.HO + ' ' + gv.TEN, ltc.MAGV) AS HOTEN_GV,
               dk.DIEM_CC, dk.DIEM_GK, dk.DIEM_CK
        FROM DANGKY dk
        INNER JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
        INNER JOIN MONHOC mh ON mh.MAMH = ltc.MAMH
        LEFT JOIN GIANGVIEN gv ON gv.MAGV = ltc.MAGV
        WHERE dk.MASV = @MASV AND dk.HUYDANGKY = 0
          AND ltc.NIENKHOA = @CUR_NK
          AND ltc.HOCKY = @CUR_HK
        ORDER BY mh.TENMH;
    END
    ELSE
    BEGIN
        -- Trả result set rỗng với cùng schema
        SELECT TOP 0 CAST(0 AS INT) AS MALTC, CAST('' AS NCHAR(10)) AS MAMH, 
               CAST('' AS NVARCHAR(50)) AS TENMH, CAST(0 AS INT) AS SOTIET_LT, 
               CAST(0 AS INT) AS SOTIET_TH, CAST('' AS NVARCHAR(100)) AS HOTEN_GV,
               CAST(NULL AS FLOAT) AS DIEM_CC, CAST(NULL AS FLOAT) AS DIEM_GK, 
               CAST(NULL AS FLOAT) AS DIEM_CK;
    END
END
GO

-- Cấp quyền EXEC cho nhóm SV
GRANT EXECUTE ON sp_LayThongTinDashboardSinhVien TO SV;
GO

