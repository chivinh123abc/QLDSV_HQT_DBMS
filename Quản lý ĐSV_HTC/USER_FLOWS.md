# QLDSV_HTC - User Flow Documentation

## 📋 Overview
Tài liệu này mô tả chi tiết các user flow cho hệ thống QLDSV_HTC với 3 roles: Sinh viên (SV), Phòng Giáo Vụ (PGV), và Khoa (KHOA).

---

## 🔐 Authentication Flows

### Login Flow
```
1. User visits application
   ├─ Already logged in? → Redirect to Dashboard
   └─ Not logged in → Show Login Page
   
2. Login Page
   ├─ Select Role Type (Admin/Student)
   ├─ Enter Username/Password
   ├─ Remember me checkbox
   └─ Submit
   
3. Login Validation
   ├─ Success → Save session → Redirect to role-based Dashboard
   └─ Failed → Show error message
   
4. Forgot Password Link
   └─ Navigate to /forgot-password
```

### Forgot Password Flow
```
1. User clicks "Quên mật khẩu?" on Login Page
   └─ Navigate to Forgot Password Page

2. Step 1: Email/Student ID Input
   ├─ Enter email or student ID
   └─ Click "Gửi mã xác thực"
   
3. Step 2: OTP Verification
   ├─ Enter 6-digit OTP code
   ├─ Auto-focus to next input
   ├─ "Không nhận được mã? Gửi lại" button
   └─ Click "Xác nhận"
   
4. Step 3: Success
   ├─ Show success message
   ├─ "Mật khẩu mới đã được gửi đến email"
   └─ Click "Đăng nhập ngay" → Navigate to Login
   
5. Cancel/Back
   └─ "Quay lại đăng nhập" → Navigate to Login
```

### Logout Flow
```
1. User clicks "Đăng xuất" in Sidebar
   ├─ Clear session/localStorage
   └─ Redirect to Login Page
```

---

## 👨‍🎓 Student (SV) User Flows

### Initial Login
```
1. Login with sv/sv
   └─ Redirect to Student Dashboard (/)
   
2. Student Dashboard
   ├─ View "Lịch học hôm nay" (Today's Schedule)
   ├─ View "Thông báo" (Announcements)
   ├─ Quick Stats: GPA, Credits, Courses
   └─ Sidebar shows: Dashboard, Lịch học, Điểm, Đăng ký học phần, Thông báo
```

### View Schedule
```
1. Click "Lịch học" in Sidebar
   └─ Navigate to /schedule
   
2. Schedule Page
   ├─ Weekly view (Monday - Sunday)
   ├─ Time slots (7:00 - 17:00)
   ├─ Color-coded classes
   └─ Class details on hover/click:
       ├─ Course name
       ├─ Lecturer
       ├─ Room
       └─ Time
```

### View Grades
```
1. Click "Điểm" in Sidebar
   └─ Navigate to /student-grades
   
2. Grades Page
   ├─ Semester selector
   ├─ GPA display
   ├─ Grade table:
   │   ├─ Course Code
   │   ├─ Course Name
   │   ├─ Credits
   │   ├─ Midterm
   │   ├─ Final
   │   └─ Total
   └─ Grade statistics/charts
```

### Course Registration
```
1. Click "Đăng ký học phần" in Sidebar
   └─ Navigate to /registration
   
2. Registration Page
   ├─ Select semester/year
   ├─ View available courses
   ├─ Filter/search courses
   ├─ Check prerequisites
   ├─ View schedule conflicts
   ├─ Add courses to cart
   └─ Submit registration
   
3. Confirmation
   ├─ Review selected courses
   ├─ Check total credits
   ├─ Confirm registration
   └─ Success message
```

### View Notifications
```
1. Click "Thông báo" in Sidebar OR Bell icon in Header
   └─ Navigate to /notifications
   
2. Notifications Page
   ├─ Search notifications
   ├─ Filter: All / Unread
   ├─ Notification types:
   │   ├─ Grade updates
   │   ├─ Schedule changes
   │   ├─ Registration reminders
   │   └─ General announcements
   ├─ Mark as read
   ├─ Delete notifications
   ├─ Mark all as read
   └─ Delete all read
```

### Profile Management
```
1. Click Profile Avatar/Name in Header
   └─ Navigate to /profile
   
2. Profile Settings Page
   ├─ View/Edit personal info:
   │   ├─ Name
   │   ├─ Email
   │   ├─ Phone
   │   ├─ Date of Birth
   │   ├─ Gender
   │   ├─ Address
   │   └─ Student ID (read-only)
   ├─ Upload avatar
   ├─ Notification settings
   └─ "Đổi mật khẩu" button → Navigate to /change-password
```

