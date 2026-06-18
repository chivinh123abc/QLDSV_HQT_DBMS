# 🔍 BÁO CÁO PHÂN TÍCH HỆ THỐNG & RÀNG BUỘC DỮ LIỆU (QLDSV_HTC)

Báo cáo này tài liệu hóa toàn bộ kiến trúc nghiệp vụ, luồng thực thi kỹ thuật và các quy tắc ràng buộc cơ sở dữ liệu cốt lõi của hệ thống **Quản lý Điểm Sinh viên Hệ Tín chỉ (QLDSV_HTC)** được ánh xạ từ mã nguồn C# (.NET 10.0 Web MVC) và SQL Server Stored Procedures dựa trên yêu cầu đặc tả đề tài.

---

## 1. Chức năng Đăng ký Lớp Tín Chỉ (Course Registration)

### 1. Phân quyền (Actors & RBAC)
- **Quyền truy cập:** Chỉ dành cho sinh viên (**SV**).
- **Quyền bị chặn:** Các vai trò Phòng Giáo vụ (**PGV**) và Khoa (**KHOA**) bị chặn truy cập ở mức Controller bằng thuộc tính `[Authorize(Roles = AppConstants.Groups.SV)]`.

### 2. Luồng Nghiệp Vụ (Business Flow)
- **Bắt đầu:** Sinh viên đăng nhập vào hệ thống (sử dụng Login chung `sv`, sau đó nhập Mã sinh viên làm thông tin định danh riêng).
- **Tương tác UI:**
  1. Giao diện tự động hiển thị thông tin Sinh viên (Mã SV, Họ tên, Lớp học).
  2. Sinh viên chọn **Niên khóa** và **Học kỳ** mong muốn, rồi nhấn **Lọc**.
  3. UI hiển thị danh sách các lớp tín chỉ đang mở (chưa bị hủy).
  4. Sinh viên click vào nút **Đăng ký** hoặc **Hủy** trên từng lớp tương ứng.
- **Kết quả:** Đăng ký của sinh viên được ghi nhận tức thời, số lượng sinh viên đã đăng ký của lớp tín chỉ tăng/giảm tương ứng.

### 3. Luồng Kỹ Thuật (Technical Execution Flow)
- **UI/View:** Giao diện `Index.cshtml` trong thư mục [src/QLDSV_HTC.Web/Views/Registration](../src/QLDSV_HTC.Web/Views/Registration) dùng HTML/JS và AJAX gửi request JSON.
- **Controller/Endpoint:**
  - `GET /registration` (Index): Trả về trang đăng ký cùng thông tin sinh viên hiện tại.
  - `GET /registration/filter?year=...&semester=...` (Filter): Trả về JSON danh sách lớp tín chỉ khả dụng.
  - `POST /registration/register` (Register): Payload JSON chứa `{ creditClassId }`.
  - `POST /registration/unregister` (Unregister): Payload JSON chứa `{ creditClassId }`.
- **Service/Repository Layer:** [RegistrationController](../src/QLDSV_HTC.Web/Controllers/RegistrationController.cs) gọi [IRegistrationRepository](../src/QLDSV_HTC.Application/Interfaces/IRegistrationRepository.cs):
  - `GetAvailableClassesAsync(year, semester, studentId)`
  - `RegisterAsync(studentId, creditClassId)`
  - `UnregisterAsync(studentId, creditClassId)`
- **Database Layer:** [RegistrationRepository](../src/QLDSV_HTC.Infrastructure/Repositories/RegistrationRepository.cs) gọi các Stored Procedures:
  - `sp_LayDanhSachLopTinChi_SinhVien` để lấy danh sách lớp tín chỉ kèm cờ `DA_DANGKY`.
  - `sp_DangKyLopTinChi` để đăng ký: sử dụng `BEGIN TRAN` và cô lập giao dịch `SET TRANSACTION ISOLATION LEVEL SERIALIZABLE` để chặn Race Condition khi số lượng đăng ký chạm ngưỡng tối đa cùng lúc.
  - `sp_HuyDangKyLopTinChi` để hủy đăng ký (cập nhật cờ `HUYDANGKY = 1`, sử dụng transaction và cô lập `SERIALIZABLE`).

### 4. Ràng Buộc Nghiệp Vụ (Business Rules & Constraints) - [CRITICAL]
- **Ràng buộc thời gian (Temporal Check):** Không thể đăng ký hoặc hủy đăng ký lớp tín chỉ trong quá khứ. Stored procedure tự động tính niên khóa/học kỳ hiện tại dựa trên ngày hệ thống (`GETDATE()`) để chặn các request trái phép.
- **Ràng buộc trạng thái sinh viên:** Sinh viên đã nghỉ học (`DANGHIHOC = 1` trong bảng `SINHVIEN`) bị chặn đăng ký hoàn toàn.
- **Ràng buộc trùng môn:** Không được phép đăng ký hai lớp tín chỉ khác nhau cho cùng một môn học trong một học kỳ.
- **Ràng buộc đậu/rớt (Hoàn thành môn):** Nếu sinh viên đã đăng ký môn này trong quá khứ và đạt điểm đậu (`DIEM_GK >= 5.0` - tính theo điểm hết môn cuối cùng), hệ thống sẽ chặn không cho đăng ký lại môn đó. Nếu rớt (`DIEM_GK < 5.0`), cho phép đăng ký học cải thiện ở các học kỳ tiếp theo.
- **Ràng buộc điểm số khi hủy:** Không cho phép hủy đăng ký lớp tín chỉ nếu sinh viên đó đã được nhập điểm giữa kỳ (`DIEM_GK IS NOT NULL`) hoặc điểm cuối kỳ (`DIEM_CK IS NOT NULL`).
- **Tái đăng ký (Re-enrollment):** Nếu sinh viên đăng ký lại một lớp đã từng hủy (`HUYDANGKY = 1`), hệ thống sẽ chỉ cập nhật `HUYDANGKY = 0` thay vì chèn một dòng mới trùng khóa chính (`MALTC`, `MASV`).

