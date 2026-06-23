**CHUYÊN ĐỀ: TỔNG QUAN VỀ TRIGGER, STORED PROCEDURE VÀ INDEX**

Mục đích: Giúp lập trình viên hiểu rõ bản chất, cách thức vận hành và sự đánh đổi (Ưu & Nhược điểm) của 5 đối tượng cốt lõi trong SQL Server: **Trigger**, **Stored Procedure**, **Index**, **View**, và **User-Defined Function (UDF)**. Từ đó đưa ra các quyết định thiết kế cơ sở dữ liệu tối ưu nhất cho hệ thống.

---

## I. TRIGGER (TRÌNH KÍCH HOẠT)

### 1. Khái niệm
**Trigger** là một loại Stored Procedure đặc biệt không có tham số đầu vào. Nó không thể được gọi thực thi trực tiếp bằng tay, mà sẽ **tự động kích hoạt (fire)** khi có một sự kiện thay đổi dữ liệu xảy ra trên bảng (hoặc view).

*   **Các loại sự kiện:** `INSERT` (Thêm), `UPDATE` (Sửa), `DELETE` (Xóa).
*   **Hai bảng tạm hệ thống cốt lõi trong Trigger:**
    *   **`inserted`:** Chứa dữ liệu mới vừa được thêm vào (`INSERT`) hoặc dữ liệu mới sau khi sửa (`UPDATE`).
    *   **`deleted`:** Chứa dữ liệu cũ vừa bị xóa (`DELETE`) hoặc dữ liệu cũ trước khi sửa (`UPDATE`).

### 2. Phân loại chính (DML Triggers)
*   **`AFTER` Trigger (Mặc định):** Kích hoạt **sau khi** hành động thay đổi dữ liệu đã hoàn tất và vượt qua hết các ràng buộc (Constraints) như Khóa chính, Khóa ngoại.
*   **`INSTEAD OF` Trigger:** Kích hoạt **thay thế cho** hành động gốc. Ví dụ: Khi chạy lệnh `DELETE` trên bảng, trigger này sẽ chặn lệnh delete lại và thực thi các câu lệnh SQL khác bên trong nó (thường dùng để làm "xóa mềm" - cập nhật cột `IsDeleted = 1` thay vì xóa thực tế).

### 3. Ưu và Nhược điểm

#### 👍 Ưu điểm:
*   **Duy trì toàn vẹn dữ liệu phức tạp:** Ràng buộc được những logic nghiệp vụ chéo cực kỳ phức tạp giữa nhiều bảng mà các Constraint thông thường (`FOREIGN KEY`, `CHECK`) không làm được.
*   **Tự động hóa hành động:** Rất phù hợp để làm hệ thống **Audit Log** (ghi lại nhật ký lịch sử: Ai đã sửa dòng này, sửa từ giá trị cũ nào sang giá trị mới nào).
*   **Đồng bộ số liệu tự động:** Tự động tính toán lại các cột tổng hợp (ví dụ: khi thêm một hóa đơn chi tiết, trigger tự cộng dồn tiền vào bảng hóa đơn tổng).

