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

## Preparación de Apache

Para que el Linked Data Server funcione de manera adecuada tenemos que preparar un archivo de configuracion de Apache con estos datos. Esta configuración hace basicamente que lo que se pida por http se re dirija al servidor de Linked Data Server que en este caso estaria en la misma máquina y en su puerto establecido "8081"

	<VirtualHost *:80>
    		ServerName linkeddata2test.um.es
		DocumentRoot "/var/www/html"
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

El resto de peticiones se harán por https y bastaria con editar el ssl.conf y editar o añadir estas líneas:

	ServerName linkeddata2test.um.es:443

	#APIFRONTCARGA
	ProxyPass /carga-web http://ip_del_servicio:5103
	ProxyPassReverse /carga-web http://ip_del_servicio:5103

	#APICARGA
	ProxyPass /carga http://ip_del_servicio:5100
	ProxyPassReverse /carga http://ip_del_servicio:5100

	#BENCHMARK
	ProxyPass /benchmark http://ip_del_servicio:8401
	ProxyPassReverse /benchmark http://ip_del_servicio:8401

	#OAI-PMH-CVN
	ProxyPass /oai-pmh-cvn http://ip_del_servicio:5102
	ProxyPassReverse /oai-pmh-cvn http://ip_del_servicio:5102

	#CRON
	ProxyPass /cron-config http://ip_del_servicio:5107
	ProxyPassReverse /cron-config http://ip_del_servicio:5107

	#DOCUMENTACION
	ProxyPass /documentacion http://ip_del_servicio:5109
	ProxyPassReverse /documentacion http://ip_del_servicio:5109

	#IDENTITY-SERVER
	ProxyPass /identityserver http://ip_del_servicio:5108
	ProxyPassReverse /identityserver http://ip_del_servicio:5108

	#APIURIS
	ProxyPass /uris http://ip_del_servicio:5000
	ProxyPassReverse /uris http://ip_del_servicio:5000

	#XMLRDFCONVERSOR
	ProxyPass /conversor_xml_rdf http://ip_del_servicio:5114
	ProxyPassReverse /conversor_xml_rdf http://ip_del_servicio:5114

	#UNIDATA
	ProxyPass /unidata http://ip_del_servicio:5106
	ProxyPassReverse /unidata http://ip_del_servicio:5106

	#CVN
	ProxyPass /cvn http://ip_del_servicio:5104
	ProxyPassReverse /cvn http://ip_del_servicio:5104
	ProxyPass /cvn_swagger http://ip_del_servicio:8080
	ProxyPassReverse /cvn_swagger http://ip_del_servicio:8080  

	#BRIDGE
	ProxyPass /fairmetrics_bridge http://ip_del_servicio:5200
	ProxyPassReverse /fairmetrics_bridge http://ip_del_servicio:5200
	ProxyPass /bridgeswagger http://ip_del_servicio:8082
	ProxyPassReverse /bridgeswagger http://ip_del_servicio:8082

	#VIRTUOSO1
	ProxyPass /sparql http://ip_del_servicio:8890/sparql
	ProxyPassReverse /sparql http://ip_del_servicio:8890/sparql

	#VIRTUOSO2
	ProxyPass /sparql2 http://ip_del_servicio:8890/sparql
	ProxyPassReverse /sparql2 http://ip_del_servicio:8890/sparql

Por último, para que la aplicación disponga de los archivos necesarios tenemos que meter estos estilos en la capeta publica de Apache.

	wget http://herc-as-front-desa.atica.um.es/docs/contenido.tar.gz

## Despliegue de los servicios

Para simplificar el despliegue de los servicios, hemos creado un script que debemos descargar en nuestra máquina. Partiendo desde la home del usurio (ej. /home/usuario/).

	wget http://herc-as-front-desa.atica.um.es/docs/docker-servicios/actualizar.sh
	
Este escript clonará los repositorios necesarios y luego generará las imágenes docker automáticamente. Le debemos dar permisos de ejecución.

	chmod +x actualizar.sh

Depués creamos el directorio donde vamos a alojar el docker-compose.yml que va orquestar todos los servicios. Lo hemos llamado dock1 porque en el script actualizar.sh así se llama papero podemos jugar con estos valores. Después lo descargamos.

	mkdir dock1
	cd dock1
	wget http://herc-as-front-desa.atica.um.es/docs/docker-servicios/docker-compose.yml
	
Antes de lentar los servicios debemos editar este archivo y reemplezar "herc-as-front-desa.atica.um.es" por la ip de la máquina donde estemos levantando los servicios. Asi todos los servicios se podran comunicar conrrectamente entre ellos.	

Con la ip ajustada ya podemos ejecutar script que nos prepara el entorno.

	./actualizar.sh
	
Cuando accedamos por primera vez el frontal web nos debería fallar porque no tenemos las vistas personalizadas cargadas en la base de datos. Para conseguir esto tenemos que hacer estos sencillos pasos:

Primero nos vajamos un script sql con los insert necesarios desde la máquina donde tenemos PostgreSQL instalado.

	wget http://herc-as-front-desa.atica.um.es/docs/vistas.sql
	
Ahora tenemos que modificar los inserts ajustando los enlaces http y https y poner los adecuados para nuestro entorno.

Una vez modificado el script tenemos que ejecutar estos comandos:

	docker exec -it postgresql_db_1 bash

	su postgres

	psql -f vistas.sql

Si todo ha ido bien veremos el recuento de los inserts con este formato:

	INSERT 0 14

Ahora si accedemos a http://ip_de_nuestra_maquina:5103 podemos ver el interfaz web para poder hacer cargas.

![](http://herc-as-front-desa.atica.um.es/docs/capturas/front.png)
