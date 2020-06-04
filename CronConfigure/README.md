# Sobre CronConfigure

Accesible desde: [http://herc-as-front-desa.atica.um.es/cron-config/swagger/index.html](http://herc-as-front-desa.atica.um.es/cron-config/swagger/index.html)
Es un api para la gestión y configuración del programado de tareas, tanto de ejecución recurrente como ejecución única sobre los repositorios configurados.
Este api contiene 3 controladores:

 - **JobController**: Con los métodos disponibles en este controlador se pueden crear una sincronización, obtener las tareas y volver a ejecutar una tarea ya ejecutada.
 - **RecurringJobController**: Mediante los métodos de este controlador se puede crear una tarea recurrente especificando el patrón de repetición, borrar las tareas recurrentes y obtener dichas tareas, así como obtener las tareas que se han ejecutado a partir de una tarea recurrente.
 -  **ScheduledJobController**: Este controlador sirve para crear tareas que se ejecuten en una determinada fecha, obtener este tipo de tareas y eliminarlas
 
 La documentación de la librería está disponible en:

http://herc-as-front-desa.atica.um.es/cronconfigure/library/api/CronConfigure.Controllers.html

Los resultados de las pruebas unitarias se pueden consultar en [ResultsTest](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/CronConfigure/ResultsTest).

## Dependencias

- **coverlet.collector**: versión 1.2.1
- **docfx.console**: versión 2.53.1
- **Hangfire**: versión 1.7.10
- **Hangfire.AspNetCore**: versión 1.7.10
- **Hangfire.Core**: versión 1.7.10
- **Hangfire.PostgreSql**: versión 1.6.4.2
- **Microsoft.EntityFrameworkCore**: versión 3.1.3
- **Microsoft.EntityFrameworkCore.Tools**: versión 3.1.3
- **Microsoft.VisualStudio.Web.CodeGeneration.Design**: versión 3.1.1
- **ncrontab**: versión 3.3.1
- **Npgsql.EntityFrameworkCore.PostgreSQL**: versión 3.1.3
- **Serilog.AspNetCore**: versión 3.2.0
- **Swashbuckle**: versión 5.6.0
- **Swashbuckle.AspNetCore**: versión 5.3.1
- **Swashbuckle.AspNetCore.Annotations**: versión 5.3.1
- **Swashbuckle.AspNetCore.Filters**: versión 5.1.1
- **Swashbuckle.AspNetCore.Swagger**: versión 5.3.1
- **Swashbuckle.AspNetCore.SwaggerGen**: versión 5.3.1
- **Swashbuckle.AspNetCore.SwaggerUI**: versión 5.3.1