---

## 2. Chức năng Nhập Điểm Hàng Loạt (Bulk Grade Entry)

### 1. Phân quyền (Actors & RBAC)
- **Quyền truy cập:** Điểm thuộc khoa nào thì khoa đó nhập hoặc PGV nhập (**PGV**, **KHOA**).
- **Quyền bị chặn:** Sinh viên (**SV**) bị chặn hoàn toàn ở mức định tuyến và Controller.

### 2. Luồng Nghiệp Vụ (Business Flow)
- **Bắt đầu:** Giảng viên/Nhân viên đăng nhập -> Chọn chức năng Nhập điểm.
- **Tương tác UI:**
  1. Chọn Niên khóa, Học kỳ, Môn học, Nhóm lớp tín chỉ và nhấn **Bắt đầu**.
  2. Bảng nhập điểm hiển thị danh sách sinh viên đã đăng ký hợp lệ.
  3. Người dùng nhập điểm Chuyên cần, Giữa kỳ, Cuối kỳ trực tiếp trên lưới dữ liệu. Cột điểm Hết môn tự động tính theo tỷ lệ nhưng ở chế độ Read-only.
  4. Nhấn **Ghi điểm** để lưu hàng loạt.
- **Kết quả:** Bảng điểm được cập nhật an toàn vào cơ sở dữ liệu trong một transaction duy nhất.

### 3. Luồng Kỹ Thuật (Technical Execution Flow)
- **UI/View:** Razor View `Index.cshtml` trong thư mục [src/QLDSV_HTC.Web/Views/Grade](../src/QLDSV_HTC.Web/Views/Grade).
- **Controller/Endpoint:**
  - `GET /grade/grades?year=...&semester=...` (GetGrades): Trả về danh sách sinh viên kèm điểm hiện tại dưới dạng JSON.
  - `POST /grade/save-grades` (SaveGrades): Payload nhận một danh sách đối tượng `IEnumerable<GradeEntryDto>` dạng JSON.
- **Service/Repository Layer:** [GradeController](../src/QLDSV_HTC.Web/Controllers/GradeController.cs) gọi [IGradeRepository](../src/QLDSV_HTC.Application/Interfaces/IGradeRepository.cs):
  - `GetGradesAsync(...)`
  - `UpdateGradesAsync(grades)`: Đóng gói danh sách điểm vào đối tượng `DataTable` C# với cấu trúc khớp chính xác kiểu dữ liệu bảng tự định nghĩa `dbo.GradeEntryType` trong cơ sở dữ liệu.
- **Database Layer:** [GradeRepository](../src/QLDSV_HTC.Infrastructure/Repositories/GradeRepository.cs) gọi:
  - `sp_LayBangDiemMonHocCuaMotLopTinChi` để tải dữ liệu.
  - `sp_CapNhatDiem` truyền tham số có cấu trúc (Table-Valued Parameter - TVP) `@Grades`. Transaction được mở và rollback tự động nếu xảy ra lỗi.

### 4. Ràng Buộc Nghiệp Vụ (Business Rules & Constraints) - [CRITICAL]
- **Ràng buộc đăng ký lớp:** Stored procedure `sp_CapNhatDiem` kiểm tra chéo (`LEFT JOIN`) toàn bộ danh sách điểm gửi lên với bảng đăng ký gốc. Nếu phát hiện sinh viên không đăng ký lớp tín chỉ đó hoặc đã hủy đăng ký (`HUYDANGKY = 1`), giao dịch bị hủy bỏ (`ROLLBACK TRAN`) và quăng lỗi: `"Có sinh viên không đăng ký lớp tín chỉ này hoặc lớp không tồn tại."`
- **Giới hạn thang điểm:** Lớp Controller C# thực hiện xác thực nghiệp vụ, đảm bảo các cột điểm nằm trong đoạn $[0, 10]$.
- **Xử lý giá trị trống (Null Handling):** Điểm chưa nhập được lưu là `NULL` trong database. Điểm hết môn chỉ được tính khi cả 3 cột điểm (`DIEM_CC`, `DIEM_GK`, `DIEM_CK`) đều có giá trị. Công thức tính:
  $$DIEM\_HET\_MON = DIEM\_CC \times 0.1 + DIEM\_GK \times 0.3 + DIEM\_CK \times 0.6$$

