![](..//Docs/media/CabeceraDocumentosMD.png)

## Acerca de OAI PMH CVN
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

Accesible en pruebas en esta dirección a través de swagger: http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/swagger/index.html.

La documentación de la librería está disponible en: 
http://herc-as-front-desa.atica.um.es/oaipmh-cvn/library/api/OAI_PMH.Controllers.html

Las librerías compiladas se encuentran en la carpeta [librerías](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/libraries).

OAI PMH CVN es un servicio web basado en OAI-PMH ([https://www.openarchives.org/OAI/openarchivesprotocol.html](https://www.openarchives.org/OAI/openarchivesprotocol.html)) que sirve los datos de los curículums de los investigadores de la Universidad de Murcia en formato RDF y dublin core.

Para ello, esté servicio hará uso de dos servicios externos:
 - API CVN UM: Api que proveerá de los currículums en formato XML CVN.
 - CVN: Servidor HTTP que ofrece una API para convertir XML CVN a RDF ROH.

*Obtención del Token*
-------------------------
Este api esta protegida mediante tokens, por ello para poder usar la interfaz swagger hay que obtener un token, el cual se puede obtener desde https://herc-as-front-desa.atica.um.es/carga-web/Token

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
