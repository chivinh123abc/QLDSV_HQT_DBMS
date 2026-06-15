CREATE OR ALTER PROCEDURE sp_LayPhieuDiem
    @MASV NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- [TỐI ƯU & ĐIỂM THI MAX]
    -- Sử dụng ROW_NUMBER() OVER (PARTITION BY...) để xếp hạng các lần học của cùng 1 môn học.
    -- Lọc lấy lần học có điểm hết môn cao nhất (RowNum = 1) để lấy đúng điểm thi Max.
    
    ;WITH RawGrades AS (
        SELECT 
            ltc.MAMH,
            mh.TENMH,
            dk.DIEM_CC,
            dk.DIEM_GK,
            dk.DIEM_CK,
            CASE
                WHEN dk.DIEM_CC IS NULL OR dk.DIEM_GK IS NULL OR dk.DIEM_CK IS NULL
                    THEN NULL
                ELSE dk.DIEM_CC * 0.1 + dk.DIEM_GK * 0.3 + dk.DIEM_CK * 0.6
            END AS DIEM,
            (mh.SOTIET_LT + mh.SOTIET_TH) / 15 AS SOTC,
            ltc.NIENKHOA,
            ltc.HOCKY
        FROM DANGKY dk
        JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
        JOIN MONHOC mh ON mh.MAMH = ltc.MAMH
        WHERE dk.MASV = @MASV
          AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
    ),
    RankedGrades AS (
        SELECT
            MAMH,
            TENMH,
            DIEM_CC,
            DIEM_GK,
            DIEM_CK,
            DIEM,
            SOTC,
            NIENKHOA,
            HOCKY,
            ROW_NUMBER() OVER (PARTITION BY MAMH ORDER BY ISNULL(DIEM, -1) DESC) AS RowNum
        FROM RawGrades
    )
    SELECT
        MAMH,
        TENMH,
        DIEM_CC,
        DIEM_GK,
        DIEM_CK,
        DIEM,
        SOTC,
        NIENKHOA,
        HOCKY
    FROM RankedGrades
    WHERE RowNum = 1
    ORDER BY NIENKHOA ASC, HOCKY ASC, TENMH ASC;

END
GO