using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;

namespace ProjeTakipUygulaması.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            var model = new UserVM
            {
                Roles = _unitOfWork.Roles.GetAll().Select(r => new SelectListItem
                {
                    Text = r.RoleName,
                    Value = r.RoleId.ToString()
                })
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(UserVM model)
        {
            if (ModelState.IsValid)
            {
                // Şifreyi saltlayıp hashliyoruz
                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);

                var user = new User
                {
                    UserFName = model.UserFName,
                    UserLName = model.UserLName,
                    UserEmail = model.Email,
                    UserSalt = salt,
                    UserHash = hashedPassword,
                    Enabled = true
                };

                _unitOfWork.Users.Add(user);
                _unitOfWork.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }

            model.Roles = _unitOfWork.Roles.GetAll().Select(r => new SelectListItem
            {
                Text = r.RoleName,
                Value = r.RoleId.ToString()
            });
            return View(model);
        }
    }
}

