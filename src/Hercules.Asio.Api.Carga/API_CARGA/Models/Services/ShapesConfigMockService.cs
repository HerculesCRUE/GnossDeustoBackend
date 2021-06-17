// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para gestionar los shapes en memoria
using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Clase para gestionar los shapes en memoria
    ///</summary>
    public class ShapesConfigMockService : IShapesConfigService
    {
        readonly private List<ShapeConfig> _listShapesConfig;

        ///<summary>
        ///Inicializa la lista de shapes
        ///</summary>
        public ShapesConfigMockService()
        {
            _listShapesConfig = new List<ShapeConfig>();

            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_1",
                RepositoryID = Guid.NewGuid(),
                Shape = "Definition_1"
            });
            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_2",
                RepositoryID = Guid.NewGuid(),
                Shape = "Definition_1"
            });
            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_3",
                RepositoryID = Guid.NewGuid(),
                Shape = "Definition_1"
            });
            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_4",
                RepositoryID = Guid.NewGuid(),
                Shape = "Definition_1"
            });
            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_5",
                RepositoryID = Guid.NewGuid(),
                Shape = "Definition_1"
            });

            StringBuilder personShape = new StringBuilder();
            personShape.AppendLine("@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>.");
            personShape.AppendLine("@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#>.");
            personShape.AppendLine("@prefix xsd: <http://www.w3.org/2001/XMLSchema#>.");
            personShape.AppendLine("@prefix xml: <http://www.w3.org/XML/1998/namespace>.");
            personShape.AppendLine("@prefix ns: <http://www.w3.org/2003/06/sw-vocab-status/ns#>.");
            personShape.AppendLine("@prefix ro: <http://purl.org/roh/mirror/obo/ro#>.");
            personShape.AppendLine("@prefix bfo: <http://purl.org/roh/mirror/obo/bfo#>.");
            personShape.AppendLine("@prefix iao: <http://purl.org/roh/mirror/obo/iao#>.");
            personShape.AppendLine("@prefix obo: <http://purl.obolibrary.org/obo/>.");
            personShape.AppendLine("@prefix owl: <http://www.w3.org/2002/07/owl#>.");
            personShape.AppendLine("@prefix bibo: <http://purl.org/roh/mirror/bibo#>.");
            personShape.AppendLine("@prefix foaf: <https://xmlns.com/foaf/0.1/>.");
            personShape.AppendLine("@prefix iao1: <http://purl.org/roh/mirror/obo/iao#>.");
            personShape.AppendLine("@prefix skos: <http://www.w3.org/2004/02/skos/core#>.");
            personShape.AppendLine("@prefix vivo: <http://purl.org/roh/mirror/vivo#>.");
            personShape.AppendLine("@prefix skos1: <http://purl.org/roh/mirror/skos#>.");
            personShape.AppendLine("@prefix terms: <http://purl.org/dc/terms/>.");
            personShape.AppendLine("@prefix vitro: <http://vitro.mannlib.cornell.edu/ns/vitro/0.7#>.");
            personShape.AppendLine("@prefix uneskos: <http://purl.org/umu/uneskos#>.");
            personShape.AppendLine("@prefix skos-thes: <http://purl.org/iso25964/skos-thes#>.");
            personShape.AppendLine("@prefix sh: <http://www.w3.org/ns/shacl#>.");
            personShape.AppendLine("@prefix roh: <http://purl.org/roh#>.");
            personShape.AppendLine("@prefix foaf1: <http://purl.org/roh/mirror/foaf#>.");
            personShape.AppendLine("@prefix vcard: <http://purl.org/roh/mirror/vcard#>.");
            personShape.AppendLine("roh:foaf1_PersonShape");
            personShape.AppendLine("\ta sh:NodeShape ;");
            personShape.AppendLine("\th:targetClass foaf1:Person ;");
            personShape.AppendLine("\th:property roh:someValuesDataType__foaf1__Person__foaf1__firstName;");
            personShape.AppendLine("\tsh:property roh:someValuesDataType__foaf1__Person__vivo__identifier;");
            personShape.AppendLine("\tsh:property roh:someValuesDataType__foaf1__Person__foaf1__surname;");
            personShape.AppendLine("\tsh:property roh:allValuesDataType__foaf1__Person__foaf1__surname;");
            personShape.AppendLine("\tsh:property roh:allValuesDataType__foaf1__Person__foaf1__firstName;");
            personShape.AppendLine("\tsh:property roh:allValuesDataType__foaf1__Person__vivo__identifier.");
            personShape.AppendLine("roh:someValuesDataType__foaf1__Person__foaf1__firstName ");
            personShape.AppendLine("\tsh:severity sh:Violation;");
            personShape.AppendLine("\tsh:path foaf1:firstName;");
            personShape.AppendLine("\tsh:qualifiedMinCount  1;");
            personShape.AppendLine("\tsh:qualifiedValueShape [");
            personShape.AppendLine("\t\tsh:datatype xsd:string;");
            personShape.AppendLine("\t].");
            personShape.AppendLine("roh:someValuesDataType__foaf1__Person__vivo__identifier ");
            personShape.AppendLine("\tsh:severity sh:Violation;");
            personShape.AppendLine("\tsh:path vivo:identifier;");
            personShape.AppendLine("\tsh:qualifiedMinCount  1;");
            personShape.AppendLine("\tsh:qualifiedValueShape [");
            personShape.AppendLine("\t\tsh:datatype xsd:string;");
            personShape.AppendLine("\t].");
            personShape.AppendLine("roh:someValuesDataType__foaf1__Person__foaf1__surname ");
            personShape.AppendLine("\tsh:severity sh:Violation;");
            personShape.AppendLine("\tsh:path foaf1:surname;");
            personShape.AppendLine("\tsh:qualifiedMinCount  1;");
            personShape.AppendLine("\tsh:qualifiedValueShape [");
            personShape.AppendLine("\t\tsh:datatype xsd:string;");
            personShape.AppendLine("\t].");
            personShape.AppendLine("roh:allValuesDataType__foaf1__Person__foaf1__surname ");
            personShape.AppendLine("\tsh:severity sh:Violation;");
            personShape.AppendLine("\tsh:path foaf1:surname;");
            personShape.AppendLine("\tsh:datatype xsd:string.");
            personShape.AppendLine("roh:allValuesDataType__foaf1__Person__foaf1__firstName ");
            personShape.AppendLine("\tsh:severity sh:Violation;");
            personShape.AppendLine("\tsh:path foaf1:firstName;");
            personShape.AppendLine("\tsh:datatype xsd:string.");
            personShape.AppendLine("roh:allValuesDataType__foaf1__Person__vivo__identifier ");
            personShape.AppendLine("\tsh:severity sh:Violation;");
            personShape.AppendLine("\tsh:path vivo:identifier;");
            personShape.AppendLine("\tsh:datatype xsd:string.");

            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = new Guid("390cde26-b39d-41c8-89e0-b87c207d8cf0"),
                Name = "ShapeConfig_5",
                RepositoryID = new Guid("390cde26-b39d-41c8-89e0-b87c207d8cf2"),
                Shape = personShape.ToString()
            });
        }

        ///<summary>
        ///Añade un shape
        ///</summary>
        ///<param name="shapeConfig">Shape a añadir</param>
        public Guid AddShapeConfig(ShapeConfig shapeConfig)
        {
            Guid addedID = Guid.NewGuid();
            shapeConfig.ShapeConfigID = addedID;
            _listShapesConfig.Add(shapeConfig);
            return addedID;
        }

        ///<summary>
        ///Devuelve un shape
        ///</summary>
        ///<param name="id">Identificador del shape a devolver</param>
        public ShapeConfig GetShapeConfigById(Guid id)
        {
            return _listShapesConfig.FirstOrDefault(shape => shape.ShapeConfigID.Equals(id));
        }

        ///<summary>
        ///Devuelve una lista shapes
        ///</summary>
        public List<ShapeConfig> GetShapesConfigs()
        {
            return _listShapesConfig.OrderBy(shape=>shape.Name).ToList();
        }

        ///<summary>
        ///Modifica un shape existente
        ///</summary>
        ///<param name="shapeConfig">Shape a modificar con los nuevos valores</param>
        public bool ModifyShapeConfig(ShapeConfig shapeConfig)
        {
            bool modified = false;
            ShapeConfig shapeConfigOriginal = GetShapeConfigById(shapeConfig.ShapeConfigID);
            if(shapeConfigOriginal != null)
            {
                shapeConfigOriginal.Name = shapeConfig.Name;
                shapeConfigOriginal.Shape = shapeConfig.Shape;
                shapeConfigOriginal.RepositoryID = shapeConfig.RepositoryID;
                modified = true;
            }
            return modified;
        }

        ///<summary>
        ///Elimina un shape existente
        ///</summary>
        ///<param name="identifier">Identificador del shape a eliminar</param>
        public bool RemoveShapeConfig(Guid identifier)
        {
            try
            {
                ShapeConfig shapeConfig = GetShapeConfigById(identifier);
                if(shapeConfig != null)
                {
                    _listShapesConfig.Remove(shapeConfig);
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            } 
        }
    }
}
