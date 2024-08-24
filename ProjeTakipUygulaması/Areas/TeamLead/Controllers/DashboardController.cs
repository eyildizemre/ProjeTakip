using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;
using System.Threading.Tasks;

namespace ProjeTakipUygulaması.Areas.TeamLead.Controllers
{
    [Area("TeamLead")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Giriş yapan kullanıcının ID'sini al
            var userId = HttpContext.Session.GetInt32("UserId");

            // TeamLead'in yönettiği projeleri al
            var projects = _unitOfWork.Projects.GetAll(p => p.TeamLeadId == userId && p.Enabled, includeProperties: "Team,Status");

            // TeamLead'e atanmış görevleri al
            var tasks = _unitOfWork.Tasks.GetAll(t => t.UserId == userId && t.Enabled, includeProperties: "Status");

            // CalendarEvent listesi oluştur
            var calendarEvents = new List<CalendarEvent>();

            // Projeleri CalendarEvent olarak ekle
            foreach (var project in projects)
            {
                calendarEvents.Add(new CalendarEvent
                {
                    Title = project.ProjectName,
                    Start = project.StartDate.ToString("yyyy-MM-dd"),
                    End = project.EndDate.ToString("yyyy-MM-dd"),
                    BackgroundColor = "#007bff", // Proje için renk
                    BorderColor = "#007bff"
                });
            }

            // Görevleri CalendarEvent olarak ekle
            foreach (var task in tasks)
            {
                calendarEvents.Add(new CalendarEvent
                {
                    Title = task.TaskName,
                    Start = task.StartDate.ToString("yyyy-MM-dd"),
                    End = task.EndDate.ToString("yyyy-MM-dd"),
                    BackgroundColor = "#28a745", // Görev için renk
                    BorderColor = "#28a745"
                });
            }

            // Modeli oluştur ve view'a gönder
            var x = HttpContext.Session.GetString("UserFName");
            var model = new TeamLeadVM
            {
                TeamLeadName = x,
                CalendarEvents = calendarEvents
            };

            return View(model);
        }
    }
}
