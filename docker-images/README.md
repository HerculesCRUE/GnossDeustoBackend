# Despliegue del backend

## Requisitos previos
Para hacer funcionar el backend será necesario tener instalado en nuestro servidor:
* Apache

* Docker (podemos seguir la documentacion oficial dependiendo de nuestra dristrubución de Linux) 
    - Centos https://docs.docker.com/engine/install/centos/
    - Ubuntu https://docs.docker.com/engine/install/ubuntu/
    - Debian https://docs.docker.com/engine/install/debian/
    
* docker-compose https://docs.docker.com/compose/install/  

* Base de datos RDF (en este ejemplo, Virtuoso desplegado con Docker)

* Base de datos SQL (en este ejemplo PostgreSQL desplegada con Docker)
 
## Descarga de imágenes necesarias

Este es el listado de imágenes docker de las aplicaciones incluidas en GnossDeustoBackend:

 - PostgreSQL - Imagen de PostgreSQL preparada para funcionar con el backend: http://herc-as-front-desa.atica.um.es/docs/herculessql.tar.gz
 - [API_CARGA](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_CARGA "API_CARGA") - Servicio web que realiza las tareas de carga/configuración: http://herc-as-front-desa.atica.um.es/docs/apicarga.tar.gz
 - [FrontEndCarga](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/FrontEndCarga "FrontEndCarga") - Interfaz web para la parte de Repository y Validation del API_CARGA: http://herc-as-front-desa.atica.um.es/docs/apifrontcarga.tar.gz
 - [OAI_PMH_CVN](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/OAI_PMH_CVN "OAI_PMH_CVN") - Servicio OAI-PMH para la obtención de invstigadores de la Universidad de Murcia: http://herc-as-front-desa.atica.um.es/docs/apioaipmh.tar.gz
 - [UrisFactory](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/UrisFactory "UrisFactory") - Servicio que genera las uris de los recursos: http://herc-as-front-desa.atica.um.es/docs/apiuris.tar.gz
 - [cvn](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/cvn) - Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH: http://herc-as-front-desa.atica.um.es/docs/apicvn.tar.gz
 - [CronConfigure](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/CronConfigure) - Servicio Web que realiza la creación de tareas para la sincronización de un repositorio: http://herc-as-front-desa.atica.um.es/docs/apicron.tar.gz
 - PostgreSQL - Imagen de PostgreSQL preparada para funcionar con el backend: http://herc-as-front-desa.atica.um.es/docs/herculessql.tar.gz
 
## Despliegue de Virtuoso con docker-compose

Para desdeplegar Virtuoso con docker-compose necesitamos un docker-compose.yml con el siguiete contenido. Sería recomendable ajustar el DBA_PASSWORD. El resto de variables dependerá de las caractísticas de nuesta infraestructura.

	version: "3"
	services:
	    virtuoso:
		container_name:
		    virtuoso
		image:
		    openlink/virtuoso-opensource-7:latest
		environment:
		    DBA_PASSWORD: mysecret      
		    VIRTUOSO_INI_FILE:            
		    VIRT_Parameters_NumberOfBuffers: 100000
		    VIRT_Parameters_MaxDirtyBuffers: 60000
		    VIRT_Parameters_MaxClientConnections: 100
		    VIRT_HTTPServer_MaxClientConnections: 50
		    VIRT_HTTPServer_ServerIdString: "virtuoso"
		    VIRT_Zero Config_ServerName: "virtuoso"
		    VIRT_I18N_XAnyNormalization: 3
		ports:
		    - "1111:1111"
		    - "8890:8890"
		volumes:
		    - /var/container-data/virtuoso/db:/database   
		user: ${CURRENT_UID}
		restart: unless-stopped

Para levantar Virtuoso ejecutaremos este comando en la misma ruta donde tengamos el docker-compose.yml:
	
	docker-compose up -d
	
