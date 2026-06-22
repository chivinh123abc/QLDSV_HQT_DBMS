**CHUYÊN ĐỀ: CÁC MỨC CÔ LẬP GIAO DỊCH (ISOLATION LEVELS)**

Mục đích: Hiểu rõ cơ chế khóa (Locking) của hệ quản trị CSDL khi có nhiều người dùng (đa luồng) cùng truy xuất và thay đổi một dữ liệu tại cùng một thời điểm. Việc thiết lập đúng mức Isolation Level giúp cân bằng giữa **Hiệu suất (Performance)** và **Tính nhất quán dữ liệu (Data Consistency)**.

### I. CÁC HIỆN TƯỢNG XUNG ĐỘT DỮ LIỆU THƯỜNG GẶP (CONCURRENCY PHENOMENA)

Khi nhiều giao dịch (Transaction) chạy song song mà không bị cô lập tốt, sẽ xảy ra 3 hiện tượng sai lệch dữ liệu cơ bản sau:

1. **Dirty Read (Đọc dữ liệu "rác"):** \* Xảy ra khi Giao dịch A đang sửa một dòng dữ liệu nhưng chưa xác nhận (`COMMIT`). Giao dịch B nhảy vào đọc dòng dữ liệu đó. Ngay sau đó, Giao dịch A gặp lỗi và `ROLLBACK` (hủy bỏ lệnh sửa).

- _Hậu quả:_ Giao dịch B đã đọc và sử dụng một dữ liệu "ảo", thực chất chưa từng tồn tại chính thức trong CSDL.

2. **Non-repeatable Read (Đọc không thể lặp lại):** \* Xảy ra khi Giao dịch A đọc một dòng dữ liệu 2 lần. Giữa 2 lần đọc đó, Giao dịch B nhảy vào **SỬA (`UPDATE`)** hoặc **XÓA (`DELETE`)** dòng đó và `COMMIT`.

- _Hậu quả:_ Giao dịch A đọc cùng một dòng nhưng lại nhận được 2 kết quả khác nhau, gây sai lệch logic tính toán.

3. **Phantom Read (Đọc "bóng ma"):** * Xảy ra khi Giao dịch A đọc một *tập hợp\* các dòng dữ liệu thỏa mãn một điều kiện (`WHERE`). Ngay sau đó, Giao dịch B **THÊM (`INSERT`)** một dòng mới cũng thỏa mãn điều kiện đó và `COMMIT`.

- _Hậu quả:_ Khi Giao dịch A thực hiện đọc lại lần 2 với cùng điều kiện, tự nhiên xuất hiện thêm một (hoặc nhiều) dòng dữ liệu "bóng ma" mà lần 1 không thấy.

---

### II. CÁC MỨC ISOLATION LEVEL TRONG SQL SERVER

Để ngăn chặn các hiện tượng trên, SQL Server cung cấp 5 mức độ cô lập (Isolation Levels) đi từ nới lỏng nhất đến nghiêm ngặt nhất. Mức độ càng nghiêm ngặt thì dữ liệu càng an toàn, nhưng hiệu suất hệ thống sẽ càng chậm do phải chờ đợi khóa (Lock) lẫn nhau.

**1. READ UNCOMMITTED (Đọc không giới hạn - Thấp nhất)**

- Giao dịch được phép đọc dữ liệu đang bị chỉnh sửa bởi giao dịch khác dù chưa `COMMIT`.
- Bỏ qua tất cả các khóa bảo vệ (Locks).
- **Đặc điểm:** Tốc độ nhanh nhất, không bị hiện tượng chờ nghẽn mạch (Blocking). Tuy nhiên, mắc phải cả 3 lỗi: _Dirty Read, Non-repeatable Read, và Phantom Read_.

**2. READ COMMITTED (Mặc định của SQL Server)**

