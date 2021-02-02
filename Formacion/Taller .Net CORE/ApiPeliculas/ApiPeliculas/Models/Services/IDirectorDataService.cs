using System;
using System.Collections.Generic;
using ApiPeliculas.Models.Entities;
using ApiPeliculas.Models.Wrappers;

namespace ApiPeliculas.Models.Services
{
    public interface IDirectorDataService
    {
        public List<Film> GetFilms(string nameDirector);
    }
}
