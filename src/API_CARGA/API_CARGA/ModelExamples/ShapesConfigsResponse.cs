// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve para mostrar un ejemplo de respuesta de una lista de Shapes
using API_CARGA.Models.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace API_CARGA.ModelExamples
{
    ///<summary>
    ///Sirve para mostrar un ejemplo de respuesta de una lista de Shapes
    ///</summary>
    ///
    [ExcludeFromCodeCoverage]
    public class ShapesConfigsResponse : IExamplesProvider<List<ShapeConfig>>
    {
        public List<ShapeConfig> GetExamples()
        {
            List<ShapeConfig> listShapesConfig = new List<ShapeConfig>();
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
            personShape.AppendLine("	a sh:NodeShape ;");
            personShape.AppendLine("	sh:targetClass foaf1:Person ;");
            personShape.AppendLine("	sh:property roh:someValuesDataType__foaf1__Person__foaf1__firstName;");
            personShape.AppendLine("	sh:property roh:someValuesDataType__foaf1__Person__vivo__identifier;");
            personShape.AppendLine("	sh:property roh:someValuesDataType__foaf1__Person__foaf1__surname;");
            personShape.AppendLine("	sh:property roh:allValuesDataType__foaf1__Person__foaf1__surname;");
            personShape.AppendLine("	sh:property roh:allValuesDataType__foaf1__Person__foaf1__firstName;");
            personShape.AppendLine("	sh:property roh:allValuesDataType__foaf1__Person__vivo__identifier.");
            personShape.AppendLine("roh:someValuesDataType__foaf1__Person__foaf1__firstName ");
            personShape.AppendLine("	sh:severity sh:Violation;");
            personShape.AppendLine("	sh:path foaf1:firstName;");
            personShape.AppendLine("	sh:qualifiedMinCount  1;");
            personShape.AppendLine("	sh:qualifiedValueShape [");
            personShape.AppendLine("		sh:datatype xsd:string;");
            personShape.AppendLine("	].");
            personShape.AppendLine("roh:someValuesDataType__foaf1__Person__vivo__identifier ");
            personShape.AppendLine("	sh:severity sh:Violation;");
            personShape.AppendLine("	sh:path vivo:identifier;");
            personShape.AppendLine("	sh:qualifiedMinCount  1;");
            personShape.AppendLine("	sh:qualifiedValueShape [");
            personShape.AppendLine("		sh:datatype xsd:string;");
            personShape.AppendLine("	].");
            personShape.AppendLine("roh:someValuesDataType__foaf1__Person__foaf1__surname ");
            personShape.AppendLine("	sh:severity sh:Violation;");
            personShape.AppendLine("	sh:path foaf1:surname;");
            personShape.AppendLine("	sh:qualifiedMinCount  1;");
            personShape.AppendLine("	sh:qualifiedValueShape [");
            personShape.AppendLine("		sh:datatype xsd:string;");
            personShape.AppendLine("	].");
            personShape.AppendLine("roh:allValuesDataType__foaf1__Person__foaf1__surname ");
            personShape.AppendLine("	sh:severity sh:Violation;");
            personShape.AppendLine("	sh:path foaf1:surname;");
            personShape.AppendLine("	sh:datatype xsd:string.");
            personShape.AppendLine("roh:allValuesDataType__foaf1__Person__foaf1__firstName ");
            personShape.AppendLine("	sh:severity sh:Violation;");
            personShape.AppendLine("	sh:path foaf1:firstName;");
            personShape.AppendLine("	sh:datatype xsd:string.");
            personShape.AppendLine("roh:allValuesDataType__foaf1__Person__vivo__identifier ");
            personShape.AppendLine("	sh:severity sh:Violation;");
            personShape.AppendLine("	sh:path vivo:identifier;");
            personShape.AppendLine("	sh:datatype xsd:string.");
            listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_1",
                RepositoryID = Guid.NewGuid(),
                Shape = personShape.ToString()
            });
            listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_2",
                RepositoryID = Guid.NewGuid(),
                Shape = personShape.ToString()
            });
            listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_3",
                RepositoryID = Guid.NewGuid(),
                Shape = personShape.ToString()
            });
            return listShapesConfig;
        }
    }
}
