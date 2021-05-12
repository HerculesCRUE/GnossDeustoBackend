![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 3/5/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|OAI PMH CVN| 
|Descripción|servicio web basado en OAI-PMH ([https://www.openarchives.org/OAI/openarchivesprotocol.html](https://www.openarchives.org/OAI/openarchivesprotocol.html)) que sirve los datos de los currículums de los investigadores de la Universidad de Murcia en formato RDF y dublin core|
|Versión|1.6|
|Módulo|Documentación|
|Tipo|Especificación|
|Cambios de la Versión|Cambios de enlaces|


## Acerca de OAI PMH CVN

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/dashboard?id=OAI_PMH_CVN)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=OAI_PMH_CVN&metric=bugs)](https://sonarcloud.io/dashboard?id=OAI_PMH_CVN)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=OAI_PMH_CVN&metric=security_rating)](https://sonarcloud.io/dashboard?id=OAI_PMH_CVN)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=OAI_PMH_CVN&metric=ncloc)](https://sonarcloud.io/dashboard?id=OAI_PMH_CVN)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=OAI_PMH_CVN&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=OAI_PMH_CVN)

[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

Accesible en pruebas en esta dirección a través de swagger: http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/swagger/index.html.

La documentación de la librería está disponible en: 
http://herc-as-front-desa.atica.um.es/oaipmh-cvn/library/api/OAI_PMH.Controllers.html

Las librerías compiladas se encuentran en la carpeta [librerías](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/libraries).

OAI PMH CVN es un servicio web basado en OAI-PMH ([https://www.openarchives.org/OAI/openarchivesprotocol.html](https://www.openarchives.org/OAI/openarchivesprotocol.html)) que sirve los datos de los currículums de los investigadores de la Universidad de Murcia en formato RDF y dublin core.

Para ello, esté servicio hará uso de dos servicios externos:
 - API CVN UM: Api que proveerá de los currículums en formato XML CVN.
 - CVN: Servidor HTTP que ofrece una API para convertir XML CVN a RDF ROH.

*Obtención del Token*
-------------------------
Este api esta protegida mediante tokens, por ello para poder usar la interfaz swagger hay que obtener un token, el cual se puede obtener desde https://herc-as-front-desa.atica.um.es/carga-web/Token

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
 
 Se puede encontrar un el appsettings usado para este servicio sin datos sensibles en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/OAI_PMH_CVN/OAI_PMH_CVN/appsettings.json

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
