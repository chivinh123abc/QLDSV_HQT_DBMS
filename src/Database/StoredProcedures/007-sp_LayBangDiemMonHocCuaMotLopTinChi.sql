CREATE OR ALTER PROCEDURE sp_LayBangDiemMonHocCuaMotLopTinChi
    @NIENKHOA NVARCHAR(9) = NULL,
    @HOCKY INT = NULL,
    @MAMH NVARCHAR(10) = NULL,
    @NHOM INT = NULL,
    @MASV NVARCHAR(20) = NULL,
    @TENSV NVARCHAR(100) = NULL,
    @MALOP NVARCHAR(20) = NULL,
    @MAKHOA NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- [TỐI ƯU]
    -- P2: Khử LEFT JOIN LOP → dùng EXISTS cho @MAKHOA (tránh JOIN thừa khi NULL)
    -- P3: Tách HO + ' ' + TEN concat → LIKE riêng HO/TEN (SARGable)
    -- P4: Reorder WHERE → @MASV (PK, selective nhất) lên đầu

    SELECT
        dk.MALTC,
        sv.MASV,
        sv.HO,
        sv.TEN,
        mh.TENMH,
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
    JOIN LOPTINCHI ltc ON ltc.MALTC = dk.MALTC
    JOIN MONHOC mh ON mh.MAMH = ltc.MAMH
    -- P2: Bỏ LEFT JOIN LOP — dùng EXISTS bên dưới
    WHERE (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
      -- P4: @MASV là PK → selective nhất → đặt trước
      AND (@MASV IS NULL OR @MASV = '' OR sv.MASV LIKE '%' + @MASV + '%')
      AND (@MAMH IS NULL OR @MAMH = '' OR ltc.MAMH = @MAMH)
      AND (@NHOM IS NULL OR @NHOM = 0 OR ltc.NHOM = @NHOM)
      AND (@NIENKHOA IS NULL OR @NIENKHOA = '' OR ltc.NIENKHOA = @NIENKHOA)
      AND (@HOCKY IS NULL OR @HOCKY = 0 OR ltc.HOCKY = @HOCKY)
      -- P3: SARGable — tách HO và TEN thay vì concat
      AND (@TENSV IS NULL OR @TENSV = '' OR sv.HO LIKE N'%' + @TENSV + '%' OR sv.TEN LIKE N'%' + @TENSV + '%')
      AND (@MALOP IS NULL OR @MALOP = '' OR sv.MALOP = @MALOP)
      -- P2: EXISTS thay LEFT JOIN LOP (chỉ chạy khi @MAKHOA có giá trị)
      AND (@MAKHOA IS NULL OR @MAKHOA = '' OR EXISTS (
          SELECT 1 FROM LOP WHERE MALOP = sv.MALOP AND MAKHOA = @MAKHOA
      ))
    ORDER BY ltc.MALTC, sv.TEN, sv.HO;
END
GO