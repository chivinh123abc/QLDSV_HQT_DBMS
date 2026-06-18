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
