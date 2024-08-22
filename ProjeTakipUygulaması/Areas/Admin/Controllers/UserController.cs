using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

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
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UserVM model)
        {
            model.RoleId = Convert.ToInt32(Request.Form["RoleId"]);
            if (ModelState.IsValid)
            {
                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);

                var user = new User
                {
                    UserFName = model.UserFName,
                    UserLName = model.UserLName,
                    UserEmail = model.Email,
                    UserSalt = salt,
                    UserHash = hashedPassword,
                    GitHubProfile = model.GitHubProfile,
                    Enabled = true
                };

                _unitOfWork.Users.Add(user);
                await _unitOfWork.SaveChangesAsync();

                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = model.RoleId,
                };
                _unitOfWork.UserRoles.Add(userRole);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction("Index", "Home"); // HomePage'e yönlendiriliyor
            }

            model.Roles = _unitOfWork.Roles.GetAll().Select(r => new SelectListItem
            {
                Text = r.RoleName,
                Value = r.RoleId.ToString()
            });

            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateUser(int id)
        {
            var user = _unitOfWork.Users.GetFirstOrDefault(u => u.UserId == id, includeProperties: "UserRoles");
            if (user == null) return NotFound();

            var model = new UserVM
            {
                UserId = user.UserId,
                UserFName = user.UserFName,
                UserLName = user.UserLName,
                Email = user.UserEmail,
                GitHubProfile = user.GitHubProfile,
                RoleId = user.UserRoles.FirstOrDefault()?.RoleId ?? 0,
                Roles = _unitOfWork.Roles.GetAll().Select(r => new SelectListItem
                {
                    Text = r.RoleName,
                    Value = r.RoleId.ToString()
                })
            };

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.Users.GetFirstOrDefault(u => u.UserId == model.UserId, includeProperties: "UserRoles");
                if (user == null) return NotFound();

                user.UserFName = model.UserFName;
                user.UserLName = model.UserLName;
                user.UserEmail = model.Email;
                user.GitHubProfile = model.GitHubProfile;

                _unitOfWork.Users.Update(user);

                var userRole = _unitOfWork.UserRoles.GetFirstOrDefault(ur => ur.UserId == user.UserId);
                if (userRole != null)
                {
                    userRole.RoleId = model.RoleId;
                    _unitOfWork.UserRoles.Update(userRole);
                }
                else
                {
                    userRole = new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = model.RoleId
                    };
                    _unitOfWork.UserRoles.Add(userRole);
                }

                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }

            model.Roles = _unitOfWork.Roles.GetAll().Select(r => new SelectListItem
            {
                Text = r.RoleName,
                Value = r.RoleId.ToString()
            });

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = _unitOfWork.Users.GetFirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();

            user.Enabled = false; // Kullanıcıyı devre dışı bırak
            _unitOfWork.Users.Update(user);

            // RoleId'yi 0 yapma işlemini kaldırıyoruz, sadece Enabled'ı false yapıyoruz
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction("Index", "Admin");
        }
    }
}
