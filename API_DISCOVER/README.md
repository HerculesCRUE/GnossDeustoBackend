![](..//Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 26/11/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|API DISCOVER readme| 
|Descripción|Manual del servicio API DISCOVER|
|Versión|0.2|
|Módulo|API DISCOVER|
|Tipo|Manual|
|Cambios de la Versión|Modificación appsettings.json|

## Sobre API DISCOVER
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

![](https://github.com/HerculesCRUE/GnossDeustoBackend/workflows/Build%20and%20test%20API_DISCOVER/badge.svg)
[![codecov](https://codecov.io/gh/HerculesCRUE/GnossDeustoBackend/branch/master/graph/badge.svg?token=4SONQMD1TI&flag=discover)](https://codecov.io/gh/HerculesCRUE/GnossDeustoBackend)

La documentación de la librería está disponible en: 
http://herc-as-front-desa.atica.um.es/api-discover/library/api/API_DISCOVER.Utility.Discover.html

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
 - properties.mandatory: Indica si el cumplimineto de esa propiedad es condición necesaria para considerar a dos entidades la misma
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
		"LogPath": "",
		"ScopusUrl": "https://api.elsevier.com/",
		"ScopusApiKey": "",
		"CrossrefUserAgent": "HerculesASIO-University-of-Murcia (https://github.com/HerculesCRUE/GnossDeustoBackend; mailto:<mail>) AsioBot",
		"WOSAuthorization": "Basic XXXXXXXXXXXXXXXXXXXX",
		"MaxScore": "0.9",
		"MinScore": "0.7",  
		"Authority": "",  
		"GrantType": "client_credentials",
		"Scope": "apiCarga",
		"ConfigUrl": "http://herc-as-front-desa.atica.um.es/carga/",
		"ScopeCron": "apiCron",
		"ConfigUrlCron": "http://herc-as-front-desa.atica.um.es/cron-config/",
		"ClientId": "Discover",
		"ClientSecret": "secretDiscover" 
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
 - LogPath: Ruta en la que escribir los logs
 - ScopusUrl: url del API de Scopus
 - ScopusApiKey: API key de Scopus
 - CrossrefUserAgent: User agent a utilizar con el API de crosref 
 - WOSAuthorization: Autorización apara el API de Web of Science
 - MaxScore: Score máximo para la desmbiguación
 - MinSocre: Score mínimo para la desmbiguación 
 - Authority: Url de la servicio de identidades
 - GrantType: Tipo de concesión de Oauth
 - Scope: Limitación de acceso al api Carga
 - ConfigUrl: Url donde está lanzada la aplicación API Carga
 - ScopeCron: Limitación de acceso al api CRON
 - ConfigUrlCron: URL donde está lanzada la aplicación CRON
 - ClientId: Id de cliente del api
 - ClientSecret: "clave" de acceso del api
  

# Comprobaciones y pruebas

Para comprobar el correcto funcionamiento del servicio API DISCOVER, se utilizarán un conjunto de 5 RDF's en formato xml que están disponibles en [este enlace](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/API_DISCOVER/xml_descubrimiento.zip).

Realizaremos las pruebas accediendo con el método /etl/data-publish, que podemos probar desde Swagger siguiendo estos pasos:

 - Pulsamos en el método /etl/data-publish
 - Seleccionamos Try it out
 - Hay que completar los siguientes campos
	 - jobID: indicamos el identificador de una tarea existente para que podamos desde la misma comprobar si se ha realizado correctamente el descubrimiento
	 - discoverProcessed: seleccionamos false
	 - rdfFile: seleccionaremos los RDF's de uno en uno para irlos cargando.
 - Al realizar el descubrimiento del primer RDF se realizara lo siguiente:
	 - Un investigador (Diego López de Ipiña) sube su CV con publicaciones y coautores (Diego Casado-Mansilla)
	 - Se enriquecen los identificadores de los elementos encontrados en las fuentes externas de información
	 - No se encuentra nada en el grafo porque es la primera entidad cargada

Comprobamos que se ha realizado correctamente el descubrimiento buscando tanto a Diego López de Ipiña como a Diego Casado-Mansilla, ambos aparecerán con su información correspondiente.

 - Al subir el segundo RDF:
	 - Un investigador (Esteban Sota) sube su CV con sus publicaciones y sus coautores (Diego Casado-Mansilla)
	 - En este caso las publicaciones del RDF son inventadas, así que no ayudan a la reconciliación, porque no se van a encontrar en las fuentes externas
	 - Pero Diego Casado-Mansilla tiene el ORCID en el RDF, por lo que reconoce la entidad cargada previamente con ese mismo identificador (se cargó enriquecida en el paso anterior). 
	 
Buscando a Diego Casado-Mansilla nos muestra correctamente la nueva entidad

 - Con el descubrimiento del tercer RDF:
	 - Una investigadora (Oihane Gómez-Carmona) sube su CV con sus publicaciones y sus coautores (Diego Casado-Mansilla)
	 - En este caso se enriquecen los identificadores de los elementos encontrados en las fuentes externas
	 - Reconoce a “Diego Casado” porque, aunque no estaba en el grafo ninguna de las publicaciones del RDF de Ohiane ni tiene ningún identificador (ORCID u otros), se ha encontrado en las fuentes externas que este autor más sus publicaciones coinciden con el autor ya cargado más sus publicaciones

Buscando a Diego Casado-Mansilla nos muestra correctamente la nueva entidad

- En el descubrimiento del cuarto RDF:
	 - Un investigador (Álvaro Palacios) sube su CV con sus publicaciones y sus coautores (Diego Casado-Mansilla)
	 - En este caso las publicaciones del RDF son inventadas y no están cargadas previamente en el grafo ni se encontrarán en las fuentes externas, por lo que habrá que desambiguar a Diego Casado-Mansilla manualmente porque no hay datos suficientes (en este caso se tratará del mismo Diego)
	 - Además, también se propondrá para la desambiguación manual un documento llamado ‘Documento de prueba con título inventado’, ya que en el segundo caso se ha cargado un documento con el mismo nombre que no tiene en común ningún autor (en este caso no se tratará del mismo documento)
 - Con el ultimo RDF:
	 - Un investigador (Diego Casado-Mansilla) sube su CV con todas sus publicaciones y sus coautores. Contiene las publicaciones y los coautores que se habían cargado en los pasos anteriores además de otras nuevas
	 - En este caso las publicaciones y autores que ya estaban cargados se reconocen y se desambiguan automáticamente y las publicaciones nuevas se cargan
	 - Además, cabe destacar que este caso 'Diego López de Ipiña’ se cita como 'D. López de Ipiña' y también es reconocido y actualiza las propiedades monoevaluada

Si revisamos a Diego López de Ipiña vemos que se ha modificado su nombre correctamente.

Si queremos eliminar los datos subidos en estas pruebas, hay que ejecutar las siguiente sentencias en el RDF Store:

    with <http://graph.um.es/graph/sgi>
    DELETE
     { ?s ?p ?o }
    WHERE
     { ?s ?p ?o .
     ?s <http://purl.org/roh/mirror/foaf#prueba> ?x
     }
Y posteriormente:

    with <http://graph.um.es/graph/sgi>
    DELETE { ?s ?p ?o. }
    WHERE 
    {
    	?s ?p ?o.
    	MINUS{?x ?y ?s. FILTER(isblank(?s))}
    	MINUS{?s ?p ?o. FILTER(!isblank(?s))}
    }

## Dependencias

- **dotNetRDF**: versión 2.5.1
- **Microsoft.EntityFrameworkCore**: versión 3.1.8
- **Microsoft.EntityFrameworkCore.SqlServer**: versión 3.1.8
- **Microsoft.EntityFrameworkCore.Tools**: versión 3.1.8
- **Newtonsoft.Json**: versión 12.0.3
- **Npgsql.EntityFrameworkCore.PostgreSQL**: versión 3.1.4
- **RabbitMQ.Client**: versión 6.2.1
- **Serilog.AspNetCore**: versión 3.4.0
