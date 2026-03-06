using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Models;
using QLDSV_HTC.Application.Interfaces;

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
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isValid = _authRepository.ValidateUser(model.Username, model.Password);

            if (isValid)
            {
                // In actual project, you'd set session/cookies here
                TempData["SuccessMessage"] = "Đăng nhập thành công";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Sai tài khoản hoặc mật khẩu";
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Clear session/cookies here
            return RedirectToAction("Login");
        }
    }
}
