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
        public DbSet<AggregatedCounter> AggregatedCounter { get; set; }
        public DbSet<Hash> Hash { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<JobParameter> JobParameter { get; set; }
        public DbSet<JobQueue> JobQueue { get; set; }
        public DbSet<List> List { get; set; }
        public DbSet<Schema> Schema { get; set; }
        public DbSet<Server> Server { get; set; }
        public DbSet<Set> Set { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<Counter> Counter { get; set; }
        public DbSet<JobRepository> JobRepository { get; set; }
        public DbSet<ProcessingJobState> ProcessingJobState { get; set; }
        public DbSet<ProcessDiscoverStateJob> ProcessDiscoverStateJob { get; set; }
        public DbSet<DiscoverItem> DiscoverItem { get; set; }
        public DbSet<RepositoryConfig> RepositoryConfig { get; set; }
        public DbSet<ShapeConfig> ShapeConfig { get; set; }
        public DbSet<RepositoryConfigSet> RepositoryConfigSet { get; set; }
        public HangfireEntityContext(DbContextOptions<HangfireEntityContext> options)
            : base(options) 
        {
           
            // Database.Migrate();
        }


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
