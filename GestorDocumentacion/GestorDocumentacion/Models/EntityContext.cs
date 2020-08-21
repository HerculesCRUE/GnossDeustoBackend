using GestorDocumentacion.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models
{
    public class EntityContext : DbContext
    {
        public DbSet<Page> Page { get; set; }
        public DbSet<Template> Template { get; set; }
        public DbSet<Document> Document { get; set; }
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
            modelBuilder.Entity<Page>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<Template>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<Document>()
               .HasIndex(u => u.Name)
               .IsUnique();
        }
    }
}
