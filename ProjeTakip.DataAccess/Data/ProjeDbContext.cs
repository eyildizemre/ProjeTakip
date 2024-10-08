﻿using Microsoft.EntityFrameworkCore;
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
        public DbSet<UserTeam> UserTeams { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Görev> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OnayDurumu> OnayDurumu { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API ile ilişkileri tanımlama

            // Role tablosu ile diğer tablolar arasındaki ilişkiler
            modelBuilder.Entity<Role>()
                .HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // Role silindiğinde ilgili UserRoles kayıtları da silinir.

            modelBuilder.Entity<Role>()
                .HasMany(r => r.UserTeams)
                .WithOne(ut => ut.Role)
                .HasForeignKey(ut => ut.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // Role silindiğinde ilgili UsersTeams kayıtları da silinir.

            // UserRole tablosu ile diğer tablolar arasındaki ilişkiler
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde ilişkili UserRole kayıtları da silinir.

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Rol silindiğinde UserRole silinmez.

            // Project tablosu ile diğer tablolar arasındaki ilişkiler
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Projects)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Restrict); // Eğer bir takım silinirse, projedeki TeamId null olacak.

            modelBuilder.Entity<Project>()
                .HasOne(p => p.TeamLead)
                .WithMany()
                .HasForeignKey(p => p.TeamLeadId)
                .OnDelete(DeleteBehavior.SetNull); // Eğer bir takım lideri silinirse, projedeki TeamLeadId null olacak.

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Status)
                .WithMany()
                .HasForeignKey(p => p.ProjectStatusId)
                .OnDelete(DeleteBehavior.Restrict); // Eğer bir durum silinirse, proje silinmeyecek.

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Project)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // Proje silindiğinde ilişkili yorumlar da silinir.

            // Team tablosu ile diğer tablolar arasındaki ilişkiler
            modelBuilder.Entity<Team>()
                .HasOne(t => t.TeamLead)
                .WithMany()
                .HasForeignKey(t => t.TeamLeadId)
                .OnDelete(DeleteBehavior.Restrict); // TeamLead silindiğinde TeamLeadId null yapılmayacak.

            modelBuilder.Entity<Team>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Teams)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // Project silindiğinde ilişkili Teams kayıtları da silinir.

            modelBuilder.Entity<Team>()
                .HasMany(t => t.UserTeams)
                .WithOne(ut => ut.Team)
                .HasForeignKey(ut => ut.TeamId)
                .OnDelete(DeleteBehavior.Cascade); // Ekip silindiğinde ilişkili UserTeams kayıtları da silinir.

            // User tablosu ile diğer tablolar arasındaki ilişkiler
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde ilişkili UserRoles kayıtları da silinir.

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserTeams)
                .WithOne(ut => ut.User)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde ilişkili UserTeams kayıtları da silinir.

            modelBuilder.Entity<User>()
                .HasMany(u => u.LeadProjects)
                .WithOne(p => p.TeamLead)
                .HasForeignKey(p => p.TeamLeadId)
                .OnDelete(DeleteBehavior.SetNull); // Kullanıcı silindiğinde LeadProjects'teki TeamLeadId null yapılır.

            modelBuilder.Entity<User>()
                .HasMany(u => u.Tasks)
                .WithOne(t => t.AssignedUser)
                .HasForeignKey(t => t.TeamLeadId)
                .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silindiğinde ilişkili görevler silinmez.

            // UserTeam tablosu ile diğer tablolar arasındaki ilişkiler
            modelBuilder.Entity<UserTeam>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTeams)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde ilişkili UserTeam kayıtları da silinir.

            modelBuilder.Entity<UserTeam>()
                .HasOne(ut => ut.Team)
                .WithMany(t => t.UserTeams)
                .HasForeignKey(ut => ut.TeamId)
                .OnDelete(DeleteBehavior.Cascade); // Ekip silindiğinde ilişkili UserTeam kayıtları da silinir.

            modelBuilder.Entity<UserTeam>()
                .HasOne(ut => ut.Role)
                .WithMany(r => r.UserTeams)
                .HasForeignKey(ut => ut.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Rol silindiğinde UserTeam kayıtları silinmez.

            modelBuilder.Entity<UserTeam>()
                .HasOne(ut => ut.Project)
                .WithMany(p => p.UserTeams)
                .HasForeignKey(ut => ut.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Görev tablosu ile diğer tablolar arasındaki ilişkiler
            // Görev ve Proje İlişkisi
            modelBuilder.Entity<Görev>()
                .HasOne(g => g.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(g => g.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Görev ve TeamLead (User) İlişkisi
            modelBuilder.Entity<Görev>()
                .HasOne(g => g.TeamLead)
                .WithMany()
                .HasForeignKey(g => g.TeamLeadId)
                .OnDelete(DeleteBehavior.Restrict);

            // Görev ve AssignedUser (User) İlişkisi
            modelBuilder.Entity<Görev>()
                .HasOne(g => g.AssignedUser)
                .WithMany()
                .HasForeignKey(g => g.AssignedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Görev ve Status İlişkisi
            modelBuilder.Entity<Görev>()
                .HasOne(g => g.Status)
                .WithMany()
                .HasForeignKey(g => g.TaskStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            //// Görev ve Comment İlişkisi
            //modelBuilder.Entity<Görev>()            //modelBuilder.Entity<Görev>()
            //    .HasOne(g => g.Comment)             //    .HasOne(g => g.Comment)
            //    .WithMany(c => c.Tasks)             //    .WithOne()
            //    .HasForeignKey(g => g.CommentId)    //    .HasForeignKey<Görev>(g => g.TaskCommentId)
            //    .OnDelete(DeleteBehavior.Restrict); //    .OnDelete(DeleteBehavior.Restrict);

            // Görev ve Comments İlişkisi (Çoktan çoğa)
            modelBuilder.Entity<Görev>()
                .HasMany(g => g.Comments)
                .WithOne(c => c.Görev)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Görev>()
                .HasOne(g => g.OnayDurumu)
                .WithMany()
                .HasForeignKey(g => g.OnayDurumuId)
                .OnDelete(DeleteBehavior.Restrict); // Onay Durumu silindiğinde görevi silme

            // Comment tablosu ile diğer tablolar arasındaki ilişkiler
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.TeamLead)
                .WithMany()
                .HasForeignKey(c => c.TeamLeadId)
                .OnDelete(DeleteBehavior.Restrict); // Bu, TeamLead silindiğinde Comment'in silinmemesini sağlar.

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.TeamMember)
                .WithMany()
                .HasForeignKey(c => c.TeamMemberId)
                .OnDelete(DeleteBehavior.Restrict); // Bu, TeamMember silindiğinde Comment'in silinmemesini sağlar.

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Görev)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Restrict); // Bu, Task silindiğinde ilgili Comment'lerin de silinmesini sağlar.

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Project)  // Project navigation property kullanılmalı
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // Bu, Project silindiğinde ilgili Comment'lerin de silinmesini sağlar.

            // Status tablosu ile diğer tablolar arasındaki ilişkiler
            modelBuilder.Entity<Status>()
                .HasMany(s => s.Projects)
                .WithOne(p => p.Status)
                .HasForeignKey(p => p.ProjectStatusId)
                .OnDelete(DeleteBehavior.Restrict); // Status silindiğinde ilişkili projeler silinmez.

            modelBuilder.Entity<Status>()
                .HasMany(s => s.Tasks)
                .WithOne(t => t.Status)
                .HasForeignKey(t => t.TaskStatusId)
                .OnDelete(DeleteBehavior.Restrict); // Status silindiğinde ilişkili görevler silinmez.

            // Notification tablosu ile diğer tablolar arasındaki ilişkiler
            // Notification ile Görev arasındaki ilişki
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Task)
                .WithMany(t => t.Notifications)
                .HasForeignKey(n => n.TaskId)
                .OnDelete(DeleteBehavior.NoAction);

            // Notification ile Proje arasındaki ilişki
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Project)
                .WithMany()
                .HasForeignKey(n => n.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            // Notification ile User (SentBy) arasındaki ilişki
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.SentBy)
                .WithMany()
                .HasForeignKey(n => n.SentById)
                .OnDelete(DeleteBehavior.Restrict); // Bir kullanıcının gönderdiği bildirimin silinmesiyle kullanıcı silinmemeli

            // Notification ile User (ReceivedBy) arasındaki ilişki
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.ReceivedBy)
                .WithMany()
                .HasForeignKey(n => n.ReceivedById)
                .OnDelete(DeleteBehavior.Restrict); // Bir kullanıcının aldığı bildirimin silinmesiyle kullanıcı silinmemeli

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

            modelBuilder.Entity<OnayDurumu>().HasData(
                new OnayDurumu { OnayDurumuId = 1, OnayDurumuAdi = "Onay Bekliyor" },
                new OnayDurumu { OnayDurumuId = 2, OnayDurumuAdi = "Onaylandı" },
                new OnayDurumu { OnayDurumuId = 3, OnayDurumuAdi = "Reddedildi" },
                new OnayDurumu { OnayDurumuId = 4, OnayDurumuAdi = "Onay durumu yok" }
            );

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin123*", salt);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    UserFName = "Admin",
                    UserLName = "User",
                    UserEmail = "admin@gmail.com",
                    UserSalt = salt,
                    UserHash = hashedPassword,
                    GitHubProfile = "https://github.com/eyildizemre",
                    Enabled = true
                }
            );

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserRoleId = 1, UserId = 1, RoleId = 1, Enabled = true });
        }
    }
}