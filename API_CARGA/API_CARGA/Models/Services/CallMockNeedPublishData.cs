using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class CallMockNeedPublishData: ICallNeedPublishData
    {
        public void CallDataPublish(string rdf)
        {
            
        }

        public void CallDataValidate(string rdf, Guid repositoryIdentifier)
        {
            
        }

        public string CallGetApi(string urlMethod)
        {
            string xml = "";
            if (urlMethod.Contains("ListIdentifiers"))
            {
                xml = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?><OAI-PMH xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\" xmlns=\"http://www.openarchives.org/OAI/2.0/\"><responseDate>2020-05-18T16:44:51Z</responseDate><request verb=\"ListIdentifiers\" metadataPrefix=\"rdf\" from=\"2020-05-17 17:05:04Z\" until=\"2020-05-19 17:05:04Z\">http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH</request><ListIdentifiers><header><identifier>100</identifier><datestamp>2020-05-18T15:04:53Z</datestamp><setSpec>cvn</setSpec></header></ListIdentifiers></OAI-PMH>";
            }
            else if (urlMethod.Contains("GetRecord"))
            {
                StringBuilder rdfFile = new StringBuilder();
                rdfFile.Append("<? xml version = \"1.0\" encoding = \"utf - 8\" standalone = \"no\" ?>");
                rdfFile.Append("<OAI-PMH xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\" xmlns=\"http://www.openarchives.org/OAI/2.0/\">");
                rdfFile.Append("<responseDate>2020-05-18T16:56:06Z</responseDate>");
                rdfFile.Append("<request verb=\"GetRecord\" identifier=\"1\" metadataPrefix=\"rdf\">http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH</request>");
                rdfFile.Append("<GetRecord>");
                rdfFile.Append("<record>");
                rdfFile.Append("<header>");
                rdfFile.Append("<identifier>1</identifier>");
                rdfFile.Append("<datestamp>2020-02-09T15:16:54Z</datestamp>");
                rdfFile.Append("<setSpec>cvn</setSpec>");
                rdfFile.Append("</header>");
                rdfFile.Append("<metadata>");
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
                rdfFile.Append("</metadata>");
                rdfFile.Append("</record>");
                rdfFile.Append("</GetRecord>");
                rdfFile.Append("</OAI-PMH>");

                xml = rdfFile.ToString();
            }

            return xml;
        }

        public string CallPostApiFile(string urlMethod, MultipartFormDataContent item, string parameters = null)
        {
            return "";
        }
    }
}
