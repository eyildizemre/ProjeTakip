using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class Comment
	{
		[Key]
		public int CommentId { get; set; }

		public string CommentText { get; set; }
		public DateTime CommentDate { get; set; }

		[ForeignKey("TeamLead")]
		public int TeamLeadId { get; set; }

		[ForeignKey("TeamMember")]
		public int TeamMemberId { get; set; }

		public bool Enabled { get; set; }

		// Navigasyon Özellikleri
		public User TeamLead { get; set; }
		public User TeamMember { get; set; }

		// Task ile olan ilişkiyi temsil eden ICollection özelliği
		public ICollection<Görev> Tasks { get; set; }
	}

}
