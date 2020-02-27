using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMH.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PRH.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PMHController : Controller
    {
        /// <summary>
        /// Obtiene la entidad solicitada
        /// </summary>
        /// <param name="pIdentifier">Identificador de la entidad</param>
        /// <param name="pEntityClass">Clase de la entidad</param>
        /// <returns></returns>
        [HttpGet("GetEntity",Name ="GetEntity")]       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetEntity(string pIdentifier,string pEntityClass)
        {
            
            return Ok("");
        }

        /// <summary>
        /// Obtiene el listado con las entidades solicitada
        /// </summary>
        /// <param name="pEntities">Listado con los identificadores de las entidades solicitadas</param>
        /// <returns></returns>
        [HttpPost("GetEntities", Name = "GetEntities")]       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetEntities(List<Entity> pEntities)
        {
            // + tipo + POSTS para N usar objeto
            return Ok("");
        }

        /// <summary>
        /// Obtiene un lista con  los identificadores de las entidades a partir de las fecha establecida, con un máximo definido por el usuario (por defecto 100)
        /// </summary>
        /// <param name="from">Fecha de inicio en formato UTC</param>
        /// <param name="max">Número máximo de identidicadores</param>
        /// <returns></returns>
        [HttpGet("ListIdentifiers", Name = "ListIdentifiers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<EntityUpdate> ListIdentifiers(DateTime from, int max=100)
        {
            if(from==DateTime.MinValue)
            {
                throw new Exception("from es obligatorio");
            }
            List<EntityUpdate> entidades = new List<EntityUpdate>();
            entidades.Add(new EntityUpdate() { id = "1", entityClass = "tipo", updateType = EntityUpdate.UpdateType.create, updateDate = DateTime.Now });
            entidades.Add(new EntityUpdate() { id = "2", entityClass = "tipo", updateType = EntityUpdate.UpdateType.delete, updateDate = DateTime.Now });
            entidades.Add(new EntityUpdate() { id = "3", entityClass = "tipo", updateType = EntityUpdate.UpdateType.modify, updateDate = DateTime.Now });
            entidades.Add(new EntityUpdate() { id = "4", entityClass = "tipo", updateType = EntityUpdate.UpdateType.create, updateDate = DateTime.Now });
            return entidades;
        }
    }
}