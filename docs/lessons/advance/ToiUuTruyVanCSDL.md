**CHUYÊN ĐỀ: ĐIỀU CHỈNH VÀ TỐI ƯU HIỆU SUẤT TRUY VẤN SQL (QUERY PERFORMANCE TUNING)**

Mục đích: Tìm hiểu và áp dụng các phương pháp nâng cao tốc độ truy xuất dữ liệu trong cơ sở dữ liệu lớn (Big Data). Nắm vững cách phân tích kế hoạch thực thi (Execution Plan) và cách viết câu lệnh T-SQL tối ưu.

---

### I. CÁC CÔNG CỤ ĐÁNH GIÁ HIỆU SUẤT TRUY VẤN

Để biết một câu truy vấn chạy nhanh hay chậm, nguyên nhân gây nghẽn ở đâu, Quản trị viên CSDL (DBA) cần sử dụng các công cụ đo lường sau:

**1. Kế hoạch thực thi truy vấn (SQL Execution Plan)**

- **Khái niệm:** Khi bạn gửi một câu lệnh SQL đến Server, hệ thống (Cụ thể là _Query Optimizer_) không chạy trực tiếp lệnh đó ngay. Nó sẽ phân tích và lập ra một "Kế hoạch thực thi" (Execution Plan) - bản đồ đường đi tối ưu nhất để lấy dữ liệu.
- Kế hoạch này được tính toán dựa trên các số liệu thống kê (Statistics) hiện có của bảng, các chỉ mục (Indexes) và cấu trúc phần cứng.
- **Cách xem Execution Plan:** Trong SQL Server Management Studio (SSMS), bạn có thể bật tính năng **"Include Actual Execution Plan"** (hoặc ấn `Ctrl + M`) trước khi chạy truy vấn. Kết quả sẽ trả về một biểu đồ dạng cây (Tree graph) thể hiện các bước thực thi và chi phí (Cost) phần trăm của từng bước.

**2. Công cụ đo lường thời gian và I/O (Lệnh SET)**
Trước khi chạy truy vấn, bạn nên đặt các lệnh T-SQL sau để hệ thống trả về thông số thống kê chi tiết ở tab "Messages":

- **`SET STATISTICS TIME ON`**: Hiển thị thời gian phân tích, biên dịch và thời gian thực thi (tính bằng mili-giây).
- **`SET STATISTICS IO ON`**: Hiển thị số lượng trang dữ liệu vật lý (Pages) và trang logic mà máy chủ đã phải đọc từ ổ cứng hoặc từ bộ nhớ đệm (RAM) để thực thi truy vấn. Thông số _Logical Reads_ càng cao thì truy vấn càng chậm.
- **`DBCC FREEPROCCACHE`**: Lệnh này dùng để xóa bộ nhớ đệm của Execution Plan (chỉ dùng trong môi trường Test). Nếu không dùng lệnh này, những lần chạy truy vấn sau sẽ nhanh hơn lần đầu do SQL Server lấy lại kế hoạch đã lưu trong Cache, làm sai lệch kết quả test tối ưu.

---

### II. ĐỌC VÀ PHÂN TÍCH EXECUTION PLAN

Để tối ưu, bạn cần nhìn vào sơ đồ Execution Plan (đọc từ phải sang trái, từ trên xuống dưới) và tìm kiếm các "Nút cổ chai" (Bottlenecks) - những bước có chi phí (Cost %) cao nhất.

Các phép toán (Operators) thường gặp trong Execution Plan và mức độ ảnh hưởng đến tốc độ:

**1. Khối đọc dữ liệu (Data Access Operators)**

- **Table Scan:** Hệ thống phải quét toàn bộ bảng từ dòng đầu tiên đến dòng cuối cùng để tìm dữ liệu. (Rất chậm đối với bảng lớn).
- **Index Scan:** Tương tự Table Scan, nhưng quét toàn bộ trên một Index. Tốc độ quét vẫn chậm nếu bảng có hàng triệu dòng.
- **Index Seek:** (Tối ưu nhất) Hệ thống dựa vào cây cấu trúc của Index (B-Tree) để "đi thẳng" đến dòng dữ liệu cần tìm mà không cần duyệt toàn bộ. Đây là mục tiêu DBA luôn hướng tới.
- **Key Lookup (Bookmark Lookup):** Xảy ra khi một truy vấn dùng Index Seek để tìm được khóa chính, nhưng cột cần `SELECT` lại không nằm trong Index đó. Hệ thống phải thực hiện thêm một phép nhảy (Lookup) về bảng gốc để lấy đủ dữ liệu. Quá nhiều Key Lookup sẽ làm giảm hiệu suất nghiêm trọng.

