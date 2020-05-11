using System;
using System.Collections.Generic;
using API_CARGA.ModelExamples;
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using API_CARGA.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace PMH.Controllers
{
    /// <summary>
    /// Configuración del ETL
    /// </summary>
    [ApiController]
    [Route("Etl-config/[controller]")]
    public class RepositoryController : ControllerBase
    {
        private IRepositoriesConfigService _repositoriesConfigService;

        public RepositoryController(IRepositoriesConfigService iRepositoriesConfigService)
        {
            _repositoriesConfigService = iRepositoriesConfigService;
        }
        /// <summary>
        /// Obtiene un listado con todas las configuraciones de los repositorios OAI-PMH
        /// </summary>
        /// <returns>Listado con todas las configuraciones de los repositorios OAI-PMH</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(List<RepositoryConfig>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ConfigRepositoriesResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetRepository()
        {
            return Ok(_repositoriesConfigService.GetRepositoryConfigs());
        }

        /// <summary>
        /// Obtiene la configuración de un repositorio OAI-PMH
        /// </summary>
        /// <param name="identifier">Identificador del repositorio</param>
        /// <returns>Configuración del repositorio</returns>
        [HttpGet("{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(RepositoryConfig))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ConfigRepositoryResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetRepository(Guid identifier)
        {
            return Ok(_repositoriesConfigService.GetRepositoryConfigById(identifier));
        }

        /// <summary>
        /// Añade una nueva configuración de un repositorio OAI-PMH
        /// </summary>
        /// <param name="repositoryConfig">Datos de configuración del repositorio</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddConfigRepository(RepositoryConfig repositoryConfig)
        {
            Guid addedID = _repositoriesConfigService.AddRepositoryConfig(repositoryConfig);
            return Ok(addedID);
        }

        /// <summary>
        /// Elimina la configuración de un repositorio OAI-PMH.
        /// </summary>
        /// <param name="identifier">Identificador del repositorio</param>
        /// <returns></returns>
        [HttpDelete("{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteRepository(Guid identifier)
        {
            bool deleted = _repositoriesConfigService.RemoveRepositoryConfig(identifier);
            if (deleted)
            {
                return Ok($"Config repository {identifier} has been deleted");
            }
            else
            {
                return Problem("Error has ocurred");
            }
        }

        /// <summary>
        /// Modifica la configuración de un repositorio OAI-PMH.
        /// </summary>
        /// <param name="repositoryConfig">Datos de configuración del repositorio</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(ErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ModifyRepositoryErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ModifyRepositoryConfig(RepositoryConfig repositoryConfig)
        {
            bool modified = _repositoriesConfigService.ModifyRepositoryConfig(repositoryConfig);
            if (modified)
            {
                return Ok($"Config repository {repositoryConfig.RepositoryConfigID} has been modified");
            }
            else
            {
                return BadRequest(new ErrorExample { Error = $"Check that repository config with id {repositoryConfig.RepositoryConfigID} exist" });
            }
        }


    }
}