## Sobre GnossDeustoBackend
Éste es el repositorio del proyecto Hércules ASIO en el que se incluiyen los siguientes programas y servicios

 - [API_CARGA](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_CARGA "API_CARGA"): Servicio web que realiza las tareas de carga/configuración.
 - [FrontEndCarga](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/FrontEndCarga "FrontEndCarga"): Interfaz web para la parte de Repository y Validation del API_CARGA
 - [OAI_PMH_CVN](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/OAI_PMH_CVN "OAI_PMH_CVN"): Servicio OAI-PMH para la obtención de invstigadores de la Universidad de Murcia.
 - [UrisFactory](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/UrisFactory "UrisFactory"): Servicio que genera las uris de los recursos
 - [cvn](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/cvn): Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH.
 - [CronConfigure](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/CronConfigure): Servicio Web que realiza la creación de tareas para la sincronización de un repositorio.
 
 ### Diagrama de componentes del proyecto:
 

<img src="img/diagrama_de_componentes.png" />


# Configuración e instalación


## índice

 - [Configuración del entorno](#configuración-del-entorno)
	 - [Instalar PostgreSQL](#instalar-postgreSQL)
   	- [Instalar dotnet](#instalar-dotnet)
   	- [HTPP + proxy](#htpp-+-proxy)
   	- [Instalar Git](#instalar-git)
- [Descarga de los proyectos](#descarga-de-los-proyectos)
- [Instalación](#instalación)
	- [Configuración de los apis](#configuración-de-los-apis)
	- [Control de aplicaciones](#control-de-aplicaciones)
		- [Creación de los script para levantar los apis](#creación-de-los-script-para-levantar-los-apis)
		- [Creación de servicios](#creación-de-servicios)

## Configuración del entorno
### Instalar PostgreSQL


Para realizar la instalación de PostgreSQL en CentOS, utilizamos el comando yum install:

    sudo yum install postgresql-server postgresql-contrib

En centos debemos inicializar la base de datos manualmente ejecutando el comando initdb de PostgreSQL:

    sudo -u postgres pg_ctl initdb -D /var/lib/pgsql/data

Ahora que PostgreSQL ha sido instalado correctamente, debemos asegurarnos que esté configurado para iniciar sesión desde localhost. Para esto, abrimos el archivo pg_hba.conf ubicado en el directorio de configuración y lo modificamos de la siguiente forma:

    # TYPE  DATABASE        USER            ADDRESS                 METHOD
    
    # "local" is for Unix domain socket connections only
    local   all             all                                     peer
    # IPv4 local connections:
    host    all             all             127.0.0.1/32            md5
    # IPv6 local connections:
    host    all             all             ::1/128                 md5

Si la aplicación se encuentra en un servidor diferente debemos añadir otra línea idéntica a la de localhost pero indicando la ip del servidor remoto.
Como se puede apreciar, el método de autenticación para las dos últimas entradas debe ser md5 en lugar de peer, ident o trust. Esto permitirá a cualquier aplicación web configurada para acceder a una base de datos específica a través del host localhost.
Este archivo de configuración se encuentra en :

    /var/lib/pgsql/data/pg_hba.conf

Para finalizar, vamos a probar que PostgreSQL esté funcionando correctamente. Para esto vamos a crear una base de datos con su respectivo usuario.
Para crear un usuario utilizamos el comando createuser de PostgreSQL:

    $ sudo -u postgres createuser -P -d testdb-user
    Enter password for new role:
    Enter it again:

Para crear una base de datos utilizamos el comando createdb de PostgreSQL, indicando el nombre del usuario que acabamos de crear y escribiendo su contraseña cuando se nos pida:

    $ createdb testdb -U testdb-user -h localhost
    Password:

Con la base de datos y el usuario creados, será posible iniciar sesión en la base de datos, utilizando el comando psql:

    $ psql testdb -U testdb-user -h localhost
    Password for user testdb-user:
    testdb=>

Ejecutamos el comando psql indicándole el nombre de la base de datos (testdb), el nombre de usuario (-U testdb-user) y el host (-h localhost).
Para cerrar la sesión del motor de bases de datos utilizamos la instrucción \q o la combinación de teclas CTRL-D:

    testdb=> \q
    $

### Instalar dotnet


Abra un terminal y ejecute el comando siguiente:

    sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm

Instalación del SDK de .NET Core
Actualice los productos disponibles para la instalación y, después, instale el SDK de .NET Core. En el terminal, ejecute el comando siguiente.

    sudo yum install dotnet-sdk-3.1

Instalación del entorno de ejecución de ASP.NET Core
Actualice los productos disponibles para la instalación y, después, instale el entorno de ejecución de ASP.NET. En el terminal, ejecute el comando siguiente.

    sudo yum install aspnetcore-runtime-3.1

Instalación del entorno de ejecución de .NET Core
Actualice los productos disponibles para la instalación y, después, instale el entorno de ejecución de .NET Core. En el terminal, ejecute el comando siguiente.
sudo yum install dotnet-runtime-3.1


### HTTP + proxy


Para poder utilizar las aplicaciones debemos instalar un proxy que redirija las peticiones que hagamos al servidor apache al puerto donde tengamos levantada nuestra aplicación.
Primero instalamos httpd con este comando:

    yum install httpd mod_ssl

Para que nuestro proxy funcione correctamente debemos ejecutar el siguiente comando:

    /usr/sbin/setsebool -P httpd_can_network_connect 1

Para más información sobre el paso anterior:
https://unix.stackexchange.com/questions/174593/centos-7-httpd-failed-to-make-connection-with-backend
Lo primero que debemos hacer es indicar a httpd que use los módulos del proxy. Para ello debemos añadir estas líneas en /etc/httpd/conf/httpd.conf

    LoadModule proxy_module modules/mod_proxy.so
    LoadModule proxy_http_module modules/mod_proxy_http.so

Una vez hecho esto tenemos que hacer un archivo de configuración para redirigir las peticiones a httpd hacia el sitio correcto. Para ello creamos un archivo .conf en /etc/httpd/conf.d con un contenido como este:

    <VirtualHost *:80>
        ServerName pruebasdotnet.gnoss.com
        #URIS
        ProxyPass /uris http://127.0.0.1:5000
        ProxyPassReverse /uris http://127.0.0.1:5000
        #CARGA
        ProxyPass /carga http://127.0.0.1:5100
        ProxyPassReverse /carga http://127.0.0.1:5100
        #SAML
        ProxyPass /loginsir http://127.0.0.1:5101
        ProxyPassReverse /loginsir http://127.0.0.1:5101
        #OAI-PMH
        ProxyPass /oai-pmh-cvn http://127.0.0.1:5102
        ProxyPassReverse /oai-pmh-cvn http://127.0.0.1:5102
        #CARGA-WEB
        ProxyPass /carga-web http://127.0.0.1:5103
        ProxyPassReverse /carga-web http://127.0.0.1:5103
        #CVN
        ProxyPass /cvn http://127.0.0.1:5104
        ProxyPassReverse /cvn http://127.0.0.1:5104
    </VirtualHost>

Con esta configuración conseguimos que lo que se pida a través del puerto 80 a pruebasdotnet.gnoss.com/uris el proxy lo redirija a localhost:5000 que es donde nuestra aplicación URIS está a la escucha.
Por último Activamos el servicio HTTPD y lo iniciamos con estos comandos:

 - `systemctl enable httpd`
 - `systemctl start httpd`

### Instalar Git

Para instalar Git basta con ejecutar el siguiente comando:

    yum install git


## Descarga de los proyectos

Primero empezamos descargando el repositorio de git, mediante el comando:

    git clone  https://github.com/HerculesCRUE/GnossDeustoBackend.git

Tras realizar este comando de git, se nos pedirá nuestra autenticación para verificar que tenemos acceso al repositorio. 
Una vez realizado tendremos descargada una carpeta GnossDeustoBackend con los diferentes proyectos:

 - **API_CARGA**: se encuentra en API_CARGA/API_CARGA.
 - **FrontEndCarga**: se encuentra en FrontEndCarga/ApiCargaWebInterface.
 - **OAI_PMH_CVN**: se encuentra en OAI_PMH_CVN/OAI_PMH_CVN
 - **UrisFactory:** se encuentra en UrisFactory/UrisAutoGenerator
 - **UrisFactory:** se encuentra en CronConfigure/CronConfigure
 
Las anteriores rutas son las que hay que compilar y ejecutar con los comandos dotnet, por lo que hay que tenerlas en cuenta.

## Instalación

### Configuración de los Apis
Antes de ejecutar los apis deberíamos configurar los elementos necesarios para que funcionen correctamente, por ello habrá que configurar los apis que aparecen a continuación:

 - **API_CARGA**: En este api tendremos que configurar las cadenas de conexión de postgresql ("PostgreConnection" y "PostgreConnectionmigration"), en el fichero appsettings.json que se encuentra en la raíz del proyecto. Estas cadenas de conexión deberán ir dentro de "ConnectionStrings", quedando: 
 >
    "ConnectionStrings": {
       "PostgreConnection": "Host=ip_del_servidor;Database=nombre_de_la_base_de_datos;Username=usuario;Password=contraseña",
       "PostgreConnectionmigration": "Host=ip_del_servidor;Database=nombre_de_la_base_de_datos;Username=usuario;Password=contraseña"
    },
Además, habrá que configurar en el fichero SparqlConfig.json los datos de configración del endpoint Sparql contra el que se realizarán las publicaciones, quedando:
>
    {
      "graph": "nombre_del_grafo_para_las_publicaciones",
      "endpoint": "url_del_endpoint_sparql",
      "queryparam": "nombre_del_parametro_para_la_query"
    }

    
 - **FrontEndCarga**: tendremos que configurar la ubicación del API_CARGA y del CronConfigure al que deben hacer las llamadas mediante el parámetro “ConfigURL” y "ConfigUrlCron" en el appsettings.json que se encontrará en la raíz del proyecto. Ejemplo: `"ConfigUrl": "http://herc-as-front-desa.atica.um.es/carga/","ConfigUrlCron": "https://localhost:44359/"`
 
  - **OAI_PMH_CVN**: En este api tendremos que configurar en el fichero OAI_PMH_CVN_Config.json la url del repositorio de curriculums CVN-XML de la UM y el servicio de CVN-ROH encargado de transformar los CVN-XML al formato RDF ROH, quedando: 
 >
    {
      "XML_CVN_Repository": "url_repositorio_curriculums_CVN-XML_UM",
      "CVN_ROH_converter": "url_CVN-ROH"
    }

### Control de aplicaciones

Aspectos a tener en cuenta:

 - Puerto en el que vamos a levantar la aplicación.
 - Ruta del servicio descargado: esta ruta la podemos obtener realizando el comando pwd dentro de la ruta del proyecto donde nos hemos descargado el repositorio GnossDeustoBackend.
 
Para levantar las aplicaciones tenemos dos opciones, mediante la ejecución de scripts o crear estas aplicaciones como servicios, que se explican a continuación.

#### Creación de los script para levantar los apis

En este apartado vamos a crear los script para levantar, para ello nos movemos a la raíz con cd # y creamos un scritp para cada servicio:

 1. Creamos el servicio con nano nombre_del_scritp.sh
 2. Dentro del script ponemos las siguientes líneas
	 1. `#!/bin/sh`
	 2. cd y la ruta que hemos obtenido con el comando pwd. Ejemplo:
		 1.  `cd /root/GnossDeustoBackend/UrisFactory/UrisAutoGenerator`
	 3. El comando dotnet run –urls”http:/0.0.0.0:puertoConfigurado” y el carácter & para ejecutarlo en segundo plano. Ejemplo:
		 1. `dotnet run --urls "http://0.0.0.0:5000" &`
	 4. Pulsamos CTRL-O para guardar.
	 5. Pulsamos CTRL-X para salir.
 3. Finalmente deberemos darle permisos de ejecución por lo que tendremos que ejecutar el siguiente comando: `chmod +x apiUris.sh`
 4. Para ejecutar el script vale con ejecutar `./apiUris.sh` que levantará nuestro servicio. 

#### Creación de servicios
Para que nuestro sistema controle la ejecución de las aplicaciones como un servicio debemos crear un archivo .service por cada aplicación en /etc/systemd/system
Este es un archivo de servicio de ejemplo de la aplicación. Debemos indicar el WorkingDirectory, el puerto en ExecStart y el usuario con que vayamos a ejecutar la aplicación

    [Unit]
    Description=Example .NET Web API App running on Centos
    
    [Service]
    WorkingDirectory=”Ponemos el path donde esté la app”
    ExecStart=dotnet run --urls "http://0.0.0.0:puerto" 
    Restart=always
    # Restart service after 10 seconds if the dotnet service crashes:
    RestartSec=10
    KillSignal=SIGINT
    SyslogIdentifier=dotnet-example
    User=www-data
    Environment=ASPNETCORE_ENVIRONMENT=Production
    Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
    
    [Install]
    WantedBy=multi-user.target

Por último ejecutamos estos comandos en el directorio donde estamos.

 - `systemctl enable “nombre del archivo.service”`
 - `systemctl start “nombre del archivo.service”`
De este modo no nos tendremos que preocupar de que nuestras aplicaciones se ejecuten porque serán servicios controlados por el sistema.
