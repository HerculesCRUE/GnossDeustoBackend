using ApiPeliculas.Models.Entities;
using ApiPeliculas.Models.Services;
using ApiPeliculas.Models.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTestPeliculas.Mock
{
    class DataFilmMockService : IFilmDataService
    {
        private List<Film> films;
        public DataFilmMockService()
        {
            films = new List<Film>();
            Guid filmIDSW = Guid.NewGuid();
            Film film1 = new Film()
            {
                MinuteRunTime = 139,
                Director_ID = Guid.NewGuid(),
                Title = "Star Wars: Episodio III - La venganza de los Sith",
                Year = 2005,
                Film_ID = filmIDSW
            };

            Guid filmIDSW2 = Guid.NewGuid();
            Film film2 = new Film()
            {
                MinuteRunTime = 142,
                Director_ID = Guid.NewGuid(),
                Title = "Star Wars: Episodio II - El ataque de los clones",
                Year = 2002,
                Film_ID = filmIDSW2
            };

            Guid filmIDMB = Guid.NewGuid();
            Film filmMB = new Film()
            {
                MinuteRunTime = 153,
                Director_ID = Guid.NewGuid(),
                Title = "Malditos bastardos",
                Year = 2009,
                Film_ID = filmIDMB
            };
            films.Add(film1);
            films.Add(film2);
            films.Add(filmMB);
        }
        public bool AddActor(FilmActor filmActor)
        {
            throw new NotImplementedException();
        }

        public Guid AddFilm(FilmWrapper film)
        {
            throw new NotImplementedException();
        }

        public void DeleteFilm(Guid filmID)
        {
            throw new NotImplementedException();
        }

        public Film GetFilm(Guid filmID)
        {
            throw new NotImplementedException();
        }

        public List<Film> GetFilm(string name)
        {
            throw new NotImplementedException();
        }

        public List<Film> GetFilms()
        {
            return films;
        }

        public bool ModifyFilm(FilmWrapper film)
        {
            throw new NotImplementedException();
        }
    }
}
