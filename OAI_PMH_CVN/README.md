## Sobre OAI PMH CVN
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

Accesible en pruebas en esta dirección a través de swagger: http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/swagger/index.html.

OAI PMH CVN es un servicio web basado en OAI-PMH ([https://www.openarchives.org/OAI/openarchivesprotocol.html](https://www.openarchives.org/OAI/openarchivesprotocol.html)) que sirve los datos de los curículums de los investigadores de la Universidad de Murcia en formato RDF y dublin core.

Para ello, esté servicio hará uso de dos servicios externos:
 - API CVN UM: Api que proveerá de los currículums en formato XML CVN.
 - CVN: Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH.

En estos momentos aún no se hace uso de estos dos servicios:
 - El API CVN UM está pendiente de probar, de momento se trabaja con un mock que provee siempre los dos mismos cv.
 - El CVN: Su funcionamiento ahora mismo está integrado dentro del servicio OAI PMH CVN, peo se externalizará a este servicio.