---

## 3. Chức năng Quản lý Lớp học & Sinh viên (Class & Student Management)

### 1. Phân quyền (Actors & RBAC)
- **Quyền xem danh sách:** Cả **PGV** và **KHOA** đều có quyền xem.
- **Quyền Thêm, Sửa, Xóa (CUD):** Chỉ dành riêng cho Phòng Giáo vụ (**PGV**). Vai trò khoa (**KHOA**) bị từ chối quyền chỉnh sửa ở mức Controller bằng thuộc tính `[Authorize(Roles = AppConstants.Groups.PGV)]`.

### 2. Luồng Nghiệp Vụ (Business Flow)
- **Lớp học:** PGV quản lý thông tin các lớp học chính quy (Mã lớp, Tên lớp, Khóa học, Khoa).
- **Sinh viên (Master-Detail - SubForm 2 cấp):**
  - Giao diện được thiết kế theo cấu trúc SubForm 2 cấp:
    - **Cấp 1 (Master):** Lưới hiển thị danh sách lớp thuộc Khoa quản lý.
    - **Cấp 2 (Detail):** Khi click chọn một lớp cụ thể từ danh sách cấp 1, danh sách sinh viên của lớp đó sẽ tự động được lọc và tải lên ở lưới bên dưới.
  - PGV thực hiện thêm sinh viên mới, sửa thông tin cá nhân hoặc xóa sinh viên trực tiếp tại SubForm chi tiết này.

### 3. Luồng Kỹ Thuật (Technical Execution Flow)
- **UI/View:** Các view tương ứng tại [src/QLDSV_HTC.Web/Views/Class](../src/QLDSV_HTC.Web/Views/Class) và [src/QLDSV_HTC.Web/Views/Student](../src/QLDSV_HTC.Web/Views/Student).
- **Controller/Endpoint:**
  - `GET /class`, `POST /class/add`, `POST /class/edit`, `POST /class/delete`
  - `GET /student`, `POST /student/add`, `POST /student/edit`, `POST /student/delete`
- **Service/Repository Layer:** [ClassController](../src/QLDSV_HTC.Web/Controllers/ClassController.cs) và [StudentController](../src/QLDSV_HTC.Web/Controllers/StudentController.cs) gọi `IClassRepository` và `IStudentRepository`.
- **Database Layer:** Thực thi các stored procedure CRUD:
  - `sp_LayDanhSachLop`, `sp_ThemLop`, `sp_SuaLop`, `sp_XoaLop`
  - `sp_LayDanhSachSinhVien`, `sp_ThemSinhVien`, `sp_SuaSinhVien`, `sp_XoaSinhVien`

### 4. Ràng Buộc Nghiệp Vụ (Business Rules & Constraints) - [CRITICAL]
- **Khóa dữ liệu theo khoa (Data Isolation Lock):**
  - Tài khoản quản trị **PGV** hoặc tài khoản hệ thống `dbo` có thể xem toàn bộ các lớp học và sinh viên toàn trường.
  - Tài khoản **KHOA** bị khóa cứng tại database (bên trong stored procedure `sp_LayDanhSachLop` và `sp_LayDanhSachSinhVien`): hệ thống tự động xác định mã khoa của giảng viên đăng nhập (`MAKHOA FROM GIANGVIEN WHERE MAGV = USER_NAME()`) và chỉ truy vấn dữ liệu lớp/sinh viên thuộc khoa đó quản lý.
- **Ràng buộc phụ thuộc ngoại khóa (FK Deletion Locks):**
  - Không được xóa lớp nếu đang có sinh viên thuộc lớp đó (`IF EXISTS (SELECT 1 FROM SINHVIEN WHERE MALOP = @MALOP)` -> báo lỗi).
  - Không được xóa sinh viên nếu sinh viên đó đã phát sinh dữ liệu học tập/đăng ký tín chỉ (SQL Server tự trả về lỗi FK mã `547` khi thực thi lệnh `DELETE FROM SINHVIEN`, stored procedure sẽ bẫy lỗi này để hiển thị thông báo thân thiện: `"Không thể xóa sinh viên do dữ liệu đang bị ràng buộc..."`).
- **Ràng buộc định danh (PrimaryKey Mutability):** Cho phép đổi mã lớp học (`MALOP`) hoặc mã sinh viên (`MASV`) từ cũ sang mới. Stored procedure cập nhật mã mới và tự động cascade đến các bảng liên quan nếu có thiết lập hoặc chặn trùng mã mới với bản ghi khác.
- **Ràng buộc tuổi tác:** Ngày sinh sinh viên nhập vào phải đảm bảo sinh viên từ 16 tuổi trở lên (`DATEDIFF(YEAR, @NGAYSINH, GETDATE()) >= 16`).

---

## 4. Chức năng Quản lý Môn học (Subject Management)

### 1. Phân quyền (Actors & RBAC)
- **Quyền truy cập:** Cả **PGV** và **KHOA** đều có quyền xem danh sách môn học.
- **Quyền Thêm, Sửa, Xóa (CUD):** Chỉ dành riêng cho Phòng Giáo vụ (**PGV**). Vai trò khoa (**KHOA**) bị từ chối quyền chỉnh sửa ở mức Controller bằng thuộc tính `[Authorize(Roles = AppConstants.Groups.PGV)]`.

