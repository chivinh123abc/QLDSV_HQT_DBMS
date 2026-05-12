CREATE OR ALTER PROCEDURE sp_LayBangDiemMonHocCuaMotLopTinChi
    @NIENKHOA NVARCHAR(9) = NULL,
    @HOCKY INT = NULL,
    @MAMH NVARCHAR(10) = NULL,
    @NHOM INT = NULL,
    @MASV NVARCHAR(20) = NULL,
    @TENSV NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        dk.MALTC,
        sv.MASV,
        sv.HO,
        sv.TEN,
        dk.DIEM_CC,
        dk.DIEM_GK,
        dk.DIEM_CK,
        CASE
            WHEN dk.DIEM_CC IS NULL OR dk.DIEM_GK IS NULL OR dk.DIEM_CK IS NULL
                THEN NULL
            ELSE dk.DIEM_CC * 0.1 + dk.DIEM_GK * 0.3 + dk.DIEM_CK * 0.6
        END AS DIEM_HET_MON
    FROM DANGKY dk
    JOIN SINHVIEN sv ON sv.MASV = dk.MASV
    JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
    WHERE (@MAMH IS NULL OR @MAMH = '' OR ltc.MAMH = @MAMH)
      AND (@NHOM IS NULL OR @NHOM = 0 OR ltc.NHOM = @NHOM)
      AND (@NIENKHOA IS NULL OR @NIENKHOA = '' OR ltc.NIENKHOA = @NIENKHOA)
      AND (@HOCKY IS NULL OR @HOCKY = 0 OR ltc.HOCKY = @HOCKY)
      AND (@MASV IS NULL OR @MASV = '' OR sv.MASV LIKE '%' + @MASV + '%')
      AND (@TENSV IS NULL OR @TENSV = '' OR (sv.HO + ' ' + sv.TEN) LIKE N'%' + @TENSV + '%')
      AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
    ORDER BY ltc.MALTC, sv.TEN, sv.HO;
END
GO
GO
