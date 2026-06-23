# Hệ thống Quản lý Điểm Sinh viên Hệ Tín chỉ (QLDSV_HTC)

> Đồ án thực hành môn học **Hệ quản trị Cơ sở dữ liệu (DBMS) SQL Server - Đề tài 3** trên nền tảng .NET 10.0 Web MVC, sử dụng ADO.NET (Raw SQL Parameterized) kết hợp Stored Procedures tối ưu và DevExpress Web Document Viewer để in ấn báo cáo.

---

## 🏗️ Tổng quan Kiến trúc Hệ thống

Dự án tuân thủ mô hình **Kiến trúc sạch (Clean Architecture)** nhằm tách biệt tuyệt đối giữa logic nghiệp vụ và cơ sở dữ liệu:

- **[QLDSV_HTC.Domain](src/QLDSV_HTC.Domain)**: Chứa các thực thể, cấu trúc hằng số nghiệp vụ, hằng số cơ sở dữ liệu (`DbConstants.cs`, `AppConstants.cs`), và các hằng số stored procedure (`StoredProcedureConstants.cs`). Hoàn toàn độc lập với database và framework.
- **[QLDSV_HTC.Application](src/QLDSV_HTC.Application)**: Chứa các định nghĩa DTOs, interfaces dịch vụ và các hợp đồng truy cập dữ liệu (Repositories Interfaces).
- **[QLDSV_HTC.Infrastructure](src/QLDSV_HTC.Infrastructure)**: Thực thi kết nối cơ sở dữ liệu và truy cập dữ liệu (ADO.NET SqlClient). Sử dụng giải thuật tối ưu kết nối động dựa trên tài khoản SQL Server đăng nhập của người dùng.
- **[QLDSV_HTC.Web](src/QLDSV_HTC.Web)**: Giao diện người dùng Web MVC (Razor Pages), các Controllers, phân quyền và cấu hình tích hợp DevExpress Web Document Viewer phục vụ in ấn PDF.

> [!NOTE]
> **Cơ chế Đăng nhập Động (Dynamic Connection String)**
>
> - **Giảng viên / Quản trị**: Khi đăng nhập, hệ thống sẽ sử dụng chính tài khoản và mật khẩu SQL Server của người dùng đó để thiết lập chuỗi kết nối (`ConnectionString`). Nếu kết nối SQL Server thành công, hệ thống mới khởi tạo session và lưu trữ kết nối.
> - **Sinh viên**: Tất cả sinh viên sử dụng chung tài khoản đăng nhập SQL Server cấp thấp `sv` cấu hình sẵn trong `.env` để kết nối vào DB, sau đó kiểm tra mật khẩu sinh viên thông qua stored procedure `sp_DangNhap_SinhVien`.

---

## ⚡ Bắt đầu Nhanh (Quick Start)

Làm theo các bước sau để thiết lập dự án trên môi trường Windows local của bạn:

### 1. Yêu cầu Hệ thống

- **SQL Server** 2019 / 2022 trở lên.
- **.NET SDK 10.0** trở lên.
- **DevExpress Libraries** (để render các báo cáo PDF).

### 2. Thiết lập Cơ sở dữ liệu SQL Server

1. Mở SQL Server Management Studio (SSMS) hoặc Azure Data Studio và tạo một cơ sở dữ liệu trống có tên là `QLDSV_HTC`.
2. Chạy file SQL tạo cấu trúc bảng (`Tables`):
   - Mở file **[QLDSV_HTC.sql](src/Database/Tables/QLDSV_HTC.sql)**.
   - _Lưu ý:_ File được định dạng mã hóa **UTF-16LE** để hiển thị đúng ký tự tiếng Việt. Bạn cần mở trực tiếp bằng SSMS hoặc trình soạn thảo hỗ trợ mã hóa này để thực thi chính xác.
3. Tạo các Stored Procedure:
   - Chạy toàn bộ các script SQL trong thư mục **[src/Database/StoredProcedures/](src/Database/StoredProcedures)**.
