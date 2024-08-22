using Microsoft.AspNetCore.Mvc;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;
using ProjeTakip.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace ProjeTakipUygulaması.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.Users.GetFirstOrDefault(
                    u => u.UserEmail == model.Email && u.Enabled,
                    includeProperties: "UserRoles");

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.UserHash))
                {
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());

                    var userRole = user.UserRoles?.FirstOrDefault(); // userRole null olup olmadığını kontrol et

                    if (userRole != null)
                    {
                        HttpContext.Session.SetInt32("RoleId", userRole.RoleId);

                        if (userRole.RoleId == 1)
                        {
                            return RedirectToAction("Index", "Admin", new { area = "Admin" });
                        }
                        else if (userRole.RoleId == 2)
                        {
                            return RedirectToAction("Index", "TeamLead", new { area = "TeamLead" });
                        }
                        else if (userRole.RoleId == 3)
                        {
                            return RedirectToAction("Index", "TeamMember", new { area = "TeamMember" });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Kullanıcının atanmış bir rolü yok.");
                        return View(model);
                    }
                }

                ModelState.AddModelError("", "Geçersiz giriş bilgileri.");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
