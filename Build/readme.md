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
### Configuración
 >
    "ConnectionStrings": {
       "PostgreConnection": "Host=ip_del_servidor;Database=nombre_de_la_base_de_datos;Username=usuario;Password=contraseña",
       "PostgreConnectionmigration": "Host=ip_del_servidor;Database=nombre_de_la_base_de_datos;Username=usuario;Password=contraseña"
    }
### Ejecución Linux
    dotnet UrisFactory.dll
## Api Carga
### Configuración

    { 
    "ConnectionStrings": {
    "PostgreConnection": "Username=herculesdb;Password=NUuPIsrUV4x3o6sZEqE8;Host=155.54.239.203;Port=5432;Database=herculesdb;Pooling=true",
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
         "ConfigUrl": "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/",
         "Sparql": {
         "Graph": "http://data.um.es/graph/um_cvn",
         "Endpoint": "http://155.54.239.204:8890/sparql"
         "QueryParam": "query"
         }
         }

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
  ### Ejecución Linux
    dotnet OAI_PMH_CVN.dll
