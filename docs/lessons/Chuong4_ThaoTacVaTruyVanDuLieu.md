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
