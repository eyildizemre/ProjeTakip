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

        // Bildirimi oluşturan kişi (örn. TeamMember, Admin)
        [ForeignKey("CommentedById")]
        public int? CommentedById { get; set; }
        public User CommentedBy { get; set; }

        // Bildirimin ilgili olduğu kişi (örn. TeamLead, Admin)
        [ForeignKey("CommentedAtId")]
        public int? CommentedAtId { get; set; }
        public User CommentedAt { get; set; }

        // Bildirimin metni
        public string Message { get; set; }

        // Bildirimin oluşturulma tarihi
        public DateTime CreatedAt { get; set; }

        // Bildirimin okunma durumu
        public bool? IsRead { get; set; } = false;
    }
}
