using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;

namespace ProjeTakipUygulaması.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeamController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var teams = _unitOfWork.Teams.GetAll(
                t => t.Enabled,
                includeProperties: "TeamLead,UserTeams.User,UserTeams.Role");

            return View(teams);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var teamVM = new TeamVM
            {
                Team = new Team(),
                TeamLeads = _unitOfWork.Users.GetAll(u => u.UserRoles.Any(ur => ur.RoleId == 2) && u.Enabled)
                    .Select(u => new SelectListItem
                    {
                        Text = u.UserFName + " " + u.UserLName,
                        Value = u.UserId.ToString()
                    }).ToList(),
                Users = _unitOfWork.Users.GetAll(u => u.Enabled)
                    .Select(u => new SelectListItem
                    {
                        Text = u.UserFName + " " + u.UserLName,
                        Value = u.UserId.ToString()
                    }).ToList()
            };

            return View(teamVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TeamVM teamVM)
        {
            if (!ModelState.IsValid)
            {
                // ModelState'teki hataları dökümle
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    // Hataları konsolda veya logda görüntüle
                    Console.WriteLine(error.ErrorMessage);
                }

                // Hatalar varsa formu yeniden yükle
                teamVM.TeamLeads = _unitOfWork.Users.GetAll(u => u.UserRoles.Any(ur => ur.RoleId == 2) && u.Enabled)
                    .Select(u => new SelectListItem
                    {
                        Text = u.UserFName + " " + u.UserLName,
                        Value = u.UserId.ToString()
                    }).ToList();

                teamVM.Users = _unitOfWork.Users.GetAll(u => u.Enabled)
                    .Select(u => new SelectListItem
                    {
                        Text = u.UserFName + " " + u.UserLName,
                        Value = u.UserId.ToString()
                    }).ToList();

                return View(teamVM);
            }

            try
            {
                // 1) Teams tablosuna verileri ekliyoruz
                var capacity = 1 + (teamVM.SelectedUserIds?.Count ?? 0);
                var team = new Team
                {
                    TeamName = teamVM.Team.TeamName,
                    TeamLeadId = teamVM.Team.TeamLeadId,
                    Capacity = capacity,
                    Enabled = true
                };

                _unitOfWork.Teams.Add(team);
                _unitOfWork.SaveChanges();  // İlk olarak Teams tablosuna kaydediyoruz, böylece TeamId elde ediliyor

                // 2) UserTeams tablosuna TeamLead için kayıt ekliyoruz
                _unitOfWork.UserTeams.Add(new UserTeam
                {
                    UserId = team.TeamLeadId,
                    TeamId = team.TeamId,  // Yeni oluşturulan TeamId'yi kullanıyoruz
                    RoleId = 2,  // TeamLead rolü
                    Enabled = true
                });

                // 3) UserTeams tablosuna her bir ekip üyesi için kayıt ekliyoruz
                if (teamVM.SelectedUserIds != null && teamVM.SelectedUserIds.Any())
                {
                    foreach (var userId in teamVM.SelectedUserIds)
                    {
                        var userTeam = new UserTeam
                        {
                            UserId = userId,
                            TeamId = team.TeamId,  // Yeni oluşturulan TeamId'yi kullanıyoruz
                            RoleId = 3,
                            Enabled = true
                        };

                        _unitOfWork.UserTeams.Add(userTeam);
                    }

                    _unitOfWork.SaveChanges();  // Tüm UserTeams kayıtlarını veritabanına kaydediyoruz
                }
                else
                {
                    ModelState.AddModelError("", "Herhangi bir kullanıcı seçilmedi.");
                    return View(teamVM);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Veritabanına kaydedilirken bir hata oluştu: {ex.Message}");
                return View(teamVM);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var teamVM = new TeamVM
            {
                Team = _unitOfWork.Teams.GetFirstOrDefault(t => t.TeamId == id),
                TeamLeads = _unitOfWork.Users.GetAll(u => u.UserRoles.Any(ur => ur.RoleId == 2) && u.Enabled)
                    .Select(u => new SelectListItem
                    {
                        Text = u.UserFName + " " + u.UserLName,
                        Value = u.UserId.ToString()
                    }).ToList(),
                Users = _unitOfWork.Users.GetAll(u => u.Enabled)
                    .Select(u => new SelectListItem
                    {
                        Text = u.UserFName + " " + u.UserLName,
                        Value = u.UserId.ToString()
                    }).ToList(),
                UserTeams = _unitOfWork.UserTeams.GetAll(ut => ut.TeamId == id).ToList()
            };

            if (teamVM.Team == null)
            {
                return NotFound();
            }

            return View(teamVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TeamVM teamVM)
        {
            if (ModelState.IsValid)
            {
                var capacity = 1 + (teamVM.SelectedUserIds?.Count ?? 0);
                // 1) Teams tablosundaki bilgileri güncelle
                var teamFromDb = _unitOfWork.Teams.GetFirstOrDefault(t => t.TeamId == teamVM.Team.TeamId);
                if (teamFromDb == null)
                {
                    return NotFound();
                }

                teamFromDb.TeamName = teamVM.Team.TeamName;
                teamFromDb.TeamLeadId = teamVM.Team.TeamLeadId;
                teamFromDb.Capacity = capacity;

                _unitOfWork.Teams.Update(teamFromDb);

                // 2) UserTeams tablosundaki bilgileri güncelle
                var existingUserTeams = _unitOfWork.UserTeams.GetAll(ut => ut.TeamId == teamVM.Team.TeamId).ToList();
                foreach (var userTeam in existingUserTeams)
                {
                    _unitOfWork.UserTeams.Remove(userTeam);  // Önce mevcut ilişkileri kaldırıyoruz
                }

                _unitOfWork.UserTeams.Add(new UserTeam
                {
                    UserId = teamVM.Team.TeamLeadId,
                    TeamId = teamVM.Team.TeamId,
                    RoleId = 2,
                    Enabled = true
                });

                foreach (var userId in teamVM.SelectedUserIds)
                {
                    _unitOfWork.UserTeams.Add(new UserTeam
                    {
                        UserId = userId,
                        TeamId = teamVM.Team.TeamId,
                        RoleId = 3,
                        Enabled = true
                    });
                }

                _unitOfWork.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(teamVM);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var team = _unitOfWork.Teams.GetFirstOrDefault(
                t => t.TeamId == id,
                includeProperties: "TeamLead,UserTeams.User,UserTeams.Role");

            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int id)
        {
            var teamFromDb = _unitOfWork.Teams.GetFirstOrDefault(t => t.TeamId == id);

            if (teamFromDb == null)
            {
                return NotFound();
            }

            // Takımın ve ilişkili UserTeams kayıtlarının Enabled alanını false yap
            teamFromDb.Enabled = false;
            var userTeams = _unitOfWork.UserTeams.GetAll(ut => ut.TeamId == id).ToList();
            foreach (var userTeam in userTeams)
            {
                userTeam.Enabled = false;
                _unitOfWork.UserTeams.Update(userTeam);
            }

            _unitOfWork.Teams.Update(teamFromDb);
            _unitOfWork.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var team = _unitOfWork.Teams.GetFirstOrDefault(
                t => t.TeamId == id,
                includeProperties: "TeamLead,UserTeams.User,UserTeams.Role");

            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }
    }
}
