using Microsoft.EntityFrameworkCore;
using ProjeTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.DataAccess.Data
{
	public class ProjeDbContext : DbContext
	{
		public ProjeDbContext(DbContextOptions<ProjeDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<Team> Teams { get; set; }
		public DbSet<UsersTeam> UsersTeams { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<Görev> Tasks { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Status> Status { get; set; }
		public DbSet<Notification> Notifications { get; set; }
	}
}
