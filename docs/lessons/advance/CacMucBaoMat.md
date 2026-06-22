### 1. BẢO MẬT MỨC MÁY CHỦ (Server-Level Security)

Đây là "cánh cửa" đầu tiên. Để làm việc được với SQL Server, người dùng phải được xác thực (Authentication) để truy cập vào hệ thống máy chủ.

- **Đối tượng quản lý:** `Login` (Tài khoản đăng nhập).
- **Cơ chế xác thực (Authentication):**
- _Windows Authentication:_ Sử dụng chính tài khoản của hệ điều hành Windows để đăng nhập. Thích hợp cho mạng nội bộ an toàn.
- _SQL Server Authentication:_ Máy chủ SQL tự quản lý danh sách tài khoản và mật khẩu riêng (như tài khoản `sa`).

- **Vai trò mức máy chủ (Server Roles):** Giúp cấp quyền quản trị hệ thống mà không cần gán từng quyền lẻ tẻ.
- `sysadmin`: Quyền lực tối cao, có thể làm bất cứ điều gì trên máy chủ.
- `securityadmin`: Có quyền quản lý, tạo, sửa, xóa các tài khoản Logins và reset mật khẩu.
- `dbcreator`: Có quyền tạo, sửa đổi, khôi phục (restore) hoặc xóa (drop) bất kỳ cơ sở dữ liệu nào.
- `diskadmin`: Quản lý các tập tin vật lý trên đĩa của hệ thống.

### 2. BẢO MẬT MỨC CƠ SỞ DỮ LIỆU (Database-Level Security)

Vượt qua mức Server không có nghĩa là người dùng có thể tùy ý ra vào các Cơ sở dữ liệu (Database). Cánh cửa thứ hai là mức Database.

- **Đối tượng quản lý:** `User` (Người dùng CSDL). Mỗi `User` trong Database sẽ được ánh xạ (mapping) với một `Login` ở ngoài Server.
- **Vai trò mức CSDL (Database Roles):** Quản trị các nhóm người dùng chung một nhiệm vụ.
- `db_owner`: Chủ sở hữu CSDL, có quyền cao nhất trong phạm vi CSDL đó.
- `db_datareader`: Cho phép User đọc (`SELECT`) toàn bộ dữ liệu ở tất cả các bảng.
- `db_datawriter`: Cho phép User thêm, sửa, xóa (`INSERT`, `UPDATE`, `DELETE`) trên tất cả các bảng (nhưng không được thao tác cấu trúc bảng).
- `db_ddladmin`: Có quyền thực thi các lệnh DDL để tạo, xóa, sửa đổi cấu trúc (Bảng, View, Thủ tục...).
- `db_backupoperator`: Chỉ được quyền sao lưu (Backup) CSDL.

### 3. BẢO MẬT MỨC ĐỐI TƯỢNG (Object/Table-Level Security)

Khi đã vào được Database, hệ thống tiếp tục kiểm soát xem User đó được phép làm gì trên những Bảng (Table), Khung nhìn (View), hay Thủ tục (Stored Procedure) cụ thể.

- **Ngôn ngữ thao tác (DCL - Data Control Language):**
- `GRANT`: Cấp quyền. (VD: Cấp quyền SELECT trên bảng SinhVien).
- `REVOKE`: Thu hồi quyền đã cấp.
- `DENY`: Cấm tuyệt đối. (DENY luôn có độ ưu tiên cao nhất. Nếu một người vừa được GRANT, vừa bị DENY thì hệ thống sẽ áp dụng lệnh DENY).

- **Các loại quyền mức đối tượng thường gặp:**
- Khối thao tác dữ liệu: `SELECT`, `INSERT`, `UPDATE`, `DELETE`.
- Khối thực thi mã: `EXECUTE` (Cho phép chạy một Stored Procedure hoặc Function).
- Khối thay đổi cấu trúc: `ALTER`, `CONTROL`.

### 4. BẢO MẬT MỨC CỘT (Column/Field-Level Security)

Đây là một cơ chế phân quyền cực kỳ chi tiết của SQL Server, cho phép bạn thu hẹp quyền thao tác đến cấp độ Từng Cột (Field) trong một Bảng.

- **Tính ứng dụng:** Rất hữu ích đối với dữ liệu nhạy cảm.
- _Ví dụ:_ Bạn có bảng `NhanVien(MaNV, HoTen, SoDienThoai, Luong)`. Nhân viên Phòng Hành chính được phép xem bảng này, cập nhật `SoDienThoai`, nhưng tuyệt đối **không được xem và không được sửa** cột `Luong`.

