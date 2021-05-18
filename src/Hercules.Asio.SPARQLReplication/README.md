![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 18/05/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Descripción del servicio Sparql Replication| 
|Descripción|Manual del servicio Sparql Replication|
|Versión|1|
|Módulo|SparqlReplication|
|Tipo|Documentación|
|Cambios de la Versión|Creado nuevo servicio de background de replicación SPARQL|


# Acerca de SparqlReplication

![](https://github.com/HerculesCRUE/GnossDeustoBackend/workflows/Build%20Hercules.Asio.SPARQLReplication/badge.svg)

El proyecto SparqlReplication es una apliación en segundo plano que se encarga de insertar instruciones SPARQL en un SPARQL Endpoint. Su uso principal es usarlo como sistema de replicación, para replicar las instruciones SPARQL de inserción, modificación o eliminación que se han ejecutado previamente sobre una base de datos RDF (maestro), en otra base de datos RDF (réplica). 
Cuando arranca la aplicación, se queda escuchando los eventos que se envían a una cola de RabbitMQ. Cada evento que llega a esa cola, será una instrucción SPARQL que se replicará en el servidor SPARQL réplica. 
Tanto el servidor de RabbitMQ, como el nombre de la cola y el SPARQL Endpoint de la base de datos réplica son configurables. 

Las aplicaciones que escriben datos en el servidor maestro, inmediatamente después de ejecutar cada instrucción SPARQL, deben enviar un mensaje a la cola donde está escuchando este servicio para que se repliquen de manera inmediata en la base de datos RDF réplica. 

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
	  "UrlSparqlServer": "http://192.168.2.3:8890/sparql",
	  "RabbitMQ": {
		"Hostname": "192.168.2.4",
		"User": "",
		"Password": "",
		"VirtualHost": "hercules",
		"QueueName": "SparqlReplication"
	  }
	}
 - LogLevel.Default: Nivel de error por defecto
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host
 - UrlSparqlServer: Sparql endpoint de la base de datos RDF réplica
 - RabbitMQ.Hostname: IP o url del servidor RabbitMQ
 - RabbitMQ.User: Usuario de RabbitMQ
 - RabbitMQ.Password: contraseña del usuario
 - RabbitMQ.VirtualHost: Virtual host al que está asociado la cola
 - RabbitMQ.QueueName: Nombre de la cola
 
 Se puede encontrar un el appsettings usado para este servicio sin datos sensibles en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/src/Hercules.Asio.SPARQLReplication/Hercules.Asio.SPARQLReplication/appsettings.json


## Dependencias

- **Microsoft.AspNetCore.Http.Abstractions**: versión 2.2.0
- **Microsoft.Extensions.Hosting**: versión 3.1.13
- **RabbitMQ.Client**: versión 6.2.1

