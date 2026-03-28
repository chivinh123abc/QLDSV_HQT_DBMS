CREATE PROCEDURE sp_LayPhieuDiem
    @MASV NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        mh.TENMH,
        CASE
            WHEN dk.DIEM_CC IS NULL OR dk.DIEM_GK IS NULL OR dk.DIEM_CK IS NULL
                THEN NULL
            ELSE dk.DIEM_CC * 0.1 + dk.DIEM_GK * 0.3 + dk.DIEM_CK * 0.6
        END AS DIEM
    FROM DANGKY dk
    JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
    JOIN MONHOC mh ON mh.MAMH = ltc.MAMH
    WHERE dk.MASV = @MASV
      AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
    ORDER BY mh.TENMH;
END
GO