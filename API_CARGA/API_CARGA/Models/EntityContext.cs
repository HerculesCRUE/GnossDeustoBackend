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
        public DbSet<SyncConfig> SyncConfig { get; set; }
        public DbSet<ShapeConfig> ShapeConfig { get; set; }

        public EntityContext(DbContextOptions options)
            : base(options)
        {
            Database.Migrate();

        }
    }
}
