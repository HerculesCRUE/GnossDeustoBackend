![](./Docs/media/CabeceraDocumentosMD.png)


## Sobre GnossDeustoBackend
Éste es el repositorio del proyecto Hércules ASIO en el que se incluyen los siguientes programas y servicios

 - [API_CARGA](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_CARGA "API_CARGA"): Servicio web que realiza las tareas de carga/configuración.
 - [API_DISCOVER](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_DISCOVER "API_DISCOVER"): Servicio que realiza el descubrimiento de los RDF y su posterior publicación.
 - [FrontEndCarga](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/FrontEndCarga "FrontEndCarga"): Interfaz web para la parte de Repository y Validation del API_CARGA
 - [OAI_PMH_CVN](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/OAI_PMH_CVN "OAI_PMH_CVN"): Servicio OAI-PMH para la obtención de investigadores de la Universidad de Murcia.
 - [UrisFactory](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/UrisFactory "UrisFactory"): Servicio que genera las uris de los recursos
 - [cvn](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/cvn): Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH.
 - [CronConfigure](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/CronConfigure): Servicio Web que realiza la creación de tareas para la sincronización de un repositorio.
 - [GestorDocumentacion](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/GestorDocumentacion): Servicio web para la creación de páginas de contenido html y su posterior visualización en FrontEndCarga.
 - [IdentityServer](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/IdentityServerHecules): Servicio encargada de gestionar y proporcionar los tokens de acceso a los diferentes apis.
 
 ### Diagrama de componentes del proyecto:
 

<img src="img/diagrama_de_componentes.png" />


Todas las aplicaciones aquí descritas pueden usarse de dos formas distintas: 
  - Como un API, instalando la aplicación como se describe más abajo y llamando a su Endpoint.
  - Como una [librería](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/libraries), añadiendo el ensamblado DLL a la solución de código fuente y usando las clases y métodos definidos en él. 
  

### Configuración e instalación

Las [instrucciones de configuración e instalación](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Configuraci%C3%B3n%20e%20Instalaci%C3%B3n.md) son el punto de partida para 
comenzar a usar los desarrollos de Hércules ASIO
