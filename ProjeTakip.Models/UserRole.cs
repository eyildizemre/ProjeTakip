using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class UserRole
	{
		[Key]
		public int UserRoleId { get; set; }

		[ForeignKey("UserId")]
		public int UserId { get; set; }

		[ForeignKey("RoleId")]
		public int RoleId { get; set; }

		// Navigasyon Özellikleri
		public User User { get; set; }
		public Role Role { get; set; }
	}

}
