// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Contexto de la base de datos
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
        public DbSet<ProcessingJobState> ProcessingJobState { get; set; }
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
            modelBuilder.Entity<ProcessingJobState>()
                .HasIndex(u => u.JobId)
                .IsUnique();
        }
    }
}
