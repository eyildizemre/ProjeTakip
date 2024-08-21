using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class Team
	{
		[Key]
		public int TeamId { get; set; }

		[MaxLength(100)]
		[Display(Name = "Ekip Adı")]
		public string TeamName { get; set; }

		[ForeignKey("UserId")]
		public int TeamLead { get; set; }

		public bool Enabled { get; set; }

		// Navigasyon Özellikleri
		public ICollection<UserTeam> UsersTeams { get; set; }
		public ICollection<Project> Projects { get; set; }
	}

}
