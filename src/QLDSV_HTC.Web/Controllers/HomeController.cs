using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Extensions;
using QLDSV_HTC.Web.Models;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Home.Prefix)]
    public class HomeController(IStudentRepository studentRepository, IDbConnectionProvider connectionProvider) : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Home.Index)]
        public async Task<IActionResult> Index()
        {
            if (User.IsStudent())
            {
                var maSV = User.FindFirst(AppConstants.SessionKeys.Username)?.Value ?? string.Empty;
                var dashboard = await studentRepository.GetStudentDashboardAsync(maSV);

                if (dashboard != null)
                {
                    // Tính HK hiện tại theo thời gian thực
                    var now = DateTime.Now;
                    int hocKy = now.Month switch
                    {
                        >= 9 or 1 => 1,  // HK1: Tháng 9 - Tháng 1
                        >= 2 and <= 5 => 2,  // HK2: Tháng 2 - Tháng 5
                        _ => 3,  // HK3: Tháng 6 - Tháng 8
                    };
                    string nienKhoa = now.Month >= 9
                        ? $"{now.Year}-{now.Year + 1}"
                        : $"{now.Year - 1}-{now.Year}";

                    var vm = new StudentDashboardViewModel
                    {
                        StudentName = dashboard.FullName,
                        StudentId = dashboard.StudentId,
                        StudentClass = dashboard.ClassName,
                        FacultyName = string.Empty,
                        SchoolYear = dashboard.SchoolYear,
                        OnLeave = dashboard.OnLeave,
                        CurrentSemester = new CurrentSemesterInfoViewModel
                        {
                            Year = nienKhoa,
                            Semester = $"Học kỳ {hocKy}",
                            RegisteredCourses = dashboard.RegisteredCourses,
                        },
                        RegisteredCourses = dashboard.Courses.Select(c => new RegisteredCourseViewModel
                        {
                            SubjectName = c.SubjectName,
                            LecturerName = c.LecturerName,
                            IsTheory = c.TheoryHours > 0,
                            IsPractice = c.PracticeHours > 0 && c.TheoryHours == 0,
                        }),
                    };

                    return View(RouteConstants.Home.StudentDashboard, vm);
                }

                return View(RouteConstants.Home.StudentDashboard);
            }
            if (User.Identity?.IsAuthenticated == true)
            {
                // Lấy thống kê thật từ DB cho admin dashboard
                // KHOA: chỉ thống kê theo khoa của GV đăng nhập
                var isKhoa = User.IsKhoa();
                var maKhoa = User.FindFirst(AppConstants.SessionKeys.FacultyId)?.Value;

                string sql;
                if (isKhoa && !string.IsNullOrEmpty(maKhoa))
                {
                    sql = "SELECT " +
                        "(SELECT COUNT(*) FROM SINHVIEN sv INNER JOIN LOP l ON l.MALOP = sv.MALOP WHERE sv.DANGHIHOC = 0 AND l.MAKHOA = @MAKHOA) AS TongSV, " +
                        "(SELECT COUNT(*) FROM LOPTINCHI ltc INNER JOIN GIANGVIEN gv ON gv.MAGV = ltc.MAGV WHERE ltc.HUYLOP = 0 AND gv.MAKHOA = @MAKHOA) AS LopTCMo, " +
                        "(SELECT COUNT(*) FROM GIANGVIEN WHERE MAKHOA = @MAKHOA) AS TongGV, " +
                        "(SELECT COUNT(*) FROM MONHOC) AS TongMH";
                }
                else
                {
                    sql = "SELECT " +
                        "(SELECT COUNT(*) FROM SINHVIEN WHERE DANGHIHOC = 0) AS TongSV, " +
                        "(SELECT COUNT(*) FROM LOPTINCHI WHERE HUYLOP = 0) AS LopTCMo, " +
                        "(SELECT COUNT(*) FROM GIANGVIEN) AS TongGV, " +
                        "(SELECT COUNT(*) FROM MONHOC) AS TongMH";
                }

                await using var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionProvider.GetConnectionString());
                await using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
                if (isKhoa && !string.IsNullOrEmpty(maKhoa))
                {
                    cmd.Parameters.AddWithValue("@MAKHOA", maKhoa);
                }
                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    ViewBag.TongSV = Convert.ToInt32(reader["TongSV"]);
                    ViewBag.LopTCMo = Convert.ToInt32(reader["LopTCMo"]);
                    ViewBag.TongGV = Convert.ToInt32(reader["TongGV"]);
                    ViewBag.TongMH = Convert.ToInt32(reader["TongMH"]);
                }

                return View(RouteConstants.Home.AdminDashboard);
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Home.Error)]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Home.NotFound)]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Home.ComingSoon)]
        public IActionResult ComingSoon()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Home.AccessDenied)]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
