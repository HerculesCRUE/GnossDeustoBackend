![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 14/4/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Despliegue de ASIO Backend de SGI con Docker| 
|Descripción|Instrucciones para instalar ASIO mediante el despliegue de instancias docker|
|Versión|1.1|
|Módulo|Documentación|
|Tipo|Manual|
|Cambios de la Versión|Anadida la cabecera|

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

Con esta configuración básica tendremos un interfaz sparql que se lanzará de manera anónima y con permisos exclusivamente de lectura. Para poder tener un interfaz sparql adicional con el que podamos hacer modificación de datos, lo primero que debemos hacer es acceder al interfaz isql de virtuoso entrando al docker.

	docker exec -it virtuoso bash
	cd ../bin/
	isql 1111 dba mysecret

Una vez aquí podremos ver esto:

![](http://herc-as-front-desa.atica.um.es/docs/capturas/virtuoso/isql.png)

Con estos comando creamos el usuario "UPDATE", le damos permisos, ajustamos lectura para nobody y modificar para "UPDATE"
	
	DB.DBA.USER_CREATE ('UPDATE', 'Bn4wQ6aD');
	grant SPARQL_SELECT to "UPDATE";
	grant SPARQL_UPDATE to "UPDATE";
	grant SPARQL_SPONGE to "UPDATE";	
	DB.DBA.RDF_DEFAULT_USER_PERMS_SET ('nobody', 1);
	DB.DBA.RDF_DEFAULT_USER_PERMS_SET ('UPDATE', 3);
	
Ahora solamente necesitamos añadir un interfaz que sea autenticado y ejecutado por UPDATE con el que se puedan hacer modificaciones. Para ello accedemos a http://ip_de_nuestra_maquina:8890/conductor y nos logueamos con el usuario dba (en esta guía dba / mysecret). Seguido vamos a la sección indicada en la captura:

![](http://herc-as-front-desa.atica.um.es/docs/capturas/virtuoso/breadcrumb.png)

Una vez ahí desplegamos el interfaz 0.0.0.0:8890 y buscamos el /sparql-auth

![](http://herc-as-front-desa.atica.um.es/docs/capturas/virtuoso/sparql-auth0.png)

Y lo editamos para dejarlos de la siguiente manera (con modificar el Realm y poner UPDATE sería suficiente):

![](http://herc-as-front-desa.atica.um.es/docs/capturas/virtuoso/sparql-auth.png)

Ahora si vamos a http://ip_de_nuestra_maquina:8890/sparql-auth y nos autenticamos con el usuario "UPDATE" podremos hacer modificaciones a través de esa interfaz.
	
## Despliegue de PostgreSQL

El procedimiento para desplegar PostgreSQL es similar al de virtuso. Utilizaremos docker-compose con su respectivo yml. En esta plantilla no es necesario ajustar nada aunque podemos ajustar el password que queramos en el parámetro "POSTGRES_PASSWORD" del docker-compose.yml, cosa que tenemos que tener en cuenta a la hora de ajustar el yml de los servicios que veremos más adelante.

Partiendo desde la home del usurio (ej. /home/usuario/) creamos el directorio que va a conetener el docker-compose.yml, entramos en el directorio, descargamos el yml y levantamos el docker con los siguientes comandos: 

	mkdir postgresql
	cd postgresql
	wget http://herc-as-front-desa.atica.um.es/docs/docker-postgresql/docker-compose.yml
	docker-compose up -d
	
Después de desplegar, como en el caso anterior vamos a hacer la comprobación de que el contenedor está levantado pero en esta ocasión vamos a usar el comando docker-compose ps que se limita a mostrar información solo de los procesos de este yml.
	
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

Ahora debemos loguearnos con usurio "guest" y password "guest", que son los que estan ajustados en el yml, y procederemos a crear un virtual host seguiendo estos sencillos pasos:

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

## Despliegue de los servicios de back

Para simplificar el despliegue de los servicios de back, hemos creado un script que debemos descargar en nuestra máquinas para servicios de back. Partiendo desde la home del usurio (ej. /home/usuario/).

	wget http://herc-as-front-desa.atica.um.es/docs/docker-servicios-back/actualizar-back.sh
	
Este escript clonará los repositorios necesarios y luego generará las imágenes docker automáticamente. Le debemos dar permisos de ejecución.

	chmod +x actualizar-back.sh

Depués creamos el directorio donde vamos a alojar el docker-compose.yml que va orquestar todos los servicios. Lo hemos llamado dock-back porque en el script actualizar-back.sh así se llama papero podemos jugar con estos valores. Después lo descargamos.

	mkdir dock-back
	cd dock-back
	wget http://herc-as-front-desa.atica.um.es/docs/docker-servicios-back/docker-compose.yml
	
Antes de lentar los servicios debemos editar este archivo y reemplezar "ip_del_servicio" por la ip de la máquina donde estemos levantando los servicios. Asi todos los servicios se podran comunicar conrrectamente entre ellos.	

Con la ip ajustada ya podemos ejecutar script que nos prepara el entorno.

	./actualizar-back.sh

## Despliegue de los servicios front

Para simplificar el despliegue de los servicios de front, hemos creado un script que debemos descargar en nuestra máquina donde queramos alojar los servicios de front. Partiendo desde la home del usurio (ej. /home/usuario/).

	wget http://herc-as-front-desa.atica.um.es/docs/docker-servicios-front/actualizar-front.sh
	
Este escript clonará los repositorios necesarios y luego generará las imágenes docker automáticamente. Le debemos dar permisos de ejecución.

	chmod +x actualizar-front.sh

Depués creamos el directorio donde vamos a alojar el docker-compose.yml que va orquestar todos los servicios. Lo hemos llamado dock-front porque en el script actualizar.sh así se llama papero podemos jugar con estos valores. Después lo descargamos.

	mkdir dock-front
	cd dock-front
	wget http://herc-as-front-desa.atica.um.es/docs/docker-servicios-front/docker-compose.yml
	
Antes de lentar los servicios debemos editar este archivo y reemplezar "ip_del_servicio" por la ip de la máquina donde estemos levantando los servicios. Asi todos los servicios se podran comunicar conrrectamente entre ellos.	

Con la ip ajustada ya podemos ejecutar script que nos prepara el entorno.

	./actualizar-front.sh
	
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

## Ejemplo de configuración de HAProxy

Para implementar la alta disponibilidad tanto de los frontales web, como de Virtuoso podemos colocarlos duplicados detrás de un HAProxy. Aquí podemos ver un ejemplo de configuración:
#---------------------------------------------------------------------
# Example configuration for a possible web application.  See the
# full configuration options online.
#
#   http://haproxy.1wt.eu/download/1.4/doc/configuration.txt
#
#---------------------------------------------------------------------

#---------------------------------------------------------------------
# Global settings
#---------------------------------------------------------------------
global
    # to have these messages end up in /var/log/haproxy.log you will
    # need to:
    #
    # 1) configure syslog to accept network log events.  This is done
    #    by adding the '-r' option to the SYSLOGD_OPTIONS in
    #    /etc/sysconfig/syslog
    #
    # 2) configure local2 events to go to the /var/log/haproxy.log
    #   file. A line like the following can be added to
    #   /etc/sysconfig/syslog
    #
    #    local2.*                       /var/log/haproxy.log
    #
    log         127.0.0.1 local2

    chroot      /var/lib/haproxy
    pidfile     /var/run/haproxy.pid
    maxconn     4000
    user        haproxy
    group       haproxy
    daemon

    # turn on stats unix socket
    stats socket /var/lib/haproxy/stats

#---------------------------------------------------------------------
# common defaults that all the 'listen' and 'backend' sections will
# use if not designated in their block
#---------------------------------------------------------------------
defaults
    mode                    http
    log                     global
    option                  httplog
    option                  dontlognull
    option http-server-close
    option forwardfor       except 127.0.0.0/8
    option                  redispatch
    retries                 3
    timeout http-request    10s
    timeout queue           1m
    timeout connect         10s
    timeout client          1m
    timeout server          1m
    timeout http-keep-alive 10s
    timeout check           10s
    maxconn                 3000

#WEB

listen hercules443
    bind ip_del_haproxy:443
    mode tcp
    option tcplog
    option redispatch
    option clitcpka
    option srvtcpka
    option tcpka
    timeout client 3s
    retries 2
    balance roundrobin 
    hash-type consistent
    stick-table type ip size 1m expire 1h
    stick on src
    timeout connect 3s
    timeout server 3s
    server nodo1 ip_del_nodo1_web:443 check inter 3s fall 1 rise 2
    server nodo2 ip_del_nodo2_web:443 check inter 3s fall 1 rise 2

listen hercules:80
    bind ip_del_haproxy:80
    mode tcp
    option tcplog
    option redispatch
    option clitcpka
    option srvtcpka
    option tcpka
    timeout client 3s
    retries 2
    balance roundrobin
    hash-type consistent
    stick-table type ip size 1m expire 1h
    stick on src
    timeout connect 3s
    timeout server 3s
    server nodo1 ip_del_nodo1_web:80 check inter 3s fall 1 rise 2
    server nodo2 ip_del_nodo1_web:80 check inter 3s fall 1 rise 2

#VIRTUOSO

listen VirtuosoLecturaProGnoss
    stats enable
    bind ip_del_haproxy:8890
    option forwardfor except 127.0.0.0/8
    mode http
    balance roundrobin
    option httpclose
    option redispatch
    retries 2
    option forwardfor
    option httpchk HEAD /sparql
    http-check expect status 200
    http-response add-header X-App-Server %b_%s
    server v1pro v1pro:8890 check inter 3s fall 1 rise 2 
    server v2pro v2pro:8890 check inter 3s fall 1 rise 2 
	
listen VirtuosoLecturaProGnoss1111
    bind ip_del_haproxy:1111
    mode tcp
    option tcplog
    option redispatch
    option clitcpka
    option srvtcpka
    option tcpka
    timeout client 3s
    retries 2
    balance source
    hash-type consistent
    stick-table type ip size 1m expire 1h
    stick on src
    timeout connect 3s
    timeout server 3s   
    server v1pro v1pro:1111 check inter 3s fall 1 rise 2 
    server v2pro v2pro:1111 check inter 3s fall 1 rise 2 

#STATS

listen stats
    bind *:9999
    stats enable
    stats uri /stats
    stats auth admin:admin
   
