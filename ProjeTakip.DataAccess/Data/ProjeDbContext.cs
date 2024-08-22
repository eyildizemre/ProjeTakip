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
		public DbSet<UserTeam> UsersTeams { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<Görev> Tasks { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Status> Status { get; set; }
		public DbSet<Notification> Notifications { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Fluent API ile ilişkileri tanımlama

			// User - UserRole İlişkisi
			modelBuilder.Entity<UserRole>()
				.HasOne(ur => ur.User)
				.WithMany(u => u.UserRoles)
				.HasForeignKey(ur => ur.UserId);

			modelBuilder.Entity<UserRole>()
				.HasOne(ur => ur.Role)
				.WithMany(r => r.UserRoles)
				.HasForeignKey(ur => ur.RoleId);

			// User - UsersTeam İlişkisi
			modelBuilder.Entity<UserTeam>()
				.HasOne(ut => ut.User)
				.WithMany(u => u.UsersTeams)
				.HasForeignKey(ut => ut.UserId);

			modelBuilder.Entity<UserTeam>()
				.HasOne(ut => ut.Team)
				.WithMany(t => t.UsersTeams)
				.HasForeignKey(ut => ut.TeamId);

			modelBuilder.Entity<UserTeam>()
				.HasOne(ut => ut.Role)
				.WithMany(r => r.UsersTeams)
				.HasForeignKey(ut => ut.RoleId);

			// Team - Project İlişkisi
			modelBuilder.Entity<Project>()
				.HasOne(p => p.Team)
				.WithMany(t => t.Projects)
				.HasForeignKey(p => p.TeamId);

			// User - Project İlişkisi (Team Lead)
			modelBuilder.Entity<Project>()
				.HasOne(p => p.TeamLead)
				.WithMany(u => u.LeadProjects)
				.HasForeignKey(p => p.TeamLeadId);

			// User - Task İlişkisi
			modelBuilder.Entity<Görev>()
				.HasOne(t => t.User)
				.WithMany(u => u.Tasks)
				.HasForeignKey(t => t.UserId);

			// Status - Project İlişkisi
			modelBuilder.Entity<Project>()
				.HasOne(p => p.Status)
				.WithMany(s => s.Projects)
				.HasForeignKey(p => p.ProjectStatusId);

			// Status - Task İlişkisi
			modelBuilder.Entity<Görev>()
				.HasOne(t => t.Status)
				.WithMany(s => s.Tasks)
				.HasForeignKey(t => t.TaskStatusId);

			// Task - Comment İlişkisi
			modelBuilder.Entity<Görev>()
				.HasOne(t => t.Comment)
				.WithMany(c => c.Tasks)
				.HasForeignKey(t => t.TaskCommentId);

			modelBuilder.Entity<Comment>()
				.HasOne(c => c.TeamLead)
				.WithMany()
				.HasForeignKey(c => c.TeamLeadId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Comment>()
				.HasOne(c => c.TeamMember)
				.WithMany()
				.HasForeignKey(c => c.TeamMemberId)
				.OnDelete(DeleteBehavior.Restrict);

			// Seed Verileri
			modelBuilder.Entity<Role>().HasData(
				new Role { RoleId = 1, RoleName = "Admin" },
				new Role { RoleId = 2, RoleName = "Team Lead" },
				new Role { RoleId = 3, RoleName = "Team Member" }
			);

			modelBuilder.Entity<Status>().HasData(
				new Status { StatusId = 1, StatusName = "Not Started", StatusColor = "#FF0000" },
				new Status { StatusId = 2, StatusName = "In Progress", StatusColor = "#00FF00" },
				new Status { StatusId = 3, StatusName = "Completed", StatusColor = "#0000FF" },
				new Status { StatusId = 4, StatusName = "Failed", StatusColor = "#FFFF00" }
			);

			modelBuilder.Entity<User>().HasData(
				new User
				{
					UserId = 1,
					UserFName = "Admin",
					UserLName = "User",
					UserEmail = "admin@gmail.com",
					UserSalt = string.Empty,
					UserHash = "Admin123*",
					GitHubProfile = "https://github.com/eyildizemre",
					Enabled = true
				}
			);

			modelBuilder.Entity<UserRole>().HasData(
				new UserRole { UserRoleId = 1, UserId = 1, RoleId = 1, Enabled = true });
		}
    }
}