### Change Password
```
1. From Profile Page, click "Đổi mật khẩu"
   └─ Navigate to /change-password
   
2. Change Password Page
   ├─ Enter current password
   ├─ Enter new password
   │   ├─ Password strength indicator
   │   └─ Requirements checklist:
   │       ├─ Min 8 characters
   │       ├─ Uppercase & lowercase
   │       ├─ Numbers
   │       └─ Special characters
   ├─ Confirm new password
   ├─ Validation errors
   └─ Submit
   
3. Success
   ├─ Show success message
   └─ Form reset
```

---

## 👨‍💼 Admin (PGV/KHOA) User Flows

### Initial Login
```
1. Login with pgv/pgv or khoa/khoa
   └─ Redirect to Admin Dashboard (/)
   
2. Admin Dashboard
   ├─ Welcome card with user info
   ├─ Statistics (6 cards):
   │   ├─ Total students
   │   ├─ Total classes
   │   ├─ Total subjects
   │   ├─ Total credit classes
   │   ├─ Open credit classes
   │   └─ Registered students
   ├─ Quick Actions buttons
   ├─ Registration chart by semester
   └─ Recent activities feed
```

### Class Management
```
1. Click "Lớp" in Sidebar
   └─ Navigate to /classes
   
2. Classes Page
   ├─ Search & filter classes
   ├─ Class list table
   ├─ Actions:
   │   ├─ Add new class
   │   ├─ Edit class
   │   ├─ Delete class
   │   └─ View class details
   └─ Pagination
```

### Student Management
```
1. Click "Sinh viên" in Sidebar
   └─ Navigate to /students
   
2. Students Page
   ├─ Search & filter students
   ├─ Student list table
   ├─ Actions:
   │   ├─ Add new student
   │   ├─ Edit student
   │   ├─ Delete student
   │   ├─ View student details
   │   └─ Import students (bulk)
   └─ Export to Excel
```

### Subject Management
```
1. Click "Môn học" in Sidebar
   └─ Navigate to /subjects
   
2. Subjects Page
   ├─ Search & filter subjects
   ├─ Subject list table
   ├─ Actions:
   │   ├─ Add new subject
   │   ├─ Edit subject
   │   ├─ Delete subject
   │   └─ View subject details
   └─ Export/Import
```

### Lecturer Management
```
1. Click "Giảng viên" in Sidebar
   └─ Navigate to /lecturers
   
2. Lecturers Page
   ├─ Search & filter lecturers
   ├─ Lecturer list table
   ├─ Actions:
   │   ├─ Add new lecturer
   │   ├─ Edit lecturer
   │   ├─ Delete lecturer
   │   └─ View lecturer details
   └─ Teaching schedule
```

### Credit Class Management
```
1. Click "Lớp tín chỉ" in Sidebar
   └─ Navigate to /credit-classes
   
2. Credit Classes Page
   ├─ Search & filter credit classes
   ├─ Filter by status: Open/Closed/Full
   ├─ Credit class list table
   ├─ Actions:
   │   ├─ Open new credit class
   │   ├─ Edit credit class
   │   ├─ Close registration
   │   ├─ View enrolled students
   │   └─ Delete credit class
   └─ Assign lecturer
```

### Grade Entry
```
1. Click "Nhập điểm" in Sidebar
   └─ Navigate to /grades
   
2. Grade Entry Page
   ├─ Select credit class
   ├─ Student list with grade columns
   ├─ Grade entry methods:
   │   ├─ Manual entry (individual)
   │   └─ Import from Excel (bulk)
   ├─ Save grades
   └─ Publish grades to students
```

### Reports & Printing
```
1. Click "Báo cáo / In ấn" in Sidebar
   └─ Navigate to /reports
   
2. Reports Page
   ├─ Select report type:
   │   ├─ Student grades
   │   ├─ Class statistics
   │   ├─ Registration statistics
   │   └─ Custom reports
   ├─ Set parameters/filters
   ├─ Preview report
   ├─ Actions:
   │   ├─ Print
   │   ├─ Export to PDF
   │   └─ Export to Excel
   └─ Save report template
```

### Account Management
```
1. Click "Tài khoản / Phân quyền" in Sidebar
   └─ Navigate to /accounts
   
2. Accounts Page
   ├─ User list table
   ├─ Actions:
   │   ├─ Create new account
   │   ├─ Edit account
   │   ├─ Delete account
   │   ├─ Reset password
   │   └─ Assign roles/permissions
   ├─ Role management:
   │   ├─ PGV (full access)
   │   ├─ KHOA (faculty access)
   │   └─ SV (student access)
   └─ User activity log
```

