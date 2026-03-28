CREATE PROCEDURE sp_LayDanhSachLopTinChi
    @NIENKHOA NVARCHAR(9),
    @HOCKY INT,
    @MAKHOA NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        mh.TENMH,
        ltc.NHOM,
        gv.HO + N' ' + gv.TEN AS HOTEN_GV,
        ltc.SOSVTOITHIEU,
        COUNT(dk.MASV) AS SOSV_DANGKY
    FROM LOPTINCHI ltc
    JOIN MONHOC mh ON mh.MAMH = ltc.MAMH
    JOIN GIANGVIEN gv ON gv.MAGV = ltc.MAGV
    LEFT JOIN DANGKY dk 
        ON dk.MALTC = ltc.MALTC
       AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
    WHERE ltc.NIENKHOA = @NIENKHOA
      AND ltc.HOCKY = @HOCKY
      AND ltc.MAKHOA = @MAKHOA
    GROUP BY 
        mh.TENMH,
        ltc.NHOM,
        gv.HO,
        gv.TEN,
        ltc.SOSVTOITHIEU
    ORDER BY mh.TENMH, ltc.NHOM;
END
GO