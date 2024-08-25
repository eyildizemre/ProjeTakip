using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;
using System.Threading.Tasks;

namespace ProjeTakipUygulaması.Areas.TeamLead.Controllers
{
    [Area("TeamLead")]
    public class TaskController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var teamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var tasks = _unitOfWork.Tasks.GetAll(
                t => t.Enabled && t.TeamLeadId == teamLeadId, // Sadece oturumdaki TeamLead'e ait görevler
                includeProperties: "AssignedUser,Project,Status,OnayDurumu").ToList();

            var model = tasks.Select(t => new TaskVM
            {
                TaskId = t.TaskId,
                TaskName = t.TaskName,
                TaskDescription = t.TaskDescription,
                UserId = t.AssignedUserId,
                TeamLeadId = (int)t.TeamLeadId,
                AssignedUserName = t.AssignedUser != null ? t.AssignedUser.UserFName + " " + t.AssignedUser.UserLName : "Atanmamış",
                ProjectId = t.ProjectId,
                ProjectName = t.Project.ProjectName,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                TaskStatusName = t.Status.StatusName,
                OnayDurumuId = t.OnayDurumu?.OnayDurumuId ?? 4,
                OnayDurumuAdi = t.OnayDurumu?.OnayDurumuAdi ?? "Onay Durumu Belirtilmemiş",
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var teamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var model = new TaskVM
            {
                TeamLeadId = teamLeadId,
                Users = new List<SelectListItem>(),
                Projects = _unitOfWork.Projects.GetAll(p => p.Enabled && p.TeamLeadId == teamLeadId) // Sadece oturumdaki TeamLead'e ait projeler
                    .Select(p => new SelectListItem
                    {
                        Text = p.ProjectName,
                        Value = p.ProjectId.ToString()
                    }).ToList(),
                Statuses = _unitOfWork.Statuses.GetAll().Select(s => new SelectListItem
                {
                    Text = s.StatusName,
                    Value = s.StatusId.ToString()
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskVM model)
        {
            var teamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            if (ModelState.IsValid)
            {
                var task = new Görev
                {
                    TaskName = model.TaskName,
                    TaskDescription = model.TaskDescription,
                    TeamLeadId = teamLeadId, // Oturumdaki TeamLeadId'yi kullanıyoruz
                    AssignedUserId = model.UserId,
                    ProjectId = model.ProjectId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    TaskStatusId = 2,
                    OnayDurumuId = 4,
                    Enabled = true
                };

                task.AssignedUser = _unitOfWork.Users.GetFirstOrDefault(u => u.UserId == model.UserId);

                // Görevi eklemeden önce proje için ilk görev mi kontrol et
                var existingTasks = _unitOfWork.Tasks.GetAll(t => t.ProjectId == model.ProjectId && t.Enabled);
                if (!existingTasks.Any())
                {
                    // Bu proje için henüz görev yoksa, projenin StatusId'sini 2 olarak güncelle
                    var project = _unitOfWork.Projects.GetFirstOrDefault(p => p.ProjectId == model.ProjectId && p.TeamLeadId == teamLeadId);
                    if (project != null)
                    {
                        project.ProjectStatusId = 2;
                        _unitOfWork.Projects.Update(project);
                    }
                }

                _unitOfWork.Tasks.Add(task);
                await _unitOfWork.SaveChangesAsync();
                TempData["Success"] = "Görev başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }

            // ModelState hatalıysa, gerekli listeleri yeniden yükle
            model.Users = _unitOfWork.Users.GetAll(u => u.Enabled).Select(u => new SelectListItem
            {
                Text = u.UserFName + " " + u.UserLName,
                Value = u.UserId.ToString()
            }).ToList();

            model.Projects = _unitOfWork.Projects.GetAll(p => p.Enabled && p.TeamLeadId == teamLeadId) // Yine sadece bu TeamLead'e ait projeler
                .Select(p => new SelectListItem
                {
                    Text = p.ProjectName,
                    Value = p.ProjectId.ToString()
                }).ToList();

            model.Statuses = _unitOfWork.Statuses.GetAll().Select(s => new SelectListItem
            {
                Text = s.StatusName,
                Value = s.StatusId.ToString()
            }).ToList();
            TempData["error"] = "Görev eklenirken bir hata oluştu!";
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var teamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var task = _unitOfWork.Tasks.GetFirstOrDefault(
                t => t.TaskId == id && t.TeamLeadId == teamLeadId, // Sadece bu TeamLead'in görevleri
                includeProperties: "AssignedUser,Project,Status");

            if (task == null)
            {
                return NotFound();
            }

            var model = new TaskVM
            {
                TaskId = task.TaskId,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                TeamLeadId = teamLeadId,
                ProjectId = task.ProjectId,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                GitHubPush = task.GitHubPush,
                TaskStatusId = task.TaskStatusId,
                Users = _unitOfWork.Users.GetAll(u => u.Enabled).Select(u => new SelectListItem
                {
                    Text = u.UserFName + " " + u.UserLName,
                    Value = u.UserId.ToString(),
                    Selected = (task.AssignedUserId == u.UserId) // Görev atanmış kullanıcıyı doğru seçmek için
                }).ToList(),
                Projects = _unitOfWork.Projects.GetAll(p => p.Enabled && p.TeamLeadId == teamLeadId).Select(p => new SelectListItem
                {
                    Text = p.ProjectName,
                    Value = p.ProjectId.ToString(),
                    Selected = (task.ProjectId == p.ProjectId)
                }).ToList(),
                Statuses = _unitOfWork.Statuses.GetAll().Select(s => new SelectListItem
                {
                    Text = s.StatusName,
                    Value = s.StatusId.ToString(),
                    Selected = (task.TaskStatusId == s.StatusId)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskVM model)
        {
            var teamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            if (ModelState.IsValid)
            {
                var task = _unitOfWork.Tasks.GetFirstOrDefault(
                    t => t.TaskId == model.TaskId && t.TeamLeadId == teamLeadId); // Sadece bu TeamLead'in görevleri

                if (task == null)
                {
                    return NotFound();
                }

                task.TaskName = model.TaskName;
                task.TaskDescription = model.TaskDescription;
                task.TeamLeadId = teamLeadId;
                task.ProjectId = model.ProjectId;
                task.StartDate = model.StartDate;
                task.EndDate = model.EndDate;
                task.GitHubPush = model.GitHubPush;
                task.TaskStatusId = model.TaskStatusId;

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                TempData["Success"] = "Görev başarıyla güncellendi!";
                return RedirectToAction(nameof(Index));
            }

            model.Users = _unitOfWork.Users.GetAll(u => u.Enabled).Select(u => new SelectListItem
            {
                Text = u.UserFName + " " + u.UserLName,
                Value = u.UserId.ToString(),
                Selected = (model.UserId == u.UserId)
            }).ToList();

            model.Projects = _unitOfWork.Projects.GetAll(p => p.Enabled && p.TeamLeadId == teamLeadId).Select(p => new SelectListItem
            {
                Text = p.ProjectName,
                Value = p.ProjectId.ToString(),
                Selected = (model.ProjectId == p.ProjectId)
            }).ToList();

            model.Statuses = _unitOfWork.Statuses.GetAll().Select(s => new SelectListItem
            {
                Text = s.StatusName,
                Value = s.StatusId.ToString(),
                Selected = (model.TaskStatusId == s.StatusId)
            }).ToList();

            TempData["error"] = "Görev güncellenirken bir hata oluştu!";
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var teamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var task = _unitOfWork.Tasks.GetFirstOrDefault(
                t => t.TaskId == id && t.TeamLeadId == teamLeadId, // Sadece bu TeamLead'in görevleri
                includeProperties: "AssignedUser,Project,Status");

            if (task == null)
            {
                return NotFound();
            }

            var model = new TaskVM
            {
                TaskId = task.TaskId,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                AssignedUserName = task.AssignedUser?.UserFName + " " + task.AssignedUser?.UserLName,
                ProjectName = task.Project?.ProjectName,
                TaskStatusName = task.Status?.StatusName,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                GitHubPush = task.GitHubPush
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var task = _unitOfWork.Tasks.GetFirstOrDefault(
                t => t.TaskId == id && t.TeamLeadId == teamLeadId); // Sadece bu TeamLead'in görevleri

            if (task != null)
            {
                task.Enabled = false;
                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();
                TempData["Success"] = "Görev başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var teamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var task = _unitOfWork.Tasks.GetFirstOrDefault(
                t => t.TaskId == id && t.TeamLeadId == teamLeadId, // Sadece bu TeamLead'in görevleri
                includeProperties: "AssignedUser,Project,Status,OnayDurumu,Comments");

            if (task == null)
            {
                return NotFound();
            }

            var model = new TaskVM
            {
                TaskId = task.TaskId,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                AssignedUserName = task.AssignedUser?.UserFName + " " + task.AssignedUser?.UserLName,
                ProjectName = task.Project?.ProjectName,
                TaskStatusName = task.Status?.StatusName,
                OnayDurumuId = task.OnayDurumuId ?? 0, // Eğer OnayDurumuId null ise 0 atanır
                OnayDurumuAdi = task.OnayDurumu != null ? task.OnayDurumu.OnayDurumuAdi : "Onay Durumu Belirtilmedi",
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                GitHubPush = task.GitHubPush,
                CommentText = task.Comments?.LastOrDefault()?.CommentText // Son yapılan yorumu alır
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(TaskVM taskVM)
        {
            var teamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            // Veritabanından görevi çek ve gerekli ilişkili verileri dahil et
            var taskFromDb = _unitOfWork.Tasks.GetFirstOrDefault(
                t => t.TaskId == taskVM.TaskId && t.TeamLeadId == teamLeadId, // Sadece bu TeamLead'in görevleri
                includeProperties: "TeamLead,AssignedUser,Project,Status,OnayDurumu");

            if (taskFromDb == null)
            {
                return NotFound();
            }

            // taskVM'yi doldur
            taskVM = new TaskVM
            {
                TaskId = taskFromDb.TaskId,
                TaskName = taskFromDb.TaskName,
                TaskDescription = taskFromDb.TaskDescription,
                StartDate = taskFromDb.StartDate,
                EndDate = taskFromDb.EndDate,
                TaskStatusName = taskFromDb.Status?.StatusName,
                GitHubPush = taskFromDb.GitHubPush,
                TeamLeadName = $"{taskFromDb.TeamLead.UserFName} {taskFromDb.TeamLead.UserLName}",
                AssignedUserName = $"{taskFromDb.AssignedUser.UserFName} {taskFromDb.AssignedUser.UserLName}",
                ProjectName = taskFromDb.Project?.ProjectName,
                ProjectId = taskFromDb.ProjectId,
                OnayDurumuId = taskFromDb.OnayDurumu?.OnayDurumuId ?? 0,
                OnayDurumuAdi = taskFromDb.OnayDurumu?.OnayDurumuAdi ?? "Belirtilmemiş",
                CommentText = taskVM.CommentText
            };

            // Yorum eklemek zorunlu değil, ancak varsa eklenir
            if (!string.IsNullOrWhiteSpace(taskVM.CommentText))
            {
                var comment = new Comment
                {
                    CommentText = taskVM.CommentText,
                    CommentDate = DateTime.Now,
                    TeamLeadId = taskFromDb.TeamLeadId.Value,
                    TeamMemberId = taskFromDb.AssignedUserId.Value,
                    TaskId = taskVM.TaskId,
                    ProjectId = taskFromDb.ProjectId,
                    Enabled = true
                };
                _unitOfWork.Comments.Add(comment);

                // CommentId'yi task'a ekle
                //taskFromDb.TaskCommentId = comment.CommentId;
            }

            // Görev durumunu güncelle
            taskFromDb.TaskStatusId = 3; // Task'i "Tamamlandı" durumuna getir
            taskFromDb.OnayDurumuId = 2; // Onay Durumu "Onaylandı" olarak güncelle

            _unitOfWork.Tasks.Update(taskFromDb);
            _unitOfWork.SaveChanges();

            TempData["Success"] = "Görev başarıyla onaylandı!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int taskId, string commentText)
        {
            var teamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var taskFromDb = _unitOfWork.Tasks.GetFirstOrDefault(
                t => t.TaskId == taskId && t.TeamLeadId == teamLeadId, // Sadece bu TeamLead'in görevleri
                includeProperties: "AssignedUser,Project,Status,OnayDurumu");

            if (taskFromDb == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(commentText))
            {
                ModelState.AddModelError("CommentText", "Reddetmek için bir yorum girmelisiniz.");
                return View("Details", new TaskVM
                {
                    TaskId = taskFromDb.TaskId,
                    TaskName = taskFromDb.TaskName,
                    TaskDescription = taskFromDb.TaskDescription,
                    AssignedUserName = taskFromDb.AssignedUser?.UserFName + " " + taskFromDb.AssignedUser?.UserLName,
                    ProjectName = taskFromDb.Project?.ProjectName,
                    TaskStatusName = taskFromDb.Status?.StatusName,
                    OnayDurumuId = taskFromDb.OnayDurumuId ?? 0,
                    OnayDurumuAdi = taskFromDb.OnayDurumu?.OnayDurumuAdi ?? "Belirtilmemiş",
                    StartDate = taskFromDb.StartDate,
                    EndDate = taskFromDb.EndDate,
                    GitHubPush = taskFromDb.GitHubPush,
                    CommentText = commentText
                });
            }

            var comment = new Comment
            {
                CommentText = commentText,
                CommentDate = DateTime.Now,
                TeamLeadId = teamLeadId,
                TeamMemberId = taskFromDb.AssignedUserId.Value,
                TaskId = taskFromDb.TaskId,
                ProjectId = taskFromDb.ProjectId,
                Enabled = true
            };

            _unitOfWork.Comments.Add(comment);

            taskFromDb.TaskStatusId = 2; // Task'i "Tamamlanmadı" durumuna getir
            taskFromDb.OnayDurumuId = 3; // Onay Durumu "Reddedildi" olarak güncelle

            _unitOfWork.Tasks.Update(taskFromDb);
            _unitOfWork.SaveChanges();

            TempData["Success"] = "Görev başarıyla reddedildi!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetTeamMembersByProject(int projectId)
        {
            var users = _unitOfWork.UserTeams.GetAll(ut => ut.ProjectId == projectId && ut.RoleId == 3 && ut.User.Enabled, includeProperties: "User")
                .Select(ut => new SelectListItem
                {
                    Text = ut.User.UserFName + " " + ut.User.UserLName,
                    Value = ut.User.UserId.ToString()
                }).ToList();

            return Json(users);
        }

    }
}
