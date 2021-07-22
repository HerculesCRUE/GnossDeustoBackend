// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Contexto de la base de datos

using API_DISCOVER.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace API_DISCOVER.Models
{
    /// <summary>
    /// EntityContext
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EntityContext : DbContext
    {
        /// <summary>
        /// DiscoverItem
        /// </summary>
        public DbSet<DiscoverItem> DiscoverItem { get; set; }
        /// <summary>
        /// ProcessDiscoverStateJob
        /// </summary>
        public DbSet<ProcessDiscoverStateJob> ProcessDiscoverStateJob { get; set; }

        public DbSet<ProcessingJobState> ProcessingJobState { get; set; }

        public DbSet<RepositoryConfig> RepositoryConfig { get; set; }


        /// <summary>
        /// EntityContext
        /// </summary>
        /// <param name="options"></param>
        /// <param name="memory"></param>
        public EntityContext(DbContextOptions options, bool memory = false)
            : base(options)
        {
            if (!memory)
            {
                Database.Migrate();
            }        

        }

        /// <summary>
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscoverItem>()
            .Property(e => e.LoadedEntities)
            .HasConversion(
                v => string.Join('|', v),
                v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList());

            modelBuilder.Entity<DiscoverItem.DiscardDissambiguation>()
            .Property(e => e.DiscardCandidates)
            .HasConversion(
                v => string.Join('|', v),
                v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList());
        }
    }
}
