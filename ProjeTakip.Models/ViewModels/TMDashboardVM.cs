using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models.ViewModels
{
    public class TMDashboardVM
    {
        public List<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

        // TeamMember'ın atanmış görevleri
        public List<Görev> AssignedTasks { get; set; } = new List<Görev>();

        // TeamMember'ın profil bilgileri
        public User UserProfile { get; set; }

        // Bildirimler (isteğe bağlı)
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
