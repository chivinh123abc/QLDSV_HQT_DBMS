# Tóm Tắt Công Việc — Branch `fix/final_fix`

> 📅 Ngày: 22/06/2026  
> 📊 Tổng: **56 files** thay đổi | **+2943 dòng** thêm | **-805 dòng** xóa

---

## 1. Kiến Trúc — Chuyển Từ `sa` Sang User Login Riêng

### Trước đó
- Toàn bộ hệ thống dùng chung tài khoản `sa` (admin SQL Server) cho mọi thao tác.
- Không phân biệt ai đang thao tác, vi phạm yêu cầu đề bài.

### Sau khi sửa
- Refactor `AccountRepository` sử dụng `BaseSqlRepository` với `IDbConnectionProvider`.
- Mỗi giảng viên đăng nhập bằng tài khoản SQL Server riêng (VD: `GV01`, `GV02`).
- Connection string lưu theo từng phiên đăng nhập.
- `RegistrationRepository` dùng admin connection riêng cho SV đăng ký (vì SV dùng chung login `sv`).

### Files thay đổi
- `AccountRepository.cs` — dùng user connection thay vì `sa`
- `GradeRepository.cs`, `CreditClassRepository.cs`, `LecturerRepository.cs`, `SubjectRepository.cs`, `RegistrationRepository.cs` — cập nhật tương ứng

---

## 2. Phân Quyền 3 Nhóm — PGV / KHOA / SV

### Bảng phân quyền chi tiết

| Chức năng | PGV | KHOA | SV |
|---|:---:|:---:|:---:|
| Tạo tài khoản PGV | ✅ | ❌ | ❌ |
| Tạo tài khoản KHOA | ✅ | ✅ | ❌ |
| Sửa tài khoản | ✅ | ✅ (chỉ KHOA) | ❌ |
| Xóa tài khoản | ✅ | ❌ | ❌ |
| Quản lý Khoa | ✅ | ❌ | ❌ |
| Quản lý Lớp | ✅ | ❌ | ❌ |
| Quản lý Giảng viên | ✅ | ❌ | ❌ |
| Quản lý Sinh viên | ✅ | ❌ | ❌ |
| Quản lý Môn học | ✅ | ❌ | ❌ |
| Mở Lớp tín chỉ | ✅ | ❌ | ❌ |
| Nhập điểm | ✅ (tất cả khoa) | ✅ (chỉ khoa mình) | ❌ |
| Đăng ký LTC | ❌ | ❌ | ✅ |
| Xem phiếu điểm | ✅ | ✅ | ✅ |
| In báo cáo | ✅ | ✅ | ❌ |

### Bảo vệ 2 lớp
- **Lớp 1 — Code (Controller)**: Dùng `[Authorize(Roles = ...)]` trên từng action, kiểm tra role trước khi thực hiện.
- **Lớp 2 — SQL Server**: REVOKE quyền EXECUTE trên `sp_XoaTaiKhoan` cho KHOA. SP dùng `WITH EXECUTE AS OWNER` để KHOA tạo tài khoản mà không cần cấp `db_owner`.

### Quy tắc đặc biệt
- Không ai tự xóa tài khoản chính mình.
- Không ai tự đổi quyền (role) chính mình.
- Tài khoản mới tạo tự động được grant quyền tương ứng với role.

### Files thay đổi
- `001-PhanQuyen.sql` — Grant/Revoke quyền SQL Server
- `AccountController.cs` — Kiểm tra role trước CRUD
- `StoredProcedureConstants.cs`, `RouteConstants.cs` — Thêm constants mới

---

## 3. Quản Lý Tài Khoản — Undo/Redo + Force Logout

### Undo/Redo (Phục hồi / Hoàn tác)
- Khi sửa hoặc xóa tài khoản, lưu trạng thái cũ vào `undoStack`.
- Bấm "Phục hồi" → khôi phục lại trạng thái trước đó.

### Force Logout Modal
- **Trước**: Khi admin sửa tài khoản đang online → tự động đăng xuất user bằng `setTimeout` (user không kịp biết chuyện gì xảy ra).
- **Sau**: Hiện modal chặn (không thể đóng bằng click ngoài hay Esc) với thông báo rõ ràng → user phải bấm "Đăng xuất" → mới redirect về trang login.

### Mật khẩu mặc định
- Khi tạo tài khoản mới, mật khẩu mặc định là `12345678`.
- Có nút toggle hiển thị/ẩn mật khẩu (eye icon).

### Files thay đổi
- `_AccountTable.cshtml` — Thêm force logout modal, undo/redo logic
- `_AccountFormModal.cshtml` — Toggle password, default password

---

## 4. Quản Lý Điểm — Nhập Điểm + Bảng Điểm

### Nhập điểm
- PGV nhập điểm tất cả khoa, KHOA chỉ nhập điểm khoa mình.
- Validation: điểm từ 0-10, check trước khi lưu.
- Công thức: Điểm hết môn = Chuyên cần × 0.1 + Giữa kỳ × 0.3 + Cuối kỳ × 0.6.
- Điểm hết môn hiển thị read-only, tự tính khi nhập.

