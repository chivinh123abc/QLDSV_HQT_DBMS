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
