![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 18/05/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Descripción del servicio Sparql Replication| 
|Descripción|Manual del servicio de background de replicación Sparql|
|Versión|1|
|Módulo|SparqlReplication|
|Tipo|Documentación|
|Cambios de la Versión|Creación del documento|


# Acerca de SparqlReplication

![](https://github.com/HerculesCRUE/GnossDeustoBackend/workflows/Build%20Hercules.Asio.SPARQLReplication/badge.svg)

El proyecto SparqlReplication es una aplicación en segundo plano que se encarga de ejecutar instrucciones SPARQL en un SPARQL Endpoint. Su función es servir como sistema de replicación, que repita las instruciones SPARQL de inserción, modificación o eliminación que se han ejecutado previamente sobre una base de datos RDF (maestro), en otra(s) base(s) de datos RDF (réplicas). 
Cuando arranca la aplicación se queda escuchando los eventos que se envían a una cola de RabbitMQ. Cada evento que llega a esa cola será una instrucción SPARQL que se ejecutará en el o los servidores SPARQL réplica. 
Tanto el servidor de RabbitMQ, como el nombre de la cola y el SPARQL Endpoint de la base de datos réplica son configurables. 

Las aplicaciones que escriben datos en el servidor maestro deben enviar un mensaje a la cola inmediatamente después de ejecutar cada instrucción SPARQL. El servicio estará _escuchando_ esta cola para replicar de manera inmediata en la base de datos RDF réplica. 

La aplicación tiene una clase Worker, que es la encargada de escuchar la cola e insertar las acciones en la base de datos RDF réplica. 

## Configuración en el appsettings.json
    {
	  "Logging": {
		"LogLevel": {
		  "Default": "Information",
		  "Microsoft": "Warning",
		  "Microsoft.Hosting.Lifetime": "Information"
		}
	  },
	  "SparqlServer_Url": "http://192.168.2.3:8890/sparql",
	  "SparqlServer_User": "",
	  "SparqlServer_Password": "",
	  "RabbitMQ_Hostname": "192.168.2.4",
	  "RabbitMQ_User": "",
	  "RabbitMQ_Password": "",
	  "RabbitMQ_VirtualHost": "hercules",
	  "RabbitMQ_QueueName": "HerculesQueueVirtuoso"
    }
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - SparqlServer_Url: Sparql endpoint de la base de datos RDF réplica
 - SparqlServer_User: Usuario del sparql endpoint de la base de datos RDF réplica
 - SparqlServer_Password: Password del sparql endpoint de la base de datos RDF réplica
 - RabbitMQ_Hostname: IP o url del servidor RabbitMQ
 - RabbitMQ_User: Usuario de RabbitMQ
 - RabbitMQ_Password: contraseña del usuario
 - RabbitMQ_VirtualHost: Virtual host al que está asociado la cola
 - RabbitMQ_QueueName: Nombre de la cola
 
Se puede encontrar un el appsettings usado para este servicio sin datos sensibles en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/src/Hercules.Asio.SPARQLReplication/Hercules.Asio.SPARQLReplication/appsettings.json


## Dependencias

- **Microsoft.AspNetCore.Http.Abstractions**: versión 2.2.0
- **Microsoft.Extensions.Hosting**: versión 3.1.13
- **RabbitMQ.Client**: versión 6.2.1

