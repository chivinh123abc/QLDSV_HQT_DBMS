### I. MỤC LỤC CÁC CHƯƠNG LÝ THUYẾT

Bộ giáo trình SQL Server này được chia thành 8 chương chính:

- **Chương 1: Tổng quan về SQL Server**
  - Kiến trúc mạng của SQL Server.
  - Các dịch vụ trong SQL Server.
  - Cấu trúc CSDL và đối tượng CSDL cơ bản.

- **Chương 2: Hệ quản trị SQL Server**
  - Cài đặt và cấu hình hệ thống (SQL Server Management Studio).
  - Các kiểu dữ liệu (Data types) trong SQL Server.

- **Chương 3: Ngôn ngữ định nghĩa dữ liệu (DDL) & Các đối tượng CSDL**
  - Tạo và quản lý Cơ sở dữ liệu (Database).
  - Tạo và quản lý Bảng (Table).
  - Các ràng buộc toàn vẹn (Constraints) và Table hệ thống.
  - Database Diagrams.

- **Chương 4: Ngôn ngữ thao tác dữ liệu (DML)**
  - Lệnh truy vấn (SELECT) và các toán tử.
  - Lệnh cập nhật dữ liệu (INSERT, UPDATE, DELETE).

- **Chương 5: Cơ chế đảm bảo an toàn (Security & Permissions)**
  - Cơ chế xác thực (Authentication).
  - Quản lý tài khoản (Login, User) và Nhóm (Role).
  - Cấp phát và thu hồi quyền (GRANT, REVOKE, DENY).

- **Chương 6: Sao lưu và Phục hồi dữ liệu (Backup & Restore)**
  - Các mô hình sao lưu (Full, Differential, Transaction Log).
  - Phục hồi dữ liệu (kể cả phục hồi theo thời gian điểm - Point-in-time).

- **Chương 7: Nhân bản dữ liệu (Replication)**
  - Mô hình Publisher - Subscriber.
  - Các dạng nhân bản: Snapshot, Transactional, và Merge Replication.

- **Chương 8: Trigger và Hàm (User Defined Function - UDF)**
  - Khai báo và sử dụng Trigger kiểm tra ràng buộc.
  - Các loại hàm UDF (Scalar, Inline Table-valued, Multi-statement).

---

### II. DANH SÁCH CÁC TÀI LIỆU ĐÃ TẢI LÊN (17 Files)

Để bạn tiện theo dõi, mình phân loại 17 file tài liệu bạn đã đưa thành 3 nhóm như sau:

**1. Nhóm Giáo trình chính (Theo từng chương):**

1. `Tổng Quan SQL Server Chương 1` (File text tổng hợp)
2. `SQL1_SRVR2014.docx` (Nội dung Chương 1)
3. `SQL2_SRVR2014.docx` (Nội dung Chương 2)
4. `SQL3_SRVR2014.docx` (Nội dung Chương 3)
5. `SQL4_SRVR2014.docx` (Nội dung Chương 4)
6. `SQL5_SRVR2014.docx` (Nội dung Chương 5)
7. `SQL6_SRVR2014.docx` (Nội dung Chương 6)
8. `SQL7_SRVR2014.docx` (Nội dung Chương 7)
9. `SQL8_TRIGGER_UDF.docx` (Nội dung Chương 8 - Đầy đủ)
10. `SQL8_UDF.docx` (Nội dung Chương 8 - Nhấn mạnh phần UDF)

**2. Nhóm Chuyên đề nâng cao & Mở rộng:**

- `C5_CacMucBaoMat.docx`: Tài liệu bổ trợ chương 5 đi sâu về các mức bảo mật
- `Cursor.docx`: Chuyên đề về con trỏ (Cursor) trong T-SQL, cách duyệt từng dòng dữ liệu trong Stored Procedure/Trigger
- `Các Mức Isolation Level.docx`: Chuyên đề về các mức độ cô lập giao dịch (Read Uncommitted, Read Committed, Repeatable Read, Serializable)
- `GiaoTac_PhanTan.docx`: Chuyên đề về giao tác phân tán (Distributed Transactions) trên nhiều Server.
- `Các Table hệ thống lưu mối liên kết giữa 2 các tables.docx`: Kiến thức tra cứu các bảng hệ thống lưu trữ metadata và khóa ngoại.
- `C6 - Tao bao cao -XtraReport.docx`: Hướng dẫn ứng dụng CSDL để lập báo cáo (Report) bằng công cụ XtraReport.

**3. Nhóm Đồ án / Bài tập thực hành:**

- `Đề tài 3 - HQTCSDL.docx`: Yêu cầu đồ án môn học (Quản lý điểm sinh viên theo hệ tín chỉ) để áp dụng kiến thức.
