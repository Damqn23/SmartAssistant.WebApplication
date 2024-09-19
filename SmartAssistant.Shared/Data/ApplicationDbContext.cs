﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.WebApp.Data.Entities;
using SmartAssistant.WebApplication.Data.Entities;
using System.Reflection.Emit;

namespace SmartAssistant.WebApplication.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		
		public DbSet<SmartAssistant.WebApp.Data.Entities.Task> Tasks { get; set; }
		public DbSet<Reminder> Reminders { get; set; }
		public DbSet<Note> Notes { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<Subscription> Subscriptions { get; set; }
		public DbSet<Attachment> Attachments { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<ActivityLog> ActivityLogs { get; set; }
		public DbSet<Preference> Preferences { get; set; }
		public DbSet<Team> Teams { get; set; }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<User>().ToTable("AspNetUsers");

			builder.Entity<UserTeam>()
				.HasKey(ut => new { ut.UserForeignKey, ut.TeamForeignKey });

			
		}
	}
}
