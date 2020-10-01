![](.//media/CabeceraDocumentosMD.png)

| Fecha         | 01/10/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|A20200203 Hércules ASIO Especificación de funciones de Carga| 
|Descripción|Especificación de funciones de Carga|
|Versión|0.2|
|Módulo|API CARGA|
|Tipo|Especificación|
|Cambios de la Versión|Actualizada la sección [ARQUITECTURA DE LOS PROCESOS DE CARGA](#arquitectura-de-los-procesos-de-carga)<br/>Modificada la sección [POST etl​/data-publish](#post-etldata-publish)<br/>Añadida la sección [POST etl​/data-validate-personalized](#post-etldata-validate-personalized)<br/>Añadida la sección [POST etl​/load-ontolgy](#post-etlload-ontology)<br/>Añadida la sección [GET etl​/data-discover-state/{identifier}](#get-etldata-discover-stateidentifier)<br/>Añadida la sección [GET etl​/GetOntology](#get-etlgetontology)<br/>|

# Hércules Backend ASIO. Especificación de las funciones de carga

[1 INTRODUCCIÓN](#introducción)

[2 ARQUITECTURA DE LOS PROCESOS DE CARGA](#arquitectura-de-los-procesos-de-carga)

[3 OAI-PMH. Implementación Hércules](#oai-pmh-implementación-hércules)

[3.1 Delete records](#delete-records)

[3.2 Granularidad de los 'datestamp'](#granularidad-de-los-datestamp)

[3.3 Datestamps devueltos](#datestamps-devueltos)

[3.4 Confidencialidad de los datos](#confidencialidad-de-los-datos)

[3.5 Seguridad](#seguridad)

[4 API de Carga](#api-de-carga)

[4.1 API de Carga. ETL](#api-de-carga-etl)

[4.1.1 POST etl​/data-publish](#post-etldata-publish)

[4.1.2 POST etl​/data-validate](#post-etldata-validate)

[4.1.3 POST etl​/data-validate-personalized](#post-etldata-validate-personalized)

[4.1.4 POST etl​/load-ontolgy](#post-etlload-ontolgy)

[4.1.5 POST etl​/data-discover](#post-etldata-discover)

[4.1.6 GET etl​/data-discover-state/{identifier}](#get-etldata-discover-stateidentifier)

[4.1.7 GET etl​/GetRecord/{repositoryIdentifier}](#get-etlgetrecordrepositoryidentifier)

[4.1.8 GET etl​/Identify/{repositoryIdentifier}](#get-etlidentifyrepositoryidentifier)

[4.1.9 GET etl​/ListIdentifiers/{repositoryIdentifier}](#get-etllistidentifiersrepositoryidentifier)

[4.1.10 GET etl​/ListMetadataFormats/{repositoryIdentifier}](#get-etllistmetadataformatsrepositoryidentifier)

[4.1.11 GET etl​/ListRecords/{repositoryIdentifier}](#get-etllistrecordsrepositoryidentifier)

[4.1.12 GET etl​/ListSets/{repositoryIdentifier}](#get-etllistsetsrepositoryidentifier)

[4.1.13 GET etl​/GetOntology](#get-etlgetontology)

[4.2 API de Carga. ETL-CONFIG](#api-de-carga-repository)

[4.2.1 GET etl-config/​repository](#get-etl-configrepository)

[4.2.2 POST etl-config/​repositor](#post-etl-configrepository)

[4.2.3 GET etl-config/​repository/{identifier}](#get-etl-configrepositoryidentifier)

[4.2.4 DELETE etl-config/​repository/{identifier}](#delete-etl-configrepositoryidentifier)

[4.2.5 PUT etl-config/​repository/{identifier}](#put-etl-configrepositoryidentifier)

[4.3 API de Carga. SYNC](#api-de-carga-sync)

[4.3.1 POST sync/execute ](#post-syncexecute)

[4.4 API de Carga. VALIDATION](#api-de-carga-validation)

[4.4.1 GET etl​-config/validation](#get-etl-configvalidation)

[4.4.2 POST etl​-config/validation](#post-etl-configvalidation)

[4.4.3 GET etl​-config/validation/{identifier}](#get-etl-configvalidationidentifier)

[4.4.4 DELETE etl​-config/validation/{identifier}](#delete-etl-configvalidationidentifier)

[4.4.5 PUT etl​-config/validation/{identifier}](#put-etl-configvalidationidentifier)

INTRODUCCIÓN
============

Este documento contiene la especificación de las funciones de carga del 
proyecto Hércules ASIO ([API Carga](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_CARGA)), e incluye la descripción de la implementación 
del [protocolo OAI-PMH](https://www.openarchives.org/pmh/)(Open Archive Initiative -- Protocol for Metadata Harvesting) 
para el intercambio de datos desde el SGI hacia el Backend. 
Los formatos de metadatos que devuelve el protocolo OAI-PMH se corresponden
con lo definido en la [Red de Ontologías Hércules (ROH)](https://github.com/HerculesCRUE/GnossDeustoOnto) para cada una de sus
entidades.

Los grupos funcionales del API de carga de datos son:

-   Carga ETL. Agrupa las funciones de acceso al interfaz OAI-PMH y las
    encargadas de la publicación, la validación y el descubrimiento.

-   Configuración ETL. Contiene las funciones que configuran los
    repositorios OAI-PMH a los que accederá ASIO y las de gestión de
    validaciones.

-   Sincronización ETL. Agrupa las funciones de configuración de los
    procesos de sincronización de datos.

ARQUITECTURA DE LOS PROCESOS DE CARGA
=====================================

Los procesos de carga de datos desde el SGI hacia el Backend ASIO
responden al siguiente esquema de arquitectura:

![](.//media/image2_FuncionesCarga.png)

El Sistema de Gestión de Investigación (SGI) contará con un proveedor de
datos conforme al [protocolo
OAI-PMH](http://www.openarchives.org/OAI/openarchivesprotocol.html)
(Open Archive Initiative -- Protocol for Metadata Harvesting) y a la
guía de implementación del apartado siguiente de este documento.

Este proveedor de datos será accedido por un API de Carga que, además de
otras, contará con las funciones de *harvesting* o recolección de
OAI-PMH.

El API de Carga se encargan de las funciones de conversión y validación; y
envía a una cola los RDF sobre los que hay que aplicar el descubrimiento.

El API de descubrimiento reconcilia, descubre enlaces y detecta equivalencias;
y se encarga de enviar los triples definitivos hacia el RDF Store.

Nuestra propuesta cuenta con un nodo central Unidata que recibirá y
cargará los triples publicados en cada universidad. De esto se encargarán
el proceso Sincronizador de cada universidad y un API de Carga en
Unidata que aceptará y consolidará los datos provenientes de las
universidades.

OAI-PMH. Implementación Hércules
================================

El interfaz de comunicación de datos entre el SGI (como el futuro
Hércules SGI, Sigma o cualquier otra fuente) y Hércules ASIO es un API
que implementa el protocolo OAI-PMH.

En nuestra propuesta de uso del protocolo OAI-PMH utilizamos la
definición
de [*sets*](http://www.openarchives.org/OAI/openarchivesprotocol.html#Set),
mediante el atributo setSpec, para reconocer la clase del ítem devuelto,
por ejemplo "roh:project" o "roh:researcher". \[poner un ejemplo de
petición devuelta por ListIdentifiers con datos supuestos de Hércules\]

El estándar OAI-PMH obliga a que todas las peticiones devuelvan
metadatos del ítem o ítems solicitados con el formato Dublin Core. En el
caso de Hércules, los metadatos devueltos en ese formato serán sólo
Title y Description, ya que el resto del modelo es insuficiente para
representar las entidades de ROH.

Para comunicar la información completa de las entidades, cada clase de
ítem, que se corresponde con una entidad en ROH, tendrá un formato de
metadatos alineado con su definición ontológica. El formato de metadatos
de cada entidad estará definido en un XSD que mapea los atributos y
relaciones de la entidad en ROH.

Por ejemplo, para \[añadir un ejemplo del formato de metadatos para una
entidad de ROH, como una publicación\]:

Una vez conocida la clase del ítem obtenido, a través del metadato
setSpec, podemos pedir el formato de metadatos adecuado para un registro
con las
instrucciones [GetRecord](http://www.openarchives.org/OAI/openarchivesprotocol.html#GetRecord) o [ListRecords](http://www.openarchives.org/OAI/openarchivesprotocol.html#ListRecords).
Por ejemplo, con esta petición:

[https://h-pmh.um.es/OAI-script?verb=ListRecords&from=2020-02-10&set=**roh:project**&metadataPrefix=**oai\_roh\_project**](https://h-pmh.um.es/OAI-script?verb=ListRecords&from=2020-02-10&set=roh:project&metadataPrefix=oai_roh_project)

Si alguien quisiera implementar un *harvester* o recolector sin conocer
a priori la relación entre *sets* y los formatos de metadatos, podría
recurrir al
método [ListMetadataFormats](http://www.openarchives.org/OAI/openarchivesprotocol.html#ListMetadataFormats),
que devuelve todos los formatos existentes o los formatos aceptados por
una entidad concreta \[poner un ejemplo\]

Además de lo indicado anteriormente de los sets y formatos de metadatos,
la implementación de OAI-PMH debe contar con otras características que
se describen a continuación.

Delete records
--------------

La propiedad 'deleteRecord' del repositorio estará declarada y
configurada como 'persistent' o 'transiente' para garantizar que se
puedan acceder a los datos eliminados. En caso contrario no se podrían
sincronizar las eliminaciones.

Granularidad de los 'datestamp'
-------------------------------

OAI-PMH permite definir granularidad de días o de segundos. El proveedor
debería soportar segundos si se quieren realizar varias sincronizaciones
diarias. En caso contrario sólo se podría realizar de forma efectiva una
sincronización diaria.

Datestamps devueltos
--------------------

Los 'datestamp' devueltos para los 'records' se corresponderán con la
última acción realizada sobre el 'record', sea ésta la creación, la
actualización o el borrado. En caso contrario no se podrían realizar
actualizaciones

Confidencialidad de los datos
-----------------------------

La privacidad de los datos de la entidad recuperada desde OAI-PMH estará
definida como uno de los atributos de los metadatos recuperados y será
gestionada por el API de Carga (funciones de publicación). Tomando como
referencia el documento [20200129 Hércules ASIO Data confidentiality
proposal](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/20200129%20H%C3%A9rcules%20ASIO%20Data%20confidentiality%20proposal.md)
y el enfoque acordado, las entidades marcadas como confidenciales se
almacenarán en ASIO en un grafo privado, que permitirá el uso de datos
agregados, pero no el acceso a los datos identificativos individuales.

Seguridad
---------

El proveedor de datos OAI-PMH será un punto de acceso a toda la
información del SGI (sea cual sea), incluidos los datos confidenciales.
Esto obliga a proteger el interfaz, obligando a que todas las llamadas
negocien el acceso antes de obtener información del sistema.

La propuesta es que las llamadas al proveedor OAI-PMH estén protegidas
por OAuth 2.0.

API de Carga
============

Este API tendrá todos los controladores necesarios para realizar la
carga de datos.

-   ETL: Extracción, transformación y carga de datos.

-   REPOSITORY: Contiene los procesos necesarios para la gestión de los repositorios OAI-PMH (creación, modificación, eliminación...).

-   SYNC: Contiene los procesos necesarios para la ejecución de las sincronizaciones.

-   VALIDATION: Contiene los procesos necesarios para la gestión de las validaciones (creación, modificación, eliminación...).

La especificación concreta de este API se puede consultar en:

<http://herc-as-front-desa.atica.um.es/carga/swagger/index.html>

La versión HTML de la documentación se encuentra en:

http://herc-as-front-desa.atica.um.es/docs/api-carga.html

API de Carga. ETL
-----------------

Dentro de este controlador se encuentran todos los métodos necesarios
para la extracción, transformación y carga de datos.

### POST etl​/data-publish

Ejecuta el penúltimo paso del proceso de carga, por el que el RDF generado
se encola en una cola de RABBIT para que posteriormente el servicio de descubimiento
lo procese y lo almacene en el Triple Store. Permite cargar una fuente RDF
arbitraria.

### POST etl​/data-validate

Valida un RDF mediante el shape SHACL configurado

### POST etl​/data-validate-personalized

Valida un RDF mediante el fichero de validación pasado

### POST etl​/load-ontology

Elimina la ontologia cargada y la reemplaza por la nueva

### POST etl​/data-discover

Aplica el descubrimiento sobre un RDF

### GET etl​/data-discover-state/{identifier}

Obtiene el estado de una tarea de descubrimiento

### GET etl​/GetRecord/{repositoryIdentifier}

Este método hace de PROXY entre el API y el proveedor OAI-PMH.

Recupera un registro de metadatos individual del repositorio en formato
XML OAI-PMH.

### GET etl​/Identify/{repositoryIdentifier}

Este método hace de PROXY entre el API y el proveedor OAI-PMH.

Obtiene la información del repositorio OAI-PMH configurado en formato
XML OAI-PMH.

### GET etl​/ListIdentifiers/{repositoryIdentifier}

Este método hace de PROXY entre el API y el proveedor OAI-PMH.

Es una forma abreviada de ListRecords, que recupera solo encabezados en
formato XML OAI-PMH en lugar de registros.

### GET etl​/ListMetadataFormats/{repositoryIdentifier}

Este método hace de PROXY entre el API y el proveedor OAI-PMH.

Recupera los formatos de metadatos disponibles del repositorio en
formato XML OAI-PMH.

### GET etl​/ListRecords/{repositoryIdentifier}

Este método hace de PROXY entre el API y el proveedor OAI-PMH.

Recupera registros del repositorio en formato XML OAI-PMH.

### GET etl​/ListSets/{repositoryIdentifier}

Este método hace de PROXY entre el API y el proveedor OAI-PMH.

Recuperar la estructura establecida de un repositorio en formato XML
OAI-PMH, útil para la recolección selectiva.

### GET etl​/GetOntology

Devuelve la ontología cargada.

API de Carga. REPOSITORY
------------------------

Dentro de este controlador se encuentran todos los métodos necesarios
para la gestión de los repositorios.

### GET etl-config/​repository

Obtiene un listado con todas las configuraciones de los repositorios
OAI-PMH.

### POST etl-config/​repository

Añade una nueva configuración de un repositorio OAI-PMH.

### GET etl-config/​repository/{identifier}

Obtiene la configuración de un repositorio OAI-PMH.

### DELETE etl-config/​repository/{identifier}

Elimina la configuración de un repositorio OAI-PMH.

### PUT etl-config/​repository/{identifier}

Modifica la configuración de un repostorio OAI-PMH.

API de Carga. SYNC
------------------

Dentro de este controlador se encuentran los métodos para
ejecutar las sincronizaciones.

### ​POST /sync​/execute

Ejecuta una sincronización.


API de Carga. VALIDATION
------------------------

Dentro de este controlador se encuentran todos los métodos necesarios
para la gestión de las validaciones.

### GET etl​-config/validation

Obtiene la configuración de los shape SHACL de validación.

### POST etl​-config/validation

Añade una configuración de validación mediante un shape SHACL.

### GET etl​-config/validation/{identifier}

Obtiene la configuración del shape SHACL pasado por parámetro.

### DELETE etl​-config/validation/{identifier}

Elimina la configuración una configuración de validación.

### PUT etl​-config/validation/{identifier}

Modifica la configuración de validación mediante un shape SHACL.

