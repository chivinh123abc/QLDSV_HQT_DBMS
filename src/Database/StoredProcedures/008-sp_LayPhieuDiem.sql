CREATE OR ALTER PROCEDURE sp_LayPhieuDiem
    @MASV NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- [TỐI ƯU]
    -- CHỌN / CHIẾU TRƯỚC: Lọc qua bảng DANGKY lấy duy nhất các record 
    -- đăng ký thuộc sinh viên này (lượng data nhỏ, SARGable với MASV và HUYDANGKY)
    -- sau đó mới thực hiện JOIN với LOPTINCHI và MONHOC.
    
    ;WITH FilteredDK AS (
        SELECT 
            MALTC, 
            DIEM_CC, 
            DIEM_GK, 
            DIEM_CK
        FROM DANGKY
        WHERE MASV = @MASV
          -- SARGable: Giữ nguyên HUYDANGKY
          AND (HUYDANGKY = 0 OR HUYDANGKY IS NULL)
    )
    SELECT
        mh.TENMH,
        fdk.DIEM_CC,
        fdk.DIEM_GK,
        fdk.DIEM_CK,
        CASE
            WHEN fdk.DIEM_CC IS NULL OR fdk.DIEM_GK IS NULL OR fdk.DIEM_CK IS NULL
                THEN NULL
            ELSE fdk.DIEM_CC * 0.1 + fdk.DIEM_GK * 0.3 + fdk.DIEM_CK * 0.6
        END AS DIEM
    FROM FilteredDK fdk
    JOIN LOPTINCHI ltc ON ltc.MALTC = fdk.MALTC
    JOIN MONHOC mh ON mh.MAMH = ltc.MAMH
    ORDER BY mh.TENMH;

END
GO