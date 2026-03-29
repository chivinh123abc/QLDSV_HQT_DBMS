import { createBrowserRouter, Navigate } from "react-router";
import { RootLayout } from "./components/RootLayout";
import { LoginPage } from "./pages/LoginPage";
import { DashboardPage } from "./pages/DashboardPage";
import { StudentDashboardPage } from "./pages/StudentDashboardPage";
import { StudentSchedulePage } from "./pages/StudentSchedulePage";
import { StudentGradesPage } from "./pages/StudentGradesPage";
import { ClassManagementPage } from "./pages/ClassManagementPage";
import { StudentManagementPage } from "./pages/StudentManagementPage";
import { SubjectManagementPage } from "./pages/SubjectManagementPage";
import { LecturerManagementPage } from "./pages/LecturerManagementPage";
import { CreditClassPage } from "./pages/CreditClassPage";
import { CourseRegistrationPage } from "./pages/CourseRegistrationPage";
import { GradeEntryPage } from "./pages/GradeEntryPage";
import { ReportsPage } from "./pages/ReportsPage";
import { AccountManagementPage } from "./pages/AccountManagementPage";
import { ComponentLibraryPage } from "./pages/ComponentLibraryPage";
import { ProfileSettingsPage } from "./pages/ProfileSettingsPage";
import { ChangePasswordPage } from "./pages/ChangePasswordPage";
import { NotificationsPage } from "./pages/NotificationsPage";
import { ForgotPasswordPage } from "./pages/ForgotPasswordPage";
import { NotFoundPage } from "./pages/NotFoundPage";

export const router = createBrowserRouter([
  // Public routes
  {
    path: "/login",
    Component: LoginPage,
  },
  {
    path: "/forgot-password",
    Component: ForgotPasswordPage,
  },
  
  // Protected routes
  {
    path: "/",
    Component: RootLayout,
    children: [
      { index: true, Component: DashboardPage },
      
      // Universal routes (all roles)
      { path: "profile", Component: ProfileSettingsPage },
      { path: "change-password", Component: ChangePasswordPage },
      { path: "notifications", Component: NotificationsPage },
      
      // Student-specific routes
      { path: "schedule", Component: StudentSchedulePage },
      { path: "student-grades", Component: StudentGradesPage },
      
      // Admin routes
      { path: "classes", Component: ClassManagementPage },
      { path: "students", Component: StudentManagementPage },
      { path: "subjects", Component: SubjectManagementPage },
      { path: "lecturers", Component: LecturerManagementPage },
      { path: "credit-classes", Component: CreditClassPage },
      { path: "registration", Component: CourseRegistrationPage },
      { path: "grades", Component: GradeEntryPage },
      { path: "reports", Component: ReportsPage },
      { path: "accounts", Component: AccountManagementPage },
      { path: "components", Component: ComponentLibraryPage },
    ],
  },
  
  // 404 catch-all
  {
    path: "*",
    Component: NotFoundPage,
  },
]);