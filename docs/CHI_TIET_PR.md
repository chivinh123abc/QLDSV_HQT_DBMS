# Chi Tiết Các Pull Request

> Branch gốc: `fix/final_fix` → Tách thành 5 PRs nối chuỗi
> Tổng: 56 files | +2943 dòng | -805 dòng | 18 lỗi đã fix

---

## PR 1: `fix/2-5-7-13-14-15-16-18-backend-phan-quyen` → `master`

**Title:** fix: #2 #5 #7 #13 #14 #15 #16 #18 - Backend, phân quyền và quản lý tài khoản

**~830 dòng thay đổi | 28 files**

### Các lỗi đã fix

| # | Lỗi | Mô tả chi tiết |
|---|---|---|
| #2 | Khóa cái xóa lại khi không thể xóa | KHOA không được xóa tài khoản → `[Authorize(Roles = PGV)]` trên Delete action + REVOKE EXECUTE `sp_XoaTaiKhoan` cho role KHOA ở SQL Server |
| #5 | 1 môn học 2 nhóm trùng nhau | Unique constraint trên LOPTINCHI: `NIENKHOA + HOCKY + MAMH + NHOM`. SP check trùng trước khi INSERT |
| #7 | Kiểm tra 1 GV dạy 2 nhóm ở 1 niên khóa học kỳ khác nhóm | SP validate: 1 GV chỉ được dạy nhiều nhóm nếu khác niên khóa/học kỳ |
| #13 | Không hiện lại GV đã có tài khoản | Khi tạo tài khoản mới, dropdown GV loại bỏ những GV đã có tài khoản |
| #14 | Đăng ký tài khoản thông báo đang bị sai | Fix error message: hiện thông báo lỗi chính xác từ `SqlErrorHelper.GetFriendlyMessage()` thay vì raw SQL error |
| #15 | Bắt lỗi 1 GV không có 2 tài khoản | SP `sp_TaoTaiKhoan` check trùng login trước khi CREATE USER. Controller cũng validate |
| #16 | Login name trùng với server mà không trùng với tên ở ngoài | Fix mapping login name → display name. Đảm bảo UserName hiển thị đúng |
| #18 | Khóa được tự sửa thông tin cá nhân | AccountController cho phép KHOA sửa thông tin cá nhân (tên, họ) nhưng không được đổi role |

### Files thay đổi

**Database (5 files)**
- `001-PhanQuyen.sql` — Grant `ALTER ANY USER`, `ALTER ANY ROLE`, `EXECUTE` cho KHOA/PGV. Revoke `sp_XoaTaiKhoan` cho KHOA
- `002-Indexes.sql` — Thêm indexes tối ưu truy vấn (FK columns, filter columns)
- `007-sp_LayBangDiemMonHocCuaMotLopTinChi.sql` — Fix truy vấn bảng điểm hết môn
- `010-sp_CRUD_Lop.sql` — Fix CRUD lớp
- `014-sp_CRUD_MonHoc.sql` — Fix CRUD môn học

**Domain (2 files)**
- `RouteConstants.cs` — Thêm 17 routes mới (AdminRegistration, Grade, Subject...)
- `StoredProcedureConstants.cs` — Thêm 2 SP constants mới

**Application (5 files)**
- `AvailableCreditClassDto.cs` — Thêm field cho LTC khả dụng
- `GradeEntryDto.cs` — Thêm field cho nhập điểm
- `LecturerDto.cs` — Thêm `FacultyName`
- `SubjectDto.cs` — Thêm field mới
- `DateTimeHelper.cs` — Cải thiện logic niên khóa/học kỳ

**Application Interfaces (1 file)**
- `IGradeRepository.cs` — Cập nhật interface điểm

**Infrastructure (5 files)**
- `GradeRepository.cs` — Cập nhật UpdateGradesAsync
- `RegistrationRepository.cs` — Dùng admin connection cho SV đăng ký
- `CreditClassRepository.cs` — Cập nhật repository LTC
- `LecturerRepository.cs` — Cập nhật repository GV
- `SubjectRepository.cs` — Cập nhật repository MH

**Controllers (5 files)**
- `AccountController.cs` — Phân quyền KHOA (chỉ tạo KHOA, không xóa), check self-update, fix #13 #14 #15 #16 #18
- `CreditClassController.cs` — Fix `FacultyName = d.FacultyId` → lookup đúng tên khoa. Dùng `SqlErrorHelper`
- `GradeController.cs` — Filter điểm theo khoa cho KHOA, thêm SaveGrades, fix `catch(Exception)` → `catch(SqlException)`
- `LecturerController.cs` — Thêm filter khoa
- `SubjectController.cs` — Cập nhật CRUD

