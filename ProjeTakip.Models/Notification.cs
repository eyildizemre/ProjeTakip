using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        // Bildirimi oluşturan kişi (örn. TeamLead, Admin)
        [ForeignKey("SentById")]
        public int? SentById { get; set; }
        public User SentBy { get; set; }

        // Bildirimin ilgili olduğu kişi (örn. TeamMember, TeamLead, Admin)
        [ForeignKey("ReceivedById")]
        public int? ReceivedById { get; set; }
        public User ReceivedBy { get; set; }

        // Bildirimin konusu (örn. Görev, Proje) 
        public string Subject { get; set; }

        // Bildirimin metni
        public string Message { get; set; }

        // Bildirimin oluşturulma tarihi
        public DateTime CreatedAt { get; set; }

        // Bildirimin okunma durumu
        public bool? IsRead { get; set; } = false;

        // Görevle İlişkili Bildirim
        [ForeignKey("TaskId")]
        public int? TaskId { get; set; }
        public Görev Task { get; set; }

        // Projeyle İlişkili Bildirim
        [ForeignKey("ProjectId")]
        public int? ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
