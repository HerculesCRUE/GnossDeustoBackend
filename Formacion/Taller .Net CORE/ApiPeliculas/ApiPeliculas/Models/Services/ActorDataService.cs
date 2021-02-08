using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Models.Entities;
using ApiPeliculas.Models.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Models.Services
{
    public class ActorDataService : IActorDataService
    {
        private readonly EntityContext _context;

        public ActorDataService(EntityContext context)
        {
            _context = context;
        }
        public List<Film> GetFilms(string nameActor)
        {
            var films = _context.Actor.JoinPerson(_context).JoinFilmActor(_context).JoinFilm(_context).Where(item => item.Person.Name.ToLower().Equals(nameActor.ToLower())).Select(item => item.Film);
            return films.ToList();
        }

        public List<Actor> GetActors()
        {
            var actors = _context.Actor.Include(actor => actor.Person).ToList();
            return actors.ToList();
        }
    }
}
