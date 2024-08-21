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
	public class RoleRepository : Repository<Role>, IRoleRepository
	{
		private readonly ProjeDbContext _context;

		public RoleRepository(ProjeDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(Role role)
		{
			var objFromDb = _context.Roles.FirstOrDefault(r => r.RoleId == role.RoleId);
			if (objFromDb != null)
			{
				objFromDb.RoleName = role.RoleName;
			}
		}
	}
}
