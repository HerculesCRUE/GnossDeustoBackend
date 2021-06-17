// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Añadir parámetros dinámicamente en Swagger
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;

namespace UrisFactory.Filters
{
    /// <summary>
    /// AddParametersFilter
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AddParametersFilter: IOperationFilter
    {
        readonly private ConfigJsonHandler _configJsonHandler;

        /// <summary>
        /// AddParametersFilter
        /// </summary>
        /// <param name="configJsonHandler"></param>
        public AddParametersFilter(ConfigJsonHandler configJsonHandler)
        {
            _configJsonHandler = configJsonHandler;
        }

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            UriStructureGeneral uriStructureGeneral = _configJsonHandler.GetUrisConfig();
            if (operation.OperationId != null && operation.OperationId.Equals("GenerateUri"))
            {
                foreach (UriStructure structure in uriStructureGeneral.UriStructures)
                {
                    foreach (Component component in structure.Components)
                    {
                        if (!UriComponentsList.DefaultParameters.Contains(component.UriComponent))
                        {
                            operation.Parameters.Add(new OpenApiParameter
                            {
                                Name = component.UriComponent,
                                In = ParameterLocation.Query,
                                Required = false

                            });
                        }
                    }
                }
            }
        }
    }
}
