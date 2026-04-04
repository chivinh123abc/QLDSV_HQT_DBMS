USE [QLDSV_HTC]
GO

-- =======================================================================
-- FILE: 010-Index_Suggestions.sql
-- DESCRIPTION: Các Non-Clustered Covering Index đề xuất để Optimizer 
--              có thể lợi dụng tối đa trong các Query mới tối ưu.
--
-- QUY TẮC HIỆU SUẤT TỐI ƯU CỦA SQL:
-- 1. Cột nằm trong WHERE / JOIN ON: Cho vào phần tạo Index.
-- 2. Cột truy vấn lấy lên ở SELECT nhưng không lọc logic: Cho vào INCLUDE.
-- Điều này cho phép Query Engine ĐỌC TRỰC TIẾP TỪ TREE CỦA INDEX thay vì
-- phải Seek về data row của Table gốc (Look-up).
-- =======================================================================

-- 1. Index hỗ trợ SP 005 (sp_LayDanhSachLopTinChi)
-- Lọc theo MAKHOA + NIENKHOA + HOCKY độ chọn lọc theo thứ tự.
-- INCLUDE các Field cần Select để Optimizer không Look-up lại bảng
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LOPTINCHI') AND name = 'IX_LOPTINCHI_FilterKhoaNienKhoa')
CREATE NONCLUSTERED INDEX IX_LOPTINCHI_FilterKhoaNienKhoa
ON [dbo].[LOPTINCHI] ([MAKHOA], [NIENKHOA], [HOCKY])
INCLUDE ([MALTC], [MAMH], [MAGV], [NHOM], [SOSVTOITHIEU]);
GO

-- 2. Index hỗ trợ SP 006, SP 007 (Lấy thông tin / Bảng điểm theo khóa duy nhất môn)
-- Lọc bằng MAMH, NHOM thường rất hẹp records. Nên để độ ưu tiên Selectivity.
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LOPTINCHI') AND name = 'IX_LOPTINCHI_FilterLopTinChi')
CREATE NONCLUSTERED INDEX IX_LOPTINCHI_FilterLopTinChi
ON [dbo].[LOPTINCHI] ([MAMH], [NHOM], [NIENKHOA], [HOCKY])
INCLUDE ([MALTC]);
GO

-- 3. Index hỗ trợ JOIN cho DANGKY theo môn học (SP 005, 006, 007, 009)
-- Cho phép JOIN qua MALTC và có sẵn check CANCELLATION (HUYDANGKY).
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('DANGKY') AND name = 'IX_DANGKY_MALTC_HUYDANGKY')
CREATE NONCLUSTERED INDEX IX_DANGKY_MALTC_HUYDANGKY
ON [dbo].[DANGKY] ([MALTC], [HUYDANGKY])
INCLUDE ([MASV], [DIEM_CC], [DIEM_GK], [DIEM_CK]);
GO

-- 4. Index hỗ trợ cho SP 008 (sp_LayPhieuDiem) & SP 009 (sp_LayBangDiemTongKet)
-- Tra cứu Sinh Viên trước rồi mới JOIN
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('DANGKY') AND name = 'IX_DANGKY_MASV_HUYDANGKY')
CREATE NONCLUSTERED INDEX IX_DANGKY_MASV_HUYDANGKY
ON [dbo].[DANGKY] ([MASV], [HUYDANGKY])
INCLUDE ([MALTC], [DIEM_CC], [DIEM_GK], [DIEM_CK]);
GO

-- 5. Index hỗ trợ SP 009 (sp_LayBangDiemTongKet)
-- Hỗ trợ sinh viên của chung 1 lớp
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('SINHVIEN') AND name = 'IX_SINHVIEN_MALOP')
CREATE NONCLUSTERED INDEX IX_SINHVIEN_MALOP
ON [dbo].[SINHVIEN] ([MALOP])
INCLUDE ([MASV], [HO], [TEN]);
GO
