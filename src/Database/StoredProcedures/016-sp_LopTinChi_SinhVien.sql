USE [QLDSV_HTC]
GO

-- =============================================
-- Lấy danh sách Lớp Tín Chỉ đang mở theo Niên khóa & Học kỳ
-- Dành cho Sinh Viên: chỉ trả về các LTC chưa bị hủy, kèm trạng thái đã DK hay chưa
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayDanhSachLopTinChi_SinhVien]
    @NIENKHOA   NVARCHAR(9),
    @HOCKY      INT,
    @MASV       NVARCHAR(50)   -- để biết SV đã đăng ký LTC nào rồi
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ltc.MALTC,
        ltc.MAMH,
        mh.TENMH,
        ltc.NHOM,
        gv.HO + N' ' + gv.TEN AS HOTEN_GV,
        ltc.SOSVTOITHIEU,
        (
            SELECT COUNT(*)
            FROM DANGKY dk
            WHERE dk.MALTC = ltc.MALTC
              AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)
        ) AS SOSV_DANGKY,
        CASE
            WHEN EXISTS (
                SELECT 1 FROM DANGKY dk2
                WHERE dk2.MALTC = ltc.MALTC
                  AND dk2.MASV  = @MASV
                  AND (dk2.HUYDANGKY = 0 OR dk2.HUYDANGKY IS NULL)
            ) THEN 1
            ELSE 0
        END AS DA_DANGKY
    FROM LOPTINCHI ltc
    INNER JOIN MONHOC mh ON mh.MAMH = ltc.MAMH
    INNER JOIN GIANGVIEN gv ON gv.MAGV = ltc.MAGV
    WHERE ltc.NIENKHOA = @NIENKHOA
      AND ltc.HOCKY    = @HOCKY
      AND (ltc.HUYLOP  = 0 OR ltc.HUYLOP IS NULL)
    ORDER BY mh.TENMH, ltc.NHOM;
END
GO