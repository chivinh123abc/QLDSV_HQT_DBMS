**CHƯƠNG 8. TRIGGER VÀ USER DEFINED FUNCTION (UDF)**

Mục đích: Nắm vững hai đối tượng lập trình nâng cao trong CSDL. Nắm được cách tự động hóa việc kiểm tra ràng buộc toàn vẹn dữ liệu thông qua Trigger, và cách đóng gói các đoạn mã tái sử dụng thông qua UDF.

---

### PHẦN A. TRIGGER (TRÌNH KÍCH HOẠT)

**1. Khái niệm Trigger**

- Trigger là một loại Stored Procedure (Thủ tục lưu trữ) đặc biệt, được thực hiện một cách **tự động** khi người dùng thực hiện các câu lệnh thao tác cập nhật dữ liệu (`INSERT`, `UPDATE`, `DELETE`) trên một Table hoặc View.
- **Mục đích chính:** Đảm bảo sự an toàn và toàn vẹn dữ liệu ở mức độ phức tạp (ví dụ: tham chiếu chéo dữ liệu giữa nhiều bảng, kiểm tra logic nghiệp vụ) mà các ràng buộc thông thường (như Primary Key, Foreign Key, Check Constraint) không đáp ứng được.
- Mỗi bảng có thể có nhiều trigger tương ứng với các hành động khác nhau. Không thể gọi Trigger thực thi thủ công bằng lệnh `EXECUTE` như thủ tục thông thường.

**2. Hai bảng ảo (Magic Tables): `INSERTED` và `DELETED**`
Để kiểm tra trạng thái dữ liệu trước và sau khi thay đổi, SQL Server tự động tạo ra hai bảng tạm thời nằm trong bộ nhớ RAM mỗi khi Trigger được kích hoạt:

- **Bảng `INSERTED`:** Lưu trữ bản sao của các dòng dữ liệu **MỚI** vừa được thêm vào bảng (thông qua lệnh `INSERT` hoặc `UPDATE`).
- **Bảng `DELETED`:** Lưu trữ bản sao của các dòng dữ liệu **CŨ** vừa bị xóa khỏi bảng (thông qua lệnh `DELETE` hoặc phần dữ liệu cũ bị thay thế bởi lệnh `UPDATE`).
- _Nguyên lý hoạt động:_
- Với lệnh `INSERT`: Bảng `Inserted` chứa dữ liệu mới, bảng `Deleted` rỗng.
- Với lệnh `DELETE`: Bảng `Deleted` chứa dữ liệu bị xóa, bảng `Inserted` rỗng.
- Với lệnh `UPDATE` (Bản chất là xóa dòng cũ và chèn dòng mới): Bảng `Deleted` chứa dòng cũ, bảng `Inserted` chứa dòng mới.

**3. Phân loại Trigger**

- **AFTER TRIGGER (hoặc FOR TRIGGER):** Là loại mặc định. Trigger này chỉ được thực thi **SAU KHI** câu lệnh thay đổi dữ liệu đã chạy xong và vượt qua được tất cả các kiểm tra ràng buộc Constraint. Nếu dữ liệu có vấn đề (do code kiểm tra trong trigger phát hiện), ta có thể sử dụng lệnh `ROLLBACK TRANSACTION` để hủy bỏ thao tác. Chỉ được tạo trên Table.
- **INSTEAD OF TRIGGER:** Trigger sẽ chạy **THAY THẾ** trực tiếp cho câu lệnh thao tác dữ liệu. Dữ liệu thực chất chưa được ghi vào bảng. Loại này thường được tạo trên các VIEW để cho phép người dùng thực hiện lệnh Cập nhật/Thêm/Xóa thông qua một View phức tạp (nhiều bảng).

**4. Cú pháp thao tác với Trigger**

- **Tạo Trigger:**

```sql
CREATE TRIGGER trigger_name
ON { table | view }
{ FOR | AFTER | INSTEAD OF } { [DELETE] [,] [INSERT] [,] [UPDATE] }
AS
BEGIN
    -- Các câu lệnh T-SQL xử lý logic, truy vấn bảng inserted/deleted
END

```

