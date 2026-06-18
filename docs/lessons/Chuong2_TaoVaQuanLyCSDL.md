**CHƯƠNG 2. TẠO VÀ QUẢN LÝ CƠ SỞ DỮ LIỆU**

Mục đích: Nắm vững kiến trúc vật lý của cơ sở dữ liệu (CSDL), cách lưu trữ dữ liệu, và các câu lệnh Transact-SQL (T-SQL) để tạo, chỉnh sửa, cũng như quản trị CSDL.

### I. CẤU TRÚC VẬT LÝ CỦA CƠ SỞ DỮ LIỆU (Physical Architecture)

Một CSDL trong SQL Server không chỉ là một khối duy nhất mà được ánh xạ tới hai hoặc nhiều tập tin vật lý trên đĩa cứng. Có 3 loại tập tin chính:

1. **Tập tin dữ liệu chính (Primary Data File - .mdf):**

- Mỗi CSDL có **duy nhất một** tập tin chính.
- Chứa các thông tin khởi động của CSDL, cấu hình hệ thống, và đóng vai trò lưu trữ dữ liệu cũng như con trỏ trỏ tới các tập tin khác trong CSDL.

2. **Tập tin dữ liệu phụ (Secondary Data File - .ndf):**

- Được dùng để lưu trữ phần dữ liệu không chứa vừa trong tập tin chính, hoặc để phân tán dữ liệu ra nhiều ổ đĩa nhằm tăng hiệu suất (I/O).
- Một CSDL có thể **không có, có một, hoặc có nhiều** tập tin phụ.

3. **Tập tin nhật ký giao dịch (Transaction Log File - .ldf):**

- Lưu trữ các thông tin giao dịch dùng để phục hồi CSDL (Database Recovery).
- Bất kỳ CSDL nào cũng phải có **ít nhất một** tập tin log. Tập tin này đóng vai trò quan trọng trong việc đảm bảo tính toàn vẹn và an toàn dữ liệu khi xảy ra sự cố.

### II. NHÓM TẬP TIN (Filegroups)

Filegroups được sử dụng để nhóm các tập tin dữ liệu (mdf và ndf) lại với nhau, giúp dễ dàng quản trị, sao lưu (backup) và tối ưu hóa việc phân bổ dữ liệu.

- **Primary Filegroup (Nhóm chính):** Nhóm mặc định chứa tập tin `.mdf` và các tập tin không được gán cụ thể vào nhóm nào. Tất cả các bảng hệ thống (System tables) luôn được lưu ở đây.
- **User-defined Filegroup (Nhóm tự định nghĩa):** Các nhóm do người dùng tự tạo ra để phân tách dữ liệu (ví dụ: chia bảng dữ liệu nhân sự vào nhóm 1, bảng doanh thu vào nhóm 2).
- **Default Filegroup (Nhóm mặc định):** Khi bạn tạo một đối tượng (như Table) mà không chỉ định nó nằm ở filegroup nào, nó sẽ tự động được đưa vào nhóm mặc định. Thường mặc định là Primary nhưng có thể thay đổi bằng T-SQL.
- **Lưu ý:** Tập tin nhật ký giao dịch (`.ldf`) **không bao giờ** thuộc về bất kỳ Filegroup nào. Nó được quản lý độc lập.

### III. QUẢN LÝ KHÔNG GIAN LƯU TRỮ (Space Management)

SQL Server quản lý không gian đĩa dựa trên 2 đơn vị cơ bản:

- **Pages:** Đơn vị lưu trữ nhỏ nhất. Mỗi Page có kích thước cố định là **8 KB**. Các hàng (rows) trong một bảng không thể vượt qua kích thước của 1 page (ngoại trừ kiểu dữ liệu lớn như varchar(max), text, image).
- **Extents:** Là tập hợp của **8 pages liên tiếp** (tương đương 64 KB). SQL Server cấp phát không gian cho các đối tượng theo từng Extent.
- _Mixed Extents (Extent hỗn hợp):_ 8 pages thuộc về các đối tượng khác nhau (dùng cho các bảng mới, kích thước nhỏ).
- _Uniform Extents (Extent đồng nhất):_ Cả 8 pages đều thuộc về cùng một đối tượng (dùng khi bảng đã phát triển đủ lớn).

### IV. NGÔN NGỮ ĐỊNH NGHĨA DỮ LIỆU (DDL) QUẢN TRỊ DATABASE

Bạn có thể thao tác với CSDL bằng giao diện (SQL Server Management Studio) hoặc bằng mã lệnh T-SQL.

**1. Tạo CSDL (CREATE DATABASE)**

- **Cú pháp đơn giản:** `CREATE DATABASE TenCoSoDuLieu;`
- **Cú pháp đầy đủ (chỉ định rõ thông số):** Cho phép bạn cài đặt vị trí lưu file (`FILENAME`), kích thước ban đầu (`SIZE`), mức độ tăng trưởng mỗi khi đầy (`FILEGROWTH`), và kích thước tối đa (`MAXSIZE`).

**2. Chỉnh sửa CSDL (ALTER DATABASE)**
Sử dụng để thay đổi cấu trúc CSDL sau khi đã tạo:

- `ADD FILE` / `ADD LOG FILE`: Thêm tập tin dữ liệu phụ hoặc tập tin log.
- `ADD FILEGROUP`: Thêm nhóm tập tin mới.
- `REMOVE FILE` / `REMOVE FILEGROUP`: Xóa tập tin hoặc nhóm tập tin (với điều kiện file/group đó phải rỗng).
- `MODIFY FILE`: Thay đổi các thuộc tính của tập tin như kích thước, mức độ tăng trưởng.
- `MODIFY NAME`: Đổi tên CSDL.

**3. Xóa CSDL (DROP DATABASE)**

- **Cú pháp:** `DROP DATABASE TenCoSoDuLieu;`
- Lệnh này sẽ xóa bỏ hoàn toàn cấu trúc CSDL và **xóa luôn các tập tin vật lý** (.mdf, .ldf) khỏi ổ đĩa cứng.

### V. CÁC THAO TÁC QUẢN TRỊ KHÁC

1. **Thu nhỏ CSDL (Shrink Database):** \* Trải qua thời gian sử dụng và xóa dữ liệu, CSDL có thể bị "rỗng" bên trong. Sử dụng lệnh `DBCC SHRINKDATABASE` hoặc `DBCC SHRINKFILE` để thu hồi không gian đĩa bị lãng phí trả lại cho hệ điều hành.
2. **Tách và Gắn CSDL (Detach & Attach):**

- **Detach (`sp_detach_db`):** Ngắt kết nối CSDL khỏi hệ thống SQL Server hiện tại một cách an toàn. Các tập tin vật lý vẫn giữ nguyên trên đĩa. Thường dùng khi muốn copy/di chuyển CSDL sang máy tính khác.
- **Attach:** Gắn các tập tin `.mdf` và `.ldf` từ ổ đĩa vào một SQL Server để nó nhận diện và hoạt động như một CSDL bình thường (sử dụng `CREATE DATABASE ... FOR ATTACH`).

3. **Các trạng thái của Database (Database States):**

- _ONLINE:_ Trạng thái bình thường, sẵn sàng truy xuất.
- _OFFLINE:_ CSDL không khả dụng, không thể truy xuất.
- _RESTORING:_ Đang trong quá trình phục hồi (Restore).
- _SUSPECT:_ Bị lỗi, SQL Server không thể đảm bảo tính toàn vẹn (thường do mất file log hoặc lỗi phần cứng).
