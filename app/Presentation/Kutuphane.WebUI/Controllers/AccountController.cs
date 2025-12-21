using Kutuphane.Application.Dtos.AuthDtos;
using Kutuphane.Application.Dtos.MemberDtos;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Application.Services;
using Kutuphane.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kutuphane.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMemberService _memberService;
        private readonly ILoanService _loanService;

        public AccountController(IAuthService authService, IMemberService memberService, ILoanService loanService)
        {
            _authService = authService;
            _memberService = memberService;
            _loanService = loanService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var dto = new LoginDto
                {
                    Username = model.Username,
                    Password = model.Password
                };

                var result = await _authService.LoginAsync(dto);

                if (result == null)
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış.");
                    return View(model);
                }

                // Cookie oluşturma işlemleri (Burası aynı kalıyor)
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(result.Claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                    authProperties
                );

                TempData["Success"] = $"Hoş geldin, {result.User.Username}!";

              
                if (result.User.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Giriş hatası: {ex.Message}");
                return View(model);
            }
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Şifre kontrolü
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Şifreler eşleşmiyor.");
                    return View(model);
                }

                // 1. Önce Member oluştur
                var memberDto = new CreateMemberDto
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth
                };

                var member = await _memberService.CreateMemberAsync(memberDto);

                // 2. Sonra User oluştur (MemberId bağlı)
                var userDto = new RegisterDto
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = "Member", // Normal üye
                    MemberId = member.Id // ✅ Bağlantı
                };

                var user = await _authService.RegisterAsync(userDto);

                TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Kayıt hatası: {ex.Message}");
                return View(model);
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Başarıyla çıkış yaptınız.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
          
            return View();
        }


    }
}