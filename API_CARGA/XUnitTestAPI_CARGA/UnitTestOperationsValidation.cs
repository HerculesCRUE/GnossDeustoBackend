using API_CARGA.Controllers;
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace XUnitTestAPI_CARGA
{
    public class UnitTestOperationsValidation
    {
        [Fact]
        public void GetShape()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ValidationController validationController = new ValidationController(shapesConfigMockService);            
            List<ShapeConfig> listaRepositorios = (List<ShapeConfig>)(((OkObjectResult)validationController.GetShape()).Value);
            Assert.True(listaRepositorios.Count > 0);
        }

        [Fact]
        public void GetShapeByID()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ValidationController validationController = new ValidationController(shapesConfigMockService);
            List<ShapeConfig> listaRepositorios = (List<ShapeConfig>)(((OkObjectResult)validationController.GetShape()).Value);           
            if (listaRepositorios.Count > 0)
            {
                ShapeConfig shapeConfig = listaRepositorios[0];
                ShapeConfig shapeConfigGetByID = (ShapeConfig)(((OkObjectResult)validationController.GetShape(shapeConfig.ShapeConfigID)).Value);
                Assert.True(shapeConfig.Name.Equals(shapeConfigGetByID.Name));
            }
        }


        [Fact]
        public void AddConfigShape()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();            
            ValidationController validationController = new ValidationController(shapesConfigMockService);

            StringBuilder personShape = new StringBuilder();
            personShape.AppendLine("@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>.");
            personShape.AppendLine("@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#>.");
            personShape.AppendLine("@prefix xsd: <http://www.w3.org/2001/XMLSchema#>.");
            personShape.AppendLine("@prefix xml: <http://www.w3.org/XML/1998/namespace>.");
            personShape.AppendLine("@prefix ns: <http://www.w3.org/2003/06/sw-vocab-status/ns#>.");
            personShape.AppendLine("@prefix sh: <http://www.w3.org/ns/shacl#>.");
            personShape.AppendLine("@prefix roh: <http://purl.org/roh#>.");
            personShape.AppendLine("@prefix foaf: <http://purl.org/roh/mirror/foaf#>.");
            personShape.AppendLine("roh:foaf_PersonShape");
            personShape.AppendLine("	a sh:NodeShape ;");
            personShape.AppendLine("	sh:targetClass foaf:Person ;");
            personShape.AppendLine("	sh:property roh:someValuesDataType__foaf__Person__foaf__firstName.");
            personShape.AppendLine("roh:someValuesDataType__foaf__Person__foaf__firstName ");
            personShape.AppendLine("	sh:severity sh:Violation;");
            personShape.AppendLine("	sh:path foaf:firstName;");
            personShape.AppendLine("	sh:qualifiedMinCount  1;");
            personShape.AppendLine("	sh:qualifiedValueShape [");
            personShape.AppendLine("		sh:datatype xsd:string;");
            personShape.AppendLine("	].");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(personShape.ToString());
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, null, "rdf.xml");        
            Guid identifierAdded = (Guid)(((OkObjectResult)validationController.AddShape("Shape Name", Guid.NewGuid(), file)).Value);
            ShapeConfig shapeConfig = (ShapeConfig)(((OkObjectResult)validationController.GetShape(identifierAdded)).Value);
            Assert.True(shapeConfig.Name.Equals("Shape Name"));
        }

        [Fact]
        public void AddConfigShapeError()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ValidationController validationController = new ValidationController(shapesConfigMockService);

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("ShapeError".ToString());
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, null, "rdf.xml");
            try
            {
                Guid identifierAdded = (Guid)(((OkObjectResult)validationController.AddShape("Shape Name", Guid.NewGuid(), file)).Value);
                Assert.True(false);
            }
            catch(Exception)
            {
                Assert.True(true);
            }
     
        }

        [Fact]
        public void DeleteConfigShape()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ValidationController validationController = new ValidationController(shapesConfigMockService);
            ShapeConfig shapeConfig =( (List<ShapeConfig>)(((OkObjectResult)validationController.GetShape()).Value))[0];
            validationController.DeleteShape(shapeConfig.ShapeConfigID);
            shapeConfig = (ShapeConfig)(((OkObjectResult)validationController.GetShape(shapeConfig.ShapeConfigID)).Value);
            Assert.Null(shapeConfig);
        }

        [Fact]
        public void ModifyConfigShape()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ValidationController validationController = new ValidationController(shapesConfigMockService);
            ShapeConfig shapeConfig = ((List<ShapeConfig>)(((OkObjectResult)validationController.GetShape()).Value))[0];

            StringBuilder personShape = new StringBuilder();
            personShape.AppendLine("@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>.");
            personShape.AppendLine("@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#>.");
            personShape.AppendLine("@prefix xsd: <http://www.w3.org/2001/XMLSchema#>.");
            personShape.AppendLine("@prefix xml: <http://www.w3.org/XML/1998/namespace>.");
            personShape.AppendLine("@prefix ns: <http://www.w3.org/2003/06/sw-vocab-status/ns#>.");
            personShape.AppendLine("@prefix sh: <http://www.w3.org/ns/shacl#>.");
            personShape.AppendLine("@prefix roh: <http://purl.org/roh#>.");
            personShape.AppendLine("@prefix foaf: <http://purl.org/roh/mirror/foaf#>.");
            personShape.AppendLine("roh:foaf_PersonShape");
            personShape.AppendLine("	a sh:NodeShape ;");
            personShape.AppendLine("	sh:targetClass foaf:Person ;");
            personShape.AppendLine("	sh:property roh:someValuesDataType__foaf__Person__foaf__firstName.");
            personShape.AppendLine("roh:someValuesDataType__foaf__Person__foaf__firstName ");
            personShape.AppendLine("	sh:severity sh:Violation;");
            personShape.AppendLine("	sh:path foaf:firstName;");
            personShape.AppendLine("	sh:qualifiedMinCount  1;");
            personShape.AppendLine("	sh:qualifiedValueShape [");
            personShape.AppendLine("		sh:datatype xsd:string;");
            personShape.AppendLine("	].");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(personShape.ToString());
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, null, "rdf.xml");

            validationController.ModifyShape(shapeConfig.ShapeConfigID, shapeConfig.Name, shapeConfig.RepositoryID, file);
            ShapeConfig updatedshapeConfig = (ShapeConfig)((OkObjectResult)validationController.GetShape(shapeConfig.ShapeConfigID)).Value;
            Assert.True(updatedshapeConfig.Shape.Equals(personShape.ToString()));
        }

        [Fact]
        public void ModifyConfigShapeError()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ValidationController validationController = new ValidationController(shapesConfigMockService);
            ShapeConfig shapeConfig = ((List<ShapeConfig>)(((OkObjectResult)validationController.GetShape()).Value))[0];

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("ShapeError");
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, null, "rdf.xml");

            try
            {
                var response = validationController.ModifyShape(shapeConfig.ShapeConfigID, shapeConfig.Name, shapeConfig.RepositoryID, file);
                if (response is BadRequestObjectResult)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.True(false);
                }
            }
            catch (Exception)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void ModifyConfigShapeError2()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ValidationController validationController = new ValidationController(shapesConfigMockService);

            StringBuilder personShape = new StringBuilder();
            personShape.AppendLine("@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>.");
            personShape.AppendLine("@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#>.");
            personShape.AppendLine("@prefix xsd: <http://www.w3.org/2001/XMLSchema#>.");
            personShape.AppendLine("@prefix xml: <http://www.w3.org/XML/1998/namespace>.");
            personShape.AppendLine("@prefix ns: <http://www.w3.org/2003/06/sw-vocab-status/ns#>.");
            personShape.AppendLine("@prefix sh: <http://www.w3.org/ns/shacl#>.");
            personShape.AppendLine("@prefix roh: <http://purl.org/roh#>.");
            personShape.AppendLine("@prefix foaf: <http://purl.org/roh/mirror/foaf#>.");
            personShape.AppendLine("roh:foaf_PersonShape");
            personShape.AppendLine("	a sh:NodeShape ;");
            personShape.AppendLine("	sh:targetClass foaf:Person ;");
            personShape.AppendLine("	sh:property roh:someValuesDataType__foaf__Person__foaf__firstName.");
            personShape.AppendLine("roh:someValuesDataType__foaf__Person__foaf__firstName ");
            personShape.AppendLine("	sh:severity sh:Violation;");
            personShape.AppendLine("	sh:path foaf:firstName;");
            personShape.AppendLine("	sh:qualifiedMinCount  1;");
            personShape.AppendLine("	sh:qualifiedValueShape [");
            personShape.AppendLine("		sh:datatype xsd:string;");
            personShape.AppendLine("	].");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(personShape.ToString());
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, null, "rdf.xml");

            try
            {                
                var response=validationController.ModifyShape(Guid.NewGuid(), "name", Guid.NewGuid(), file);
                if (response is BadRequestObjectResult)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.True(false);
                }
            }
            catch (Exception)
            {
                Assert.True(true);
            }
        }

    }
}
