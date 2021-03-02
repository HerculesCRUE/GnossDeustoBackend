![](.//media/CabeceraDocumentosMD.png)

| Fecha         | 03/12/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Hércules ASIO. Procedencia de datos - Provenance| 
|Descripción|Gestión de la procedencia de los datos incorporados a ASIO desde fuentes externas|
|Versión|1.1|
|Módulo|API DISCOVER|
|Tipo|Especificación|
|Cambios de la Versión|Cambio en el grafo que alojará los triples de provenance|

# Hércules Backend ASIO. Procedencia de datos - Provenance

[Introducción](#introducción)

[Datos externos](#datos-externos)

[Carga de datos y procedencia](#carga-de-datos-y-procedencia)

[Datos de procedencia](#datos-de-procedencia)

Introducción
============
La información de Hércules ASIO procederá, en su mayor parte, del sistema SGI de la universidad, previsiblemente Hércules SGI. Sin embargo, las funciones de descubrimiento van a incorporar datos procedentes de fuentes de información externas a Hércules.
En este documento se explica como se gestionará la información de procedencia de estos datos, para que su origen se pueda saber en cualquier momento.

Datos externos
==========
Los datos externos esperados son identificadores obtenidos desde fuentes externas de información utilizadas en el proceso de descubrimiento sobre resultados de investigación y también datos provenientes de otras universidades que cuenten con ASIO y consoliden sus datos en el nodo central Unidata.

Ejemplos de datos externos:

Carga de datos y procedencia
====================
Los triples generados desde fuentes externas se cargarán en el grafo de ASIO y en un grafo con nombre que identificará a cada fuente externa. Por ejemplo, el triple obtenido con el código ORCID desde una fuente externa podría ser:

    roh:res/person/ID1 roh:ORCID "0000-0001-8055-6823"
Este triple se cargaría en el grafo principal de ASIO y también en un grafo con el que indicaremos la procedencia del dato. Por ejemplo, si el dato se hubiese obtenido de DBLP, el dato se cargaría en los siguientes grafos ([nombrados según lo descrito en URIs Factory](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/UrisFactory/docs/Especificaci%C3%B3n%20Esquema%20de%20URIs.md#uri-para-identificar-named-graphs)):

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

