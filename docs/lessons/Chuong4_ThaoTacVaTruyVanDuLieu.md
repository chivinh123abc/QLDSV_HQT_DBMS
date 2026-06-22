**CHƯƠNG 4. THAO TÁC VÀ TRUY VẤN DỮ LIỆU**

Mục đích: Nắm vững các câu lệnh thuộc nhóm DML (Data Manipulation Language) để thêm, sửa, xóa dữ liệu và nhóm DQL (Data Query Language) để trích xuất, tổng hợp dữ liệu từ một hoặc nhiều bảng.

---

### I. NGÔN NGỮ THAO TÁC DỮ LIỆU (DML - DATA MANIPULATION)

Đây là các lệnh dùng để thay đổi nội dung dữ liệu bên trong các bảng.

**1. Thêm dữ liệu (INSERT)**
Lệnh `INSERT` dùng để chêm một hoặc nhiều dòng dữ liệu mới vào bảng.

- **Thêm một dòng cụ thể:**
  `INSERT INTO TenBang (Cot1, Cot2, ...) VALUES (GiaTri1, GiaTri2, ...);`
  _(Lưu ý: Số lượng và kiểu dữ liệu của Giá trị phải tương ứng với Cột. Nếu nhập đủ các cột theo đúng thứ tự, có thể bỏ qua phần danh sách Cột)._
- **Thêm nhiều dòng từ một bảng khác (Insert Select):**
  `INSERT INTO TenBang1 (Cot1, Cot2) SELECT CotA, CotB FROM TenBang2;`

**2. Cập nhật dữ liệu (UPDATE)**
Lệnh `UPDATE` dùng để sửa đổi giá trị của dữ liệu đã có trong bảng.

- **Cú pháp:**

```sql
UPDATE TenBang
SET Cot1 = GiaTriMoi1, Cot2 = GiaTriMoi2
WHERE <Điều_kiện>;

```

- > **Cảnh báo:** Nếu bạn quên mệnh đề `WHERE` trong lệnh `UPDATE`, toàn bộ dữ liệu trong bảng của các cột được chỉ định sẽ bị thay đổi đồng loạt.

**3. Xóa dữ liệu (DELETE)**
Lệnh `DELETE` dùng để xóa một, nhiều hoặc tất cả các dòng trong bảng.

- **Cú pháp:**
  `DELETE FROM TenBang WHERE <Điều_kiện>;`
- **Phân biệt DELETE và TRUNCATE:**
- `DELETE`: Xóa từng dòng, có ghi vào Transaction Log (có thể rollback phục hồi), có thể dùng kèm mệnh đề `WHERE`. Chạy chậm hơn nếu bảng lớn.
- `TRUNCATE TABLE TenBang`: Xóa toàn bộ dữ liệu trong bảng một cách triệt để, không ghi chi tiết vào Log, reset lại cột tự tăng (IDENTITY) về giá trị ban đầu. Không dùng được nếu bảng đang được tham chiếu bởi Khóa ngoại (Foreign Key).

---

### II. TRUY VẤN DỮ LIỆU CƠ BẢN (SELECT STATEMENT)

Lệnh `SELECT` là lệnh được sử dụng nhiều nhất trong SQL Server, dùng để lấy dữ liệu ra khỏi cơ sở dữ liệu.

**1. Cú pháp cơ bản:**

```sql
SELECT [DISTINCT] Cot1, Cot2... (hoặc *)
FROM TenBang
WHERE <Điều_kiện_lọc_dữ_liệu>
ORDER BY Cot_Sap_Xep [ASC | DESC];

```

- `*`: Lấy tất cả các cột.
- `DISTINCT`: Loại bỏ các dòng dữ liệu trùng lặp trong kết quả.
- `ORDER BY`: Sắp xếp kết quả tăng dần (`ASC` - mặc định) hoặc giảm dần (`DESC`).
- `TOP n`: Lấy `n` dòng đầu tiên từ kết quả (thường kết hợp với `ORDER BY`). Ví dụ: `SELECT TOP 5 * FROM SinhVien ORDER BY Diem DESC;`

**2. Các toán tử trong mệnh đề WHERE:**

