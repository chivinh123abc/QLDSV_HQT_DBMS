# QLDSV_HTC - Quick Reference Guide

## 🚀 Quick Start

### Test Accounts
```
Sinh viên:     sv / sv
Phòng Giáo Vụ: pgv / pgv
Khoa:          khoa / khoa
```

### Key URLs
```
Login:              /login
Dashboard:          /
Profile:            /profile
Change Password:    /change-password
Notifications:      /notifications
Forgot Password:    /forgot-password
404 Page:           /404 or any invalid URL
```

---

## 📄 Complete Page List (20 pages)

### ✅ Public Pages (2)
1. `/login` - LoginPage
2. `/forgot-password` - ForgotPasswordPage

### ✅ Universal Pages (All Roles) (3)
3. `/profile` - ProfileSettingsPage
4. `/change-password` - ChangePasswordPage
5. `/notifications` - NotificationsPage

### ✅ Student Pages (SV) (4)
6. `/` - StudentDashboardPage (for SV role)
7. `/schedule` - StudentSchedulePage
8. `/student-grades` - StudentGradesPage
9. `/registration` - CourseRegistrationPage (shared with admin)

### ✅ Admin Pages (PGV/KHOA) (9)
10. `/` - DashboardPage (for PGV/KHOA roles)
11. `/classes` - ClassManagementPage
12. `/students` - StudentManagementPage
13. `/subjects` - SubjectManagementPage
14. `/lecturers` - LecturerManagementPage
15. `/credit-classes` - CreditClassPage
16. `/grades` - GradeEntryPage
17. `/reports` - ReportsPage
18. `/accounts` - AccountManagementPage

### ✅ Developer Tools (1)
19. `/components` - ComponentLibraryPage

### ✅ Error Pages (1)
20. `*` - NotFoundPage (404)

---

## 🎨 Design System

### Colors
```
Primary:    #2563EB (Blue)
Success:    #22C55E (Green)
Warning:    #F59E0B (Orange)
Error:      #EF4444 (Red)
```

### Role Colors
```
SV:    Green (#22C55E)
PGV:   Purple (#9333EA)
KHOA:  Blue (#3B82F6)
```

### Typography
```
Font Family: Be Vietnam Pro
Headings: Semibold
Body: Regular
```

### Spacing
```
Base unit: 8px
Grid: 8px increments
```

### Border Radius
```
Default: 12px
Buttons: 8px
Cards: 12-16px
Full: 9999px (pills)
```

---

## 🧭 Navigation Structure

### Student Sidebar
```
1. Dashboard
2. Lịch học
3. Điểm
4. Đăng ký học phần
5. Thông báo
```

### Admin Sidebar
```
1. Dashboard
2. Lớp
3. Sinh viên
4. Môn học
5. Giảng viên
6. Lớp tín chỉ
7. Đăng ký tín chỉ
8. Nhập điểm
9. Báo cáo / In ấn
10. Tài khoản / Phân quyền
11. Component Library
```

### Header Actions (All Roles)
```
- Notifications Bell (with badge)
- Profile Avatar (clickable → /profile)
```

---

## 📦 Component Patterns

### Buttons
```tsx
// Primary
<button className="bg-primary text-primary-foreground px-4 py-2 rounded-lg hover:bg-primary/90">

// Secondary
<button className="bg-secondary text-secondary-foreground px-4 py-2 rounded-lg hover:bg-secondary/80">

// Destructive
<button className="bg-destructive text-destructive-foreground px-4 py-2 rounded-lg hover:bg-destructive/90">
```

### Inputs
```tsx
<input
  type="text"
  className="w-full px-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
/>
```

### Cards
```tsx
<div className="bg-card border border-border rounded-xl p-6">
  {/* Content */}
</div>
```

### Status Badges
```tsx
<span className="px-3 py-1.5 bg-accent/10 text-accent border border-accent/20 rounded-full text-sm font-medium">
  Active
</span>
```

