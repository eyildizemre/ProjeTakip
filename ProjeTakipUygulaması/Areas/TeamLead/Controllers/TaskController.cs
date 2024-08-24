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
            var tasks = _unitOfWork.Tasks.GetAll(t => t.Enabled, includeProperties: "AssignedUser,Project,Status").ToList();

            var model = tasks.Select(t => new TaskVM
            {
                TaskId = t.TaskId,
                TaskName = t.TaskName,
                TaskDescription = t.TaskDescription,
                UserId = t.UserId,
                UserFullName = t.AssignedUser != null ? t.AssignedUser.UserFName + " " + t.AssignedUser.UserLName : "Atanmamış",
                ProjectId = t.ProjectId,
                ProjectName = t.Project.ProjectName,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                TaskStatusName = t.Status.StatusName
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new TaskVM
            {
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
                    UserId = model.UserId,
                    ProjectId = model.ProjectId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    TaskStatusId = 2,
                    Enabled = true
                };

                task.AssignedUser = _unitOfWork.Users.GetFirstOrDefault(u => u.UserId == model.UserId);

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
                UserId = task.UserId,
                ProjectId = task.ProjectId,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                GitHubPush = task.GitHubPush,
                TaskStatusId = task.TaskStatusId,
                Users = _unitOfWork.Users.GetAll(u => u.Enabled).Select(u => new SelectListItem
                {
                    Text = u.UserFName + " " + u.UserLName,
                    Value = u.UserId.ToString(),
                    Selected = (task.UserId == u.UserId)
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
                task.UserId = model.UserId;
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
                Selected = (model.UserId == u.UserId)
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
                UserFullName = task.AssignedUser?.UserFName + " " + task.AssignedUser?.UserLName,
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
                UserFullName = task.AssignedUser?.UserFName + " " + task.AssignedUser?.UserLName,
                ProjectName = task.Project?.ProjectName,
                TaskStatusName = task.Status?.StatusName,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                GitHubPush = task.GitHubPush
            };

            return View(model);
        }
    }
}
