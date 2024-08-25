using Microsoft.AspNetCore.Mvc;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models.ViewModels;

namespace ProjeTakipUygulaması.Areas.TeamMember.Controllers
{
    [Area("TeamMember")]
    public class TaskManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskManagementController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // Oturumdaki kullanıcıyı al
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            // Kullanıcıya atanan görevleri al
            var tasks = _unitOfWork.Tasks.GetAll(t => t.AssignedUserId == userId && t.Enabled, includeProperties: "Status,OnayDurumu");
            var taskVMs = tasks.Select(t => new TaskVM
            {
                TaskId = t.TaskId,
                TaskName = t.TaskName,
                TaskDescription = t.TaskDescription,
                TaskStatusId = t.TaskStatusId,
                TaskStatusName = t.Status.StatusName,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                OnayDurumuId = t.OnayDurumu?.OnayDurumuId ?? 4,
                OnayDurumuAdi = t.OnayDurumu?.OnayDurumuAdi ?? "Onay Durumu Belirtilmemiş",
            }).ToList();

            return View(taskVMs);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var task = _unitOfWork.Tasks.GetFirstOrDefault(
                t => t.TaskId == id,
                includeProperties: "TeamLead,AssignedUser,Status,Project,Comments");

            if (task == null)
            {
                return NotFound();
            }
            var latestComment = task.Comments?.OrderByDescending(c => c.CommentDate).FirstOrDefault();

            var taskVM = new TaskVM
            {
                TaskId = task.TaskId,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                TaskStatusName = task.Status.StatusName,
                GitHubPush = task.GitHubPush,
                TeamLeadName = $"{task.TeamLead.UserFName} {task.TeamLead.UserLName}",
                AssignedUserName = $"{task.AssignedUser.UserFName} {task.AssignedUser.UserLName}",
                ProjectName = task.Project?.ProjectName,
                OnayDurumuId = task.OnayDurumuId ?? 0,
                OnayDurumuAdi = task.OnayDurumu != null ? task.OnayDurumu.OnayDurumuAdi : "Belirtilmemiş",
                CommentText = latestComment?.CommentText ?? "Yorum bulunmuyor." // Görev ile ilgili yorum
            };

            return View(taskVM);
        }

        [HttpGet]
        public IActionResult Complete(int id)
        {
            var task = _unitOfWork.Tasks.GetFirstOrDefault(t => t.TaskId == id);

            if (task == null)
            {
                return NotFound();
            }

            var taskVM = new TaskVM
            {
                TaskId = task.TaskId,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                GitHubPush = task.GitHubPush
            };

            return View(taskVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Complete(TaskVM taskVM)
        {
            if (ModelState.IsValid)
            {
                var taskFromDb = _unitOfWork.Tasks.GetFirstOrDefault(t => t.TaskId == taskVM.TaskId);

                if (taskFromDb == null)
                {
                    return NotFound();
                }

                taskFromDb.GitHubPush = taskVM.GitHubPush;
                taskFromDb.OnayDurumuId = 1; // Onay Durumu "Onay Bekliyor" olarak güncellenir

                _unitOfWork.Tasks.Update(taskFromDb);
                _unitOfWork.SaveChanges();

                TempData["Success"] = "Görev başarıyla gönderildi, onay bekleniyor.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Görev gönderilirken bir hata oluştu. Lütfen tekrar deneyin.";
            return View(taskVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateGitHubLink(int taskId, string gitHubLink)
        {
            if (string.IsNullOrEmpty(gitHubLink))
            {
                ModelState.AddModelError("GitHubLink", "Lütfen GitHub Push linkini ekleyiniz.");
                TempData["Error"] = "GitHub Push linki boş bırakılamaz.";
                return RedirectToAction("Details", new { id = taskId });
            }

            var taskFromDb = _unitOfWork.Tasks.GetFirstOrDefault(t => t.TaskId == taskId);

            if (taskFromDb == null)
            {
                TempData["Error"] = "Görev bulunamadı.";
                return NotFound();
            }

            taskFromDb.GitHubPush = gitHubLink;
            _unitOfWork.Tasks.Update(taskFromDb);
            _unitOfWork.SaveChanges();

            TempData["Success"] = "GitHub Push linki başarıyla güncellendi.";
            return RedirectToAction("Details", new { id = taskId });
        }

    }
}
