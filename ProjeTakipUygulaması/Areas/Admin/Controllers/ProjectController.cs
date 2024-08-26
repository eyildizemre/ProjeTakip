using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjeTakip.DataAccess.Data;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics;

namespace ProjeTakipUygulaması.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProjeDbContext _context;

        public ProjectController(IUnitOfWork unitOfWork, ProjeDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var projects = _context.Projects
                .Where(p => p.Enabled)
                .Include(p => p.Team)
                .Include(p => p.TeamLead)
                .Include(p => p.Status)
                .ToList();

            // Test amaçlı basit bir kontrol
            foreach (var project in projects)
            {
                if (project.TeamLead == null)
                {
                    Console.WriteLine($"Project ID {project.ProjectId} has no TeamLead assigned.");
                }
                else
                {
                    Console.WriteLine($"Project ID {project.ProjectId} has TeamLead: {project.TeamLead.UserFName} {project.TeamLead.UserLName}");
                }
            }

            var projectVMs = projects.Select(p => new ProjectVM
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                ProjectDescription = p.ProjectDescription,
                TeamId = p.TeamId,
                TeamName = p.Team?.TeamName ?? "Atanmamış",
                TeamLeadId = p.TeamLeadId,
                TeamLeadName = p.TeamLead != null ? $"{p.TeamLead.UserFName} {p.TeamLead.UserLName}" : "Atanmamış",
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ProjectStatusId = p.ProjectStatusId,
                ProjectStatusName = p.Status?.StatusName ?? "Bilinmiyor",
                Enabled = p.Enabled
            }).ToList();

            foreach (var project in projectVMs)
            {
                Debug.WriteLine($"Project ID: {project.ProjectId}, TeamLeadName: {project.TeamLeadName}");
            }


            return View(projectVMs);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var project = _unitOfWork.Projects.GetAll()
                              .AsQueryable()
                              .Include(p => p.Team)
                              .Include(p => p.TeamLead)
                              .Include(p => p.Status)
                              .Include(p => p.Tasks)
                              .FirstOrDefault(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            var teamCapacity = project.TeamId != null
                ? _unitOfWork.UserTeams.GetAll(ut => ut.TeamId == project.TeamId).Count()
                : 0;

            var model = new ProjectDetailsVM
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                TeamName = project.Team?.TeamName ?? "Bilgi yok",
                TeamLeadName = (project.TeamLead != null ? $"{project.TeamLead.UserFName} {project.TeamLead.UserLName}" : "Bilgi yok"),
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ProjectStatusName = project.Status?.StatusName ?? "Bilgi yok",
                TeamCapacity = teamCapacity,
                TotalTasks = project.Tasks?.Count ?? 0,
                SuccessfulTasks = project.Tasks?.Count(t => t.TaskStatusId == 3) ?? 0,
                FailedTasks = project.Tasks?.Count(t => t.TaskStatusId == 4) ?? 0,

                // OnayDurumuId'yi ekliyoruz
                OnayDurumuId = project.OnayDurumuId // Bu satırın eklenmesi gerekiyor
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProjectVM
            {
                Teams = _unitOfWork.Teams.GetAll(t => t.Enabled).Select(t => new SelectListItem
                {
                    Text = t.TeamName,
                    Value = t.TeamId.ToString()
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var selectedTeam = _unitOfWork.Teams.GetFirstOrDefault(t => t.TeamId == model.TeamId && t.Enabled);
                    if (selectedTeam == null)
                    {
                        ModelState.AddModelError("", "Seçilen takım bulunamadı.");
                        return View(model);
                    }

                    var project = new Project
                    {
                        ProjectName = model.ProjectName,
                        ProjectDescription = model.ProjectDescription,
                        TeamId = model.TeamId,
                        TeamLeadId = selectedTeam.TeamLeadId, // Takım lideri otomatik atanıyor
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        ProjectStatusId = 1, // "Not Started" olarak başlatıyoruz
                        Enabled = true
                    };

                    _unitOfWork.Projects.Add(project);
                    await _unitOfWork.SaveChangesAsync();

                    // Proje oluşturulduktan sonra ProjectId'yi al
                    var projectId = project.ProjectId;

                    // Ekip üyelerini UserTeams tablosuna ekle
                    var teamMembers = _unitOfWork.UserTeams.GetAll(ut => ut.TeamId == model.TeamId && ut.Enabled).ToList();
                    foreach (var member in teamMembers)
                    {
                        var userTeam = new UserTeam
                        {
                            UserId = member.UserId,
                            TeamId = (int)model.TeamId,
                            ProjectId = projectId,
                            RoleId = member.RoleId,
                            Enabled = true
                        };

                        _unitOfWork.UserTeams.Add(userTeam);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    TempData["Success"] = "Proje başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }

                model.Teams = _unitOfWork.Teams.GetAll(t => t.Enabled).Select(t => new SelectListItem
                {
                    Text = t.TeamName,
                    Value = t.TeamId.ToString()
                }).ToList();

                TempData["ErrorMessage"] = "Proje oluşturulamadı. Lütfen tekrar deneyin.";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Bir hata oluştu: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var project = _unitOfWork.Projects.GetAll().AsQueryable()
                                 .Include(p => p.Team)
                                 .Include(p => p.TeamLead)
                                 .Include(p => p.Status)
                                 .FirstOrDefault(p => p.ProjectId == id);

                if (project == null)
                {
                    return NotFound();
                }

                var model = new ProjectVM
                {
                    ProjectId = project.ProjectId,
                    ProjectName = project.ProjectName,
                    ProjectDescription = project.ProjectDescription,
                    TeamId = project.TeamId,
                    TeamLeadId = project.TeamLeadId,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Teams = _unitOfWork.Teams.GetAll(t => t.Enabled).Select(t => new SelectListItem
                    {
                        Text = t.TeamName,
                        Value = t.TeamId.ToString(),
                        Selected = (project.TeamId == t.TeamId)
                    }).ToList(),
                    TeamLeads = _unitOfWork.Users.GetAll(u => u.UserRoles.Any(r => r.RoleId == 2 && r.Enabled))
                                .Select(u => new SelectListItem
                                {
                                    Text = u.UserFName + " " + u.UserLName,
                                    Value = u.UserId.ToString(),
                                    Selected = (project.TeamLeadId == u.UserId)
                                }).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bir hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var project = _unitOfWork.Projects.GetFirstOrDefault(p => p.ProjectId == model.ProjectId);
                    if (project == null)
                    {
                        return NotFound();
                    }

                    project.ProjectName = model.ProjectName;
                    project.ProjectDescription = model.ProjectDescription;
                    project.TeamId = model.TeamId;
                    project.TeamLeadId = model.TeamLeadId;
                    project.StartDate = model.StartDate;
                    project.EndDate = model.EndDate;

                    _unitOfWork.Projects.Update(project);

                    // UserTeams tablosunu güncelle
                    var userTeam = _unitOfWork.UserTeams.GetFirstOrDefault(ut => ut.TeamId == project.TeamId && ut.ProjectId == project.ProjectId);
                    if (userTeam != null)
                    {
                        userTeam.UserId = (int)project.TeamLeadId;
                        _unitOfWork.UserTeams.Update(userTeam);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    TempData["Success"] = "Proje başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                model.Teams = _unitOfWork.Teams.GetAll(t => t.Enabled).Select(t => new SelectListItem
                {
                    Text = t.TeamName,
                    Value = t.TeamId.ToString(),
                    Selected = (model.TeamId == t.TeamId)
                });

                model.TeamLeads = _unitOfWork.Users.GetAll(u => u.UserRoles.Any(r => r.RoleId == 2 && r.Enabled))
                    .Select(u => new SelectListItem
                    {
                        Text = u.UserFName + " " + u.UserLName,
                        Value = u.UserId.ToString(),
                        Selected = (model.TeamLeadId == u.UserId)
                    });

                TempData["ErrorMessage"] = "Proje güncellenemedi. Lütfen tekrar deneyin.";
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var project = _unitOfWork.Projects.GetFirstOrDefault(p => p.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = _unitOfWork.Projects.GetFirstOrDefault(p => p.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            project.Enabled = false;
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Proje başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitProjectForApproval(int id)
        {
            var project = _unitOfWork.Projects.GetFirstOrDefault(p => p.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            project.ProjectStatusId = 3; // "Pending Approval"
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Approve(int id)
        {
            var project = _unitOfWork.Projects.GetAll().AsQueryable()
                              .Include(p => p.Team)
                              .Include(p => p.Status)
                              .FirstOrDefault(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            var model = new ProjectVM
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectStatusId = project.ProjectStatusId,
                ProjectStatusName = project.Status?.StatusName,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveProject(int id, bool isApproved, string comment)
        {
            var project = _unitOfWork.Projects.GetFirstOrDefault(p => p.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            if (!isApproved && string.IsNullOrWhiteSpace(comment))
            {
                ModelState.AddModelError("Comment", "Proje onaylanmadığında bir yorum eklemeniz zorunludur.");
                var model = new ProjectVM
                {
                    ProjectId = project.ProjectId,
                    ProjectName = project.ProjectName,
                    ProjectDescription = project.ProjectDescription,
                    ProjectStatusId = project.ProjectStatusId,
                    ProjectStatusName = project.Status?.StatusName,
                };
                return View(model);
            }

            project.ProjectStatusId = isApproved ? 4 : 5; // "Completed" veya "Rejected"
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();

            if (!isApproved)
            {
                var commentEntry = new Comment
                {
                    ProjectId = id,
                    CommentText = comment,
                    CommentDate = DateTime.Now,
                };
                _unitOfWork.Comments.Add(commentEntry);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}