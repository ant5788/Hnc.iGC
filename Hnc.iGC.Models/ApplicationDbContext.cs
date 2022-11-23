using System;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hnc.iGC.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CNC>().HasMany(p => p.AlarmMessages).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CNC>().HasMany(p => p.Spindles).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CNC>().HasMany(p => p.Axes).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CNC>().HasMany(p => p.Files).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CNC>().HasMany(p => p.CutterInfos).WithOne().OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<AppFile> AppFiles { get; set; }

        public DbSet<AirDashboard_Cnc> AirDashboard_Cncs { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<CNC> CNCs { get; set; }

        public DbSet<DistributedIO> DistributedIOs { get; set; }

        public DbSet<Balancer> Balancers { get; set; }

        public DbSet<TemperBox> TemperBoxs { get; set; }


    }
}
