using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class Team // Team tablosu
	{
		[Key]
		public int TeamId { get; set; }

		[MaxLength(100)]
		[Display(Name = "Ekip Adı")]
		public string TeamName { get; set; }

		[ForeignKey("UserId")]
		public int TeamLead { get; set; }

		public bool Enabled { get; set; }
	}
}
