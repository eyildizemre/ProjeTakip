using Microsoft.EntityFrameworkCore;
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
            var tasks = _unitOfWork.Tasks.GetAll()
                .AsQueryable()
                .Include(t => t.Status)
                .ToList(); // Sorguyu burada çalıştırıyoruz

            var events = tasks.Select(t => new CalendarEvent
            {
                Title = t.TaskName,
                Start = t.StartDate.ToString("yyyy-MM-dd"),
                End = t.EndDate.ToString("yyyy-MM-dd"),
                BackgroundColor = t.Status?.StatusColor ?? "#cccccc" // Null kontrolünü burada yapıyoruz
            }).ToList();

            return events;
        }
    }
}
