![](./Docs/media/CabeceraDocumentosMD.png)

## Configuración e instalación

Este documento explica cómo configurar el entorno en el que desplegar los
módulos desarrollados una vez descargados desde GIT y posteriormente
compilados.

## índice

 - [Configuración del entorno](#configuración-del-entorno)
	 - [Instalar PostgreSQL](#instalar-postgresql)
	 - [Instalar Git](#instalar-git)
	 - [Instalar Virtuoso](#instalar-virtuoso)
   	 - [Instalar dotnet](#instalar-dotnet)
   	 - [HTTP + proxy](#http-+-proxy)   	 
- [Descarga de los proyectos](#descarga-de-los-proyectos)
- [Instalación](#instalación)
	- [Configuración de los apis](#configuración-de-los-apis)
	- [Control de aplicaciones](#control-de-aplicaciones)
		- [Creación de los script para levantar los apis](#creación-de-los-script-para-levantar-los-apis)
		- [Creación de servicios](#creación-de-servicios)

## Configuración del entorno

### Instalar PostgreSQL

Para realizar la instalación de PostgreSQL en CentOS, utilizamos el comando yum install:
	
	sudo yum -y install https://download.postgresql.org/pub/repos/yum/reporpms/EL-7-x86_64/pgdg-redhat-repo-latest.noarch.rpm
	sudo yum update
	sudo yum -y install postgresql12-server

En centos debemos inicializar la base de datos manualmente ejecutando el comando initdb de PostgreSQL:

	sudo /usr/pgsql-12/bin/postgresql-12-setup initdb	
	sudo systemctl enable postgresql-12
	sudo systemctl start postgresql-12
	
Probamos la conexión:

	sudo -u postgres -i psql
	
Y salimos con exit.

Añadimos un usuario llamado hercules al sistema y nos logueamos con el:
	
	sudo adduser hercules

Ahora vamos a crear un rol para nuestro usuario "hercules":
	
	sudo -u postgres -i createuser --interactive
	Ingrese el nombre del rol a agregar: hercules
	¿Será el nuevo rol un superusuario? (s/n) s

Y creamos la base de datos:

	sudo -u postgres -i createdb hercules -O hercules

Nos logueamos como hercules

	su hercules

Probamos la conexion del rol hercules y establecemos el password "hercules":

	psql
	psql (12.3)
	Digite «help» para obtener ayuda.
	hercules=#\password
	Ingrese la nueva contraseña:
	Ingrésela nuevamente:
	
Ahora que PostgreSQL ha sido instalado correctamente, debemos asegurarnos que esté configurado para iniciar sesión desde localhost. Para esto, abrimos el archivo pg_hba.conf ubicado en el directorio de configuración y lo modificamos de la siguiente forma:

	sudo nano /var/lib/pgsql/12/data/pg_hba.conf
 	
	# TYPE  DATABASE        USER            ADDRESS                 METHOD
    	
	# "local" is for Unix domain socket connections only
    	
	local   all             postgres                                peer
	
	local   all             all                                     md5
    	
	# IPv4 local connections:
    	
	host    all             all             127.0.0.1/32            md5
    	
	# IPv6 local connections:
    	
	host    all             all             ::1/128                 md5

Para acabar, hacemos un restart de PostgreSQL:

	sudo systemctl reload postgresql-12

También podría desplegarse la imagen docker de [PostgreSQL](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Builds/docker-images).

### Instalar Git

Para instalar Git basta con ejecutar el siguiente comando:

    yum install git

### Instalar Virtuoso

Instalación del servidor Virtuoso:
    
    yum update 
    yum upgrade
    yum install –y epel-release 
    yum groupinstall 'Development Tools'
    yum install wget sysstat autoconf.noarch automake.noarch libedit.x86_64 flex.x86_64 bison.x86_64 bison-runtime.x86_64 bison-devel.x86_64 gperf.x86_64 gawk.x86_64 m4.x86_64 libitm47-static.x86_64 libitm47-devel.x86_64 make.x86_64 MAKEDEV.x86_64 openssl.x86_64 openssl-devel.x86_64 openssl-devel glib2-devel.x86_64 glib2.x86_64 libedit* libtool-ltdl-devel* libtool* gcc* tcl nano libitm bash-completion net-tools
    cd /opt/
    git clone git://github.com/openlink/virtuoso-opensource.git -b develop/7
    cd virtuoso-opensource/
    ./autogen.sh
    CFLAGS="-O2 -m64"
    export CFLAGS
    ./configure --prefix=/opt/virtuoso/
    make
    make install

Configuración del servidor Virtuoso editando el fichero .ini:
        
    nano /opt/virtuoso/var/lib/virtuoso/db/virtuoso.ini
        
Editamos o añadirmos los siguientes parametros:
        
    [Database]
    MaxCheckpointRemap = 250000
        
    [Parameters]
    StopCompilerWhenXOverRunTime = 1
    MaxOptimizeLayouts           = 100
    MaxClientConnections         = 100
    CheckpointInterval           = -1
    SchedulerInterval            = 1
    DirsAllowed                  = ., ../vad, ./dumps
    ThreadCleanupInterval        = 1
    ResourcesCleanupInterval     = 1
    ThreadsPerQuery              = 8
    NumberOfBuffers               = 340000 (DEPENDE DE LA RAM)
    MaxDirtyBuffers               = 250000 (DEPENDE DE LA RAM)
        
    [HTTPServer]
    EnableRequestTrap            = 0
    MaxClientConnections         = 50
    ServerThreads                = 50
    
    [Zero Config]
    ServerName                   = hercules.gnoss.net
        
    [SPARQL]
    ResultSetMaxRows             = 10000
    MaxQueryCostEstimationTime   = 99999999999999999999999999999999999999999999       ; in seconds
    MaxQueryExecutionTime        = 99999999999999999999999999999999999999999999       ; in seconds
        
    [I18N]
    XAnyNormalization            = 3
        
    [Flags]
    enable_joins_only            = 1
        
Configuración del servicio:
        
    cd /etc/systemd/system/
    nano virtuoso.service
        
En la edición del fichero anterior pegamos el siguiente contenido:

    # ***Virtuoso.Service***
    [Unit]
    Description=Demonio de Virtuoso
    After=multi-user.target
    [Service]
    Type=simple
    ExecStart=/bin/virtuoso_ha
    User=root
    WorkingDirectory=/opt/virtuoso/var/lib/virtuoso/db
    Restart=on-failure
    StandardOutput=syslog
    StandardError=syslog
    [Install]
    WantedBy=multi-user.target

Creamos a continuación el script de ejecución:

    nano /bin/virtuoso_ha 
        
Y pegamos el siguiente contenido:
        
    #!/bin/bash
    while true
        do
        cuenta=`ps -A | grep -c virtuoso-t`
        if test "$cuenta" = 0
        then
        # ulimit -c unlimited
        cd /opt/virtuoso/var/lib/virtuoso/db/
        /opt/virtuoso/bin/virtuoso-t
        fi
        sleep 3
    done
    
Damos permiso de ejecución al fichero recién creado:
    
    chmod +x /bin/virtuoso_ha
        
Activamos y arrancamos el servicio:
    
    systemctl enable virtuoso.service
    systemctl start virtuoso.service
        
Definimos una contraseña para el usuario dba de Virtuoso:
    
    /opt/virtuoso/bin/isql
    set PASSWORD dba *password*;
        
Finalmente probamos el servidor desde un navegador:
    
    http://IP_DEL_SERVIDOR:8890
        
El último paso sería configurar la ejecución de [checkpoint de Virtuoso](http://docs.openlinksw.com/virtuoso/checkpointparams/), ya que a veces es preferible
que no sea Virtuoso quien gestione este proceso, ya que no se tiene el control de la hora exacta de ejecución.

Editaríamos el siguiente fichero:
    
    nano /sbin/checkpoint
        
Y pegando el siguiente contenido:
        
    sleep 5
    echo 'checkpoint;' > /tmp/checkpoint
    /opt/virtuoso/bin/isql 1111 dba I8dTzVSnsn4MP /tmp/checkpoint        
    
A continuación damos permisos de ejecución al fichero anterior:
        
    chmod +x /sbin/checkpoint
        
Añadimos el programa recién creado a crontab
        
    nano /etc/crontab
        
Y pegamos el siguiente contenido (por ejemplo, para que se haga a los 15 minutos de cada hora):
    
    15 * * * * root /sbin/checkpoint
        
Y por último reiniciamos crond:
        
    systemctl restart crond

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

Proceso en Centos 8

	sudo dnf install dotnet-sdk-3.1
	sudo dnf install aspnetcore-runtime-3.1
	sudo dnf install dotnet-runtime-3.1

### HTTP + proxy

Para poder utilizar las aplicaciones debemos instalar un proxy que redirija las peticiones que hagamos al servidor apache al puerto donde tengamos levantada nuestra aplicación.
Primero instalamos httpd con este comando:

    yum install httpd mod_ssl

Para que nuestro proxy funcione correctamente debemos ejecutar el siguiente comando:

    /usr/sbin/setsebool -P httpd_can_network_connect 1

Una vez hecho esto tenemos que hacer un archivo de configuración para redirigir las peticiones a httpd hacia el sitio correcto. Para ello creamos un archivo .conf en /etc/httpd/conf.d con un contenido como este:

    <VirtualHost *:80>
        ServerName pruebasdotnet.gnoss.com
	
	#APIURIS
    	ProxyPass /uris http://127.0.0.1:5000
    	ProxyPassReverse /uris http://127.0.0.1:5000

    	#APICARGA
    	ProxyPass /carga http://127.0.0.1:5100
    	ProxyPassReverse /carga http://127.0.0.1:5100

    	#APIFRONTCARGA
    	ProxyPass /carga-web http://127.0.0.1:5103
    	ProxyPassReverse /carga-web http://127.0.0.1:5103

    	#APICRON
    	ProxyPass /cron-config http://127.0.0.1:5107
    	ProxyPassReverse /cron-config http://127.0.0.1:5107

    	#OAI-PMH-CVN
    	ProxyPass /oai-pmh-cvn http://127.0.0.1:5102
    	ProxyPassReverse /oai-pmh-cvn http://127.0.0.1:5102

    	#OAI-PMH-XML
    	ProxyPass /oai-pmh-xml http://127.0.0.1:5110
    	ProxyPassReverse /oai-pmh-xml http://127.0.0.1:5110

    	#CVN
    	ProxyPass /cvn http://127.0.0.1:5104
    	ProxyPassReverse /cvn http://127.0.0.1:5104
    	ProxyPass /cvn_swagger http://127.0.0.1:8080
    	ProxyPassReverse /cvn_swagger http://127.0.0.1:8080

    	#BRIDGE
    	ProxyPass /fairmetrics_bridge http://127.0.0.1:5200
    	ProxyPassReverse /fairmetrics_bridge http://127.0.0.1:5200
    	ProxyPass /bridgeswagger http://127.0.0.1:8082
    	ProxyPassReverse /bridgeswagger http://127.0.0.1:8082

    	#CLIENTE-TOKEN
    	ProxyPass /clientetoken http://127.0.0.1:5105
    	ProxyPassReverse /clientetoken http://127.0.0.1:5105

    	#IDENTITY-SERVER
    	ProxyPass /identityserver http://127.0.0.1:5108
    	ProxyPassReverse /identityserver http://127.0.0.1:5108

    	#UNIDATA
    	ProxyPass /unidata http://127.0.0.1:5106
    	ProxyPassReverse /unidata http://127.0.0.1:5106

    	#APIGESDOC
    	ProxyPass /documentacion http://127.0.0.1:5109
    	ProxyPassReverse /documentacion http://127.0.0.1:5109
	
    </VirtualHost>

Con esta configuración conseguimos que lo que se pida a través del puerto 80 a pruebasdotnet.gnoss.com/uris el proxy lo redirija a localhost:5000 que es donde nuestra aplicación URIS está a la escucha.
Por último Activamos el servicio HTTPD y lo iniciamos con estos comandos:

 - `systemctl enable httpd`
 - `systemctl start httpd`

## Descarga de los proyectos

En esta apartado se explica como desplegar mediante la descarga de los proyectos
y su posterior compilación.Empezamos descargando el repositorio de git, con el comando:

    git clone  https://github.com/HerculesCRUE/GnossDeustoBackend.git

Tras realizar este comando de git, se nos pedirá nuestra autenticación para verificar que tenemos acceso al repositorio. 
Una vez realizado tendremos descargada una carpeta GnossDeustoBackend con los diferentes proyectos:

 - **triplestore-assessment-interface**: se encuentra en GnossDeustoBackend/src/Benchmark/triplestore-assessment-interface
 - **cvn**: se encuentra en GnossDeustoBackend/src/cvn
 - **bridge**: se encuentra en GnossDeustoBackend/src/fair/bridge
 - **API_CARGA**: se encuentra en GnossDeustoBackend/src/Hercules.Asio.Api.Carga/API_CARGA
 - **API_DISCOVER**: se encuentra en GnossDeustoBackend/src/Hercules.Asio.Api.Discover/API_DISCOVER
 - **CronConfigure**: se encuentra en GnossDeustoBackend/src/Hercules.Asio.Cron/CronConfigure
 - **OAI_PMH_CVN:** se encuentra en GnossDeustoBackend/src/Hercules.Asio.CVN2OAI_PMH/OAI_PMH_CVN
 - **GestorDocumentacion:** se encuentra en GnossDeustoBackend/src/Hercules.Asio.DinamicPages/GestorDocumentacion
 - **IdentityServerHecules:** se encuentra en GnossDeustoBackend/src/Hercules.Asio.IdentityServer/IdentityServerHecules
 - **Linked_Data_Server:** se encuentra en GnossDeustoBackend/src/Hercules.Asio.LinkedDataServer/Linked_Data_Server
 - **UrisAutoGenerator:** se encuentra en GnossDeustoBackend/src/Hercules.Asio.UrisFactory/UrisAutoGenerator
 - **ApiCargaWebInterface:** se encuentra en GnossDeustoBackend/src/Hercules.Asio.Web/ApiCargaWebInterface
 - **Api_Unidata:** se encuentra en GnossDeustoBackend/src/Unidata/Api_Unidata/Api_Unidata

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