### Profile & Settings (Admin)
```
Same as Student Profile flow but with:
├─ Different default info
├─ No Student ID field
└─ Role-specific notification settings
```

---

## 🚨 Error & Edge Case Flows

### 404 Not Found
```
1. User navigates to invalid URL
   └─ Show 404 Page
   
2. 404 Page Options
   ├─ Search box
   ├─ "Quay lại trang trước" button
   ├─ "Về trang chủ" button
   ├─ Quick links to common pages
   └─ Help/support link
```

### Session Timeout
```
1. User session expires after inactivity
   └─ Redirect to Login Page
   
2. Login Page
   └─ Show message: "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại."
```

### Permission Denied
```
1. Student tries to access admin route (e.g., /students)
   └─ Options:
       ├─ Show 404 (current behavior)
       ├─ OR Redirect to Dashboard
       └─ OR Show "Unauthorized" page (future enhancement)
```

### Network Error
```
1. API call fails
   └─ Show error message
   
2. User Options
   ├─ Retry button
   └─ Contact support
```

### Form Validation Errors
```
1. User submits form with invalid data
   ├─ Highlight error fields
   ├─ Show error messages
   └─ Prevent submission
   
2. Common Validations
   ├─ Required fields
   ├─ Email format
   ├─ Phone number format
   ├─ Password requirements
   └─ Date ranges
```

---

## 🎯 Quick Navigation Patterns

### Header Quick Actions
```
All Users:
├─ Bell Icon → /notifications (with unread badge)
└─ Profile Avatar → /profile
```

### Sidebar Navigation
```
Student:
├─ Dashboard → /
├─ Lịch học → /schedule
├─ Điểm → /student-grades
├─ Đăng ký học phần → /registration
└─ Thông báo → /notifications

Admin (PGV/KHOA):
├─ Dashboard → /
├─ Lớp → /classes
├─ Sinh viên → /students
├─ Môn học → /subjects
├─ Giảng viên → /lecturers
├─ Lớp tín chỉ → /credit-classes
├─ Đăng ký tín chỉ → /registration
├─ Nhập điểm → /grades
├─ Báo cáo / In ấn → /reports
├─ Tài khoản / Phân quyền → /accounts
└─ Component Library → /components (dev tool)
```

### Breadcrumb Navigation (Future Enhancement)
```
Example: Dashboard > Sinh viên > Chi tiết sinh viên > Chỉnh sửa
```

---

## 📱 Responsive Behavior (Future Enhancement)

### Mobile Navigation
```
1. Hamburger menu replaces sidebar
2. Bottom navigation bar for primary actions
3. Collapsible sections
4. Touch-friendly buttons
```

### Tablet Navigation
```
1. Collapsible sidebar
2. Optimized table layouts
3. Touch and mouse support
```

---

## ♿ Accessibility Considerations (Future Enhancement)

### Keyboard Navigation
```
├─ Tab order follows logical flow
├─ Enter to submit forms
├─ Escape to close modals
└─ Arrow keys for navigation
```

### Screen Reader Support
```
├─ ARIA labels on interactive elements
├─ Semantic HTML structure
├─ Alt text for images
└─ Descriptive link text
```

---

## 🔄 Common User Tasks

### Daily Student Tasks
```
1. Check today's schedule → Dashboard or /schedule
2. View new grades → /student-grades
3. Check notifications → /notifications
4. Register for courses → /registration (during registration period)
```

### Daily Admin Tasks
```
1. Check dashboard statistics → Dashboard
2. Enter grades → /grades
3. Manage registrations → /credit-classes
4. Generate reports → /reports
5. Create accounts → /accounts
```

---

## 📊 Completion Status

✅ **Implemented Flows**
- Login/Logout
- Student Dashboard
- Admin Dashboard
- Schedule View
- Grades View
- Profile Settings
- Change Password
- Notifications
- Forgot Password
- 404 Error Page

⏳ **Partially Implemented Flows**
- Course Registration (UI ready, logic needs backend)
- Class Management (UI ready, CRUD operations need backend)
- Student Management (UI ready, CRUD operations need backend)
- Other management pages (UI ready, full functionality needs backend)

❌ **Not Yet Implemented**
- Role-based access control (route protection)
- Session timeout handling
- Unauthorized page
- Loading states
- Confirmation dialogs
- Toast notifications
- Form validation with error handling
- Bulk import/export
- Advanced search & filtering
- Pagination

---

*Last Updated: March 28, 2026*