4. Thiết lập Phân quyền, Index hiệu năng cao và Seed Data mẫu:
   - Chạy lần lượt các script trong thư mục **[src/Database/Scripts/](src/Database/Scripts)** theo thứ tự:
     1. **[001-PhanQuyen.sql](src/Database/Scripts/001-PhanQuyen.sql)** (Cấp quyền hạn tối thiểu cho nhóm `KHOA`, `SV` và toàn quyền cho `PGV`).
     2. **[002-Indexes.sql](src/Database/Scripts/002-Indexes.sql)** (Tạo các chỉ mục bao phủ không cụm - non-clustered covering index để tối ưu hiệu năng truy vấn dữ liệu).
     3. **[003-SeedsDatabase.sql](src/Database/Scripts/003-SeedsDatabase.sql)** (Nạp dữ liệu thử nghiệm về Khoa, Lớp, Môn học, Giảng viên, Sinh viên, Lớp tín chỉ và Đăng ký).

### 3. Cấu hình Ứng dụng

Tạo file **`.env`** tại thư mục gốc của dự án (sao chép nội dung từ **[.env.example](.env.example)**) và chỉnh sửa cấu hình kết nối SQL Server local của bạn:

```ini
# App Configuration
ASPNETCORE_ENVIRONMENT=development
ASPNETCORE_URLS=http://localhost:5211

# Database Configuration
DB_SERVER=localhost
DB_DATABASE=QLDSV_HTC
DB_USER=sa
DB_PASSWORD=YOUR_STRONG_PASSWORD_HERE
DB_TRUST_SERVER_CERTIFICATE=true

# Student Connection String (Tài khoản SQL dùng chung cho nhóm sinh viên)
DB_STUDENT_USER=sv
DB_STUDENT_PASSWORD=sv
```

> [!WARNING]
> Đảm bảo thông tin `DB_USER` và `DB_PASSWORD` là tài khoản quản trị hệ quản trị cơ sở dữ liệu (`sa` hoặc tương đương) có quyền chạy các lệnh DDL và tạo tài khoản đăng nhập/người dùng cơ sở dữ liệu.

### 4. Chạy Ứng dụng

Sử dụng các lệnh Makefile có sẵn hoặc .NET CLI trực tiếp:

- **Chạy ở chế độ Hot-Reload (Development):**
  ```powershell
  make dev
  # Hoặc chạy lệnh thủ công:
  dotnet watch --project src/QLDSV_HTC.Web run
  ```