#### 👎 Nhược điểm:
*   **Ẩn và khó kiểm soát (Invisible/Hidden):** Trigger chạy ngầm dưới database. Lập trình viên mới vào dự án nếu đọc code ứng dụng (C# / Java) sẽ không hiểu tại sao dữ liệu tự động bị thay đổi, gây khó khăn cho việc gỡ lỗi (debug) và bảo trì.
*   **Ảnh hưởng hiệu năng hệ thống:** Trigger chạy trong cùng một giao dịch (Transaction) của câu lệnh gốc. Nếu viết code trigger không tối ưu (như dùng Cursor, vòng lặp nặng), thời gian khóa dữ liệu sẽ kéo dài, gây tắc nghẽn (`Blocking`) và treo hệ thống cho người dùng khác.
*   **Rủi ro kích hoạt đệ quy (Recursive Trigger):** Nếu thiết kế không cẩn thận, Trigger trên bảng A cập nhật bảng B, Trigger bảng B lại quay lại cập nhật bảng A $\rightarrow$ tạo ra vòng lặp vô hạn làm sập server.

---

## II. STORED PROCEDURE (THỦ TỤC LƯU TRỮ - SP)

### 1. Khái niệm
**Stored Procedure (SP)** là một tập hợp các câu lệnh T-SQL được biên dịch sẵn (Pre-compiled) và lưu trữ trực tiếp bên trong cơ sở dữ liệu. SP có thể nhận tham số truyền vào (`INPUT`) và trả về kết quả qua tham số đầu ra (`OUTPUT`) hoặc dưới dạng một bảng kết quả.

### 2. Ưu và Nhược điểm

#### 👍 Ưu điểm:
*   **Hiệu năng vượt trội (Performance):** SQL Server chỉ cần biên dịch và tối ưu hóa SP một lần duy nhất ở lần chạy đầu tiên, sau đó lưu kế hoạch thực thi (**Execution Plan**) vào bộ nhớ đệm (Plan Cache). Các lần gọi sau chỉ cần lấy plan ra chạy ngay, nhanh hơn nhiều so với việc gửi câu lệnh SQL thô từ ứng dụng xuống rồi phải parse lại từ đầu.
*   **Bảo mật dữ liệu tối đa:** 
    *   **Chống SQL Injection:** SP sử dụng cơ chế tham số hóa (Parameterized Queries) giúp loại bỏ hoàn toàn các cuộc tấn công tiêm mã độc.
    *   **Ẩn cấu trúc bảng:** Có thể phân quyền cho User chỉ được phép thực thi (`EXECUTE`) SP mà không cần cấp quyền đọc/ghi trực tiếp trên các bảng gốc.
*   **Tiết kiệm băng thông:** Thay vì gửi cả đoạn mã SQL dài hàng trăm dòng qua mạng, ứng dụng chỉ cần gửi một chuỗi ngắn: `EXEC sp_CapNhatDiem @MaSV, @Diem`.
*   **Dễ bảo trì và nâng cấp:** Khi logic nghiệp vụ thay đổi, DBA chỉ cần vào sửa code bên trong SP mà không cần phải sửa code của ứng dụng Backend (C# / Java) và build/deploy lại phần mềm.

#### 👎 Nhược điểm:
*   **Gây quá tải cho Database Server:** SP đẩy toàn bộ logic tính toán xuống cho Database gánh. Trong kiến trúc hệ thống, Database Server thường là nơi khó nâng cấp mở rộng ngang (Horizontal Scaling) nhất so với Web Server.
*   **Ngôn ngữ lập trình hạn chế:** T-SQL chỉ mạnh về truy vấn dữ liệu, rất khó để viết các logic nghiệp vụ phức tạp, xử lý chuỗi hoặc kết nối bên ngoài so với các ngôn ngữ lập trình hiện đại như C#, Java, Python.
*   **Khó quản lý phiên bản (Version Control):** Việc quản lý các file SP trực tiếp trên database bằng Git khó khăn và thủ công hơn so với quản lý code ứng dụng thông thường.

---

## III. INDEX (CHỈ MỤC)

### 1. Khái niệm
**Index** là một cấu trúc dữ liệu vật lý (thường ở dạng cây cân bằng B-Tree) được tạo ra trên bảng để giúp SQL Server nhanh chóng tìm thấy các dòng dữ liệu cụ thể mà không cần phải quét qua toàn bộ bảng (Table Scan).

### 2. Phân loại
*   **Clustered Index (Chỉ mục cụm):** Quyết định trực tiếp thứ tự sắp xếp vật lý của dữ liệu trên ổ đĩa. Một bảng chỉ có duy nhất **1 Clustered Index** (mặc định tự tạo khi khai báo Khóa chính - Primary Key).
*   **Non-Clustered Index (Chỉ mục phi cụm):** Giống như trang mục lục cuối cuốn sách. Nó chứa giá trị của cột được lập chỉ mục và một con trỏ (Bookmark/RID) chỉ tới vị trí thực tế của dòng dữ liệu. Một bảng có thể tạo **nhiều Non-Clustered Index** trên các cột hay tìm kiếm.

### 3. Ưu và Nhược điểm

#### 👍 Ưu điểm:
*   **Tăng tốc độ truy vấn SELECT vượt bậc:** Giúp SQL Server chuyển đổi từ việc quét toàn bảng (`Scan`) sang tìm kiếm trực tiếp trên cây chỉ mục (`Seek`), giúp tốc độ truy vấn tăng từ hàng chục đến hàng ngàn lần đối với các câu lệnh có chứa `WHERE`, `JOIN`, `ORDER BY`, hoặc `GROUP BY`.
*   **Tối ưu hóa tài nguyên:** Giảm thiểu việc sử dụng CPU và I/O của đĩa cứng vì số lượng trang dữ liệu phải nạp lên RAM ít hơn rất nhiều.

#### 👎 Nhược điểm:
*   **Làm chậm thao tác Ghi dữ liệu (`INSERT`, `UPDATE`, `DELETE`):** Khi bạn thêm mới hoặc sửa dữ liệu, SQL Server không chỉ thay đổi dữ liệu trong bảng chính mà còn phải **tự động sắp xếp lại cây B-Tree của tất cả các Index** liên quan. Bảng càng nhiều Index thì ghi dữ liệu càng chậm.
*   **Tốn dung lượng lưu trữ:** Mỗi Index là một cấu trúc vật lý riêng biệt nên nó tốn không gian bộ nhớ đĩa cứng để lưu trữ. Trong các bảng lớn, dung lượng file Index có thể lớn hơn cả dung lượng file dữ liệu gốc.
*   **Hiện tượng phân mảnh (Fragmentation):** Qua thời gian ghi/xóa dữ liệu, các Index sẽ bị phân mảnh, đòi hỏi phải có kế hoạch bảo trì định kỳ (Rebuild/Reorganize Index) để giữ vững tốc độ.

---

## IV. VIEW (KHUNG NHÌN)

### 1. Khái niệm
**View** là một bảng ảo (Virtual Table) được định nghĩa bằng một câu truy vấn `SELECT`. View không tự lưu trữ dữ liệu thực tế trên đĩa cứng (ngoại trừ *Indexed View*), mà mỗi khi View được gọi, SQL Server sẽ thực thi câu lệnh truy vấn bên dưới nó để lấy dữ liệu từ các bảng gốc liên quan.

*   **Đặc điểm:** View không nhận bất kỳ tham số đầu vào nào từ người dùng.

### 2. Phân loại
*   **Standard View (View thông thường):** Bảng ảo bình thường, dữ liệu luôn được lấy động từ các bảng gốc.
*   **Indexed View (View được lập chỉ mục / Materialized View):** Là loại View đặc biệt được tạo chỉ mục vật lý và lưu dữ liệu trực tiếp trên đĩa cứng. Rất hữu ích cho các báo cáo thống kê phức tạp cần tính toán trước các phép tính khổng lồ.

### 3. Ưu và Nhược điểm

#### 👍 Ưu điểm:
*   **Đơn giản hóa câu lệnh truy vấn:** Ẩn đi các phép nối `JOIN` phức tạp, các phép tính toán lằng nhằng. Người dùng chỉ cần truy vấn đơn giản: `SELECT * FROM View_DiemSinhVien`.
*   **Bảo mật dữ liệu (Security):** Giới hạn quyền truy cập của người dùng. Bạn có thể cho phép họ truy vấn qua View để che đi các cột nhạy cảm (như Mật khẩu, Số dư) hoặc các dòng dữ liệu mật của bảng gốc.
*   **Độc lập dữ liệu logic (Data Independence):** Khi bạn thay đổi cấu trúc bảng gốc bên dưới (ví dụ đổi tên cột), bạn chỉ cần cập nhật lại định nghĩa View mà không làm vỡ các ứng dụng/phần mềm bên ngoài đang gọi View.

#### 👎 Nhược điểm:
*   **Hiệu năng giảm sút (Performance Overhead):** Với View thông thường, mỗi lần bạn `SELECT` từ View, SQL Server phải thực thi câu lệnh SQL gốc lồng bên dưới. Nếu thiết kế lồng View nhiều tầng (View gọi View khác), truy vấn sẽ chạy rất chậm.
*   **Hạn chế cập nhật dữ liệu (DML):** Việc chạy các lệnh `INSERT`, `UPDATE`, `DELETE` qua View bị giới hạn cực kỳ khắt khe (ví dụ: chỉ được sửa đổi trên một bảng gốc duy nhất tại một thời điểm, View không được chứa mệnh đề `GROUP BY`, `DISTINCT`, các hàm tổng hợp...).

---

## V. USER-DEFINED FUNCTION (HÀM TỰ ĐỊNH NGHĨA - UDF)

### 1. Khái niệm
**User-Defined Function (UDF)** là một đối tượng nhận các tham số đầu vào, thực hiện các tính toán logic và trả về một kết quả cụ thể. Khác với Stored Procedure, UDF có thể được nhúng trực tiếp vào các mệnh đề của câu lệnh SQL như `SELECT`, `WHERE`, hoặc `JOIN`.

### 2. Phân loại
*   **Scalar Function (Hàm vô hướng):** Trả về **duy nhất 1 giá trị đơn lẻ** (như số, chuỗi, ngày tháng). Ví dụ: Truyền vào `NgaySinh` trả về số tuổi.
*   **Table-Valued Function (TVF - Hàm trả về bảng):** Trả về kết quả dưới dạng một **Bảng dữ liệu**.
    *   *Inline TVF:* Giống như một View có tham số. Viết bằng đúng một câu lệnh `SELECT` duy nhất. Hiệu năng cực tốt vì SQL Server có thể gộp nó vào Execution Plan của câu truy vấn cha.
    *   *Multi-Statement TVF:* Viết bằng khối lệnh phức tạp, phải khai báo một bảng tạm trước rồi chèn dữ liệu vào bảng tạm đó trước khi trả về. Chậm hơn Inline TVF.

### 3. Ưu và Nhược điểm

#### 👍 Ưu điểm:
*   **Tái sử dụng code cực cao:** Bạn có thể viết các hàm định dạng ngày tháng, chuẩn hóa chuỗi tên sinh viên viết hoa viết thường, tính điểm trung bình... một lần và dùng ở mọi câu truy vấn SELECT.
*   **Linh hoạt hơn View:** UDF cho phép truyền các tham số đầu vào để lọc dữ liệu động (View không thể nhận tham số).
*   **Tăng tính mô-đun hóa:** Giúp tách các logic tính toán rời rạc ra khỏi câu lệnh SELECT chính, làm code SQL sạch sẽ và dễ đọc hơn.

#### 👎 Nhược điểm:
*   **Kẻ hủy diệt hiệu năng - Scalar UDF (Lỗi chạy từng dòng - RBAR):** Khi bạn dùng Scalar Function trong mệnh đề `SELECT` trên một bảng có 1 triệu dòng, SQL Server bắt buộc phải **gọi chạy hàm đó 1 triệu lần tuần tự** cho từng dòng. Điều này phá vỡ cơ chế xử lý tập hợp và làm câu lệnh chạy cực kỳ chậm.
*   **Hạn chế thay đổi dữ liệu:** Bên trong hàm UDF, bạn **tuyệt đối không được phép** thực thi các câu lệnh làm thay đổi dữ liệu bảng hệ thống (không chạy được `INSERT`, `UPDATE`, `DELETE` trên bảng thật, chỉ được phép ghi bảng tạm hoặc bảng trả về).
*   **Không gọi được Stored Procedure:** UDF không thể gọi thực thi một Stored Procedure bên trong nó.

---

## VI. BẢNG TỔNG HỢP SO SÁNH & LỜI KHUYÊN SỬ DỤNG

| Đối tượng | Chức năng chính | Có nhận tham số? | Có làm thay đổi dữ liệu? (DML) | Khi nào nên dùng? | Sai lầm cần tránh |
| :--- | :--- | :---: | :---: | :--- | :--- |
| **Trigger** | Giám sát & tự động đồng bộ | Không | Có | Làm hệ thống Audit Log ghi lịch sử, đồng bộ số liệu tự động. | Tránh viết logic quá nặng, dùng vòng lặp/cursor gây deadlock. |
| **Stored Procedure** | Đóng gói logic nghiệp vụ | Có | Có | Viết API giao tiếp Backend, thực hiện tác vụ CUD dữ liệu cần bảo mật và hiệu năng. | Tránh biến SP thành nơi xử lý logic của ứng dụng. |
| **Index** | Tăng tốc độ tìm kiếm | Không | Không | Tạo trên cột thường `WHERE`, `JOIN`, `ORDER BY`. | Tránh tạo quá nhiều Index trên bảng có tần suất ghi liên tục. |
| **View** | Khung nhìn ảo bảo mật, đơn giản | Không | Hạn chế | Che giấu cột nhạy cảm, đơn giản hóa câu lệnh `JOIN` dài dòng. | Tránh thiết kế lồng View nhiều tầng làm giảm hiệu năng nghiêm trọng. |
| **UDF** | Tính toán & xử lý giá trị/bảng | Có | Không | Viết các hàm tiện ích xử lý chuỗi, định dạng, hoặc Inline TVF thay thế View cần tham số. | **Tránh lạm dụng Scalar UDF** trong câu lệnh SELECT danh sách dữ liệu lớn. |

