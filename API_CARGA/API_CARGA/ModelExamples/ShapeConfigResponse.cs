using API_CARGA.Models.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.ModelExamples
{
    public class ShapeConfigResponse : IExamplesProvider<ShapeConfig>
    {
        public ShapeConfig GetExamples()
        {
            return (new ShapeConfig
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_1",
                EntityClass = "ShapeClass",
                Shape = "Definition_1"
            });
        }
    }
}