### 2. Luồng Nghiệp Vụ (Business Flow)
- **Bắt đầu:** PGV đăng nhập -> Vào chức năng Quản lý Môn học.
- **Tương tác UI:**
  1. Danh sách môn học hiển thị thông tin chi tiết: Mã môn học, Tên môn học, Số tiết lý thuyết, Số tiết thực hành.
  2. Để thêm môn học mới: PGV nhập đầy đủ thông tin mã, tên, số tiết lý thuyết/thực hành và lưu.
  3. Để sửa thông tin: PGV chỉnh sửa trên dòng tương ứng và bấm ghi lại.
  4. Để xóa môn học: PGV click nút xóa trên dòng tương ứng.
- **Kết quả:** Dữ liệu môn học thay đổi trực tiếp trên phạm vi toàn trường (vì môn học không thuộc riêng khoa nào).

### 3. Luồng Kỹ Thuật (Technical Execution Flow)
- **UI/View:** View `Index.cshtml` trong thư mục [src/QLDSV_HTC.Web/Views/Subject](../src/QLDSV_HTC.Web/Views/Subject) hỗ trợ phân trang động.
- **Controller/Endpoint:** `GET /subject`, `POST /subject/add`, `POST /subject/edit`, `POST /subject/delete`.
- **Service/Repository Layer:** [SubjectController](../src/QLDSV_HTC.Web/Controllers/SubjectController.cs) gọi [ISubjectRepository](../src/QLDSV_HTC.Application/Interfaces/ISubjectRepository.cs) thực thi nghiệp vụ:
  - `GetPagedSubjectListAsync(paging)`
  - `AddSubjectAsync(dto)`
  - `UpdateSubjectAsync(dto)`
  - `DeleteSubjectAsync(subjectId)`
- **Database Layer:** [SubjectRepository](../src/QLDSV_HTC.Infrastructure/Repositories/SubjectRepository.cs) gọi:
  - `sp_LayDanhSachMonHoc` (hoặc pagination động qua `sp_PhanTrangDong`) để hiển thị.
  - `sp_ThemMonHoc`, `sp_SuaMonHoc`, `sp_XoaMonHoc`.

### 4. Ràng Buộc Nghiệp Vụ (Business Rules & Constraints) - [CRITICAL]
- **Tính độc nhất môn học:** Mã môn học (`MAMH`) và Tên môn học (`TENMH`) phải là độc nhất trong toàn bộ cơ sở dữ liệu (Unique Key).
- **Ràng buộc xóa (FK Deletion Block):** Không được phép xóa môn học nếu môn học đó đã được lên lịch mở lớp tín chỉ trong bảng `LOPTINCHI` (`ON DELETE NO ACTION` / `RESTRICT`). Hệ thống sẽ ném lỗi FK nếu cố tình xóa môn đã có lớp tín chỉ.
- **Ràng buộc dữ liệu giờ học:** Số tiết lý thuyết (`SOTIET_LT`) và thực hành (`SOTIET_TH`) bắt buộc phải $\ge 0$.

---

## 5. Chức năng Quản lý Mở Lớp Tín Chỉ (Credit Class Management)

### 1. Phân quyền (Actors & RBAC)
- **Quyền xem danh sách:** Cả **PGV** và **KHOA** đều có quyền xem danh sách lớp tín chỉ được mở.
- **Quyền Thêm, Sửa, Xóa (CUD):** Chỉ dành riêng cho Phòng Giáo vụ (**PGV**). Vai trò khoa (**KHOA**) bị từ chối quyền chỉnh sửa ở mức Controller bằng thuộc tính `[Authorize(Roles = AppConstants.Groups.PGV)]`.

### 2. Luồng Nghiệp Vụ (Business Flow)
- **Bắt đầu:** PGV đăng nhập -> Vào chức năng Quản lý lớp tín chỉ.
- **Tương tác UI:**
  1. Chọn Niên khóa, Học kỳ và Khoa để xem danh sách lớp tín chỉ đã mở.
  2. Để mở lớp mới: chọn Môn học, Nhóm lớp, Giảng viên giảng dạy, số sinh viên tối thiểu và lưu.
  3. Để hủy lớp: sửa thông tin lớp tín chỉ và chọn cờ `HUYLOP = 1`.
  4. Để xóa lớp: click xóa trên dòng tương ứng (chỉ cho phép khi chưa có ai đăng ký).

### 3. Luồng Kỹ Thuật (Technical Execution Flow)
- **UI/View:** View `Index.cshtml` trong thư mục [src/QLDSV_HTC.Web/Views/CreditClass](../src/QLDSV_HTC.Web/Views/CreditClass).
- **Controller/Endpoint:** `GET /credit-class`, `POST /credit-class/add`, `POST /credit-class/edit`, `POST /credit-class/delete`.
- **Service/Repository Layer:** [CreditClassController](../src/QLDSV_HTC.Web/Controllers/CreditClassController.cs) gọi `ICreditClassRepository`.
- **Database Layer:** Gọi các stored procedure:
  - `sp_LayDanhSachLopTinChi` (áp dụng tối ưu: CTE chọn trước kết sau).
  - `sp_ThemLopTinChi`, `sp_SuaLopTinChi`, `sp_XoaLopTinChi`.

