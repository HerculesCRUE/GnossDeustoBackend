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
    [ExcludeFromCodeCoverage]
    public class AddParametersFilter: IOperationFilter
    {
        private ConfigJsonHandler _configJsonHandler;

        public AddParametersFilter(ConfigJsonHandler configJsonHandler)
        {
            _configJsonHandler = configJsonHandler;
        }

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
