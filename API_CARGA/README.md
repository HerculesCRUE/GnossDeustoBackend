## Sobre API CARGA
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

Accesible en el entorno de desarrollo en esta dirección a través de swagger: http://herc-as-front-desa.atica.um.es/carga/swagger/index.html.

API CARGA es un servicio web que contienen 4 controladores, utilizados cada uno de ellos para su propio propósito:
 - etlController: Contiene los procesos ETL (Extract, Transform and Load) necesarios para la carga de datos.
 - repositoryController: Contiene los procesos necesarios para la gestión de los repositorios OAI-PMH (creación, modificación, eliminación...).
 - syncController: Contiene los procesos necesarios para la ejecución de las sincronizaciones.
 - ValidationController: Contiene los procesos necesarios para la gestión de las validaciones  (creación, modificación, eliminación...).
 
Esta aplicación se encarga de sincronizar los datos de un repositorio OAI-PMH con el RDF Store. Obtiene todas las entidades actualizadas desde la última sincronización, solicita al repositorio OAI-PMH todos sus datos y los inserta en el RDF Store. 

Como no es necesario ningún conector específico para actualizar un RDF Store ya que, por definición, deben tener un SPARQL Endpoint, no se ha creado ninguna librería específica de conexión al RDF Store. Las actualizaciones se realizan vía peticiones HTTP al SPARQL Endpoint. 
