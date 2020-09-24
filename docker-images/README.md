![](..//Docs/media/CabeceraDocumentosMD.png)

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

* Gestor de colas (en este ejemplo RabbitMQ desplegada con Docker)
 
## Despliegue de Virtuoso con docker-compose

## PostgreSQL

## RabbitMQ

## Preparación de Trifid

Para poner en marcha el servicio de linked data debemos decargar este paquete http://herc-as-front-desa.atica.um.es/docs/trifid.tar.gz y descomprimirlo. Una vez descomprimido tenemos que abrir el archivo config-custom.json e indicar el interfaz SPARQL de nuestro Virtuoso y el baseurl donde vaya a responder el servicio y el puerto: 

	{
 		"baseConfig": "trifid:config-sparql.json", // inherit the default sparql config
  		"sparqlEndpointUrl": "http://localhost:8890/sparql", // overrides SPARQL endpoint
  		"datasetBaseUrl": "http://graph.um.es/", // enables "proxy" mode.
  		"listener": {
   		"port": 8081
  		}
	}

Una vez ajustados los parametros tenemos que construir la imagen con el siguiente comando:
	
	docker build -t trifid .
	
Con la imagen ya contruida la ponemos en marcha con este comando:

	docker run -d -p 8081:8081 --name trifid trifid

Ahora solamente nos faltaría añadir esta configuración a Apache:

	<VirtualHost *:80>
	
		ServerName graph.um.es
	
		ProxyPreserveHost On
		ProxyPass / http://127.0.0.1:8081/
		ProxyPassReverse / http://127.0.0.1:8081/
		Timeout 5400
		ProxyTimeout 5400
	
		<Proxy *>
			Order deny,allow
			Allow from all
			Require all granted
		</Proxy>
	
	</VirtualHost>

## Preparación de Apache

Necesitamos preparar Apache como proxy invesro y poder acceder a las APIs a través del dominio que vayamos a utilizar y luego este redirija al puerto específico de cada una de ellas.

Para que funcione correctamente debemos ajustar el ServerName con el dominio que vayamos a utilizar (en este ejemplo mihercules.com) y añadir los parametros del proxy inverso para que Apache redirija las peticiones al API adecuda. Estos parametros los podemos ver en el final de este archivo de ejemplo http://herc-as-front-desa.atica.um.es/docs/httpd.conf.

## Descarga de imágenes necesarias

Este es el listado de imágenes docker de las aplicaciones incluidas en GnossDeustoBackend:

 - PostgreSQL - Imagen de PostgreSQL preparada para funcionar con el backend: http://herc-as-front-desa.atica.um.es/docs/herculessql.tar.gz
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

Una vez que tengamos las imágenes descargadas, tenemos que importarlas como imágenes docker con este comando: 

	docker load < {nombre-imagen}.tar.gz
 
Cuando las tengamos importadas las desplegaremos con docker-compose, creando un archivo docker-compose.yml. Podemos elegir la ruta que mas nos convenga (por ejemplo ~\apis). En este ejemplo podemos ver los ajustes de las variables dependiendo de nuestro entorno:

	version: '3'

        services:
          apicarga:
            image: apicarga
            ports:
              - 5100:5100
            environment:
              PostgreConnection: "Username=docker;Password=docker;Host=localhost;Port=5432;Database=docker;Pooling=true"
              PostgreConnectionmigration: "Username=docker;Password=docker;Host=localhost;Port=5432;Database=docker;Pooling=true"
              ConfigUrl: "http://mihercules.com/carga/"
              Graph: "http://graph.um.es/graph/um_cvn"
              GraphUnidata: "http://data.um.es/graph/unidata"
              Endpoint: "http://155.54.239.204:8890/sparql"
              QueryParam: "query"
              GraphRoh: "http://graph.um.es/graph/research/roh"
              GraphRohes: "http://graph.um.es/graph/research/rohes"
              GraphRohum: "http://graph.um.es/graph/research/rohum"
              Authority: "http://mihercules.com:5108"
              ScopeCarga: "apiCarga"
              AuthorityGetToken: "http://mihercules.com:5108/connect/token"
              GrantType: "client_credentials"
              ClientId: "carga"
              ClientSecret: "secret"
              ScopeOAIPMH: "apiOAIPMH"
              ClientIdOAIPMH: "OAIPMH"
              ClientSecretOAIPMH: "secretOAIPMH"
              ConfigUrlUnidata: "http://mihercules.com/unidata"
              ScopeUnidata: "apiUnidata"
              ClientIdUnidata: "unidata"
              ClientSecretUnidata: "secretUnidata"
            volumes:
              - /path_to_logs/logs/apicarga:/app/logs
          
          apifrontcarga:
            image: apifrontcarga
            ports:
              - 5103:5103
            environment:
              ConfigUrl: "http://mihercules.com/carga/"
              ConfigUrlCron: "http://mihercules.com/cron-config/"
              ConfigUrlUrisFactory: "http://mihercules.com/uris/"
              Authority: "http://mihercules.com:5108/connect/token"
              GrantType: "client_credentials"
              Scope: "apiCarga"
              ScopeCron: "apiCron"
              ScopeUrisFactory: "apiUrisFactory"
              ClientId: "Web"
              ClientSecret: "master"
              ScopeOAIPMH: "apiOAIPMH"
              ClientIdOAIPMH: "OAIPMH"
              ClientSecretOAIPMH: "secretOAIPMH"
            volumes:
               - /path_to_logs/logs/apifrontcarga:/app/logs
              
          apicron:
            image: apicron
            ports:
              - 5107:5107
            environment:
              HangfireConnection: "Username=docker;Password=docker;Host=localhost;Port=5432;Database=docker;Pooling=true" 
              ConfigUrl: "http://mihercules.com/carga/"
              Authority: "http://mihercules.com:5108"
              AuthorityGetToken: "http://mihercules.com:5108/connect/token"
              Scope: "apiCron"
              ScopeCarga: "apiCarga"
              GrantType: "client_credentials"
              ClientId: "carga"
              ClientSecret: "secret"
            volumes:
              - /path_to_logs/logs/apicron:/app/logs
          
          apiuris:
            image: apiuris
            ports:
              - 5000:5000
            environment:
              Authority: "http://mihercules.com:5108"
              Scope: "apiUrisFactory"
            volumes:
              - /path_to_logs/logs/apiuris:/app/logs

          apiidentity:
            image: apiidentity
            ports: 
              - 5108:5108
            environment:
              PostgreConnection: "Username=docker;Password=docker;Host=localhost;Port=5432;Database=docker;Pooling=true"
            volumes:
              - /path_to_logs/logs/apiidentity:/app/logs
              
          apiunidata:
            image: apiunidata
            ports:
              - 5106:5106
            environment:
              GraphUnidata: "http://data.um.es/graph/unidata"
              EndpointUnidata: "http://155.54.239.204:8890/sparql"
              Authority: "http://mihercules.com:5108"
              AuthorityGetToken: "http://mihercules.com:5108/connect/token"
            volumes:
               - /path_to_logs/logs/apiunidata:/app/logs
               
          apicvn:
            image: apicvn
            ports:
              - 5104:5104
  
          apioaipmh:
            image: apioaipmh
            ports:
              - 5102:80
            environment:
              XML_CVN_Repository: "http://curriculumpruebas.um.es/curriculum/rest/v1/auth/"
              CVN_ROH_converter: "http://mihercules.com/cvn/v1/convert"
              ConfigUrl: "http://mihercules.com/oai-pmh-cvn/OAI_PMH"
              Authority: "http://mihercules.com:5108"
              Scope: "apiOAIPMH"
          
          apibridge:
            image: apibridge
            ports:
              - 5200:5200
          
          briggeswagger:
            image: bridgeswagger
            ports:
              - 8082:8080

            
Para lanzar las APIs usamos este comando como en el caso de Virtuoso:

	docker-compose up -d

## Preparación del interfaz Fuseki para benchmark

Para poder utilizar benchmark necesitamos almacenar los datos sparql en Fuseki. Para desplegar Fuseki facilmente lo podemos hacer por medio de este comando Docker:

	docker run -d -p 3030:3030 stain/jena-fuseki fuseki

Durante el despliegue nos generará un usuario y contraseña que debemos apuntar.

Cuando tengamos Fuseki operativo podemos entrar a la interfaz gráfica y cargarle información. Craremos un data set y cargaremos los achivos ttl y nq que se encuentran en estas urls:

- https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/criterion-ontology/src
- https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/triplestore-dataset/static/data
- https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/triplestore-assessments



