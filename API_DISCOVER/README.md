![](..//Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 01/10/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|API DISCOVER readme| 
|Descripción|Manual del servicio API DISCOVER|
|Versión|0.1|
|Módulo|API DISCOVER|
|Tipo|Manual|
|Cambios de la Versión|Creación|

## Sobre API DISCOVER
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

La documentación de la librería está disponible en: 
http://herc-as-front-desa.atica.um.es/api_discover/api/API_DISCOVER.html

API DISCOVER es un servicio encargado de aplicar el descubrimiento sobre los RDF de carga para su posterior publicación la BBDD.
 
Para una especificación más detallada acerca del funcionaminto del descubrimiento se puede consultar la siguiente documentación:
[Hércules Backend ASIO. Especificación de las funciones de descubrimiento](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/H%C3%A9rcules%20ASIO.%20Especificaci%C3%B3n%20de%20las%20funciones%20de%20descubrimiento.md)
 
*Conexión a Triple Store*
-------------------------

Como no es necesario ningún conector específico para actualizar un RDF Store ya que, por definición, deben tener un SPARQL Endpoint, no se ha creado ninguna librería específica de conexión al RDF Store. Las actualizaciones se realizan vía peticiones HTTP al SPARQL Endpoint.

El SPARQL Endpoint provisional se encuentra disponible en un servidor de la Universidad de Murcia, con acceso protegido por una VPN en la siguiente URL:

http://155.54.239.204:8890/sparql

Hay ejemplos de consultas en el documento [20200325 Hércules ASIO Ejemplos de consultas](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/SPARQL/20200325%20H%C3%A9rcules%20ASIO%20Ejemplos%20de%20consultas%20SPARQL.md)

Los datos cargados se pueden consultar en una versión preliminar del servidor Linked Data, soportado por [Trifid](https://github.com/zazuko/trifid), desplegado en los servidores de la Universidad de Murcia. Por ejemplo:

http://graph.um.es/res/project/RAYD-A-2002-6237

## Formato de configuración en el fichero reconciliationConfig.json
En este fichero se configuran, por cada una de las clases de la ontología, las propiedades a tener en cuenta para realizar el descubrimiento con los datos que ya están cargados en el Triple Store.

Este fichero se encuentra aqui: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/API_DISCOVER/API_DISCOVER/Config/reconciliationConfig.json.

A continuación se muestra un fragmento del fichero con la configuración del descubrimiento para las entidades del tipo 'http://purl.org/roh/mirror/foaf#Person'

    
	{
		"rdfType": "http://purl.org/roh/mirror/foaf#Person",
		"identifiers": [
			"http://purl.org/roh/mirror/vivo#identifier",
			"http://purl.org/roh/mirror/vivo#eRACommonsId",
			"http://purl.org/roh/mirror/vivo#researcherID",
			"http://purl.org/roh#ORCID",
			"http://purl.org/roh/mirror/vivo#scopusId",
			"http://purl.org/roh#taxID"
		],
		"properties": [
			{
				"property": "http://purl.org/roh/mirror/foaf#name",
				"mandatory": true,
				"inverse": false,
				"type": 2,
				"maxNumWordsTitle": null,
				"scorePositive": 0.89,
				"scoreNegative": null
			},
			{
				"property": "http://purl.org/roh#birthdate",
				"mandatory": false,
				"inverse": false,
				"type": 0,
				"maxNumWordsTitle": null,
				"scorePositive": 0.5,
				"scoreNegative": 0.05
			},
			{
				"property": "http://purl.org/roh#participates",
				"mandatory": false,
				"inverse": false,
				"type": 0,
				"maxNumWordsTitle": null,
				"scorePositive": 0.5,
				"scoreNegative": null
			},
			{
				"property": "http://purl.org/roh/mirror/foaf#homepage",
				"mandatory": false,
				"inverse": false,
				"type": 0,
				"maxNumWordsTitle": null,
				"scorePositive": 0.5,
				"scoreNegative": 0.025
			},
			{
				"property": "http://purl.org/roh#correspondingAuthorOf",
				"mandatory": false,
				"inverse": false,
				"type": 0,
				"maxNumWordsTitle": null,
				"scorePositive": 0.5,
				"scoreNegative": null
			},
			{
				"property": "http://purl.org/roh/mirror/bibo#authorList@@@?",
				"mandatory": false,
				"inverse": true,
				"type": 0,
				"maxNumWordsTitle": null,
				"scorePositive": 0.5,
				"scoreNegative": null
			},
			{
				"property": "http://purl.org/roh/mirror/vivo#relatedBy@@@http://purl.org/roh/mirror/vivo#relates",
				"mandatory": false,
				"inverse": true,
				"type": 0,
				"maxNumWordsTitle": null,
				"scorePositive": 0.5,
				"scoreNegative": null
			},
			{
				"property": "http://purl.org/roh/mirror/vivo#correspondingAuthorOf",
				"mandatory": false,
				"inverse": false,
				"type": 0,
				"maxNumWordsTitle": null,
				"scorePositive": 0.5,
				"scoreNegative": null
			}
		]
	}
	
	
 - rdfType: Clase a la que afecta la configuración de descubrimiento
 - identifiers: Propiedades que son identificadores de la entidad (si 2 entidades tienen el mismo identificador es la misma entidad, independientemente del resto de propiedades)
 - properties: Listado de propiedades junto con sus características para tener en cuenta en el descubrimiento
 - properties.property:  Propiedad a tener en cuanta en al reconciliación ('@@@' implica un 'salto' y '?' implica que puede ser cualquier propiedad)
 - properties.mandatory: Indica si el cumplimineto de esa propiedad es obligatorio para considerar a dos entidades la misma
 - properties.inverse: Si vale 'false' se buscan los valores de las propiedades utilizando la entidad como sujeto, si vale 'true' se buscan los valores de las propiedades utilizando la entidad como objeto
 - properties.type: Es el tipo de igualdad que se debe cumplir 
	- 0 (equals): Misma entidad o mismo valor de la propiedad
	- 1 (ignoreCaseSensitive): Mismo valor de la propiedad (ignorando mayúsculas y minúsculas)
	- 2 (name): Mismo nombre (para nombres de personas)
	- 3 (title): Mismo título (para títulos de documentos por ejemplo)
 - properties.maxNumWordsTitle: En el caso de que properties.type tenga como valor '3' implica el número de palabras que debe tener el título para considerar el máximo valor de igualdad
 - properties.scorePositive: Score positivo que se da a la relación cuando se da una coincidencia.
 - properties.scoreNegative: Score negativo que se da a la relación cuando no se da una coincidencia y ambas entidad tienen algún valor para esa propiedad.

## Configuración en el appsettings.json

    { 
		"ConnectionStrings": {
			"PostgreConnectionmigration": ""
		},
		"Logging": {
			"LogLevel": {
				"Default": "Information",
				"Microsoft": "Warning",
				"Microsoft.Hosting.Lifetime": "Information"
			}
		},
		"Sparql": {
			"Graph": "http://HerculesDemo.com",
			"Endpoint": "http://155.54.239.204:8890/sparql",
			"QueryParam": "query"
		},
		"RabbitMQ": {
			"usernameRabbitMq": "user",
			"passwordRabbitMq": "pass",
			"hostnameRabbitMq": "hercules",
			"uriRabbitMq": "amqp://user:pass@ip:puerto/hercules",
			"virtualhostRabbitMq": "hercules"
		},
		"RabbitQueueName": "HerculesDemoQueue",
		"Authority": "https://localhost:44354/connect/token",
		"GrantType": "client_credentials",
		"ScopeCron": "apiCron",
		"ClientId": "Web",
		"ClientSecret": "master",
		"ConfigUrlCron": "http://localhost:56255/",
		"ScopusUrl": "https://api.elsevier.com/",
		"ScopusApiKey": "",
		"CrossrefUserAgent": "Hercules/API discover (https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_DISCOVER; mailto:<correo>)"
	}
		 
		 
 - PostgreConnectionmigration: Cadena de conexión a la base de datos PostgreSQL
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - Sparql.Graph: Grafo en el que se van a almacenar los triples
 - Sparql.Endpoint: URL del Endpoint Sparql
 - Sparql.QueryParam: Parámetro para la query en el Endpoint Sparql
 - RabbitMQ.usernameRabbitMq: usuario para acceder a Rabbit
 - RabbitMQ.passwordRabbitMq: contraseña del usuario para acceder a Rabbit
 - RabbitMQ.hostnameRabbitMq: host de Rabbit
 - RabbitMQ.uriRabbitMq: cadena de conexión para acceder a Rabbit
 - RabbitMQ.virtualhostRabbitMq: host virtual configurado en Rabbit
 - RabbitQueueName: Nombre de la cola de Rabbit
 - Authority: Url de la servicio de identidades
 - GrantType: Tipo de concesión de Oauth
 - ScopeCron: Limitación de acceso al api CRON
 - ClientId: Id de cliente del api
 - ClientSecret: "clave" de acceso del api
 - ConfigUrlCron: URL donde está lanzada la aplicación CRON
 - ScopusUrl: url del API de Scopus
 - ScopusApiKey: API key de Scopus
 - CrossrefUserAgent: User agent a utilizar con el API de crosref 
 

## Dependencias

- **dotNetRDF**: versión 2.5.1
- **Microsoft.EntityFrameworkCore**: versión 3.1.8
- **Microsoft.EntityFrameworkCore.SqlServer**: versión 3.1.8
- **Microsoft.EntityFrameworkCore.Tools**: versión 3.1.8
- **Newtonsoft.Json**: versión 12.0.3
- **Npgsql.EntityFrameworkCore.PostgreSQL**: versión 3.1.4
- **RabbitMQ.Client**: versión 6.2.1
- **Serilog.AspNetCore**: versión 3.4.0