### 4. Ràng Buộc Nghiệp Vụ (Business Rules & Constraints) - [CRITICAL]
- **Chặn thời gian quá khứ:** Không được mở lớp tín chỉ cho niên khóa/học kỳ đã diễn ra so với thời điểm hiện tại.
- **Ràng buộc tính độc nhất:** Không cho phép trùng lặp cấu hợp khóa (`NIENKHOA` + `HOCKY` + `MAMH` + `NHOM`).
- **Giới hạn đầu vào:** Số sinh viên tối thiểu (`SOSVTOITHIEU`) bắt buộc lớn hơn 0 (áp dụng check constraint `CK_SOSVTOITHIEU`).
- **Ràng buộc xóa:** Tuyệt đối không cho xóa lớp tín chỉ nếu đã có sinh viên đăng ký môn học đó (dù đã hủy hay chưa hủy). Hệ thống chỉ cho phép cập nhật trạng thái hủy lớp (`HUYLOP = 1`).
- **Phân quyền dữ liệu:** Khoa chỉ được xem thông tin danh sách giảng viên thuộc khoa mình khi mở lớp tín chỉ.

---

## 6. Chức năng Quản lý Tài khoản & Đăng nhập (User Administration & Auth)

### 1. Phân quyền (Actors & RBAC)
- **Tạo tài khoản (Add/Register):** Cả **PGV** và **KHOA** đều có quyền tạo.
- **Xóa tài khoản (Delete):** Chỉ dành riêng cho Phòng Giáo vụ (**PGV**). Khoa (**KHOA**) bị chặn xóa tài khoản.
- **Sinh viên (SV):** Không được truy cập vào bất kỳ chức năng quản trị tài khoản nào.

### 2. Luồng Nghiệp Vụ (Business Flow)
- **Cơ chế Đăng nhập đặc biệt cho Sinh viên:**
  - Để tối ưu hóa kết nối SQL Server và không phải tạo hàng vạn SQL Login cho từng sinh viên, hệ thống áp dụng cơ chế **dùng chung một SQL Login duy nhất** tên là `sv`.
  - Khi sinh viên đăng nhập: Client dùng account `sv` để thiết lập kết nối (ConnectionString) xuống SQL Server. Sau khi kết nối thành công, hệ thống sử dụng Mã sinh viên và Password do sinh viên nhập để đối chiếu thông tin trong bảng `SINHVIEN` (qua stored procedure `sp_DangNhap_SinhVien`). Nhãn hiển thị Login chuyển sang Mã sinh viên để định danh và phân quyền nội bộ trên Web.
- **Giảng viên (PGV / KHOA) Đăng nhập:**
  - Mỗi Giảng viên được cấp một tài khoản SQL Login riêng.
  - Khi đăng nhập, hệ thống kết nối trực tiếp bằng SQL Login này. Stored procedure `sp_DangNhap` sẽ kiểm tra tên user đăng nhập để xác định xem user thuộc nhóm quyền nào (`PGV` hay `KHOA`).
- **Tạo tài khoản mới:**
  - Nhập Tên đăng nhập (SQL Login), Mật khẩu, Tên User (Mã giảng viên) và Nhóm quyền (PGV/KHOA) -> Hệ thống tạo SQL Login ở cấp Server và Database User tương ứng ở cấp Database.

### 3. Luồng Kỹ Thuật (Technical Execution Flow)
- **UI/View:** Giao diện quản lý tài khoản dạng bảng AJAX tại `_AccountTable.cshtml`.
- **Controller/Endpoint:** `GET /account/management`, `POST /account/management/add`, `POST /account/management/edit`, `POST /account/management/delete`.
- **Service/Repository Layer:** [AccountController](../src/QLDSV_HTC.Web/Controllers/AccountController.cs) gọi `IAccountRepository`.
- **Database Layer:** Gọi các stored procedure:
  - `sp_LayDanhSachTaiKhoan`, `sp_TaoTaiKhoan`, `sp_SuaTaiKhoan`, `sp_XoaTaiKhoan`.

