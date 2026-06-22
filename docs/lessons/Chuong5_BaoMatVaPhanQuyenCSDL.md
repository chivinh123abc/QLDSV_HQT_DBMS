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

---

### GHI CHÚ TRÊN LỚP (Quicknote)

> Các câu hỏi, bài tập từ bài giảng trên lớp (ngày 7/3/2026).

**1. Câu hỏi: Login có được trùng không? Vì sao?**

> **❓ Câu hỏi trên lớp:** Nếu 1 Server có 10 Database, mỗi DB có 10 users → 100 username. Vậy Login có được trùng không?

**Trả lời:** Login **KHÔNG được trùng** vì tất cả Login đều nằm trong bảng hệ thống chung `master.sys.server_principals` (cấp Server). Mỗi Login là duy nhất trên toàn bộ SQL Server instance.

```sql
-- Kiểm tra danh sách Login trên Server
SELECT name, type_desc, create_date
FROM master.sys.server_principals
WHERE type IN ('S', 'U')  -- S = SQL Login, U = Windows Login
ORDER BY name;
```

**2. Câu hỏi: Username có được trùng không? Vì sao?**

**Trả lời:** Username **ĐƯỢC trùng nếu thuộc các Database khác nhau**, nhưng **KHÔNG được trùng trong cùng 1 Database**. Lý do: mỗi Database có bảng `sys.database_principals` riêng (cấp Database), nên tên User chỉ cần duy nhất trong phạm vi Database đó.

```sql
-- Xem danh sách User trong Database hiện tại
USE QLDSV_TC;
SELECT name, type_desc, authentication_type_desc
FROM sys.database_principals
WHERE type IN ('S', 'U')
ORDER BY name;
```

**3. Bài tập 1: Truy vấn lịch sử Backup gần nhất trên Device**

Sử dụng CTE (Common Table Expression) để lấy lịch sử backup từ lần ghi đè (overwrite) gần nhất trở đi:

```sql
WITH LatestInit AS (
    -- Tìm ID của lần backup ghi đè (position = 1) gần nhất trên Device này
    SELECT MAX(bs.backup_set_id) AS Last_Init_ID
    FROM msdb.dbo.backupset bs
    INNER JOIN msdb.dbo.backupmediafamily bmf
        ON bs.media_set_id = bmf.media_set_id
    WHERE bs.database_name = 'qldsv_htc'
      AND bmf.logical_device_name = 'DEVICE_QLDSV_TC'
      AND bs.position = 1
)
-- Chỉ hiển thị các backup từ lần ghi đè gần nhất trở đi
SELECT
    bs.backup_set_id,
    bs.media_set_id,
    bs.position,
    bs.backup_start_date
FROM msdb.dbo.backupset bs
INNER JOIN msdb.dbo.backupmediafamily bmf
    ON bs.media_set_id = bmf.media_set_id
WHERE bs.database_name = 'qldsv_htc'
  AND bmf.logical_device_name = 'DEVICE_QLDSV_TC'
  AND bs.backup_set_id >= (SELECT Last_Init_ID FROM LatestInit)
ORDER BY bs.backup_set_id;
```

> **Ghi chú thầy:** Cách tối ưu là lấy `MAX(bs.backup_start_date)` thay vì `MAX(bs.backup_set_id)`.

**4. Bài tập 2: SP lấy thông tin đăng nhập theo Login**

Cần JOIN giữa `sys.server_principals` (Login cấp Server) → `sys.database_principals` (User cấp DB) → `sys.database_role_members` (Role membership):

```sql
CREATE PROCEDURE sp_ThongTinDangNhap
    @tenlogin NVARCHAR(128)
AS
BEGIN
    SELECT
        u.name AS Username,
        u.name AS HoTen,
        r.name AS Rolename
    FROM sys.server_principals l
    JOIN sys.database_principals u ON l.sid = u.sid
    JOIN sys.database_role_members rm ON u.principal_id = rm.member_principal_id
    JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
    WHERE l.name = @tenlogin;
END
GO
```

> **Ghi chú thầy:** Cần tối ưu lại — lấy thêm `HoTen` từ bảng giảng viên (JOIN thêm bảng GiangVien), và có thể dùng `sys.sysusers` hoặc `sys.database_principals` tùy phiên bản.

**5. Bài tập 3: SP lấy thông tin tài khoản**

> **❓ Chưa ghi kịp đáp án trên lớp.** Dưới đây là đáp án bổ sung:

```sql
CREATE PROCEDURE sp_LayThongTinTaiKhoan
    @tenlogin NVARCHAR(128)
AS
BEGIN
    -- Lấy thông tin Login + User + Role + Họ tên giảng viên
    SELECT
        l.name AS LoginName,
        u.name AS Username,
        r.name AS RoleName,
        gv.HOTEN AS HoTenGiangVien
    FROM sys.server_principals l
    JOIN sys.database_principals u ON l.sid = u.sid
    LEFT JOIN sys.database_role_members rm ON u.principal_id = rm.member_principal_id
    LEFT JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
    LEFT JOIN GiangVien gv ON u.name = gv.MAGV  -- Liên kết với bảng nghiệp vụ
    WHERE l.name = @tenlogin;
END
GO

-- Test:
EXEC sp_LayThongTinTaiKhoan @tenlogin = 'GV001';
```

**6. Bài tập 4: SP tạo tài khoản mới (Login + User + Role)**

```sql
CREATE PROCEDURE sp_TaoTaiKhoan
    @LGNAME   VARCHAR(50),
    @PASS     VARCHAR(50),
    @USERNAME VARCHAR(50),
    @ROLE     VARCHAR(50)
AS
BEGIN
    DECLARE @ret INT

    -- Bước 1: Tạo Login (cấp Server)
    EXEC @ret = sp_addlogin @LGNAME, @PASS, 'QLDSV_TC'
    IF @ret = 1
    BEGIN
        RAISERROR (N'Login name bị trùng', 16, 1)
        RETURN
    END

    -- Bước 2: Tạo User trong Database (cấp Database)
    EXEC @ret = sp_grantdbaccess @LGNAME, @USERNAME
    IF @ret = 1
    BEGIN
        -- Rollback: xóa Login vừa tạo nếu User bị trùng
        EXEC sp_droplogin @LGNAME
        RAISERROR (N'User name bị trùng', 16, 1)
        RETURN
    END

    -- Bước 3: Gán Role cho User (cấp Database)
    EXEC sp_addrolemember @ROLE, @USERNAME

    -- Bước 4: Nếu Role là Admin → cấp thêm quyền Server-level
    IF (@ROLE = 'Admin')
        EXEC sp_addsrvrolemember @LGNAME, 'securityadmin'
END
GO

-- Test:
EXEC sp_TaoTaiKhoan
    @LGNAME   = 'GV001',
    @PASS     = 'P@ssw0rd123',
    @USERNAME = 'GV001_User',
    @ROLE     = 'GiangVien';
```

> **⚠️ Lưu ý:** `sp_addlogin`, `sp_grantdbaccess`, `sp_droplogin` là các SP hệ thống **đã deprecated**. Trong SQL Server hiện đại, nên dùng `CREATE LOGIN`, `CREATE USER`, `ALTER ROLE` thay thế. Tuy nhiên, các SP cũ vẫn hoạt động và thầy sử dụng trong bài giảng để minh họa.