- **So sánh:** `=`, `>`, `<`, `>=`, `<=`, `<>` (hoặc `!=` - Khác).
- **Toán tử logic:** `AND`, `OR`, `NOT`.
- **BETWEEN ... AND ...:** Nằm trong khoảng (bao gồm cả 2 đầu mút).
- **IN (Danh sách):** So sánh giá trị với một tập hợp. Ví dụ: `WHERE MaLop IN ('L01', 'L02')`.
- **LIKE:** Tìm kiếm theo mẫu chuỗi. Dùng kèm ký tự đại diện:
- `%`: Đại diện cho chuỗi ký tự bất kỳ (bao gồm rỗng).
- `_`: Đại diện cho 1 ký tự bất kỳ.
- Ví dụ: `LIKE N'Nguyễn%'` (Tìm người họ Nguyễn).

- **IS NULL / IS NOT NULL:** Kiểm tra giá trị rỗng (không thể dùng toán tử `=` để so sánh với `NULL`).

---

### III. TRUY VẤN TỪ NHIỀU BẢNG (JOIN)

Trong CSDL quan hệ, dữ liệu thường bị phân tán ở nhiều bảng. `JOIN` dùng để kết nối chúng lại dựa trên các cột có chung giá trị (thường là Primary Key và Foreign Key).

1. **INNER JOIN (Kết nối bằng):** Lấy các dòng có dữ liệu khớp nhau ở **cả hai** bảng.
   `SELECT A.*, B.* FROM BangA A INNER JOIN BangB B ON A.Ma = B.Ma;`
2. **LEFT OUTER JOIN (Kết nối trái):** Lấy toàn bộ dữ liệu bảng bên **Trái**, và dữ liệu khớp ở bảng bên Phải. Nếu bảng Phải không có, giá trị trả về là `NULL`.
3. **RIGHT OUTER JOIN (Kết nối phải):** Ngược lại với LEFT JOIN, ưu tiên lấy toàn bộ dữ liệu bảng bên **Phải**.
4. **FULL OUTER JOIN (Kết nối đầy đủ):** Lấy tất cả dữ liệu từ cả hai bảng. Nếu không có dữ liệu khớp, phần thiếu sẽ hiển thị `NULL`.

---

### IV. GOM NHÓM VÀ THỐNG KÊ (GROUP BY & HAVING)

Dùng để tóm tắt và phân tích dữ liệu.

**1. Các hàm kết hợp (Aggregate Functions):**

- `COUNT(*)` hoặc `COUNT(Cột)`: Đếm số lượng dòng.
- `SUM(Cột)`: Tính tổng.
- `AVG(Cột)`: Tính trung bình cộng.
- `MAX(Cột)` / `MIN(Cột)`: Lấy giá trị lớn nhất / nhỏ nhất.

**2. Mệnh đề GROUP BY:**
Dùng để nhóm các dòng có cùng giá trị lại với nhau trước khi thực hiện các hàm thống kê.

- _Quy tắc:_ Mọi cột nằm trong `SELECT` (mà không nằm trong hàm thống kê) bắt buộc phải xuất hiện trong mệnh đề `GROUP BY`.

**3. Mệnh đề HAVING:**
Tương tự `WHERE` nhưng dùng để **lọc kết quả sau khi đã gom nhóm**. Không thể dùng hàm thống kê trong `WHERE`, mà phải dùng trong `HAVING`.

- _Ví dụ:_ Lấy ra các lớp có sĩ số trên 40 sinh viên:

```sql
SELECT MaLop, COUNT(MaSV) AS SiSo
FROM SinhVien
GROUP BY MaLop
HAVING COUNT(MaSV) > 40;

```

---

### V. TRUY VẤN CON (SUBQUERIES)

Truy vấn con là một câu lệnh `SELECT` được lồng vào bên trong một câu lệnh T-SQL khác (SELECT, INSERT, UPDATE, DELETE).

- **Trong mệnh đề WHERE / HAVING:** Thường dùng với các toán tử `IN`, `NOT IN`, `>=`, `<=`, hoặc `EXISTS`. Nó trả về một giá trị đơn hoặc một danh sách giá trị làm điều kiện cho câu truy vấn ngoài.
- **Trong mệnh đề FROM (Derived Tables - Bảng dẫn xuất):** Truy vấn con đóng vai trò như một bảng tạm thời có thể được `JOIN` với các bảng khác. Bắt buộc phải đặt tên định danh (Alias) cho bảng dẫn xuất này.

---

### VI. TOÁN TỬ TẬP HỢP (SET OPERATORS)

Dùng để kết hợp nhiều kết quả (Result Sets) của nhiều câu lệnh `SELECT` độc lập thành một kết quả duy nhất. (Điều kiện: Các câu lệnh `SELECT` phải có cùng số lượng cột và kiểu dữ liệu tương ứng).