**2. Khối kết nối bảng (Join Operators)**
Khi dùng lệnh `JOIN`, SQL Server tự động chọn 1 trong 3 thuật toán sau:

- **Nested Loops Join:** Tốt nhất cho việc kết nối các tập dữ liệu nhỏ. Thuật toán này lấy từng dòng của bảng ngoài đi dò tìm trong bảng trong (hoạt động giống 2 vòng lặp `For` lồng nhau).
- **Merge Join:** Nhanh và hiệu quả khi cả hai tập dữ liệu đầu vào có kích thước vừa/lớn và **đã được sắp xếp sẵn** theo cột dùng để kết nối.
- **Hash Match Join:** SQL Server buộc phải dùng cách này khi các tập dữ liệu rất lớn và chưa được sắp xếp. Nó tốn rất nhiều bộ nhớ RAM và CPU để băm (Hash) dữ liệu, là nguyên nhân gây chậm phổ biến.

---

### III. HIỆU CHỈNH CÁCH VIẾT TRUY VẤN (QUERY TUNING BEST PRACTICES)

Cách bạn viết câu lệnh T-SQL ảnh hưởng trực tiếp đến Execution Plan. Dưới đây là các nguyên tắc vàng để tối ưu mã lệnh:

1. **Tránh sử dụng `SELECT \***`: Chỉ nên liệt kê đích danh các cột cần thiết (Ví dụ: `SELECT MaSV, HoTen FROM...`). Việc lấy dư cột sẽ làm tăng tải mạng, tốn I/O đĩa và vô tình làm mất tác dụng của các Index.
2. **Hạn chế sử dụng Function (Hàm) trên cột trong mệnh đề `WHERE**`:

- _Viết sai (Không dùng được Index):_ `WHERE YEAR(NgaySinh) = 2023` -> SQL Server sẽ phải duyệt (Scan) toàn bộ bảng, dùng hàm `YEAR()` cắt từng dòng ra rồi mới so sánh.
- _Viết đúng (Index Seek):_ `WHERE NgaySinh >= '2023-01-01' AND NgaySinh < '2024-01-01'`.

3. **Sử dụng toán tử `LIKE` hợp lý**:

- `LIKE 'Nguyen%'`: Tìm kiếm chuỗi bắt đầu bằng chữ "Nguyen". SQL Server **CÓ THỂ** sử dụng Index Seek.
- `LIKE '%Nguyen'` hoặc `LIKE '%Nguyen%'`: Tìm kiếm chuỗi chứa chữ "Nguyen" ở giữa hoặc cuối. SQL Server **BẮT BUỘC** phải thực hiện Index Scan (duyệt toàn bộ), rất chậm. Nếu thực sự cần tìm kiểu này, hãy cân nhắc dùng tính năng _Full-Text Search_.

4. **Cẩn thận với các toán tử phủ định**: Các toán tử như `<>` (Khác), `NOT IN`, `NOT EXISTS`, `NOT LIKE` thường không thể tận dụng tốt cấu trúc B-Tree của chỉ mục, dẫn đến Table Scan. Hãy cố gắng chuyển đổi thành toán tử khẳng định nếu có thể logic.
5. **Tận dụng `EXISTS` thay cho `IN**`: Khi kiểm tra sự tồn tại của một tập con, sử dụng truy vấn con `WHERE EXISTS (SELECT 1 FROM ...)`thường có tốc độ thực thi (Plan) tối ưu hơn so với mệnh đề`IN`, đặc biệt khi bảng con có chứa giá trị `NULL`.
6. **Tránh ép kiểu dữ liệu ngầm định (Implicit Conversion)**: Đảm bảo kiểu dữ liệu của biến truyền vào phải trùng khớp chính xác với kiểu dữ liệu của cột. Nếu cột là `VARCHAR` nhưng bạn so sánh `WHERE Cot = N'Chuỗi'` (truyền vào `NVARCHAR`), hệ thống sẽ phải ép kiểu ngầm định toàn bộ dữ liệu cột đó, làm tê liệt hiệu năng Index.

