using CRUD_API.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace CRUD_API.Data
{
    public class ApplicationDataContext : IdentityDbContext
    {
        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options): base(options) { }

        public DbSet<FormModel>? FormModels { get; set; }

        public DbSet<FormDataModel>? FormDataModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FormModel>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<FormDataModel>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<FormModel>()
                .Property(f => f.CreatedTime)
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<FormModel>()
                .Property(f => f.UpdatedTime)
                .HasDefaultValue(DateTime.Now);
        }

    }
}
