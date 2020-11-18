![](..//Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 16/11/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|LINKED DATA SERVER readme| 
|Descripción|Manual del servicio LINKED DATA SERVER|
|Versión|0.1|
|Módulo|API DISCOVER|
|Tipo|Manual|
|Cambios de la Versión|Creación|

## Sobre LINKED DATA SERVER
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

LINKED DATA SERVER es el Servidor Linked Data de Hércules ASIO.

Para una especificación más detallada acerca de las acaracterísticas de este servidor se puede consultar la siguiente documentación:
[Hércules Backend ASIO. Evaluación de cumplimiento Linked Data Platform (LDP)](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/20200415%20H%C3%A9rcules%20ASIO%20Evaluaci%C3%B3n%20de%20cumplimiento%20Linked%20Data%20Platform.md)
 
 
*Conexión a Triple Store*
-------------------------

Como no es necesario ningún conector específico para consultar un RDF Store ya que, por definición, deben tener un SPARQL Endpoint, no se ha creado ninguna librería específica de conexión al RDF Store.

El SPARQL Endpoint provisional se encuentra disponible en un servidor de la Universidad de Murcia, con acceso protegido por una VPN en la siguiente URL:

http://155.54.239.204:8890/sparql

## Configuración en el appsettings.json

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
		"NameTitle": "Title",
		"ConstrainedByUrl": "",
		"PropsTitle": "http://purl.org/roh#title|http://purl.org/roh/mirror/foaf#name",
		"PropsTransform": "http://purl.org/roh/mirror/vivo#researcherId|http://www.researcherid.com/rid/{value};http://purl.org/roh#ORCID|https://orcid.org/{value};http://purl.org/roh/mirror/vivo#scopusId|https://www.scopus.com/authid/detail.uri?authorId={value};http://purl.org/roh#researcherDBLP|https://dblp.org/pid/{value}.html;http://purl.org/roh#roDBLP|https://dblp.org/rec/{value}.html;http://purl.org/roh/mirror/bibo#doi|https://doi.org/{value};http://purl.org/roh#roPubmed|https://pubmed.ncbi.nlm.nih.gov/{value}/;"
	}
		 
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - LogPath: Ruta en la que escribir los logs
 - Sparql.Graph: Grafo en el que se van a consultar los triples
 - Sparql.Endpoint: URL del Endpoint Sparql
 - Sparql.QueryParam: Parámetro para la query en el Endpoint Sparql
 - NameTitle: Nombre para mostrar en el título de la página tras el nombre de la entidad
 - ConstrainedByUrl: Url en la que se encuentran las restricciones ConstrainedBy
 - PropsTitle: Propiedades de la ontología para utilizar como título de las entidades
 - PropsTransform: Propiedades a transformar en la presentación de las entidades
  

## Dependencias

- **dotNetRDF**: versión 2.6.0
- **Marvin.Cache.Headers**: versión 5.0.1
- **Microsoft.AspNetCore.Http.Extensions**: versión 2.2.0
- **Serilog.AspNetCore**: versión 3.4.0