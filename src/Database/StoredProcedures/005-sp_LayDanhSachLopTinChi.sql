CREATE OR ALTER PROCEDURE sp_LayDanhSachLopTinChi
    @NIENKHOA NVARCHAR(9),
    @HOCKY INT,
    @MAKHOA NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- [TỐI ƯU]
    -- 1. CHỌN TRƯỚC: Dùng CTE () để lọc riêng bảng LOPTINCHI 
    -- trước khi JOIN với các bảng khác để làm nhỏ số lượng bản ghi cần JOIN.
    -- 2. THỨ TỰ ĐIỀU KIỆN AND: Để MAKHOA lên trước vì tính phân loại (selectivity) thường cao hơn NIENKHOA/HOCKY.
    --    (Lưu ý: Mặc dù SQL Server Optimizer tự sắp xếp, nhưng việc viết code có ý đồ giúp tăng readability).
    -- 3. ĐIỀU KIỆN SARGable: Giữ nguyên (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL). 
    --    Tuyệt đối không dùng hàm ISNULL(dk.HUYDANGKY, 0) ở WHERE/ON để tránh vô hiệu hóa index.

    ;WITH FilteredLTC AS (
        SELECT 
            MALTC, 
            MAMH, 
            MAGV, 
            NHOM, 
            SOSVTOITHIEU
        FROM LOPTINCHI
        WHERE MAKHOA = @MAKHOA
          AND NIENKHOA = @NIENKHOA
          AND HOCKY = @HOCKY
    )
    SELECT 
        mh.TENMH,
        fltc.NHOM,
        gv.HO + N' ' + gv.TEN AS HOTEN_GV,
        fltc.SOSVTOITHIEU,
        COUNT(dk.MASV) AS SOSV_DANGKY
    FROM FilteredLTC fltc
    JOIN MONHOC mh ON mh.MAMH = fltc.MAMH
    JOIN GIANGVIEN gv ON gv.MAGV = fltc.MAGV
    LEFT JOIN DANGKY dk 
        ON dk.MALTC = fltc.MALTC
       AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
    GROUP BY 
        mh.TENMH,
        fltc.NHOM,
        gv.HO,
        gv.TEN,
        fltc.SOSVTOITHIEU
    ORDER BY mh.TENMH, fltc.NHOM;

END
GO