- **UNION:** Kết hợp kết quả của 2 câu truy vấn và tự động **loại bỏ** các dòng trùng lặp.
- **UNION ALL:** Kết hợp kết quả nhưng **giữ lại** toàn bộ các dòng (bao gồm cả dòng trùng). Hiệu suất chạy nhanh hơn `UNION`.
- **INTERSECT:** Trả về các dòng xuất hiện **ở cả hai** tập kết quả (Phép giao).
- **EXCEPT:** Trả về các dòng có ở tập kết quả thứ nhất nhưng **không có** trong tập kết quả thứ hai (Phép trừ).

---

### GHI CHÚ TRÊN LỚP (Quicknote)

> Các ghi chú lý thuyết và bài tập từ bài giảng trên lớp.

#### A. Lệnh MERGE

Lệnh `MERGE` kết hợp `INSERT`, `UPDATE` và `DELETE` vào **1 lệnh duy nhất**, tùy thuộc vào sự tồn tại của bản ghi:

```sql
MERGE INTO dbo.VATTU AS Target
USING (SELECT MAVT='VT20', TENVT=N'ĐƯỜNG BIÊN HÒA', DVT=N'KG', SLT=0) AS Source
  ON Target.MAVT = Source.MAVT
  WHEN MATCHED THEN
       UPDATE SET TARGET.SOLUONGTON = 20
  WHEN NOT MATCHED THEN
     INSERT (MAVT, TENVT, DVT, SOLUONGTON)
          VALUES (Source.MAVT, Source.TENVT, Source.DVT, Source.SLT);
```

> **Lưu ý:** Nếu Source là 1 table thì lệnh MERGE sẽ lấy **từng mẫu tin** trong Source để Insert/Update vào Target.

#### B. Stored Procedure — Các cách trả kết quả

SP có **3 cách** trả kết quả:

| Cách               | Mô tả                                                                           | Ví dụ                      |
| ------------------ | ------------------------------------------------------------------------------- | -------------------------- |
| **`RETURN`**       | Trả về **1 số nguyên** (thường dùng cho mã trạng thái: 0 = thành công, 1 = lỗi) | `RETURN 0`                 |
| **`SELECT`**       | Trả về **tập kết quả** (result set) dạng bảng                                   | `SELECT * FROM NhanVien`   |
| **`OUTPUT` param** | Trả về giá trị qua **tham biến** (biến đầu ra)                                  | `@Name VARCHAR(30) OUTPUT` |

```sql
-- Cách 1: Trả về qua RETURN + SELECT
CREATE PROCEDURE ListCustWithDiscount
    @MinDiscount DEC(5,3) = 0.0001
AS
    IF (@MinDiscount > 1) RETURN 1      -- Mã lỗi
    SELECT * FROM Customer WHERE Discount >= @MinDiscount
    RETURN 0                             -- Thành công

-- Cách 2: Trả về qua OUTPUT parameter (tham biến)
CREATE PROCEDURE GetNameAndDiscount
    @CustID INT,
    @Name VARCHAR(30) OUTPUT,
    @Discount DEC(5,3) OUTPUT
AS
    SELECT @Name = Name, @Discount = Discount
    FROM Customer
    WHERE CustID = @CustID
```

#### C. Giao tác (Transaction)

- Gồm **2 loại**: Giao tác tập trung (local) và Giao tác phân tán (distributed).
- Cấu trúc cơ bản: `BEGIN TRAN` → `COMMIT` (thành công) / `ROLLBACK` (thất bại).

```sql
-- Ví dụ: SP chuyển tiền với xử lý lỗi đầy đủ
CREATE PROC [dbo].[SP_CHUYENTIEN]
    @TKCHUYEN NVARCHAR(10),
    @TKNHAN NVARCHAR(10),
    @SOTIEN BIGINT
AS
SET XACT_ABORT ON
BEGIN TRAN
BEGIN TRY
    UPDATE TAIKHOAN SET SODU = SODU + @SOTIEN WHERE SOTK = @TKNHAN
    UPDATE TAIKHOAN SET SODU = SODU - @SOTIEN WHERE SOTK = @TKCHUYEN
    COMMIT
END TRY
BEGIN CATCH
    ROLLBACK
    DECLARE @ErrorMessage VARCHAR(2000)
    SELECT @ErrorMessage = N'Lỗi: ' + ERROR_MESSAGE()
    RAISERROR(@ErrorMessage, 16, 1)
END CATCH
```

#### D. Sysobjects — Tra cứu đối tượng hệ thống

```sql
-- Liệt kê tất cả đối tượng trong database (table, view, SP, trigger,...)
SELECT * FROM SYS.sysobjects
```

---

