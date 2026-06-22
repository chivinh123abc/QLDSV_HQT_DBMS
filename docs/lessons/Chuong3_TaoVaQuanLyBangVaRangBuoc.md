**CHƯƠNG 3. TẠO, QUẢN LÝ BẢNG VÀ RÀNG BUỘC TOÀN VẸN**

Mục đích: Nắm vững các kiểu dữ liệu trong SQL Server, cách tạo lập cấu trúc lưu trữ dữ liệu (Table) và cách thiết lập các quy tắc (Constraints) để đảm bảo tính chính xác, hợp lệ của dữ liệu.

### I. CÁC KIỂU DỮ LIỆU (DATA TYPES) TRONG SQL SERVER

Trước khi tạo bảng, bạn cần xác định kiểu dữ liệu cho từng cột. SQL Server cung cấp các nhóm kiểu dữ liệu chính sau:

1. **Kiểu dữ liệu số (Numeric):**

- _Số nguyên:_ `bit` (0, 1 hoặc NULL), `tinyint` (0 đến 255), `smallint`, `int` (chuẩn, hay dùng nhất), `bigint`.
- _Số thực (xấp xỉ):_ `float`, `real`.
- _Số thực (chính xác):_ `decimal(p, s)` hoặc `numeric(p, s)` (với p là tổng số chữ số, s là số chữ số phần thập phân).
- _Tiền tệ:_ `money`, `smallmoney`.

2. **Kiểu dữ liệu chuỗi ký tự (Character Strings):**

- _Non-Unicode (Không dấu):_ `char(n)` (kích thước cố định), `varchar(n)` (kích thước thay đổi, tối đa 8000 ký tự), `varchar(max)`, `text`.
- _Unicode (Có dấu):_ `nchar(n)`, `nvarchar(n)` (kích thước thay đổi, tối đa 4000 ký tự), `nvarchar(max)`, `ntext`. _(Lưu ý: Luôn dùng N đứng trước chuỗi khi nhập dữ liệu Unicode, vd: N'Nguyễn Văn A')_.

3. **Kiểu thời gian (Date and Time):**

- `date`: Chỉ lưu ngày (YYYY-MM-DD).
- `time`: Chỉ lưu giờ (hh:mm:ss).
- `datetime`: Lưu cả ngày và giờ (từ năm 1753 đến 9999).
- `smalldatetime`: Giống datetime nhưng độ chính xác thấp hơn (từ năm 1900 đến 2079).

4. **Kiểu nhị phân (Binary):**

- `binary`, `varbinary`, `image` (thường dùng để lưu hình ảnh, tập tin).

5. **Kiểu dữ liệu khác:**

- `uniqueidentifier`: Lưu mã định danh duy nhất toàn cầu (GUID).
- `xml`: Lưu trữ dữ liệu định dạng XML.

### II. QUẢN LÝ BẢNG (TABLE MANAGEMENT) DẰNG T-SQL

Bảng (Table) là đối tượng cơ bản nhất dùng để lưu trữ dữ liệu, bao gồm các Dòng (Row/Record) và Cột (Column/Field).

**1. Tạo Bảng (CREATE TABLE)**

- **Cú pháp cơ bản:**

```sql
CREATE TABLE TenBang (
    TenCot1 KieuDuLieu [ThuocTinh],
    TenCot2 KieuDuLieu [ThuocTinh],
    ...
);

```

- **Thuộc tính IDENTITY (Cột tự tăng):**
  Thường dùng cho các cột mã số tự động tăng. Cú pháp: `IDENTITY(seed, increment)`
- _seed:_ Giá trị bắt đầu (mặc định là 1).
- _increment:_ Bước nhảy (mặc định là 1).
- _Ví dụ:_ `MaKH int IDENTITY(1,1)`

**2. Sửa đổi cấu trúc Bảng (ALTER TABLE)**

- **Thêm cột mới:** `ALTER TABLE TenBang ADD TenCot KieuDuLieu;`
- **Xóa cột:** `ALTER TABLE TenBang DROP COLUMN TenCot;`
- **Sửa kiểu dữ liệu của cột:** `ALTER TABLE TenBang ALTER COLUMN TenCot KieuDuLieuMoi;`

**3. Xóa Bảng (DROP TABLE)**

