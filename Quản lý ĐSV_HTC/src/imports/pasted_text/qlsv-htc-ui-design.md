Design a modern desktop management UI for a university credit-based student management system (SQL Server project) named “QLDSV_HTC”.

STYLE:
- Clean, professional, academic, modern admin dashboard
- Suitable for university / education management software
- UI should feel trustworthy, organized, easy to use
- Similar to a modern ERP / admin panel
- Use a calm blue primary color (#2563EB or #3B82F6)
- Secondary color: white / light gray
- Accent color: teal / green for success
- Red for destructive actions
- Yellow / orange for warnings
- Soft shadows, subtle borders
- Rounded corners 12px–16px
- Spacious layout, clear hierarchy
- Minimal but polished

TYPOGRAPHY:
- Font: Inter or Be Vietnam Pro
- Large page title: 28px bold
- Section title: 20px semibold
- Table header: 14px semibold
- Body text: 14px–16px regular
- Small caption: 12px
- Good readability for long forms and data tables

LAYOUT:
- Desktop web app / admin dashboard
- Frame size: 1440x1024
- Left sidebar navigation
- Top header with page title, user info, role badge, logout button
- Main content area with cards, forms, filters, tables, tabs, and modals
- Use 8px spacing system
- Horizontal page padding: 24px to 32px
- Use auto layout principles
- Responsive admin structure

MAIN NAVIGATION (SIDEBAR):
Create a left sidebar with icons and labels:
- Dashboard
- Lớp
- Sinh viên
- Môn học
- Giảng viên
- Lớp tín chỉ
- Đăng ký tín chỉ
- Nhập điểm
- Báo cáo / In ấn
- Tài khoản / Phân quyền

Add role-based visual badges:
- PGV
- KHOA
- SV

SCREENS TO DESIGN:

1. LOGIN SCREEN
Create a clean academic login screen with:
- University logo placeholder
- System name: “QUẢN LÝ ĐIỂM SINH VIÊN THEO HỆ TÍN CHỈ”
- Role switch or role hint:
  + Giảng viên / Khoa / PGV login
  + Sinh viên login
- Input fields:
  + Login / Mã SV
  + Password
- Login button
- Optional “remember me”
- Subtle illustration related to education / database / campus
- Centered card layout
- Light gradient or soft academic background

2. DASHBOARD SCREEN
Create an admin dashboard overview with:
- Header: page title + breadcrumb
- Welcome card with current user info:
  + Name
  + Role
  + Khoa
- Statistic cards:
  + Tổng số sinh viên
  + Tổng số lớp
  + Tổng số môn học
  + Tổng số lớp tín chỉ
  + Lớp tín chỉ đang mở
  + Sinh viên đang đăng ký
- Quick action buttons:
  + Thêm lớp
  + Thêm sinh viên
  + Mở lớp tín chỉ
  + Nhập điểm
  + Tạo tài khoản
- Recent activity table
- Small chart / bar chart for class registration stats
- Academic / management aesthetic

3. CLASS MANAGEMENT SCREEN (LỚP)
Design a CRUD management page for class list:
- Page title: “Quản lý lớp”
- Action buttons:
  + Thêm
  + Xóa
  + Ghi
  + Phục hồi
  + Thoát
- Search bar
- Department filter (Khoa)
- Main table with columns:
  + Mã lớp
  + Tên lớp
  + Khóa học
  + Mã khoa
- Right side or modal form for editing:
  + Mã lớp
  + Tên lớp
  + Khóa học
  + Khoa
- Show disabled states for unauthorized roles

4. STUDENT MANAGEMENT SCREEN (SINH VIÊN)
Design a 2-level management UI:
- Top section: class selector
- Middle section: student list table
- Bottom / side form: student details
Include:
- Action buttons:
  + Thêm
  + Xóa
  + Ghi
  + Phục hồi
  + Thoát
- Filters:
  + Khoa
  + Lớp
- Table columns:
  + Mã SV
  + Họ
  + Tên
  + Phái
  + Ngày sinh
  + Địa chỉ
  + Đang nghỉ học
- Student detail form:
  + Mã SV
  + Họ
  + Tên
  + Phái (radio)
  + Ngày sinh
  + Địa chỉ
  + Password
  + Trạng thái nghỉ học
- Use a clean academic table layout

5. SUBJECT MANAGEMENT SCREEN (MÔN HỌC)
Create a clean subject management screen:
- Action buttons:
  + Thêm
  + Xóa
  + Ghi
  + Phục hồi
  + Thoát
- Subject table with columns:
  + Mã môn học
  + Tên môn học
  + Số tiết LT
  + Số tiết TH
- Subject form panel
- Search / filter by subject name
- Clean card-based admin layout

6. GIẢNG VIÊN MANAGEMENT SCREEN
Create a lecturer management screen:
- Table columns:
  + Mã GV
  + Họ
  + Tên
  + Học vị
  + Học hàm
  + Chuyên môn
  + Khoa
- Side form for lecturer info
- Standard CRUD action bar
- Clean admin layout with professional feel

7. OPEN CREDIT CLASS SCREEN (LỚP TÍN CHỈ)
Design a management screen for opening credit classes:
- Page title: “Mở lớp tín chỉ”
- Filters:
  + Niên khóa
  + Học kỳ
  + Khoa
- Action buttons:
  + Thêm
  + Xóa
  + Ghi
  + Phục hồi
- Table columns:
  + Mã LTC
  + Niên khóa
  + Học kỳ
  + Mã môn học
  + Nhóm
  + Giảng viên
  + Khoa
  + Số SV tối thiểu
  + Hủy lớp
- Form fields:
  + Niên khóa
  + Học kỳ
  + Môn học
  + Nhóm
  + Giảng viên
  + Khoa
  + Số SV tối thiểu
  + Hủy lớp checkbox
- Use dropdowns and number inputs
- Include status badge: Đang mở / Đã hủy

8. STUDENT COURSE REGISTRATION SCREEN (ĐĂNG KÝ TÍN CHỈ)
Create a student-friendly registration screen:
- Header with student info card:
  + Mã SV
  + Họ tên
  + Mã lớp
- Filter bar:
  + Niên khóa
  + Học kỳ
  + Search môn học
- Main registration table:
  + Mã MH
  + Tên môn học
  + Nhóm
  + Giảng viên
  + Số SV đã đăng ký
  + Action (Đăng ký / Hủy đăng ký)
- Use green/blue buttons for registration
- Show disabled / full / unavailable states
- Add a small right-side panel:
  + “Môn đã chọn”
  + số lượng lớp đã đăng ký
- Student UX should be simpler and cleaner than admin pages

9. GRADE ENTRY SCREEN (NHẬP ĐIỂM)
Design a very important data-entry screen for grades:
- Filter section at top:
  + Niên khóa
  + Học kỳ
  + Môn học
  + Nhóm
  + Nút “Bắt đầu”
- Below that: grade input table
Columns:
  + Mã SV
  + Họ tên SV
  + Điểm chuyên cần
  + Điểm giữa kỳ
  + Điểm cuối kỳ
  + Điểm hết môn
- Mark:
  + Mã SV = read only
  + Họ tên = read only
  + Điểm hết môn = auto-calculated, read only
- Add visual formula note:
  “Điểm hết môn = CC * 0.1 + GK * 0.3 + CK * 0.6”
- Add bottom action buttons:
  + Ghi điểm
  + Làm mới
  + Xuất file
- Use spreadsheet-like editable table design
- This screen should feel highly usable and data-entry optimized

10. REPORTS / PRINTING SCREEN (IN ẤN / BÁO CÁO)
Create a reporting center page with report cards and filters.
Include report categories:
- Danh sách lớp tín chỉ
- Danh sách sinh viên đăng ký lớp tín chỉ
- Bảng điểm môn học
- Phiếu điểm sinh viên
- Bảng điểm tổng kết cuối khóa
For each report card:
- title
- short description
- icon
- “Xem trước”
- “In PDF”
- “Xuất Excel”
At the top or side, include dynamic filter panel:
- Niên khóa
- Học kỳ
- Môn học
- Nhóm
- Mã lớp
- Mã sinh viên
Create one preview panel example showing a printable report layout:
- table
- school title
- report title
- filter summary
- footer/signature area

11. ACCOUNT / PERMISSION MANAGEMENT SCREEN
Design a role and account management screen:
- Page title: “Quản trị tài khoản”
- Tabs:
  + Danh sách tài khoản
  + Tạo tài khoản
  + Phân quyền
- Account table columns:
  + Login
  + Họ tên
  + Vai trò
  + Khoa
  + Trạng thái
- Create account form:
  + Login
  + Password
  + Confirm password
  + Chọn vai trò (PGV / KHOA / SV)
  + Chọn khoa
- Role permission matrix card:
  Show which role can access which modules
- Use lock / shield icons for permissions

12. MODAL / COMPONENT LIBRARY PAGE
Create a separate component sheet with reusable UI:
- Buttons:
  + primary
  + secondary
  + danger
  + disabled
- Inputs:
  + text input
  + password input
  + dropdown
  + date picker
  + checkbox
  + radio
- Table row states:
  + default
  + selected
  + hover
- Status badges:
  + Active
  + Disabled
  + Cancelled
  + Pending
- Modal dialogs:
  + confirm delete
  + save success
  + access denied
- Toast notifications:
  + success
  + error
  + warning

UX / VISUAL RULES:
- Make all forms aligned and practical for real data entry
- Use tables heavily because this is a management system
- Keep spacing consistent
- Use clear labels in Vietnamese
- Use modern icon set (academic / admin / database)
- Make it look like a real graduation project UI, polished enough for presentation and implementation
- Avoid overly playful mobile style; this should look like a serious but modern student management platform
- Use realistic Vietnamese labels everywhere
- Design should be visually impressive for thesis / project defense