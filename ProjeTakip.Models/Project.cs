using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class Project
	{
		[Key]
		public int ProjectId { get; set; }

		[MaxLength(200)]
		[Display(Name = "Proje İsmi")]
		public string ProjectName { get; set; }

		[Display(Name = "Proje Açıklaması")]
        public string ProjectDescription { get; set; }

        [ForeignKey("TeamId")]
		public int? TeamId { get; set; }

		[ForeignKey("UserId")]
		public int? TeamLeadId { get; set; } // Foreign key alanını int olarak tutuyoruz

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

        [MaxLength(255)]
        public string? GitHubPush { get; set; }

        [ForeignKey("StatusId")]
		public int ProjectStatusId { get; set; } = 1; // Team Lead ilk görevi verdiği zaman proje durumu 2 olacak
        [ForeignKey("OnayDurumuId")]
        public int? OnayDurumuId { get; set; }
        public bool Enabled { get; set; }

        // Navigasyon Özellikleri
        [ForeignKey("TeamId")]
        public Team Team { get; set; }
		public User TeamLead { get; set; }
		[ForeignKey("ProjectStatusId")]
		public Status Status { get; set; }
		 // TeamLead alanını User türünde tanımlıyoruz

        public ICollection<Team> Teams { get; set; } // Proje ile ilişkili takımlar
        public ICollection<Comment> Comments { get; set; } // Proje ile ilişkili yorumlar
        public ICollection<Görev> Tasks { get; set; } // Proje ile ilişkili görevler
        public ICollection<UserTeam> UserTeams { get; set; }
        public OnayDurumu OnayDurumu { get; set; }
    }
}
