CREATE PROCEDURE sp_LayDanhSachSinhVienDangKyLopTinChi
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
        CASE 
            WHEN sv.PHAI = 1 THEN N'Nữ'
            ELSE N'Nam'
        END AS PHAI,
        sv.MALOP
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