### BÀI TẬP TRÊN LỚP (13 bài)

**Bài 1:** Liệt kê nhân viên chưa lập phiếu nhập trong năm `@nam`:

```sql
CREATE PROCEDURE sp_NV_ChuaLapPhieuNhap @nam INT
AS
BEGIN
    SELECT nv.MANV, nv.HO + ' ' + nv.TEN AS HOTEN
    FROM NhanVien nv
    WHERE NOT EXISTS (
        SELECT 1 FROM PhieuNhap pn
        WHERE pn.MANV = nv.MANV AND YEAR(pn.NGAY) = @nam
    );
END
```

**Bài 2:** Chi tiết mặt hàng đã xuất theo mã phiếu `@sohd`:

```sql
CREATE PROC cacMatHangDaXuatTheoSHD @sohd NVARCHAR(10)
AS
BEGIN
    SELECT px.Ngay, vt.MAVT, vt.TENVT, ctpx.SOLUONG, ctpx.DONGIA,
           ctpx.DONGIA * ctpx.SOLUONG AS TriGia
    FROM PhieuXuat px
    INNER JOIN CTPX ctpx ON px.MAPX = ctpx.MAPX
    INNER JOIN Vattu vt ON ctpx.MAVT = vt.MAVT
    WHERE px.MAPX = @sohd
END
```

**Bài 3:** Phiếu nhập trong 6 tháng đầu năm `@nam`:

```sql
CREATE PROC cacPNTrong6ThangDauNam @nam INT
AS
BEGIN
    SELECT pn.MAPN AS SoPhieu, pn.NGAY, pn.MANV,
           nv.HO + ' ' + nv.TEN AS HoTenNV
    FROM PhieuNhap pn
    INNER JOIN NhanVien nv ON pn.MANV = nv.MANV
    WHERE YEAR(pn.NGAY) = @nam AND MONTH(pn.NGAY) BETWEEN 1 AND 6
END
```

**Bài 4–6:** Đếm số lượng NV, phiếu, NV theo khoảng lương:

```sql
-- Bài 4: Đếm số lượng nhân viên
SELECT COUNT(*) AS SoLuongNhanVien FROM NhanVien

-- Bài 5: Đếm tổng phiếu (nhập + xuất)
SELECT (SELECT COUNT(*) FROM PhieuNhap) + (SELECT COUNT(*) FROM PhieuXuat) AS TongPhieu

-- Bài 6: NV có lương trong khoảng
CREATE PROC cacNVCoLuongTrongKhoang @luongmin INT, @luongmax INT
AS
BEGIN
    SELECT nv.MANV, nv.HO + ' ' + nv.TEN AS HOTEN, nv.NGAYSINH, nv.LUONG
    FROM NhanVien nv
    WHERE nv.LUONG BETWEEN @luongmin AND @luongmax
END
```

**Bài 7:** Liệt kê phiếu theo loại (Nhập/Xuất) trong khoảng thời gian:

```sql
-- Dùng IF để phân nhánh theo @loai = 'N' (Nhập) hoặc 'X' (Xuất)
IF (@loai = 'N')
    SELECT pn.MAPN AS PHIEU, pn.NGAY, SUM(ctn.SOLUONG * ctn.DONGIA) AS THANHTIEN,
           nv.HO + ' ' + nv.TEN AS HOTENNV
    FROM PhieuNhap pn
    JOIN CTPN ctn ON pn.MAPN = ctn.MAPN
    JOIN NhanVien nv ON pn.MANV = nv.MANV
    WHERE pn.NGAY BETWEEN @tungay AND @denngay
    GROUP BY pn.MAPN, pn.NGAY, nv.HO, nv.TEN;
```

**Bài 9:** Đếm số lượt nhập/xuất vật tư `@mavt` (dùng `UNION ALL`):

```sql
SELECT vt.MAVT, vt.TENVT,
    COUNT(DISTINCT ctn.MAPN) AS SOLUOT_NHAP,
    COUNT(DISTINCT ctx.MAPX) AS SOLUOT_XUAT
FROM Vattu vt
LEFT JOIN CTPN ctn ON vt.MAVT = ctn.MAVT
LEFT JOIN CTPX ctx ON vt.MAVT = ctx.MAVT
WHERE vt.MAVT = @mavt
GROUP BY vt.MAVT, vt.TENVT;
```

**Bài 11:** Doanh thu theo tháng (tháng không có doanh thu vẫn hiển thị, dùng CTE):

