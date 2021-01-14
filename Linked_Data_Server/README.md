![](..//Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 16/12/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|LINKED DATA SERVER readme| 
|Descripción|Manual del servicio LINKED DATA SERVER|
|Versión|0.4|
|Módulo|API DISCOVER|
|Tipo|Manual|
|Cambios de la Versión|Presentación y motivación del desarrollo de Linked Data Server|

## LINKED DATA SERVER de ASIO

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/dashboard?id=LinkedDataServer)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=LinkedDataServer&metric=bugs)](https://sonarcloud.io/dashboard?id=LinkedDataServer)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=LinkedDataServer&metric=security_rating)](https://sonarcloud.io/dashboard?id=LinkedDataServer)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=LinkedDataServer&metric=ncloc)](https://sonarcloud.io/dashboard?id=LinkedDataServer)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=LinkedDataServer&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=LinkedDataServer)

[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

LINKED DATA SERVER es un componente de ASIO desarrollado en .Net Core que proporciona el servicio de datos enlazados de Hércules ASIO.

En la ejecución del proyecto se ha optado por el desarrollo de un componente propio, en lugar de integrar desarrollos existentes de software abierto, como [Trellis](https://www.trellisldp.org/) o [Trifid](https://zazuko.com/products/trifid/), por tres motivos.

En primer lugar, las soluciones analizadas están desarrolladas con lenguajes y entornos distintos a .Net Core. Integrar un componente Linked Data Server desarrollado con otro _stack_ tecnológico no habría sido un gran obstáculo pero, en lo posible, hemos preferido mantener la homogeneidad tecnológica.

En segundo, el uso de estos servicios tampoco era inmediato ni trivial, sino que habría requerido de unos tiempos de análisis, personalización y configuración relevantes, especialmente si hay que mostrar datos con una cierta profundidad en sus relaciones, como sucede en muchas entidades de la ontología ROH.

Finalmente, tenemos el proceso de descubrimiento, que tiene que ofrecer al usuario administrador de la plataforma un interfaz con el que comparar los datos en carga con los existentes, para el caso en el que el descubrimiento no alcance el nivel de confianza que le permita decidir si una entidad en carga coincide o no con una existente. Estos datos en carga se deberían consultar del mismo modo en que ASIO permite la consulta de los datos de una entidad existente, es decir, con el interfaz del servicio Linked Data. El problema es que los desarrollos revisados no permiten la presentación de datos RDF arbitrarios (como serían los de las entidades en carga) sino que trabajan con fuentes de datos existentes. En este punto teníamos dos opciones: generar un servicio que permitiera la visualización de datos RDF arbitrarios a partir del código de un servicio Linked Data existente, con un lenguaje y arquitectura distinto al del resto del proyecto; o desarrollar un servicio propio de visualización de RDF. Descartamos la primera opción porque el tiempo de desarrollo no habría sido menor y habría añadido complejidad a nuestro desarrollo, y decidimos generar un visualizador de RDF arbitrario para el proceso de descubrimiento.

Por tanto, ya que íbamos a tener un visualizador de RDF y que la integración de otros servidores Linked Data tampoco hubiera sido trivial, decidimos generar un Linked Data Server de ASIO , coherente con la tecnología y arquitectura general del proyecto.

El Linked Data Server de ASIO, desarrollado en tecnologías .Net Core, quedará disponible para la comunidad de desarrolladores como un software abierto y reutilizable en cualquier otro proyecto de Linked Data que necesite un servicio desarrollado en el _stack_ tecnológico de Microsoft.

El Linked Data Server de ASIO cumple la especificación LDP:
[Hércules Backend ASIO. Evaluación de cumplimiento Linked Data Platform (LDP)](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/20200415%20H%C3%A9rcules%20ASIO%20Evaluaci%C3%B3n%20de%20cumplimiento%20Linked%20Data%20Platform.md)
 
Ejemplos de resolución de URIs
-----------------------

Se puede comprobar el funcionamiento del servidor mediante los siguientes ejemplos (actualizable):
- [http://graph.um.es/res/person/63ec870e-9908-49b9-9242-07a449bc562f](http://graph.um.es/res/person/63ec870e-9908-49b9-9242-07a449bc562f) Investigador con publicaciones e identificadores externos.
- [http://graph.um.es/res/article/a248813c-cfe9-4208-8009-87464d6cfade](http://graph.um.es/res/article/a248813c-cfe9-4208-8009-87464d6cfade) Artículo con DOI y otros identificadores obtenidos desde fuentes externas.
- [http://graph.um.es/res/project/5d35f1da-6eee-49e3-a350-54447ab24344](http://graph.um.es/res/project/5d35f1da-6eee-49e3-a350-54447ab24344) Proyecto de investigación.

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
		"NameTitle": "Hércules",
		"ConstrainedByUrl": "",
		"PropsTitle": "http://purl.org/roh#title|http://purl.org/roh/mirror/foaf#name",
		"PropsTransform": "http://purl.org/roh/mirror/vivo#researcherId|http://www.researcherid.com/rid/{value};http://purl.org/roh#ORCID|https://orcid.org/{value};http://purl.org/roh/mirror/vivo#scopusId|https://www.scopus.com/authid/detail.uri?authorId={value};http://purl.org/roh#researcherDBLP|https://dblp.org/pid/{value}.html;http://purl.org/roh#roDBLP|https://dblp.org/rec/{value}.html;http://purl.org/roh/mirror/bibo#doi|https://doi.org/{value};http://purl.org/roh#roPubmed|https://pubmed.ncbi.nlm.nih.gov/{value}/;",		
		"Authority": "http://localhost:56306/connect/token",
		"GrantType": "client_credentials",
		"Scope": "apiCarga",
		"ConfigUrl": "http://herc-as-front-desa.atica.um.es/carga/",
		"ClientId": "LinkedDataServer",
		"ClientSecret": "secretLinkedDataServer",
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
 - NameTitle: Nombre para mostrar en el título de la página tras el nombre de la entidad
 - ConstrainedByUrl: Url en la que se encuentran las restricciones ConstrainedBy
 - PropsTitle: Propiedades de la ontología para utilizar como título de las entidades en la presentación de la web. Generalmente http://purl.org/roh/mirror/foaf#name para las personas y http://purl.org/roh#title para el resto de entidades
 - PropsTransform: Propiedades cuya presentación en la web se transforma. Se visualizarán transformadas como enlaces externos según lo especificado en esta configuración. Por ejemplo: 'http://purl.org/roh#ORCID|https://orcid.org/{value}' indica que el valor de la propiedad 'http://purl.org/roh#ORCID' se visualizará como un hipervínculo en la web: https://orcid.org/{valor_de_la_propiedad}
 - Authority: Endpoint para la llamada de obtención del token
 - GrantType: Tipo de concesión de Oauth
 - Scope: Limitación de acceso al api de carga
 - ConfigUrl: Url donde se ejecuta la aplicación API Carga
 - ClientId: Id de cliente
 - ClientSecret: "clave" de acceso del cliente
 - UrlHome: Url con la que enlazar el logo de la cabecera 'Hércules'

  

## Dependencias

- **dotNetRDF**: versión 2.6.0
- **Marvin.Cache.Headers**: versión 5.0.1
- **Microsoft.AspNetCore.Http.Extensions**: versión 2.2.0
- **Serilog.AspNetCore**: versión 3.4.0
