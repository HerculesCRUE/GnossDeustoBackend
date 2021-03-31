![](../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 31/03/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Hércules ASIO. Test Suite de carga de datos| 
|Descripción|MTest Suite de carga de datos|
|Versión|0.1|
|Módulo|Test|
|Tipo|Manual|
|Cambios de la Versión|Creación|

# Hércules ASIO. Test Suite de carga de datos

[1 Introducción](#introducción)

[2 Pasos](#pasos)

[2.1 Vaciar el grafo](#vaciar-el-grafo)

[2.2 Crear reposiotrios](#crear-repositorios)

[2.3 Sincronizar repositorio](#sincronizar-repositorio)

[2.4 Monitorizar el estado de la carga](#monitorizar-el-estado-de-la-carga)

[2.5 Comprobación de los datos en el grafo](#comprobación-de-los-datos-en-el-grafo)



Introducción
============

Esta carpeta contiene los datos, programas y manuales para la ejecución la ejecución de las pruebas
de carga de datos en la Arquitectura Semántica de ASIO.

Este documento constituye el manual para la ejecución de las pruebas.

La suite de pruebas utilizará las siguientes fuentes de datos:
 - OAI-PMH del SGI. Simula el comportamiento que tendría la actualización desde Hércules SGI y carga 
 las entidades definidas mediante los esquemas XSD con los datos de los ficheros XML, según se indica en
 el proyecto [OAI-PMH de Hércules](https://github.com/HerculesCRUE/oai-pmh). 
 - OAI-PMH para CVNs de la Universidad de Murcia. Carga las entidades de la producción y actividad científica 
 de un investigador, según lo que se puede definir en un cv normalizado (estándar CVN).
 - CERIF de Radboud. Se trata de un repositorio de investigación de la Universidad de Radboud, al que 
 se puede interrogar mediante un servicio OAI-PMH. El servicio contesta con datos en formato CERIF-XML y 
 contiene personas, organizaciones, proyectos, publicacione y productos (actividades) de investigación.
 
Pasos
============

Los pasos para llevar a cabo las pruebas de carga son los siguientes:

Vaciar el grafo
---------------------
Vaciar el grafo en el que se van a cargar los datos.

Crear repositorios
---------------------
Creación del repositorio fuente de datos desde las páginas de administración.

Sincronizar repositorio
---------------------
Sincronizar todo el repositorio, sin crear una tarea de sincronización, desde las páginas de administración.

Monitorizar el estado de la carga
---------------------
Monitorizar el estado de la carga, desde las páginas de administración.

Comprobación de los datos en el grafo
---------------------
Comprobación de los datos en el grafo, mediante consultas SPARQL, y desde las páginas de Linked Data Server.
