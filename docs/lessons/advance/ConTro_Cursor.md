### I. KHÁI NIỆM VỀ CURSOR (CON TRỎ)

Trong cơ sở dữ liệu quan hệ (RDBMS) như SQL Server, các câu lệnh SQL thông thường hoạt động theo cơ chế **hướng tập hợp (Set-based)** – nghĩa là xử lý toàn bộ các dòng dữ liệu cùng một lúc.

Tuy nhiên, trong một số bài toán nghiệp vụ phức tạp, lập trình viên cần xử lý dữ liệu theo cơ chế **tuần tự từng dòng một (Row-by-row / Procedural)**. Lúc này, ta sử dụng **Cursor**.

- **Cursor là gì?** Là một đối tượng cơ sở dữ liệu dùng để quản lý một tập các records (kết quả trả về của một phát biểu `SQL-Select`) và cho phép duyệt qua, xử lý từng mẫu tin một trong tập kết quả đó.
- **Phạm vi áp dụng:** Thường được sử dụng phổ biến bên trong các **Stored Procedures** (Thủ tục lưu trữ) và **Triggers** (Trình kích hoạt).
- **Khi nào nên dùng?** Khi bạn muốn thực hiện các cách thức xử lý khác nhau cho từng mẫu tin cụ thể, hoặc khi cần nhận kết quả trả về từng hàng để tính toán lũy tiến (ví dụ: thuật toán khớp lệnh tài chính/chứng khoán, tính số dư ngân hàng, gửi email tự động theo danh sách).

---

### II. VÒNG ĐỜI VÀ TIẾN TRÌNH SỬ DỤNG CURSOR

Quá trình sử dụng một Cursor trong T-SQL bắt buộc phải trải qua 5 bước nghiêm ngặt theo trình tự sau:

1. **Khai báo biến dữ liệu & Khai báo Cursor (`DECLARE`):** Định nghĩa câu lệnh `SELECT` tạo ra tập dữ liệu và khai báo các biến có kiểu dữ liệu tương ứng với các cột trong câu lệnh `SELECT` để chứa dữ liệu khi duyệt.
2. **Mở Cursor (`OPEN`):** Thực thi câu lệnh `SELECT` liên kết với Cursor và nạp tập kết quả vào bộ nhớ vùng đệm.
3. **Đọc dữ liệu (`FETCH INTO`):** Lấy dữ liệu từ một mẫu tin hiện tại trong Cursor đưa vào các biến đã khai báo để xử lý. Tiến trình này thường được lồng trong một vòng lặp `WHILE` để duyệt cho đến dòng cuối cùng.
4. **Đóng Cursor (`CLOSE`):** Ngắt kết nối dữ liệu và giải phóng các tài nguyên đệm của dòng hiện tại, nhưng giữ nguyên cấu trúc định nghĩa để có thể mở lại (`OPEN`) nếu cần.
5. **Giải phóng vùng nhớ (`DEALLOCATE`):** Xóa bỏ hoàn toàn định nghĩa Cursor và giải phóng toàn bộ tài nguyên hệ thống gắn liền với nó.

---

### III. CÚ PHÁP CHI TIẾT (SYNTAX) VÀ ĐOẠN MÃ MẪU

#### 1. Khai báo biến nhận dữ liệu

Trước khi khai báo con trỏ, bạn phải dùng lệnh `DECLARE` để tạo các biến lưu trữ tạm thời một dòng dữ liệu lấy từ con trỏ:

```sql
DECLARE @Bien_1 Kieu_Du_Lieu, @Bien_2 Kieu_Du_Lieu;

```

#### 2. Cú pháp khai báo Cursor đầy đủ

```sql
DECLARE cursor_name CURSOR
    [ LOCAL | GLOBAL ]
    [ FORWARD_ONLY | SCROLL ]
    [ STATIC | KEYSET | DYNAMIC | FAST_FORWARD ]
    [ READ_ONLY | SCROLL_LOCKS | OPTIMISTIC ]
    [ TYPE_WARNING ]
FOR select_statement
    [ FOR UPDATE [ OF column_name [ , ... n ] ] ]

```

_Giải thích các tùy chọn quan trọng:_

