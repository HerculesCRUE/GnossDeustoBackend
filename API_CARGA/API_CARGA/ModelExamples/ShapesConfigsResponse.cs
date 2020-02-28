using API_CARGA.Models.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.ModelExamples
{
    public class ShapesConfigsResponse : IExamplesProvider<List<ShapeConfig>>
    {
        public List<ShapeConfig> GetExamples()
        {
            List<ShapeConfig> listShapesConfig = new List<ShapeConfig>();

            listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_1",
                EntityClass = "ShapeClass",
                Shape = "Definition_1"
            });
            listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_2",
                EntityClass = "ShapeClass",
                Shape = "Definition_1"
            });
            listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_3",
                EntityClass = "ShapeClass",
                Shape = "Definition_1"
            });
            return listShapesConfig;
        }
    }
}
