using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Filters
{
    public class AddParametersFilter: IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.OperationId.Equals("GenerateUri"))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "inventado",
                    In = ParameterLocation.Query,
                    Required = false
                }) ;
            }
        }
    }
}
