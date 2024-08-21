using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class Status
	{
		[Key]
		public int StatusId { get; set; }

		[MaxLength(50)]
		[Display(Name = "Durum")]
		public string StatusName { get; set; }

		[MaxLength(7)]
		public string StatusColor { get; set; }

		// Navigasyon Özellikleri
		public ICollection<Project> Projects { get; set; }
		public ICollection<Görev> Tasks { get; set; }
	}

}