- **Truy cập hệ thống:** Mở trình duyệt và truy cập địa chỉ [http://localhost:5211](http://localhost:5211).

---

## 🌟 Tính năng Chính của Hệ thống

Hệ thống được thiết kế đầy đủ các chức năng nghiệp vụ của Đề tài 3:

### 1. Quản lý Đăng nhập & Động cơ Phân quyền

- Hỗ trợ 2 phương thức đăng nhập: **Giảng viên / Phòng giáo vụ** (dùng tài khoản SQL Server đăng ký trực tiếp trên database) và **Sinh viên** (sử dụng Mã sinh viên và mật khẩu nội bộ).
- Chống tấn công CSRF (Cross-Site Request Forgery) trên tất cả các tác vụ POST/PUT/DELETE bằng token xác thực.

### 2. Quản lý Danh mục (Chỉ dành cho Phòng Giáo Vụ - PGV)

- **Lớp học & Sinh viên**: Giao diện Master-Detail 2 cấp trực quan, quản lý danh sách sinh viên theo từng lớp học.
- **Môn học**: Thêm, sửa, xóa môn học (Số tiết lý thuyết, số tiết thực hành).
- **Mở lớp tín chỉ**: Mở các lớp tín chỉ mới theo học kỳ và niên khóa, phân công giảng viên giảng dạy.

### 3. Nhập Điểm Hàng Loạt (Bulk Grade Entry)

- Hỗ trợ giảng viên/khoa nhập điểm trực quan theo từng lớp tín chỉ.
- Tự động tính Điểm hết môn theo công thức: $Điểm\_Hết\_Môn = Điểm\_CC \times 0.1 + Điểm\_GK \times 0.3 + Điểm\_CK \times 0.6$.
- **Tối ưu hóa hiệu năng bằng TVP (Table-Valued Parameters):** Toàn bộ bảng điểm đã chỉnh sửa được đóng gói thành một `DataTable` cấu trúc khớp với kiểu dữ liệu bảng `dbo.GradeEntryType` trong SQL Server và gửi về một stored procedure `sp_CapNhatDiem` duy nhất. Việc lưu điểm thực thi trong 1 transaction độc lập, cam kết tính toàn vẹn (atomicity) và tốc độ lưu trữ vượt trội (tránh vòng lặp kết nối N+1).

### 4. Đăng ký Lớp Tín Chỉ (Dành cho Sinh Viên)

- Sinh viên tự đăng nhập bằng mã số của mình.
- Tìm kiếm, lọc danh sách lớp tín chỉ được mở trong học kỳ/niên khóa được chỉ định (chưa bị hủy).
- Thực hiện đăng ký môn học và hủy đăng ký theo thời gian quy định.

### 5. In ấn Báo cáo (Tích hợp DevExpress Reporting)

Hệ thống kết xuất các báo cáo chuẩn PDF chất lượng cao:

1.  **Danh sách lớp tín chỉ**: In các lớp đã mở trong niên khóa, học kỳ kèm số sinh viên đã đăng ký.
2.  **Danh sách sinh viên đăng ký lớp tín chỉ**: In danh sách sinh viên theo môn học và nhóm.
3.  **Bảng điểm môn học của 1 lớp tín chỉ**: Bảng điểm chi tiết (chuyên cần, giữa kỳ, cuối kỳ, hết môn) của tất cả sinh viên tham gia lớp học.
4.  **Phiếu điểm cá nhân**: In phiếu điểm của một sinh viên cụ thể (Lấy điểm cao nhất của các lần thi).
5.  **Bảng điểm tổng kết khóa học**: Báo cáo dạng **Cross-Tab** (Ma trận) thể hiện điểm tổng kết của từng sinh viên đối với toàn bộ các môn học đã hoàn thành trong khóa học của Lớp.

### 6. Báo cáo Động (Dynamic Report Builder)

Cho phép người dùng **tự tạo báo cáo tùy chỉnh** mà không cần viết code — qua giao diện 6 bước kéo-thả:

| Bước | Mô tả |
| :--- | :--- |
| **Step 1** | Chọn bảng dữ liệu nguồn |
| **Step 2** | JOIN bảng liên quan (INNER/LEFT — tự detect FK) |
| **Step 3** | Chọn cột hiển thị + Alias + Sắp xếp (ASC/DESC toggle) |
| **Step 4** | Cấu hình Thống kê: Aggregate (COUNT/SUM/AVG/MIN/MAX), HAVING AND/OR, In theo nhóm |
| **Step 5** | Điều kiện lọc WHERE (10 operators, parameterized) |
| **Step 6** | Cài đặt: Tiêu đề (@PARAM interpolation), Tên file xuất |

**Tính năng nổi bật:**
- **Standalone Aggregates**: SUM/AVG/COUNT không cần GROUP BY (VD: tổng SV toàn trường).
- **HAVING AND/OR**: Toggle giữa AND và OR cho điều kiện lọc sau gom nhóm.
- **@PARAM Title**: Tiêu đề báo cáo có tham số động — `DANH SÁCH SV @MAKHOA` → `DANH SÁCH SV CNTT`.
- **Print by Group**: In PDF theo nhóm với GroupHeader/GroupFooter + tùy chọn sang trang mới mỗi nhóm.
- **Phân trang Preview**: Dropdown 10/25/50/100 dòng + nút ◀▶ + SQL Preview real-time cập nhật OFFSET/FETCH.
- **SQL Preview Live**: Hiển thị câu SQL đang thực thi, cập nhật real-time khi thay đổi cấu hình.
- **Guide Panel**: Sidebar hướng dẫn nhanh cách sử dụng + ràng buộc cần biết.

### 7. Dashboard Sinh Viên

- Hiển thị thông tin cá nhân, lớp, khóa học.
- Học kỳ hiện tại: niên khóa/HK mới nhất có đăng ký + số môn.
- Danh sách môn đã đăng ký ở HK hiện tại kèm điểm (CC, GK, CK).

---

## 🛠️ Makefile và Các lệnh Phát triển hữu ích

Dự án cung cấp các lệnh tự động hóa trong file **[Makefile](Makefile)**:

| Lệnh             | Ý nghĩa                                                                             |
| :--------------- | :---------------------------------------------------------------------------------- |
| `make format`    | Tự động định dạng mã nguồn C# và cảnh báo lỗi vi phạm chuẩn mã nguồn.               |
| `make build`     | Thực hiện build toàn bộ Solution.                                                   |
| `make dev`       | Chạy ứng dụng Web ở chế độ Watch (Hot Reload) đồng thời tự động định dạng mã nguồn. |
| `make clean`     | Dọn dẹp các thư mục rác `bin`/`obj` sinh ra trong quá trình biên dịch.              |
| `make db-update` | Chạy cập nhật database Migration (dành cho EF Core nếu có cấu hình).                |
| `make db-setup`  | Khởi tạo toàn bộ Cơ sở dữ liệu và Stored Procedures từ mã nguồn SQL.                |

---

## 📚 Tài liệu Dự án & Bài học (Documentation & Lessons)

Hệ thống cung cấp bộ tài liệu đầy đủ về yêu cầu nghiệp vụ đồ án cũng như giáo trình học tập SQL Server từ cơ bản đến nâng cao trong thư mục **[docs](docs)**:

### 📋 1. Đồ án & Nghiệp vụ (Assignment & Specification)

- **[Yêu cầu Đề tài 3](docs/assignment/De3.md)**: Chi tiết yêu cầu đề tài quản lý điểm sinh viên hệ tín chỉ từ giáo viên.
- **[Tóm tắt & Trạng thái triển khai](docs/assignment/TomTatDe3.md)**: Bảng theo dõi tiến độ thực tế, đánh giá chi tiết các tính năng đã hoàn thành và kỹ thuật tối ưu hóa đã áp dụng trên codebase.

### 📖 2. Giáo trình học tập SQL Server (MS SQL Server Lessons)

Hệ thống tài liệu học tập được tổ chức khoa học theo từng chương tại **[Mục lục bài học](docs/lessons/MucLuc.md)**:

- **[Chương 1: Tổng quan về SQL Server](docs/lessons/Chuong1_TongQuanSQLServer.md)**: Kiến trúc mạng, các dịch vụ và đối tượng CSDL cơ bản.
- **[Chương 2: Hệ quản trị SQL Server](docs/lessons/Chuong2_TaoVaQuanLyCSDL.md)**: Hướng dẫn cấu hình SSMS và các kiểu dữ liệu.
- **[Chương 3: Ngôn ngữ định nghĩa dữ liệu (DDL)](docs/lessons/Chuong3_TaoVaQuanLyBangVaRangBuoc.md)**: Quản lý database, bảng, các ràng buộc toàn vẹn.
- **[Chương 4: Ngôn ngữ thao tác dữ liệu (DML)](docs/lessons/Chuong4_ThaoTacVaTruyVanDuLieu.md)**: SELECT nâng cao, các lệnh INSERT, UPDATE, DELETE.
- **[Chương 5: Cơ chế an toàn và bảo mật](docs/lessons/Chuong5_BaoMatVaPhanQuyenCSDL.md)**: Login, User, Role và cơ chế GRANT/REVOKE/DENY.
- **[Chương 6: Sao lưu & Phục hồi dữ liệu](docs/lessons/Chuong6_SaoLuuVaPhucHoiDuLieu.md)**: Full/Diff/Log Backups và Point-in-time Restore.
- **[Chương 7: Nhân bản dữ liệu (Replication)](docs/lessons/Chuong7_NhanBanDuLieu.md)**: Snapshot, Transactional, Merge Replication.
- **[Chương 8: Trigger và UDF](docs/lessons/Chuong8_TriggerVaUDF.md)**: Ràng buộc bằng Trigger và xây dựng các hàm tự định nghĩa.

### 🚀 3. Chuyên đề nâng cao (Advanced Topics)

Tài liệu đi sâu giải quyết các bài toán kỹ thuật phức tạp trong SQL Server tại **[Thư mục Chuyên đề nâng cao](docs/lessons/advance)**:

- **[Tối ưu hóa Truy vấn CSDL](docs/lessons/advance/ToiUuTruyVanCSDL.md)**: Quy tắc tối ưu hóa câu lệnh SELECT, cách viết Stored Procedure hiệu năng cao.
- **[Các mức cô lập giao dịch (Isolation Levels)](docs/lessons/advance/CacMucCoLapGiaoDich_IsolationLevels.md)**: Dirty Read, Non-repeatable Read, Phantom Read và cách thiết lập mức cô lập phù hợp.
- **[Con trỏ (Cursor) trong T-SQL](docs/lessons/advance/ConTro_Cursor.md)**: Cách khai báo, duyệt và giải phóng tài nguyên khi sử dụng Cursor.
- **[Báo cáo XtraReport](docs/lessons/advance/TaoBaoCao.md)**: Cách thiết kế báo cáo động bằng DevExpress Reporting.
- **[Bảo mật chuyên sâu trong SQL Server](docs/lessons/advance/CacMucBaoMat.md)**: Tìm hiểu sâu về các lớp bảo mật Server, Database và Schema.
- **[Tra cứu Mối liên kết Khóa ngoại bằng System Tables](docs/lessons/advance/CacBangHeThongLuuMoiLienKet_FK.md)**: Sử dụng các bảng hệ thống như `sys.foreign_keys`, `sys.foreign_key_columns` để truy vấn metadata liên kết dữ liệu.

---

## 📜 Quy tắc Viết mã và Tối ưu Cơ sở dữ liệu

Tất cả các thành viên tham gia phát triển dự án cần tuân thủ nghiêm ngặt các quy tắc đã được định nghĩa tại file hướng dẫn:

👉 **[Xem chi tiết Quy tắc Phát triển và Tối ưu CSDL](.agents/rules/rules.md)**

Các quy tắc quan trọng bao gồm:

1.  **Không Hardcode hằng số**: Tuyệt đối không viết trực tiếp tên cột, tên stored procedure hoặc route trong logic controller/repository. Phải sử dụng lớp hằng số tương ứng như `DbConstants` hoặc `AppConstants`.
2.  **Không nối chuỗi SQL**: Sử dụng hoàn toàn SQL Parameterized để chống lỗi bảo mật SQL Injection.
3.  **Thực thi Bulk Insert/Update qua TVP**: Tuyệt đối không chạy vòng lặp cập nhật điểm qua nhiều kết nối đơn lẻ.
4.  **Quy luật Tối ưu Truy vấn SQL**:
    - Phép chọn (Filter) và phép chiếu (Select các trường cần thiết) trước, phép kết (Join) sau.
    - Khử các phép nối không cần thiết (dùng `EXISTS` thay `LEFT JOIN` khi chỉ cần validate).
    - Tách điều kiện non-SARGable (VD: `HO + ' ' + TEN` → LIKE riêng `HO`/`TEN`).
    - Tối ưu vị trí của điều kiện lọc trong mệnh đề `AND` (sai đặt trước) và `OR` (đúng đặt trước).
    - Khai thác tối đa non-clustered indexes đã định cấu hình.

### Kết quả Audit SP (21 Stored Procedures)

| SP | Kỹ thuật tối ưu đã áp dụng |
| :--- | :--- |
| 006 | Khử phép nối — truy vấn `MALTC` riêng trước, giảm JOIN từ 3→2 bảng |
| 007 | `LEFT JOIN LOP` → `EXISTS`, tách `HO+TEN` SARGable, reorder WHERE (PK first) |
| 008 | CTE `FilteredDK` lọc trước JOIN + `ROW_NUMBER` lấy điểm max 1 lần quét |
| 009 | Temp table materialize → tránh CTE double-scan khi PIVOT |
| 012 | `QUOTENAME()` + `OBJECT_ID` validate + `sp_executesql` parameterize OFFSET/FETCH |
| 017 | `SERIALIZABLE` isolation + TRY/CATCH pattern |
| 019 | TVP (`GradeEntryType`) — bulk update điểm 1 round-trip |
| 021 | Extract `@HAS_DK`, `@CUR_NK/@CUR_HK` biến cục bộ — giảm 3 lần quét bảng DANGKY |

---

## 📝 Giấy phép

Dự án được phân phối dưới giấy phép nội bộ phục vụ học tập và nghiên cứu môn học Hệ quản trị cơ sở dữ liệu.