---

### IV. DEMO ĐÁNH GIÁ (TRÍCH XUẤT TỪ TÀI LIỆU)

Tài liệu cung cấp một bài thử nghiệm (Demo) sinh ra **10.000.000 dòng** (10 triệu mẫu tin) để kiểm chứng hiệu năng:

- **Tình huống 1: Không có Index**
- Lệnh `SELECT TOP 10000000 lastname, count(id_contact) ... GROUP BY lastname` (nhóm theo 1 cột không có Index).
- _Thời gian thực thi:_ Mất **2 phút 49 giây**. Quá trình sử dụng Table Scan và tốn nhiều RAM cho Hash Match (Gom nhóm).

- **Tình huống 2: Có dùng `ORDER BY` nhưng chưa có Index**
- _Thời gian thực thi:_ Dao động từ **1 phút 30 giây - 1 phút 51 giây**. Khá chậm vì hệ thống phải nạp 10 triệu dòng vào bộ nhớ (Sort Operator) để sắp xếp thủ công.

- **Tình huống 3: Đã tạo Index trên cột `lastname**`
- Khi truy vấn và sắp xếp lại, SQL Server sử dụng Index để lấy kết quả đã được cây B-Tree xếp sẵn.
- _Thời gian thực thi:_ Nhanh hơn đáng kể, dao động từ **1 phút 18 giây - 1 phút 31 giây**. Mức độ đọc I/O đĩa cứng cũng giảm rõ rệt (dựa vào thông số _Logical Reads_).

---

### GHI CHÚ TRÊN LỚP (Quicknote)

> Các ghi chú bổ sung từ bài giảng trên lớp về tối ưu truy vấn.

**1. Thứ tự thực hiện các phép toán đại số quan hệ:**
- **Phép chọn (σ) / Phép chiếu (π)** nên thực hiện **TRƯỚC**, phép kết nối (⋈) thực hiện **SAU**.
- Lý do: Giảm kích thước tập dữ liệu trước khi join sẽ giảm đáng kể số lượng phép so sánh.

**2. Khử phép nối (JOIN) khi có thể:**
- Nếu kết quả truy vấn chỉ cần dữ liệu từ một bảng, hãy dùng `EXISTS` hoặc `IN` thay vì `JOIN` để tránh tạo tập kết quả trung gian không cần thiết.

**3. Giảm thiểu điều kiện trùng lặp:**
- Nếu một điều kiện xuất hiện nhiều lần, hãy tái cấu trúc (refactor) mệnh đề WHERE.

```sql
-- ❌ Chưa tối ưu (điều kiện phai = 'Nam' bị lặp)
WHERE (malop = 'x' AND phai = 'Nam') OR (malop = 'y' AND phai = 'Nam')

-- ✅ Đã tối ưu (rút gọn điều kiện chung)
WHERE phai = 'Nam' AND malop IN ('x', 'y')
```

**4. Sắp xếp điều kiện theo xác suất:**

| Toán tử | Quy tắc |
|---|---|
| **AND** | Đặt điều kiện có **xác suất SAI cao** lên **đầu** (short-circuit nhanh hơn) |
| **OR** | Đặt điều kiện có **xác suất ĐÚNG cao** lên **đầu** (short-circuit nhanh hơn) |

**5. Sử dụng Index Hint (`WITH (INDEX=...)`):**
- Các field tham gia trong điều kiện truy vấn nên được đánh Index trước.
- Thứ tự sắp xếp Index phải được sử dụng đúng trong mệnh đề truy vấn:

```sql
-- Ép SQL Server dùng Index cụ thể (thay vì để Query Optimizer tự chọn)
SELECT * FROM NhanVien WITH (INDEX = IX_NhanVien_MaPhong)
WHERE MaPhong = 'P01'
```
