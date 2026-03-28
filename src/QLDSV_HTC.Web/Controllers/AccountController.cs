using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Models;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Controllers
{
    [Route(RouteConstants.Account.Prefix)]
    public class AccountController(IAuthRepository authRepository) : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        [Route(RouteConstants.Account.Login)]
        public IActionResult Login()
        {
            // If already logged in, redirect to Home
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return Redirect(RouteConstants.Home.HomePath);
            }
            return View();
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

            // Gọi repository với tham số isSinhVien
            var session = await authRepository.ValidateUserAsync(model.LoginName, model.Password ?? string.Empty, model.IsSinhVien ?? false);

            if (session != null && session.IsValid)
            {
                // Create Claims based on retrieved session info
                List<Claim> claims =
                [
                    new(AppConstants.SessionKeys.Username, session.LoginName),
                    new(AppConstants.SessionKeys.FullName, session.UserName),
                    new(AppConstants.SessionKeys.Group, session.Group),
                    new(AppConstants.SessionKeys.UserConnectionString, session.ConnectionString)
                ];

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
    }
}
