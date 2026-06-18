**CHUYÊN ĐỀ: CÁC TABLE HỆ THỐNG LƯU MỐI LIÊN KẾT (FOREIGN KEYS)**

Mục đích: Giúp lập trình viên viết câu lệnh SQL để tự động lấy ra danh sách tất cả các bảng có quan hệ với nhau, biết được bảng nào là bảng cha (Primary Key table), bảng nào là bảng con (Foreign Key table) và chi tiết các cột được liên kết.

Trong SQL Server, có 3 cách chính để truy vấn mối quan hệ giữa các bảng: dùng System Tables cũ (tương thích ngược), dùng Catalog Views mới (khuyên dùng), và dùng Information Schema.

### I. CÁCH 1: SỬ DỤNG CÁC TABLE HỆ THỐNG CŨ (Backward Compatibility Views)

Đây là các bảng hệ thống (System Tables) được giới thiệu từ SQL Server 2000 và vẫn được nhắc đến trong các giáo trình cơ bản:

1. **`sysforeignkeys`**: Đây là bảng cốt lõi chứa định nghĩa về khóa ngoại. Các cột quan trọng:

- `constid`: Mã ID của Ràng buộc khóa ngoại (Constraint ID).
- `fkeyid`: Mã ID của bảng chứa khóa ngoại (Bảng Con - Foreign Key Table).
- `rkeyid`: Mã ID của bảng được tham chiếu (Bảng Cha - Referenced Table).
- `fkey`: Mã ID của cột khóa ngoại trong Bảng Con.
- `rkey`: Mã ID của cột khóa chính trong Bảng Cha.

2. **`sysobjects`**: Chứa thông tin về mọi đối tượng (Table, View, Stored Procedure, Constraint). Dùng để lấy "Tên bảng" và "Tên ràng buộc" dựa vào mã ID.
3. **`syscolumns`**: Chứa thông tin về tất cả các cột. Dùng để lấy "Tên cột" dựa vào mã ID của cột và bảng.

**Câu lệnh T-SQL mẫu để lấy danh sách liên kết:**

```sql
SELECT
    fk_obj.name AS Ten_Khoa_Ngoai,
    table_con.name AS Bang_Con,
    col_con.name AS Cot_Con,
    table_cha.name AS Bang_Cha,
    col_cha.name AS Cot_Cha
FROM sysforeignkeys fk
    INNER JOIN sysobjects fk_obj ON fk.constid = fk_obj.id
    INNER JOIN sysobjects table_con ON fk.fkeyid = table_con.id
    INNER JOIN syscolumns col_con ON fk.fkeyid = col_con.id AND fk.fkey = col_con.colid
    INNER JOIN sysobjects table_cha ON fk.rkeyid = table_cha.id
    INNER JOIN syscolumns col_cha ON fk.rkeyid = col_cha.id AND fk.rkey = col_cha.colid;

```

---

### II. CÁCH 2: SỬ DỤNG CATALOG VIEWS (TỪ SQL SERVER 2005 TRỞ LÊN)

Microsoft khuyên dùng hệ thống `sys.*` Views mới vì cấu trúc rõ ràng và bảo mật hơn.

1. **`sys.foreign_keys`**: Lưu thông tin mức độ bảng (Bảng nào liên kết với bảng nào).
2. **`sys.foreign_key_columns`**: Lưu thông tin mức độ cột (Cột nào liên kết với cột nào, rất hữu ích với các khóa ngoại dạng Composite Key - khóa ngoại gồm nhiều cột).
3. **`sys.tables`**: Lấy tên bảng.
4. **`sys.columns`**: Lấy tên cột.

**Câu lệnh T-SQL mẫu chuẩn:**

```sql
SELECT
    fk.name AS Ten_Khoa_Ngoai,
    tp.name AS Bang_Con,
    cp.name AS Cot_Con,
    tr.name AS Bang_Cha,
    cr.name AS Cot_Cha
FROM sys.foreign_keys fk
    INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
    INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    INNER JOIN sys.columns cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
    INNER JOIN sys.columns cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
ORDER BY tp.name, fk.name;

```

---

### III. CÁCH 3: SỬ DỤNG INFORMATION SCHEMA VIEWS (Chuẩn ANSI SQL)

Nếu bạn muốn code của mình có thể chạy được trên cả SQL Server, MySQL, hay PostgreSQL, bạn có thể dùng cấu trúc chuẩn quốc tế `INFORMATION_SCHEMA`.

- **`INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS`**: Lưu các tham chiếu ràng buộc.
- **`INFORMATION_SCHEMA.KEY_COLUMN_USAGE`**: Lưu các cột được sử dụng làm khóa.

```sql
SELECT
    KCU1.CONSTRAINT_NAME AS Ten_Khoa_Ngoai,
    KCU1.TABLE_NAME AS Bang_Con,
    KCU1.COLUMN_NAME AS Cot_Con,
    KCU2.TABLE_NAME AS Bang_Cha,
    KCU2.COLUMN_NAME AS Cot_Cha
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1 ON RC.CONSTRAINT_NAME = KCU1.CONSTRAINT_NAME
    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2 ON RC.UNIQUE_CONSTRAINT_NAME = KCU2.CONSTRAINT_NAME
         AND KCU1.ORDINAL_POSITION = KCU2.ORDINAL_POSITION;

```

---

### IV. CÁCH 4: SỬ DỤNG THỦ TỤC HỆ THỐNG CÓ SẴN (System Stored Procedure)

Nếu bạn không muốn tự viết câu lệnh `JOIN` dài dòng, SQL Server cung cấp sẵn một Stored Procedure tên là **`sp_fkeys`** để tự động liệt kê toàn bộ khóa ngoại trỏ TỚI một bảng, hoặc khóa ngoại trỏ TỪ một bảng.

- **Cú pháp tìm các bảng con đang trỏ tham chiếu đến Bảng Cha:**

```sql
EXEC sp_fkeys @pktable_name = 'TenBangCha';

```

_(Ví dụ: Xem những bảng nào đang có khóa ngoại trỏ vào bảng `KHOA`)_

- **Cú pháp tìm các bảng cha mà Bảng Con đang tham chiếu đến:**

```sql
EXEC sp_fkeys @fktable_name = 'TenBangCon';

```

_(Ví dụ: Xem bảng `SINHVIEN` đang bị ràng buộc khóa ngoại bởi những bảng nào)_
