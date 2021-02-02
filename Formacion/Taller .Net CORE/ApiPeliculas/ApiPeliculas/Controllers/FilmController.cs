using ApiPeliculas.Models.Entities;
using ApiPeliculas.Models.Services;
using ApiPeliculas.Models.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        public readonly IFilmDataService _filmDataService;
        public FilmController(IFilmDataService filmDataService)
        {
            _filmDataService = filmDataService;
        }
        /// <summary>
        /// Obtiene una pelicula
        /// </summary>
        /// <param name="name">nombre de la pelicula a obtener</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetFilm(string name)
        {
            return Ok(_filmDataService.GetFilm(name));
        }
        /// <summary>
        /// Obtiene una lista de peliculas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public IActionResult GetFilms()
        {
            return Ok(_filmDataService.GetFilms());
        }
        /// <summary>
        /// Añade una pelicula
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddFilm(FilmWrapper film)
        {

            Guid filmAdded = _filmDataService.AddFilm(film);
            if (filmAdded.Equals(Guid.Empty))
            {
                return BadRequest("Ya existe una película con ese título");
            }
            return Ok(filmAdded);
        }

        /// <summary>
        /// Elimina una pelicula
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteFilm(Guid film_id)
        {
            _filmDataService.DeleteFilm(film_id);
            return Ok();
        }

        /// <summary>
        /// Modifica una pelicula
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public IActionResult ModifyFilm(FilmWrapper film)
        {

            bool added = _filmDataService.ModifyFilm(film);
            if (added)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Datos incorrectos");
            }
            
        }

        [HttpPost]
        [Route("actor")]
        public IActionResult AddActor(Guid actor_id, Guid film_id)
        {
            FilmActor filmActor = new FilmActor();
            filmActor.Actor_ID = actor_id;
            filmActor.Film_ID = film_id;
            bool added = _filmDataService.AddActor(filmActor);
            if (added)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Actor ya añadido a la pelicula");
            }
        }
    }
}
