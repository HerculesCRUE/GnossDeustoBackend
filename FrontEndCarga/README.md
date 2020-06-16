# Sobre FrontEndCarga

Accesible en pruebas en esta direcciÃ³n: http://herc-as-front-desa.atica.um.es/carga-web.

## Manual
Este proyecto es una interfaz web desarrollada con el patrón MVC. Al acceder a la web vemos un menú en la parte superior desde la que se puede acceder a los repositorios, los shapes y las operaciones del urisFactory.

 - La página de inicio de la web es el listado de los repositorios, desde el cuál se puede crear un repositorio nuevo con el botón + que se encuentra por la mitad de la página. Al acceder a un repositorio podemos ver los shapes que tiene vinculados ese repositorio, así como las tareas de sincronización programadas y ejecutadas que ha tenido el repositorio, desde esta pantalla se pueden crear tanto nuevos shapes asociados al repositorio, como nuevas sincronizaciones. Además se puede editar o eliminar dicho repositorio así como la información asociada a él (sincronizaciones y shapes).
 - Shapes: Listado global de shapes.
 - factoría de uris: Interfaz desde la que se puede:
	 - Obtener una uri.
	 - Descargar el archivo el archivo de configuración de las uris.
	 - Reemplazar el archivo de configuración de uris.
	 - Eliminar una estructura de uris.
	 - Añadir una nueva estructura de uris.