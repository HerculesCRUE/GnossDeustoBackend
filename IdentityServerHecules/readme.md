![](../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 01/10/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|versión inicial| 
|Descripción|Documentación del IdentityServer|
|Versión|0.1|
|Módulo|IdentityServerHercules|
|Tipo|Documentación|
|Cambios de la Versión|Creación|


# Sobre api IdentityServer

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/dashboard?id=IdentityServerHercules)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=IdentityServerHercules&metric=bugs)](https://sonarcloud.io/dashboard?id=IdentityServerHercules)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=IdentityServerHercules&metric=security_rating)](https://sonarcloud.io/dashboard?id=IdentityServerHercules)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=IdentityServerHercules&metric=ncloc)](https://sonarcloud.io/dashboard?id=IdentityServerHercules)

Este módulo es el encargado de la securización mediante tokens para los apis que forman el proyecto, este api valida los tokens de seguridad y proporciona nuevos tokens de acceso.

## Configuración en el appsettings.json
 >
    {
	"ConnectionStrings": {
	"PostgreConnection": ""
	},
	"Logging": {
    "LogLevel": {
    "Default": "Information",
    "Microsoft": "Warning",
    "Microsoft.Hosting.Lifetime": "Information"
    }
    },
    "AllowedHosts": "*",
	"IssuerUri": "http://herc-as-front-desa.atica.um.es:5108",
    }
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - PostgreConnection: Conexión con la base de datos
 - IssuerUri: nombre de la entidad que proporciona los tokens y que aparecerá en el documento de descubrimiento y los tokens JWT emitidos. Url donde está instalado el IdentityServer

## Dependencias

- **IndetityServer4**: versión 3.1.2
- **IdentityServer4.AccessTokenValidation**: versión 3.0.1
- **IdentityServer4.AspNetIdentity**: versión 3.1.2
- **IdentityServer4.EntityFramework.Storage**: versión 3.1.2
- **IdentityServer4.Storage**: versión 3.1.2
- **IdentityServer4.EntityFrameworkCore**: versión 3.1.3
- **IdentityServer4.EntityFrameworkCore.Tools**: versión 3.1.3
- **Npgsql.EntityFrameworkCore.PostgreSQL**: versión 3.1.3
