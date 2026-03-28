CREATE PROCEDURE sp_LayBangDiemMonHocCuaMotLopTinChi
    @NIENKHOA NVARCHAR(9),
    @HOCKY INT,
    @MAMH NVARCHAR(10),
    @NHOM INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
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
    FROM LOPTINCHI ltc
    JOIN DANGKY dk ON dk.MALTC = ltc.MALTC
    JOIN SINHVIEN sv ON sv.MASV = dk.MASV
    WHERE ltc.NIENKHOA = @NIENKHOA
      AND ltc.HOCKY = @HOCKY
      AND ltc.MAMH = @MAMH
      AND ltc.NHOM = @NHOM
      AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
    ORDER BY sv.TEN, sv.HO;
END
GO
