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
	public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
	{
		private readonly ProjeDbContext _context;

		public UserRoleRepository(ProjeDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(UserRole userRole)
		{
			var objFromDb = _context.UserRoles.FirstOrDefault(ur => ur.UserRoleId == userRole.UserRoleId);
			if (objFromDb != null)
			{
				objFromDb.UserId = userRole.UserId;
				objFromDb.RoleId = userRole.RoleId;
			}
		}
	}
}