### Tables
```tsx
<table className="w-full">
  <thead className="bg-muted">
    <tr>
      <th className="px-4 py-3 text-left text-sm font-semibold">Header</th>
    </tr>
  </thead>
  <tbody>
    <tr className="border-t border-border hover:bg-muted/50">
      <td className="px-4 py-3 text-sm">Data</td>
    </tr>
  </tbody>
</table>
```

---

## 🔐 Authentication Context

### Usage
```tsx
import { useAuth } from "../contexts/AuthContext";

function MyComponent() {
  const { currentUser, currentRole, login, logout, isAuthenticated } = useAuth();
  
  // Use auth data
}
```

### User Object
```typescript
interface User {
  name: string;
  role: "PGV" | "KHOA" | "SV";
  department: string;
  studentId?: string; // Only for SV role
}
```

---

## 🛣️ Routing

### Protected Routes
```tsx
// In RootLayout.tsx
if (!currentUser || !currentRole) {
  navigate("/login");
  return null;
}
```

### Role-Based Rendering
```tsx
// In DashboardPage.tsx
if (currentRole === "SV") {
  return <StudentDashboardPage />;
}
// Otherwise show admin dashboard
```

---

## 📋 Common Patterns

### Success Message
```tsx
const [success, setSuccess] = useState(false);

// Show success
setSuccess(true);
setTimeout(() => setSuccess(false), 3000);

// Render
{success && (
  <div className="bg-accent/10 border border-accent/20 rounded-xl p-4">
    Success message
  </div>
)}
```

### Error Message
```tsx
const [error, setError] = useState("");

// Render
{error && (
  <div className="bg-destructive/10 border border-destructive/20 rounded-xl p-4">
    {error}
  </div>
)}
```

### Loading State (Future)
```tsx
const [loading, setLoading] = useState(false);

{loading ? <Spinner /> : <Content />}
```

### Empty State
```tsx
{items.length === 0 && (
  <div className="text-center p-12">
    <Icon className="w-16 h-16 text-muted-foreground mx-auto mb-4 opacity-50" />
    <h3 className="text-lg font-semibold mb-2">No items found</h3>
    <p className="text-sm text-muted-foreground">Description</p>
  </div>
)}
```

---

## 🎯 Key Features by Role

### Sinh viên (SV)
- ✅ View personal dashboard
- ✅ Check today's schedule
- ✅ View full weekly schedule
- ✅ View grades by semester
- ✅ Register for courses
- ✅ View and manage notifications
- ✅ Edit profile
- ✅ Change password

### Phòng Giáo Vụ (PGV)
- ✅ View admin dashboard with statistics
- ✅ Manage all classes
- ✅ Manage all students
- ✅ Manage all subjects
- ✅ Manage all lecturers
- ✅ Manage credit classes
- ✅ Handle course registrations
- ✅ Enter and manage grades
- ✅ Generate reports
- ✅ Manage user accounts
- ✅ Edit profile
- ✅ Change password

### Khoa (KHOA)
- Same as PGV (full access)
- Future: May have department-specific restrictions

---

## 🐛 Known Issues & Limitations

### Current Limitations
- ❌ No actual backend integration
- ❌ Mock data only
- ❌ No real authentication (test accounts only)
- ❌ No role-based route protection
- ❌ No data persistence (except localStorage for session)
- ❌ No loading states
- ❌ No confirmation dialogs
- ❌ No toast notifications
- ❌ No form validation libraries
- ❌ No mobile responsive design

### Edge Cases Not Handled
- Session timeout
- Concurrent logins
- Permission denied scenarios
- Network errors
- Large dataset pagination
- File upload/download

---

## 🚧 Future Enhancements

### P0 - Critical
- [ ] Backend API integration
- [ ] Real authentication & session management
- [ ] Role-based route protection
- [ ] Loading states for all async operations
- [ ] Error boundaries