- **Cú pháp:** `DROP TABLE TenBang;`
- _Lưu ý:_ Khi xóa bảng, toàn bộ dữ liệu và các ràng buộc (constraints) gắn với bảng đó cũng sẽ bị xóa. Không thể xóa một bảng đang được bảng khác tham chiếu tới bằng khóa ngoại (phải xóa khóa ngoại trước).

### III. RÀNG BUỘC TOÀN VẸN (CONSTRAINTS)

Ràng buộc (Constraint) là các quy tắc được áp đặt lên cột hoặc bảng để hạn chế loại dữ liệu được phép chèn vào, nhằm đảm bảo tính toàn vẹn và độ tin cậy của dữ liệu (Data Integrity).

**Các loại ràng buộc toàn vẹn:**

1. **PRIMARY KEY (Khóa chính):**

- Xác định duy nhất một dòng (record) trong bảng.
- Dữ liệu trong cột khóa chính **không được phép trùng lặp** (Unique) và **không được rỗng** (NOT NULL).
- Mỗi bảng chỉ có tối đa một Khóa chính (nhưng có thể được tạo thành từ nhiều cột - Composite Key).
- Cú pháp lúc tạo bảng: `MaSV int PRIMARY KEY`

2. **FOREIGN KEY (Khóa ngoại):**

- Dùng để thiết lập và thực thi mối quan hệ (Relationship) giữa dữ liệu ở hai bảng.
- Giá trị nhập vào cột khóa ngoại phải **tồn tại trong cột khóa chính** của bảng được tham chiếu, hoặc là giá trị NULL.
- **Cascading Options (Hành động khi bảng tham chiếu thay đổi):**
- `ON DELETE CASCADE`: Khi xóa 1 dòng ở bảng cha, các dòng liên quan ở bảng con sẽ bị xóa theo.
- `ON UPDATE CASCADE`: Khi cập nhật khóa chính ở bảng cha, khóa ngoại ở bảng con tự động cập nhật theo.

3. **UNIQUE (Duy nhất):**

- Đảm bảo không có hai giá trị nào trong cột (hoặc tập hợp các cột) bị trùng nhau.
- Khác với khóa chính, cột Unique **cho phép chứa 1 giá trị NULL**. Một bảng có thể có nhiều ràng buộc Unique.

4. **CHECK (Kiểm tra điều kiện):**

- Giới hạn miền giá trị được phép nhập vào một cột dựa trên một biểu thức logic.
- _Ví dụ:_ Ràng buộc tuổi phải từ 18 trở lên: `CHECK (Tuoi >= 18)` hoặc giới hạn giới tính `CHECK (GioiTinh IN ('Nam', 'Nu'))`.

5. **DEFAULT (Mặc định):**

- Tự động gán một giá trị mặc định cho cột nếu người dùng khi thêm dữ liệu (INSERT) không cung cấp giá trị cho cột đó.
- _Ví dụ:_ Gán ngày hiện tại làm mặc định: `DEFAULT GETDATE()`.

6. **NOT NULL:**

- Bắt buộc cột phải có dữ liệu, không được để trống khi chèn hoặc cập nhật một dòng mới.

### IV. THÊM / XÓA RÀNG BUỘC BẰNG LỆNH ALTER TABLE

Thay vì tạo ràng buộc ngay lúc tạo bảng, lập trình viên thường dùng lệnh `ALTER TABLE` để thêm ràng buộc sau, giúp dễ dàng đặt tên và quản lý.

- **Thêm ràng buộc:**

```sql
ALTER TABLE TenBang
ADD CONSTRAINT TenRangBuoc LoaiRangBuoc (ChiTietRangBuoc);

```

_Ví dụ thêm Khóa ngoại:_
`ALTER TABLE SinhVien ADD CONSTRAINT FK_SV_Lop FOREIGN KEY (MaLop) REFERENCES Lop(MaLop);`

- **Xóa ràng buộc:**

```sql
ALTER TABLE TenBang DROP CONSTRAINT TenRangBuoc;

```

---

### GHI CHÚ TRÊN LỚP (Quicknote)

> Các ghi chú bổ sung từ bài giảng trên lớp.

**1. Cấu trúc tập tin Database:**
- Khi tạo Database, hệ thống tạo **2 tập tin** vật lý:
  - `ten_file.mdf` — Tập tin dữ liệu chính.
  - `ten_file_log.ldf` — Tập tin nhật ký giao dịch.

**2. View (Khung nhìn):**

