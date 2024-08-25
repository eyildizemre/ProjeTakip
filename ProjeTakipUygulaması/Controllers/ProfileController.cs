using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ProjeTakipUygulaması.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // Kullanıcıyı session'dan al
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            // Kullanıcının bilgilerini veritabanından al
            var user = _unitOfWork.Users.GetFirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileVM
            {
                UserId = user.UserId,
                UserFName = user.UserFName,
                UserLName = user.UserLName,
                Email = user.UserEmail,
                GitHubProfile = user.GitHubProfile
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _unitOfWork.Users.GetFirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileVM
            {
                UserFName = user.UserFName,
                UserLName = user.UserLName,
                Email = user.UserEmail,
                GitHubProfile = user.GitHubProfile
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userFromDb = _unitOfWork.Users.GetFirstOrDefault(u => u.UserId == userId);

            if (userFromDb == null)
            {
                return NotFound();
            }

            userFromDb.UserFName = model.UserFName;
            userFromDb.UserLName = model.UserLName;
            userFromDb.UserEmail = model.Email;
            userFromDb.GitHubProfile = model.GitHubProfile;

            _unitOfWork.Users.Update(userFromDb);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Profil bilgileriniz başarıyla güncellendi.";
            return RedirectToAction("Index", "Profile");
        }

        [HttpGet]
        public IActionResult Password()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password(PasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userFromDb = _unitOfWork.Users.GetFirstOrDefault(u => u.UserId == userId);

            if (userFromDb == null)
            {
                return NotFound();
            }

            // Eski şifreyi doğrula
            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, userFromDb.UserHash))
            {
                ModelState.AddModelError("", "Şifreyi yanlış girdiniz.");
                return View(model);
            }

            // Yeni şifreyi hash'le ve kaydet
            userFromDb.UserHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _unitOfWork.Users.Update(userFromDb);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Şifreniz başarıyla güncellendi.";
            return RedirectToAction("Index", "Profile");
        }
    }
}