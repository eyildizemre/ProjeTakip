using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class Görev
	{
		[Key]
		public int TaskId { get; set; }

		[MaxLength(200)]
		[Display(Name = "Görev İsmi")]
		public string TaskName { get; set; }

		public string TaskDescription { get; set; }

		[ForeignKey("UserId")]
		public int UserId { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		[MaxLength(255)]
		public string GitHubPush { get; set; }

		[ForeignKey("TaskStatusId")]
		public int TaskStatusId { get; set; }

		[ForeignKey("TaskCommentId")]
		public int TaskCommentId { get; set; }

		public bool Enabled { get; set; }

		// Navigasyon Özellikleri
		public User User { get; set; }
		public Status Status { get; set; }
		public Comment Comment { get; set; }
	}

}
