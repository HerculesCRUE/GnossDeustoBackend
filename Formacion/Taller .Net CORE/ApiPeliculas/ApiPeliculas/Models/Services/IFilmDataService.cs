using ApiPeliculas.Models.Entities;
using ApiPeliculas.Models.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Services
{
    public interface IFilmDataService
    {
        public Film GetFilm(Guid filmID);
        public List<Film> GetFilms();
        public List<Film> GetFilm(string name);
        public Guid AddFilm(FilmWrapper film);
        public void DeleteFilm(Guid filmID);
        public bool ModifyFilm(FilmWrapper film);
        public bool AddActor(FilmActor filmActor);
    }
}
