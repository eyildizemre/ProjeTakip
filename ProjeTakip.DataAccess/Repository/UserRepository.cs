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
	public class UserRepository : Repository<User>, IUserRepository
	{
		private readonly ProjeDbContext _context;

		public UserRepository(ProjeDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(User user)
		{
			var objFromDb = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
			if (objFromDb != null)
			{
				objFromDb.UserFName = user.UserFName;
				objFromDb.UserLName = user.UserLName;
				objFromDb.UserEmail = user.UserEmail;

				// Şifreyi hashleyip saltladıktan sonra kaydetme işlemi
				objFromDb.UserSalt = user.UserSalt; // Saltı kaydeder
				objFromDb.UserHash = user.UserHash; // Hashlenmiş şifreyi kaydeder

                objFromDb.GitHubProfile = user.GitHubProfile;
                objFromDb.Enabled = user.Enabled;
            }
		}

		public IEnumerable<User> GetUsersWithTasks()
		{
			return _context.Users.Include(u => u.Tasks).ToList();
		}
	}


}
