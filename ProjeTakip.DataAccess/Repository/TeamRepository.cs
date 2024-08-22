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
	public class TeamRepository : Repository<Team>, ITeamRepository
	{
		private readonly ProjeDbContext _context;

		public TeamRepository(ProjeDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(Team team)
		{
			var objFromDb = _context.Teams.FirstOrDefault(t => t.TeamId == team.TeamId);
			if (objFromDb != null)
			{
				objFromDb.TeamName = team.TeamName;
				objFromDb.TeamLeadId = team.TeamLeadId;
                objFromDb.ProjectId = team.ProjectId;
                objFromDb.Capacity = team.Capacity;
                objFromDb.Enabled = team.Enabled;
			}
		}

		public IEnumerable<Team> GetUsersWithTasks()
		{
			return _context.Teams.Include(t => t.UserTeams).ThenInclude(ut => ut.User).Include(t => t.Projects).ToList();
		}
	}
}