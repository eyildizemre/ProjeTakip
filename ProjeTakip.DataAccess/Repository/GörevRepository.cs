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
	public class GörevRepository : Repository<Görev>, IGörevRepository
	{
		private readonly ProjeDbContext _context;

		public GörevRepository(ProjeDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(Görev görev)
		{
			var objFromDb = _context.Tasks.FirstOrDefault(t => t.TaskId == görev.TaskId);
			if (objFromDb != null)
			{
				objFromDb.TaskName = görev.TaskName;
				objFromDb.TaskDescription = görev.TaskDescription;
				objFromDb.TeamLeadId = görev.TeamLeadId;
                objFromDb.AssignedUserId = görev.AssignedUserId;
                objFromDb.StartDate = görev.StartDate;
				objFromDb.EndDate = görev.EndDate;
				objFromDb.GitHubPush = görev.GitHubPush;
				objFromDb.TaskStatusId = görev.TaskStatusId;
				objFromDb.TaskCommentId = görev.TaskCommentId;
				objFromDb.ProjectId = görev.ProjectId;
                objFromDb.OnayDurumu = görev.OnayDurumu;
                objFromDb.Enabled = görev.Enabled;
			}
		}
	}
}
