![](..//Docs/media/CabeceraDocumentosMD.png)

# Despliegue del backend con Docker

## Requisitos previos
Para hacer funcionar el backend será necesario tener instalado en nuestro servidor:

* Docker (podemos seguir la documentacion oficial dependiendo de nuestra dristrubución de Linux) 
    - Centos https://docs.docker.com/engine/install/centos/
    - Ubuntu https://docs.docker.com/engine/install/ubuntu/
    - Debian https://docs.docker.com/engine/install/debian/
    
* docker-compose https://docs.docker.com/compose/install/  

* Base de datos RDF (en este ejemplo, Virtuoso desplegado con Docker)

* Base de datos SQL (en este ejemplo PostgreSQL desplegada con Docker)

* Gestor de colas (en este ejemplo RabbitMQ desplegada con Docker)
 
## Despliegue de Virtuoso

Partiendo desde la home del usurio (ej. /home/usuario/) creamos el directorio que va a conetener el docker-compose.yml, entramos en el directorio, descargamos el yml y levantamos el docker con los siguientes comandos: 

	mkdir virtuoso
	cd virtuoso
	wget http://herc-as-front-desa.atica.um.es/docs/docker-virtuoso/docker-compose.yml
	docker-compose up -d
	
## Despliegue de PostgreSQL

Partiendo desde la home del usurio (ej. /home/usuario/) creamos el directorio que va a conetener el docker-compose.yml, entramos en el directorio, descargamos el yml y levantamos el docker con los siguientes comandos: 

	mkdir postgresql
	cd postgresql
	wget http://herc-as-front-desa.atica.um.es/docs/docker-postgresql/docker-compose.yml
	docker-compose up -d
	
## Despliegue de RabbitMQ

Partiendo desde la home del usurio (ej. /home/usuario/) creamos el directorio que va a conetener el docker-compose.yml, entramos en el directorio, descargamos el yml y levantamos el docker con los siguientes comandos: 
	
	mkdir rabbitmq
	cd rabbitmq
	wget http://herc-as-front-desa.atica.um.es/docs/docker-rabbitmq/docker-compose.yml
	docker-compose up -d

## Preparación de Trifid

Para poner en marcha el servicio de linked tenemos que crear una imagen docker con la configuración adecuada para nuestro entorno. En la home del usuario descargamos el paquete de trifid con este comando:

	wget http://herc-as-front-desa.atica.um.es/docs/trifid.tar.gz
	
Después lo  descomprimimimos:

	tar xzvf http://herc-as-front-desa.atica.um.es/docs/trifid.tar.gz
	
Una vez descomprimido entramos en el directorio trifid.

	cd trifid
	
Y editamos el archivo config-custom.json indicando la ip de nuestra máquina en el sparqlEndpointUrl y en datasetBaseUrl.

	{
 		"baseConfig": "trifid:config-sparql.json", // inherit the default sparql config
  		"sparqlEndpointUrl": "http://ip_de_nuestra_máquina:8890/sparql", // overrides SPARQL endpoint
  		"datasetBaseUrl": "http://ip_de_nuestra_máquina:8081/", // enables "proxy" mode.
  		"listener": {
   		"port": 8081
  		}
	}

Una vez ajustados los parametros tenemos que construir la imagen con el siguiente comando:
	
	docker build -t trifid .
	
Con la imagen ya contruida la ponemos en marcha con este comando:

	docker run -d -p 8081:8081 --name trifid trifid


## Preparación de Apache

## Descarga de imágenes necesarias

Este es el listado de imágenes docker de las aplicaciones incluidas en GnossDeustoBackend:

 - [UrisFactory](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/UrisFactory "UrisFactory") - Servicio que genera las uris de los recursos: http://herc-as-front-desa.atica.um.es/docs/apiuris.tar.gz
 - [API_CARGA](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_CARGA "API_CARGA") - Servicio web que realiza las tareas de carga/configuración: http://herc-as-front-desa.atica.um.es/docs/apicarga.tar.gz
 - [API_IDENTITY](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/IdentityServerHecules "API_IDENTITY") - 
API que proporciona los tokens de acceso para las diferntes APIs securizadas: http://herc-as-front-desa.atica.um.es/docs/apiidentity.tar.gz
 - [API_UNIDATA](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Unidata/Api_Unidata "API_UNIDATA") - 
API que publica los RDF en el nodo central unidata: http://herc-as-front-desa.atica.um.es/docs/apiunidata.tar.gz
 - [FrontEndCarga](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/FrontEndCarga "FrontEndCarga") - Interfaz web para la parte de Repository y Validation del API_CARGA: http://herc-as-front-desa.atica.um.es/docs/apifrontcarga.tar.gz
 - [CronConfigure](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/CronConfigure) - Servicio Web que realiza la creación de tareas para la sincronización de un repositorio: http://herc-as-front-desa.atica.um.es/docs/apicron.tar.gz
 - [OAI_PMH_CVN](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/OAI_PMH_CVN "OAI_PMH_CVN") - Servicio OAI-PMH para la obtención de invstigadores de la Universidad de Murcia: http://herc-as-front-desa.atica.um.es/docs/apioaipmh.tar.gz
 - [cvn](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/cvn) - Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH: http://herc-as-front-desa.atica.um.es/docs/apicvn.tar.gz
- [Bridge](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/fair/bridge) - Bridge para métricas fair: http://herc-as-front-desa.atica.um.es/docs/apibridge.tar.gz
- [BrideSwagger](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/fair/bridge) - Interfaz swagger para el bridge: http://herc-as-front-desa.atica.um.es/docs/bridgeswagger.tar.gz
- [GestorDocumentacion](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/GestorDocumentacion) - Interfaz para la gestion de la documentación: http://herc-as-front-desa.atica.um.es/docs/apigesdoc.tar.gz


Despliegue de las APIs
----------------------------------



## Preparación del interfaz Fuseki para benchmark

Para poder utilizar benchmark necesitamos almacenar los datos sparql en Fuseki. Para desplegar Fuseki facilmente lo podemos hacer por medio de este comando Docker:

	docker run -d -p 3030:3030 stain/jena-fuseki fuseki

Durante el despliegue nos generará un usuario y contraseña que debemos apuntar.

Cuando tengamos Fuseki operativo podemos entrar a la interfaz gráfica y cargarle información. Craremos un data set y cargaremos los achivos ttl y nq que se encuentran en estas urls:

- https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/criterion-ontology/src
- https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/triplestore-dataset/static/data
- https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/triplestore-assessments



