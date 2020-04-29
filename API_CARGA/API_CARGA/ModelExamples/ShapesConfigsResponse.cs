using API_CARGA.Models.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;

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
                RepositoryID = Guid.NewGuid(),
                Shape = "Definition_1"
            });
            listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_2",
                RepositoryID = Guid.NewGuid(),
                Shape = "Definition_1"
            });
            listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_3",
                RepositoryID = Guid.NewGuid(),
                Shape = "Definition_1"
            });
            return listShapesConfig;
        }
    }
}
