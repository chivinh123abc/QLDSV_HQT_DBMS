**CHƯƠNG 5. BẢO MẬT VÀ PHÂN QUYỀN TRONG SQL SERVER**

Mục đích: Nắm vững cơ chế bảo mật của SQL Server, cách quản lý tài khoản người dùng, và các lệnh thuộc nhóm DCL (Data Control Language) để cấp phát hoặc thu hồi quyền truy cập.

---

### I. MÔ HÌNH BẢO MẬT 3 LỚP CỦA SQL SERVER

SQL Server áp dụng cơ chế bảo mật theo hình chóp (từ ngoài vào trong) gồm 3 mức độ kiểm soát:

1. **Mức Server (Server-level / Authentication):** Xác thực việc một người có được phép "bước vào" hệ thống SQL Server hay không. Nó được quản lý thông qua tài khoản đăng nhập gọi là **Login**.
2. **Mức Cơ sở dữ liệu (Database-level / Authorization):** Khi đã vào được Server, người đó có được phép truy cập vào một Cơ sở dữ liệu (Database) cụ thể nào đó hay không. Nó được quản lý thông qua người dùng CSDL gọi là **User**. _Lưu ý: Một Login ở mức Server phải được ánh xạ (mapping) với một User trong Database._
3. **Mức Đối tượng (Object-level / Permissions):** Khi đã vào được Database, User đó được làm gì (Đọc, Thêm, Xóa, Sửa, hay Thi hành) trên các bảng (Table), khung nhìn (View) hay thủ tục (Stored Procedure). Nó được quản lý thông qua **Quyền (Permissions)**.

---

### II. CÁC CHẾ ĐỘ XÁC THỰC (AUTHENTICATION MODES)

Khi cài đặt cấu hình hoặc quản trị Server, có 2 chế độ xác thực đăng nhập:

- **Windows Authentication Mode:** Người dùng sử dụng luôn tài khoản của hệ điều hành Windows (hoặc mạng Domain/Active Directory) để kết nối thẳng vào SQL Server mà không cần nhập lại mật khẩu. Chế độ này bảo mật tốt nhất nhưng chỉ phù hợp dùng trong mạng nội bộ (LAN).
- **Mixed Mode (SQL Server and Windows Authentication):** Hỗ trợ song song cả tài khoản Windows và tài khoản do chính SQL Server quản lý độc lập. Khi chọn chế độ này, tài khoản quản trị hệ thống tối cao mặc định của SQL Server là `sa` (System Administrator) sẽ được kích hoạt.

---

### III. QUẢN LÝ LOGIN VÀ USER BẰNG T-SQL

Thay vì dùng giao diện Management Studio, bạn có thể tạo tài khoản qua lệnh T-SQL.

**1. Tạo Server Login (Tài khoản đăng nhập)**

```sql
-- Tạo Login sử dụng xác thực của SQL Server
CREATE LOGIN [Ten_Login] WITH PASSWORD = 'Mat_Khau_Manh';

-- Tạo Login từ tài khoản Windows (Ví dụ máy tên là MYPC)
CREATE LOGIN [MYPC\Ten_Tai_Khoan_Win] FROM WINDOWS;

```

**2. Tạo Database User (Người dùng CSDL)**
Phải trỏ vào CSDL cụ thể trước khi tạo User và ánh xạ nó với Login đã tạo.

```sql
USE TenCoSoDuLieu;
GO
CREATE USER [Ten_User] FOR LOGIN [Ten_Login];

```

---

### IV. NGÔN NGỮ ĐIỀU KHIỂN DỮ LIỆU (DCL) - CẤP & THU HỒI QUYỀN

Có 3 câu lệnh T-SQL cơ bản để quản lý quyền cho User:

1. **GRANT (Cấp quyền):** Cho phép User hoặc Role thực hiện một hành động cụ thể trên đối tượng.

