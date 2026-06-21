using System.Security.Claims;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Infrastructure.Helpers;
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
        ILecturerRepository lecturerRepository,
        IStudentRepository studentRepository) : Controller
    {
        private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> ResetOtpCache = new();

        [HttpGet]
        [AllowAnonymous]
        [Route("forgot-password")]
        public IActionResult ForgotPassword()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Redirect(RouteConstants.Home.HomePath);
            }
            return View(new ForgotPasswordViewModel { Step = 1, IsStudent = true });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            model.IsStudent = true; // Chỉ xử lý sinh viên

            if (model.ResendOtp)
            {
                if (string.IsNullOrWhiteSpace(model.LoginName))
                {
                    ViewBag.ErrorMessage = "Vui lòng nhập mã sinh viên.";
                    model.Step = 1;
                    ModelState.Clear();
                    return View(model);
                }

                var student = await studentRepository.GetStudentByIdAsync(model.LoginName);
                if (student == null)
                {
                    ViewBag.ErrorMessage = "Mã sinh viên không tồn tại trong hệ thống.";
                    model.Step = 1;
                    ModelState.Clear();
                    return View(model);
                }

                var loginNameClean = model.LoginName.Trim().ToLower();
                var email = $"{loginNameClean}@student.ptithcm.edu.vn";
                model.Email = email;

                var random = new Random();
                var otp = random.Next(100000, 999999).ToString();

                ResetOtpCache[loginNameClean] = (otp, DateTime.UtcNow.AddMinutes(5));

                try
                {
                    var host = Environment.GetEnvironmentVariable("MAIL_HOST") ?? "smtp.gmail.com";
                    var portStr = Environment.GetEnvironmentVariable("MAIL_PORT") ?? "587";
                    var user = Environment.GetEnvironmentVariable("MAIL_USER");
                    var pass = Environment.GetEnvironmentVariable("MAIL_PASSWORD");
                    var fromAddress = Environment.GetEnvironmentVariable("MAIL_FROM_ADDRESS") ?? user;

                    if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
                    {
                        int port = int.TryParse(portStr, out var p) ? p : 587;

                        using var message = new MailMessage();
                        message.From = new MailAddress(fromAddress ?? user, "Hệ Thống Quản Lý Đào Tạo");
                        message.To.Add(new MailAddress(email));
                        message.Subject = "Mã Xác Nhận Khôi Phục Mật Khẩu (Gửi Lại)";
                        message.Body = $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px;'>
                                <h3 style='color: #2563eb; margin-bottom: 20px;'>Mã Xác Nhận Khôi Phục Mật Khẩu (Gửi Lại)</h3>
                                <p>Xin chào,</p>
                                <p>Chúng tôi nhận được yêu cầu gửi lại mã xác nhận đặt lại mật khẩu cho tài khoản của bạn.</p>
                                <p>Mã xác nhận (OTP) mới của bạn là: <strong style='font-size: 24px; color: #2563eb; letter-spacing: 2px; background: #eff6ff; padding: 5px 15px; border-radius: 4px; display: inline-block;'>{otp}</strong></p>
                                <p>Mã này có hiệu lực trong vòng 5 phút. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
                                <p style='color: #64748b; font-size: 12px; margin-top: 30px; border-top: 1px solid #e2e8f0; padding-top: 15px;'>Trân trọng,<br/>Phòng Giáo vụ</p>
                            </div>";
                        message.IsBodyHtml = true;

                        using var client = new SmtpClient(host, port);
                        client.Credentials = new NetworkCredential(user, pass);
                        client.EnableSsl = true;

                        await client.SendMailAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SMTP ERROR]: {ex.Message}");
                }

                Console.WriteLine("==================================================");
                Console.WriteLine($"[RESEND OTP]: {otp} for student {loginNameClean}");
                Console.WriteLine("==================================================");

                ViewBag.SuccessMessage = $"Mã xác nhận (OTP) mới đã được gửi lại đến email: {email}.";
                model.Step = 2;
                model.Otp = null;
                model.ResendOtp = false;
                ModelState.Clear();
                return View(model);
            }

            if (model.Step == 1)
            {
                if (string.IsNullOrWhiteSpace(model.LoginName))
                {
                    ViewBag.ErrorMessage = "Vui lòng nhập mã sinh viên.";
                    return View(model);
                }

                var student = await studentRepository.GetStudentByIdAsync(model.LoginName);
                bool exists = student != null;

                if (!exists)
                {
                    ViewBag.ErrorMessage = "Mã sinh viên không tồn tại trong hệ thống.";
                    return View(model);
                }

                var loginNameClean = model.LoginName.Trim().ToLower();
                var email = $"{loginNameClean}@student.ptithcm.edu.vn";

                model.Email = email;

                var random = new Random();
                var otp = random.Next(100000, 999999).ToString();

                ResetOtpCache[loginNameClean] = (otp, DateTime.UtcNow.AddMinutes(5));

                try
                {
                    var host = Environment.GetEnvironmentVariable("MAIL_HOST") ?? "smtp.gmail.com";
                    var portStr = Environment.GetEnvironmentVariable("MAIL_PORT") ?? "587";
                    var user = Environment.GetEnvironmentVariable("MAIL_USER");
                    var pass = Environment.GetEnvironmentVariable("MAIL_PASSWORD");
                    var fromAddress = Environment.GetEnvironmentVariable("MAIL_FROM_ADDRESS") ?? user;

                    if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
                    {
                        int port = int.TryParse(portStr, out var p) ? p : 587;

                        using var message = new MailMessage();
                        message.From = new MailAddress(fromAddress ?? user, "Hệ Thống Quản Lý Đào Tạo");
                        message.To.Add(new MailAddress(email));
                        message.Subject = "Mã Xác Nhận Khôi Phục Mật Khẩu";
                        message.Body = $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px;'>
                                <h3 style='color: #2563eb; margin-bottom: 20px;'>Mã Xác Nhận Khôi Phục Mật Khẩu</h3>
                                <p>Xin chào,</p>
                                <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn trên hệ thống Quản lý Đào tạo.</p>
                                <p>Mã xác nhận (OTP) của bạn là: <strong style='font-size: 24px; color: #2563eb; letter-spacing: 2px; background: #eff6ff; padding: 5px 15px; border-radius: 4px; display: inline-block;'>{otp}</strong></p>
                                <p>Mã này có hiệu lực trong vòng 5 phút. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
                                <p style='color: #64748b; font-size: 12px; margin-top: 30px; border-top: 1px solid #e2e8f0; padding-top: 15px;'>Trân trọng,<br/>Phòng Giáo vụ</p>
                            </div>";
                        message.IsBodyHtml = true;

                        using var client = new SmtpClient(host, port);
                        client.Credentials = new NetworkCredential(user, pass);
                        client.EnableSsl = true;

                        await client.SendMailAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SMTP ERROR]: {ex.Message}");
                }

                Console.WriteLine("==================================================");
                Console.WriteLine($"[RESET PASSWORD OTP]: {otp} for student {loginNameClean}");
                Console.WriteLine("==================================================");

                ViewBag.SuccessMessage = $"Mã xác nhận (OTP) đã được gửi đến email: {email}. Vui lòng nhập mã để tiếp tục.";
                model.Step = 2;
                ModelState.Clear();
                return View(model);
            }
            else if (model.Step == 2)
            {
                var loginNameClean = model.LoginName.Trim().ToLower();
                var email = model.Email;
                if (string.IsNullOrEmpty(email))
                {
                    email = $"{loginNameClean}@student.ptithcm.edu.vn";
                }
                ViewBag.EmailInfo = $"Mã OTP đã được gửi đến email: {email}.";

                if (string.IsNullOrWhiteSpace(model.Otp))
                {
                    ViewBag.ErrorMessage = "Vui lòng nhập mã xác nhận OTP.";
                    return View(model);
                }



                if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 8)
                {
                    ViewBag.ErrorMessage = "Mật khẩu mới phải chứa ít nhất 8 ký tự.";
                    return View(model);
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    ViewBag.ErrorMessage = "Mật khẩu xác nhận không khớp.";
                    return View(model);
                }

                if (!ResetOtpCache.TryGetValue(loginNameClean, out var cachedData) || cachedData.Otp != model.Otp.Trim())
                {
                    ViewBag.ErrorMessage = "Mã xác nhận OTP không đúng hoặc đã hết hạn.";
                    return View(model);
                }

                if (DateTime.UtcNow > cachedData.Expiry)
                {
                    ResetOtpCache.TryRemove(loginNameClean, out _);
                    ViewBag.ErrorMessage = "Mã xác nhận OTP đã hết hạn. Vui lòng yêu cầu gửi lại mã.";
                    model.Step = 1;
                    ModelState.Clear();
                    return View(model);
                }

                try
                {
                    await studentRepository.ResetPasswordAsync(model.LoginName, model.NewPassword);

                    ResetOtpCache.TryRemove(loginNameClean, out _);

                    TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập với mật khẩu mới.";
                    return RedirectToAction("Login", new
                    {
                        successMessage = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập với mật khẩu mới.",
                        isStudent = true
                    });
                }
                catch (SqlException ex)
                {
                    ViewBag.ErrorMessage = SqlErrorHelper.GetFriendlyMessage(ex);
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Account.Login)]
        public IActionResult Login([FromQuery] string? successMessage = null, [FromQuery] string? errorMessage = null, [FromQuery] bool isStudent = false)
        {
            // If already logged in, redirect to Home
            if (User.Identity?.IsAuthenticated == true)
            {
                return Redirect(RouteConstants.Home.HomePath);
            }
            if (!string.IsNullOrEmpty(successMessage))
            {
                ViewBag.SuccessMessage = successMessage;
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.ErrorMessage = errorMessage;
            }
            return View(new LoginViewModel { IsStudent = isStudent });
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
                return BadRequest(new { success = false, message = SqlErrorHelper.GetFriendlyMessage(ex) });
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
                return BadRequest(new { success = false, message = SqlErrorHelper.GetFriendlyMessage(ex) });
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
                return BadRequest(new { success = false, message = SqlErrorHelper.GetFriendlyMessage(ex) });
            }
        }
    }
}
