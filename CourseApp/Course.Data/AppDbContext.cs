using System;
using System.Reflection;
using Course.Core.Entities;
using Course.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Course.Data
{
    public class AppDbContext : DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Group> Groups { get; set; }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());

            base.OnModelCreating(modelBuilder);
        }

    }
}

