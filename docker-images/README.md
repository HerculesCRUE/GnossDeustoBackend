# Despliegue del backend con Docker

## Requisitos previos
Para hacer funcionar el backend será necesario tener instalado en nuestro servidor:
* Apache configurado como proxy inverso

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
 - [UrisFactory](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/UrisFactory "UrisFactory") - Servicio que genera las uris de los recursos: http://herc-as-front-desa.atica.um.es/docs/apiuris.tar.gz
 - [API_CARGA](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_CARGA "API_CARGA") - Servicio web que realiza las tareas de carga/configuración: http://herc-as-front-desa.atica.um.es/docs/apicarga.tar.gz
 - [FrontEndCarga](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/FrontEndCarga "FrontEndCarga") - Interfaz web para la parte de Repository y Validation del API_CARGA: http://herc-as-front-desa.atica.um.es/docs/apifrontcarga.tar.gz
 - [CronConfigure](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/CronConfigure) - Servicio Web que realiza la creación de tareas para la sincronización de un repositorio: http://herc-as-front-desa.atica.um.es/docs/apicron.tar.gz
 - [OAI_PMH_CVN](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/OAI_PMH_CVN "OAI_PMH_CVN") - Servicio OAI-PMH para la obtención de invstigadores de la Universidad de Murcia: http://herc-as-front-desa.atica.um.es/docs/apioaipmh.tar.gz
 - [cvn](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/cvn) - Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH: http://herc-as-front-desa.atica.um.es/docs/apicvn.tar.gz
- [Bridge](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/fair/bridge) - Servicio Web que realiza la creación de tareas para la sincronización de un repositorio: http://herc-as-front-desa.atica.um.es/docs/apibridge.tar.gz
- [BrideSwagger](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/fair/bridge) - Servicio Web que realiza la creación de tareas para la sincronización de un repositorio: http://herc-as-front-desa.atica.um.es/docs/bridgeswagger.tar.gz

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
	
Despues de ejecutar el comando ya tendríamos un servidor Virtuoso operativo en nuestro entorno. Podemos probar que efectivamente está funcionando correctamente accediiendo a http://localhost:8890, nos debería paracer la consola de administración de Virtuoso.

## PostgreSQL

Para PostgreSQL necesitamos importar la imagen que obtenemos en el enlace http://herc-as-front-desa.atica.um.es/docs/herculessql.tar.gz. Una vez descargada la importamos en el nodo de docker con el siguiente comando:
	
	docker load < herculessql.tar.gz
	
Una vez importada la ejecutamos con este comando:
	
	docker run -p 5432:5432 --name herculessql herculessql
	
Así obtenemos una base de datos lista para que las APIs del backend puedan usarla.

## Preparación de Apache

Necesitamos preparar Apache como proxy invesro y poder acceder a las APIs a través del dominio que vayamos a utilizar y luego este redirija al puerto específico de cada una de ellas.

Para que funcione correctamente debemos ajustar el ServerName con el dominio que vayamos a utilizar (en este emplo mihercules.com) y añadir los parametros del proxy inverso para que Apache redirija las peticiones al API adecuda. Estos parametros los podemos ver en el final de este archivo de ejemplo http://herc-as-front-desa.atica.um.es/docs/httpd.conf.


Despliegue de las APIs
----------------------------------

Una vez que tengamos las imágenes descargadas, tenemos que importarlas como imágenes docker con este comando: 

	docker load < {nombre-imagen}.tar.gz
 
Cuando las tengamos importadas las desplegaremos con docker-compose, creando un archivo docker-compose.yml. En este ejemplo podemos ver los ajustes de las variables dependiendo de nuestro entorno:

	version: '3'
	
	services:
	  apicarga:
	    image: apicarga
	    ports:
	      - 5100:5100
	    environment:
	      PostgreConnection: "Username=docker;Password=docker;Host=127.0.0.1;Port=5432;Pooling=true"
	      PostgreConnectionmigration: "Username=docker;Password=docker;Host=127.0.0.1;Port=5432;Pooling=true"
	      ConfigUrl: "http://mihercules.com/carga/"
	      Graph: "http://graph.um.es/graph/um_cvn"
	      GraphUnidata: "http://data.um.es/graph/unidata"
	      Endpoint: "http://localhost:8890/sparql"
	      QueryParam: "query"
		  
	  apifrontcarga:
	    image: apifrontcarga
	    ports:
	      - 5103:5103
	    environment:
	      ConfigUrl: "http://mihercules.com/carga/"
	      ConfigUrlCron: "http://mihercules.comcron-config/"
		  
	  apicron:
	    image: apicron
	    ports:
	      - 5107:5107
	    environment:
	      HangfireConnection: "Username=docker;Password=docker;Host=127.0.0.1;Port=5432;Pooling=true"
	      ConfigUrl: "http://mihercules.com/carga/"
	  
	  apiuris:
	    image: apiuris
	    ports:
	      - 5000:5000
	  
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
	      CVN_ROH_converter: "http://mihercules.com/cvn/v1/convert"
	      ConfigUrl: "http://mihercules.com/oai-pmh-cvn/OAI_PMH"

Para lanzar las APIs usamos este comando como en el caso de Virtuoso:

	docker-compose up -d

