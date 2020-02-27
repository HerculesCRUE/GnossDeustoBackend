﻿using System;
using System.Collections.Generic;
using System.Linq;
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OaiPmhNet;
using OaiPmhNet.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PMH.Controllers
{
    /// <summary>
    /// Configuración del ETL
    /// </summary>
    [ApiController]
    [Route("etl-config/[controller]")]
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddConfigRepository(RepositoryConfig repositoryConfig)
        {
            bool added = _repositoriesConfigService.AddRepositoryConfig(repositoryConfig);
            if (added)
            {
                return Ok($"new config repository {repositoryConfig.Name} has been added");
            }
            else
            {
                return BadRequest($"config repository {repositoryConfig.Name} already exist");
            }
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
                return BadRequest($"Check that repository config with id {repositoryConfig.RepositoryConfigID} exist");
            }
        }


    }
}