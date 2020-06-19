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
### Ejecución Linux
    dotnet UrisFactory.dll
## Api Carga
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
         "QueryParam": "query"
         }
         }
 - PostgreConnectionmigration: Cadena de conexión a la base de datos PostgreSQL
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - Urls: Url en la que se va a lanzar la aplicación
 - ConfigUrl: URL donde está lanzada esta aplicación
 - Sparql.Graph: Grafo en el que se van a almacenar los triples
 - Sparql.Endpoint: URL del Endpoint Sparql
 - Sparql.QueryParam: Nombre del parámetro en el que hay que pasar la query al SPARQL Endpoint
### Ejecución Linux
    dotnet API_CARGA.dll
## FrontEndCarga
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
    "ConfigUrlCron": "http://herc-as-front-desa.atica.um.es/cron-config/"
    }
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - Urls: Url en la que se va a lanzar la aplicación
 - ConfigUrl: Url donde está lanzada la aplicación API Carga
 - ConfigUrlCron: Url donde está lanzada la aplicación CronConfigure
### Ejecución
    dotnet ApiCargaWebInterface.dll
   
## Api CronConfigure
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
        "ConfigUrl": "http://herc-as-front-desa.atica.um.es/carga/"
            }
 - HangfireConnection: Cadena de conexión a la base de datos PostgreSQL de tareas programadas
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - Urls: Url en la que se va a lanzar la aplicación
 - ConfigUrl: Url donde está lanzada la aplicación API Carga
### Ejecución Linux
    dotnet CronConfigure.dll
## Api OAIPMH
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
    "ConfigUrl": "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH"
          }
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - Urls: Url en la que se va a lanzar la aplicación
 - XML_CVN_Repository: URL del repositorio CVN
 - CVN_ROH_converter: URL del conversor de PDF a CVN
 - ConfigUrl: URL donde está lanzada esta aplicación
  ### Ejecución Linux
    dotnet OAI_PMH_CVN.dll
