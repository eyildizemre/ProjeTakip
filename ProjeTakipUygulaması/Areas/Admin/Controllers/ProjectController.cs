using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;

namespace ProjeTakipUygulaması.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var projects = _unitOfWork.Projects.GetAll(includeProperties: "Team");
            return View(projects);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProjectVM
            {
                Teams = _unitOfWork.Teams.GetAll().Select(t => new SelectListItem
                {
                    Text = t.TeamName,
                    Value = t.TeamId.ToString()
                })
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectVM model)
        {
            if (ModelState.IsValid)
            {
                var project = new Project
                {
                    ProjectName = model.ProjectName,
                    ProjectDescription = model.ProjectDescription,
                    TeamId = model.TeamId,
                    TeamLeadId = model.TeamLeadId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ProjectStatusId = 1, // Proje oluşturulurken varsayılan olarak "Not Started" durumu
                    Enabled = true
                };

                _unitOfWork.Projects.Add(project);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.Teams = _unitOfWork.Teams.GetAll().Select(t => new SelectListItem
            {
                Text = t.TeamName,
                Value = t.TeamId.ToString()
            });

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var project = _unitOfWork.Projects.GetFirstOrDefault(p => p.ProjectId == id, includeProperties: "Team");
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
                Teams = _unitOfWork.Teams.GetAll().Select(t => new SelectListItem
                {
                    Text = t.TeamName,
                    Value = t.TeamId.ToString(),
                    Selected = (project.TeamId == t.TeamId)
                })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectVM model)
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
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.Teams = _unitOfWork.Teams.GetAll().Select(t => new SelectListItem
            {
                Text = t.TeamName,
                Value = t.TeamId.ToString(),
                Selected = (model.TeamId == t.TeamId)
            });

            return View(model);
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

            return RedirectToAction(nameof(Index));
        }

        // TeamLead projeyi onaylamak için gönderdiğinde submit edilecek action bu olacak
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitProjectForApproval(int id)
        {
            var project = _unitOfWork.Projects.GetFirstOrDefault(p => p.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            // Projeyi onay için gönderildi olarak işaretle
            project.ProjectStatusId = 3; // "Pending Approval"
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Approve(int id)
        {
            var project = _unitOfWork.Projects.GetFirstOrDefault(p => p.ProjectId == id, includeProperties: "Team");
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
                // Diğer alanlar burada eklenebilir
            };

            return View(model); // "Approve" view'unu döndürüyoruz
        }


        // Admin projeyi onayladığında veya reddettiğinde submit edilecek action da bu olacak
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
                // View'a projeyi ve mevcut durumu geri gönder
                var model = new ProjectVM
                {
                    ProjectId = project.ProjectId,
                    ProjectName = project.ProjectName,
                    ProjectDescription = project.ProjectDescription,
                    ProjectStatusId = project.ProjectStatusId,
                    ProjectStatusName = project.Status?.StatusName,
                    // Diğer alanları da doldurmanız gerekebilir
                };
                return View(model); // Geri döndüğünüz view için uygun ismi belirtin
            }

            // Admin onayı durumu
            project.ProjectStatusId = isApproved ? 4 : 5; // "Completed" veya "Rejected"
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();

            if (!isApproved)
            {
                // Yorum ekleme
                var commentEntry = new Comment
                {
                    ProjectId = id,
                    CommentText = comment,
                    CommentDate = DateTime.Now,
                    // Diğer gerekli alanlar
                };
                _unitOfWork.Comments.Add(commentEntry);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
