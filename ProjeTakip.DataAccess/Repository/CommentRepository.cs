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
	public class CommentRepository : Repository<Comment>, ICommentRepository
	{
		private readonly ProjeDbContext _context;

		public CommentRepository(ProjeDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(Comment comment)
		{
			var objFromDb = _context.Comments.FirstOrDefault(c => c.CommentId == comment.CommentId);
			if (objFromDb != null)
			{
				objFromDb.CommentText = comment.CommentText;
				objFromDb.CommentDate = comment.CommentDate;
				objFromDb.TeamLeadId = comment.TeamLeadId;
				objFromDb.TeamMemberId = comment.TeamMemberId;
				objFromDb.Enabled = comment.Enabled;
			}
		}
	}
}