**Models (4 files)**
- `AdminRegistrationController.cs` — **[MỚI]** Controller admin đăng ký LTC
- `AccountViewModel.cs` — Xóa field không dùng
- `LecturerViewModel.cs` — Thêm field khoa
- `SubjectViewModel.cs` — Thêm field mới

**Services (1 file)** — Không có trong PR này

---

## PR 2: `fix/1-3-4-6-17-form-nien-khoa-thong-bao` → `fix/2-5-7-...`

**Title:** fix: #1 #3 #4 #6 #17 - Form, ngày sinh, niên khóa, thông báo lỗi

**~970 dòng thay đổi | 10 files**

### Các lỗi đã fix

| # | Lỗi | Mô tả chi tiết |
|---|---|---|
| #1 | Sửa lại khi đóng form | Fix modal form: khi đóng form (X hoặc Esc), reset đúng trạng thái các fields, không bị giữ data cũ |
| #3 | Ngày tháng năm sinh kiểm tra (tối thiểu 16 tuổi) | Validation JS: ngày sinh phải ≥ 16 tuổi tính từ năm hiện tại. Hiển thị lỗi nếu không đủ tuổi |
| #4 | Max niên khóa 21-22 HK2, không mở LTC về quá khứ | DateTimeHelper tự tính niên khóa hiện tại. Dropdown chỉ cho chọn từ hiện tại trở đi |
| #6 | Khi mở LTC mới, nên giữ lại niên khóa học kỳ | Sau khi thêm LTC thành công, form giữ nguyên niên khóa + học kỳ đã chọn, không reset |
| #17 | Sửa thông báo khi tạo lỗi xong lại tạo thành công → vẫn hiện bảng đỏ | Fix toast: clear thông báo lỗi cũ trước khi hiện thông báo mới. Không stack thông báo |

### Files thay đổi

**Shared Components (2 files)**
- `_CrudToolbar.cshtml` — Cải thiện toolbar chung (Thêm, Xóa, Ghi, Phục hồi, Thoát)
- `_FormModalWrapper.cshtml` — Fix modal wrapper: reset form đúng khi đóng

**Account Views (2 files)**
- `_AccountFormModal.cshtml` — Fix đóng form reset fields, toggle password eye icon
- `_AccountTable.cshtml` — Force logout modal (chặn, phải bấm OK), undo/redo, fix thông báo lỗi #17

