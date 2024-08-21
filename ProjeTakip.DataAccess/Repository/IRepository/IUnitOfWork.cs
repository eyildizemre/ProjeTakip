using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProjectRepository Projects { get; }
        IGörevRepository Tasks { get; }
        IRoleRepository Roles { get; }
        IStatusRepository Statuses { get; }
        ITeamRepository Teams { get; }
        IUserRoleRepository UserRoles { get; }
        IUserTeamRepository UserTeams { get; }
        ICommentRepository Comments { get; }
        INotificationRepository Notifications { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
