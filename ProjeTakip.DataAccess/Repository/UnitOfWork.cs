using ProjeTakip.DataAccess.Data;
using ProjeTakip.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ProjeDbContext _db;

		public UnitOfWork(ProjeDbContext db)
		{
			_db = db;
			Users = new UserRepository(_db);
			Projects = new ProjectRepository(_db);
			Tasks = new GörevRepository(_db);
			Roles = new RoleRepository(_db);
			Statuses = new StatusRepository(_db);
			Teams = new TeamRepository(_db);
			UserRoles = new UserRoleRepository(_db);
			UserTeams = new UserTeamRepository(_db);
			Comments = new CommentRepository(_db);
			Notifications = new NotificationRepository(_db);
		}

		public IUserRepository Users { get; private set; }
		public IProjectRepository Projects { get; private set; }
		public IGörevRepository Tasks { get; private set; }
		public IRoleRepository Roles { get; private set; }
		public IStatusRepository Statuses { get; private set; }
		public ITeamRepository Teams { get; private set; }
		public IUserRoleRepository UserRoles { get; private set; }
		public IUserTeamRepository UserTeams { get; private set; }
		public ICommentRepository Comments { get; private set; }
		public INotificationRepository Notifications { get; private set; }

		public int SaveChanges()
		{
			return _db.SaveChanges();
		}

		public void Dispose()
		{
			_db.Dispose();
		}
	}
}