### P1 - High Priority
- [ ] Form validation (react-hook-form)
- [ ] Toast notifications (sonner)
- [ ] Confirmation dialogs
- [ ] Pagination for large lists
- [ ] Search & filter functionality
- [ ] Bulk operations

### P2 - Medium Priority
- [ ] Mobile responsive design
- [ ] Dark mode
- [ ] Advanced reporting
- [ ] File upload/download
- [ ] Email notifications
- [ ] Activity logs

### P3 - Nice to Have
- [ ] Real-time updates (WebSocket)
- [ ] Advanced analytics
- [ ] Export to various formats
- [ ] Print optimization
- [ ] Internationalization (i18n)
- [ ] Accessibility improvements

---

## 📞 Support & Documentation

### Documentation Files
- `/SITEMAP_AND_AUDIT.md` - Complete page audit and sitemap
- `/USER_FLOWS.md` - Detailed user flow documentation
- `/QUICK_REFERENCE.md` - This file
- `/src/app/pages/ComponentLibraryPage.tsx` - UI component showcase

### Help Resources
- Component Library: `/components`
- Test Accounts: Displayed on login page
- Role Guide: Available in Component Library page

---

## 🔧 Development Notes

### Tech Stack
- React 18
- React Router 6 (Data Mode)
- TypeScript
- Tailwind CSS v4
- Lucide React (icons)
- Recharts (for charts)

### Project Structure
```
/src
  /app
    /components
      - RootLayout.tsx (main layout)
      - (other shared components)
    /contexts
      - AuthContext.tsx (authentication)
    /pages
      - (20 page components)
    - App.tsx (entry point)
    - routes.tsx (routing configuration)
  /styles
    - fonts.css
    - theme.css (design tokens)
```

### Adding New Pages
1. Create page component in `/src/app/pages/`
2. Import in `/src/app/routes.tsx`
3. Add route to router configuration
4. Add navigation item to RootLayout (if needed)
5. Update documentation

### Modifying Navigation
Edit arrays in `/src/app/components/RootLayout.tsx`:
- `adminNavigationItems` for PGV/KHOA
- `studentNavigationItems` for SV

---

## 📊 Testing Checklist

### Login Tests
- [ ] Login with sv/sv → Student dashboard
- [ ] Login with pgv/pgv → Admin dashboard
- [ ] Login with khoa/khoa → Admin dashboard
- [ ] Invalid credentials → Error message
- [ ] Already logged in → Redirect to dashboard
- [ ] Logout → Redirect to login

### Navigation Tests
- [ ] All sidebar links work
- [ ] Header profile link → Profile page
- [ ] Header bell icon → Notifications page
- [ ] 404 for invalid URLs
- [ ] Back button works correctly

### Student Feature Tests
- [ ] Dashboard loads correctly
- [ ] Schedule displays properly
- [ ] Grades show by semester
- [ ] Notifications load and filter
- [ ] Profile edit works
- [ ] Password change validates

### Admin Feature Tests
- [ ] Dashboard shows statistics
- [ ] All management pages load
- [ ] Component library displays
- [ ] Profile works same as student
- [ ] Can access student features via URL

---

## 🎓 Tips & Best Practices

### For Developers
1. Always check user role before rendering sensitive content
2. Use AuthContext for all auth-related operations
3. Follow the established component patterns
4. Keep pages in `/src/app/pages/`
5. Use Lucide icons consistently

### For Testers
1. Test with all 3 roles (sv, pgv, khoa)
2. Check responsive behavior (desktop 1440x1024)
3. Verify navigation works correctly
4. Test error cases (404, invalid input)
5. Check localStorage persistence

### For Designers
1. Follow design system tokens in theme.css
2. Use established color palette
3. Maintain 8px grid spacing
4. Keep 12px border radius
5. Use Be Vietnam Pro font

---

*Last Updated: March 28, 2026*
*Version: 1.0.0*
