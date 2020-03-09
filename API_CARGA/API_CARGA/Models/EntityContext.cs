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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=SchoolDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (_defaultSchema != null && !_defaultSchema.Equals("dbo"))
            {
                modelBuilder.HasDefaultSchema(_defaultSchema);
            }
        }

        private static string GetDefaultSchema(DbConnection pConexionMaster)
        {
            string schemaDefecto = null;
            return schemaDefecto;
        }
    }
}
