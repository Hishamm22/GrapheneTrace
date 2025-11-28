using GrapheneTrace.Models;
using Microsoft.EntityFrameworkCore;

namespace GrapheneTrace.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<SensorSession> SensorSessions { get; set; }
        public DbSet<SensorFrame> SensorFrames { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------------------------------
            // Explicit primary keys (EF won't have to guess)
            // -------------------------------------------------
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            modelBuilder.Entity<Patient>()
                .HasKey(p => p.PatientID);

            modelBuilder.Entity<SensorSession>()
                .HasKey(s => s.SessionID);

            modelBuilder.Entity<SensorFrame>()
                .HasKey(f => f.FrameID);

            modelBuilder.Entity<Comment>()
                .HasKey(c => c.CommentID);

            // -------------------------------------------------
            // Relationships
            // -------------------------------------------------

            // User–Patient one-to-one
            modelBuilder.Entity<User>()
                .HasOne(u => u.PatientProfile)
                .WithOne(p => p.User)
                .HasForeignKey<Patient>(p => p.UserID);

            // Patient–Session one-to-many
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Sessions)
                .WithOne(s => s.Patient)
                .HasForeignKey(s => s.PatientID);

            // Session–Frame one-to-many
            modelBuilder.Entity<SensorSession>()
                .HasMany(s => s.Frames)
                .WithOne(f => f.Session)
                .HasForeignKey(f => f.SessionID);

            // Session–Comment one-to-many
            modelBuilder.Entity<SensorSession>()
                .HasMany(s => s.Comments)
                .WithOne(c => c.Session)
                .HasForeignKey(c => c.SessionID);

            // Patient–Comment one-to-many
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Patient)
                .HasForeignKey(c => c.PatientID);

            // Frame–Comment one-to-many (optional frame link)
            modelBuilder.Entity<SensorFrame>()
                .HasMany(f => f.Comments)
                .WithOne(c => c.Frame)
                .HasForeignKey(c => c.FrameID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
