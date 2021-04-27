![](./Docs/media/CabeceraDocumentosMD.png)

# Hércules ASIO. Arquitectura Semántica e Infraestructura Ontológica

El objetivo del Proyecto ASIO es adquirir un servicio de I+D consistente en el desarrollo de soluciones innovadoras para la Universidad de Murcia en relación al reto de Arquitectura Semántica e Infraestructura Ontológica. En concreto, se pretende desarrollar e incorporar soluciones que superen las actualmente disponibles en el mercado.

La solución ASIO es susceptible de ser utilizada en el futuro de forma regular tanto por la Universidad de Murcia como por las restantes Universidades que forman parte de la CRUE con necesidades y competencias similares, ya que como proceso de compra pública precomercial, el objetivo ha sido alcanzar una solución innovadora dirigida específicamente a los retos y necesidades que afectan al sector público y que persiguen la dinamización de la I+D+i.

El proyecto Infraestructura Ontológica de la información del Sistema Universitario Español consiste en crear una red de ontologías que describa con fidelidad y alta granularidad los datos del dominio de la Gestión de la Investigación.

El proyecto Arquitectura Semántica de Datos del SUE ha consistido en desarrollar una plataforma eficiente para almacenar, gestionar y publicar los datos del SGI (Sistema de Gestión de la Investigación), basándose en la Infraestructura Ontológica, con la capacidad de sincronizar instancias instaladas en diferentes Universidades.

Dado de los demás proyectos que componen HÉRCULES dependen tanto de la Infraestructura Ontológica, como de la Arquitectura Semántica de Datos, el proyecto ASIO interactúa con los desarrollos y resultados de los demás proyectos HÉRCULES: desarrollo de un Prototipo Innovador de Gestión de la Investigación para Universidades y Enriquecimiento de Datos y Métodos de Análisis.

## Documentación del Backend SGI

La documentación del backend se puede consultar en la carpeta [Docs](Docs/).

Además, la carpeta de [formación](Formacion/) contiene el material usado en las jornadas de formación del proyecto ASIO.

## Desarrollo de Hércules ASIO

Hércules ASIO es un proyecto de software libre alojado en dos repositorios públicos de GitHub:

 - [GitHub de Infraestructura Ontológica](https://github.com/HerculesCRUE/GnossDeustoOnto). Este repositorio aloja la Red de Ontologías Hércules - ROH y tiene los siguientes documentos principales: 
   - [Tutorial de la Red de Ontologías Hércules (ROH)](https://github.com/HerculesCRUE/GnossDeustoOnto/tree/master/Documentation). Se   trata de una documentación explicativa, generada como primera lectura recomendada. El documento ilustra con diagramas como se relacionan entre sí las entidades principales de ROH. También incluye una tabla por cada entidad, en la que se indican las subclases y propiedades de tipo object y datatype.
   - [ROH Reference Specification](https://github.com/HerculesCRUE/GnossDeustoOnto/blob/master/Documentation/1-%20OntologyDocumentation.pdf). Este documento   detalla, en formato tabular, las subclases y propiedades de tipo  object y datatype de cada concepto de la ontología ROH. 
 - **GitHub de  Arquitectura Semántica (Backend SGI)**. Se trata del presente repositorio, que contiene los componentes de software que, junto con el software base de sistemas y bases de datos, constituyen la arquitectura semántica del proyecto ASIO.

## Desarrollo del Backend SGI

[![codecov](https://codecov.io/gh/HerculesCRUE/GnossDeustoBackend/branch/master/graph/badge.svg?token=4SONQMD1TI)](https://codecov.io/gh/HerculesCRUE/GnossDeustoBackend)


El repositorio de desarrollo de la Arquitectura Semántica del proyecto Hércules ASIO (Backend SGI) contiene los siguientes programas y servicios:

<img src="Docs/media/Hércules ASIO Diagrama de componentes.png" />

 - [Hercules.Asio.Api.Carga](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/Hercules.Asio.Api.Carga): servicio web que contiene 4 controladores:
   - etlController: Contiene los procesos ETL (Extract, Transform and Load) necesarios para la carga de datos.
   - repositoryController: Contiene los procesos necesarios para la gestión de los repositorios OAI-PMH (creación, modificación, eliminación...).
   - syncController: Contiene los procesos necesarios para la ejecución de las sincronizaciones.
   - ValidationController: Contiene los procesos necesarios para la gestión de las validaciones (creación, modificación, eliminación...). La carpeta    Validaciones contiene información sobre los shapes SHACL definidos para validar.
- [Hercules.Asio.Api.Discover](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/Hercules.Asio.Api.Discover): Ofrece las siguientes funciones, que forman parte del proceso de carga:
   - Reconciliación de entidades, que evita la duplicación de entidades.
   - Descubrimiento de enlaces, que genera enlaces hacia datasets externos y ofrece información de ayuda en la reconciliación de entidades.
   - Detección de equivalencias, entre nodos Unidata.
 - [Hercules.Asio.Web](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/Hercules.Asio.Web): interfaz Web de administración de las cargas de datos en la plataforma Hércules ASIO.
 - [Hercules.Asio.CVN2OAI_PMH](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/Hercules.Asio.CVN2OAI_PMH): Sirve los datos de los curículums de los investigadores de la Universidad de Murcia en formato RDF y dublin core.
 - [Hercules.Asio.UrisFactory](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/Hercules.Asio.UrisFactory): Servicio que genera URIs según el esquema definido en ASIO.
 - [cvn](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/cvn): Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH.
 - [Hercules.Asio.Cron](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/Hercules.Asio.Cron): Es un api para la gestión y configuración del programado de tareas, tanto de ejecución recurrente como ejecución única sobre los repositorios configurados.
 - [Hercules.Asio.DynamicPages](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/Hercules.Asio.DinamicPages): Servicio web para la creación de páginas de contenido html y su posterior visualización.
 - [Hercules.Asio.IdentityServer](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/Hercules.Asio.IdentityServer):encargado de la securización mediante tokens para los apis que forman el proyecto.
 - [Hercules.Asio.LinkedDataServer](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/Hercules.Asio.LinkedDataServer): proporciona el servicio de datos enlazados de Hércules ASIO.

### Configuración e instalación

Las [instrucciones de configuración e instalación](Configuracion-e-Instalacion.md) son el punto de partida para comenzar a usar los desarrollos de Hércules ASIO.

Todas las aplicaciones aquí descritas pueden usarse de dos formas distintas: 
  - Como un API, instalando la aplicación y llamando a su Endpoint.
  - Como una [librería](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Builds/libraries), añadiendo el ensamblado DLL a la solución de código fuente y usando las clases y métodos definidos.

