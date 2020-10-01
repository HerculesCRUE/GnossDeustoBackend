![](.//media/CabeceraDocumentosMD.png)

# Prueba del entorno y carga inicial

Una vez desplegados y configurados todos los contenedores podemos realizar las pruebas necesarias para comprobar todos los servicios están respondiendo correctamente, para ello accederemos vía el interfaz web y realizaremos una carga inicial, y comprobaremos en virtuoso que los datos se hayan obtenido correctamente. 

El primer paso es acceder a la interfaz web de los servicios dados de alta, si hemos seria: http://ipdelamaquina:5103
CapturaHer1

Comprobamos que podemos acceder al endpoint de virtuoso accediendo a http://ipdelamaquina:8089/sparql y comprobamos que no hay cargado ningún dato con la siguiente con la siguiente consulta:
```
select * from <http://HerculesDemo.com> where

 {

      ?s a <http://purl.org/roh/mirror/foaf#Person>

 }
```

Posteriormente creamos un nuevo repositorio, para ello pulsamos en el botón de añadir en la parte derecha y seguimos estos pasos:
CapturaHer2

* Completamos el campo Name con el nombre que queramos para el repositorio por ejemplo Prueba_CVN
* El Oauth token lo completamos con el código que queramos
* En la url introducimos la URL o IP y puerto que hemos configurado para el servicio apiOAIPMH
* Pulsamos en Guardar 'Cambiar nombre por crear' 

Una vez creado el repositorio accedemos al mismo pulsando en el nombre, al acceder al mismo nos mostrara los datos que hemos proporcionado para la configuración del mismo, el porcentaje de las tareas lanzadas que se ha ejecutado de forma correcta, cual ha sido la última tarea ejecutada y si estado ha sido ejecutada correctamente. Además podemos comprobar las configuraciones de validación de los RDF's del repositorio pulsando en el enlace *Ver validaciones* o bien en el enlace *Shapes* en la cabecera.
CapturaHer5

El primer paso a realizar es una sincronización inicial del repositorio con los RDF's, para ello pulsando en Sincronizar.
CapturaHer6

Al realizar esta acción estamos generando una tarea que obtiene los RDF's y los carga en la base de datos RDF, 

Para comprobar que se han cargado correctamente volvemos a acceder al endpoint de virtuoso y realizamos la siguiente consulta:
```
select * from <http://HerculesDemo.com> where

 {

      ?s a <http://purl.org/roh/mirror/foaf#Person>.

      ?s <http://purl.org/roh/mirror/foaf#name> ?nombre

 }
```

Posteriormente vamos a añadir una tarea para que sincronice periódicamente los RDF's, volvemos a la página principal del repositorio y navegamos por la página hacia abajo hasta encontrar el apartado "Tareas de sincronización", aquí podemos ver las tareas configuradas así como cuando van a ejecutarse de nuevo y cuál es la configuración del cron para la periodicidad de ejecución, podemos comprobar el valor haciendo uso de esta página: https://crontab.guru/ en la cual podemos introducir los valores configurador en el cron para ver a que periodicidad corresponden.


Para añadir una nueva tarea, pulsamos en Crear nueva tarea y seguimos estos pasos:

* Indicamos la fecha inicial en la que se va a ejecutar la tarea
* En la fecha de ultima sincronización indicamos la fecha desde la que queremos que se empiecen a sincronizar los RDF's
* En caso de que queramos sincronizar ciertos objetos y los RDF's que los contienen, indicamos aquí los objetos
* En caso de que queramos sincronizar un objeto concreto indicamos su código

Si queremos que la tarea se ejecute periódicamente, tenemos que completar también los campos siguientes:
* Indicamos el nombre de la tarea para poder identificarla
* La expresión cron indica cada cuanto se va a ejecutar la tarea, para validar que la expresión es correcta y se ejecuta cuando queremos, podemos ayudarnos de esta web: https://crontab.guru/

Por último, hacemos clic en Guardar para crear la tarea	

Cuando accedamos de nuevo al repositorio, nos mostrara la tarea en el listado de "Tareas de sincronización"

Para comprobar las ejecuciones y su estado, accedemos al repositorio y navegamos hasta el apartado
