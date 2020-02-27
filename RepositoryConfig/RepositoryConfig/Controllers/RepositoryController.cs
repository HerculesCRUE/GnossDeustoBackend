using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepositoryConfigSolution.Models.Entities;
using RepositoryConfigSolution.Models.Services;

namespace RepositoryConfigSolution.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RepositoryController : ControllerBase
    {
        private IRepositoriesConfigService _repositoriesConfigService;

        public RepositoryController(IRepositoriesConfigService iRepositoriesConfigService)
        {
            _repositoriesConfigService = iRepositoriesConfigService;
        }

        [HttpGet]
        public IActionResult GetRepositoriesConfigs()
        {
            return Ok(_repositoriesConfigService.GetRepositoryConfigs());
        }

        [HttpGet("{name}")]
        public IActionResult GetRepositoryConfig(string name)
        {
            return Ok(_repositoriesConfigService.GetRepositoryConfigByName(name));
        }

        [HttpPost]
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

        [HttpDelete]
        public IActionResult DeleteRepositoryConfig(string nombre)
        {
            bool deleted = _repositoriesConfigService.RemoveRepositoryConfig(nombre);
            if (deleted)
            {
                return Ok($"Config repository {nombre} has been deleted");
            }
            else
            {
                return Problem("Error has ocurred");
            }
        }

        [HttpPut]
        public IActionResult ModifyRepositoryConfig(RepositoryConfig repositoryConfig)
        {
            bool modified = _repositoriesConfigService.ModifyRepositoryConfig(repositoryConfig);
            if (modified)
            {
                return Ok($"Config repository {repositoryConfig.Name} has been modified");
            }
            else
            {
                return BadRequest($"Check that repository config with id {repositoryConfig.RepositoryConfigID} exist");
            }
        }
    }
}