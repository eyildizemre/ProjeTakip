using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class User // User tablosu için model sınıfı
	{
		[Key]
		public int UserId { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Kullanıcı Adı")]
		public string UserFName { get; set; }

        [Required]
        [MaxLength(100)]
		[Display(Name = "Kullanıcı Soyadı")]
		public string UserLName { get; set; }

        [Required]
        [MaxLength(255)]
		[Display(Name = "Kullanıcı E-Posta")]
		public string UserEmail { get; set; }

		public string UserSalt { get; set; }
		public string UserHash { get; set; }

		[MaxLength(255)]
		[Display(Name = "Kullanıcı GitHub Profili")]
		public string? GitHubProfile { get; set; }

		public bool Enabled { get; set; }

		// Navigasyon Özellikleri
		public ICollection<UserRole> UserRoles { get; set; }
		public ICollection<UserTeam> UsersTeams { get; set; }
		public ICollection<Project> LeadProjects { get; set; }
		public ICollection<Görev> Tasks { get; set; }
	}

}
