using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Models.Entities;

namespace ApiPeliculas.Models.Services
{
    public interface IActorDataService
    {
        public List<Film> GetFilms(string nameActor);
        public List<Actor> GetActors();
    }
}
