# Hệ thống Quản lý Điểm Sinh viên Hệ Tín chỉ (QLDSV_HTC)

> Đồ án thực hành môn học **Hệ quản trị Cơ sở dữ liệu (DBMS) SQL Server - Đề tài 3** trên nền tảng .NET 10.0 Web MVC, sử dụng ADO.NET (Raw SQL Parameterized) kết hợp Stored Procedures tối ưu và DevExpress Web Document Viewer để in ấn báo cáo.

---

## 🏗️ Tổng quan Kiến trúc Hệ thống

Dự án tuân thủ mô hình **Kiến trúc sạch (Clean Architecture)** nhằm tách biệt tuyệt đối giữa logic nghiệp vụ và cơ sở dữ liệu:

- **[QLDSV_HTC.Domain](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/QLDSV_HTC.Domain)**: Chứa các thực thể, cấu trúc hằng số nghiệp vụ, hằng số cơ sở dữ liệu (`DbConstants.cs`, `AppConstants.cs`), và các hằng số stored procedure (`StoredProcedureConstants.cs`). Hoàn toàn độc lập với database và framework.
- **[QLDSV_HTC.Application](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/QLDSV_HTC.Application)**: Chứa các định nghĩa DTOs, interfaces dịch vụ và các hợp đồng truy cập dữ liệu (Repositories Interfaces).
- **[QLDSV_HTC.Infrastructure](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/QLDSV_HTC.Infrastructure)**: Thực thi kết nối cơ sở dữ liệu và truy cập dữ liệu (ADO.NET SqlClient). Sử dụng giải thuật tối ưu kết nối động dựa trên tài khoản SQL Server đăng nhập của người dùng.
- **[QLDSV_HTC.Web](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/QLDSV_HTC.Web)**: Giao diện người dùng Web MVC (Razor Pages), các Controllers, phân quyền và cấu hình tích hợp DevExpress Web Document Viewer phục vụ in ấn PDF.

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
   - Mở file **[QLDSV_HTC.sql](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/Database/Tables/QLDSV_HTC.sql)**.
   - _Lưu ý:_ File được định dạng mã hóa **UTF-16LE** để hiển thị đúng ký tự tiếng Việt. Bạn cần mở trực tiếp bằng SSMS hoặc trình soạn thảo hỗ trợ mã hóa này để thực thi chính xác.
3. Tạo các Stored Procedure:
   - Chạy toàn bộ các script SQL trong thư mục **[src/Database/StoredProcedures/](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/Database/StoredProcedures)**.
4. Thiết lập Phân quyền, Index hiệu năng cao và Seed Data mẫu:
   - Chạy lần lượt các script trong thư mục **[src/Database/Scripts/](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/Database/Scripts)** theo thứ tự:
     1. **[001-PhanQuyen.sql](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/Database/Scripts/001-PhanQuyen.sql)** (Cấp quyền hạn tối thiểu cho nhóm `KHOA`, `SV` và toàn quyền cho `PGV`).
     2. **[002-Indexes.sql](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/Database/Scripts/002-Indexes.sql)** (Tạo các chỉ mục bao phủ không cụm - non-clustered covering index để tối ưu hiệu năng truy vấn dữ liệu).
     3. **[003-SeedsDatabase.sql](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/Database/Scripts/003-SeedsDatabase.sql)** (Nạp dữ liệu thử nghiệm về Khoa, Lớp, Môn học, Giảng viên, Sinh viên, Lớp tín chỉ và Đăng ký).

### 3. Cấu hình Ứng dụng

Tạo file **`.env`** tại thư mục gốc của dự án (sao chép nội dung từ **[.env.example](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/.env.example)**) và chỉnh sửa cấu hình kết nối SQL Server local của bạn:

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

---

## 🛠️ Makefile và Các lệnh Phát triển hữu ích

Dự án cung cấp các lệnh tự động hóa trong file **[makefile](file:///d:/CODE%20PLAYGROUND%20Projects/HQTCSDL/QLDSV_HQT_DBMS/makefile)**:

| Lệnh             | Ý nghĩa                                                                             |
| :--------------- | :---------------------------------------------------------------------------------- |
| `make format`    | Tự động định dạng mã nguồn C# và cảnh báo lỗi vi phạm chuẩn mã nguồn.               |
| `make build`     | Thực hiện build toàn bộ Solution.                                                   |
| `make dev`       | Chạy ứng dụng Web ở chế độ Watch (Hot Reload) đồng thời tự động định dạng mã nguồn. |
| `make clean`     | Dọn dẹp các thư mục rác `bin`/`obj` sinh ra trong quá trình biên dịch.              |
| `make db-update` | Chạy cập nhật database Migration (dành cho EF Core nếu có cấu hình).                |

---

## 📜 Quy tắc Viết mã và Tối ưu Cơ sở dữ liệu

Tất cả các thành viên tham gia phát triển dự án cần tuân thủ nghiêm ngặt các quy tắc đã được định nghĩa tại file hướng dẫn:

👉 **[Xem chi tiết Quy tắc Phát triển và Tối ưu CSDL](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/.agents/rules/rules.md)**

Các quy tắc quan quan trọng bao gồm:

1.  **Không Hardcode hằng số**: Tuyệt đối không viết trực tiếp tên cột, tên stored procedure hoặc route trong logic controller/repository. Phải sử dụng lớp hằng số tương ứng như `DbConstants` hoặc `AppConstants`.
2.  **Không nối chuỗi SQL**: Sử dụng hoàn toàn SQL Parameterized để chống lỗi bảo mật SQL Injection.
3.  **Thực thi Bulk Insert/Update qua TVP**: Tuyệt đối không chạy vòng lặp cập nhật điểm qua nhiều kết nối đơn lẻ.
4.  **Quy luật Tối ưu Truy vấn SQL**:
    - Phép chọn (Filter) và phép chiếu (Select các trường cần thiết) trước, phép kết (Join) sau.
    - Khử các phép nối không cần thiết.
    - Tối ưu vị trí của điều kiện lọc trong mệnh đề `AND` (sai đặt trước) và `OR` (đúng đặt trước).
    - Khai thác tối đa non-clustered indexes đã định cấu hình.

---

## 📝 Giấy phép

Dự án được phân phối dưới giấy phép nội bộ phục vụ học tập và nghiên cứu môn học Hệ quản trị cơ sở dữ liệu.
