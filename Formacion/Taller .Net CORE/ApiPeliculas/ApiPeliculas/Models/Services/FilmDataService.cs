using ApiPeliculas.Models.Entities;
using ApiPeliculas.Models.Wrappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Services
{
    public class FilmDataService : IFilmDataService
    {
        private readonly EntityContext _context;

        public FilmDataService(EntityContext context)
        {
            _context = context;
        }

        public Guid AddFilm(FilmWrapper film)
        {
            Film filmToAdd = new Film();
            filmToAdd.Title = film.Title;
            filmToAdd.Year = film.Year;
            filmToAdd.Released = film.Released;
            filmToAdd.MinuteRunTime = film.MinuteRunTime;
            filmToAdd.Director_ID = film.Director_ID;

            var filmBD = _context.Film.Where(item => item.Title.ToLower().Equals(film.Title.ToLower())).FirstOrDefault();
            if (filmBD == null)
            {
                filmToAdd.Film_ID = Guid.NewGuid();
                _context.Film.Add(filmToAdd);
                _context.SaveChanges();
                return filmToAdd.Film_ID;
            }
            else{
                return Guid.Empty;
            }
        }

        public void DeleteFilm(Guid filmID)
        {
            var film = _context.Film.Where(item => item.Film_ID.Equals(filmID)).FirstOrDefault();
            if(film != null)
            {
                _context.Film.Remove(film);
            }
            _context.SaveChanges();
        }

        public Film GetFilm(Guid filmID)
        {
            var film = _context.Film.Where(item => item.Film_ID.Equals(filmID)).FirstOrDefault();
            return film;
        }

        public List<Film> GetFilm(string name)
        {
            var films = _context.Film.Where(item => item.Title.ToLower().Contains(name.ToLower())).ToList();
            return films;
        }
        public List<Film> GetFilms()
        {
            var films = _context.Film.ToList();
            return films;
        }
        public bool ModifyFilm(FilmWrapper film)
        {
            bool dataCorrect = true;
            var filmDB = _context.Film.Where(item => item.Film_ID.Equals(film.Film_ID)).FirstOrDefault();
            if (filmDB != null)
            {
                if(!film.Director_ID.Equals(Guid.Empty)){
                    filmDB.Director_ID = film.Director_ID;
                }
                if (!string.IsNullOrEmpty(film.Title) )
                {
                    if (_context.Film.Where(item => item.Title.ToLower().Equals(film.Title.ToLower())).FirstOrDefault() == null)
                    {
                        filmDB.Title = film.Title;
                    }
                    else
                    {
                        dataCorrect = false;
                    }
                }
                if(film.MinuteRunTime != 0)
                {
                    filmDB.MinuteRunTime = film.MinuteRunTime;
                }
                if(film.Year != 0)
                {
                    filmDB.MinuteRunTime = film.MinuteRunTime;
                }
                _context.SaveChanges();
            }
            return dataCorrect;
        }
        public bool AddActor(FilmActor filmActor)
        {
            bool add = true;
            try
            {
                _context.FilmActor.Add(filmActor);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                add = false;
            }
            return add;
        }
    }
}