### 4. Ràng Buộc Nghiệp Vụ (Business Rules & Constraints) - [CRITICAL]
- **Ràng buộc phân cấp (KHOA Privilege Limit):** Người dùng nhóm **KHOA** chỉ được phép tạo tài khoản có quyền **KHOA**. Nếu cố tình truyền quyền **PGV** lên từ client, Controller C# sẽ chặn đứng và báo lỗi: `"Bạn không có quyền tạo tài khoản nhóm này."`
- **Ràng buộc độc nhất:** Tên đăng nhập (SQL Login) không được trùng trên toàn máy chủ SQL Server (`sys.server_principals`) và Tên Database User không được trùng trong database (`sys.database_principals`).
- **Chặn tự xóa/tự sửa quyền:** Không thể tự xóa hoặc tự thay đổi vai trò (Role) của bản thân khi đang đăng nhập.
- **Chặn thao tác tài khoản đang online:** Stored procedure `sp_SuaTaiKhoan` và `sp_XoaTaiKhoan` kiểm tra bảng quản lý phiên hoạt động của hệ thống `sys.dm_exec_sessions`. Nếu tài khoản mục tiêu đang có kết nối hoạt động với database, SQL Server sẽ chặn đứng và ném lỗi: `"Tài khoản [...] đang có phiên đăng nhập hoạt động. Không thể xóa/sửa lúc này!"`.
- **Bảo mật Server Role (`securityadmin`):** Khi tạo tài khoản có quyền **PGV** hoặc **KHOA**, SQL Server tự động add tài khoản đó vào server role `securityadmin`. Điều này cho phép tài khoản đó có đủ quyền quản trị ở cấp máy chủ để tạo SQL Login mới cho người khác.
- **An toàn SQL Injection:** Khi tạo/sửa tài khoản bằng T-SQL động (`sp_executesql`), giá trị mật khẩu gửi lên được escape kỹ càng (`REPLACE(@PASS, '''', '''''')`) để ngăn chặn việc chèn mã SQL độc hại.

---

## 7. Chức năng In ấn Báo cáo & Báo cáo Động (Reporting & Dynamic Query Builder)

### 1. Phân quyền (Actors & RBAC)
- **Xem bảng điểm cá nhân (Phiếu điểm):** Sinh viên (**SV**) được phép xem của chính mình. **PGV** và **KHOA** được phép xem của bất kỳ sinh viên nào.
- **In các báo cáo tổng hợp khác:** Chỉ **PGV** và **KHOA** được phép in (Bảng điểm lớp, danh sách sinh viên đăng ký, danh sách lớp tín chỉ mở, bảng điểm tổng kết). Sinh viên bị chặn hoàn toàn.

### 2. Luồng Nghiệp Vụ (Business Flow)
- **Báo cáo chuẩn:** Người dùng chọn loại báo cáo -> Chọn các tham số lọc -> Xem báo cáo trực quan trên trình duyệt sử dụng DevExpress Viewer và có thể in/tải PDF.
- **Báo cáo động (Dynamic Builder):** Người dùng kéo chọn bảng nghiệp vụ được phân quyền -> Chọn danh sách các cột -> Cấu hình các điều kiện lọc và nhóm -> Xem trước kết quả -> Tạo báo cáo PDF động.

### 3. Quy cách Nội dung & Cách sắp xếp Báo cáo (Report Formats & Sort Rules) - [CRITICAL]
Dưới đây là chi tiết các mẫu báo cáo chuẩn bắt buộc phải tuân thủ đúng yêu cầu của Đề tài:

| Tên Báo Cáo | Tham số đầu vào | Quy tắc Sắp xếp (Sort Rules) | Cấu trúc Cột / Layout hiển thị |
| :--- | :--- | :--- | :--- |
| **Danh sách lớp tín chỉ** | Niên khóa, Học kỳ, Khoa | Tăng dần theo **Tên môn học**, sau đó đến **Nhóm** | STT, Tên môn học, Nhóm, Họ tên GV giảng, Số SV tối thiểu, Số SV đã đăng ký.<br>*Tổng kết cuối trang:* "Số lượng lớp đã mở: ###" |
| **Danh sách sinh viên đăng ký lớp tín chỉ** | Niên khóa, Học kỳ, Môn học, Nhóm | Tăng dần theo **Tên + Họ** sinh viên | STT, Mã SV, Họ, Tên, Phái (Nam/Nữ), Mã lớp.<br>*Tổng kết cuối trang:* "Số sinh viên đã đăng ký: ###" |
| **Bảng điểm môn học của một lớp tín chỉ** | Niên khóa, Học kỳ, Môn học, Nhóm | Tăng dần theo **Tên + Họ** sinh viên | STT, Mã SV, Họ, Tên, Điểm chuyên cần, Điểm giữa kỳ, Điểm cuối kỳ, Điểm hết môn.<br>*Tổng kết cuối trang:* "Số sinh viên: ###" |
| **Phiếu Điểm** | Mã Sinh viên | Tăng dần theo **Tên môn học** | STT, Tên môn học, Điểm (Lấy **Điểm lớn nhất** của các lần thi/lần đăng ký môn học đó nếu sinh viên học lại). |
| **Bảng điểm tổng kết** | Mã Lớp chính quy | Tăng dần theo **Tên + Họ** sinh viên | Dạng **Cross-Tab (Pivot)**:<br>- Dòng (Rows): MASV - Họ tên<br>- Cột (Columns): Danh sách các Môn học phát sinh điểm của lớp đó.<br>- Giá trị (Cells): Điểm lớn nhất (Max) của sinh viên đó đối với môn học tương ứng. |