Una vez creado el Con esto ya tendríamos un Virtuoso operativo en nuestro entorno. Podemos probar que efectivamente está funcionando correctamente accediiendo a http://localhost:8890, nos debería paracer la con

## PostgreSQL

Para PostgreSQL necesitamos importar la imagen que obtenemos en el enlace http://herc-as-front-desa.atica.um.es/docs/herculessql.tar.gz. Una vez descargada la importamos en el nodo de docker con el siguiente comando:
	docker load < herculessql.tar.gz
	
Una vez importada la ejecutamos con este comando:
	docker run -p 5432:5432 --name herculessql herculessql
	
Así obtenemos una base de datos lista para que las APIs del backend puedan usarla.

Despliegue DOCKER / DOCKER-COMPOSE
----------------------------------

Una vez que tengamos las imágenes descargadas, tenemos que importarlas como imágenes docker con este comando: 

	docker load < {nombre-imagen}.tar.gz
 
Cuando las tengamos importadas las desplegaremos con docker-compose, creando un archivo docker-compose.yml. Hay que tener en cuenta que los docker-compose.yml deben estar en ubicaciones separadas ya que tienen el mismo nombre (docker-compose.yml) y, además, respetar el formato yaml, ya que si hay tabulaciones no funcionará, aunque lanza errores bastante claros cuando ocurre esto. 

Indicamos a continuación el contenido del primer compose, que va a contener varios servicios. En cada uno hemos adaptado las variables de entorno (enviroment:) a nuestras necesidades y definido en un segundo bloque los puertos (ports). El segundo indica el que utiliza internamente cada api en docker y el primero es el que se levanta externamente, que podemos adaptar segun nuestras necesidades.

	version: '3'
	
	services:
	  apicarga:
	    image: apicarga
	    ports:
	      - 5100:5100
		environment:
		  PostgreConnection: "Username=herculesdb;Password=NUuPIsrUV4x3o6sZEqE8;Host=155.54.239.203;Port=5432;Database=herculesdb;Pooling=true"
	      PostgreConnectionmigration: "Username=herculesdb;Password=NUuPIsrUV4x3o6sZEqE8;Host=155.54.239.203;Port=5432;Database=herculesdb;Pooling=true"
		  ConfigUrl: "http://herc-as-front-desa.atica.um.es/carga/"
		  Graph: "http://graph.um.es/graph/um_cvn"
	      Endpoint: "http://155.54.239.204:8890/sparql"
	      QueryParam: "query"
		  
	  apifrontcarga:
	    image: apifrontcarga
	    ports:
	      - 5103:5103
		environment:
	      ConfigUrl: "http://herc-as-front-desa.atica.um.es/carga/"
		  ConfigUrlCron: "http://herc-as-front-desa.atica.um.es/cron-config/"
		  
	  apicron:
	    image: apicron
	    ports:
	      - 5107:5107
	    environment:
		  HangfireConnection: "Username=herculesdb;Password=NUuPIsrUV4x3o6sZEqE8;Host=155.54.239.203;Port=5432;Database=herculesdb;Pooling=true"
		  ConfigUrl: "http://herc-as-front-desa.atica.um.es/carga/"
	  
	  apiuris:
	    image: apiuris
	    ports:
	      - 5000:5000
	  
Del mismo modo, el segundo compose sería:

	version: '3'
	
	services:
	  apicvn:
	    image: apicvn
	    ports:
	      - 5104:5104
		  
	  apioaipmh:
	    image: apioaipmh
	    ports:
	      - 5102:5102
		environment:
	      XML_CVN_Repository: "http://curriculumpruebas.um.es/curriculum/rest/v1/auth/"
	      CVN_ROH_converter: "http://herc-as-front-desa.atica.um.es/cvn/v1/convert"
	      ConfigUrl: "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH"

Los despliegues se realizan ejecuntando el siguiente comando, en la misma ubicacion donde se encuentre el docker-compose.yml:

docker-compose up -d

