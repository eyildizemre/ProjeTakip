using ProjeTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.DataAccess.Repository.IRepository
{
	public interface IUserRepository : IRepository<User>
	{
		void Update(User user); // User nesnesini güncellemek için özel bir metot
		IEnumerable<User> GetUsersWithTasks(); // Task'leri ile birlikte User'ları almak için bir örnek metot

	}

}
