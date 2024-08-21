﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class Project // Project tablosu
	{
		[Key]
		public int ProjectId { get; set; }

		[MaxLength(200)]
		[Display(Name = "Proje İsmi")]
		public string ProjectName { get; set; }

		[ForeignKey("TeamId")]
		public int TeamId { get; set; }

		[ForeignKey("UserId")]
		public int TeamLead { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		[ForeignKey("StatusId")]
		public int ProjectStatusId { get; set; }

		public bool Enabled { get; set; }
	}
}
