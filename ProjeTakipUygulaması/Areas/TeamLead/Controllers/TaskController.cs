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
            var tasks = _unitOfWork.Tasks.GetAll(t => t.Enabled, includeProperties: "AssignedUser,Project,Status,OnayDurumu").ToList();

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
            var model = new TaskVM
            {
                TeamLeadId = Convert.ToInt32(HttpContext.Session.GetString("UserId")),
                Users = _unitOfWork.Users.GetAll(u => u.Enabled).Select(u => new SelectListItem
                {
                    Text = u.UserFName + " " + u.UserLName,
                    Value = u.UserId.ToString()
                }).ToList(),
                Projects = _unitOfWork.Projects.GetAll(p => p.Enabled).Select(p => new SelectListItem
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
            if (ModelState.IsValid)
            {
                var task = new Görev
                {
                    TaskName = model.TaskName,
                    TaskDescription = model.TaskDescription,
                    TeamLeadId = model.TeamLeadId,
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
                    var project = _unitOfWork.Projects.GetFirstOrDefault(p => p.ProjectId == model.ProjectId);
                    if (project != null)
                    {
                        project.ProjectStatusId = 2;
                        _unitOfWork.Projects.Update(project);
                    }
                }

                _unitOfWork.Tasks.Add(task);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // ModelState hatalıysa, gerekli listeleri yeniden yükle
            model.Users = _unitOfWork.Users.GetAll(u => u.Enabled).Select(u => new SelectListItem
            {
                Text = u.UserFName + " " + u.UserLName,
                Value = u.UserId.ToString()
            }).ToList();

            model.Projects = _unitOfWork.Projects.GetAll(p => p.Enabled).Select(p => new SelectListItem
            {
                Text = p.ProjectName,
                Value = p.ProjectId.ToString()
            }).ToList();

            model.Statuses = _unitOfWork.Statuses.GetAll().Select(s => new SelectListItem
            {
                Text = s.StatusName,
                Value = s.StatusId.ToString()
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var task = _unitOfWork.Tasks.GetFirstOrDefault(t => t.TaskId == id, includeProperties: "AssignedUser,Project,Status");
            if (task == null)
            {
                return NotFound();
            }

            var model = new TaskVM
            {
                TaskId = task.TaskId,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                TeamLeadId = (int)task.TeamLeadId, // Burada düzeltildi
                ProjectId = task.ProjectId,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                GitHubPush = task.GitHubPush,
                TaskStatusId = task.TaskStatusId,
                Users = _unitOfWork.Users.GetAll(u => u.Enabled).Select(u => new SelectListItem
                {
                    Text = u.UserFName + " " + u.UserLName,
                    Value = u.UserId.ToString(),
                    Selected = (task.TeamLeadId == u.UserId) // Burada düzeltildi
                }).ToList(),
                Projects = _unitOfWork.Projects.GetAll(p => p.Enabled).Select(p => new SelectListItem
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
            if (ModelState.IsValid)
            {
                var task = _unitOfWork.Tasks.GetFirstOrDefault(t => t.TaskId == model.TaskId);
                if (task == null)
                {
                    return NotFound();
                }

                task.TaskName = model.TaskName;
                task.TaskDescription = model.TaskDescription;
                task.TeamLeadId = model.TeamLeadId; // Burada düzeltildi
                task.ProjectId = model.ProjectId;
                task.StartDate = model.StartDate;
                task.EndDate = model.EndDate;
                task.GitHubPush = model.GitHubPush;
                task.TaskStatusId = model.TaskStatusId;

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            model.Users = _unitOfWork.Users.GetAll(u => u.Enabled).Select(u => new SelectListItem
            {
                Text = u.UserFName + " " + u.UserLName,
                Value = u.UserId.ToString(),
                Selected = (model.TeamLeadId == u.UserId) // Burada düzeltildi
            }).ToList();

            model.Projects = _unitOfWork.Projects.GetAll(p => p.Enabled).Select(p => new SelectListItem
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

            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var task = _unitOfWork.Tasks.GetFirstOrDefault(t => t.TaskId == id, includeProperties: "AssignedUser,Project,Status");
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
            var task = _unitOfWork.Tasks.GetFirstOrDefault(t => t.TaskId == id);
            if (task != null)
            {
                task.Enabled = false;
                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var task = _unitOfWork.Tasks.GetFirstOrDefault(t => t.TaskId == id, includeProperties: "AssignedUser,Project,Status,OnayDurumu");
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
                GitHubPush = task.GitHubPush
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(TaskVM taskVM)
        {
            // Veritabanından görevi çek ve gerekli ilişkili verileri dahil et
            var taskFromDb = _unitOfWork.Tasks.GetFirstOrDefault(
                t => t.TaskId == taskVM.TaskId,
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

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int taskId, string commentText)
        {
            var taskFromDb = _unitOfWork.Tasks.GetFirstOrDefault(t => t.TaskId == taskId, includeProperties: "TeamLead,AssignedUser,Project,Status,OnayDurumu");

            if (taskFromDb == null)
            {
                return NotFound();
            }

            var taskVM = new TaskVM
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
                CommentText = commentText
            };

            // Yorum kontrolü
            if (string.IsNullOrWhiteSpace(commentText))
            {
                ModelState.AddModelError("CommentText", "Reddetmek için bir yorum girmelisiniz.");
                return View("Details", taskVM);
            }

            // Yorum oluştur
            var comment = new Comment
            {
                CommentText = taskVM.CommentText,
                CommentDate = DateTime.Now,
                TeamLeadId = (int)taskFromDb.TeamLeadId,
                TeamMemberId = (int)taskFromDb.AssignedUserId,
                TaskId = taskFromDb.TaskId,
                ProjectId = taskFromDb.ProjectId, // ProjectId doğru şekilde ayarlanıyor
                Enabled = true
            };

            _unitOfWork.Comments.Add(comment);

            //taskFromDb.TaskCommentId = comment.CommentId;
            taskFromDb.TaskStatusId = 2;
            taskFromDb.OnayDurumuId = 3;

            _unitOfWork.Tasks.Update(taskFromDb);
            _unitOfWork.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
