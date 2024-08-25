using Microsoft.AspNetCore.Mvc;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;

namespace ProjeTakipUygulaması.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Kullanıcının bildirimlerini listele
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcının ilgili olduğu bildirimleri alıyoruz
            var notifications = _unitOfWork.Notifications.GetAll(n => n.ReceivedById == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(notifications);
        }

        // Bildirimi okundu olarak işaretle
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = _unitOfWork.Notifications.GetFirstOrDefault(n => n.NotificationId == id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Yeni bildirim oluşturma (görev, yorum vb. durumlar için kullanılabilir)
        public async Task<IActionResult> CreateNotification(int receivedById, int? sentById, string message, int? taskId = null, int? projectId = null)
        {
            var notification = new Notification
            {
                ReceivedById = receivedById,
                SentById = sentById,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false,
                TaskId = taskId,
                ProjectId = projectId
            };

            _unitOfWork.Notifications.Add(notification);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