- Là mức độ tiêu chuẩn của hầu hết các hệ quản trị CSDL.
- Chỉ cho phép đọc những dữ liệu đã được `COMMIT` thành công. Nếu dữ liệu đang bị người khác sửa (chưa xong), giao dịch đọc phải chờ.
- **Đặc điểm:** Chống được hiện tượng _Dirty Read_. Nhưng vẫn có thể bị _Non-repeatable Read_ và _Phantom Read_.

**3. REPEATABLE READ (Đọc lặp lại)**

- Khi một giao dịch đọc dữ liệu, hệ thống sẽ "khóa" (Lock) các dòng đó lại, tuyệt đối không cho giao dịch khác sửa (`UPDATE`) hay xóa (`DELETE`) cho đến khi giao dịch đọc hoàn tất.
- **Đặc điểm:** Chống được _Dirty Read_ và _Non-repeatable Read_. Dữ liệu đọc 2 lần chắc chắn giống nhau. Tuy nhiên, vẫn không cấm được người khác chèn thêm dòng mới (`INSERT`), nên vẫn có thể bị _Phantom Read_.

**4. SERIALIZABLE (Cô lập tuần tự - Khắt khe nhất)**

- Giao dịch sẽ khóa toàn bộ một phạm vi dữ liệu (Range Lock). Nghĩa là không ai được phép sửa, xóa, và cũng **không được phép thêm (INSERT)** bất kỳ dòng nào vào phạm vi mà giao dịch này đang truy vấn.
- **Đặc điểm:** Hệ thống hoạt động như thể các giao dịch đang xếp hàng chạy từng cái một (tuần tự). Ngăn chặn được toàn bộ 100% các lỗi sai lệch dữ liệu.
- _Đánh đổi:_ Dễ gây ra hiện tượng nghẽn mạch (Blocking) và tắc nghẽn vô phương cứu chữa (Deadlock). Rất chậm, chỉ dùng trong các giao dịch tài chính/kế toán cực kỳ nhạy cảm.

**5. SNAPSHOT (Chụp ảnh trạng thái)**

- SQL Server sẽ "chụp" lại một bản sao của dữ liệu gốc và lưu vào bộ nhớ tạm (`tempdb`) ngay trước khi giao dịch bắt đầu.
- Thay vì khóa lẫn nhau, người đọc cứ đọc trên bản chụp cũ, người sửa cứ sửa trên bản chính.
- **Đặc điểm:** Tránh được cả 3 hiện tượng sai lệch, đồng thời không gây hiện tượng đọc chờ ghi (Blocking) như _Serializable_. Rất tối ưu nhưng tốn cực nhiều dung lượng ổ đĩa cho bộ nhớ đệm `tempdb`.

---

### III. BẢNG TỔNG HỢP SO SÁNH

| Mức độ Isolation (Từ thấp lên cao) | Ngăn Dirty Read | Ngăn Non-Repeatable Read | Ngăn Phantom Read |
| ---------------------------------- | --------------- | ------------------------ | ----------------- |
| **READ UNCOMMITTED**               | Không           | Không                    | Không             |
| **READ COMMITTED (Mặc định)**      | **Có**          | Không                    | Không             |
| **REPEATABLE READ**                | **Có**          | **Có**                   | Không             |
| **SNAPSHOT**                       | **Có**          | **Có**                   | **Có**            |
| **SERIALIZABLE**                   | **Có**          | **Có**                   | **Có**            |

---

### IV. CÚ PHÁP LẬP TRÌNH (T-SQL)

Trong Stored Procedure hoặc khối lệnh, bạn có thể thiết lập mức Isolation trước khi bắt đầu giao dịch bằng lệnh sau:

```sql
-- Cú pháp thiết lập mức cô lập cho phiên làm việc hiện tại
SET TRANSACTION ISOLATION LEVEL
    { READ UNCOMMITTED
    | READ COMMITTED
    | REPEATABLE READ
    | SNAPSHOT
    | SERIALIZABLE }
GO

-- Ví dụ ứng dụng trong Giao dịch (Transaction)
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRAN;
    SELECT * FROM TaiKhoan WHERE SoTaiKhoan = '12345';
    -- [Xử lý logic, không ai được phép thay đổi hay chèn dòng mới vào bảng TaiKhoan lúc này]
    UPDATE TaiKhoan SET SoDu = SoDu - 100000 WHERE SoTaiKhoan = '12345';
COMMIT TRAN;

```

