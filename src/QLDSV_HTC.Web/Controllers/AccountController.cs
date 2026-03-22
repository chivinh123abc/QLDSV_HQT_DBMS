using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Models;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthRepository _authRepository;

        public AccountController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to Home
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return Redirect(AppConstants.Routes.Home);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gọi repository với tham số isSinhVien
            var session = await _authRepository.ValidateUserAsync(model.LoginName, model.Password, model.IsSinhVien);

            if (session != null && session.IsValid)
            {
                // Create Claims based on retrieved session info
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, session.LoginName),
                    new Claim(ClaimTypes.Name, session.UserName),
                    new Claim(AppConstants.SessionKeys.Group, session.Group),
                    new Claim(ClaimTypes.Role, session.Group), 
                    // Lưu lại ConnectionString để tái sử dụng ở toàn bộ các Queries sau này
                    new Claim("UserConnectionString", session.ConnectionString)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // You could wire this to a "Remember Me" checkbox if you had one
                };

                // Sign in the user
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào {session.UserName} ({session.Group})";
                return Redirect(AppConstants.Routes.Home);
            }

            ViewBag.ErrorMessage = string.IsNullOrEmpty(session?.ErrorMessage) ? "Sai tài khoản hoặc mật khẩu" : session.ErrorMessage;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(AppConstants.Routes.Login);
        }
    }
}
