using AttendanceV3.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AttendanceV3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ✅ Use the exact class names
        public DbSet<Student> Students { get; set; }
        public DbSet<AttendanceSessions> AttendanceSessions { get; set; } // plural class


        public DbSet<StudentDevices> StudentDevices { get; set; }

        public DbSet<AttendanceRecord> AttendanceRecord { get; set; }   // plural DbSet name

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StudentDevices>()
                .HasIndex(d => d.DeviceId)
                .IsUnique();
        }

    }
}
