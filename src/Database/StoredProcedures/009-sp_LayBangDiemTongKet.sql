CREATE PROCEDURE sp_LayBangDiemTongKet
    @MALOP NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @cols NVARCHAR(MAX);
    DECLARE @sql  NVARCHAR(MAX);

    ;WITH MON AS
    (
        SELECT DISTINCT mh.TENMH
        FROM SINHVIEN sv
        JOIN DANGKY dk     ON dk.MASV = sv.MASV
        JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
        JOIN MONHOC mh     ON mh.MAMH = ltc.MAMH
        WHERE sv.MALOP = @MALOP
          AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
    )
    SELECT @cols = STUFF((
        SELECT ',' + QUOTENAME(TENMH)
        FROM MON
        ORDER BY TENMH
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

    IF @cols IS NULL OR @cols = ''
    BEGIN
        SELECT CAST(NULL AS NVARCHAR(200)) AS MASV_HOTEN WHERE 1 = 0;
        RETURN;
    END

    SET @sql = N'
    ;WITH DIEM_MAX AS
    (
        SELECT
            sv.MASV + N'' - '' + sv.HO + N'' '' + sv.TEN AS MASV_HOTEN,
            mh.TENMH,
            MAX(
                CASE
                    WHEN dk.DIEM_CC IS NULL OR dk.DIEM_GK IS NULL OR dk.DIEM_CK IS NULL
                        THEN NULL
                    ELSE dk.DIEM_CC * 0.1 + dk.DIEM_GK * 0.3 + dk.DIEM_CK * 0.6
                END
            ) AS DIEM
        FROM SINHVIEN sv
        JOIN DANGKY dk     ON dk.MASV = sv.MASV
        JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
        JOIN MONHOC mh     ON mh.MAMH = ltc.MAMH
        WHERE sv.MALOP = @MALOP
          AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
        GROUP BY sv.MASV, sv.HO, sv.TEN, mh.TENMH
    )
    SELECT MASV_HOTEN, ' + @cols + '
    FROM DIEM_MAX
    PIVOT
    (
        MAX(DIEM) FOR TENMH IN (' + @cols + ')
    ) p
    ORDER BY MASV_HOTEN;';

    EXEC sp_executesql @sql, N'@MALOP NVARCHAR(10)', @MALOP;
END
GO
