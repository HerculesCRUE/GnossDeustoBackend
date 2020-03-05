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

namespace API_CARGA.Controllers
{
    [Route("etl-config/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {
        IShapesConfigService _shapeConfigService;
        public ValidationController(IShapesConfigService iShapeConfigService)
        {
            _shapeConfigService = iShapeConfigService;
        }
        /// <summary>
        /// Obtiene la configuración de los shape SHACL de validación
        /// </summary>
        /// <returns>Listado con las definiciones de las validaciones</returns>       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(List<ShapeConfig>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ShapesConfigsResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetShape()
        {
            return Ok(_shapeConfigService.GetShapesConfigs());
        }

        /// <summary>
        /// Obtiene la configuración del shape SHACL pasado por parámetro
        /// </summary>
        /// <param name="identifier">Identificador de la validación</param>
        /// <returns>Definición de la validación</returns>       
        [HttpGet("{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(ShapeConfig))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ShapeConfigResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetShape(Guid identifier)
        {
            return Ok(_shapeConfigService.GetShapeConfigById(identifier));
        }


        /// <summary>
        /// Añade una configuración de validación mediante un shape SHACL
        /// </summary>
        /// <param name="shapeconfig">Configuración de la validación</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(ErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AddShapeConfigResponseError))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddShape(ShapeConfig shapeconfig)
        {
            Guid addedID = _shapeConfigService.AddShapeConfig(shapeconfig);
            if (!addedID.Equals(Guid.Empty))
            {
                return Ok(addedID);
            }
            else
            {
                return BadRequest(new ErrorExample { Error = $"shape config {shapeconfig.Name} already exist" });
            }
        }

        /// <summary>
        /// Elimina la configuración una configuración de validación 
        /// </summary>
        /// <param name="identifier">Identificador de la validación</param>
        /// <returns></returns>
        [HttpDelete("{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteShape(Guid identifier)
        {
            bool deleted = _shapeConfigService.RemoveShapeConfig(identifier);
            if (deleted)
            {
                return Ok($"shape config {identifier} has been deleted");
            }
            else
            {
                return Problem("Error has ocurred");
            }
        }

        /// <summary>
        /// Modifica la configuración de validación mediante un shape SHACL
        /// </summary>
        /// <param name="shapeconfig">Datos de configuración de la validación</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(ErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ModifyShapeConfigResponseError))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ModifyShape(ShapeConfig shapeconfig)
        {
            bool modified = _shapeConfigService.ModifyShapeConfig(shapeconfig);
            if (modified)
            {
                return Ok($"shape config {shapeconfig.ShapeConfigID} has been modified");
            }
            else
            {
                return BadRequest(new ErrorExample { Error = $"Check that shape config with id {shapeconfig.ShapeConfigID} exist"});
            }
        }
    }
}