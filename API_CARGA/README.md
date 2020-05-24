## Sobre API CARGA
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

Accesible en el entorno de desarrollo en esta dirección a través de swagger: http://herc-as-front-desa.atica.um.es/carga/swagger/index.html.

La documentación de la librería está disponible en: 
http://herc-as-front-desa.atica.um.es/api-carga/library/api/API_CARGA.Controllers.html

API CARGA es un servicio web que contienen 4 controladores, utilizados cada uno de ellos para su propio propósito:
 - etlController: Contiene los procesos ETL (Extract, Transform and Load) necesarios para la carga de datos.
 - repositoryController: Contiene los procesos necesarios para la gestión de los repositorios OAI-PMH (creación, modificación, eliminación...).
 - syncController: Contiene los procesos necesarios para la ejecución de las sincronizaciones.
 - ValidationController: Contiene los procesos necesarios para la gestión de las validaciones  (creación, modificación, eliminación...).
 
Para una especificación más detallada del servicio se puede consultar la siguiente documentación: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/20200203%20H%C3%A9rcules%20ASIO%20Especificaci%C3%B3n%20de%20funciones%20de%20Carga.md
 
Esta aplicación se encarga de sincronizar los datos de un repositorio OAI-PMH con el RDF Store. Obtiene todas las entidades actualizadas desde la última sincronización, solicita al repositorio OAI-PMH todos sus datos y los inserta en el RDF Store. 

*Conexión a Triple Store*
-------------------------

Como no es necesario ningún conector específico para actualizar un RDF Store ya que, por definición, deben tener un SPARQL Endpoint, no se ha creado ninguna librería específica de conexión al RDF Store. Las actualizaciones se realizan vía peticiones HTTP al SPARQL Endpoint.

El SPARQL Endpoint provisional se encuentra disponible en un servidor de la Universidad de Murcia, con acceso protegido por una VPN en la siguiente URL:

http://155.54.239.204:8890/sparql

Hay ejemplos de consultas en el documento [20200325 Hércules ASIO Ejemplos de consultas](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/SPARQL/20200325%20H%C3%A9rcules%20ASIO%20Ejemplos%20de%20consultas%20SPARQL.md)

Los datos cargados se pueden consultar en una versión preliminar del servidor Linked Data, soportado por [Trifid](https://github.com/zazuko/trifid), desplegado en los servidores de la Universidad de Murcia. Por ejemplo:

http://graph.um.es/res/project/RAYD-A-2002-6237



## Dependencias

- **dotNetRDF**: versión 2.5.1
- **IdentityServer4**: versión 3.1.2
- **IdentityServer4.EntityFramework**: versión 3.1.2
- **Microsoft.AspNetCore.Mvc.Formatters.Json**: versión 2.2.0
- **Microsoft.AspNetCore.Mvc.NewtonsoftJson**: versión 3.0.0
- **Microsoft.EntityFrameworkCore**: versión 3.1.4
- **Microsoft.EntityFrameworkCore.SqlServer**: versión 3.1.2
- **Microsoft.EntityFrameworkCore.Tools**: versión 3.1.2
- **Microsoft.Extensions.Logging.Debug**: versión 3.0.0
- **Microsoft.VisualStudio.Web.CodeGeneration.Design**: versión 3.0.0
- **Npgsql.EntityFrameworkCore.PostgreSQL**: versión 3.1.2
- **OaiPmhNet**: versión 0.4.1
- **Serilog.AspNetCore**: versión 3.2.0
- **Swashbuckle.AspNetCore**: versión 5.0.0
- **Swashbuckle.AspNetCore.Annotations**: versión 5.0.0
- **Swashbuckle.AspNetCore.Filters**: versión 5.0.2
- **Swashbuckle.AspNetCore.SwaggerGen**: versión 5.0.0
- **Swashbuckle.AspNetCore.SwaggerUI**: versión 5.0.0
