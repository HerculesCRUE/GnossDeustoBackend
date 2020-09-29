![](..//Docs/media/CabeceraDocumentosMD.png)

# Acerca de CronConfigure

Accesible desde: [http://herc-as-front-desa.atica.um.es/cron-config/swagger/index.html](http://herc-as-front-desa.atica.um.es/cron-config/swagger/index.html)
Es un api para la gestión y configuración del programado de tareas, tanto de ejecución recurrente como ejecución única sobre los repositorios configurados.
Este api contiene 3 controladores:

 - **JobController**: Con los métodos disponibles en este controlador se pueden crear una sincronización, obtener las tareas y volver a ejecutar una tarea ya ejecutada.
 - **RecurringJobController**: Mediante los métodos de este controlador se puede crear una tarea recurrente especificando el patrón de repetición, borrar las tareas recurrentes y obtener dichas tareas, así como obtener las tareas que se han ejecutado a partir de una tarea recurrente.
 -  **ScheduledJobController**: Este controlador sirve para crear tareas que se ejecuten en una determinada fecha, obtener este tipo de tareas y eliminarlas

Mediante estos controladores se pueden crear los siguientes tipos de tareas: 
 - **Tarea recurrente:** Una tarea se le denomina recurrente cuando tiene una repeteción o recurrencia a través de un patrón. Por ejemplo que se ejecute todos los lunes a las 8 de la mañana.
 - **Tarea programada:** Una tarea se le denomina programada cuando está configurada para ejecutarse en un momento en el futuro.
 - **Tarea/ tarea de única ejecución:** Una tarea se le denomina de ejecución única cuando se ejecuta una sola vez en el momento de su creación o cuando se ha ejecutado ya, con esto último lo que 
 se quiere decir que una tarea recurrente cada vez que se ejecuta crea tareas de única ejecución al igual que una tarea programada cuando pasa a ser ejecutada ejecuta una tarea de ejecución única.

 La documentación de la librería está disponible en:

http://herc-as-front-desa.atica.um.es/cronconfigure/library/api/CronConfigure.Controllers.html

Los resultados de las pruebas unitarias se pueden consultar en [ResultsTest](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/CronConfigure/ResultsTest).

Las librerías compiladas se encuentran en la carpeta [librerías](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/libraries).

*Obtención del Token*
-------------------------
Este api esta protegida mediante tokens, por ello para poder usar la interfaz swagger hay que obtener un token, el cual se puede obtener desde https://herc-as-front-desa.atica.um.es/carga-web/Token

## Configuración en el appsettings.json
    {
      "ConnectionStrings": {
          "HangfireConnection":""
           },
             "Logging": {
                 "LogLevel": {
                       "Default": "Information",
                       "Microsoft": "Warning",
                       "Microsoft.Hosting.Lifetime":"Information"
    }
      },
        "AllowedHosts": "*",
        "Urls": "http://0.0.0.0:5107",
        "ConfigUrl": "http://herc-as-front-desa.atica.um.es/carga/",
		"Authority": "http://localhost:56306",
		"AuthorityGetToken": "http://localhost:56306/connect/token",
		"Scope": "apiCron",
		"ScopeCarga": "apiCarga",
		"GrantType": "client_credentials",
		"ClientId": "carga",
		"ClientSecret": "secret"
            }
 - HangfireConnection: Cadena de conexión a la base de datos PostgreSQL de tareas programadas
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - Urls: Url en la que se va a lanzar la aplicación
 - ConfigUrl: Url donde está lanzada la aplicación API Carga
 - Authority: Url de la servicio de identidades
 - AuthorityGetToken: Endpoint para la llamada de obtención del token
 - GrantType: Tipo de concesión de Oauth
 - Scope: Limitación de acceso al api de cron
 - ScopeCarga: Limitación de acceso al api de carga
 - ScopeUrisFactory: Limitación de acceso al api de urisFactory
 - ClientId: Id de cliente, en este caso se ha configurado un cliente que pueda acceder a todas las apis que usa la web
 - ClientSecret: "clave" de acceso del cliente

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
