using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class Role // Role tablosu
	{
		[Key]
		public int RoleId { get; set; }

		[MaxLength(50)]
		[Display(Name = "Rol Adı")]
		public string RoleName { get; set; }
	}
}
