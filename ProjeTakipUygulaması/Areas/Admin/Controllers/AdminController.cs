using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;
using ProjeTakipUygulaması.Services;

namespace ProjeTakipUygulaması.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CalendarService _calendarService;

        public AdminController(IUnitOfWork unitOfWork, CalendarService calendarService)
        {
            _unitOfWork = unitOfWork;
            _calendarService = calendarService;
        }

        public IActionResult Index()
        {
            // CalendarService kullanarak takvim olaylarını alıyoruz
            var events = _calendarService.GetCalendarEvents();

            // Tüm projeleri alıyoruz
            var allProjects = _unitOfWork.Projects.GetAll().ToList();

            // Projelerin durumlarına göre filtreleme yapıyoruz
            var activeProjects = allProjects.Where(p => p.Enabled).ToList();
            var completedProjects = allProjects.Where(p => p.ProjectStatusId == 3).ToList();
            var ongoingProjects = allProjects.Where(p => p.ProjectStatusId == 2).ToList();

            // Tüm görevleri alıyoruz
            var allTasks = _unitOfWork.Tasks.GetAll().ToList();

            // Görevlerin durumlarına göre filtreleme yapıyoruz
            var activeTasks = allTasks.Where(t => t.Enabled).ToList();
            var completedTasks = allTasks.Where(t => t.TaskStatusId == 3).ToList();
            var inProgressTasks = allTasks.Where(t => t.TaskStatusId == 2).ToList();
            var incompleteTasks = allTasks.Where(t => t.TaskStatusId == 4).ToList();

            // Aktif ekiplerin sayısını alıyoruz (Enabled = true olanlar)
            var activeTeamCount = _unitOfWork.Teams.GetAll().Count(t => t.Enabled);

            // İstatistikleri ViewModel'e yüklüyoruz
            var viewModel = new AdminDashboardVM
            {
                ActiveProjects = activeProjects,
                ActiveTasks = activeTasks,
                ActiveProjectCount = activeProjects.Count,
                CompletedProjectCount = completedProjects.Count,
                OngoingProjectCount = ongoingProjects.Count,

                TaskCount = allTasks.Count,
                ActiveTaskCount = activeTasks.Count,
                CompletedTaskCount = completedTasks.Count,
                InProgressTaskCount = inProgressTasks.Count,
                IncompleteTaskCount = incompleteTasks.Count,

                ActiveTeamCount = activeTeamCount, // Aktif ekip sayısı
                CalendarEvents = events // Takvim olayları (CalendarService'den gelen veriler)
            };

            return View(viewModel);
        }

    }
}

