![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 14/04/2021                                                 |
| ------------- | ------------------------------------------------------------ |
|Titulo|LINKED DATA SERVER| 
|Descripción|Manual del servicio LINKED DATA SERVER|
|Versión|1|
|Módulo|API DISCOVER|
|Tipo|Manual|
|Cambios de la Versión|Cambios en la configuración|

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/dashboard?id=LinkedDataServer)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=LinkedDataServer&metric=bugs)](https://sonarcloud.io/dashboard?id=LinkedDataServer)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=LinkedDataServer&metric=security_rating)](https://sonarcloud.io/dashboard?id=LinkedDataServer)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=LinkedDataServer&metric=ncloc)](https://sonarcloud.io/dashboard?id=LinkedDataServer)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=LinkedDataServer&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=LinkedDataServer)

[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

## LINKED DATA SERVER de ASIO

[Introducción](#introducción)

[Ejemplos de resolución de URIs](#ejemplos-de-resolución-de-uris) 

[Conexión a Triple Store](#conexión-a-triple-store)

[Configuración de appsettings.json](#configuración-de-appsettings.json)

[Configuración de Linked_Data_Server_Config.json](#configuración-de-linked-data-server-config.json)

[Dependencias](#dependencias)


Introducción
------------

LINKED DATA SERVER es un componente de ASIO desarrollado en .Net Core que proporciona el servicio de datos enlazados de Hércules ASIO.

En la ejecución del proyecto se ha optado por el desarrollo de un componente propio, en lugar de integrar desarrollos existentes de software abierto, como [Trellis](https://www.trellisldp.org/) o [Trifid](https://zazuko.com/products/trifid/), por tres motivos.

En primer lugar, las soluciones analizadas están desarrolladas con lenguajes y entornos distintos a .Net Core. Integrar un componente Linked Data Server desarrollado con otro _stack_ tecnológico no habría sido un gran obstáculo pero, en lo posible, hemos preferido mantener la homogeneidad tecnológica.

En segundo, el uso de estos servicios tampoco era inmediato ni trivial, sino que habría requerido de unos tiempos de análisis, personalización y configuración relevantes, especialmente si hay que mostrar datos con una cierta profundidad en sus relaciones, como sucede en muchas entidades de la ontología ROH.

Finalmente, tenemos el proceso de descubrimiento, que tiene que ofrecer al usuario administrador de la plataforma un interfaz con el que comparar los datos en carga con los existentes, para el caso en el que el descubrimiento no alcance el nivel de confianza que le permita decidir si una entidad en carga coincide o no con una existente. Estos datos en carga se deberían consultar del mismo modo en que ASIO permite la consulta de los datos de una entidad existente, es decir, con el interfaz del servicio Linked Data. El problema es que los desarrollos revisados no permiten la presentación de datos RDF arbitrarios (como serían los de las entidades en carga) sino que trabajan con fuentes de datos existentes. En este punto teníamos dos opciones: generar un servicio que permitiera la visualización de datos RDF arbitrarios a partir del código de un servicio Linked Data existente, con un lenguaje y arquitectura distinto al del resto del proyecto; o desarrollar un servicio propio de visualización de RDF. Descartamos la primera opción porque el tiempo de desarrollo no habría sido menor y habría añadido complejidad a nuestro desarrollo, y decidimos generar un visualizador de RDF arbitrario para el proceso de descubrimiento.

Por tanto, ya que íbamos a tener un visualizador de RDF y que la integración de otros servidores Linked Data tampoco hubiera sido trivial, decidimos generar un Linked Data Server de ASIO , coherente con la tecnología y arquitectura general del proyecto.

El Linked Data Server de ASIO, desarrollado en tecnologías .Net Core, quedará disponible para la comunidad de desarrolladores como un software abierto y reutilizable en cualquier otro proyecto de Linked Data que necesite un servicio desarrollado en el _stack_ tecnológico de Microsoft.

El Linked Data Server de ASIO cumple la especificación LDP:
[Hércules Backend ASIO. Evaluación de cumplimiento Linked Data Platform (LDP)](../../Docs/Hercules-ASIO-Evaluacion-de-cumplimiento-Linked-Data-Platform.md)
 
Ejemplos de resolución de URIs
-----------------------

Se puede comprobar el funcionamiento del servidor mediante los siguientes ejemplos:
- [http://graph.um.es/res/person/26d09e44-68bf-4629-8f4e-8ffdf27ba0b3](http://graph.um.es/res/person/26d09e44-68bf-4629-8f4e-8ffdf27ba0b3) Investigador con publicaciones y código ORCID.
- [http://graph.um.es/res/academic-article/161158](http://graph.um.es/res/academic-article/161158) Artículo científico.
- [http://graph.um.es/res/project/RADBOUDUMC](http://graph.um.es/res/project/RADBOUDUMC) Proyecto de investigación.

*Conexión a Triple Store*
-------------------------

Como no es necesario ningún conector específico para consultar un RDF Store ya que, por definición, deben tener un SPARQL Endpoint, no se ha creado ninguna librería específica de conexión al RDF Store.

El SPARQL Endpoint provisional se encuentra disponible en un servidor de la Universidad de Murcia, con acceso protegido por una VPN en la siguiente URL:

http://155.54.239.204:8890/sparql

## Configuración de appsettings.json

    {
		"Logging": {
			"LogLevel": {
				"Default": "Information",
				"Microsoft": "Warning",
				"Microsoft.Hosting.Lifetime": "Information"
			}
		},
		"AllowedHosts": "*",
		"Sparql": {
			"Graph": "",
			"Endpoint": "http://127.0.0.1:8890/sparql",
			"QueryParam": "query"
		},
		"LogPath": "",
		"OntologyGraph": "http://graph.um.es/graph/research/roh",
		"NameTitle": "Hércules",
		"ConstrainedByUrl": "",		
		"UrlHome": "https://herc-as-front-desa.atica.um.es/carga-web"
	}

Las opciones de configuración son: 
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - LogPath: Ruta en la que escribir los logs
 - Sparql.Graph: Grafo en el que se van a consultar los triples
 - Sparql.Endpoint: URL del Endpoint Sparql
 - Sparql.QueryParam: Parámetro para la query en el Endpoint Sparql
 - OntologyGraph: Grafo del Endpoint Sparql en el que está cargada la ontología
 - NameTitle: Nombre para mostrar en el título de la página tras el nombre de la entidad
 - ConstrainedByUrl: Url en la que se encuentran las restricciones ConstrainedBy 
 - UrlHome: Url con la que enlazar el logo de la cabecera 'Hércules'

## Configuración de Linked_Data_Server_Config.json

    {
		"ConfigTables": [
			{
			  "rdfType": "http://purl.org/roh/mirror/foaf#Person",
			  "tables": [
				{
				 "name": "Documentos",
          			 "fields": [ "ID", "Título", "RdfType" ],
           			 "query": "select distinct ?ID ?Nombre ?RdfType where { {?ID <http://purl.org/roh/mirror/bibo#authorList> ?lista. ?lista ?p <{ENTITY_ID}>.}UNION{?ID <http://purl.org/roh#correspondingAuthor> <{ENTITY_ID}>.} ?ID <http://purl.org/roh#title> ?Nombre. ?ID a ?RdfType. } "
       				 }
			   ]
			}
		 ],
		 "ExcludeRelatedEntity": [ "http://purl.org/roh/mirror/foaf#Person" ],
		 "ConfigArborGraphs": {
			 "icons": [
      				{
					"rdfType": "http://purl.org/roh/mirror/foaf#Person",
					"icon": "person-grafo-hercules.svg"
      				 }
    			   ],
			  "arborGraphsRdfType": [
			  	{
					"rdfType": "http://purl.org/roh/mirror/foaf#Person",
					"arborGraphs": [
				  		{
						    "name": "Coautores",
						    "properties": [
				      			{
								"name": "Coautor",
								"query": "select distinct ?coautorID_1 as ?level1 ?coautorID_2 as ?level2 where { ?doc <http://purl.org/roh/mirror/bibo#authorList> ?lista. ?lista ?autores ?coautorID_1. ?lista ?autores2 ?coautorID_2. FILTER(?coautorID_1 in (?coautorID_A)) FILTER(?coautorID_2 in (?coautorID_B)) FILTER(?coautorID_1 != ?coautorID_2 ) { select ?coautorID_A where { ?doc_A <http://purl.org/roh/mirror/bibo#authorList> ?lista_A. ?lista_A ?autor_A <{ENTITY_ID}>. ?lista_A ?autores2_A ?coautorID_A. ?coautorID_A a ?rdftype_A. FILTER(?rdftype_A = <http://purl.org/roh/mirror/foaf#Person>). ?coautorID_A <http://purl.org/roh/mirror/foaf#name> ?name_A. filter(?coautorID_A !=<{ENTITY_ID}>) } order by desc (count(distinct ?doc_A )) asc(?coautorID_A) limit 10 } { select ?coautorID_B where { ?doc_B <http://purl.org/roh/mirror/bibo#authorList> ?lista_B. ?lista_B ?autor_B <{ENTITY_ID}>. ?lista_B ?autores2_B ?coautorID_B. ?coautorID_B a ?rdftype_B. FILTER(?rdftype_B = <http://purl.org/roh/mirror/foaf#Person>). ?coautorID_B <http://purl.org/roh/mirror/foaf#name> ?name. filter(?coautorID_B !=<{ENTITY_ID}>) } order by desc (count(distinct ?doc_B )) asc(?coautorID_B ) limit 10 } }"
				      			  }
				    		      ]
				  		 }
					 ]
			      	  }
			     ]
		 },
		 "PropsTitle": [ "http://purl.org/roh#title", "http://purl.org/roh/mirror/foaf#name" ],
		 "PropsTransform": [
			    {
				      "property": "http://purl.org/roh/mirror/vivo#researcherId",
				      "transform": "http://www.researcherid.com/rid/{value}"
			    },
			    {
				      "property": "http://purl.org/roh#ORCID",
				      "transform": "https://orcid.org/{value}"
			    },
			    {
				      "property": "http://purl.org/roh/mirror/vivo#scopusId",
				      "transform": "https://www.scopus.com/authid/detail.uri?authorId={value}"
			    },
			    {
				      "property": "http://purl.org/roh#researcherDBLP",
				      "transform": "https://dblp.org/pid/{value}.html"
			    },
			    {
				      "property": "http://purl.org/roh#roDBLP",
				      "transform": "https://dblp.org/rec/{value}.html"
			    },
			    {
				      "property": "http://purl.org/roh/mirror/bibo#doi",
				      "transform": "https://doi.org/{value}"
			    },
			    {
				      "property": "http://purl.org/roh#roPubmed",
				      "transform": "https://pubmed.ncbi.nlm.nih.gov/{value}/"
			    }
		 ]
	}

Las opciones de configuración son: 
 - ConfigTables.rdfType: Si la entidad es del mismo rdfType, se muestra la tabla
 - ConfigTables.tables.name: Nombre de la tabla a mostrar
 - ConfigTables.tables.fields: Nombre de los campos a mostrar en la tabla
 - ConfigTables.tables.query: Consulta que obtiene los datos a mostrar en la tabla
 - ExcludeRelatedEntity: Entidades relacionadas a excluir
 - ConfigArborGraphs.icons: Icono a mostrar en el gráfico según su rdfType
 - arborGraphsRdfType.rdfType: rdfType de la entidad principal del gráfico
 - arborGraphsRdfType.arborGraps.name: Nombre del gráfico a mostrar
 - arborGraphsRdfType.arborGraps.properties.name: Nombre de las propiedades del gráfico
 - arborGraphsRdfType.arborGraps.properties.query: Consulta que obtiene los datos a mostrar en el gráfico
 - PropsTitle: Propiedades de la ontología para utilizar como título de las entidades en la presentación de la web. Generalmente http://purl.org/roh/mirror/foaf#name para las personas y http://purl.org/roh#title para el resto de entidades
 - PropsTransform.property: Propiedades cuya presentación en la web se transforma. Se visualizarán transformadas como enlaces externos según lo especificado en PropsTransform.transform
 - PropsTransform.transform: Las propiedades especificadas en PropsTransform.property se visualizarán como un hipervínculo en la web según lo especificado en esta configuración

## Dependencias

- **dotNetRDF**: versión 2.6.0
- **Marvin.Cache.Headers**: versión 5.0.1
- **Microsoft.AspNetCore.Http.Extensions**: versión 2.2.0
- **Serilog.AspNetCore**: versión 3.4.0
