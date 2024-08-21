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
	public class UserTeamRepository : Repository<UserTeam>, IUserTeamRepository
	{
		private readonly ProjeDbContext _context;

		public UserTeamRepository(ProjeDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(UserTeam userTeam)
		{
			var objFromDb = _context.UsersTeams.FirstOrDefault(ut => ut.No == userTeam.No);
			if (objFromDb != null)
			{
				objFromDb.UserId = userTeam.UserId;
				objFromDb.TeamId = userTeam.TeamId;
				objFromDb.RoleId = userTeam.RoleId;
				objFromDb.UserColor = userTeam.UserColor;
				objFromDb.Enabled = userTeam.Enabled;
			}
		}
	}
}
