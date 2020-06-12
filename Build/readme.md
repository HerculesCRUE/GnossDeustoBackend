# Configuraciones y ejecuciones 
## Api urisFactory
### Configuración
 >
    "ConnectionStrings": {
       "PostgreConnection": "Host=ip_del_servidor;Database=nombre_de_la_base_de_datos;Username=usuario;Password=contraseña",
       "PostgreConnectionmigration": "Host=ip_del_servidor;Database=nombre_de_la_base_de_datos;Username=usuario;Password=contraseña"
    }
  ### Ejecución
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

  ### Ejecución
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
   
   ## FrontEndCarga
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
  ### Ejecución
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
  ### Ejecución
    dotnet OAI_PMH_CVN.dll