![](..//Docs/media/CabeceraDocumentosMD.png)

## Acerca de OAI PMH XML
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

Accesible en pruebas en esta dirección a través de swagger: http://herc-as-front-desa.atica.um.es/oai-pmh-xml/swagger/index.html.

OAI PMH XML es un servicio web basado en OAI-PMH ([https://www.openarchives.org/OAI/openarchivesprotocol.html](https://www.openarchives.org/OAI/openarchivesprotocol.html)) que sirve XML ubicados dentro del propio servicio. Este servicio funciona como un mock-up, devuleve todos los registros con la fecha actual e ignora los parámetros from, until y resumptionToken. EN lugar de ello cuando se le solicitan identificadores o records siempre deuelve el 50% de los que tiene de forma aleatoria.

## Configuración del repositorio de XML del servicio
Los XML que sirve el servicio se tienen que ubicar dentro de la carpeta XML, dentro de esta carpeta se crearán N carpetas que representarán los setspec y dentro de cada carpeta setspec se ubicarán los ficheros XML, el nombre de cada uno de ellos se utilizará como identificador seguido por el nombre de setspec.

## Configuración en el appsettings.json
    {
      "Logging": {
          "LogLevel": {
                "Default": "Information",
                "Microsoft": "Warning",
                "Microsoft.Hosting.Lifetime":"Information"
          }
      },
      "AllowedHosts": "*",
      "ConfigUrl": "http://herc-as-front-desa.atica.um.es/oai-pmh-xml/OAI_PMH"
    }
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - ConfigUrl: URL donde está lanzada esta aplicación 

## Dependencias

- **docfx.console**: versión 2.53.1
- **Microsoft.AspNetCore.Mvc.Formatters.Json**: versión 2.2.0
- **Microsoft.AspNetCore.Mvc.NewtonsoftJson**: versión 3.0.0
- **EntityFrameworkCore**: versión 3.1.1
- **OaiPmhNet**: versión 0.4.1
- **RestSharp**: versión 106.10.1
- **Swashbuckle.AspNetCore**: versión 5.0.0
- **Swashbuckle.AspNetCore.Annotations**: versión 5.0.0
- **System.ServiceModel.Duplex**: versión 4.4.*
- **System.ServiceModel.Http**: versión 4.4.*
- **System.ServiceModel.NetTcp**: versión 4.4.*
- **System.ServiceModel.Security**: versión 4.4.*
