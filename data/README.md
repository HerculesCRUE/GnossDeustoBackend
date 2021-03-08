![](../Docs/media/CabeceraDocumentosMD.png)

# Hércules ASIO. Test Suite de carga de datos

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

[TODO definir completamente el proceso de pruebas de las cargas, con pantallazos]

A grandes rasgos, los pasos para probar las cargas son:
- Vaciar el grafo en el que se van a cargar los datos.
- Creación del repositorio fuente de datos desde las páginas de administración.
- Sincronizar todo el repositorio, sin crear una tarea de sincronización, desde las páginas de administración.
- Monitorizar el estado de la carga, desde las páginas de administración.
- Comprobación de los datos en el grafo, mediante consultas SPARQL, y desde las páginas de Linked Data Server.

