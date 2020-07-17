![](../Docs/media/CabeceraDocumentosMD.png)

# Compilados de los entregables software
Esta carpeta contiene los compilados de los entregables de software desarrollados
en .Net Core y sus instrucciones de instalación y ejecución.

# Prerrequisitos de instalación para Windows y Linux
Instalar el entorno de .Net Core 3.1 Runtime. Las instrucciones y descargas están en: 
[https://docs.microsoft.com/es-es/dotnet/core/install/runtime?pivots=os-windows](https://docs.microsoft.com/es-es/dotnet/core/install/runtime?pivots=os-windows)

# Ejecución

En Windows basta con descargar y descomprimir cada uno de los compilados en 
diferentes carpetas de un dominio ya existente en el IIS (Internet Information Server).

En Linux, hay que descargar y descomprimir el compilado en una carpeta; y luego 
ejecutarlo el comando dotnet desde la propia carpeta. Por ejemplo:

    dotnet UrisFactory.dll


# Configuración de cada compilado 
## Api urisFactory

[apiuris.tar.gz](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Build/apiuris.tar.gz)

### Configuración
    {
      "Logging": {
          "LogLevel": {
                "Default": "Information",
                 "Microsoft": "Warning",
                  "Microsoft.Hosting.Lifetime": "Information"
                    }
              },
        "Authority": "http://localhost:56306",
         "Scope": "apiUrisFactory",
          "AllowedHosts": "*"
    }
### Ejecución Linux
    dotnet UrisFactory.dll
## Api Carga

[apicarga.tar.gz](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Build/apicarga.tar.gz)

### Configuración

    { 
    "ConnectionStrings": {
    "PostgreConnectionmigration": "Username=herculesdb;Password=NUuPIsrUV4x3o6sZEqE8;Host=155.54.239.203;Port=5432;Database=herculesdb;Pooling=true"},
      "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
      },
        "AllowedHosts": "*",
        "Urls": "http://0.0.0.0:5100",
         "ConfigUrl": "http://herc-as-front-desa.atica.um.es/carga/",
         "Sparql": {
           "Graph": "http://data.um.es/graph/um_cvn",
           "Endpoint": "http://155.54.239.204:8890/sparql"
           "QueryParam": "query",
		   "GraphRoh": "http://graph.um.es/graph/research/roh",
		   "GraphRohes": "http://graph.um.es/graph/research/rohes",
		   "GraphRohum": "http://graph.um.es/graph/research/rohum"
         },
         "Authority": "http://localhost:56306",
         "ScopeCarga": "apiCarga",
         "AuthorityGetToken": "http://localhost:56306/connect/token",
         "GrantType": "client_credentials",
         "ClientId": "carga",
         "ClientSecret": "secret",
         "ScopeOAIPMH": "apiOAIPMH",
         "ClientIdOAIPMH": "OAIPMH",
        "ClientSecretOAIPMH": "secretOAIPMH",
		"ConfigUrlUnidata": "https://localhost:44354/",
		"ScopeUnidata": "apiUnidata",
		"ClientIdUnidata": "unidata",
		"ClientSecretUnidata": "secretUnidata"
         }
 - PostgreConnectionmigration: Cadena de conexión a la base de datos PostgreSQL
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - Urls: Url en la que se va a lanzar la aplicación
 - ConfigUrl: URL donde está lanzada esta aplicación
 - Sparql.Graph: Grafo en el que se van a almacenar los triples
 - Sparql.Endpoint: URL del Endpoint Sparql
 - Authority: Url de la servicio de identidades
 - ScopeCarga: Limitación de acceso al api de carga
 - AuthorityGetToken: Endpoint para la llamada de obtención del token
 - GrantType: Tipo de concesión de Oauth
 - ClientId: Id de cliente del api de OAIPMH
 - ClientSecret: "clave" de acceso del api de carga
 - ScopeOAIPMH: Limitación de acceso al api de OAIPMH
 - ClientIdOAIPMH: Id de cliente del api de OAIPMH
 - ClientSecretOAIPMH: "clave" de acceso del api de OAIPMH
### Ejecución Linux
    dotnet API_CARGA.dll
## FrontEndCarga

[apifrontcarga.tar.gz](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Build/apifrontcarga.tar.gz)

