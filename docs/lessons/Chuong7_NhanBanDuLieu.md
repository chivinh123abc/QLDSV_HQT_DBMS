**CHƯƠNG 7. NHÂN BẢN DỮ LIỆU**

### I. KHÁI NIỆM (CONCEPTS)

- Nhân bản dữ liệu cho phép ta phân bố các bản sao dữ liệu từ một nguồn (source) đến các hệ thống đích (target) một cách tự động.

- Nhân bản dữ liệu trong SQL Server hoạt động dựa trên mô hình push-pull (đẩy - kéo).

- Trong mô hình này, tiến trình nhân bản sẽ được thực hiện theo hai cách:
- Server nguồn chủ động đẩy dữ liệu được nhân bản đến server đích.

- Hoặc Server đích tự kéo dữ liệu từ server nguồn về.

- Các thuật ngữ định danh máy chủ:

- **Publisher:** Server nguồn (nơi cung cấp và xuất bản dữ liệu).

- **Subscriber:** Server đích (nơi nhận và đăng ký dữ liệu).

- Khi nhân bản dữ liệu, người quản trị phải cân nhắc chọn kiểu nhân bản dựa vào 2 yếu tố chính: dữ liệu tự động đồng bộ giữa các site (đảm bảo tính nhất quán trong giao tác) hoặc dữ liệu chỉ đồng bộ theo nhu cầu của người dùng (có tính tự quản).

### II. CÁC THÀNH PHẦN CƠ BẢN TRONG REPLICATION

- **Article (Bài báo/Thành phần):** Là một đơn vị dữ liệu cơ sở. Article tiêu biểu cho đối tượng dữ liệu được nhân bản. Một article có thể là dữ liệu từ một table hay một đối tượng cơ sở dữ liệu (toàn bộ table, stored procedure, view, UDF). Tất cả các articles đều bắt buộc phải thuộc về một Publication.

- **Publication (Ấn bản):** Đại diện cho một nhóm tập hợp các article.

- **Distributor (Nhà phân phối):** Là hệ thống SQL Server có trách nhiệm chuyển dữ liệu được nhân bản giữa Publisher và Subscriber. Đối với lượng dữ liệu nhân bản nhỏ, Distributor và Publisher luôn thuộc về một hệ thống máy chủ; trong trường hợp ngược lại, Distributor và Publisher sẽ thuộc hai Server khác nhau nhằm tối ưu hiệu suất.

### III. CÁC HÌNH THỨC NHÂN BẢN (REPLICATION TYPES)

Dựa vào mức độ cập nhật và kiến trúc mạng, SQL Server cung cấp 3 loại nhân bản chính:

**1. Snapshot Replication (Nhân bản chụp nhanh)**

- Phù hợp áp dụng đối với những hệ thống không yêu cầu dữ liệu cập nhật mới nhất liên tục mà chỉ cần cập nhật dữ liệu đồng loạt vào cuối ngày.

**2. Transactional Replication (Nhân bản giao tác)**

- Dùng transaction log (nhật ký giao dịch) để nhân bản các giao tác cá nhân giữa Publisher và Subscriber.

- Transaction log của Publisher sẽ nắm bắt các thay đổi dữ liệu, sau đó áp dụng các thay đổi đó đến Subscriber theo đúng trình tự mà chúng được thực hiện.

- Với Transactional replication, sự chuyển dữ liệu giữa Publisher và Subscriber có thể xảy ra liên tục hoặc định kỳ trong một khoảng thời gian nhất định.

- _Ví dụ:_ Trong tình huống có 2 nơi cùng đặt hàng từ một cơ sở dữ liệu có chung kho hàng và cả 2 nơi đều cần cập nhật số liệu tồn kho mới nhất liên tục, ta sẽ sử dụng Transactional Replication.

**3. Merge Replication (Nhân bản trộn)**

- Dạng nhân bản này sẽ theo dõi các thay đổi trong cả 2 cơ sở dữ liệu nguồn và đích, từ đó thực hiện sự đồng bộ về dữ liệu giữa Subscriber và Publisher khi cơ sở dữ liệu được nhân bản.

- _Ví dụ:_ Áp dụng Merge replication, một chi nhánh công ty có thể thực hiện các chức năng bán hàng trong khi disconnect (ngắt kết nối) hoàn toàn với cơ sở dữ liệu đang đặt ở văn phòng trung tâm. Trong ngày, văn phòng chi nhánh dùng cơ sở dữ liệu cục bộ của mình để thực hiện các công việc. Vào cuối ngày, văn phòng chi nhánh sẽ kết nối lại với văn phòng trung tâm và "merge" (trộn) với cơ sở dữ liệu tại trung tâm về tình hình bán, đặt hàng, thông tin khách hàng mới đã phát sinh trong ngày. Tương tự, các thay đổi đã thực hiện đến khách hàng và tín dụng từ trung tâm sẽ được merge ngược về cơ sở dữ liệu tại văn phòng chi nhánh.

---

### GHI CHÚ TRÊN LỚP (Quicknote)

> Các ghi chú bổ sung từ bài giảng trên lớp.

**1. Yêu cầu phiên bản SQL Server:**
- **SQL Server Express** không hỗ trợ tính năng Replication (nhân bản dữ liệu).
- Phải sử dụng bản **Standard** hoặc **Enterprise** mới có thể cấu hình Publisher, Distributor và Subscriber.

**2. Điều kiện tiên quyết trước khi cấu hình Replication:**
- Đảm bảo **SQL Server Agent** đang chạy trên cả Publisher và Subscriber.
- Tài khoản SQL Server Agent phải có đủ quyền truy cập các database tham gia nhân bản.
- Firewall (tường lửa) phải cho phép kết nối giữa các server trên port TCP của SQL Server (mặc định: **1433**).
