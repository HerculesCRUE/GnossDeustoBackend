![](../Docs/media/CabeceraDocumentosMD.png)
 
# Manual de usuario del FrontEnd de Carga

[Introducción](#introduccion)

[Página principal. Listado de repositorios](#página-principal-listado-de-repositorios)

[Vista de una tarea](#vista-de-una-tarea)

[Vista de una tarea recurrente](#vista-de-una-tarea-recurrente)

[Listado de shapes](#listado-de-shapes)

[Factoría de URIs](#factoría-de-uris)

Introducción
------------

Al acceder a la web vemos un menú en la parte superior desde la que se puede acceder
a los repositorios, los shapes y las operaciones del urisFactory.

Página principal. Listado de repositorios
-----------------------------------------

La página de inicio de la web es el listado de los repositorios. Desde este listado se
puede crear un repositorio nuevo con el botón + que se encuentra por la mitad 
de la página. 
![](img/repositorios.png)

Al acceder a un repositorio podemos ver los shapes que tiene vinculados ese repositorio, 
así como las tareas de sincronización programadas y ejecutadas que ha tenido, 
como se muestra a continuación. 
![](img/repositorio.png)

Desde esta pantalla se pueden crear nuevos shapes asociados al repositorio y nuevas 
sincronizaciones. Además se puede editar o eliminar el repositorio y de modificar la
información asociada a él (shapes y tareas):
 - Modificar shape.
 - Eliminar shape.
 - Eliminar tarea programada.
 - Eliminar tarea recurrente.

También se puede acceder a la información de las tareas, tanto de tareas de ejecución
única como de tareas recurrentes, como se muestra en los apartados siguientes.

Vista de una tarea
------------------

En la pantalla que se muestra a continuación se muestran los datos de una tarea ejecutada
y un botón con el cual se puede volver a lanzar. Los datos mostrados son:
 - Identificador de la tarea.
 - La tarea ejecutada.
 - El estado de la tarea, que puede ser que se haya ejecutado con éxito o esté en estado fallido.
 - La fecha de la ejecución en formato UTC.
 - Error que haya causado el fallo de la tarea.
![](img/JobFailDetails.png)

Vista de una tarea recurrente
-----------------------------

En la siguiente imágen se muestran los datos de una tarea recurrente:
 - El nombre de la tarea recurrente.
 - La expresión del cron que indica la recurrencia de dicha tarea.
 - La fecha de la próxima ejecución.
 - El identificador de la última tarea ejecutada, en el caso de que se haya ejecutado alguna tarea.
 - El estado de la última tarea ejecutada, en el caso de que se haya ejecutado alguna tarea.
 - Por último se muestra un litado de las tareas ejecutadas a partir de la tarea recurrente.
![](img/RecurringJobDetails.png)

Listado de shapes
-----------------

Listado global de shapes. Desde este listado se pueden acceder a todos los shapes que hay creados
![](img/shapes.png)
Para poder acceder a su información y poder editarla o eliminar el shape tendremos que acceder a él.
![](img/shape.png)

Factoría de URIs
----------------

Interfaz desde la que se puede:
 - Obtener un URI.
 - Descargar el archivo el archivo de configuración de los URIs.
 - Reemplazar el archivo de configuración de URIs.
 - Eliminar una estructura de URIs.
 - Añadir una nueva estructura de URIs.
 ![](img/urisFactory.png)
 
 A la hora de crear una estructura URI nos mostrará un texto editable en el que aparece una
 estructura a modo de ayuda, como se ve en la siguiente imágen:
![](img/AddUriStructure.png)
