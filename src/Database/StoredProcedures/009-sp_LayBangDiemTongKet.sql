CREATE OR ALTER PROCEDURE sp_LayBangDiemTongKet
    @MALOP NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @cols NVARCHAR(MAX);
    DECLARE @sql  NVARCHAR(MAX);

    -- [TỐI ƯU]
    -- 1. GIẢM THIỂU ĐIỀU KIỆN LẶP / QUÉT 2 LẦN:
    -- Sử dụng Temp Table (#TEMP_DIEMTK) thay cho CTE. 
    -- CTE thực chất chạy truy vấn 2 lần (do SQL cấp phép chạy sub-query ngầm)
    -- nếu ta gọi nó 2 lần trong xử lý. Bằng cách insert vào Temp Table, dữ liệu 
    -- được materialize tại tempdb và được cấp phát statistics => Tốc độ đọc (khi
    -- lấy cột name và khi pivot) nhanh hơn cực nhiều.
    --
    -- 2. SARGABLE: (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL) được giữ nguyên.

    SELECT 
        sv.MASV + N' - ' + sv.HO + N' ' + sv.TEN AS MASV_HOTEN,
        mh.TENMH,
        MAX(
            CASE
                WHEN dk.DIEM_CC IS NULL OR dk.DIEM_GK IS NULL OR dk.DIEM_CK IS NULL
                    THEN NULL
                ELSE dk.DIEM_CC * 0.1 + dk.DIEM_GK * 0.3 + dk.DIEM_CK * 0.6
            END
        ) AS DIEM
    INTO #TEMP_DIEMTK
    FROM SINHVIEN sv
    -- Chọn lọc bằng MALOP ngay tại đây để giảm kích thước trước khi JOIN
    JOIN DANGKY dk     ON dk.MASV = sv.MASV
    JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
    JOIN MONHOC mh     ON mh.MAMH = ltc.MAMH
    WHERE sv.MALOP = @MALOP
      AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
    GROUP BY sv.MASV, sv.HO, sv.TEN, mh.TENMH;

    -- Kiểm tra và tạo danh sách cột MON HOC động
    SELECT @cols = STUFF((
        -- Lấy ra danh sách tên MH từ Temp Table thay vì Full scan lại 4 bảng
        SELECT DISTINCT ',' + QUOTENAME(TENMH)
        FROM #TEMP_DIEMTK
        ORDER BY ',' + QUOTENAME(TENMH)
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

    -- Return false/empty string result handle
    IF @cols IS NULL OR @cols = ''
    BEGIN
        SELECT CAST(NULL AS NVARCHAR(200)) AS MASV_HOTEN WHERE 1 = 0;
        
        IF OBJECT_ID('tempdb..#TEMP_DIEMTK') IS NOT NULL 
            DROP TABLE #TEMP_DIEMTK;
        RETURN;
    END

    -- Dynamic SQL cho PIVOT table
    SET @sql = N'
    SELECT MASV_HOTEN, ' + @cols + '
    FROM #TEMP_DIEMTK
    PIVOT
    (
        -- DIEM ở đây thực tế đã được MAX ở temp table, nhưng PIVOT vẫn yêu cầu
        -- aggregate function nên để MAX(DIEM)
        MAX(DIEM) FOR TENMH IN (' + @cols + ')
    ) p
    ORDER BY MASV_HOTEN;';

    EXEC sp_executesql @sql;

    -- Dọn rác
    IF OBJECT_ID('tempdb..#TEMP_DIEMTK') IS NOT NULL 
        DROP TABLE #TEMP_DIEMTK;

END
GO
