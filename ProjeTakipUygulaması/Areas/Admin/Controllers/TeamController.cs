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
                var capacity = 1 + (teamVM.SelectedUserIds?.Count ?? 0);
                var team = new Team
                {
                    TeamName = teamVM.Team.TeamName,
                    TeamLeadId = teamVM.Team.TeamLeadId,
                    Capacity = capacity,
                    Enabled = true
                };

                _unitOfWork.Teams.Add(team);
                _unitOfWork.SaveChanges();

                _unitOfWork.UserTeams.Add(new UserTeam
                {
                    UserId = team.TeamLeadId,
                    TeamId = team.TeamId,
                    RoleId = 2,
                    Enabled = true
                });

                if (teamVM.SelectedUserIds != null && teamVM.SelectedUserIds.Any())
                {
                    foreach (var userId in teamVM.SelectedUserIds)
                    {
                        _unitOfWork.UserTeams.Add(new UserTeam
                        {
                            UserId = userId,
                            TeamId = team.TeamId,
                            RoleId = 3,
                            Enabled = true
                        });
                    }

                    _unitOfWork.SaveChanges();
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
                var teamFromDb = _unitOfWork.Teams.GetFirstOrDefault(t => t.TeamId == teamVM.Team.TeamId);
                if (teamFromDb == null)
                {
                    return NotFound();
                }

                teamFromDb.TeamName = teamVM.Team.TeamName;
                teamFromDb.TeamLeadId = teamVM.Team.TeamLeadId;

                var existingUserTeams = _unitOfWork.UserTeams.GetAll(ut => ut.TeamId == teamVM.Team.TeamId).ToList();
                var existingUserIds = existingUserTeams.Select(ut => ut.UserId).ToList();
                var newSelectedUserIds = teamVM.SelectedUserIds ?? new List<int>();
                var userIdsToRemove = existingUserIds.Except(newSelectedUserIds).ToList();
                var userIdsToAdd = newSelectedUserIds.Except(existingUserIds).ToList();

                // Eski TeamLead'in durumu kontrol ediliyor
                var oldTeamLead = existingUserTeams.FirstOrDefault(ut => ut.RoleId == 2);
                if (oldTeamLead != null)
                {
                    if (newSelectedUserIds.Contains(oldTeamLead.UserId))
                    {
                        // Eğer eski TeamLead ekip üyesi olarak seçilmişse, rolü güncelleniyor
                        oldTeamLead.RoleId = 3;
                        _unitOfWork.UserTeams.Update(oldTeamLead);
                    }
                    else
                    {
                        // Eğer eski TeamLead ekip üyesi olarak seçilmemişse, kaydı devre dışı bırakılıyor
                        oldTeamLead.Enabled = false;
                        _unitOfWork.UserTeams.Update(oldTeamLead);
                    }
                }

                foreach (var userId in userIdsToRemove)
                {
                    var userTeam = existingUserTeams.FirstOrDefault(ut => ut.UserId == userId);
                    if (userTeam != null)
                    {
                        _unitOfWork.UserTeams.Remove(userTeam);
                    }
                }

                foreach (var userId in userIdsToAdd)
                {
                    _unitOfWork.UserTeams.Add(new UserTeam
                    {
                        UserId = userId,
                        TeamId = teamVM.Team.TeamId,
                        RoleId = 3,
                        Enabled = true
                    });
                }

                // Yeni TeamLead ekleniyor
                var newTeamLeadUserTeam = existingUserTeams.FirstOrDefault(ut => ut.UserId == teamVM.Team.TeamLeadId);
                if (newTeamLeadUserTeam == null)
                {
                    _unitOfWork.UserTeams.Add(new UserTeam
                    {
                        UserId = teamVM.Team.TeamLeadId,
                        TeamId = teamVM.Team.TeamId,
                        RoleId = 2,
                        Enabled = true
                    });
                }

                teamFromDb.Capacity = 1 + newSelectedUserIds.Count;

                _unitOfWork.Teams.Update(teamFromDb);
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
