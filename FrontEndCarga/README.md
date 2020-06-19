![](../Docs/media/CabeceraDocumentosMD.png)

# Sobre FrontEnd de Carga

Este módulo constituye el interfaz Web de administración de las cargas de datos en la plataforma Hércules ASIO. Esta aplicación web realizada mediante el patrón MVC (módelo-vista-controlador) esta formado por 5 controladores, que a su vez estos controladores se comunican con los diferentes apis creados en este proyecto. Estos controladores dividen su ámbito en la gestión de errores, de los repositorios, de los shapes, de las tareas y la gestión de la factoria de uris:
  - ErrorController: Gestiona los errores para devolver una página 404 o 500 según la excepción que se genera.
  - JobController: Controlador que gestiona las operaciones (listado, obtención de tareas, ejecución, ...) que se realizan en la web con cualquier tipo de tarea.
  - RepositoryConfigController: Controlador encargado de gestionar las operaciones con los repositorios. Creación, modificación, eliminación y listado.
  - ShapeConfigController: Encargado de gestionar las operaciones con los shapes. Creación, modificación, eliminación y listado.
  - UrisFactoryController: Este controlador es el encargado de gestionar todas las tareas que se pueden realizar con el api de UrisFactory, tanto la obtención de uris como las operaciones con el fichero de configuración de las uris.

Hay una versión disponible en el entorno de pruebas de la Universidad de Murcia, en esta dirección: 

http://herc-as-front-desa.atica.um.es/carga-web.


En esta carpeta está disponible el [Manual de Usuario del FrontEnd](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/FrontEndCarga/Manual%20de%20usuario.md).
