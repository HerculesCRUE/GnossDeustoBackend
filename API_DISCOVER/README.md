![](..//Docs/media/CabeceraDocumentosMD.png)

## Sobre API DISCOVER
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

La documentación de la librería está disponible en: 
http://herc-as-front-desa.atica.um.es/TODO.html

API DISCOVER es un servicio encargado de aplicar el descubrimiento sobre los RDF de carga para su posterior publicación la BBDD.
 
Para una especificación más detallada del servicio se puede consultar la siguiente documentación: https://github.com/TODO
 
Esta aplicación se encarga de realizar el descubrimiento sobre los RDFs para su posterior carga./GnossDeustoBackend/tree/master/libraries).

*Conexión a Triple Store*
-------------------------

Como no es necesario ningún conector específico para actualizar un RDF Store ya que, por definición, deben tener un SPARQL Endpoint, no se ha creado ninguna librería específica de conexión al RDF Store. Las actualizaciones se realizan vía peticiones HTTP al SPARQL Endpoint.

El SPARQL Endpoint provisional se encuentra disponible en un servidor de la Universidad de Murcia, con acceso protegido por una VPN en la siguiente URL:

http://155.54.239.204:8890/sparql

Hay ejemplos de consultas en el documento [20200325 Hércules ASIO Ejemplos de consultas](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/SPARQL/20200325%20H%C3%A9rcules%20ASIO%20Ejemplos%20de%20consultas%20SPARQL.md)

Los datos cargados se pueden consultar en una versión preliminar del servidor Linked Data, soportado por [Trifid](https://github.com/zazuko/trifid), desplegado en los servidores de la Universidad de Murcia. Por ejemplo:

http://graph.um.es/res/project/RAYD-A-2002-6237

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
			"Graph": "",
			"Endpoint": "",
			"QueryParam": "query"
		},
		"RabbitMQ": {
			"usernameRabbitMq": "",
			"passwordRabbitMq": "",
			"hostnameRabbitMq": "",
			"uriRabbitMq": "",
			"virtualhostRabbitMq": ""
		},
		"RabbitQueueName": "",
		"Authority": "",
		"GrantType": "client_credentials",
		"ScopeCron": "apiCron",
		"ClientId": "Web",
		"ClientSecret": "master",
		"ConfigUrlCron": "http://localhost:56255/"
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

## Dependencias

- **dotNetRDF**: versión 2.5.1
- **Microsoft.EntityFrameworkCore**: versión 3.1.8
- **Microsoft.EntityFrameworkCore.SqlServer**: versión 3.1.8
- **Microsoft.EntityFrameworkCore.Tools**: versión 3.1.8
- **Newtonsoft.Json**: versión 12.0.3
- **Npgsql.EntityFrameworkCore.PostgreSQL**: versión 3.1.4
- **RabbitMQ.Client**: versión 6.2.1
- **Serilog.AspNetCore**: versión 3.4.0