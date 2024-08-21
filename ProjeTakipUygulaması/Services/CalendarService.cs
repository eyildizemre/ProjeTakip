using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;

namespace ProjeTakipUygulaması.Services
{
    public class CalendarService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CalendarService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<CalendarEvent> GetCalendarEvents()
        {
            var events = _unitOfWork.Tasks.GetAll().Select(t => new CalendarEvent
            {
                Title = t.TaskName,
                Start = t.StartDate.ToString("yyyy-MM-dd"),
                End = t.EndDate.ToString("yyyy-MM-dd"),
                BackgroundColor = t.Status.StatusColor // Renk direkt olarak StatusColor'dan alınıyor
            }).ToList();

            return events;
        }
    }
}
