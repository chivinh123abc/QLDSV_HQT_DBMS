using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Models;
using QLDSV_HTC.Web.Models;

namespace QLDSV_HTC.Web.Controllers
{
    [Route(RouteConstants.Account.Prefix)]
    public class AccountController(
        IAuthRepository authRepository,
        IAccountRepository accountRepository,
        ILecturerRepository lecturerRepository) : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Account.Login)]
        public IActionResult Login()
        {
            // If already logged in, redirect to Home
            if (User.Identity?.IsAuthenticated == true)
            {
                return Redirect(RouteConstants.Home.HomePath);
            }
            return View(new LoginViewModel { IsStudent = false });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(RouteConstants.Account.Login)]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var session = await authRepository.ValidateUserAsync(model.LoginName, model.Password ?? string.Empty, model.IsStudent);

            if (session?.IsValid == true)
            {
                // Create Claims based on retrieved session info
                List<Claim> claims =
                [
                    new(AppConstants.SessionKeys.Username, session.LoginName),
                    new(AppConstants.SessionKeys.FullName, session.UserName),
                    new(AppConstants.SessionKeys.Group, session.Group),
                    new(AppConstants.SessionKeys.UserConnectionString, session.ConnectionString)
                ];

                if (!string.IsNullOrEmpty(session.FacultyId))
                {
                    claims.Add(new Claim(AppConstants.SessionKeys.FacultyId, session.FacultyId));
                }

                ClaimsIdentity claimsIdentity = new(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    nameType: AppConstants.SessionKeys.FullName,
                    roleType: AppConstants.SessionKeys.Group);

                AuthenticationProperties authProperties = new()
                {
                    IsPersistent = true // You could wire this to a "Remember Me" checkbox if you had one
                };

                // Sign in the user
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào {session.UserName} ({session.Group})";
                return Redirect(RouteConstants.Home.HomePath);
            }

            ViewBag.ErrorMessage = string.IsNullOrEmpty(session?.ErrorMessage) ? "Sai tài khoản hoặc mật khẩu" : session.ErrorMessage;
            return View(model);
        }

        [HttpPost]
        [Route(RouteConstants.Account.Logout)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(RouteConstants.Account.LoginPath);
        }

        // ────────────────────────────────────────────────
        // GET /account/management — Trang quản lý tài khoản
        // ────────────────────────────────────────────────
        [HttpGet]
        [Route(RouteConstants.Account.Management)]
        [Authorize(Roles = AppConstants.Groups.Faculty)]
        public async Task<IActionResult> Management()
        {
            var accounts = (await accountRepository.GetAccountListAsync()).ToList();
            var lecturers = (await lecturerRepository.GetLecturerListAsync(null)).ToList();

            var currentGroup = User.FindFirst(AppConstants.SessionKeys.Group)?.Value ?? string.Empty;

            var vm = new AccountManagementViewModel
            {
                Accounts = accounts.Select(a => new AccountViewModel
                {
                    LoginName = a.LoginName,
                    UserName = a.UserName,
                    GroupName = a.GroupName,
                    LecturerId = a.LecturerId,
                    LecturerFullName = a.LecturerFullName,
                    IsDisabled = a.IsDisabled,
                }),
                Lecturers = lecturers.Select(l => new LecturerViewModel
                {
                    Id = l.LecturerId,
                    FirstName = l.FirstName,
                    LastName = l.LastName,
                    FacultyId = l.FacultyId,
                    FacultyName = l.FacultyName,
                }),
                CurrentUserGroup = currentGroup,
            };

            return View("_AccountTable", vm);
        }

        // ────────────────────────────────────────────────
        // POST /account/management/add — Tạo tài khoản mới
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Account.Add)]
        [Authorize(Roles = AppConstants.Groups.Faculty)]
        public async Task<IActionResult> Add([FromBody] AccountInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            // Phân quyền: KHOA chỉ được tạo tài khoản nhóm KHOA
            var currentGroup = User.FindFirst(AppConstants.SessionKeys.Group)?.Value ?? string.Empty;
            if (currentGroup.Equals(AppConstants.Groups.KHOA, StringComparison.OrdinalIgnoreCase) &&
                !input.Role.Equals(AppConstants.Groups.KHOA, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { success = false, message = "Bạn không có quyền tạo tài khoản nhóm này." });
            }

            try
            {
                await accountRepository.CreateAccountAsync(new CreateAccountDto
                {
                    LoginName = input.LoginName.Trim(),
                    Password = input.Password,
                    UserName = input.UserName.Trim(),
                    Role = input.Role.Trim(),
                });
                return Ok(new { success = true, message = "Tạo tài khoản thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────
        // POST /account/management/edit — Đổi mật khẩu
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Account.Edit)]
        [Authorize(Roles = AppConstants.Groups.Faculty)]
        public async Task<IActionResult> Edit([FromBody] AccountUpdateInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                if (string.IsNullOrWhiteSpace(input.NewLoginName) &&
                    string.IsNullOrWhiteSpace(input.NewPassword) &&
                    string.IsNullOrWhiteSpace(input.NewUserName) &&
                    string.IsNullOrWhiteSpace(input.NewRole))
                {
                    return Ok(new { success = true, message = "Không có thay đổi nào được thực hiện." });
                }

                var currentLoginName = User.FindFirst(AppConstants.SessionKeys.Username)?.Value?.Trim();
                var accounts = await accountRepository.GetAccountListAsync();
                var targetAccount = accounts.FirstOrDefault(a => a.LoginName.Trim().Equals(input.OldLoginName.Trim(), StringComparison.OrdinalIgnoreCase));

                bool isSelfUpdate = targetAccount != null && !string.IsNullOrEmpty(currentLoginName) &&
                                   targetAccount.UserName.Trim().Equals(currentLoginName, StringComparison.OrdinalIgnoreCase);

                var currentGroup = User.FindFirst(AppConstants.SessionKeys.Group)?.Value ?? string.Empty;
                bool isKhoa = currentGroup.Equals(AppConstants.Groups.KHOA, StringComparison.OrdinalIgnoreCase);

                if ((isSelfUpdate || isKhoa) && !string.IsNullOrWhiteSpace(input.NewRole))
                {
                    string msg = isKhoa ? "Nhóm KHOA không có quyền thay đổi nhóm quyền." : "Bạn không thể tự đổi quyền của chính mình khi đang thao tác!";
                    return BadRequest(new { success = false, message = msg });
                }

                await accountRepository.UpdateAccountAsync(new UpdateAccountDto
                {
                    OldLoginName = input.OldLoginName.Trim(),
                    NewLoginName = string.IsNullOrWhiteSpace(input.NewLoginName) ? null : input.NewLoginName.Trim(),
                    NewPassword = input.NewPassword,
                    NewUserName = string.IsNullOrWhiteSpace(input.NewUserName) ? null : input.NewUserName.Trim(),
                    NewRole = string.IsNullOrWhiteSpace(input.NewRole) ? null : input.NewRole.Trim(),
                });

                if (isSelfUpdate && (!string.IsNullOrWhiteSpace(input.NewPassword) || !string.IsNullOrWhiteSpace(input.NewLoginName)))
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return Ok(new { success = true, forceLogout = true, message = "Cập nhật thành công. Hệ thống sẽ đăng xuất để áp dụng thay đổi." });
                }

                return Ok(new { success = true, message = "Cập nhật tài khoản thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────
        // POST /account/management/delete — Xóa tài khoản
        // ────────────────────────────────────────────────
        [HttpPost]
        [Route(RouteConstants.Account.Delete)]
        [Authorize(Roles = AppConstants.Groups.PGV)]
        public async Task<IActionResult> Delete([FromBody] AccountDeleteModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            if (string.IsNullOrWhiteSpace(input?.LoginName))
                return BadRequest(new { success = false, message = "Tên đăng nhập không hợp lệ." });

            var currentLoginName = User.FindFirst(AppConstants.SessionKeys.Username)?.Value?.Trim();
            var accounts = await accountRepository.GetAccountListAsync();
            var targetAccount = accounts.FirstOrDefault(a => a.LoginName.Trim().Equals(input.LoginName.Trim(), StringComparison.OrdinalIgnoreCase));

            if (targetAccount != null && !string.IsNullOrEmpty(currentLoginName) &&
                targetAccount.UserName.Trim().Equals(currentLoginName, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { success = false, message = "Bạn không thể tự xóa tài khoản của chính mình!" });
            }

            try
            {
                await accountRepository.DeleteAccountAsync(input.LoginName.Trim());
                return Ok(new { success = true, message = "Xóa tài khoản thành công." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
