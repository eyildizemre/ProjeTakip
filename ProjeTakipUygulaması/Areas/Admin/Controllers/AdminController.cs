using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;
using ProjeTakip.Utility;
using ProjeTakipUygulaması.Services;
using System.Drawing;

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

            var events = activeProjects.Select(p => new CalendarEvent
            {
                Title = p.ProjectName,
                Start = p.StartDate.ToString("yyyy-MM-dd"),
                End = p.EndDate.ToString("yyyy-MM-dd"),
                IsCritical = (p.EndDate - DateTime.Now).TotalDays < 7, // Bir haftadan az kalan projeler
                BackgroundColor = (p.EndDate - DateTime.Now).TotalDays < 7 ? "red" : HelperFunctions.GetRandomColor(),
                BorderColor = (p.EndDate - DateTime.Now).TotalDays < 7 ? "red" : HelperFunctions.GetRandomColor(),
                IsEnabled = p.Enabled
            }).ToList();

            // Aktif ekiplerin sayısını alıyoruz (Enabled = true olanlar)
            var activeTeamCount = _unitOfWork.Teams.GetAll().Count(t => t.Enabled);

            // Teslim tarihine kalan gün sayısını hesaplıyoruz
            var projectDueDateInfos = activeProjects.Select(p => new ProjectDueDateInfo
            {
                ProjectName = p.ProjectName,
                DaysUntilDueDate = (p.EndDate - DateTime.Now).Days
            }).ToList();

            var taskDueDateInfos = activeTasks.Select(t => new TaskDueDateInfo
            {
                TaskName = t.TaskName,
                DaysUntilDueDate = (t.EndDate - DateTime.Now).Days
            }).ToList();

            // Onay bekleyen projelerin sayısını alıyoruz
            var pendingApprovalProjects = _unitOfWork.Projects.GetAll(p => p.OnayDurumuId == 1).ToList();
            var pendingApprovalProjectCount = pendingApprovalProjects.Count;

            // Admin'in ismini alıyoruz
            var adminName = HttpContext.Session.GetString("UserFName");

            var projectMessages = new List<MessageWithColors>();
            foreach (var project in activeProjects)
            {
                var daysLeft = (project.EndDate - DateTime.Now).Days;
                string message;
                string color;

                if (daysLeft < 0)
                {
                    message = $"{project.ProjectName} isimli projenin teslim tarihi geçti.";
                    color = "red"; // Teslim tarihi geçtiyse kırmızı
                }
                else if (daysLeft == 0)
                {
                    message = $"{project.ProjectName} isimli projenin bugün bitene kadar teslim edilmesi gerekiyor.";
                    color = "orangered"; // Bugün bitmesi gereken proje için turuncu
                }
                else if (daysLeft < 7)
                {
                    message = $"{project.ProjectName} isimli projenin bitimine {daysLeft} gün kaldı.";
                    color = "orange"; // Yedi günden fazla kaldıysa yeşil
                }
                else
                {
                    message = $"{project.ProjectName} isimli projenin bitimine {daysLeft} gün kaldı.";
                    color = "green";
                }

                projectMessages.Add(new MessageWithColors { Message = message, Color = color });
            }

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
                CalendarEvents = events, // Takvim olayları (CalendarService'den gelen veriler)
                PendingProjectCount = pendingApprovalProjectCount,
                AdminName = adminName,
                ProjectMessagesWithColor = projectMessages,
                ProjectDueDateInfos = projectDueDateInfos,
                TaskDueDateInfos = taskDueDateInfos
            };
            // Onay bekleyen projeler varsa, Hoşgeldin mesajını güncelliyoruz
            if (pendingApprovalProjectCount > 0)
            {
                ViewBag.WelcomeMessage = $"Hoşgeldin, {HttpContext.Session.GetString("UserFName")}. Onaylanmayı bekleyen {pendingApprovalProjectCount} proje var.";
            }
            else
            {
                ViewBag.WelcomeMessage = $"Hoşgeldin, {HttpContext.Session.GetString("UserFName")}.";
            }

            return View(viewModel);
        }
    }
}

