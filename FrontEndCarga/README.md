![](../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 01/10/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Cambios en la documentación| 
|Descripción|insercción de configuraciones en el appsettings|
|Versión|0.3|
|Módulo|FrontEndCarga|
|Tipo|Documentación|
|Cambios de la Versión| Se ha añadido configuraciones al appsettings|


# Sobre FrontEnd de Carga
[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/dashboard?id=FrontEndCarga)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=FrontEndCarga&metric=bugs)](https://sonarcloud.io/dashboard?id=FrontEndCarga)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=FrontEndCarga&metric=security_rating)](https://sonarcloud.io/dashboard?id=FrontEndCarga)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=FrontEndCarga&metric=ncloc)](https://sonarcloud.io/dashboard?id=FrontEndCarga)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=FrontEndCarga&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=FrontEndCarga)

Este módulo constituye el interfaz Web de administración de las cargas de datos en la plataforma Hércules ASIO. Esta aplicación web realizada mediante el patrón MVC (módelo-vista-controlador) esta formado por diferentes controladores que se comunican con los diferentes apis creados en este proyecto. Estos controladores dividen su ámbito en la gestión de errores, de los repositorios, de los shapes, de las tareas, la gestión de la factoria de uris, etc.:
  - ErrorController: Gestiona los errores para devolver una página 404 o 500 según la excepción que se genera.
  - JobController: Controlador que gestiona las operaciones (listado, obtención de tareas, ejecución, ...) que se realizan en la web con cualquier tipo de tarea.
  - RepositoryConfigController: Controlador encargado de gestionar las operaciones con los repositorios. Creación, modificación, eliminación y listado.
  - ShapeConfigController: Encargado de gestionar las operaciones con los shapes. Creación, modificación, eliminación y listado.
  - UrisFactoryController: Este controlador es el encargado de gestionar todas las tareas que se pueden realizar con el api de UrisFactory, tanto la obtención de uris como las operaciones con el fichero de configuración de las uris.
  - CMSController: Controlador encargado de gestionar las páginas creadas por los usuarios.
  - TokenController: Encargado de obtener los tokens de acceso para los diferentes apis.
  - CheckSystemController: Se encarga de verificar que el api cron y el api carga funcionan correctamente.

Hay una versión disponible en el entorno de pruebas de la Universidad de Murcia, en esta dirección: 

https://herc-as-front-desa.atica.um.es/carga-web.


En esta carpeta está disponible el [Manual de Usuario del FrontEnd](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/FrontEndCarga/Manual%20de%20usuario.md).

## Configuración en el appsettings.json
 >
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
			"Endpoint": "http://localhost:8890/sparql",
			"QueryParam": "query"
		},
		"AllowedHosts": "*",
		"LogPathBase": "",
		"LogPath": "",
		"LogPathCarga": "",
		"LogPathCron": "",
		"Urls": "http://0.0.0.0:5103",
		"ConfigUrl": "http://herc-as-front-desa.atica.um.es/carga/",
		"ConfigUrlDocumentacion": "http://herc-as-front-desa.atica.um.es/documentacion/",
		"ConfigUrlCron": "http://herc-as-front-desa.atica.um.es/cron-config/",
		"ConfigUrlUrisFactory": "http://herc-as-front-desa.atica.um.es/uris/",
		"Authority": "http://localhost:56306/connect/token",
		"GrantType": "client_credentials",
		"Scope": "apiCarga",
		"ScopeCron": "apiCron",
		"ScopeUrisFactory": "apiUrisFactory",
		"ScopeDocumentacion": "apiGestorDocumentacion",
		"ScopeOAIPMH": "apiOAIPMH",
		"ClientId": "Web",
		"ClientIdOAIPMH": "OAIPMH",
		"ClientSecretOAIPMH": "secretOAIPMH",
		"ClientSecret": "master",
		"Proxy": "/carga-web",
		"UnidataDomain": "http://graph.unidata.es"
	}
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - PostgreConnectionmigration: Conexión con la base de datos
 - Sparql.Endpoint: URL del Endpoint Sparql
 - Sparql.QueryParam: Parámetro para la query en el Endpoint Sparql
 - LogPathBase: Ruta común para el path donde se almacenan los logs
 - LogPath: Nombre de la carpeta donde va a guardar los logs de la aplicación
 - LogPathCarga: Nombre de la carpeta donde escribe los logs el apiCarga
 - LogPathCron: Nombre de la carpeta donde escribe los logs el apiCron
 - Urls: Url en la que se va a lanzar la aplicación
 - ConfigUrlDocumentacion: Url donde está lanzada la aplicación de apiDocumentacion
 - ConfigUrl: Url donde está lanzada la aplicación API Carga
 - ConfigUrlCron: Url donde está lanzada la aplicación CronConfigure
 - ConfigUrlUrisFactory: Url donde está lanzada la aplicación UrisFactory
 - Authority: Endpoint para la llamada de obtención del token
 - GrantType: Tipo de concesión de Oauth
 - Scope: Limitación de acceso al api de carga
 - ScopeCron: Limitación de acceso al api de cron
 - ScopeDocumentacion: Limitación de acceso al api de documentación
 - ScopeOAIPMH: Limitación de acceso al api de OAIPMH
 - ScopeUrisFactory: Limitación de acceso al api de urisFactory
 - ClientId: Id de cliente, en este caso se ha configurado un cliente que pueda acceder a todas las apis que usa la web
 - ClientIdOAIPMH: Id de cliente de OAIPMH
 - ClientSecretOAIPMH: "clave" de acceso del cliente de OAIPMH
 - ClientSecret: "clave" de acceso del cliente
 - Proxy: directorio virtual que se ha configurado para el proxy inverso, en caso de que no se haya configurado dejar vacío.
 - UnidataDomain: Dominio del Linked Data server de Unidata
 
Se puede encontrar un el appsettings usado para este servicio sin datos sensibles en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/FrontEndCarga/ApiCargaWebInterface/appsettings.json

## Dependencias

- **AspNetCore.Security.CAS**: versión 2.0.5
- **CsvHelper**: versión 15.0.6
- **dotNetRDF**: versión 2.6.0
- **Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation**: versión 3.1.8
- **Microsoft.EntityFrameworkCore**: versión 3.1.8
- **Microsoft.NETCore.App**: versión 2.2.8
- **Microsoft.VisualStudio.Web.CodeGeneration.Design**: versión 3.1.4
- **NCrontab**: versión 3.3.1
- **Npgsql.EntityFrameworkCore.PostgreSQL**: versión 3.1.4
- **Serilog.AspNetCore**: versión 3.2.0
