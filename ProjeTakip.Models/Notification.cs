using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
	public class Notification
	{
		[Key]
		public int NotificationId { get; set; }

		[ForeignKey("UserId")]
		public int UserId { get; set; }

		public string Message { get; set; }

		public DateTime CreatedAt { get; set; }

		public bool? IsRead { get; set; } = false;

        // Navigasyon Özelliği
        public User User { get; set; }
	}

}
