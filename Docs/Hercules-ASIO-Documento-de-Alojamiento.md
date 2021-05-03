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

[1 INTRODUCCIÓN](#1-introducción)

[2 DESCRIPCIÓN DE LA SOLUCIÓN](#2-descripción-de-la-solución)

[2.1 INTRODUCCIÓN AL PRODUCTO](#21-introducción-al-producto)

[2.2 ARQUITECTURA DE LA SOLUCIÓN SOFTWARE](#22-arquitectura-de-la-solución-software)

[2.3 DEPENDENCIAS CON OTRAS APLICACIONES O SISTEMAS](#23-dependencias-con-otras-aplicaciones-o-sistemas)

[3 DESPLIEGUE DE LA SOLUCIÓN](#3-despliegue-de-la-solución)

[3.1 DESPLIEGUE DE SOFTWARE](#31-despliegue-de-software)

[3.2 ENTORNO DE PRODUCCIÓN](#32-entorno-de-producción)

[3.2.1 Configuración de los servicios web de Apache](#321-configuración-de-los-servicios-web-de-apache)

[3.2.2 Inventario hardware](#322-inventario-hardware)

[3.2.3 Reglas de servicio](#323-reglas-de-servicio)

[3.2.4 Elementos en alta disponibilidad](#324-elementos-en-alta-disponibilidad)

[4 NECESIDADES ESPECIALES DE EXPLOTACIÓN](#4-necesidades-especiales-de-explotación)

[4.1 BACKUPS Y RECUPERACIÓN](#41-backups-y-recuperación)

[4.2 MONITORIZACIÓN](#42-monitorización)

[4.2.1 Indicadores y umbrales de los servidores](#421-indicadores-y-umbrales-de-los-servidores)

[4.2.2 Indicadores y umbrales de los servicios Web](#422-indicadores-y-umbrales-de-los-servicios-web)

[4.2.3 Información específica de disponibilidad](#423-información-específica-de-disponibilidad)

[4.3 POSIBLES PROBLEMAS Y SOLUCIONES](#43-posibles-problemas-y-soluciones)



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

2.2 ARQUITECTURA DE LA SOLUCIÓN SOFTWARE
---------------------------------

La Arquitectura Semántica de ASIO cuenta con los siguientes componentes lógicos:

-	Frontal Web público, que incluye la web de presentación del proyecto, el servidor Linked Data, datos FAIR, Benchmark RDF Store y acceso a un punto SPARQL.
-	Frontal Web privado, que incluye las funciones de administración.
-	Base de datos relacional con PostgreSQL para el almacenamiento y gestión de la información de administración de la plataforma.
-	RDF Store con Openlink Virtuoso, para el almacenamiento, gestión y consulta de los datos del grafo de conocimiento.
-	Gestión de colas de eventos con RabbitMQ.
-	Frontal Web que realiza la función de Proxy inverso hacia las aplicaciones Web, redirigiendo a los frontales públicos o privados en función de la URL.
-	Balanceo de carga con HAProxy (o similar).

2.3 DEPENDENCIAS CON OTRAS APLICACIONES O SISTEMAS
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
|linkeddata.domain|apifrontcarga|/carga-web|Front-end público|
|linkeddata.domain|linkeddataserver|/graph/sgi|Proporciona el servicio de datos enlazados de Hércules ASIO.|

El inventario de aplicaciones back es:

|Nombre|Rol de la Aplicación|
|:----|:----|
|apidiscover|Aplicación que ofrece las siguientes funciones del proceso de carga:|
| |- Reconciliación de entidades, que evita la duplicación de entidades.|
| |- Descubrimiento de enlaces, que genera enlaces hacia datasets externos y ofrece información de ayuda en la reconciliación de entidades.|
| |- Detección de equivalencias, entre nodos Unidata.|


3.2 ENTORNO DE PRODUCCIÓN
---------------------

3.2.1 Configuración de los servicios web de Apache
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

El resto de peticiones las podríamos gestionar por SSL añadiendo estas líneas al archivo ssl.conf. Podemos ver las redirecciones de proxy a diversos servicios y interfaces sparql (con IPs locales supuestas en 10.0.0.x):

    #APIFRONTCARGA
    ProxyPass /carga-web http://127.0.0.1:5103
    ProxyPassReverse /carga-web http://127.0.0.1:5103
    #APICARGA
    ProxyPass /carga http://10.0.0.219:5100
    ProxyPassReverse /carga http://10.0.0.219:5100
    #BENCHMARK
    ProxyPass /benchmark http://127.0.0.1:8401
    ProxyPassReverse /benchmark http://127.0.0.1:8401
    #OAI-PMH-CVN
    ProxyPass /oai-pmh-cvn http://10.0.0.219:5102
    ProxyPassReverse /oai-pmh-cvn http://10.0.0.219:5102
    #CRON
    ProxyPass /cron-config http://10.0.0.219:5107
    ProxyPassReverse /cron-config http://10.0.0.219:5107
    #DOCUMENTACION
    ProxyPass /documentacion http://10.0.0.219:5109
    ProxyPassReverse /documentacion http://10.0.0.219:5109
    #IDENTITY-SERVER
    ProxyPass /identityserver http://10.0.0.219:5108
    ProxyPassReverse /identityserver http://10.0.0.219:5108
    #APIURIS
    ProxyPass /uris http://10.0.0.219:5000
    ProxyPassReverse /uris http://10.0.0.219:5000
    #XMLRDFCONVERSOR
    ProxyPass /conversor_xml_rdf http://10.0.0.219:5114
    ProxyPassReverse /conversor_xml_rdf http://10.0.0.219:5114
    #UNIDATA
    ProxyPass /unidata http://10.0.0.219:5106
    ProxyPassReverse /unidata http://10.0.0.219:5106
    #CVN
    ProxyPass /cvn http://127.0.0.1:5104
    ProxyPassReverse /cvn http://127.0.0.1:5104
    ProxyPass /cvn_swagger http://127.0.0.1:8080
    ProxyPassReverse /cvn_swagger http://127.0.0.1:8080  
    #BRIDGE
    ProxyPass /fairmetrics_bridge http://10.0.0.219:5200
    ProxyPassReverse /fairmetrics_bridge http://10.0.0.219:5200
    ProxyPass /bridgeswagger http://10.0.0.219:8082
    ProxyPassReverse /bridgeswagger http://10.0.0.219:8082
    #VIRTUOSO1
    ProxyPass /sparql http://10.0.0.221:8890/sparql
    ProxyPassReverse /sparql http://10.0.0.221:8890/sparql
    #VIRTUOSO2
    ProxyPass /sparql2 http://10.0.0.222:8890/sparql
    ProxyPassReverse /sparql2 http://10.0.0.222:8890/sparql

3.2.2 Inventario hardware
-------------------------

Los servidores tendrían las siguientes características físicas y software instalado:

|Nombre del Servidor|Función|Características|
|:----|:----|:----|
|Proxy Inverso|Proxy Inverso|Centos 8  2 Cores, RAM 4 GB, 50GB. Apache|
|HAProxy|Balanceo Interno|Centos 8  2 Cores, RAM 2 GB, 50GB. HAProxy|
|Web pública 1|Instanciar webs públicas|Centos 8  4 Cores, RAM 4 GB, 50GB. Docker version 19.03.5 + docker-compose|
|Web pública 2|Instanciar webs públicas|Centos 8  4 Cores, RAM 4 GB, 50GB. Docker version 19.03.5 + docker-compose|
|Web privada|Instanciar servicios de back y web privada|Centos 8  4 Cores, RAM 8 GB, 50GB. Docker version 19.03.5 + docker-compose|
|RDF Store 1|RDF Store|Centos 8  4 Cores, RAM 9 GB, 50GB. P.e. Virtuoso  07.20.3230|
|RDF Store 2|RDF Store|Centos 8  4 Cores, RAM 9 GB, 50GB. P.e. Virtuoso  07.20.3230|
|RabbitMQ|Gestor de colas|Centos 8  2 Cores, RAM 2 GB, 50Gb. Docker version 19.03.5 + docker-compose|
|PostgreSQL|Base de datos SQL|Centos 8  2 Cores, RAM 5 GB, 50Gb. Docker version 19.03.5 + docker-compose|

3.2.3 Reglas de servicio
---------------------

Tendríamos las siguientes reglas de red:

|Elemento A Servidor|Módulo que precisa comunicación|Elemento B servidor|Módulo que precisa comunicación|Dirección de la Comunicación|Tipo de Tráfico|Puerto|
|:----|:----|:----|:----|:----|:----|:----|
|Proxy|Proxy |Web privada|Web privada|A->B|TCP|80|
|Proxy|Proxy|HAProxy|HAProxy|A->B|TCP|80|
|HAProxy|HAProxy|Web pública 1 y 2|Web pública|A->B|TCP|80|
|HAProxy|HAProxy|RDF Store 1 y 2|Virtuoso|A->B|TCP|8890|
|Web pública 1 y 2|Web pública|Web privada|Web privada|A->B|TCP|5108, 5100, 5107, 5000, 5109|
|Web pública 1 y 2|Web pública|PostgreSQL|PostgreSQL|A->B|TCP|5432|
|Web privada|Web privada|RabbitMQ|RabbitMQ|A->B|TCP|5672|
|Web privada|Web privada|PostgreSQL|PostgreSQL|A->B|TCP|5432|
|Web privada|Web privada|RDF Store 1 y 2|P.e. Virtuoso|A->B|TCP|8890|

3.2.4 Elementos en alta disponibilidad
-------------------

Se dispone de la siguiente arquitectura para proporcionar alta disponibilidad en los procesos de lectura de datos:

![](media/Hércules%20ASIO%20Arquitectura%20alta%20disponibilidad.png)

Los componentes son:

- Proxy. Se trata de un proxy inverso con balanceo que redirige las peticiones hacia 2 servidores que alojan los Docker que proporcionan los servicios Web. Cada frontal web público devuelve una cabecera que informa de cuál está contestando
- HAProxy. Se trata de un balanceo interno que recibe las peticiones de lectura de datos hacia los servidores RDF Store y las balancea hacia los dos servidores (con Virtuoso) que cuentan con la misma información y pueden contestar indistintamente. Este componente detecta si uno de los servidores está caído temporalmente (por ejemplo, en el proceso de checkpoint de Virtuoso) y redirige el tráfico hacia el otro servidor. Cada RDF Store devuelve una cabecera que informa de qué servidor está contestando.

Con la arquitectura anterior se podrían añadir más servidores frontales Web y también más RDF Store de lectura. Para disponer 
de un balanceo de lectura con 2 o más RDF Store es necesario disponer de un proceso de escritura que se encargue
de que los datos de los servidores tengan la misma información. El siguiente diagrama explica este proceso:
 
![](media/Hércules%20ASIO%20Proceso%20alta%20disponibilidad.png)

El proceso de escritura en alta disponibilidad tiene los siguientes pasos:

- Actualiza los datos directamente en el RDF Store establecido como máster.
- Al mismo tiempo se añade un evento en la cola de replicación, con las instrucciones para realizar la actualización de datos.
- El mismo proceso de escritura cuenta con un hilo que se encarga leer los eventos de la cola y los procesa.
- Cada evento procesado genera una actualización de datos en el RDF Store establecido como réplica. 

4 NECESIDADES ESPECIALES DE EXPLOTACIÓN
=======================
 
4.1 BACKUPS Y RECUPERACIÓN
-----------------

Tendrían que establecerse los siguientes Backups, ajustados al procedimiento de sistemas en vigor:
- Backup estándar de servidores: 
- Backup estándar BBDD PostgreSQL: 
- BBDD RDF Store: 

Y definirse el escenario de restauración, que debería incluir los pasos que los responsables de sistemas determinen, que podrían ser:
- Si fuera necesario, reinstalación del sistema operativo y software base.
- Recuperación de la última copia de seguridad del grafo.
- Recuperación de la última copia de seguridad de la BBDD de PostgreSQL con la definición de procesos de carga.
- Si fuera necesario, reinstalación de los servicios Web y Aplicaciones.

4.2 MONITORIZACIÓN
---------------

Se describen a continuación los parámetros a monitorizar en los servidores, y las webs cuyo servicio habrá que comprobar.

4.2.1 Indicadores y umbrales de los servidores
------------------------

Las indicadores y umbrales a tener en cuenta podrían ser:

|Servidor|Indicador a Monitorizar|Descripción breve|Umbral|
|:----|:----|:----|:----|
|Frontales Web|Uso de RAM| |Warning: 95% 5 últimos minutos; Critical: 99% 5 últimos minutos|
| |Uso de CPU| |Warning: 95% 5 últimos minutos; Critical: 99% 5 últimos minutos|
| |Disco| |Warning: 80%; Critical: 90%|
| |Proceso de docker|Estado proceso docker se encuentre en ejecución| |
|PostgreSQL|Uso de RAM| |Warning: 95% 5 últimos minutos; Critical: 99% 5 últimos minutos|
| |Uso de CPU| |Warning: 95% 5 últimos minutos; Critical: 99% 5 últimos minutos|
| |Disco| |Warning: 80%; Critical: 90%|
| |Proceso de postgresql|Estado proceso postgresql se encuentre en ejecución| |
|RDF Store|P.e. virtuoso-t|Estado proceso virtuoso-t running| |
| |Chequeo HTTP Puerto 8890|Servicio Web sparql endpoint; P.e. en Nagios: check_http -I $HOSTADDRESS$  -p 8890 -j HEAD -w 1 -c 2 -t 120)| |
| |Estado de los buffers|Controlar el estado de los buffers con la función sys_stat('st_db_used_buffers')|Warning: 80%; Critical: 90%|
| | |Controlar lecturas de disco con la función sys_stat('disk_reads')|Warning: 80%; Critical: 90%|

4.2.2 Indicadores y umbrales de los servicios Web
-----------------------

Se monitorizará el estado de los servicios Web de ASIO controlando la respuesta a las siguientes peticiones, que se realizarán con un agente Web que se comporte como un BOT:

|Indicador a Monitorizar|Descripción breve|Umbral Crítical|
|:----|:----|:----|
|Web|https://linkeddata.domain/benchmark|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |https://linkeddata.domain/carga/swagger/index.html|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |https://linkeddata.domain/oai-pmh-cvn/swagger/index.html|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |https://linkeddata.domain/cron-config/swagger/index.html|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |https://linkeddata.domain/documentacion/swagger/index.html|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |http://linkeddata.domain/identity/connect/token |<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |http://linkeddata.domain/graph/sgi|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |https://linkeddata.domain/uris/swagger/index.html|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |https://linkeddata.domain/uris/swagger/index.html|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |https://linkeddata.domain/carga-web/public/gnossdeustobackend/home|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |https://linkeddata.domain/conversor_xml_rdf/swagger/index.html|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |https://linkeddata.domain/unidata/swagger/index.html|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|
| |https://linkeddata.domain/bridgeswagger/v1/ui/#/|<> HTTP STATUS 200; Warning 2 seg.; Critical 4 seg|

4.2.3 Información específica de disponibilidad
--------------------

El sistema estará disponible si se puede:
- Actualizar los datos del grafo desde el proceso de sincronización.
- Administrar la plataforma mediante las utilidades de administración.
- Se pueden consultar los datos del grafo mediante el servidor Linked Data y el punto SPARQL.
- Se pueden consultar las páginas de la web pública.

El sistema se podría considerar parcialmente disponible si alguna de las funciones anteriores estuviera fallando. Por ejemplo, se podría soportar que la actualización diaria fallase un día si los procesos de consulta web siguen funcionando.


|Funciones/transacciones críticas| |
|:----|:----|
|Función/Transacción|Actividades que la componen|
|Actualización de datos|Lectura de datos, descubrimiento y carga. Habría que establecer cuál es el periodo de caída soportable, en función de las necesidades de la universidad.|

4.3 POSIBLES PROBLEMAS Y SOLUCIONES
----------------------

En este apartado se consignan algunos problemas esperables y sus soluciones. La lista debería ser mantenida y ampliada por los técnicos de la Universidad durante la vida del proyecto:

|Posibles problemas y soluciones| |
|:----|:----|
|Problema|Solución|
|Virtuoso no contesta, aunque el proceso está levantado|Intentar conectar a traves del interfaz iSQL para hacer una salida ordenada ( checkpoint; shutdown; ). Si no es posible conectarse, hay que reiniciar el proceso.|
|Las páginas de la web pública no se actualizan tras subir una nueva página personalizada, después de esperar un minuto|Reiniciar los procesos de Docker: docker-compose down –v; docker-compose up -d|
|Espacio en disco agotado|Si nos quedamos sin espacio en disco tenemos que averiguar la causa. En el caso de los frontales web, probablemente sea algún log. En caso de bases de datos es posible que hayan aumentado los datos y tenga que ampliarse el disco.|
|Al intentar ejecutar el entorno por primera vez, el primer acceso puede fallar porque aún no tiene las vistas cargadas|Al ejecutarlo la primera vez se generará el esquema de la base datos. Cargar las vistas tras la primera ejecución.|
|Faltan archivos estáticos (estilos, imágenes, javascript)|Configurar un servicio apache que se encargue de servir estos ficheros.|
|El servicio **identity** falla|Comprobar que en la configuración se ha escrito sin la barra final en las configuraciones de docker: “Authority: http://155.54.239.219:5108”|