### 4. Luồng Kỹ Thuật (Technical Execution Flow)
- **UI/View:** DevExpress Web Document Viewer tích hợp trong [ReportController](../src/QLDSV_HTC.Web/Controllers/ReportController.cs) và giao diện kéo thả động trong [DynamicReportController](../src/QLDSV_HTC.Web/Controllers/DynamicReportController.cs).
- **Controller/Endpoint:** Các endpoint GET để kết xuất PDF như `/report/grades`, `/report/class-summary`, `/dynamic-report/preview`, v.v.
- **Service/Repository Layer:** `IReportRepository` và `IDynamicReportRepository` gọi truy vấn SQL.
- **Database Layer:** Chạy các stored procedure in ấn (`sp_LayPhieuDiem`, `sp_LayBangDiemTongKet`, `sp_LayDanhSachSinhVienDangKyLopTinChi`) và chạy các câu lệnh SQL động được tự sinh an toàn từ Repository.

### 5. Ràng Buộc Bảo Mật & Chống SQL Injection (Security Constraints) - [CRITICAL]
- **Chống lỗi IDOR đối với Sinh viên:** Khi sinh viên gửi yêu cầu xuất phiếu điểm cá nhân `/report/grades?studentId=...`, Controller C# thực hiện kiểm tra chéo: nếu vai trò là `SV`, hệ thống so sánh mã sinh viên yêu cầu với mã sinh viên trong Session. Nếu không trùng khớp, hệ thống từ chối và trả về lỗi **Forbid (403)**.
- **Quy tắc lấy điểm lớn nhất (Max Grade Rule):** Trong phiếu điểm cá nhân (`sp_LayPhieuDiem`), nếu sinh viên học lại một môn học nhiều lần, báo cáo chỉ hiển thị một dòng duy nhất cho môn học đó với số điểm cao nhất bằng cách sử dụng hàm xếp hạng:
  `ROW_NUMBER() OVER (PARTITION BY MAMH ORDER BY ISNULL(DIEM, -1) DESC) AS RowNum` và chỉ lấy các dòng có `RowNum = 1`.
- **Ràng buộc bảng của Báo cáo Động (Sandbox allowed tables):** Để chống SQL Injection và đọc trộm thông tin hệ thống thông qua truy vấn động, [DynamicReportRepository](../src/QLDSV_HTC.Infrastructure/Repositories/DynamicReportRepository.cs) áp dụng bộ lọc bảo mật nghiêm ngặt:
  1. Chỉ hiển thị các bảng nghiệp vụ có tiền tố sạch, không chứa bảng hệ thống (`TABLE_NAME NOT LIKE 'sys%'`).
  2. Bắt buộc kiểm tra quyền đọc của tài khoản hiện hành trên bảng đó: `HAS_PERMS_BY_NAME(..., 'OBJECT', 'SELECT') = 1`.
  3. Mối liên kết JOIN giữa các bảng được kiểm tra cứng thông qua danh bạ quan hệ định nghĩa sẵn tại [TableRelationRegistry.cs](../src/QLDSV_HTC.Domain/Constants/TableRelationRegistry.cs), ngăn chặn việc sinh ra các câu lệnh JOIN tự do gây treo hoặc tràn dữ liệu.
  4. Mọi giá trị lọc truyền lên từ giao diện đều được đẩy vào các đối tượng `SqlParameter` được đánh số tự động (`@p0`, `@p1`), tuyệt đối không ghép chuỗi để triệt tiêu lỗ hổng SQL Injection.

---

## 8. Sơ đồ Cấu trúc CSDL & Ràng buộc Chi tiết (Database Schema Catalog)

Dưới đây là chi tiết các bảng được cài đặt trong cơ sở dữ liệu chính [src/Database/Tables/QLDSV_HTC.sql](../src/Database/Tables/QLDSV_HTC.sql) cùng các ràng buộc check, default và các mối quan hệ (Foreign Key cascade).

### 1. Bảng KHOA (Khoa)
- **Primary Key:** `MAKHOA` nChar(10)
- **Cấu trúc cột:**
  - `MAKHOA` nChar(10) NOT NULL
  - `TENKHOA` nVarchar(50) NOT NULL
- **Ràng buộc:** `TENKHOA` phải là Unique (độc nhất).

### 2. Bảng LOP (Lớp học chính quy)
- **Primary Key:** `MALOP` nChar(10)
- **Cấu trúc cột:**
  - `MALOP` nChar(10) NOT NULL
  - `TENLOP` nVarchar(50) NOT NULL (Unique)
  - `KHOAHOC` nChar(9) NOT NULL (Ví dụ: `2015-2019`)
  - `MAKHOA` nChar(10) NOT NULL (Foreign Key liên kết tới `KHOA`)
- **Foreign Key:** `FK_LOP_KHOA` (`MAKHOA` -> `KHOA(MAKHOA)`) ON DELETE NO ACTION / ON UPDATE NO ACTION.

### 3. Bảng SINHVIEN (Sinh viên)
- **Primary Key:** `MASV` nChar(10)
- **Cấu trúc cột:**
  - `MASV` nChar(10) NOT NULL
  - `HO` nVarchar(50) NOT NULL
  - `TEN` nVarchar(10) NOT NULL
  - `PHAI` Bit NOT NULL DEFAULT (0) (0: Nam, 1: Nữ)
  - `DIACHI` nVarchar(100) NULL
  - `NGAYSINH` Date NULL (Ràng buộc tuổi: $\ge 16$ tuổi)
  - `MALOP` nChar(10) NOT NULL (Foreign Key liên kết tới `LOP`)
  - `DANGHIHOC` Bit NOT NULL DEFAULT (0) (0: Đang học, 1: Đã nghỉ học)
  - `PASSWORD` nVarchar(40) NULL DEFAULT ('123456')
