# TÓM TẮT YÊU CẦU ĐỀ TÀI 3: QUẢN LÝ ĐIỂM SINH VIÊN THEO HỆ TÍN CHỈ

---

## Cấu trúc các bảng dữ liệu

*   **a. Khoa**: `MAKHOA` (PK), `TENKHOA` (Unique, Not Null).
*   **b. Lop**: `MALOP` (PK), `TENLOP` (Unique, Not Null), `KHOAHOC` (Not Null), `MAKHOA` (FK).
*   **c. Table Sinhvien**: `MASV` (PK), `HO`, `TEN`, `MALOP` (FK), `PHAI` (Bit, default Nam: 0 / Nữ: 1), `NGAYSINH`, `DIACHI`, `DANGHIHOC` (Bit, default 0), `PASSWORD` (default '123456').
*   **d. Cấu trúc của Table Monhoc**: `MAMH` (PK), `TENMH` (Unique, Not Null), `SOTIET_LT`, `SOTIET_TH`.
*   **e. Cấu trúc của Table GIANGVIEN**: `MAGV` (PK), `HO`, `TEN`, `HOCVI`, `HOCHAM`, `CHUYENMON`, `MAKHOA` (FK).
*   **f. Cấu trúc của Table LOPTINCHI**: `MALTC` (PK - Tự động), `NIENKHOA`, `HOCKY` (1-3), `MAMH` (FK), `NHOM` (>=1), `MAGV` (FK), `MAKHOA` (FK), `SOSVTOITHIEU` (>0), `HUYLOP` (Bit, default 0). 
    *   *Ràng buộc:* Unique Key (`NIENKHOA` + `HOCKY` + `MAMH` + `NHOM`).
*   **g. Cấu trúc của Table DANGKY**: `MALTC` (FK), `MASV` (FK), `DIEM_CC` (0-10, int), `DIEM_GK` (0-10, float, làm tròn 0.5), `DIEM_CK` (0-10, float, làm tròn 0.5), `HUYDANGKY` (Bit, default 0). 
    *   *Ràng buộc:* PK (`MALTC` + `MASV`).

---

## [Yêu cầu]

### 3.1. Đăng nhập
*   **Đối tượng**: GIẢNG VIÊN, SINH VIÊN.
*   **Quy trình**: Trước khi sử dụng phải đăng ký trước. Sinh viên dùng chung tài khoản login `sv` để kết nối, khi đăng nhập xong label Login chuyển thành Mã SV.

### 3.2. Nhập liệu
*   **Nhập danh mục lớp**: Các nút: Thêm, Xóa, Ghi, Phục hồi, Thoát. (Nhóm PGV nhập, chỉ thấy lớp thuộc khoa của mình quản lý).
*   **Nhập danh sách sinh viên**: Dạng **SubForm 2 cấp** (Lớp - Sinh viên). Các nút tương tự Nhập danh mục lớp. (Nhóm PGV nhập).
*   **Nhập môn học**: Các nút: Thêm, Xóa, Ghi, Phục hồi, Thoát. (Nhóm PGV nhập).
*   **Mở Lớp tín chỉ**: Các nút: Thêm, Xóa, Ghi, Phục hồi. (Nhóm PGV nhập).
*   **Đăng ký lớp tín chỉ**:
    *   Nhập Mã SV -> In thông tin SV (Họ, Tên, Mã lớp).
    *   Nhập Niên khóa, Học kỳ -> Tự động lọc ra lớp tín chỉ đã mở (chưa hủy) để đăng ký (gồm: MAMH, TENMH, Nhóm, Họ tên GV, Số SV đã đăng ký). (Nhóm SV thực hiện).
*   **Nhập điểm**: (Khoa hoặc PGV thực hiện)
    *   Nhập Niên khóa, Học kỳ, Môn học, Nhóm -> Click "Bắt đầu" -> Tải danh sách sinh viên đăng ký.
    *   Nhập điểm chuyên cần, giữa kỳ, cuối kỳ.
    *   Điểm hết môn (Read-only, tự động tính): `DIEM_CC * 0.1 + DIEM_GK * 0.3 + DIEM_CK * 0.6`.
    *   Click "Ghi điểm" mới lưu toàn bộ điểm về CSDL.

### 3.3. Phân quyền
*   **PGV (Phòng giáo vụ)**: Toàn quyền. Được tạo tài khoản cho nhóm PGV, Khoa.
*   **KHOA**: Không cho nhập Khoa, Lớp, Giảng viên, Sinh viên, mở Lớp tín chỉ. Được tạo tài khoản cho nhóm Khoa.
*   **SV (Sinh viên)**: Được đăng ký lớp tín chỉ, xem Phiếu điểm của chính mình. Dùng chung 1 login `sv` để kết nối.

### 3.4. In ấn
*   **Danh sách lớp tín chỉ**: Nhập Niên khóa, Học kỳ -> In lớp đã mở (chưa hủy) thuộc khoa đang chọn (Sắp xếp theo: Tên môn học, Nhóm).
*   **Danh sách sinh viên đăng ký lớp tín chỉ**: Nhập Niên khóa, Học kỳ, Môn học, Nhóm -> In danh sách SV đăng ký (Sắp xếp theo: Tên + Họ).
*   **Bảng điểm môn học của 1 lớp tín chỉ**: Nhập Niên khóa, Học kỳ, Môn học, Nhóm -> In bảng điểm hết môn của lớp đó (Sắp xếp theo: Tên + Họ).
*   **Phiếu Điểm**: Nhập/Chọn Mã SV -> In phiếu điểm của sinh viên đó (Cột: STT, Tên môn học, Điểm thi Max; Sắp xếp theo: Tên môn học).
*   **Bảng điểm tổng kết**: Nhập Mã lớp -> In bảng điểm tổng kết cuối khóa của lớp dạng **Cross-Tab** (Điểm thi lấy điểm lớn nhất).

### 3.5. Quản trị
*   Tạo tài khoản cho người dùng sử dụng phần mềm (Login, Password, Phân quyền làm việc tương ứng).
