using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using Microsoft.EntityFrameworkCore;

namespace API_CARGA.Models
{
    public class EntityContext : DbContext
    {
        private string _defaultSchema;
        public DbSet<RepositoryConfig> RepositoryConfig { get; set; }
        public DbSet<ShapeConfig> ShapeConfig { get; set; }
        public DbSet<RepositorySync> RepositorySync { get; set; }
        public EntityContext(DbContextOptions options, bool memory = false)
            : base(options)
        {
            if (!memory)
            {
                Database.Migrate();
            }
           

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<RepositorySync>()
            //    .HasKey(c => new { c.RepositoryId, c.Set });

        }
    }
}