- `LOCAL` / `GLOBAL`: Phạm vi của con trỏ. `LOCAL` nghĩa là con trỏ cục bộ (chỉ tồn tại trong khối lệnh, SP hoặc Trigger tạo ra nó). `GLOBAL` giúp con trỏ có tác dụng toàn cục trong suốt phiên kết nối (Connection).
- `FORWARD_ONLY` / `SCROLL`: Hướng duyệt dữ liệu. `FORWARD_ONLY` (Mặc định) chỉ cho phép duyệt từ dòng đầu đến dòng cuối. `SCROLL` cho phép nhảy tự do tới bất kỳ dòng nào bằng các lệnh: `FETCH FIRST` (dòng đầu), `FETCH LAST` (dòng cuối), `FETCH PRIOR` (dòng trước), `FETCH ABSOLUTE n` (dòng thứ n).
- `STATIC` / `KEYSET` / `DYNAMIC`: Cách con trỏ phản ánh sự thay đổi dữ liệu của bảng gốc. `STATIC` sẽ sao lưu dữ liệu vào bảng tạm ở `tempdb` nên khi bảng gốc bị thay đổi bởi user khác, dữ liệu trong con trỏ không đổi. `DYNAMIC` phản ánh tức thời mọi thay đổi ở bảng gốc khi ta đang duyệt.
- `FAST_FORWARD`: Cấu hình tối ưu hóa đặc biệt, tương đương với một con trỏ có thuộc tính `FORWARD_ONLY` và `READ_ONLY` được tối ưu hiệu năng đọc tốt nhất.

#### 3. Mở, Đọc và Vòng lặp duyệt Cursor

Để kiểm tra xem việc đọc dữ liệu có thành công hay không, SQL Server sử dụng biến toàn cục hệ thống **`@@FETCH_STATUS`**:

- `0`: Lệnh `FETCH` thực hiện thành công, đã lấy được dữ liệu dòng tiếp theo.
- `-1`: Lệnh `FETCH` thất bại hoặc đã duyệt hết dữ liệu (vượt quá dòng cuối cùng).
- `-2`: Dòng được chọn không còn tồn tại trong bảng (do bị user khác xóa).

_Cấu trúc vòng lặp mẫu chuẩn:_

```sql
-- Mở con trỏ
OPEN cursor_name;

-- Đọc mẫu tin đầu tiên
FETCH NEXT FROM cursor_name INTO @Bien_1, @Bien_2;

-- Vòng lặp duyệt dữ liệu
WHILE @@FETCH_STATUS = 0
BEGIN
    -- [Nơi xử lý logic nghiệp vụ với @Bien_1, @Bien_2]

    -- Tiếp tục đọc mẫu tin tiếp theo
    FETCH NEXT FROM cursor_name INTO @Bien_1, @Bien_2;
END

-- Đóng và Giải phóng tài nguyên
CLOSE cursor_name;
DEALLOCATE cursor_name;

```

---

### IV. TÍNH NĂNG CẬP NHẬT DỮ LIỆU QUA CURSOR (`WHERE CURRENT OF`)

Một kỹ thuật rất mạnh mẽ của Cursor là khả năng chỉnh sửa (`UPDATE`) hoặc xóa (`DELETE`) chính xác ngay tại dòng dữ liệu mà con trỏ đang đứng mà không cần phải tìm lại bằng Khóa chính. Bạn thực hiện bằng cách dùng mệnh đề **`WHERE CURRENT OF cursor_name`**.

_Đoạn mã ứng dụng thực tế trong quản lý khớp lệnh của bạn:_

```sql
-- Khai báo biến nhận dữ liệu từ con trỏ
DECLARE @ngaydat NVARCHAR(50), @soluong INT, @giadat FLOAT;

-- Tiến hành duyệt con trỏ @CrsrVar
FETCH NEXT FROM @CrsrVar INTO @ngaydat, @soluong, @giadat;

WHILE (@@FETCH_STATUS <> -1 AND @soluongMB > 0)
BEGIN
    IF (@LoaiGD = 'B')
    BEGIN
        IF (@giadatMB <= @giadat)
        BEGIN
            IF @soluongMB > @soluong
            BEGIN
                -- Cập nhật trực tiếp số lượng của dòng hiện tại trong con trỏ về 0
                UPDATE dbo.LENHDAT
                SET SOLUONG = 0
                WHERE CURRENT OF @CrsrVar;

                SET @soluongMB = @soluongMB - @soluong;
            END
            ELSE
            BEGIN
                -- Trừ bớt số lượng ngay tại dòng hiện tại con trỏ đang đứng
                UPDATE dbo.LENHDAT
                SET SOLUONG = SOLUONG - @soluongMB
                WHERE CURRENT OF @CrsrVar;

                SET @soluongMB = 0;
            END
        END
    END
    FETCH NEXT FROM @CrsrVar INTO @ngaydat, @soluong, @giadat;
END

```