---

### 📌 CÁC LƯU Ý QUAN TRỌNG (HỎI ĐÁP THỰC TẾ)

#### 1. Tại Sao Lại Đọc Được Dữ Liệu Chưa Commit ở Mức `READ UNCOMMITTED`? (Dưới Góc Độ Vật Lý)
*   **Buffer Pool (RAM):** Khi Giao dịch A chạy lệnh `UPDATE`, SQL Server không ghi ngay xuống đĩa cứng (file `.mdf`) mà nạp trang dữ liệu lên **RAM (Buffer Pool)** và sửa trực tiếp trên đó, tạo ra các **Dirty Page (Trang bẩn)**.
*   **Write-Ahead Logging (WAL):** Thay đổi này đồng thời được ghi tuần tự vào nhật ký giao dịch (file `.ldf` trên đĩa) để đảm bảo an toàn nếu mất điện.
*   **Không dùng "Góc riêng tư":** Để tối ưu hiệu năng và tiết kiệm RAM, SQL Server cho phép tất cả các Transaction dùng chung một bộ đệm Buffer Pool chứ không nhân bản dữ liệu ra từng góc riêng cho từng transaction. 
*   **Vượt rào khóa:** SQL Server mặc định dùng cơ chế **Khóa (Locking)** để giấu dữ liệu chưa commit (cắm biển khóa Exclusive Lock không cho ai đọc). Nhưng nếu Giao dịch B dùng mức `READ UNCOMMITTED` hoặc hint `NOLOCK`, Giao dịch B sẽ không xin khóa chia sẻ (S-Lock) $\rightarrow$ lách qua biển cấm để đọc thẳng giá trị thô đang có trên RAM.

#### 2. Tại Sao Phải `SELECT` Trước Khi `UPDATE`?
Trong các giao dịch như rút tiền, nhập điểm, chuyển khoản, ta luôn thấy code chạy `SELECT` trước rồi mới `UPDATE` vì:
*   **Kiểm tra điều kiện nghiệp vụ (Validation):** Cần đọc số dư lên để check xem có đủ tiền trừ không (`SoDu >= X`), tài khoản có bị khóa hay không. Nếu không kiểm tra mà chạy `UPDATE` ngay, số dư tài khoản có thể bị âm hoặc bị sửa trái phép.
*   **Giữ tài nguyên từ sớm (Concurrency Control):** Ở mức cô lập cao, câu lệnh `SELECT` đầu tiên sẽ đặt khóa đọc (S-Lock/Range Lock) trên dòng dữ liệu đó và giữ chặt đến cuối transaction. Nó đảm bảo trong suốt quá trình xử lý logic ở giữa, **không ai được phép xen vào sửa dòng này**.

#### 3. Cơ Chế Khóa của `READ COMMITTED` Khác Gì `REPEATABLE READ`?
*   **`READ COMMITTED`:** Khi đọc một dòng, SQL Server xin khóa đọc S-Lock. Nhưng **ngay sau khi đọc xong dòng đó, nó nhả khóa ra luôn**. Do đó, ở khoảng giữa (trước khi giao dịch kết thúc), giao dịch khác hoàn toàn có thể nhảy vào `UPDATE` hoặc `DELETE` dòng này $\rightarrow$ Gây lỗi *Non-repeatable Read*.
*   **`REPEATABLE READ`:** Khi đọc một dòng, nó xin khóa S-Lock và **ôm chặt khóa này cho đến khi giao dịch kết thúc hoàn toàn (COMMIT/ROLLBACK)**. Giao dịch khác muốn `UPDATE`/`DELETE` ở giữa sẽ bị block (chờ đợi) $\rightarrow$ Ngăn chặn hoàn toàn lỗi *Non-repeatable Read*.

