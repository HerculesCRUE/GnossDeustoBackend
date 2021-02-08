using ApiPeliculas.Models.Services;
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
    public class DirectorController : ControllerBase
    {
        public readonly IDirectorDataService _directorDataService;
        public DirectorController(IDirectorDataService directorDataService)
        {
            _directorDataService = directorDataService;
        }
        /// <summary>
        /// Devuleve una lista de peliculas dirigidas por un director
        /// </summary>
        /// <param name="director">nombre del director</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetFilm(string director)
        {
            return Ok(_directorDataService.GetFilms(director));
        }

    }
}