---

### V. ĐÁNH GIÁ ƯU - NHƯỢC ĐIỂM VÀ LỜI KHUYÊN KHI SỬ DỤNG

Dù Cursor giải quyết tốt các bài toán xử lý tuần tự, nhưng trong quản trị CSDL, nó luôn đi kèm với những khuyến cáo lớn về hiệu năng.

| Tiêu chí    | Đánh giá                                                                                                    |
| ----------- | ----------------------------------------------------------------------------------------------------------- |
| **Ưu điểm** | - Giúp viết các thuật toán phức tạp đòi hỏi tính toán bắc cầu, lũy tiến giữa các dòng dữ liệu với nhau.<br> |

<br>- Dễ dàng tích hợp các hành động lập trình bên ngoài (như gọi Stored Procedure khác ứng với mỗi dòng). |
| **Nhược điểm** | - **Hạ thấp hiệu năng nghiêm trọng (Slow Performance):** Việc đọc/ghi từng dòng một gây tốn chi phí I/O ổ đĩa và CPU gấp nhiều lần xử lý tập hợp.<br>

<br>- **Nghẽn mạch hệ thống (Locking & Blocking):** Khi duyệt lâu, Cursor có thể giữ các khóa (Locks) trên dòng hoặc bảng, gây ra hiện tượng hàng chờ kéo dài hoặc `Deadlock` cho các User khác. |

**Lời khuyên cốt lõi (Best Practices):**

1. **Hạn chế tối đa:** Hãy luôn cố gắng tìm giải pháp thay thế Cursor bằng các kỹ thuật hướng tập hợp hiệu năng cao như: `JOIN`, `Subquery` (Truy vấn con), `CTE` (Bảng tạm biểu thức), hoặc `Window Functions` (`ROW_NUMBER()`, `SUM() OVER()`).
2. **Tối ưu hóa khi bắt buộc phải dùng:** Nếu không thể thay thế, hãy luôn khai báo rõ thuộc tính con trỏ là **`LOCAL FORWARD_ONLY STATIC`** hoặc **`LOCAL FAST_FORWARD`**. Điều này giúp SQL Server hiểu con trỏ chỉ đọc một chiều, không tốn tài nguyên quản lý khóa, giúp bảo vệ tốc độ vận hành của hệ thống một cách tốt nhất.

---

### GHI CHÚ TRÊN LỚP (Quicknote)

> Bài tập ứng dụng Cursor: Import dữ liệu NCKH từ file Excel vào database.

#### Bài tập: Import dữ liệu NCKH từ Excel

**Dữ liệu mẫu trong file Excel** (Sheet: `Sheet1`):

| MAGV       | Tên Giảng viên      | Tổng giờ NCKH |
|------------|---------------------|---------------|
| GV001      | Nguyễn Văn An       | 120.5         |
| GV002      | Trần Thị Bích       | 85.0          |
| GV003      | Lê Hoàng Cường      | 200.0         |
| GV004      | Phạm Minh Đức       | 0.0           |
| GV005      | Võ Thị Em           | 45.5          |