- **Hàm kiểm tra cột `UPDATE(tên_cột)`:** Thường dùng bên trong Trigger để kiểm tra xem một cột cụ thể có bị thay đổi (bị cập nhật) hay không. Ví dụ: `IF UPDATE(SoLuong) BEGIN ... END`
- **Xóa / Quản lý Trigger:**
- Xóa: `DROP TRIGGER trigger_name;`
- Vô hiệu hóa: `DISABLE TRIGGER trigger_name ON table_name;`

---

### PHẦN B. USER DEFINED FUNCTION - UDF (HÀM DO NGƯỜI DÙNG ĐỊNH NGHĨA)

**1. Khái niệm Hàm (Function)**

- Hàm trong ngôn ngữ truy vấn là những chương trình con dùng để đóng gói các đoạn lệnh được thực thi thường xuyên nhằm tái sử dụng mã.
- Tên của hàm (Gồm `database_name.owner_name.function_name`) không được trùng lặp.
- Khác với Stored Procedure, hàm **bắt buộc phải trả về kết quả**.
- **Lưu ý quan trọng về lỗi:** Trong Stored Procedure hay Trigger, nếu một câu lệnh bị lỗi thì câu lệnh tiếp theo trong khối vẫn có thể được thực hiện tiếp. Nhưng trong hàm, lỗi sẽ làm **dừng ngay hàm**, đồng thời làm cho câu lệnh SQL đang gọi hàm đó bị hủy bỏ hoàn toàn.

**2. Phân loại UDF theo nguồn gốc**

- **Hàm cài đặt sẵn (Built-in Functions):** Được định nghĩa sẵn bởi SQL Server, không thể hiệu chỉnh hay xóa.
- _Toán học:_ `ABS()` (trị tuyệt đối), `ROUND()` (làm tròn), `POWER()` (lũy thừa), `SQRT()` (căn), `CEILING()`, `FLOOR()`...
- _Chuỗi:_ `LEN()` (chiều dài), `LEFT()`, `RIGHT()`, `SUBSTRING()` (cắt chuỗi), `CHARINDEX()` (tìm vị trí), `UPPER()`, `LOWER()`...
- _Thời gian:_ `GETDATE()` (lấy ngày giờ hiện hành, rất hữu ích khi làm báo cáo), `YEAR()`, `MONTH()`...

- **Hàm do người dùng định nghĩa (User-defined Functions):** Được tạo ra thông qua phát biểu `CREATE FUNCTION`. UDF có thể nhận nhiều tham số, hoặc không nhận tham số nào.

**3. Các loại Hàm do người dùng tự tạo (UDF Types)**
Dựa vào giá trị trả về, UDF được chia làm 3 dạng chính:

- **Loại 1: Scalar Function (Hàm vô hướng)**
- Thực hiện tính toán và trả về **một giá trị đơn nhất** (kiểu int, float, varchar...).
- Thường được sử dụng như một hằng số trong các mệnh đề `SELECT`, `WHERE` của một truy vấn khác.
- _Cú pháp:_

```sql
CREATE FUNCTION ten_ham (@thamso kieudulieu)
RETURNS kieu_tra_ve
AS
BEGIN
    -- Xử lý
    RETURN @gia_tri
END

```

- **Loại 2: Inline Table-valued Function (Hàm giá trị bảng nội tuyến)**
- Trả về kết quả là một **Bảng (Table)** thông qua **một câu lệnh SELECT duy nhất**.
- Loại hàm này không sử dụng khối lệnh `BEGIN...END`. Nó hoạt động tương tự như một dạng View có thể truyền tham số.
- _Cú pháp:_

```sql
CREATE FUNCTION ten_ham (@thamso kieudulieu)
RETURNS TABLE
AS
RETURN ( SELECT ... FROM ... WHERE ... = @thamso )

```

