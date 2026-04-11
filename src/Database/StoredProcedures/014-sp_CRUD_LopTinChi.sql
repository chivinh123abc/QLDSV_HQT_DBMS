USE [QLDSV_HTC]
GO

-- =============================================
-- Description: Thêm mới Lớp Tín Chỉ
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ThemLopTinChi]
    @NIENKHOA      NVARCHAR(9),
    @HOCKY         INT,
    @MAMH          NVARCHAR(10),
    @NHOM          INT,
    @MAGV          NVARCHAR(10),
    @MAKHOA        NVARCHAR(10),
    @SOSVTOITHIEU  INT,
    @HUYLOP        BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM LOPTINCHI WHERE NIENKHOA = @NIENKHOA AND HOCKY = @HOCKY AND MAMH = @MAMH AND NHOM = @NHOM)
    BEGIN
        RAISERROR(N'Lớp tín chỉ này đã tồn tại (Niên khóa, Học kỳ, Môn học, Nhóm).', 16, 1);
        RETURN;
    END

    INSERT INTO LOPTINCHI (NIENKHOA, HOCKY, MAMH, NHOM, MAGV, MAKHOA, SOSVTOITHIEU, HUYLOP)
    VALUES (@NIENKHOA, @HOCKY, @MAMH, @NHOM, @MAGV, @MAKHOA, @SOSVTOITHIEU, @HUYLOP);
    
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

-- =============================================
-- Description: Cập nhật Lớp Tín Chỉ
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SuaLopTinChi]
    @MALTC         INT,
    @NIENKHOA      NVARCHAR(9),
    @HOCKY         INT,
    @MAMH          NVARCHAR(10),
    @NHOM          INT,
    @MAGV          NVARCHAR(10),
    @MAKHOA        NVARCHAR(10),
    @SOSVTOITHIEU  INT,
    @HUYLOP        BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM LOPTINCHI WHERE MALTC = @MALTC)
    BEGIN
        RAISERROR(N'Mã lớp tín chỉ không tồn tại.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM LOPTINCHI 
               WHERE NIENKHOA = @NIENKHOA AND HOCKY = @HOCKY AND MAMH = @MAMH AND NHOM = @NHOM AND MALTC <> @MALTC)
    BEGIN
        RAISERROR(N'Lớp tín chỉ với thông tin mới đã tồn tại.', 16, 1);
        RETURN;
    END

    UPDATE LOPTINCHI
    SET NIENKHOA = @NIENKHOA,
        HOCKY = @HOCKY,
        MAMH = @MAMH,
        NHOM = @NHOM,
        MAGV = @MAGV,
        MAKHOA = @MAKHOA,
        SOSVTOITHIEU = @SOSVTOITHIEU,
        HUYLOP = @HUYLOP
    WHERE MALTC = @MALTC;
END
GO

-- =============================================
-- Description: Xoá Lớp Tín Chỉ
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_XoaLopTinChi]
    @MALTC INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM LOPTINCHI WHERE MALTC = @MALTC)
    BEGIN
        RAISERROR(N'Mã lớp tín chỉ không tồn tại.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM DANGKY WHERE MALTC = @MALTC)
    BEGIN
        RAISERROR(N'Không thể xóa lớp tín chỉ vì đã có sinh viên đăng ký.', 16, 1);
        RETURN;
    END

    DELETE FROM LOPTINCHI WHERE MALTC = @MALTC;
END
GO

-- =============================================
-- Description: Lấy danh sách Lớp Tín Chỉ theo lọc
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_LayDanhSachLopTinChi_Full]
    @NIENKHOA NVARCHAR(9) = NULL,
    @HOCKY    INT = NULL,
    @MAKHOA   NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ltc.MALTC,
        ltc.NIENKHOA,
        ltc.HOCKY,
        ltc.MAMH,
        mh.TENMH,
        ltc.NHOM,
        ltc.MAGV,
        gv.HO + ' ' + gv.TEN AS HOTEN_GV,
        ltc.MAKHOA,
        ltc.SOSVTOITHIEU,
        ltc.HUYLOP,
        (SELECT COUNT(*) FROM DANGKY dk WHERE dk.MALTC = ltc.MALTC AND (dk.HUYDANGKY = 0 OR dk.HUYDANGKY IS NULL)) AS SOSV_DANGKY
    FROM LOPTINCHI ltc
    INNER JOIN MONHOC mh ON ltc.MAMH = mh.MAMH
    INNER JOIN GIANGVIEN gv ON ltc.MAGV = gv.MAGV
    WHERE (@NIENKHOA IS NULL OR ltc.NIENKHOA = @NIENKHOA)
      AND (@HOCKY IS NULL OR ltc.HOCKY = @HOCKY)
      AND (@MAKHOA IS NULL OR ltc.MAKHOA = @MAKHOA)
    ORDER BY ltc.NIENKHOA DESC, ltc.HOCKY DESC, mh.TENMH, ltc.NHOM;
END
GO