**Class Views (1 file)**
- `Class/Index.cshtml` — Validation ngày sinh ≥16 tuổi (#3), cải thiện master-detail layout

**CreditClass Views (3 files)**
- `CreditClass/Index.cshtml` — Giữ niên khóa/HK khi mở LTC mới (#6), không cho chọn quá khứ (#4)
- `_CreditClassFormModal.cshtml` — Fix form modal LTC
- `_CreditClassTable.cshtml` — Đổi label button "Cập nhật Lớp tín chỉ"

---

## PR 3: `fix/12-doi-ten-gv-views` → `fix/1-3-4-6-17-...`

**Title:** fix: #12 - Đổi tên cột GV và cập nhật views Khoa, Điểm, Giảng viên

**~500 dòng thay đổi | 5 files**

### Các lỗi đã fix

| # | Lỗi | Mô tả chi tiết |
|---|---|---|
| #12 | Cột GV liên kết đổi thành GiangVien | Đổi tên cột hiển thị từ "GV" thành "Giảng Viên" trong tất cả bảng liên quan |

### Files thay đổi

**Faculty Views (2 files)**
- `Faculty/Index.cshtml` — Cải thiện layout trang quản lý khoa
- `_FacultyFormModal.cshtml` — Fix form modal khoa

**Grade Views (1 file)**
- `Grade/Index.cshtml` — Nhập điểm inline, cột GV → Giảng Viên, tính điểm hết môn tự động

**Lecturer Views (2 files)**
- `Lecturer/Index.cshtml` — Đổi header cột GV → Giảng Viên, cải thiện layout
- `_LecturerFormModal.cshtml` — Fix form modal giảng viên, thêm field khoa

---

## PR 4: `fix/8-9-10-11-dang-ky-ltc` → `fix/12-doi-ten-gv-views`

**Title:** fix: #8 #9 #10 #11 - Đăng ký LTC, AdminRegistration, các views còn lại

**~1200 dòng thay đổi | 11 files**

### Các lỗi đã fix

| # | Lỗi | Mô tả chi tiết |
|---|---|---|
| #8 | Cho phép PGV đăng ký hộ | Tạo mới `AdminRegistrationController` + Views. PGV/KHOA có thể đăng ký LTC hộ SV |
| #9 | Chọn niên khóa đăng ký tự show ra, không cho chọn quá khứ | Tự fill niên khóa/HK hiện tại, disable dropdown không cho chọn ngược về quá khứ |
| #10 | Đăng ký 2 nhóm cùng 1 HK phải khác môn | Validation: trong 1 niên khóa + HK, SV không được đăng ký 2 nhóm cùng 1 môn học |
| #11 | Đăng ký vào 1 lớp đã có điểm thì không cho đăng ký | Check: nếu SV đã thi (có điểm) môn đó trong HK đó → không cho đăng ký lại |

### Files thay đổi

**Lecturer Views (1 file)**
- `_LecturerTable.cshtml` — Cải thiện bảng giảng viên

**Registration Views (1 file)**
- `Registration/Index.cshtml` — Tự fill niên khóa (#9), validation không đăng ký quá khứ, check trùng môn (#10), check có điểm (#11)

**Report Views (1 file)**
- `Report/Index.cshtml` — Thêm auto-fill năm mới nhất trong modal in

**Student Views (1 file)**
- `Student/Index.cshtml` — Cải thiện layout, search/filter

**Subject Views (2 files)**
- `Subject/Index.cshtml` — CRUD đầy đủ, cải thiện layout
- `_SubjectFormModal.cshtml` — Fix form modal môn học

**AdminRegistration — [MỚI] (4 files)**
- `AdminRegistrationViewModel.cs` — **[MỚI]** ViewModel cho admin đăng ký
- `AdminRegistration/Index.cshtml` — **[MỚI]** Trang admin đăng ký LTC cho SV
- `_AdminRegistrationTable.cshtml` — **[MỚI]** Bảng danh sách đăng ký
- `_SubjectTable.cshtml` — Cập nhật bảng môn học

**Services (1 file)**
- `SidebarService.cs` — Thêm menu "Đăng ký hộ" vào sidebar cho PGV/KHOA

---

## PR 5: `fix/final_fix` → `fix/8-9-10-11-dang-ky-ltc`

**Title:** fix: CSS/JS và tài liệu

**~810 dòng thay đổi | 3 files**

### Mô tả

Cập nhật giao diện toàn bộ ứng dụng (CSS) và logic nhập điểm (JS).

### Files thay đổi

**CSS (1 file)**
- `site.css` — **+557 dòng CSS mới**:
  - Styles cho tất cả bảng, form modal, toolbar
  - Responsive design (mobile/tablet/desktop)
  - Toast notification styles
  - Force logout modal styles
  - Print styles cho báo cáo
  - Hover effects, transitions, animations

**JS (1 file)**
- `grade/index.js` — Logic nhập điểm:
  - Tính điểm hết môn tự động (CC×0.1 + GK×0.3 + CK×0.6)
  - Validation điểm 0-10
  - Gửi điểm hàng loạt qua API

**Docs (1 file)**
- `TOM_TAT_CONG_VIEC.md` — Tóm tắt toàn bộ công việc đã làm

---

## Cách tạo PR trên GitHub

Mở từng link dưới đây → bấm **"Create pull request"** → paste title + body từ file này:

| PR | Link |
|---|---|
| PR 1 | https://github.com/chivinh123abc/QLDSV_HQT_DBMS/compare/master...fix/2-5-7-13-14-15-16-18-backend-phan-quyen |
| PR 2 | https://github.com/chivinh123abc/QLDSV_HQT_DBMS/compare/fix/2-5-7-13-14-15-16-18-backend-phan-quyen...fix/1-3-4-6-17-form-nien-khoa-thong-bao |
| PR 3 | https://github.com/chivinh123abc/QLDSV_HQT_DBMS/compare/fix/1-3-4-6-17-form-nien-khoa-thong-bao...fix/12-doi-ten-gv-views |
| PR 4 | https://github.com/chivinh123abc/QLDSV_HQT_DBMS/compare/fix/12-doi-ten-gv-views...fix/8-9-10-11-dang-ky-ltc |
| PR 5 | https://github.com/chivinh123abc/QLDSV_HQT_DBMS/compare/fix/8-9-10-11-dang-ky-ltc...fix/final_fix |
