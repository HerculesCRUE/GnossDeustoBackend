![](media/CabeceraDocumentosMD.png)

| Fecha         | 3/5/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Documento de alojamiento| 
|Descripción|Documento genérico para el despliegue de la arquitectura física|
|Versión|1|
|Módulo|Documentación|
|Tipo|Manual|
|Cambios de la Versión|Creación del documento|

## Hércules ASIO. Documento de alojamiento


1 INTRODUCCIÓN
===============

Este documento describe una solución de alojamiento genérica para el sistema Hércules ASIO dentro de la infraestructura de una universidad.
Se contemplan dentro del mismo aspectos de diseño del sistema (arquitectura software y hardware), particularidades
para su explotación y monitorización, así como aspectos de seguridad y capacidad.

Este documento va dirigido al personal técnico encargado del mantenimiento de la Hércules ASIO en la universidad.


2 Descripción de la solución
========================

El diagrama de la solución, a falta de definir los nombres reales de los servidores, sería el siguiente:
![](media/Hércules%20ASIO%20Arquitectura%20física%20genérica.png)

En el diagrama se incluyen ejemplos de servicios y URLs a las que contestarían algunos de los componentes. 

2.1 INTRODUCCIÓN AL PRODUCTO
-------------------

Se trata de la implantación de la Arquitectura Semántica de Hércules ASIO Backend SGI (ASIO), que cuenta 
con procesos de incorporación de datos del ámbito de la gestión de la investigación desde sistemas externos, 
como el futuro Hércules SGI, en un almacenamiento RDF Store; y con unos interfaces web que presentan el 
proyecto y permiten consultar el grafo de conocimiento almacenado en formato de triples en el RDF Store.

2.2	ARQUITECTURA DE LA SOLUCIÓN SOFTWARE
---------------------------------

La Arquitectura Semántica de ASIO cuenta con los siguientes componentes lógicos:

-	Frontal Web público, que incluye la web de presentación del proyecto, el servidor Linked Data, datos FAIR, Benchmark RDF Store y acceso a un punto SPARQL.
-	Frontal Web privado, que incluye las funciones de administración.
-	Base de datos relacional con PostgreSQL para el almacenamiento y gestión de la información de administración de la plataforma.
-	RDF Store con Openlink Virtuoso, para el almacenamiento, gestión y consulta de los datos del grafo de conocimiento.
-	Gestión de colas de eventos con RabbitMQ.
-	Frontal Web que realiza la función de Proxy inverso hacia las aplicaciones Web, redirigiendo a los frontales públicos o privados en función de la URL.
-	Balanceo de carga con HAProxy (o similar).

2.3	DEPENDENCIAS CON OTRAS APLICACIONES O SISTEMAS
-------------------------------------

La Arquitectura Semántica de ASIO no depende de otras aplicaciones para su funcionamiento, más allá de
las fuentes de información externas que se configuren en su momento para la incorporación de datos de
gestión de la investigación.

3 Despliegue de la solución
======================

3.1 Despliegue de Software
---------------------

Se indican los nombres sugeridos para los servidores, que tendrán que sustituirse por los 
definitivos, y el software más relevante de cada servidor.

|Servidor|Componente|SO|Software|
|:----|:----|:----|:----|
|Proxy|Proxy Inverso|Linux Centos 8|Apache|
|Web Pública 1|Web Pública |Linux Centos 8|Docker (.Net Core 3.1)|
|Web Pública 2|Web Pública |Linux Centos 8|Docker (.Net Core 3.1)|
|Web privada |Web privada|Linux Centos 8|Docker (Net Core 3.1)|
|HAPorxy|HAProxy|Linux Centos 8|HAProxy|
|PostgreSQL|PostgreSQL|Linux Centos|Docker (PostgreSQL)|
|RDF Store 1|RDF Store|Linux Centos 8|P.e. Virtuoso  07.20.3230|
|RDF Store 2|RDF Store|Linux Centos 8|PP.e. Virtuoso 07.20.3230|
|RabbitMQ|Gestión de colas|Linux Centos 8|Docker (RabbitMQ)|

El inventario de aplicaciones es:

|Nombre del dominio|Nombre de la aplicación|Ruta de la aplicación|Rol de la Aplicación|
|:----|:----|:----|:----|
|linkeddata.domain|apicarga|/carga|Procesos ETL; gestión de repositorios; gestor de sincronizaciones; procesos de validación|
|linkeddata.domain|apicron|/cron-config|Gestión de la programación de tareas|
|linkeddata.domain|apiuris|/uris|Generación de URIs según el Esquema de URIs Hércules|
|linkeddata.domain|apiidentity|/identityserver|Securización mediante tokens|
|linkeddata.domain|apigesdoc|/documentacion|Servicio web para la creación de páginas de contenido html y su posterior visualización.|
|linkeddata.domain|apifrontcarga|/carga-web|Front-end privado|
|linkeddata.domain|xmlrdfconversor|/conversor_xml_rdf|Conversor de XML a RDF|
|linkeddata.domain|apicvn|/cvn|Conversión de CVN a RDF|
|linkeddata.domain|apioaipmh|/oai-pmh-cvn|Sirve los datos de los curículums de los investigadores de la universidad en formato RDF y dublin core|
|linkeddata.domain|apioaipmhxml|/oai-pmh-xml|Versión preliminar del servicio OAI-PMH de Hércules SGI, simula el servicio de datos|
|linkeddata.domain|FAIR|/|Librería y frontal de FAIR Metrics|