### Configuración
 >
    {
    "Logging": {
    "LogLevel": {
    "Default": "Information",
    "Microsoft": "Warning",
    "Microsoft.Hosting.Lifetime": "Information"
    }
    },
    "AllowedHosts": "*",
    "Urls": "http://0.0.0.0:5103",
    "ConfigUrl": "http://herc-as-front-desa.atica.um.es/carga/",
    "ConfigUrlCron": "http://herc-as-front-desa.atica.um.es/cron-config/",
    "ConfigUrlUrisFactory": "http://herc-as-front-desa.atica.um.es/uris/",
    "Authority": "http://localhost:56306/connect/token",
    "GrantType": "client_credentials",
    "Scope": "apiCarga",
    "ScopeCron": "apiCron",
    "ScopeUrisFactory": "apiUrisFactory",
    "ClientId": "Web",
    "ClientSecret": "master"
    }
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - Urls: Url en la que se va a lanzar la aplicación
 - ConfigUrl: Url donde está lanzada la aplicación API Carga
 - ConfigUrlCron: Url donde está lanzada la aplicación CronConfigure
 - ConfigUrlUrisFactory: Url donde está lanzada la aplicación UrisFactory
 - Authority: Endpoint para la llamada de obtención del token
 - GrantType: Tipo de concesión de Oauth
 - Scope: Limitación de acceso al api de carga
 - ScopeCron: Limitación de acceso al api de cron
 - ScopeUrisFactory: Limitación de acceso al api de urisFactory
 - ClientId: Id de cliente, en este caso se ha configurado un cliente que pueda acceder a todas las apis que usa la web
 - ClientSecret: "clave" de acceso del cliente
### Ejecución
    dotnet ApiCargaWebInterface.dll
   
## Api CronConfigure

[apicron.tar.gz](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Build/apicron.tar.gz)

### Configuración
    {
      "ConnectionStrings": {
          "HangfireConnection":"Username=herculesdb;Password=NUuPIsrUV4x3o6sZEqE8;Host=155.54.239.203;Port=5432;Database=herculesdb;Pooling=true"
           },
             "Logging": {
                 "LogLevel": {
                       "Default": "Information",
                       "Microsoft": "Warning",
                       "Microsoft.Hosting.Lifetime":"Information"
    }
      },
        "AllowedHosts": "*",
        "Urls": "http://0.0.0.0:5107",
        "ConfigUrl": "http://herc-as-front-desa.atica.um.es/carga/",
		"Authority": "http://localhost:56306",
		"AuthorityGetToken": "http://localhost:56306/connect/token",
		"Scope": "apiCron",
		"ScopeCarga": "apiCarga",
		"GrantType": "client_credentials",
		"ClientId": "carga",
		"ClientSecret": "secret"
            }
 - HangfireConnection: Cadena de conexión a la base de datos PostgreSQL de tareas programadas
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - Urls: Url en la que se va a lanzar la aplicación
 - ConfigUrl: Url donde está lanzada la aplicación API Carga
 - Authority: Url de la servicio de identidades
 - AuthorityGetToken: Endpoint para la llamada de obtención del token
 - GrantType: Tipo de concesión de Oauth
 - Scope: Limitación de acceso al api de cron
 - ScopeCarga: Limitación de acceso al api de carga
 - ScopeUrisFactory: Limitación de acceso al api de urisFactory
 - ClientId: Id de cliente, en este caso se ha configurado un cliente que pueda acceder a todas las apis que usa la web
 - ClientSecret: "clave" de acceso del cliente
### Ejecución Linux
    dotnet CronConfigure.dll
## Api OAIPMH

[apioaipmh.tar.gz](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Build/apioaipmh.tar.gz)

### Configuración
    {
      "Logging": {
          "LogLevel": {
                "Default": "Information",
                "Microsoft": "Warning",
                "Microsoft.Hosting.Lifetime":"Information"
    }
          },
            "AllowedHosts": "*",
    "Urls": "http://0.0.0.0:5102",
    "XML_CVN_Repository":"http://curriculumpruebas.um.es/curriculum/rest/v1/auth/",
    "CVN_ROH_converter": "http://herc-as-front-desa.atica.um.es/cvn/v1/convert",
    "ConfigUrl": "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH",
	"Authority": "http://localhost:56306",
	"Scope": "apiOAIPMH"
          }
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - Urls: Url en la que se va a lanzar la aplicación
 - XML_CVN_Repository: URL del repositorio CVN
 - CVN_ROH_converter: URL del conversor de PDF a CVN
 - ConfigUrl: URL donde está lanzada esta aplicación
 - Authority: Url de la servicio de identidades
 - Scope: Limitación de acceso al api de apiOAIPMH
  ### Ejecución Linux
    dotnet OAI_PMH_CVN.dll
