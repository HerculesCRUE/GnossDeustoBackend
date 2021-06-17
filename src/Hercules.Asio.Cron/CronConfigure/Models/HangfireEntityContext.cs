// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Contexto para la base de datos
using Microsoft.EntityFrameworkCore;
using CronConfigure.Models.Hangfire;
using System.Diagnostics.CodeAnalysis;
using CronConfigure.Models.Entitties;

namespace CronConfigure.Models
{
    /// <summary>
    /// Contexto de hangfire para la base de datos
    /// </summary>
    /// 
    [ExcludeFromCodeCoverage]
    public class HangfireEntityContext : DbContext
    {
        /// <summary>
        /// AggregatedCounter
        /// </summary>
        public DbSet<AggregatedCounter> AggregatedCounter { get; set; }
        /// <summary>
        /// Hash
        /// </summary>
        public DbSet<Hash> Hash { get; set; }
        /// <summary>
        /// Job
        /// </summary>
        public DbSet<Job> Job { get; set; }
        /// <summary>
        /// JobParameter
        /// </summary>
        public DbSet<JobParameter> JobParameter { get; set; }
        /// <summary>
        /// JobQueue
        /// </summary>
        public DbSet<JobQueue> JobQueue { get; set; }
        /// <summary>
        /// List
        /// </summary>
        public DbSet<List> List { get; set; }
        /// <summary>
        /// Schema
        /// </summary>
        public DbSet<Schema> Schema { get; set; }
        /// <summary>
        /// Server
        /// </summary>
        public DbSet<Server> Server { get; set; }
        /// <summary>
        /// Set
        /// </summary>
        public DbSet<Set> Set { get; set; }
        /// <summary>
        /// State
        /// </summary>
        public DbSet<State> State { get; set; }
        /// <summary>
        /// Counter
        /// </summary>
        public DbSet<Counter> Counter { get; set; }
        /// <summary>
        /// JobRepository
        /// </summary>
        public DbSet<JobRepository> JobRepository { get; set; }
        /// <summary>
        /// ProcessingJobState
        /// </summary>
        public DbSet<ProcessingJobState> ProcessingJobState { get; set; }
        /// <summary>
        /// ProcessDiscoverStateJob
        /// </summary>
        public DbSet<ProcessDiscoverStateJob> ProcessDiscoverStateJob { get; set; }
        /// <summary>
        /// DiscoverItem
        /// </summary>
        public DbSet<DiscoverItem> DiscoverItem { get; set; }
        /// <summary>
        /// RepositoryConfig
        /// </summary>
        public DbSet<RepositoryConfig> RepositoryConfig { get; set; }
        /// <summary>
        /// ShapeConfig
        /// </summary>
        public DbSet<ShapeConfig> ShapeConfig { get; set; }
        /// <summary>
        /// RepositoryConfigSet
        /// </summary>
        public DbSet<RepositoryConfigSet> RepositoryConfigSet { get; set; }
        /// <summary>
        /// HangfireEntityContext
        /// </summary>
        /// <param name="options"></param>
        public HangfireEntityContext(DbContextOptions<HangfireEntityContext> options)
            : base(options) 
        {           
        }

        /// <summary>
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hash>()
                .HasKey(c => new { c.Key, c.Field });

            modelBuilder.Entity<JobParameter>()
                .HasKey(c => new { c.JobId, c.Name });

            modelBuilder.Entity<JobQueue>()
                .HasKey(c => new { c.Id, c.Queue });

            modelBuilder.Entity<List>()
                .HasKey(c => new { c.Id, c.Key});

            modelBuilder.Entity<Set>()
                .HasKey(c => new { c.Value, c.Key });

            modelBuilder.Entity<State>()
                .HasKey(c => new { c.Id, c.JobId });

            modelBuilder.Entity<Counter>()
                .HasKey(c => new { c.Value, c.Key });

            modelBuilder.Entity<ProcessDiscoverStateJob>().
                HasIndex(u => u.JobId)
                .IsUnique();
        }
    }
}
