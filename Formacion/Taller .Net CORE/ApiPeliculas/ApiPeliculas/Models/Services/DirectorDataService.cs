using ApiPeliculas.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Models.Wrappers;

namespace ApiPeliculas.Models.Services
{
    public class DirectorDataService : IDirectorDataService
    {
        private readonly EntityContext _context;

        public DirectorDataService(EntityContext context)
        {
            _context = context;
        }

        public List<Film> GetFilms(string nameDirector)
        {
            Guid directorID = _context.Person.Join(_context.Director,
                persona => persona.Person_ID, director => director.Person_ID, (persona, director) => new
                {
                    Persona = persona,
                    Director = director
                }).Where(item => item.Persona.Name.Equals(nameDirector)).Select(item => item.Director.Director_ID).FirstOrDefault();

            var films = _context.Film.Where(film=>film.Director_ID.Equals(directorID)).ToList();
            return films;
        }
    }
}
