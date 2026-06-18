**CHƯƠNG 1. TỔNG QUAN VỀ SQL SERVER**

Mục đích: Biết được các thành phần thiết yếu của SQL SERVER.

### I. KIẾN TRÚC MẠNG CỦA SQL SERVER

- Kiến trúc của SQL SERVER phân chia các ứng dụng truy xuất cơ sở dữ liệu qua bộ điều khiển cơ sở dữ liệu (database engine).

- SQL SERVER chạy trên hệ điều hành NT cho phép kết nối đến nhiều hệ thống client qua mạng LAN hay Ethernet.

- Hệ thống client thông thường là các PCs chạy trên phần mềm client của SQL SERVER.

- Bộ điều khiển cơ sở dữ liệu SQL SERVER chạy trên WINDOWS NT hay WINDOWS 9x.

- Các user truy xuất cơ sở dữ liệu của SQL SERVER thông qua hệ thống client của nó. Thành phần database server chỉ chạy trên hệ thống Server của SQL SERVER.

- SQL SERVER sử dụng các mạng phổ biến như là Ethernet và Token Ring. Nó cũng sử dụng các giao thức phổ biến: TCP/IP, Named Pipe, IPX/SPX, Apple’s AppleTalk.

- Một trong những thuận lợi chính của SQL SERVER là nó có thể hợp nhất với các công cụ phát triển client/server và các ứng dụng như Excel, và Access.

- Cơ sở dữ liệu của SQL SERVER cũng có thể được truy xuất qua các ứng dụng: Visual Basic, Visual Foxpro, Visual C++, C#, Delphi, PowerBuilder… và với các bộ điều khiển như MicroSoft Jet Engine, Data Access Objects (DAO), Remote Data Objects (RDO), Activex Data Objects (ADO), ODBC, thư viện có sẵn của SQL SERVER, ….

- Trong kiến trúc client/server, các ứng dụng client sử dụng các giao tiếp lập trình ứng dụng để truy xuất dữ liệu mà SQL SERVER cung cấp (API- Application Programming Interface). SQL SERVER có 3 API truy xuất dữ liệu chính: OLE DB, ODBC, và DB Library.

- **Trình tự liên lạc giữa SQL Server và Client:**
- Ứng dụng client gọi OLE DB, ODBC, thư viện DB, hay API chứa SQL.

- Hành động này khởi tạo bộ cung cấp OLE DB, ODBC driver, hay DB-Library DLL.

- Các bộ phận cung cấp này gọi một thư viện mạng client, và thư viện này gọi tới một IPC API (phương thức liên lạc bên trong mạng).

- Lời gọi được truyền đến một thư viện mạng server bằng một IPC nằm bên dưới (IPC cục bộ hoặc IPC mạng).

- Thư viện mạng server nhận gói dữ liệu, trao cho các dịch vụ mở dữ liệu (ODS) và chuyển những yêu cầu của client đến CSDL SQL Server. Bản thân SQL SERVER là 1 ứng dụng ODS, nó xử lý và trả kết quả về cho ODS. Quá trình trả lời cho client diễn ra theo chiều ngược lại.

### II. CÁC DỊCH VỤ TRONG SQL SERVER

1.  **Database Engine:** Chịu trách nhiệm lưu trữ, bảo mật dữ liệu và xử lý giao dịch nhanh chóng.

2.  **Dịch vụ tìm kiếm (Full text Search Service):** Dịch vụ tìm kiếm Microsoft là một bộ máy tìm kiếm với chỉ mục bằng văn bản (full-text). Kỹ thuật này có khả năng tăng thêm chỉ mục, giữ từ cần tìm kiếm, không giới hạn chiều dài/dạng chuỗi và có khả năng tìm một ký tự, một từ, hoặc một chuỗi.

3.  **Dịch vụ SQL Server (SQL Server Service):** \* Quản lý tất cả các file cơ sở dữ liệu. Thi hành tất cả các phát biểu SQL và cấp phát tài nguyên hệ thống. Hoạt động như một dịch vụ trên Hệ điều hành Windows.

- Khi nhiều Server của SQL Server chạy trên cùng một máy tính, mỗi Server có dịch vụ riêng.

- Xử lý tất cả các câu lệnh giao tác từ client và server, chỉ ra vị trí các tài nguyên, áp đặt các luật hoạt động (business rules) để bảo đảm tính nhất quán của dữ liệu và ngăn chặn xung đột (ví dụ: hai người cùng cập nhật một dữ liệu).

- Hỗ trợ 3 loại giao thức kết nối: Shared Memory (cùng máy), TCP/IP (mạng LAN, WAN), và Named Pipes (mạng LAN).

4.  **Dịch vụ SQL Server Agent:** Hỗ trợ lên kế hoạch sẵn cho các hoạt động theo chu kỳ và thông báo các vấn đề xảy ra. Cần thiết trong lệnh nhân bản database (Replication). Bao gồm:

- _Jobs:_ Định nghĩa đối tượng gồm các bước thực thi (câu lệnh SQL) được lên kế hoạch tại một thời điểm chỉ định trước.

- _Alerts:_ Đưa ra cảnh báo khi có sự kiện (lỗi, hết bộ nhớ...) và có thể thực hiện hành động giải quyết như gởi email.

5.  **MSDTC (MicroSoft Distributed Transaction Coordinator):** Quản lý và điều phối các giao tác của cơ sở dữ liệu trên nhiều server. Theo dõi việc chuyển các giao tác phân tán an toàn qua các server tham gia.

### III. NHIỀU SERVER CỦA SERVER SQL TRÊN CÙNG MỘT MÁY TÍNH (Multiple Instances of SQL Server)