- **Cú pháp minh họa:**

```sql
-- Cho phép User 'NhanSu' được xem toàn bộ thông tin nhân viên
GRANT SELECT ON NhanVien TO NhanSu;

-- Tuy nhiên, CẤM 'NhanSu' được xem cột Lương (Mức Cột)
DENY SELECT ON NhanVien(Luong) TO NhanSu;

-- Cho phép 'NhanSu' cập nhật Cột Số Điện Thoại (Mức Cột)
GRANT UPDATE ON NhanVien(SoDienThoai) TO NhanSu;

```

### MỞ RỘNG: BẢO MẬT MỨC DÒNG (Row-Level Security - RLS)

Từ phiên bản SQL Server 2016 trở đi, Microsoft cung cấp thêm tính năng **Row-Level Security (RLS)** để bảo mật ở mức độ **Dòng dữ liệu**.

- Tính năng này giúp giới hạn quyền xem dữ liệu dựa trên ngữ cảnh của người dùng đăng nhập.
- _Ví dụ:_ Bảng `HoaDon` chứa dữ liệu của toàn quốc. Khi Trưởng chi nhánh Hà Nội đăng nhập, anh ta gửi câu lệnh `SELECT * FROM HoaDon`, hệ thống sẽ tự động lọc và chỉ trả về các dòng hóa đơn thuộc chi nhánh Hà Nội, giấu đi các dòng của các chi nhánh khác mà không cần phải thay đổi mã nguồn truy vấn của ứng dụng.

---

### 📌 CÁC LƯU Ý QUAN TRỌNG (HỎI ĐÁP THỰC TẾ)

#### 1. Sự Khác Biệt Giữa "Có Tài Khoản Login" và "Gán Vai Trò Server Roles"
*   **Có cần Server Roles để truy cập database không?**
    *   **Không.** Bạn chỉ cần một tài khoản `Login` cơ bản để kết nối vào máy chủ (Cửa 1). Sau đó, chỉ cần `Login` đó được ánh xạ (map) sang một `User` trong database và được cấp quyền tương ứng (ví dụ: role `db_datareader`), họ hoàn toàn xem và làm việc với DB được.
    *   Các vai trò như `sysadmin`, `securityadmin`, `dbcreator`... chỉ dành cho tác vụ quản trị hệ thống mức cao.
*   **Tài khoản của Sinh viên và Giảng viên dùng Login nào trong dự án?**
    *   **Sinh viên (SV):** Không dùng Login riêng lẻ cho từng người (tránh quá tải SQL Server). Hệ thống sử dụng một **Login dùng chung (Shared Login)** duy nhất ở mức cấu hình (ví dụ: `user_sv`). Khi SV đăng nhập, ứng dụng Backend dùng `user_sv` kết nối vào SQL và gọi Stored Procedure `sp_DangNhap_SinhVien` để kiểm tra Mã SV và Mật khẩu lưu trong bảng dữ liệu `SINHVIEN` (Bảo mật mức ứng dụng).
    *   **Giảng viên / PGV / Khoa:** Có tài khoản **SQL Login riêng biệt** (thường trùng với Mã giảng viên `MAGV`). Việc này giúp ghi vết (Audit) chính xác ai đã thực hiện thay đổi điểm số hoặc chỉnh sửa dữ liệu để quy trách nhiệm khi có sự cố.
    *   **Tài khoản `sa`:** Chỉ dùng cho người quản trị tối cao (DBA) khi cài đặt, cấu hình hệ thống ban đầu. Tuyệt đối không nhúng `sa` vào mã nguồn kết nối của phần mềm chạy hàng ngày.

#### 2. Phân Biệt Sâu Hơn Giữa `REVOKE` và `DENY`
Nhiều người nhầm tưởng `REVOKE` (thu hồi) và `DENY` (cấm) là giống nhau. Tuy nhiên:
*   **`DENY` (Cấm tuyệt đối):** Có độ ưu tiên cao nhất. Nếu User bị `DENY` cá nhân trên một bảng, thì dù họ có thuộc nhóm (Role) được cấp `GRANT` đi chăng nữa, họ vẫn bị chặn xem bảng đó.
*   **`REVOKE` (Hủy bỏ lệnh gán):** Chỉ đưa quyền về trạng thái mặc định (chưa gán gì). Nếu ta `REVOKE` quyền cá nhân của một User, nhưng User đó tham gia vào một nhóm (Role) vẫn đang có quyền `GRANT`, thì User đó **vẫn có thể** truy cập bảng đó nhờ quyền kế thừa từ nhóm.

