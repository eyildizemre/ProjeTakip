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
		public int CommentedBy { get; set; } // Bu UserId olmalı

		// Navigasyon Özelliği
		public ICollection<Görev> Tasks { get; set; } // Bu koleksiyon eklendi
	}


}