- **Khái niệm:** SQL Server hỗ trợ nhiều Server chạy đồng thời trên cùng một máy tính. Mỗi Server có cơ sở dữ liệu hệ thống riêng và không chia sẻ cơ sở dữ liệu người dùng.

- **Có hai loại Server SQL Server:**
- _Server mặc định (Default Instance):_ Hoạt động giống bộ xử lý CSDL của các phiên bản trước. Được nhận diện bằng tên của máy tính.

- _Các Server đặt tên (Named Instance):_ Được nhận diện bằng tên của chúng với định dạng: `computer_name \ instance_name`. Có thể có nhiều Server đặt tên và mỗi Server có tên duy nhất.

- **Làm việc với nhiều Server:** Bạn có thể đăng ký mỗi Server thông qua SQL Server Management Studio. Biến hệ thống `@@SERVERNAME` dùng để nhận diện tên của Server.

### IV. TRANSACT-SQL (T-SQL)

- Trong SQL SERVER, SQL được gọi là Transact-SQL, tuân thủ các cú pháp của SQL chuẩn và cung cấp một số option mở rộng. Thường dùng để quản trị CSDL, tạo/xóa bảng, viết thủ tục và thay đổi cấu hình Server.

- **Transact-SQL có 3 loại:**
- _Data Definition Language (DDL):_ create database, create table, create view….

- _Data Manipulation Language (DML):_ select, update, delete, insert into, merge.

- _Data Control Language (DCL):_ dùng để điều khiển quyền truy xuất qua các lệnh GRANT và REVOKE.

### V. KIẾN TRÚC CƠ SỞ DỮ LIỆU TRONG SQL SERVER

1.  **Server:** Bộ điều khiển cơ sở dữ liệu, có nhiệm vụ xử lý yêu cầu và trả kết quả.

2.  **Cơ sở dữ liệu (Database):** Mỗi SQL SERVER chứa nhiều CSDL. Có 2 loại: SystemDatabase và User database. Có 4 cơ sở dữ liệu hệ thống mặc định:

- _master:_ Ghi thông tin mức hệ thống (login accounts, lựa chọn cấu hình, thông tin khởi tạo, sự tồn tại của các CSDL khác). _Lưu ý: Không được thay đổi master database và nên thường xuyên backup_.

- _model:_ Là template để SQL SERVER dùng hỗ trợ tạo CSDL mới. Thay đổi model database sẽ ảnh hưởng tới tất cả các CSDL được tạo mới sau đó.

- _msdb:_ Dùng bởi SQL Server Agent để lập lịch alert, job và ghi toán tử, ví dụ như thông tin các tập sao lưu.

- _tempdb:_ Lưu trữ table tạm. Được tạo lại mỗi lần SQL SERVER khởi động, tự động xóa table tạm khi user thoát.

3.  **Các đối tượng cơ sở dữ liệu:** Gồm lưu trữ dữ liệu, ràng buộc và xử lý dữ liệu.

- a. Table: Thành phần lưu trữ dữ liệu chính, biểu diễn một loại đối tượng có ý nghĩa với user. Có 2 thành phần chính là Cột và Hàng. Có 3 loại table:

- _Table hệ thống:_ Chứa dữ liệu định nghĩa cấu hình (bắt đầu bởi sys), user không thể cập nhật trực tiếp.

- _Table tạm:_ Được lưu trữ trong tempdb, sẽ tự động hủy khi ngắt kết nối. Gồm Table tạm cục bộ (bắt đầu bằng `#`, chỉ hữu hình với kết nối tạo ra nó) và Table tạm toàn cục (bắt đầu bằng `##`, hữu hình với mọi kết nối).

- _Table của user:_ Nơi user thao tác thông qua lệnh DML.

- b. Column: Mỗi cột có tên và kiểu dữ liệu tương ứng. SQL Server hỗ trợ nhiều kiểu dữ liệu như int, char, datetime, money.... Có kiểu dữ liệu do user định nghĩa và giá trị `NULL` (biểu diễn một giá trị chưa biết).

- c. Index: Tăng tốc độ truy xuất dữ liệu. Gồm:

- _Cluster Index:_ Mặc định theo khóa chính, dữ liệu được lưu trữ theo thứ tự index (chỉ có duy nhất 1).

- _Noncluster Index:_ Không đổi cách dữ liệu lưu trữ, dùng con trỏ chỉ tới dữ liệu (tối đa 1024 index).

- d. View: Là một Table ảo hoặc một truy vấn được lưu trữ, không chứa tham số từ user truyền vào. Dùng để giới hạn quyền đọc các hàng/cột, nhóm các cột, thống kê thông tin....

- e. Constraint (Ràng buộc): Đảm bảo tính toàn vẹn dữ liệu. Có 5 loại: Primary Key, Foreign key, Unique key, Check, và Not Null.

- f. Rule: Giới hạn giá trị đưa vào field (có thể qua biểu thức điều kiện hoặc danh sách).

- g. Default: Gán giá trị ngầm định cho field khi chưa có dữ liệu.

- h. Thủ tục (Stored Procedure): Nhóm các phát biểu Transact-SQL được compile thành chương trình con, dùng để quản trị hoặc thao tác dữ liệu tự động. Có thể nhận tối đa 1024 tham số.

- i. Trigger: Thủ tục tự động thực hiện khi thay đổi dữ liệu qua lệnh Update, Insert, Delete, thường để kiểm tra ràng buộc toàn vẹn.

- j. Hàm do người dùng định nghĩa (UDF): Có các hàm cài đặt sẵn (Built-in) và hàm do người dùng tự định nghĩa (cho phép định nghĩa bằng CREATE FUNCTION). Có thể nhận/không nhận tham số và trả về giá trị đơn giản hoặc tập records.
