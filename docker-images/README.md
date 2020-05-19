## Sobre docker-images
Listado de imagenes docker de todas las aplicaciones incluidas en GnossDeustoBackend

 - [API_CARGA](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_CARGA "API_CARGA") - Servicio web que realiza las tareas de carga/configuración: http://herc-as-front-desa.atica.um.es/docs/apicarga.tar.gz
 - [FrontEndCarga](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/FrontEndCarga "FrontEndCarga") - Interfaz web para la parte de Repository y Validation del API_CARGA: http://herc-as-front-desa.atica.um.es/docs/apifrontcarga.tar.gz
 - [OAI_PMH_CVN](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/OAI_PMH_CVN "OAI_PMH_CVN") - Servicio OAI-PMH para la obtención de invstigadores de la Universidad de Murcia: http://herc-as-front-desa.atica.um.es/docs/apioaipmh.tar.gz
 - [UrisFactory](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/UrisFactory "UrisFactory") - Servicio que genera las uris de los recursos: http://herc-as-front-desa.atica.um.es/docs/apiuris.tar.gz
 - [cvn](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/cvn) - Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH: http://herc-as-front-desa.atica.um.es/docs/apicvn.tar.gz
 - [CronConfigure](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/CronConfigure) - Servicio Web que realiza la creación de tareas para la sincronización de un repositorio: http://herc-as-front-desa.atica.um.es/docs/apicron.tar.gz

Despliegue DOCKER / DOCKER-COMPOSE
----------------------------------

Una vez que tengamos las imágenes tenemos descargadas en el nodo de Docker, tenemos que importarlas como imágenes docker con este comando: 

docker load < imagen.tar

Cuando las tengamos importadas las desplegaremos con docker-compose creando un archivo docker-compose.yml. Lo único que debemos tener en cuenta es que los docker-compose.yml deben estar en ubicaciones separadas ya que se llaman igual (docker-compose.yml) y deber respetar el formato yaml, ya que si hay tabulaciones no funcionará aunque lanza errores bastante claros cuando ocurre esto. 

El primero de ellos va a contener lo siguiente, adaptando las variables de entorno (enviroment:) a nuestras necesidades. El segundo bloque de los ports es el puerto que utiliza inernamente la api en docker y el primero en el que se levanta externamente y con este podemos jugar segun nuestras necesidades.

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
	  
  uris:
    image: uris
    ports:
      - 5000:5000
	  
El segundo compose sería el siguiente, también adaptando las variables a nuestras necesidades:

version: '3'

services:
  apicvn:
    image: apicvn
    ports:
      - 5104:5104
	  
  apioaipmh:
    image: oaipmh
    ports:
      - 5102:5102
	environment:
      XML_CVN_Repository: "http://curriculumpruebas.um.es/curriculum/rest/v1/auth/"
      CVN_ROH_converter: "http://herc-as-front-desa.atica.um.es/cvn/v1/convert"
      ConfigUrl: "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH"

Los despliegues se realizan con el siguiente comando en la misma ubicacion donde se encuentre el docker-compose.yml:

docker-compose up -d

