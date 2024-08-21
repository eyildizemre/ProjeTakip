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
	public class ProjectRepository : Repository<Project>, IProjectRepository
	{
		private readonly ProjeDbContext _context;

		public ProjectRepository(ProjeDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(Project project)
		{
			var objFromDb = _context.Projects.FirstOrDefault(p => p.ProjectId == project.ProjectId);
			if (objFromDb != null)
			{
				objFromDb.ProjectName = project.ProjectName;
				objFromDb.TeamId = project.TeamId;
				objFromDb.TeamLead = project.TeamLead;
				objFromDb.StartDate = project.StartDate;
				objFromDb.EndDate = project.EndDate;
				objFromDb.ProjectStatusId = project.ProjectStatusId;
				objFromDb.Enabled = project.Enabled;
			}
		}
	}
}
