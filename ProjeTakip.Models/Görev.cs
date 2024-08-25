using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
    public class Görev
    {
        [Key]
        public int TaskId { get; set; }

        [MaxLength(200)]
        [Display(Name = "Görev İsmi")]
        public string TaskName { get; set; }

        public string? TaskDescription { get; set; }

        [ForeignKey("TeamLeadId")]
        public int? TeamLeadId { get; set; } // Task'i veren kişi (TeamLead)
        public User TeamLead { get; set; } // Task'i veren kişi için navigasyon property

        [ForeignKey("AssignedUserId")]
        public int? AssignedUserId { get; set; } // Task'i alan kişi (TeamMember)
        public User AssignedUser { get; set; } // AssignedUser navigasyon property

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [MaxLength(255)]
        public string? GitHubPush { get; set; }

        [ForeignKey("TaskStatusId")]
        public int TaskStatusId { get; set; } = 2;

        //[ForeignKey("TaskCommentId")]
        //public int? TaskCommentId { get; set; }

        [ForeignKey("ProjectId")]
        public int ProjectId { get; set; } // Görev'in ait olduğu Proje

        [ForeignKey("OnayDurumuId")]
        public int? OnayDurumuId { get; set; }

        public int? CommentId { get; set; }

        public bool Enabled { get; set; }

        // Navigasyon Özellikleri
        public Status Status { get; set; }
        public Comment Comment { get; set; }
        public Project Project { get; set; } // Navigasyon özelliği
        public ICollection<Comment> Comments { get; set; } // Comment ile ilişkili navigasyon özelliği
        public OnayDurumu OnayDurumu { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
