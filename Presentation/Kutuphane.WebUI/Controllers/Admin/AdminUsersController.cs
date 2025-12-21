using Kutuphane.Application.Dtos.AuthDtos;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Domain.Enums;
using Kutuphane.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kutuphane.WebUI.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IUserService _userService;

        public AdminUsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            var users = await _userService.GetAllUsersAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                users = users.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(searchTerm)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(searchTerm)) ||
                    (u.Username != null && u.Username.ToLower().Contains(searchTerm)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchTerm))
                ).ToList();
            }

            ViewBag.SearchTerm = searchTerm;
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var dto = new CreateUserDto
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Username = model.Username,
                    Password = model.Password,
                    Role = model.Role
                };

                await _userService.CreateUserAsync(dto);

                TempData["Success"] = "Personel hesabı ve üye kaydı başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Hata: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
               
                var currentUser = User.Identity.Name;
                var targetUser = await _userService.GetUserByIdAsync(id);

                if (targetUser.Username == currentUser)
                {
                    TempData["Error"] = "Güvenlik gereği kendi hesabınızı silemezsiniz.";
                    return RedirectToAction("Index");
                }

                await _userService.DeleteUserAsync(id);
                TempData["Success"] = "Kullanıcı başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Silme hatası: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
               
                var currentUser = User.Identity.Name;
                var targetUser = await _userService.GetUserByIdAsync(id);

                if (targetUser.Username == currentUser)
                {
                    TempData["Error"] = "Güvenlik gereği kendi hesabınızı pasife alamazsınız.";
                    return RedirectToAction("Index");
                }

        
                await _userService.UpdateUserStatusAsync(id);
                TempData["Success"] = "Kullanıcı durumu güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int id, UserRole newRole)
        {
            try
            {
            
                var currentUser = User.Identity.Name;
                var targetUser = await _userService.GetUserByIdAsync(id);

                if (targetUser.Username == currentUser)
                {
                    TempData["Error"] = "Kendi yetki seviyenizi değiştiremezsiniz.";
                    return RedirectToAction("Index");
                }

                await _userService.UpdateUserRoleAsync(id, newRole);
                TempData["Success"] = "Kullanıcı yetkisi güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newPassword)) throw new Exception("Şifre boş olamaz.");

                await _userService.ResetUserPasswordAsync(id, newPassword);
                TempData["Success"] = "Kullanıcı şifresi başarıyla sıfırlandı.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}