### Cải thiện error handling
- Đổi `catch (Exception)` → `catch (SqlException)` cho chính xác hơn.
- Dùng `SqlErrorHelper.GetFriendlyMessage()` hiển thị lỗi tiếng Việt.

### Files thay đổi
- `GradeController.cs` — Filter theo khoa, fix catch, thêm SaveGrades
- `GradeRepository.cs` — Cập nhật UpdateGradesAsync
- `IGradeRepository.cs` — Cập nhật interface
- `grade/index.js` — JS nhập điểm, tính điểm tự động

---

## 5. Quản Lý Lớp Tín Chỉ

### CRUD đầy đủ
- Thêm / Sửa / Xóa lớp tín chỉ (chỉ PGV).
- Lọc theo niên khóa, học kỳ, khoa.
- Unique constraint: Niên khóa + Học kỳ + Mã MH + Nhóm.

### Fix bug hiển thị
- `FacultyName = d.FacultyId` → Dùng `facultyLookup` dictionary tra cứu tên khoa từ mã.
- `ex.Message` → `SqlErrorHelper.GetFriendlyMessage()` cho 3 catch blocks.

### Files thay đổi
- `CreditClassController.cs` — Fix FacultyName, SqlErrorHelper
- `CreditClassRepository.cs` — Cập nhật repository
- `CreditClass/Index.cshtml`, `_CreditClassFormModal.cshtml`, `_CreditClassTable.cshtml` — Views

---

## 6. Đăng Ký Lớp Tín Chỉ (Sinh Viên + Admin)

### SV đăng ký
- Sinh viên đăng nhập bằng login chung `sv` + mã sinh viên.
- Xem danh sách LTC khả dụng, đăng ký / hủy đăng ký.

### Admin đăng ký (NEW)
- Tạo mới `AdminRegistrationController` + Views cho PGV/KHOA quản lý đăng ký.
- PGV xem tất cả, KHOA chỉ xem khoa mình.

### Files thay đổi
- `RegistrationController.cs` — Controller SV
- `AdminRegistrationController.cs` — **[MỚI]** Controller admin
- `AdminRegistrationViewModel.cs` — **[MỚI]** ViewModel
- `AdminRegistration/Index.cshtml`, `_AdminRegistrationTable.cshtml` — **[MỚI]** Views
- `Registration/Index.cshtml` — Cập nhật view SV
- `RegistrationRepository.cs` — Dùng admin connection cho SV
- `AvailableCreditClassDto.cs` — Thêm field mới

---

## 7. Cải Thiện Giao Diện Toàn Bộ Views

### Shared Components
- `_CrudToolbar.cshtml` — Toolbar chung (Thêm, Xóa, Ghi, Phục hồi, Thoát).
- `_FormModalWrapper.cshtml` — Modal wrapper dùng chung cho tất cả form.

### Các trang đã cập nhật

| Trang | Thay đổi chính |
|---|---|
| **Khoa** | Cải thiện layout, form modal |
| **Lớp** | Master-detail 2 cấp (Lớp → DSSV), form modal |
| **Sinh viên** | Cải thiện bảng, search/filter |
| **Giảng viên** | Thêm field khoa, cải thiện form |
| **Môn học** | CRUD đầy đủ, validation |
| **Lớp tín chỉ** | Lọc theo niên khóa/HK/khoa, form modal |
| **Điểm** | Nhập điểm inline, tính tự động |
| **Đăng ký** | SV đăng ký/hủy, admin quản lý |
| **Báo cáo** | 5 loại báo cáo, auto-fill năm mới nhất |

### Files thay đổi
- 25+ view files (`.cshtml`)
- `site.css` — **+557 dòng** CSS mới (styles, responsive, animations)
- `SidebarService.cs` — Thêm menu AdminRegistration

---

## 8. Database — Stored Procedures + Indexes

### Stored Procedures đã sửa
- `007-sp_LayBangDiemMonHocCuaMotLopTinChi.sql` — Fix truy vấn bảng điểm
- `010-sp_CRUD_Lop.sql` — Fix CRUD lớp
- `014-sp_CRUD_MonHoc.sql` — Fix CRUD môn học

### Indexes
- `002-Indexes.sql` — Thêm indexes tối ưu truy vấn cho các bảng thường xuyên filter/join.

### Phân quyền SQL
- `001-PhanQuyen.sql` — Grant `ALTER ANY USER`, `ALTER ANY ROLE`, `EXECUTE` cho KHOA/PGV. Revoke `sp_XoaTaiKhoan` cho KHOA.

---

## 9. Application Layer — DTOs, Helpers, Interfaces

### DTOs đã cập nhật
- `GradeEntryDto.cs` — Thêm field cho nhập điểm
- `LecturerDto.cs` — Thêm `FacultyName`
- `SubjectDto.cs` — Thêm field mới
- `AvailableCreditClassDto.cs` — Thêm field cho đăng ký LTC

### Helpers
- `DateTimeHelper.cs` — Cải thiện logic niên khóa/học kỳ

### Constants
- `RouteConstants.cs` — Thêm 17 routes mới (AdminRegistration, Grade, Subject...)
- `StoredProcedureConstants.cs` — Thêm 2 SP constants mới