- _Cú pháp:_ `GRANT <Loại_Quyền> ON <Tên_Đối_Tượng> TO <Tên_User_Hoặc_Role>;`
- _Ví dụ:_ `GRANT SELECT, INSERT ON NhanVien TO user_KeToan;`

2. **REVOKE (Thu hồi quyền):** Hủy bỏ một quyền đã được cấp (bằng GRANT) hoặc đã bị từ chối (bằng DENY) trước đó.

- _Cú pháp:_ `REVOKE <Loại_Quyền> ON <Tên_Đối_Tượng> FROM <Tên_User_Hoặc_Role>;`

3. **DENY (Từ chối / Cấm):** Cấm tuyệt đối User thực hiện hành động. **DENY có mức độ ưu tiên cao nhất.** Nếu một User nằm trong một Group được cấp quyền (GRANT), nhưng chính User đó lại bị cấm (DENY), thì User đó vẫn KHÔNG có quyền thao tác.

- _Cú pháp:_ `DENY <Loại_Quyền> ON <Tên_Đối_Tượng> TO <Tên_User>;`

---

### V. CÁC LOẠI QUYỀN (PERMISSIONS)

Có hai nhóm quyền cơ bản mà quản trị viên cần nắm:

- **Quyền mức lệnh (Statement Permissions):** Cấp quyền thực hiện các câu lệnh DDL (Tạo/Sửa/Xóa cấu trúc). Thường cấp cho người quản trị hoặc lập trình viên.
- Ví dụ: `CREATE TABLE`, `CREATE DATABASE`, `CREATE PROCEDURE`, `BACKUP DATABASE`...

- **Quyền mức đối tượng (Object Permissions):** Cấp quyền thao tác trên dữ liệu thực tế (DML).
- `SELECT`, `INSERT`, `UPDATE`, `DELETE` (Dùng cho Table và View).
- `EXECUTE` (Dùng để chạy các Stored Procedure / Hàm UDF).
- `REFERENCES` (Quyền tạo khóa ngoại tham chiếu tới bảng khác).

---

### VI. VAI TRÒ (ROLES) TRONG SQL SERVER

Thay vì đi cấp quyền lắt nhắt cho từng User một, SQL Server quản lý theo **Roles (Vai trò/Nhóm)**. Gán User vào một Role thì User đó tự động thừa hưởng toàn bộ quyền của Role đó.

**1. Cấp Server (Fixed Server Roles):** Dùng để quản trị hệ thống máy chủ.

- `sysadmin`: Toàn quyền trên toàn bộ hệ thống (tương đương user `sa`).
- `securityadmin`: Có quyền tạo/quản lý Logins và Reset password.
- `dbcreator`: Có quyền tạo, thay đổi, xóa, restore bất kỳ cơ sở dữ liệu nào.

**2. Cấp Database (Fixed Database Roles):** Dùng để quản trị trong phạm vi 1 CSDL cụ thể.

- `db_owner`: Toàn quyền trên CSDL hiện tại.
- `db_datareader`: Cho phép User đọc (SELECT) dữ liệu từ tất cả các bảng trong CSDL.
- `db_datawriter`: Cho phép User thêm, xóa, sửa (INSERT, DELETE, UPDATE) trên mọi bảng.
- `db_accessadmin`: Có quyền thêm hoặc xóa các User của CSDL đó.
- `db_denydatareader` / `db_denydatawriter`: Cấm đọc / Cấm ghi dữ liệu.

**3. Vai trò tự định nghĩa (User-defined Database Roles):**
Khi các role mặc định không đáp ứng được nghiệp vụ thực tế, bạn có thể tự tạo Role mới, cấp các quyền tuỳ chỉnh, rồi nạp User vào Role đó.

```sql
-- Bước 1: Tạo Role mới
CREATE ROLE [Role_TruongPhong];

-- Bước 2: Cấp quyền cho Role
GRANT SELECT, UPDATE ON BangLuong TO [Role_TruongPhong];

-- Bước 3: Thêm User vào Role
ALTER ROLE [Role_TruongPhong] ADD MEMBER [NguyenVanA_User];

```
