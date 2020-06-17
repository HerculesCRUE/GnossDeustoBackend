# Sobre FrontEndCarga

Accesible en pruebas en esta dirección: http://herc-as-front-desa.atica.um.es/carga-web.

## Manual
Este proyecto es una interfaz web desarrollada con el patrón MVC. Al acceder a la web vemos un menú en la parte superior desde la que se puede acceder a los repositorios, los shapes y las operaciones del urisFactory.
### Página principal - listado de repositorios
La página de inicio de la web es el listado de los repositorios, desde este listado se puede crear un repositorio nuevo con el botón + que se encuentra por la mitad de la página. ![](img/repositorios.png)
Al acceder a un repositorio podemos ver los shapes que tiene vinculados ese repositorio, así como las tareas de sincronización programadas y ejecutadas que ha tenido el repositorio, como se muestra a continuación.
![](img/repositorio.png)
Desde esta pantalla se pueden crear tanto nuevos shapes asociados al repositorio, como nuevas sincronizaciones. Además se puede editar o eliminar dicho repositorio así como modificar la información asociada a él:
 - Modificar shape.
 - Eliminar shape.
 - Eliminar tarea.
### Listado de shapes
Listado global de shapes. Desde este listado se pueden acceder a todos los shapes que hay creados
![](img/shapes.png)
Para poder acceder a su información y poder editarla o eliminar el shape tendremos que acceder a él.
![](img/shape.png)
### Factoria de uris
Interfaz desde la que se puede:
 - Obtener una uri.
 - Descargar el archivo el archivo de configuración de las uris.
 - Reemplazar el archivo de configuración de uris.
 - Eliminar una estructura de uris.
 - Añadir una nueva estructura de uris.
 ![](img/urisFactory.png)
 A la hora de crear una estructura uri nos aparecerá un texto editable en el que aparece una estructura uri a modo de ayuda, como se ve en la siguiente imágen
![](img/AddUriStructure.png)