```sql
WITH Thang AS (
    SELECT 1 AS THANG UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL
    SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL
    SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9 UNION ALL
    SELECT 10 UNION ALL SELECT 11 UNION ALL SELECT 12
)
SELECT t.THANG, ISNULL(SUM(ctx.SOLUONG * ctx.DONGIA), 0) AS DOANHTHU
FROM Thang t
LEFT JOIN PhieuXuat px ON MONTH(px.NGAY) = t.THANG AND YEAR(px.NGAY) = @nam
LEFT JOIN CTPX ctx ON px.MAPX = ctx.MAPX
GROUP BY t.THANG
ORDER BY t.THANG;
```

---

### TỐI ƯU BÀI TẬP (Demo trên lớp)

#### Tối ưu Bài 2 — 3 cấp độ

**Cấp 1: Chọn/Chiếu trước, JOIN sau** (Subquery lọc CTPX trước khi JOIN):

```sql
CREATE PROC cacMatHangDaXuatTheoSHD_v2 @sohd NVARCHAR(10)
AS
BEGIN
    SELECT px.Ngay, t.MAVT, vt.TENVT, t.SOLUONG, t.DONGIA,
           (t.DONGIA * t.SOLUONG) AS TriGia
    FROM (SELECT MAPX, MAVT, SOLUONG, DONGIA
          FROM CTPX WHERE MAPX = @sohd) AS t  -- Lọc trước
    INNER JOIN Vattu vt ON t.MAVT = vt.MAVT    -- JOIN sau
    INNER JOIN PhieuXuat px ON t.MAPX = px.MAPX
END
```

**Cấp 2: Khử phép kết** (Dùng biến loại bỏ JOIN PhieuXuat):

```sql
CREATE PROC cacMatHangDaXuatTheoSHD_v3 @sohd NVARCHAR(10)
AS
BEGIN
    DECLARE @NgayLap DATE;
    SELECT @NgayLap = Ngay FROM PhieuXuat WHERE MAPX = @sohd; -- Chỉ truy cập 1 lần

    SELECT @NgayLap AS Ngay, vt.MAVT, vt.TENVT, ctpx.SOLUONG, ctpx.DONGIA,
           (ctpx.DONGIA * ctpx.SOLUONG) AS TriGia
    FROM CTPX ctpx
    INNER JOIN Vattu vt ON ctpx.MAVT = vt.MAVT
    WHERE ctpx.MAPX = @sohd;  -- Chỉ còn JOIN 2 bảng thay vì 3
END
```

**Cấp 3: Kết hợp cả 2 kỹ thuật** (Tối ưu nhất):

```sql
CREATE PROC cacMatHangDaXuatTheoSHD_v4 @sohd NVARCHAR(10)
AS
BEGIN
    DECLARE @NgayLap DATE;
    SELECT @NgayLap = Ngay FROM PhieuXuat WHERE MAPX = @sohd;

    SELECT @NgayLap AS Ngay, t.MAVT, vt.TENVT, t.SOLUONG, t.DONGIA,
           (t.DONGIA * t.SOLUONG) AS TriGia
    FROM (SELECT MAVT, SOLUONG, DONGIA FROM CTPX WHERE MAPX = @sohd) AS t
    INNER JOIN Vattu vt ON t.MAVT = vt.MAVT;
END
```

#### Tối ưu Bài 3 — Áp dụng tất cả 5 nguyên tắc

```sql
CREATE PROC cacPNTrong6ThangDauNam_ToiUu @nam INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Nguyên tắc 3: Giảm thiểu điều kiện lặp → tính dải ngày 1 lần
    DECLARE @Start DATE = CAST(CAST(@nam AS VARCHAR) + '-01-01' AS DATE);
    DECLARE @End DATE = CAST(CAST(@nam AS VARCHAR) + '-07-01' AS DATE);

    SELECT t.MAPN AS SoPhieu, t.NGAY, t.MANV,
           nv.HO + ' ' + nv.TEN AS HoTenNV
    FROM
        -- Nguyên tắc 1: CHỌN/CHIẾU TRƯỚC
        (SELECT MAPN, MANV, NGAY FROM PhieuNhap
         WITH (INDEX(IDX_PhieuNhap_Ngay))      -- Nguyên tắc 5: Chỉ định Index
         WHERE NGAY >= @Start AND NGAY < @End   -- Nguyên tắc 4: Điều kiện sai cao → đầu
        ) AS t
        -- Nguyên tắc 1 (tiếp): PHÉP KẾT SAU
        INNER JOIN NhanVien nv WITH (INDEX(PK_NhanVien)) ON t.MANV = nv.MANV
END
```
