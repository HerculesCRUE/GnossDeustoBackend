using API_CARGA.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_CARGA.Models
{
    public class EntityContext : DbContext
    {
        private string _defaultSchema;
        public DbSet<RepositoryConfig> RepositoryConfig { get; set; }
        public DbSet<SyncConfig> SyncConfig { get; set; }
        public DbSet<ShapeConfig> ShapeConfig { get; set; }
        public DbSet<RepositorySync> RepositorySync { get; set; }
        public EntityContext(DbContextOptions options)
            : base(options)
        {
            Database.Migrate();

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<RepositorySync>()
            //    .HasKey(c => new { c.RepositoryId, c.Set });

        }
    }
}
