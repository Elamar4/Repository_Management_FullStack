using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository_management_backend.Data;
using Repository_management_backend.Models.ViewModels;
using Repository_management_backend.Security;
using System.Security.Claims;

namespace Repository_management_backend.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _hasher;

        public AccountController(AppDbContext db, IPasswordHasher hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await PopulateBranchesAsync();
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateBranchesAsync();
                return View(model);
            }

            var uname = model.Username.Trim().ToLower();
            var pwd = (model.Password ?? string.Empty).Trim();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == uname);

            if (user == null || !_hasher.Verify(pwd, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "İstifadəçi adı və ya şifrə yanlışdır.");
                await PopulateBranchesAsync();
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Bu hesab deaktivdir. Admin ilə əlaqə saxlayın.");
                await PopulateBranchesAsync();
                return View(model);
            }

            var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Code == model.BranchCode);
            if (branch == null)
            {
                ModelState.AddModelError(string.Empty, "Filial seçin.");
                await PopulateBranchesAsync();
                return View(model);
            }

            // İşçi yalnız öz filialı ilə girə bilər
            if (branch.Id != user.BranchId)
            {
                ModelState.AddModelError(string.Empty, "Bu işçi seçilmiş filiala aid deyil.");
                await PopulateBranchesAsync();
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, user.Role.ToString()),
                new("BranchId", user.BranchId.ToString()),
                new("BranchCode", branch.Code),
                new("Username", user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = false });

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout  (sadə keçid üçün GET; istəsəniz POST-a keçirin)
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task PopulateBranchesAsync()
        {
            ViewBag.Branches = await _db.Branches
                .Where(b => b.IsActive)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }
    }
}
