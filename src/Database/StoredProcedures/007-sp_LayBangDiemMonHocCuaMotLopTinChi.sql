CREATE OR ALTER PROCEDURE sp_LayBangDiemMonHocCuaMotLopTinChi
    @NIENKHOA NVARCHAR(9),
    @HOCKY INT,
    @MAMH NVARCHAR(10),
    @NHOM INT
AS
BEGIN
    SET NOCOUNT ON;

    -- [TỐI ƯU] Tương tự như sp_LayDanhSachSinhVienDangKyLopTinChi
    -- Lấy mã lớp tín chỉ từ bảng LOPTINCHI ra riêng trước (chọn/chiếu trước, 
    -- dựa vào các keys tạo thành tính độc nhất: NIENKHOA, HOCKY, MAMH, NHOM) 
    -- để tránh scan dư thừa ở bảng SINHVIEN/DANGKY.

    DECLARE @MALTC INT;

    -- Thể hiện selectivity của INDEX trên MAMH, NHOM tốt hơn
    SELECT @MALTC = MALTC
    FROM LOPTINCHI
    WHERE MAMH = @MAMH
      AND NHOM = @NHOM
      AND NIENKHOA = @NIENKHOA
      AND HOCKY = @HOCKY;

    IF @MALTC IS NOT NULL
    BEGIN
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
        FROM DANGKY dk
        JOIN SINHVIEN sv ON sv.MASV = dk.MASV
        WHERE dk.MALTC = @MALTC
          -- Giữ SARGable compliance cho SQL Optimizer
          AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
        ORDER BY sv.TEN, sv.HO;
    END

END
GO