> **Tải driver:** Cần cài đặt `Microsoft.ACE.OLEDB.12.0` — [Download](https://www.microsoft.com/en-us/download/details.aspx?id=54920)

**Bước 1: Bật cấu hình nâng cao (chỉ cần chạy 1 lần trên Server):**

```sql
USE [master];
GO
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'Ad Hoc Distributed Queries', 1;
RECONFIGURE;
GO

-- Bật AllowInProcess và DynamicParameters cho ACE.OLEDB.12.0
EXEC master.dbo.sp_MSset_oledb_prop N'Microsoft.ACE.OLEDB.12.0', N'AllowInProcess', 1;
EXEC master.dbo.sp_MSset_oledb_prop N'Microsoft.ACE.OLEDB.12.0', N'DynamicParameters', 1;
GO
```

**Bước 2: Tạo bảng GiangVien và NCKH:**

```sql
USE [BT_CURSOR];
GO

CREATE TABLE GiangVien (
    MA_GV NCHAR(15) PRIMARY KEY,
    HOTEN_GV NVARCHAR(200)
);

CREATE TABLE NCKH (
    MA_GV NCHAR(15),
    NIEN_KHOA NCHAR(11),
    ID_HOCKY INT,
    TT_NCKH FLOAT,
    PRIMARY KEY (MA_GV, NIEN_KHOA, ID_HOCKY),
    FOREIGN KEY (MA_GV) REFERENCES GiangVien(MA_GV)
);
```

**Bước 3: SP Import dùng OPENROWSET + Cursor:**

```sql
CREATE PROCEDURE SP_Import_TrucTiep_Tu_Excel
    @FilePath NVARCHAR(500),   -- VD: 'D:\NCKH_2018.xlsx'
    @SheetName NVARCHAR(100),  -- VD: 'Sheet1$' (nhớ dấu $ cuối)
    @NienKhoa NCHAR(11),       -- VD: '2022-2023'
    @IdHocKy INT               -- VD: 1
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Tạo bảng tạm để chứa dữ liệu từ Excel
    IF OBJECT_ID('tempdb..#TEMP_EXCEL') IS NOT NULL DROP TABLE #TEMP_EXCEL;
    CREATE TABLE #TEMP_EXCEL (
        MA_GV NCHAR(15),
        HOTEN_GV NVARCHAR(200),
        NCKH FLOAT
    );

    -- 2. Đọc Excel bằng Dynamic SQL + OPENROWSET
    DECLARE @SQL NVARCHAR(MAX);
    SET @SQL = N'
        INSERT INTO #TEMP_EXCEL (MA_GV, HOTEN_GV, NCKH)
        SELECT [MAGV], [Tên Giảng viên], [Tổng giờ NCKH]
        FROM OPENROWSET(''Microsoft.ACE.OLEDB.12.0'',
                        ''Excel 12.0 Xml;Database=' + @FilePath + ';HDR=YES;IMEX=1'',
                        ''SELECT * FROM [' + @SheetName + ']'')';

    BEGIN TRY
        EXEC sp_executesql @SQL;
    END TRY
    BEGIN CATCH
        PRINT N'LỖI: Không thể đọc file Excel. Kiểm tra đường dẫn hoặc driver ACE.OLEDB.12.0';
        PRINT ERROR_MESSAGE();
        RETURN;
    END CATCH

    -- 3. Dùng Cursor duyệt từng dòng để INSERT/UPDATE
    DECLARE @MaGV NCHAR(15), @HoTenGV NVARCHAR(200), @SoGioNCKH FLOAT;

    DECLARE cur_ImportNCKH CURSOR LOCAL FAST_FORWARD FOR
        SELECT MA_GV, HOTEN_GV, NCKH FROM #TEMP_EXCEL;

    OPEN cur_ImportNCKH;
    FETCH NEXT FROM cur_ImportNCKH INTO @MaGV, @HoTenGV, @SoGioNCKH;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Xử lý 1: Cập nhật / Thêm Giảng Viên
        IF NOT EXISTS (SELECT 1 FROM GiangVien WHERE MA_GV = @MaGV)
            INSERT INTO GiangVien (MA_GV, HOTEN_GV) VALUES (@MaGV, @HoTenGV);
        ELSE
            UPDATE GiangVien SET HOTEN_GV = @HoTenGV WHERE MA_GV = @MaGV;

        -- Xử lý 2: Import NCKH (kết hợp tham số từ bên ngoài vào làm Khóa chính)
        IF EXISTS (SELECT 1 FROM NCKH WHERE MA_GV = @MaGV AND NIEN_KHOA = @NienKhoa AND ID_HOCKY = @IdHocKy)
            UPDATE NCKH SET TT_NCKH = @SoGioNCKH
            WHERE MA_GV = @MaGV AND NIEN_KHOA = @NienKhoa AND ID_HOCKY = @IdHocKy;
        ELSE
            INSERT INTO NCKH (MA_GV, NIEN_KHOA, ID_HOCKY, TT_NCKH)
            VALUES (@MaGV, @NienKhoa, @IdHocKy, @SoGioNCKH);

        FETCH NEXT FROM cur_ImportNCKH INTO @MaGV, @HoTenGV, @SoGioNCKH;
    END

    -- 4. Dọn dẹp
    CLOSE cur_ImportNCKH;
    DEALLOCATE cur_ImportNCKH;
    DROP TABLE #TEMP_EXCEL;

    PRINT N'Import thành công từ file Excel!';
END
GO
```

**Bước 4: Chạy thử:**

```sql
EXEC SP_Import_TrucTiep_Tu_Excel
    @FilePath  = 'C:\BT_CURSOR\BAOCAO_NCKH.xlsx',
    @SheetName = 'Sheet1$',    -- Nhớ dấu $ ở cuối tên Sheet
    @NienKhoa  = '2022-2023',
    @IdHocKy   = 1;

-- Kiểm tra kết quả
SELECT * FROM GiangVien;
SELECT * FROM NCKH;
```

### 📌 CÁC LƯU Ý QUAN TRỌNG (HỎI ĐÁP THỰC TẾ)

#### 1. Tại Sao Lại Cần Dùng Cursor Khi SQL Đã SELECT Được Hết?
Bản chất SQL là hướng tập hợp (Set-based) rất nhanh, nhưng ta bắt buộc phải dùng Cursor (xử lý tuần tự từng dòng) trong các trường hợp sau:
*   **Hành động bên ngoài hệ thống:** Cần thực hiện hành động phi-SQL cho từng dòng như: duyệt danh sách gửi 100 email riêng lẻ, gọi 100 API thanh toán của bên thứ ba.
*   **Tính toán bắc cầu/lũy tiến:** Giá trị xử lý của dòng sau phụ thuộc trực tiếp vào kết quả cập nhật của dòng trước (ví dụ: thuật toán khớp lệnh mua bán cổ phiếu, tính lãi suất cộng dồn).
*   **Chạy SQL động (Dynamic SQL):** Ví dụ duyệt danh sách tên database thu được từ bảng hệ thống để chạy lệnh `BACKUP DATABASE` từng cái một.

#### 2. Quy Tắc Hoạt Động của `FETCH NEXT INTO`
*   **NEXT:** Đẩy "mũi tên chỉ vị trí" dịch xuống dòng tiếp theo ngay dưới dòng hiện tại.
*   **INTO:** Copy các giá trị cột của dòng đó vào các biến cục bộ tương ứng để lập trình logic.
*   **Nguyên tắc số lượng:** Số lượng biến gán `INTO` **bắt buộc phải bằng đúng** số lượng cột trong lệnh `SELECT` của con trỏ. Nếu con trỏ chọn 4 cột mà chỉ `INTO` 3 biến, SQL Server sẽ báo lỗi biên dịch lập tức.
*   **Trạng thái `@@FETCH_STATUS`:** Nếu đọc được dòng mới thì biến này bằng `0`. Nếu hết dòng (duyệt hết bảng) thì bằng `-1`. Bắt buộc phải có câu lệnh `FETCH NEXT` thứ hai ở cuối thân vòng lặp `WHILE` để cập nhật lại status này, tránh vòng lặp vô hạn.

#### 3. Phân Biệt Biến `@` (1 Dấu) và `@@` (2 Dấu)
*   **Một dấu `@` (Local Variables):** Biến cục bộ do bạn tự khai báo (`DECLARE`) và quản lý (có toàn quyền đọc/ghi).
*   **Hai dấu `@@` (System Variables):** Biến hệ thống toàn cục do SQL Server tự định nghĩa sẵn và quản lý (chỉ được đọc - Read-only, tự động nhảy số theo trạng thái hệ thống).

#### 4. Biến Con Trỏ (`@CrsrVar`) Là Gì?
*   Là một biến cục bộ có kiểu dữ liệu là `CURSOR` (tham chiếu đến vùng nhớ con trỏ).
*   **Ưu điểm:** Có thể truyền biến con trỏ này qua lại giữa các Stored Procedure khác nhau (bằng tham số `OUTPUT`). Ngoài ra, khi thoát khỏi Stored Procedure, SQL Server sẽ tự động giải phóng bộ nhớ của biến con trỏ này, hạn chế tối đa lỗi rò rỉ bộ nhớ (Memory Leak) do quên `DEALLOCATE`.

#### 5. Cơ Chế `WHERE CURRENT OF` và Cách Reset Con Trỏ Quay Lại Đầu Bảng
*   **`WHERE CURRENT OF`:** SQL Server lưu địa chỉ vật lý (RID hoặc Clustered Index Key) của dòng con trỏ đang đứng trực tiếp trên RAM. Lệnh này giúp thực hiện `UPDATE`/`DELETE` thẳng vào địa chỉ đó cực nhanh mà không cần quét bảng tìm khóa chính.
*   **Quay lại đầu bảng:** Mặc định con trỏ chỉ đi một chiều (`FORWARD_ONLY`). Khi duyệt đến cuối bảng và hết dữ liệu, để chạy lại vòng lặp mới từ đầu, ta có 2 cách:
    *   **Cách 1:** Chạy `CLOSE @CrsrVar` rồi `OPEN @CrsrVar` lại (Cách này tối ưu hiệu năng nhất).
    *   **Cách 2:** Khai báo con trỏ dạng `SCROLL` từ đầu, sau đó gọi lệnh `FETCH FIRST` để nhảy thẳng về dòng 1.

