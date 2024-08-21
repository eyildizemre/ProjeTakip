using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class Status // Status tablosu
	{
		[Key]
		public int StatusId { get; set; }

		[MaxLength(50)]
		[Display(Name = "Durum")]
		public string StatusName { get; set; }

		[MaxLength(7)]
		public string StatusColor { get; set; }
	}
}