- View **không chứa dữ liệu thực tế** mà chứa **câu lệnh SQL** truy vấn dữ liệu.
- Các chức năng chính:
  - Truy vấn trên **nhiều bảng** và trả kết quả tổng hợp.
  - Chọn ra một **tập con** các records từ bảng cơ sở.
  - Tạo ra **các cột mới** dựa trên biểu thức tính toán từ các cột đã có.
  - Kết nối (JOIN) nhiều records từ các bảng thành một record trong View.
  - Cho phép thay đổi dữ liệu trên bảng gốc thông qua View.

> **⚠️ Hai trường hợp View chỉ là readonly (chỉ đọc):**
> 1. View chứa **`GROUP BY`**
> 2. View chứa **Subquery** (truy vấn con)

**3. `WITH CHECK OPTION` trong View:**

Khi khai báo `WITH CHECK OPTION`, View sẽ **từ chối** mọi thao tác INSERT/UPDATE nếu dữ liệu mới vi phạm điều kiện WHERE của View.

```sql
-- Tạo View chỉ hiển thị nhân viên Nam
CREATE VIEW NV_nam AS
  SELECT * FROM [QLVT].dbo.NhanVien
  WHERE Phai = 'Nam'
  WITH CHECK OPTION

-- Câu lệnh sau sẽ BỊ TỪ CHỐI vì vi phạm điều kiện Phai = 'Nam'
UPDATE NV_nam SET Phai = 'Nu'
```

**4. Index (Chỉ mục) — Mẹo hiệu suất:**

- Trong câu lệnh `SELECT`, có **2 thao tác làm chậm truy vấn**:
  - `ORDER BY` — Sắp xếp kết quả.
  - `DISTINCT` — Loại bỏ trùng lặp.
- **Giải pháp:** Tạo **Index** trên các cột thường xuyên dùng để sắp xếp hoặc lọc, giúp tăng tốc truy vấn đáng kể.

**5. Restore Database — 3 điều kiện cần thiết:**

> **❓ Ghi chú chưa đầy đủ trên lớp** — đã bổ sung chi tiết bên dưới.

Để có thể **Restore (phục hồi)** database về một thời điểm cụ thể, cần đảm bảo **3 điều kiện** được thiết lập ngay khi tạo database:

| # | Điều kiện | Cách thiết lập | Giải thích |
|---|---|---|---|
| 1 | **Recovery Model = FULL** | `ALTER DATABASE TenDB SET RECOVERY FULL` | Cho phép SQL Server ghi lại **toàn bộ** giao dịch vào Transaction Log, cần thiết cho Point-in-time recovery |
| 2 | **Đã thực hiện Full Backup ít nhất 1 lần** | `BACKUP DATABASE TenDB TO DISK = 'path.bak'` | Full Backup là "mốc gốc" để tất cả các bản sao lưu tiếp theo (Differential, Log) tham chiếu đến |
| 3 | **Đã thực hiện Transaction Log Backup** | `BACKUP LOG TenDB TO DISK = 'path.trn'` | Sao lưu nhật ký giao dịch cho phép phục hồi đến **bất kỳ thời điểm nào** giữa 2 lần backup |

```sql
-- Bước 1: Thiết lập Recovery Model = FULL (ngay khi tạo DB)
ALTER DATABASE QLDSV_TC SET RECOVERY FULL;

-- Bước 2: Full Backup lần đầu
BACKUP DATABASE QLDSV_TC TO DISK = N'D:\Backup\QLDSV_TC_FULL.bak';

-- Bước 3: Transaction Log Backup (có thể chạy định kỳ)
BACKUP LOG QLDSV_TC TO DISK = N'D:\Backup\QLDSV_TC_LOG.trn';

-- Restore về thời điểm cụ thể (Point-in-time Recovery)
RESTORE DATABASE QLDSV_TC
FROM DISK = N'D:\Backup\QLDSV_TC_FULL.bak'
WITH NORECOVERY;

RESTORE LOG QLDSV_TC
FROM DISK = N'D:\Backup\QLDSV_TC_LOG.trn'
WITH STOPAT = '2026-03-07 14:30:00', RECOVERY;
```

> **⚠️ Lưu ý:** Nếu Recovery Model là **SIMPLE**, Transaction Log sẽ tự động bị cắt bớt (truncate) → **không thể** khôi phục theo thời điểm (Point-in-time recovery).