- **Foreign Key:** `FK_SINHVIEN_LOP` (`MALOP` -> `LOP(MALOP)`) **ON UPDATE CASCADE** (Nếu cập nhật lại mã lớp chính quy, thông tin lớp của sinh viên tự động cập nhật theo).

### 4. Bảng MONHOC (Môn học)
- **Primary Key:** `MAMH` nChar(10)
- **Cấu trúc cột:**
  - `MAMH` nChar(10) NOT NULL
  - `TENMH` nVarchar(50) NOT NULL (Unique)
  - `SOTIET_LT` Int NOT NULL (Ràng buộc $\ge 0$)
  - `SOTIET_TH` Int NOT NULL (Ràng buộc $\ge 0$)

### 5. Bảng GIANGVIEN (Giảng viên)
- **Primary Key:** `MAGV` nChar(10)
- **Cấu trúc cột:**
  - `MAGV` nChar(10) NOT NULL
  - `MAKHOA` nChar(10) NOT NULL (Foreign Key liên kết tới `KHOA`)
  - `HO` nVarchar(50) NOT NULL
  - `TEN` nVarchar(10) NOT NULL
  - `HOCVI` nVarchar(20) NULL
  - `HOCHAM` nVarchar(20) NULL
  - `CHUYENMON` nVarchar(50) NULL
- **Foreign Key:** `FK_GIANGVIEN_KHOA` (`MAKHOA` -> `KHOA(MAKHOA)`)

### 6. Bảng LOPTINCHI (Lớp tín chỉ mở)
- **Primary Key:** `MALTC` Int IDENTITY(1,1) NOT NULL (Tự động tăng)
- **Cấu trúc cột:**
  - `MALTC` Int NOT NULL
  - `NIENKHOA` nChar(9) NOT NULL (Ví dụ: `2021-2022`)
  - `HOCKY` Int NOT NULL (Ràng buộc Check: $1 \le HOCKY \le 3$)
  - `MAMH` nChar(10) NOT NULL (Foreign Key liên kết tới `MONHOC`)
  - `NHOM` Int NOT NULL (Ràng buộc Check: $NHOM \ge 1$)
  - `MAGV` nChar(10) NOT NULL (Foreign Key liên kết tới `GIANGVIEN`)
  - `MAKHOA` nChar(10) NOT NULL (Foreign Key liên kết tới `KHOA`)
  - `SOSVTOITHIEU` Int NOT NULL
  - `HUYLOP` Bit NOT NULL DEFAULT (0) (0: Bình thường, 1: Đã hủy lớp)
- **Unique Constraint:** `UK_LOPTINCHI` trên các cột (`NIENKHOA`, `HOCKY`, `MAMH`, `NHOM`).
- **Check Constraint:** `CK_SOSVTOITHIEU` (`SOSVTOITHIEU > 0`).
- **Foreign Keys:**
  - `FK_LOPTINCHI_GIANGVIEN` (`MAGV` -> `GIANGVIEN(MAGV)`) **ON UPDATE CASCADE**
  - `FK_LOPTINCHI_MONHOC` (`MAMH` -> `MONHOC(MAMH)`) **ON UPDATE CASCADE**
  - `FK_LOPTINCHI_KHOA` (`MAKHOA` -> `KHOA(MAKHOA)`)

### 7. Bảng DANGKY (Đăng ký & Điểm số)
- **Primary Key:** (`MALTC`, `MASV`)
- **Cấu trúc cột:**
  - `MALTC` Int NOT NULL (Foreign Key liên kết tới `LOPTINCHI`)
  - `MASV` nChar(10) NOT NULL (Foreign Key liên kết tới `SINHVIEN`)
  - `DIEM_CC` Int NULL (Check: $0 \le DIEM\_CC \le 10$)
  - `DIEM_GK` Float NULL (Check: $0 \le DIEM\_GK \le 10$)
  - `DIEM_CK` Float NULL (Check: $0 \le DIEM\_CK \le 10$)
  - `HUYDANGKY` Bit NULL DEFAULT (0) (0: Đăng ký hoạt động, 1: Đã hủy đăng ký lớp)
- **Unique Constraint:** `UK_DANGKY` trên (`MALTC`, `MASV`).
- **Check Constraints:**
  - `CK_DANGKY_DIEM_CC` (`DIEM_CC >= 0 AND DIEM_CC <= 10`)
  - `CK_DANGKY_DIEM_GK` (`DIEM_GK >= 0.0 AND DIEM_GK <= 10.0`)
  - `CK_DANGKY_DIEM_CK` (`DIEM_CK >= 0.0 AND DIEM_CK <= 10.0`)
- **Foreign Keys:**
  - `FK_CTLTC_SINHVIEN` (`MASV` -> `SINHVIEN(MASV)`) **ON UPDATE CASCADE**
  - `FK_DANGKY_LOPTINCHI` (`MALTC` -> `LOPTINCHI(MALTC)`) ON DELETE NO ACTION
