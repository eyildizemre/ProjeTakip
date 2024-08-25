using Microsoft.AspNetCore.Mvc;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using ProjeTakip.Models.ViewModels;
using ProjeTakipUygulaması.Services;

namespace ProjeTakipUygulaması.Areas.TeamMember.Controllers
{
    [Area("TeamMember")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CalendarService _calendarService;

        public DashboardController(IUnitOfWork unitOfWork, CalendarService calendarService)
        {
            _unitOfWork = unitOfWork;
            _calendarService = calendarService;
        }

        public IActionResult Index()
        {
            // Oturumdaki kullanıcıyı al
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            int userIdInt = Convert.ToInt32(userId);

            // Bu kullanıcıya atanan görevleri al
            var tasks = _unitOfWork.Tasks.GetAll(t => t.TeamLeadId == userIdInt && t.Enabled, includeProperties: "Status");

            // CalendarService'i kullanarak takvim etkinliklerini oluştur
            var calendarEvents = tasks.Select(t => new CalendarEvent
            {
                Title = t.TaskName,
                Start = t.StartDate.ToString("yyyy-MM-dd"),
                End = t.EndDate.ToString("yyyy-MM-dd"),
                BackgroundColor = t.Status.StatusColor
            }).ToList();

            var viewModel = new TMDashboardVM
            {
                CalendarEvents = calendarEvents
            };

            return View(viewModel);
        }
    }
}
