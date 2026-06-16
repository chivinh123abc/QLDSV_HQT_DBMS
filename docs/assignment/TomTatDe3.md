# 📋 BẢNG THEO DÕI TÍNH NĂNG ĐỀ TÀI 3: QUẢN LÝ ĐIỂM SINH VIÊN HỆ TÍN CHỈ

Tài liệu này tóm tắt toàn bộ yêu cầu nghiệp vụ của **Đề tài 3 (môn Hệ quản trị Cơ sở dữ liệu SQL Server)** và trạng thái triển khai thực tế trên codebase dự án.

---

## 🗂️ 1. Cấu trúc Cơ sở Dữ liệu & Ràng buộc (Schema & Constraints)
Các bảng dữ liệu và ràng buộc được thiết lập đầy đủ trong file SQL Server DDL: **[QLDSV_HTC.sql](file:///d:/CODE%20PLAYGROUND%20Projects/HQTCSDL/QLDSV_HQT_DBMS/src/Database/Tables/QLDSV_HTC.sql)**.

*   [x] **a. Khoa (`KHOA`)**:
    *   `MAKHOA` (PK - nChar(10))
    *   `TENKHOA` (Unique, Not Null - nVarchar(50))
*   [x] **b. Lớp (`LOP`)**:
    *   `MALOP` (PK - nChar(10))
    *   `TENLOP` (Unique, Not Null - nVarchar(50))
    *   `KHOAHOC` (Not Null - nChar(9))
    *   `MAKHOA` (FK - nChar(10))
*   [x] **c. Sinh viên (`SINHVIEN`)**:
    *   `MASV` (PK - nChar(10))
    *   `HO` (Not Null - nVarchar(50)), `TEN` (Not Null - nVarchar(10))
    *   `MALOP` (FK - nChar(10))
    *   `PHAI` (Bit, default 0 - Nam: 0 / Nữ: 1)
    *   `NGAYSINH` (DateTime), `DIACHI` (nVarchar(100))
    *   `DANGHIHOC` (Bit, default 0 - Đang học: 0 / Nghỉ học: 1)
    *   `PASSWORD` (nVarchar(40), default '123456')
*   [x] **d. Môn học (`MONHOC`)**:
    *   `MAMH` (PK - nChar(10))
    *   `TENMH` (Unique, Not Null - nVarchar(50))
    *   `SOTIET_LT` (Int, Not Null), `SOTIET_TH` (Int, Not Null)
*   [x] **e. Giảng viên (`GIANGVIEN`)**:
    *   `MAGV` (PK - nChar(10))
    *   `HO` (Not Null - nVarchar(50)), `TEN` (Not Null - nVarchar(10))
    *   `HOCVI` (nVarchar(20)), `HOCHAM` (nVarchar(20)), `CHUYENMON` (nVarchar(50))
    *   `MAKHOA` (FK - nChar(10))
*   [x] **f. Lớp tín chỉ (`LOPTINCHI`)**:
    *   `MALTC` (PK - Int tự động tăng)
    *   `NIENKHOA` (nChar(9), Not Null), `HOCKY` (Int, 1 <= học kỳ <= 3)
    *   `MAMH` (FK - nChar(10)), `NHOM` (Int, >= 1)
    *   `MAGV` (FK - nChar(10)), `MAKHOA` (FK - nChar(10))
    *   `SOSVTOITHIEU` (SmallInt, > 0), `HUYLOP` (Bit, default 0)
    *   *Ràng buộc ràng buộc:* Unique Key (`NIENKHOA` + `HOCKY` + `MAMH` + `NHOM`)
*   [x] **g. Đăng ký (`DANGKY`)**:
    *   `MALTC` (FK - Int), `MASV` (FK - nChar(10))
    *   `DIEM_CC` (Int, 0 - 10)
    *   `DIEM_GK` (Float, 0 - 10, làm tròn đến 0.5)
    *   `DIEM_CK` (Float, 0 - 10, làm tròn đến 0.5)
    *   `HUYDANGKY` (Bit, default 0)
    *   *Ràng buộc ràng buộc:* PK (`MALTC` + `MASV`)

---

## 💻 2. Chi tiết Trạng thái Tính năng Nghiệp vụ

### 🔑 3.1. Đăng nhập (Authentication)
*   [x] **Đăng nhập Giảng viên / Quản trị viên**: Kết nối trực tiếp bằng tài khoản SQL Server cá nhân (`sa` hoặc tài khoản được cấp) qua cơ chế dynamic connection string.
*   [x] **Đăng nhập Sinh viên**: Sử dụng Mã sinh viên làm tài khoản và mật khẩu riêng. Hệ thống tự động dùng kết nối SQL Server của login dùng chung `sv` để gọi stored procedure `sp_DangNhap_SinhVien` kiểm tra thông tin.
*   [x] **Chuyển đổi nhãn (Label Switch)**: Giao diện đăng nhập tự động đổi nhãn thành "Mã SV" khi chọn vai trò Sinh viên.

### 📝 3.2. Nhập liệu (Data Entry - Chỉ dành cho PGV)
*   [x] **Nhập danh mục Lớp**:
    *   [x] Thêm, Sửa, Ghi lớp học mới vào cơ sở dữ liệu.
    *   [x] Xóa lớp (chỉ cho phép khi chưa có sinh viên đăng ký).
    *   [x] Phục hồi (Undo) trực quan trên giao diện form.
    *   [x] Thoát/Hủy phiên làm việc.
    *   [x] Chỉ hiển thị danh sách lớp thuộc Khoa quản lý (nếu là tài khoản khoa).
*   [x] **Nhập danh sách Sinh viên**:
    *   [x] Giao diện **SubForm 2 cấp** trực quan (chọn Lớp ở bảng trên để tải danh sách Sinh viên ở bảng dưới).
    *   [x] Thêm, Sửa, Ghi, Phục hồi (Undo), Thoát/Hủy thông tin sinh viên.
    *   [x] Khóa tính năng xóa đối với các sinh viên đã có lịch sử học tập/đăng ký tín chỉ.
*   [x] **Nhập danh mục Môn học**:
    *   [x] Thêm, Sửa, Ghi, Phục hồi (Undo), Thoát/Hủy môn học.
*   [x] **Mở Lớp tín chỉ**:
    *   [x] Thêm, Sửa, Ghi, Phục hồi (Undo), Thoát/Hủy thông tin mở lớp tín chỉ.
    *   [x] Ràng buộc ràng buộc logic (không được trùng nhóm trên cùng niên khóa, học kỳ, môn học).
*   [x] **Đăng ký Lớp tín chỉ (Dành cho Sinh viên)**:
    *   [x] Tự động hiển thị thông tin Sinh viên đăng nhập (Họ tên, Mã lớp).
    *   [x] Nhập Niên khóa + Học kỳ -> Tự động lọc danh sách lớp tín chỉ được mở (chưa bị hủy).
    *   [x] Hiển thị chi tiết: Mã môn, Tên môn, Nhóm, Họ tên GV, Số sinh viên đã đăng ký.
    *   [x] Thực hiện đăng ký hoặc hủy đăng ký.
*   [x] **Nhập điểm (Khoa hoặc PGV)**:
    *   [x] Chọn Niên khóa, Học kỳ, Môn học, Nhóm và nhấn **Bắt đầu** để tải danh sách sinh viên.
    *   [x] Giao diện bảng nhập điểm trực quan (Điểm Chuyên cần, Giữa kỳ, Cuối kỳ).
    *   [x] Tự động tính Điểm hết môn (Read-only): $Điểm\_CC \times 0.1 + Điểm\_GK \times 0.3 + Điểm\_CK \times 0.6$.
    *   [x] Chỉ ghi dữ liệu điểm về CSDL khi nhấn **Ghi điểm** (Lưu hàng loạt).

### 🛡️ 3.3. Phân quyền (Role-Based Access Control)
*   [x] **Nhóm PGV (Phòng giáo vụ)**:
    *   [x] Toàn quyền thao tác trên CSDL (được gán vai trò `db_owner`).
    *   [x] Được phép tạo, chỉnh sửa và xóa tài khoản cho nhóm quyền PGV và KHOA.
*   [x] **Nhóm KHOA**:
    *   [x] Bị chặn hoàn toàn quyền CUD (Thêm/Sửa/Xóa) trên các danh mục Khoa, Lớp, Giảng viên, Sinh viên, mở Lớp tín chỉ (Được bảo vệ bằng chính sách phân quyền Web và lệnh `DENY EXECUTE` các stored procedure ở database).
    *   [x] Được phép nhập điểm cho các lớp tín chỉ thuộc khoa mình.
    *   [x] Được phép tạo tài khoản mới cho nhóm quyền KHOA.
*   [x] **Nhóm SV (Sinh viên)**:
    *   [x] Chỉ có quyền đăng ký/hủy lớp tín chỉ và xem phiếu điểm của chính mình.
    *   [x] Sử dụng login dùng chung `sv` để kết nối hạn chế vào SQL Server.

### 🖨️ 3.4. In ấn & Báo cáo (Integration DevExpress Report)
Tích hợp thư viện báo cáo chuyên nghiệp xuất ra tệp PDF trực tiếp từ dữ liệu SQL Server:
*   [x] **Danh sách Lớp tín chỉ**: In các lớp tín chỉ đã mở (chưa hủy) theo Niên khóa, Học kỳ thuộc Khoa được chọn (sắp xếp tăng dần theo Tên môn học, Nhóm).
*   [x] **Danh sách Sinh viên đăng ký lớp tín chỉ**: In danh sách theo Niên khóa, Học kỳ, Môn học, Nhóm (sắp xếp tăng dần theo Tên + Họ sinh viên).
*   [x] **Bảng điểm môn học của 1 lớp tín chỉ**: In bảng điểm hết môn chi tiết của lớp tín chỉ (sắp xếp tăng dần theo Tên + Họ sinh viên).
*   [x] **Phiếu Điểm cá nhân**: In phiếu điểm của sinh viên theo Mã SV nhập vào (hiển thị điểm lớn nhất của môn học nếu đăng ký học lại nhiều lần, sắp xếp theo Tên môn học).
*   [x] **Bảng điểm tổng kết khóa học của Lớp**: Báo cáo dạng ma trận xoay chiều (**Cross-Tab**) hiển thị điểm tổng kết của tất cả sinh viên trong lớp đối với các môn học trong chương trình đào tạo.

### ⚙️ 3.5. Quản trị Tài khoản (User Administration)
*   [x] Giao diện tạo tài khoản người dùng trực quan, gán mật khẩu và phân quyền tương ứng (PGV, KHOA).
*   [x] Hỗ trợ thay đổi mật khẩu tài khoản.
*   [x] Cho phép xóa tài khoản (chỉ PGV được thực hiện xóa).

---

## 🚀 3. Các Tính năng Tối ưu & Nâng cao (Advanced Enhancements)
Dự án đã áp dụng các kỹ thuật tối ưu hóa hệ thống vượt trội so với yêu cầu cơ bản của đồ án:

*   [x] **Tối ưu hóa ghi điểm hàng loạt bằng Table-Valued Parameters (TVP)**:
    *   Đóng gói toàn bộ bảng điểm chỉnh sửa thành một Structured Parameter gửi lên SQL Server.
    *   Thực hiện cập nhật hàng loạt trong một stored procedure duy nhất (`sp_CapNhatDiem`) thông qua kiểu bảng `dbo.GradeEntryType`, thực thi trong 1 transaction an toàn. Tránh tối đa lỗi nghẽn mạng N+1 kết nối và cam kết tính toàn vẹn rollback khi gặp sự cố.
*   [x] **Chỉ mục phủ không cụm (Non-Clustered Covering Indexes)**:
    *   Thiết lập các chỉ mục tối ưu (`IX_LOPTINCHI_FilterKhoaNienKhoa`, `IX_DANGKY_MALTC_HUYDANGKY`, v.v.) dựa trên các cột lọc trong mệnh đề `WHERE` và `JOIN`, kèm mệnh đề `INCLUDE` các trường cần `SELECT`. Giúp SQL Server Optimizer đọc trực tiếp từ cây chỉ mục mà không phải thực hiện thao tác Key Lookup về bảng gốc.
*   [x] **Tạo Báo cáo Động (Dynamic Reports Editor)**:
    *   Tích hợp mô-đun thiết kế truy vấn động cho phép người dùng chọn bảng, chọn cột cần in, cấu hình mối quan hệ bảng, tự sinh câu lệnh SQL và xuất báo cáo PDF tùy biến trực tiếp trên giao diện web.
*   [x] **Bảo mật hai lớp (Dual-Layer RBAC Security)**:
    *   Bảo vệ ở tầng ứng dụng bằng ASP.NET Core Authorize Policy (Roles: `PGV`, `KHOA`, `SV`).
    *   Bảo vệ ở tầng CSDL bằng SQL Server Roles kết hợp lệnh `GRANT`/`DENY` cụ thể trên từng stored procedure và kiểu dữ liệu tự định nghĩa (User-Defined Type).
*   [x] **Tối ưu hóa stored procedures**:
    *   Áp dụng quy tắc lọc (chọn/chiếu) trước, kết nối (join) sau.
    *   Khử phép nối lặp trong câu lệnh truy vấn bằng biến cục bộ.
    *   Đặt các điều kiện dễ sai lên trước trong mệnh đề `AND` và ngược lại trong mệnh đề `OR` để lợi dụng cơ chế short-circuit evaluation của SQL Server.
