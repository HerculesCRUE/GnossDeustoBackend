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

* Servidor de Liked Data (en este ejemplo Trifid desplegada con Docker)

* Acceso a http://curriculumpruebas.um.es/curriculum/rest/v1/auth desde la máquina donde vayamos a desplegar.
 
## Despliegue de Virtuoso

Como base de datos de triples vamos a utilizar Virtuoso, para desplegarlo, obtendremos el fichero yml con todas las configuraciones necesarias para su despliegue, desde este ficheor podemos realizar varios ajustos para la configuración del contenedor y virtuoso las más importantes son las siguientes:
* DBA_PASSWORD: mysecret - Ajusta la clave para el usuario dba.
* VIRT_Parameters_NumberOfBuffers: 100000 - Nivel de buffer ajustado para 1 GB de RAM, para más RAM se incrementaria proporcionalmente.
* VIRT_Parameters_MaxDirtyBuffers: 60000 - Nivel de buffer ajustado para 1 GB de RAM, para más RAM se incrementaria proporcionalmente.
* VIRT_Parameters_MaxClientConnections: 100 - Máximo de conexiones por el puerto 1111.
* VIRT_HTTPServer_MaxClientConnections: 50 - Máximo de conexiones por el puerto 8890.

Partiendo desde la home del usurio (ej. /home/usuario/) creamos el directorio que va a contener el docker-compose.yml, entramos en el directorio, descargamos el yml y levantamos el docker con los siguientes comandos: 

	mkdir virtuoso
	cd virtuoso
	wget http://herc-as-front-desa.atica.um.es/docs/docker-virtuoso/docker-compose.yml
	docker-compose up -d
	
Un vez deplegado podemos ver el proceso de docker con este comando:

	docker ps
	
