using ApiPeliculas.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models
{
    public class EntityContext : DbContext
    {
        public DbSet<Film> Film { get; set; }
        public DbSet<Director> Director { get; set; }
        public DbSet<Actor> Actor { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<FilmActor> FilmActor { get; set; }
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
            modelBuilder.Entity<FilmActor>()
                .HasKey(fa => new { fa.Actor_ID, fa.Film_ID });
            modelBuilder.Entity<FilmActor>()
                .HasOne(fa => fa.Actor)
                .WithMany(a => a.Films)
                .HasForeignKey(fa => fa.Actor_ID);
            modelBuilder.Entity<FilmActor>()
                .HasOne(fa => fa.Film)
                .WithMany(f => f.Actors)
                .HasForeignKey(fa => fa.Film_ID);
            
            modelBuilder.Entity<Film>()
                .HasOne(f => f.Director)
                .WithMany(d=>d.Films);
        }
    }
}
