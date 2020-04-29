using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using CronConfigure.Models.Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CronConfigure.Models.Entitties;

namespace CronConfigure.Models
{
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
        }
    }
}
