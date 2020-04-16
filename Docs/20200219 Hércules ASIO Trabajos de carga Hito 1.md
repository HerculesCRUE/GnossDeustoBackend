# Hércules Backend ASIO. Trabajos de carga. Hito 1

[1 INTRODUCCIÓN 3](#introducción)

[2 INTERFAZ DE CARGA Y SINCRONIZACIÓN
4](#interfaz-de-carga-y-sincronización)

[3 SERVICIO OAI-PMH PARA CARGA DE CVN
5](#servicio-oai-pmh-para-carga-de-cvn)

[3.1 ACTUALIZACIÓN DE DATOS DESDE EL REPOSITORIO CVN
6](#actualización-de-datos-desde-el-repositorio-cvn)

[4 CARGA INICIAL DE DATOS DE LA UNIVERSIDAD DE MURCIA
7](#carga-inicial-de-datos-de-la-universidad-de-murcia)

INTRODUCCIÓN
============

Este documento contiene una descripción general del funcionamiento de
los procesos de carga, fundamentalmente sincronización, y la descripción
funcional de los trabajos de carga del Hito 1, que serán:

-   Desarrollo de un servicio
    [OAI-PMH](http://www.openarchives.org/OAI/openarchivesprotocol.html)
    (Open Archive Initiative -- Protocol for Metadata Harvesting) que se
    comunique con un repositorio CVN (inicialmente el de Murcia) y
    convierta los metadatos CVN en metadatos ROH (Red de Ontologías
    Hércules) de salida.

-   Proceso de actualización contra el repositorio CVN. En este formato
    es posible identificar los CVs modificados, lo que permitirá que el
    sincronizador de datos de ASIO pida a OAI-PMH los CVs actualizados.

-   Desarrollo de un proceso de carga *ad-hoc* para la importación
    inicial de los datos de la Universidad de Murcia.

Este documento usa como referencia de OAI-PMH la guía de implementación
contenida en [20200203 Hércules ASIO Especificación de funciones de
Carga](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/20200203%20H%C3%A9rcules%20ASIO%20Especificaci%C3%B3n%20de%20funciones%20de%20Carga.md).

INTERFAZ DE CARGA Y SINCRONIZACIÓN
==================================

El interfaz estándar de carga y sincronización de datos de Hércules ASIO
será la salida de un servicio OAI-PMH desarrollado según la guía de
implementación indicada contenida en [20200203 Hércules ASIO
Especificación de funciones de
Carga](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/20200203%20H%C3%A9rcules%20ASIO%20Especificaci%C3%B3n%20de%20funciones%20de%20Carga.md).

Esto implica que los metadatos de salida de un servicio OAI-PMH para el
proyecto Hércules siempre tendrán el mismo formato, alineado con la ROH
del Backend ASIO que llamó al servicio OAI-PMH.

Sin embargo, esto no quiere decir que el desarrollo del interfaz sea
siempre tarea del SGI, sino que un SGI podría desarrollar su propio
servicio OAI-PMH para Hércules siguiendo la guía de implementación;
reutilizar el módulo liberado como software libre adaptándolo a su caso;
o reutilizar el módulo reconfigurando alguno de los mapeos predefinidos.
En cualquiera de los 3 casos, el formato de salida de los metadatos del
servicio OAI-PMH que serían recibidos por el API de ASIO sería siempre
el mismo, alineado con ROH

Durante el desarrollo del proyecto ASIO, se desarrollarán servicios
OAI-PMH que mapeen hacia el formato ROH los metadatos procedentes de:

-   Hércules SGI, en el hito 2, en colaboración con la empresa
    adjudicataria de este proyecto.

-   CVN, para el hito 1, como se describe en el siguiente apartado.

-   CERIF.xml, para el hito 2.

-   Alguno de los SGI más habituales, para el hito 2.

Para estos dos últimos casos habrá que determinar una universidad que
sirva como fuente de datos.

SERVICIO OAI-PMH PARA CARGA DE CVN
==================================

El proceso de incorporación de datos desde CVN hacia Hércules ASIO
responde al siguiente esquema de arquitectura:

![](.//media/image2_DataLoad.png)

Desarrollaremos un servicio OAI-PMH para carga de CVN que ofrecerá el
formato de salida especificado para Hércules ASIO (alineado con ROH) y
se comunicará con los servicios que le permiten la recuperación de
metadatos de CVN.

La Universidad tendrá que disponer de algún servicio que permita esta
recuperación. Las funciones de comunicación desde OAI-PMH con el
repositorio CVN se incluirán en un módulo funcional que podría adaptarse
a diferentes modos de acceso: API, FTP, sistema de ficheros, etc.

Este servicio OAI-PMH funcionará del siguiente modo:

-   Petición ListIdentifiers. El servicio OAI-PMH hará una petición a un
    servicio/repositorio de CVN de la Universidad que le devolverá los
    identificadores de los CVs modificados desde la fecha dada, o todos
    si no se indica nada.

-   Petición GetRecord. El servicio OAI-PMH hará una petición a un
    servicio/repositorio de CVN de la Universidad que le devolverá el
    PDF-XML con el currículo en formato CVN, para cada CV devuelto por
    la petición ListIdentifiers. A continuación:

    -   Petición al servicio Web de FECYT que devuelve los metadatos del
        fichero PDF-XML.

    -   Mapeo de los metadatos CVN al formato ROH de salida de OAI-PMH.

    -   Devolución de los metadatos ROH hacia la función del API de
        carga.

ACTUALIZACIÓN DE DATOS DESDE EL REPOSITORIO CVN
-----------------------------------------------

El proceso de actualización de datos debe incluirse entre los
desarrollos del hito 1. La única fuente disponible con actualizaciones
es el repositorio CVN, que permite conocer la fecha de actualización de
los CVs.

El sincronizador de datos lanzará una petición ListIdentifiers hacia el
servicio OAI-PMH, que le devolverá los CVs actualizados desde la fecha
dada. Posteriormente pedirá cada uno de los CVs actualizados y procederá
a actualizar los datos con la nueva información.

CARGA INICIAL DE DATOS DE LA UNIVERSIDAD DE MURCIA
==================================================

La Universidad de Murcia proporciona un conjunto de datos en formato XML
que van a constituir una primera carga de información en el RDF Store.
Esta fuente de datos no se va a utilizar para realizar procesos de
actualización, por lo que no se conectará a través del interfaz OAI-PMH,
sino que el proceso de carga será un desarrollo *ad-hoc*, cuyos
resultados podrán reutilizarse en casos similares. En cualquier caso, se
espera que los interfaces más comunes sean servicios OAI-PMH que
intermedien con Hércules SGI, otros SGI o repositorios CVN (como se
indica en el apartado anterior), y que permitan las actualizaciones
continuas. El siguiente diagrama refleja la arquitectura de este
proceso:

![](.//media/image3_DataLoad.png)

Este desarrollo permitirá establecer unos mapeos entre las tablas,
entregadas en formato XML por la UM, y las entidades y atributos de ROH,
generando como salida un conjunto de triples, que podrán cargarse en el
RDF Store de dos modos:

-   Utilizando las funciones de ingesta masiva con la que cuentan
    algunos RDF Store.

-   Enviando los triples a un interfaz del API de carga que los acepte y
    los almacene en el RDF Store.
