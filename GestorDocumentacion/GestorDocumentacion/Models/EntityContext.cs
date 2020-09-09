// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Modelo de Base de datos
using GestorDocumentacion.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models
{
    /// <summary>
    /// Modelo de Base de datos
    /// </summary>
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
                .HasIndex(u => u.Route)
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
