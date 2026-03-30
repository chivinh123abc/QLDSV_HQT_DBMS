CREATE OR ALTER PROCEDURE sp_LayDanhSachSinhVienDangKyLopTinChi
    @NIENKHOA NVARCHAR(9),
    @HOCKY INT,
    @MAMH NVARCHAR(10),
    @NHOM INT
AS
BEGIN
    SET NOCOUNT ON;

    -- [TỐI ƯU]
    -- 1. KHỬ PHÉP NỐI: NIENKHOA, HOCKY, MAMH, NHOM là khóa xác định 1 Lớp Tín Chỉ duy nhất.
    -- Thay vì mang bảng LOPTINCHI đi JOIN với DANGKY rồi mới lọc để thu về 1 môn, ta
    -- truy vấn lấy riêng MALTC ra thành 1 biến cục bộ. Từ đó, ta rút gọn câu JOIN
    -- chính chỉ còn 2 bảng (DANGKY và SINHVIEN) với bộ điều kiện nhỏ gọn nhất.
    --
    -- 2. THỨ TỰ ĐIỀU KIỆN AND: MAMH, NHOM thường cô lập bản ghi nhanh hơn HOCKY, 
    -- NIENKHOA nên ưu tiên để trước.
    -- Xử lý tham số
    DECLARE @MALTC INT;

    SELECT @MALTC = MALTC
    FROM LOPTINCHI
    WHERE MAMH = @MAMH
      AND NHOM = @NHOM
      AND NIENKHOA = @NIENKHOA
      AND HOCKY = @HOCKY;

    -- Nếu tìm được MALTC thì mới chạy lấy sinh viên (giảm thiểu xử lý rác do truyền sai params)
    IF @MALTC IS NOT NULL
    BEGIN
        SELECT
            sv.MASV,
            sv.HO,
            sv.TEN,
            CASE 
                WHEN sv.PHAI = 1 THEN N'Nữ'
                ELSE N'Nam'
            END AS PHAI,
            sv.MALOP
        FROM DANGKY dk
        JOIN SINHVIEN sv ON sv.MASV = dk.MASV
        -- 3. SARGABLE: Giữ nguyên HUYDANGKY nguyên thủy
        WHERE dk.MALTC = @MALTC
          AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
        ORDER BY sv.TEN, sv.HO;
    END

END
GO
