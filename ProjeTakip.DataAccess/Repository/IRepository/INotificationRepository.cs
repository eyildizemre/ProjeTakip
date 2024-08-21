using ProjeTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.DataAccess.Repository.IRepository
{
	public interface INotificationRepository : IRepository<Notification>
	{
		void Update(Notification notification);
	}

}