![](http://herc-as-front-desa.atica.um.es/docs/capturas/virtuoso/01_docker_ps.png)

Y podemos hacer una sencilla comprobación de que funciona entrando en la interfaz web con http://ip_de_nuestra_maquina:8890

![](http://herc-as-front-desa.atica.um.es/docs/capturas/virtuoso/02_web.png)

	
## Despliegue de PostgreSQL

El procedimiento para desplegar PostgreSQL es similar al de virtuso. Utilizaremos docker-compose con su respectivo yml. En esta plantilla no es necesario ajustar nada aunque podemos ajustar el password que queramos en el parámetro "POSTGRES_PASSWORD" del docker-compose.yml, cosa que tenemos que tener en cuenta a la hora de ajustar el yml de los servicios que veremos más adelante.

Partiendo desde la home del usurio (ej. /home/usuario/) creamos el directorio que va a conetener el docker-compose.yml, entramos en el directorio, descargamos el yml y levantamos el docker con los siguientes comandos: 

	mkdir postgresql
	cd postgresql
	wget http://herc-as-front-desa.atica.um.es/docs/docker-postgresql/docker-compose.yml
	docker-compose up -d
	
Depués de desplegar, como en el caso anterior vamos a hacer la comprobación de que el contenedor está levantado pero en esta ocasión vamos a usar el comando docker-compose ps que se limita a mostrar información solo de los procesos de este yml.
	
	docker-compose ps
	
![](http://herc-as-front-desa.atica.um.es/docs/capturas/postgre/04_docker_ps.png)

	
## Despliegue de RabbitMQ

RabbitMQ lo desplegaremos con la misma mecánica que en los casos anteriores.

Partiendo desde la home del usurio (ej. /home/usuario/) creamos el directorio que va a conetener el docker-compose.yml, entramos en el directorio, descargamos el yml y levantamos el docker con los siguientes comandos: 
	
	mkdir rabbitmq
	cd rabbitmq
	wget http://herc-as-front-desa.atica.um.es/docs/docker-rabbitmq/docker-compose.yml
	docker-compose up -d
	
Una vez levantado podemos hacer la comprobación de que esta el contenedor levantado con este comando:

	docker-compose ps

![](http://herc-as-front-desa.atica.um.es/docs/capturas/rabbitmq/00_rabbitq_docker_ps.png)

Y podemos probar a cargar el interfaz web de rabbitmq con http://ip_de_nuestra_maquina:15672 y ver como nos sale la pantalla de login.

![](http://herc-as-front-desa.atica.um.es/docs/capturas/rabbitmq/01_rabbitmq_login.png)

Ahora debemos loguearnos con usurio "guest" y password "password", que son los que estan ajustados en el yml, y procederemos a crear un virtual host seguiendo estos sencillos pasos:

Ya logueados vamos a la sección "Admin".

![](http://herc-as-front-desa.atica.um.es/docs/docker-rabbitmq/rmq/2.png)

Una vez logueados pinchamos en "Virtual Hosts".

![](http://herc-as-front-desa.atica.um.es/docs/docker-rabbitmq/rmq/3.png)

Escribimos el nombre del virtual host. En nuestro caso "hercules" porque es el que está ajustado en el docker-compose.yml de servicios. Después pinchamos en "Add virtual host".

![](http://herc-as-front-desa.atica.um.es/docs/docker-rabbitmq/rmq/4.png)

Una vez añadido entramos en sus ajustes.

![](http://herc-as-front-desa.atica.um.es/docs/docker-rabbitmq/rmq/5.png)

Le damos permisos al usuario guest. En nuestro caso "guest" porque es el que está ajustado en el docker-compose.yml de servicios.

![](http://herc-as-front-desa.atica.um.es/docs/docker-rabbitmq/rmq/6.png)

Y vemos como han aplicado correctamente estos permisos.

![](http://herc-as-front-desa.atica.um.es/docs/docker-rabbitmq/rmq/7.png)

Ya tenemos RabbitMQ listo para trabajar en nuestro entorno.




## Despliegue de los servicios

Este es el listado de imágenes docker de las aplicaciones incluidas en GnossDeustoBackend:

- [FrontEndCarga](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/FrontEndCarga "FrontEndCarga") - Interfaz web para la parte de Repository y Validation del API_CARGA: http://herc-as-front-desa.atica.um.es/docs/apifrontcarga.tar.gz
 - [UrisFactory](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/UrisFactory "UrisFactory") - Servicio que genera las uris de los recursos: http://herc-as-front-desa.atica.um.es/docs/apiuris.tar.gz
 - [API_CARGA](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_CARGA "API_CARGA") - Servicio web que realiza las tareas de carga/configuración: http://herc-as-front-desa.atica.um.es/docs/apicarga.tar.gz
 - [API_IDENTITY](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/IdentityServerHecules "API_IDENTITY") - 
API que proporciona los tokens de acceso para las diferntes APIs securizadas: http://herc-as-front-desa.atica.um.es/docs/apiidentity.tar.gz
 - [API_UNIDATA](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Unidata/Api_Unidata "API_UNIDATA") - 
API que publica los RDF en el nodo central unidata: http://herc-as-front-desa.atica.um.es/docs/apiunidata.tar.gz
 - [CronConfigure](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/CronConfigure) - Servicio Web que realiza la creación de tareas para la sincronización de un repositorio: http://herc-as-front-desa.atica.um.es/docs/apicron.tar.gz
 - [OAI_PMH_CVN](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/OAI_PMH_CVN) - Servicio OAI-PMH para la obtención de invstigadores de la Universidad de Murcia: http://herc-as-front-desa.atica.um.es/docs/apioaipmh.tar.gz
 - [OAI_PMH_XML](https://github.com/HerculesCRUE/oai-pmh) - Servicio OAI-PMH para la obtención de invstigadores de la Universidad de Murcia: http://herc-as-front-desa.atica.um.es/docs/apioaipmhxml.tar.gz
 - [cvn](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/cvn) - Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH: http://herc-as-front-desa.atica.um.es/docs/apicvn.tar.gz
- [Bridge](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/fair/bridge) - Bridge para métricas fair: http://herc-as-front-desa.atica.um.es/docs/apibridge.tar.gz
- [BrideSwagger](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/fair/bridge) - Interfaz swagger para el bridge: http://herc-as-front-desa.atica.um.es/docs/bridgeswagger.tar.gz
- [GestorDocumentacion](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/GestorDocumentacion) - Interfaz para la gestion de la documentación: http://herc-as-front-desa.atica.um.es/docs/apigesdoc.tar.gz
- [API_DISCOVER](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_DISCOVER) - Aplica el descubrimiento de los RDFs y los publica en virtuoso.
: http://herc-as-front-desa.atica.um.es/docs/apidiscover.tar.gz
- [Linked_Data_Server](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Linked_Data_Server) - Aplica el descubrimiento de los RDFs y los publica en virtuoso.
: http://herc-as-front-desa.atica.um.es/docs/apidiscover.tar.gz


Para simplificar el despliegue de estos servicios tenemos que hacer un directorio en el home del usuario que se llame por ejemplo "Servicios" yluego entramos en el.

	mkdir servicios
	cd servicios

Una vez en el directorio "Servicios" nos descargamos el script que descarga las imágenes para posteriormente cargarlas en Docker.
	
	wget http://herc-as-front-desa.atica.um.es/docs/docker-servicios/carga_imagenes.sh
	
Le damos permisos de ejecución y lo ejecutamos:

	chmod +x carga_imagenes.sh
	./carga_imagenes.sh

Una vez cargadas las imágenes en este mismo directorio nos bajamos el yml de los servicios.
	
	wget http://herc-as-front-desa.atica.um.es/docs/docker-servicios/docker-compose.yml
	
Antes de lentar los servicios debemos editar este archivo y reemplezar "ip_de_nuestra_maquina" por la ip de la máquina donde estemos levantando los servicios. Asi todos los servicios se podran comunicar conrrectamente entre ellos.	

Con la ip ajustada ya podemos deplegar el docker-compose como de costumbre con este comando:

	docker-compose up -d

Ahora si accedemos a http://ip_de_nuestra_maquina:5103 podemos ver el interfaz web para poder hacer cargas.

![](http://herc-as-front-desa.atica.um.es/docs/capturas/front.png)
