using Microsoft.EntityFrameworkCore;
using ProjeTakip.DataAccess.Data;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.DataAccess.Repository
{
	public class NotificationRepository : Repository<Notification>, INotificationRepository
	{
		private readonly ProjeDbContext _context;

		public NotificationRepository(ProjeDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(Notification notification)
		{
			var objFromDb = _context.Notifications.FirstOrDefault(n => n.NotificationId == notification.NotificationId);
			if (objFromDb != null)
			{
				objFromDb.SentById = notification.SentById;
				objFromDb.ReceivedById = notification.ReceivedById;
				objFromDb.Subject = notification.Subject;
				objFromDb.Message = notification.Message;
				objFromDb.CreatedAt = notification.CreatedAt;
				objFromDb.IsRead = notification.IsRead;
				objFromDb.TaskId = notification.TaskId;
				objFromDb.ProjectId = notification.ProjectId;
                objFromDb.Enabled = notification.Enabled;
            }
		}
	}
}
