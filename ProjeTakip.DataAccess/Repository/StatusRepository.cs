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
	public class StatusRepository : Repository<Status>, IStatusRepository
	{
		private readonly ProjeDbContext _context;

		public StatusRepository(ProjeDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(Status status)
		{
			var objFromDb = _context.Status.FirstOrDefault(s => s.StatusId == status.StatusId);
			if (objFromDb != null)
			{
				objFromDb.StatusName = status.StatusName;
				objFromDb.StatusColor = status.StatusColor;
			}
		}
	}
}
