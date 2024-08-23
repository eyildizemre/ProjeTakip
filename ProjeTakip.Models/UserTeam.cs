using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class UserTeam
	{
		[Key]
		public int No { get; set; }

		[ForeignKey("UserId")]
		public int UserId { get; set; }

		[ForeignKey("TeamId")]
		public int TeamId { get; set; }

		[ForeignKey("RoleId")]
		public int RoleId { get; set; }

		[MaxLength(7)]
		public string? UserColor { get; set; }

		public bool Enabled { get; set; }

		// Navigasyon Özellikleri
		public User User { get; set; }
		public Team Team { get; set; }
		public Role Role { get; set; }
	}
}
