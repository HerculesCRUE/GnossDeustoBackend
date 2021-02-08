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
    public class ActorController : ControllerBase
    {
        public readonly IActorDataService _actorDataService;
        public ActorController(IActorDataService actorDataService)
        {
            _actorDataService = actorDataService;
        }
        /// <summary>
        /// Devuleve una lista de peliculas en las que participa un actor
        /// </summary>
        /// <param name="actor">nombre del actor</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetFilm(string actor)
        {
            return Ok(_actorDataService.GetFilms(actor));
        }

        /// <summary>
        /// Devuleve una lista de actores
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public IActionResult GetActors()
        {
            return Ok(_actorDataService.GetActors());
        }
    }
}
