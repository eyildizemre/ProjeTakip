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
            var roleId = HttpContext.Session.GetInt32("RoleId");

            if (userId == null || roleId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcının ilgili olduğu bildirimleri alıyoruz
            var notifications = _unitOfWork.Notifications.GetAll(n => n.CommentedAtId == userId)
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
        public async Task<IActionResult> CreateNotification(int commentedAtId, int? commentedById, string message)
        {
            var notification = new Notification
            {
                CommentedAtId = commentedAtId,
                CommentedById = commentedById,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _unitOfWork.Notifications.Add(notification);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
