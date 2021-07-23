![](.//media/CabeceraDocumentosMD.png)

| Fecha         | 23/07/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Hércules ASIO. Procedencia de datos - Provenance. Descripción del servicio SPARQL| 
|Descripción|Gestión de la procedencia de los datos incorporados a ASIO desde fuentes externas|
|Versión|1.3|
|Módulo|API DISCOVER|
|Tipo|Especificación|
|Cambios de la Versión|Añadida información acerca de la descripción del servicio SPARQL|

# Hércules Backend ASIO. Procedencia de datos - Provenance. Descripción del servicio SPARQL

[Introducción](#introducción)

[Datos externos](#datos-externos)

[Carga de datos y procedencia](#carga-de-datos-y-procedencia)

[Datos de procedencia](#datos-de-procedencia)

[Descripción del servicio SPARQL](#descripción-del-servicio-sparql)

Introducción
============
La información de Hércules ASIO procederá, en su mayor parte, del sistema SGI de la universidad, previsiblemente Hércules SGI. Sin embargo, las funciones de descubrimiento van a incorporar datos procedentes de fuentes de información externas a Hércules.
En este documento se explica como se gestionará la información de procedencia de estos datos externos, para que su origen se pueda saber en cualquier momento.

En cuanto a los datos "internos", que serán los procedentes del SGI de la universidad, el sistema de carga se limitará a indicar la fecha de carga mediante el atributo prov:endedAtTime sin describir la procedencia completa de cada entidad. Por ejemplo:

    roh:res/researcher/id1 prov:endedAtTime "2021-04-25T03:40:00Z"^^xsd:dateTime;

Generar más triples de procedencia para cada entidad, indicando el agente o la organización, supondría una sobrecarga de datos en el sistema sin justificación desde el punto de vista de la explotación de la información, ya que están implícitos por el origen SGI de los datos. 

Datos externos
==========
Los datos externos esperados son identificadores obtenidos desde fuentes externas de información utilizadas en el proceso de descubrimiento sobre resultados de investigación y también datos provenientes de otras universidades que cuenten con ASIO y consoliden sus datos en el nodo central Unidata.

Ejemplos de datos externos:

Carga de datos y procedencia
====================
Los triples generados desde fuentes externas se cargarán en el grafo de ASIO y en un grafo con nombre que identificará a cada fuente externa. Por ejemplo, el triple obtenido con el código ORCID desde una fuente externa podría ser:

    roh:res/person/ID1 roh:ORCID "0000-0001-8055-6823"
Este triple se cargaría en el grafo principal de ASIO y también en un grafo con el que indicaremos la procedencia del dato. Por ejemplo, si el dato se hubiese obtenido de DBLP, el dato se cargaría en los siguientes grafos ([nombrados según lo descrito en URIs Factory](UrisFactory/Especificacion-Esquema-de-URIs.md#uri-para-identificar-named-graphs)):

    http://graph.um.es/graph/asio
    http://graph.um.es/graph/dblp

Datos de procedencia
=============
El grafo de SGI-ASIO alojado en el RDF Store contendrá los triples que describirán cada una de las fuentes externas esperadas, de acuerdo a la ontología [PROV-O](https://www.w3.org/TR/prov-o/). Con esta información se podrá consultar y mostrar la procedencia de cada dato externo. 

Las fuentes de datos se describirán con triples similares a estos:

    @prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .
    @prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
    @prefix owl: <http://www.w3.org/2002/07/owl#> .
    @prefix prov: <http://www.w3.org/ns/prov#> .
    @prefix foaf: <http://purl.org/roh/mirror/foaf#>
    @prefix roh: <http://graph.um.es/> .
    
    roh:graph/pubmed prov:wasAttributedTo roh:res/organization/pubmed.
    roh:res/organization/pubmed 
    	a prov:Organization;
    	foaf:name "PubMed";
    	foaf:homePage "https://pubmed.ncbi.nlm.nih.gov/";
    .
    
    roh:graph/dblp prov:wasAttributedTo roh:res/organization/dblp.
    roh:res/organization/dblp 
    	a prov:Organization;
    	foaf:name "DBLP Computer Science Bibliography";
    	foaf:homePage "https://dblp.org/";
    .

En el proceso de descubrimiento se pueden incorporar triples desde fuentes externas. Por ejemplo, si se recuperase un código ORCID desde DBLP se cargarían los siguientes triples:

Grafo de SGI-ASIO:

    roh:res/researcher/id1 roh:ORCID "00000".
    
    roh:res/agent/idAgente1
	    a prov:SoftwareAgent;
	    foaf:name "Algoritmo de carga Hércules ASIO";
    .

Grafo de DBLP (roh:graph/dblp):

    roh:res/researcher/id1 prov:wasUsedBy _:bnode1.  
    _:bnode1
        a prov:Activity;
        rdf:predicate	roh:ORCID;
        rdf:object "00000";
        prov:startedAtTime "2020-04-25T01:30:00Z"^^xsd:dateTime;
        prov:endedAtTime "2012-04-25T03:40:00Z"^^xsd:dateTime;
        prov:wasAssociatedWith roh:res/agent/idAgente1;        
	    prov:wasAssociatedWith roh:res/organization/dblp;

Descripción del servicio SPARQL
============

La descripción del servicio SPARQL se obtiene mediante una petición al SPARQL Endpoint (p.e. https://linkeddata2.um.es/sparql) con la cabecera  con la cabecera “accept: application/rdf+xml”. Los datos son:

´´´
<?xml version="1.0" encoding="utf-8" ?>
<rdf:RDF
        xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#"
        xmlns:sd="http://www.w3.org/ns/sparql-service-description#"
        xmlns:ns3="http://purl.org/dc/terms/"
        xmlns:ns4="rdf:" >
  <rdf:Description rdf:about="http://linkeddata2.um.es/sparql">
    <sd:endpoint rdf:resource="https://linkeddata2.um.es/sparql" />
    <sd:feature rdf:resource="http://www.w3.org/ns/sparql-service-description#DereferencesURIs" />
    <sd:feature rdf:resource="http://www.w3.org/ns/sparql-service-description#UnionDefaultGraph" />
    <sd:resultFormat rdf:resource="http://www.w3.org/ns/formats/Turtle" />
    <sd:resultFormat rdf:resource="http://www.w3.org/ns/formats/RDF_XML" />
    <sd:resultFormat rdf:resource="http://www.w3.org/ns/formats/SPARQL_Results_CSV" />
    <sd:resultFormat rdf:resource="http://www.w3.org/ns/formats/SPARQL_Results_JSON" />
    <sd:resultFormat rdf:resource="http://www.w3.org/ns/formats/SPARQL_Results_XML" />
    <sd:resultFormat rdf:resource="http://www.w3.org/ns/formats/RDFa" />
    <sd:resultFormat rdf:resource="http://www.w3.org/ns/formats/N-Triples" />
    <sd:resultFormat rdf:resource="http://www.w3.org/ns/formats/N3" />
    <sd:supportedLanguage rdf:resource="http://www.w3.org/ns/sparql-service-description#SPARQL10Query" />
    <sd:url rdf:resource="https://linkeddata2.um.es/sparql" />
    <ns3:description>Datos de investigación de la universidad de Murcia</ns3:description>
    <ns3:source rdf:resource="https://linkeddata2.um.es/carga-web/public/home" />
    <ns3:title>Hercules ASIO Backend SGI</ns3:title>
    <ns4:type rdf:resource="http://www.w3.org/ns/sparql-service-description#Service" />
  </rdf:Description>
</rdf:RDF>
´´´

Para configurar esta información en Virtuoso hay que:
 - Editar el fichero virtuoso.ini y añadir la opción que define el SPARQL por defecto, por ejemplo:

```
[URIQA]
...
;DefaultHost  = localhost:8890
DefaultHost  = linkeddata2.um.es
```

 - Insertar los triples descriptivos en un grafo sparql, por ejemplo http://linkeddata2.um.es/sparql. Los triples son:

```
insert into <http://linkeddata2.um.es/sparql>
{
	<http://linkeddata2.um.es/sparql> <http://purl.org/dc/terms/title> "Hercules ASIO Backend SGI". 
	<http://linkeddata2.um.es/sparql> <http://purl.org/dc/terms/description> "Datos de investigación de la universidad de Murcia".
    <http://linkeddata2.um.es/sparql> <http://purl.org/dc/terms/source> <https://linkeddata2.um.es/carga-web/public/home>.
	<http://linkeddata2.um.es/sparql> <rdf:type> <http://www.w3.org/ns/sparql-service-description#Service> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#endpoint> <https://linkeddata2.um.es/sparql> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#feature> <http://www.w3.org/ns/sparql-service-description#UnionDefaultGraph> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#feature> <http://www.w3.org/ns/sparql-service-description#DereferencesURIs> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#resultFormat> <http://www.w3.org/ns/formats/RDF_XML> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#resultFormat> <http://www.w3.org/ns/formats/SPARQL_Results_CSV> . 
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#resultFormat> <http://www.w3.org/ns/formats/SPARQL_Results_JSON> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#resultFormat> <http://www.w3.org/ns/formats/SPARQL_Results_XML> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#resultFormat> <http://www.w3.org/ns/formats/RDFa> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#resultFormat> <http://www.w3.org/ns/formats/N-Triples> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#resultFormat> <http://www.w3.org/ns/formats/N3> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#resultFormat> <http://www.w3.org/ns/formats/Turtle> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#supportedLanguage> <http://www.w3.org/ns/sparql-service-description#SPARQL10Query> .
	<http://linkeddata2.um.es/sparql> <http://www.w3.org/ns/sparql-service-description#url> <https://linkeddata2.um.es/sparql> .
}
```
