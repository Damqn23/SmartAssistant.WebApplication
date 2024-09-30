using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Data.Entities;
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
        public DbSet<Message> Messages { get; set; }
        public DbSet<TeamDocument> TeamDocuments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("AspNetUsers");

            builder.Entity<UserTeam>()
               .HasKey(ut => new { ut.UserId, ut.TeamId });

            builder.Entity<UserTeam>()
        .HasOne(ut => ut.User)
        .WithMany(u => u.UserTeams)
        .HasForeignKey(ut => ut.UserId)
        .OnDelete(DeleteBehavior.Restrict);  // Disable cascade delete

            builder.Entity<UserTeam>()
                .HasOne(ut => ut.Team)
                .WithMany(t => t.UserTeams)
                .HasForeignKey(ut => ut.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
        .HasOne(m => m.Team)
        .WithMany(t => t.Messages)
        .HasForeignKey(m => m.TeamId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            builder.Entity<TeamDocument>()
                .HasOne(td => td.Team)
                .WithMany(t => t.TeamDocuments)
                .HasForeignKey(td => td.TeamId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
        }
    }
}
