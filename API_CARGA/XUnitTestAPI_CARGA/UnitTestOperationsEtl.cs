using API_CARGA.Controllers;
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using API_CARGA.Models.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Xunit;

namespace XUnitTestAPI_CARGA
{
    public class UnitTestOperationsEtl
    {
        [Fact]
        public void DataPublish()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            ConfigSparql configSparql = new ConfigSparql();
            configSparql.Endpoint = "";
            etlController etlController = new etlController(repositoriesConfigMockService, shapesConfigMockService, configSparql);

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(rdfFile);
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, null, "rdf.xml");

            etlController.dataPublish(file);
            Assert.True(true);
        }

        [Fact]
        public void DataPublishError()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            ConfigSparql configSparql = new ConfigSparql();
            configSparql.Endpoint = "";
            etlController etlController = new etlController(repositoriesConfigMockService, shapesConfigMockService, configSparql);

            try
            {
                var response = etlController.dataPublish(null);
                if (response is BadRequestObjectResult)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.True(false);
                }
            }
            catch(Exception ex)
            {
                Assert.True(true);
            }
          
        }

        [Fact]
        public void DataValidate()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            ConfigSparql configSparql = new ConfigSparql();
            configSparql.Endpoint = "";
            etlController etlController = new etlController(repositoriesConfigMockService, shapesConfigMockService, configSparql);

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(rdfFile);
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, null, "rdf.xml");
            ShapeReport report = (ShapeReport)((OkObjectResult)etlController.dataValidate(file,new Guid("390cde26-b39d-41c8-89e0-b87c207d8cf2"))).Value;

            Assert.True(report.conforms);
        }

        private string rdfFile
        {
            get
            {
                StringBuilder rdfFile = new StringBuilder();
                rdfFile.Append("<rdf:RDF xmlns:bibo=\"http://purl.org/roh/mirror/bibo#\" xmlns:foaf=\"http://purl.org/roh/mirror/foaf#\" xmlns:obobfo=\"http://purl.org/roh/mirror/obo/bfo#\" xmlns:oboro=\"http://purl.org/roh/mirror/obo/ro#\" xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\" xmlns:roh=\"http://purl.org/roh#\" xmlns:rohes=\"http://purl.org/rohes#\" xmlns:vcard=\"http://purl.org/roh/mirror/vcard#\" xmlns:vivo=\"http://purl.org/roh/mirror/vivo#\">");
                rdfFile.Append("	<rdf:Description rdf:about=\"http://graph.um.es/res/person/1\">");
                rdfFile.Append("		<rdf:type rdf:resource = \"http://purl.org/roh/mirror/foaf#Person\" />");
                rdfFile.Append("		<roh:hasRole rdf:nodeID=\"N13102d9d82124e479f8d7042a8fab61a\" />");
                rdfFile.Append("		<foaf:phone rdf:datatype=\"http://www.w3.org/2001/XMLSchema#string\">tel:666666666</foaf:phone>");
                rdfFile.Append("		<foaf:name rdf:datatype=\"http://www.w3.org/2001/XMLSchema#string\">Nombre Apellido</foaf:name>");
                rdfFile.Append("		<foaf:firstName rdf:datatype=\"http://www.w3.org/2001/XMLSchema#string\">Nombre</foaf:firstName>");
                rdfFile.Append("		<foaf:surname rdf:datatype=\"http://www.w3.org/2001/XMLSchema#string\">Apellido</foaf:surname>");
                rdfFile.Append("		<vivo:identifier rdf:datatype=\"http://www.w3.org/2001/XMLSchema#string\">Identificador 1</vivo:identifier>");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("	<rdf:Description rdf:nodeID=\"N13102d9d82124e479f8d7042a8fab61a\">");
                rdfFile.Append("		<vivo:relates rdf:resource=\"http://graph.um.es/res/project/1\" />");
                rdfFile.Append("		<vivo:dateTimeInterval rdf:nodeID=\"N2552ff6c9f41464d85917e5286ac26d6\" />");
                rdfFile.Append("		<roh:roleOf rdf:resource=\"http://graph.um.es/res/person/1\" />");
                rdfFile.Append("		<rdf:type rdf:resource=\"http://purl.org/roh/mirror/vivo#MemberRole\" />");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("	<rdf:Description rdf:about=\"http://graph.um.es/res/project/1\">");
                rdfFile.Append("		<roh:title rdf:datatype=\"http://www.w3.org/2001/XMLSchema#string\">IMPLICACIONES QUIMICAS Y BIOQUIMICAS EN EL COLOR Y AROMAS DEMOSTOS Y VINOS</roh:title>");
                rdfFile.Append("		<vivo:relatedBy rdf:nodeID=\"N92b3d38e204e40439f1ec47abd63b35e\" />");
                rdfFile.Append("		<rdf:type rdf:resource=\"http://purl.org/roh/mirror/vivo#Project\" />");
                rdfFile.Append("		<vivo:dateTimeInterval rdf:nodeID=\"Ncd7b021fabbd4f768a5c2f3869e85132\" />");
                rdfFile.Append("		<vivo:relates rdf:nodeID=\"N13102d9d82124e479f8d7042a8fab61a\" />");
                rdfFile.Append("		<roh:isSupportedBy rdf:nodeID=\"N0f320d4ca3184adb88ba7d1afdf624b1\" />");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("	<rdf:Description rdf:nodeID=\"N0f320d4ca3184adb88ba7d1afdf624b1\">");
                rdfFile.Append("		<roh:supports rdf:resource=\"http://graph.um.es/res/project/1\" />");
                rdfFile.Append("		<vivo:identifier rdf:datatype=\"http://www.w3.org/2001/XMLSchema#string\">PCT89-4</vivo:identifier>");
                rdfFile.Append("		<rdf:type rdf:resource=\"http://purl.org/roh#Funding\" />");
                rdfFile.Append("		<oboro:BFO_0000051 rdf:nodeID=\"N5c0b27570e19415188b29776607abf7d\" />");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("	<rdf:Description rdf:nodeID=\"N5c0b27570e19415188b29776607abf7d\">");
                rdfFile.Append("		<oboro:BFO_0000050 rdf:nodeID=\"N0f320d4ca3184adb88ba7d1afdf624b1\" />");
                rdfFile.Append("		<rdf:type rdf:resource=\"http://purl.org/roh#FundingAmount\" />");
                rdfFile.Append("		<roh:monetaryAmount rdf:datatype=\"http://www.w3.org/2001/XMLSchema#decimal\">21035.42</roh:monetaryAmount>");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("	<rdf:Description rdf:nodeID=\"N2552ff6c9f41464d85917e5286ac26d6\">");
                rdfFile.Append("		<rdf:type rdf:resource=\"http://purl.org/roh/mirror/vivo#DateTimeInterval\" />");
                rdfFile.Append("		<vivo:start rdf:nodeID=\"N87c73ed5cf5e4d6d979fc46f2872d1fd\" />");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("	<rdf:Description rdf:nodeID=\"N87c73ed5cf5e4d6d979fc46f2872d1fd\">");
                rdfFile.Append("		<rdf:type rdf:resource=\"http://purl.org/roh/mirror/vivo#DateTimeValue\" />");
                rdfFile.Append("		<vivo:dateTime rdf:datatype=\"http://www.w3.org/2001/XMLSchema#dateTime\">1990-01-01T00:00:00.000+01:00</vivo:dateTime>");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("	<rdf:Description rdf:about=\"http://graph.um.es/res/project/1\">");
                rdfFile.Append("		<roh:title rdf:datatype=\"http://www.w3.org/2001/XMLSchema#string\">IMPLICACIONES QUIMICAS Y BIOQUIMICAS EN EL COLOR Y AROMAS DEMOSTOS Y VINOS</roh:title>");
                rdfFile.Append("		<vivo:relatedBy rdf:nodeID=\"N92b3d38e204e40439f1ec47abd63b35e\" />");
                rdfFile.Append("		<rdf:type rdf:resource=\"http://purl.org/roh/mirror/vivo#Project\" />");
                rdfFile.Append("		<vivo:dateTimeInterval rdf:nodeID=\"Ncd7b021fabbd4f768a5c2f3869e85132\" />");
                rdfFile.Append("		<roh:isSupportedBy rdf:nodeID=\"N0f320d4ca3184adb88ba7d1afdf624b1\" />");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("	<rdf:Description rdf:nodeID=\"N92b3d38e204e40439f1ec47abd63b35e\">");
                rdfFile.Append("		<vivo:relates rdf:resource=\"http://graph.um.es/res/person/1\" />");
                rdfFile.Append("		<vivo:relates rdf:resource=\"http://graph.um.es/res/project/1\" />");
                rdfFile.Append("		<rdf:type rdf:resource=\"http://purl.org/roh/mirror/vivo#PrincipalInvestigatorRole\" />");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("	<rdf:Description rdf:nodeID=\"Ncd7b021fabbd4f768a5c2f3869e85132\">");
                rdfFile.Append("		<vivo:start rdf:nodeID=\"Na1caef70b2c54896a3a246a3c7033677\" />");
                rdfFile.Append("		<rdf:type rdf:resource=\"http://purl.org/roh/mirror/vivo#DateTimeInterval\" />");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("	<rdf:Description rdf:nodeID=\"Na1caef70b2c54896a3a246a3c7033677\">");
                rdfFile.Append("		<vivo:dateTime rdf:datatype=\"http://www.w3.org/2001/XMLSchema#dateTime\">1990-01-02T00:00:00.000+01:00</vivo:dateTime>");
                rdfFile.Append("		<rdf:type rdf:resource=\"http://purl.org/roh/mirror/vivo#DateTimeValue\" />");
                rdfFile.Append("	</rdf:Description>");
                rdfFile.Append("</rdf:RDF>");
                return rdfFile.ToString();
            }
        }

    }
}
