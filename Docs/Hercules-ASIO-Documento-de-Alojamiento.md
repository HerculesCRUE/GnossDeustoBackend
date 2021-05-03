![](media/CabeceraDocumentosMD.png)

| Fecha         | 3/5/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Documento de alojamiento| 
|Descripción|Documento genérico para el despliegue de la arquitectura física|
|Versión|31|
|Módulo|Documentación|
|Tipo|Manual|
|Cambios de la Versión|Creación del documento|

## Hércules ASIO. Documento de alojamiento


1. INTRODUCCIÓN
============

Este documento describe una solución de alojamiento genérica para el sistema Hércules ASIO dentro de la infraestructura de una universidad.
Se contemplan dentro del mismo aspectos de diseño del sistema (arquitectura software y hardware), particularidades
para su explotación y monitorización, así como aspectos de seguridad y capacidad.

Este documento va dirigido al personal técnico encargado del mantenimiento de la Hércules ASIO en la universidad.


2. Descripción de la solución
==========================

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

