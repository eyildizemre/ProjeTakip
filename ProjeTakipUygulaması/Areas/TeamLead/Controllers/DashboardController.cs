using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;
using ProjeTakip.Utility;
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
            var user = HttpContext.Session.GetString("UserId");
            var userId = Convert.ToInt32(user);

            // TeamLead'in yönettiği projeleri al
            var projects = _unitOfWork.Projects.GetAll(p => p.TeamLeadId == userId && p.Enabled, includeProperties: "Team,Status");

            // TeamLead'e atanmış görevleri al
            var tasks = _unitOfWork.Tasks.GetAll(t => t.TeamLeadId == userId && t.Enabled, includeProperties: "Status");

            var calendarEvents = new List<CalendarEvent>();

            foreach (var project in projects)
            {
                var daysUntilDue = (project.EndDate - DateTime.Now).Days;
                bool isCritical = daysUntilDue <= 7;

                calendarEvents.Add(new CalendarEvent
                {
                    Title = project.ProjectName,
                    Start = project.StartDate.ToString("yyyy-MM-dd"),
                    End = project.EndDate.AddDays(1).ToString("yyyy-MM-dd"),  // Bitiş tarihini bir gün uzatıyoruz
                    BackgroundColor = isCritical ? "red" : HelperFunctions.GetRandomColor(),
                    BorderColor = isCritical ? "red" : HelperFunctions.GetRandomColor(),
                    IsCritical = isCritical,
                    IsEnabled = project.Enabled
                });
            }

            foreach (var task in tasks)
            {
                var daysUntilDue = (task.EndDate - DateTime.Now).Days;
                bool isCritical = daysUntilDue <= 7;

                calendarEvents.Add(new CalendarEvent
                {
                    Title = task.TaskName,
                    Start = task.StartDate.ToString("yyyy-MM-dd"),
                    End = task.EndDate.AddDays(1).ToString("yyyy-MM-dd"),  // Bitiş tarihini bir gün uzatıyoruz
                    BackgroundColor = isCritical ? "red" : "green",
                    BorderColor = isCritical ? "red" : "green",
                    IsCritical = isCritical,
                    IsEnabled = task.Enabled
                });
            }
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
