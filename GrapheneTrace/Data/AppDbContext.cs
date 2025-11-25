using GrapheneTrace.Models;
using Microsoft.EntityFrameworkCore;

namespace GrapheneTrace.Data
{
    // Link between our model classes and the database.
    // EF Core uses this to create the tables and run queries.
    public class AppDbContext : DbContext
    {
        // Options (like provider + connection string) are passed in by ASP.NET Core.
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Each DbSet<T> below will become a table in the database.

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Clinician> Clinicians { get; set; }
        public DbSet<SensorSession> SensorSessions { get; set; }
        public DbSet<SensorFrame> SensorFrames { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Alert> Alerts { get; set; }
    }
}
