using API_CARGA.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models
{
    public class EntityContext : DbContext
    {
        private string _defaultSchema;
        public DbSet<RepositoryConfig> RepositoryConfig { get; set; }
        public DbSet<ShapeConfig> ShapeConfig { get; set; }

        public EntityContext(DbContextOptions options)
            : base(options)
        {
            Database.Migrate();

        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql("Host=192.168.40.179;Database=hercules;Username=hercules;Password=hercules");
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    if (_defaultSchema != null && !_defaultSchema.Equals("dbo"))
        //    {
        //        modelBuilder.HasDefaultSchema(_defaultSchema);
        //    }
        //}

        //private static string GetDefaultSchema(DbConnection pConexionMaster)
        //{
        //    string schemaDefecto = null;
        //    return schemaDefecto;
        //}
    }
}
