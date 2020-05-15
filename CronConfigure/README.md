# Sobre CronConfigure

Accesible desde: [http://herc-as-front-desa.atica.um.es/cron-config/swagger/index.html](http://herc-as-front-desa.atica.um.es/cron-config/swagger/index.html)
Es un api para la gestión y configuración del programado de tareas, tanto de ejecución recurrente como ejecución única sobre los repositorios configurados.
Este api contiene 3 controladores:

 - **JobController**: Con los métodos disponibles en este controlador se pueden crear una sincronización, obtener las tareas y volver a ejecutar una tarea ya ejecutada.
 - **RecurringJobController**: Mediante los métodos de este controlador se puede crear una tarea recurrente especificando el patrón de repetición, borrar las tareas recurrentes y obtener dichas tareas, así como obtener las tareas que se han ejecutado a partir de una tarea recurrente.
 -  **ScheduledJobController**: Este controlador sirve para crear tareas que se ejecuten en una determinada fecha, obtener este tipo de tareas y eliminarlas