El inventario de aplicaciones públicas es:

|Dominio|Nombre|Ruta de la aplicación|Rol de la Aplicación|
|:----|:----|:----|:----|
|linkeddata2.um.es|apifrontcarga|/carga-web|Front-end público|
|linkeddata2.um.es|linkeddataserver|/graph/sgi|Proporciona el servicio de datos enlazados de Hércules ASIO.|

El inventario de aplicaciones back es:

|Nombre|Rol de la Aplicación|
|:----|:----|
|apidiscover|Aplicación que ofrece las siguientes funciones del proceso de carga:|
| |- Reconciliación de entidades, que evita la duplicación de entidades.|
| |- Descubrimiento de enlaces, que genera enlaces hacia datasets externos y ofrece información de ayuda en la reconciliación de entidades.|
| |- Detección de equivalencias, entre nodos Unidata.|


3.2	ENTORNO DE PRODUCCIÓN
---------------------

3.2.1	Configuración de los servicios web de Apache
-------------------------------------

Para que nos funcione correctamente el Linked data server podemos ver este ejemplo de configuración. 
Lo que hacemos con esto es que las peticiones que llegan por http al dominio raiz las redirija al linked data server.

    <VirtualHost *:80>
    ServerName linkeddata.domain
    DocumentRoot "/var/www/html"
    ProxyPreserveHost On
    ProxyPass / http://127.0.0.1:8081/
    ProxyPassReverse / http://127.0.0.1:8081	
     Timeout 5400
    ProxyTimeout 5400
        <Proxy *>
            Order deny,allow
            Allow from all
            Require all granted
        </Proxy>
    </VirtualHost>

El resto de peticiones las podríamos gestionar por SSL añadiendo estas líneas al archivo ssl.conf. Podemos ver las redirecciones de proxy a diversos servicios y interfaces sparql.

    #APIFRONTCARGA
    ProxyPass /carga-web http://127.0.0.1:5103
    ProxyPassReverse /carga-web http://127.0.0.1:5103
    #APICARGA
    ProxyPass /carga http://155.54.239.219:5100
    ProxyPassReverse /carga http://155.54.239.219:5100
    #BENCHMARK
    ProxyPass /benchmark http://127.0.0.1:8401
    ProxyPassReverse /benchmark http://127.0.0.1:8401
    #OAI-PMH-CVN
    ProxyPass /oai-pmh-cvn http://155.54.239.219:5102
    ProxyPassReverse /oai-pmh-cvn http://155.54.239.219:5102
    #CRON
    ProxyPass /cron-config http://155.54.239.219:5107
    ProxyPassReverse /cron-config http://155.54.239.219:5107
    #DOCUMENTACION
    ProxyPass /documentacion http://155.54.239.219:5109
    ProxyPassReverse /documentacion http://155.54.239.219:5109
    #IDENTITY-SERVER
    ProxyPass /identityserver http://155.54.239.219:5108
    ProxyPassReverse /identityserver http://155.54.239.219:5108
    #APIURIS
    ProxyPass /uris http://155.54.239.219:5000
    ProxyPassReverse /uris http://155.54.239.219:5000
    #XMLRDFCONVERSOR
    ProxyPass /conversor_xml_rdf http://155.54.239.219:5114
    ProxyPassReverse /conversor_xml_rdf http://155.54.239.219:5114
    #UNIDATA
    ProxyPass /unidata http://155.54.239.219:5106
    ProxyPassReverse /unidata http://155.54.239.219:5106
    #CVN
    ProxyPass /cvn http://127.0.0.1:5104
    ProxyPassReverse /cvn http://127.0.0.1:5104
    ProxyPass /cvn_swagger http://127.0.0.1:8080
    ProxyPassReverse /cvn_swagger http://127.0.0.1:8080  
    #BRIDGE
    ProxyPass /fairmetrics_bridge http://155.54.239.219:5200
    ProxyPassReverse /fairmetrics_bridge http://155.54.239.219:5200
    ProxyPass /bridgeswagger http://155.54.239.219:8082
    ProxyPassReverse /bridgeswagger http://155.54.239.219:8082
    #VIRTUOSO1
    ProxyPass /sparql http://155.54.239.221:8890/sparql
    ProxyPassReverse /sparql http://155.54.239.221:8890/sparql
    #VIRTUOSO2
    ProxyPass /sparql2 http://155.54.239.222:8890/sparql
    ProxyPassReverse /sparql2 http://155.54.239.222:8890/sparql


