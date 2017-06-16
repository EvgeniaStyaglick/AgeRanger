using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Configuration;
using AgeRanger.Data.DBModels;

namespace AgeRanger.Data
{
    public partial class AgeRangerContext : DbContext
    {
        public virtual DbSet<AgeGroup> AgeGroup { get; set; }
        public virtual DbSet<Person> Person { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("data source =" + System.Web.Hosting.HostingEnvironment.MapPath(
                ConfigurationManager.AppSettings["SQLiteFilePath"]));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgeGroup>(entity =>
            {
                entity.Property(e => e.Description).IsRequired();
            });
        }
    }
}