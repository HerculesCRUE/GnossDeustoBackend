![](../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 01/10/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|versión inicial| 
|Descripción|Documentación del gestor de documentación|
|Versión|0.1|
|Módulo|GestorDocumentacion|
|Tipo|Documentación|
|Cambios de la Versión|Creación|


# Sobre api gestor de documentación

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/dashboard?id=GestorDocumentacion)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=GestorDocumentacion&metric=bugs)](https://sonarcloud.io/dashboard?id=GestorDocumentacion)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=GestorDocumentacion&metric=security_rating)](https://sonarcloud.io/dashboard?id=GestorDocumentacion)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=GestorDocumentacion&metric=ncloc)](https://sonarcloud.io/dashboard?id=GestorDocumentacion)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=GestorDocumentacion&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=GestorDocumentacion)

Este módulo se usa para cargar páginas HTML y servirlas a través de la web a modo de un gestor de contenidos, en el que los usuarios pueden subir páginas html con una ruta
determinada y luego acceder a esas páginas a través de la web (https://herc-as-front-desa.atica.um.es/carga-web), estas operaciones están disponibles en el controlador Page
  - Controlador Page: contiene 4 métodos para realizar las operaciones con las páginas
	 - *Get:* /Page: Que devuelve una página dada su ruta.
	 - *Get:* /Page/list: Que devuelve la lista de páginas sin el html.
	 - *Post:* /Page/loadpage: Carga una nueva página o modifica un página existente
	 - *Delete:* /Page/Delete: Elimina una página dando su identificador

 La documentación de la librería está disponible en:

http://herc-as-front-desa.atica.um.es/apiGestorDocumentacion/api/GestorDocumentacion.Controllers.html

Hay una versión disponible en el entorno de pruebas de la Universidad de Murcia, en esta dirección: 

http://herc-as-front-desa.atica.um.es/documentacion/swagger/index.html.

*Obtención del Token*
-------------------------
Este api esta protegida mediante tokens, por ello para poder usar la interfaz swagger hay que obtener un token, el cual se puede obtener desde https://herc-as-front-desa.atica.um.es/carga-web/Token

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
		"AllowedHosts": "*",
		"LogPath": "",
		"Authority": "http://localhost:56306",
		"Scope": "apiGestorDocumentacion",
    }
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - PostgreConnectionmigration: Conexión con la base de datos
 - LogPath: Ruta donde va a guardar los logs de la aplicación
 - Authority: Url donde está instalado el IdentityServer
 - Scope: Limitación de acceso al api de documentacion
 
 Se puede encontrar un el appsettings usado para este servicio sin datos sensibles en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/GestorDocumentacion/GestorDocumentacion/appsettings.json

## Dependencias

- **coverlet.collector**: versión 1.2.1
- **IndetityServer4**: versión 4.0.4
- **IdentityServer4.AccessTokenValidation**: versión 3.0.1
- **Microsoft.EntityFrameworkCore**: versión 3.1.7
- **Microsoft.EntityFrameworkCore.Tools**: versión 3.1.7
- **Microsoft.VisualStudio.Web.CodeGeneration.Design**: versión 3.1.4
- **Npgsql.EntityFrameworkCore.PostgreSQL**: versión 3.1.4
- **Serilog.AspNetCore**: versión 3.4.0
- **Swashbuckle.AspNetCore**: versión 4.0.4
- **Swashbuckle.AspNetCore.Annotations**: versión 5.5.1
- **Swashbuckle.AspNetCore.Filters**: versión 5.5.2
- **Swashbuckle.AspNetCore.Swagger**: versión 5.5.1
- **Swashbuckle.AspNetCore.SwaggerGen **: versión 5.5.1
- **Swashbuckle.AspNetCore.SwaggerUi **: versión 5.5.1