- **Loại 3: Multi-statement Table-valued Function (Hàm giá trị bảng đa câu lệnh)**
- Trả về một **Bảng (Table)**, nhưng chứa nhiều câu lệnh SQL phức tạp bên trong (có IF, WHILE, khai báo biến...).
- Yêu cầu bạn phải định nghĩa rõ cấu trúc bảng trả về ngay từ đầu, và phải đóng gói thuật toán trong cặp `BEGIN...END`. Cuối cùng phải có chữ `RETURN`.
- _Cú pháp:_

```sql
CREATE FUNCTION ten_ham (@thamso kieudulieu)
RETURNS @TenBangTraVe TABLE (Cot1 kieu, Cot2 kieu)
AS
BEGIN
    -- Tính toán phức tạp...
    INSERT INTO @TenBangTraVe VALUES (...)
    RETURN
END

```

---

### GHI CHÚ TRÊN LỚP (Quicknote)

> Các ghi chú bổ sung từ bài giảng trên lớp.

**1. So sánh View, Stored Procedure và Hàm (UDF):**

| Đặc điểm | View | Stored Procedure | Hàm (UDF) |
|---|---|---|---|
| **Nhận tham số** | ❌ Không | ✅ Có (tham trị + tham biến) | ✅ Có (chỉ tham trị, tối đa 2100) |
| **Trả kết quả kế thừa được** | ✅ Đơn giản nhất (SELECT trực tiếp) | ✅ Có (qua `INSERT INTO...EXEC`) | ✅ Có (tương đương View, dùng trong SELECT) |
| **Ưu điểm** | Đơn giản, kế thừa kết quả dễ dàng | Linh hoạt nhất, nhận cả tham biến | Kế thừa kết quả tương đương View + nhận tham số |
| **Khuyết điểm** | Không nhận tham số | Kế thừa kết quả phức tạp hơn (cần table tạm) | Chỉ nhận tham trị, có lệnh `RETURN` |

> **Kết luận:** Cả 3 đối tượng đều cho phép kế thừa kết quả trả về. View là đơn giản nhất nhưng không nhận tham số. SP có thể trả về bảng ảo (table tạm). Hàm linh hoạt hơn View nhưng hạn chế hơn SP.

**2. Hai cách lấy kết quả từ Stored Procedure:**

```sql
-- Cách 1: INSERT INTO...EXEC (trực tiếp)
INSERT INTO BangDich (Cot1, Cot2, Cot3) EXEC sp_TenSP

-- Cách 2: Dùng Table tạm trung gian
CREATE TABLE #KetQua (
    HOCKY INT,
    HELOP INT,
    TONG FLOAT
)
INSERT INTO #KetQua (HOCKY, HELOP, TONG) EXEC SP_THONGKE

-- Sau đó SELECT từ table tạm
SELECT * FROM #KetQua
```

**3. Xử lý lỗi trong Stored Procedure:**

Khi gặp lỗi trong SP, **mặc định SP vẫn chạy tiếp** các lệnh còn lại. Có 3 cách xử lý:

| Phương pháp | Mô tả | Khi nào dùng |
|---|---|---|
| **`SET XACT_ABORT ON`** | Sai là dừng ngay, tự động rollback toàn bộ transaction | Khi ưu tiên **an toàn dữ liệu** và **ngắn gọn** |
| **`TRY...CATCH`** | Bắt lỗi và xử lý logic phức tạp (ghi log, báo lỗi cụ thể cho User) | Khi cần **xử lý lỗi chi tiết** |
| **`@@ERROR`** | Kiểm tra mã lỗi sau mỗi câu lệnh (cách cũ) | Chỉ dùng khi **bảo trì legacy code** |

```sql
-- Ví dụ: SET XACT_ABORT ON
SET XACT_ABORT ON
BEGIN TRANSACTION
    UPDATE BangA SET ...
    UPDATE BangB SET ...  -- Nếu lỗi ở đây → tự rollback cả 2 lệnh
COMMIT TRANSACTION

-- Ví dụ: TRY...CATCH
BEGIN TRY
    BEGIN TRANSACTION
        UPDATE BangA SET ...
        UPDATE BangB SET ...
    COMMIT TRANSACTION
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION
    PRINT ERROR_MESSAGE()
END CATCH
```
