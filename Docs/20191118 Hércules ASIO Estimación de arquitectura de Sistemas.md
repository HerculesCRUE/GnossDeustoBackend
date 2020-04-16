
# Hércules Backend ASIO. Estimación de Arquitectura de Sistemas

[1 INTRODUCCIÓN 3](#introducción)

[2 ARQUITECTURA DE SISTEMAS 4](#arquitectura-de-sistemas)

[2.1 Arquitectura lógica 4](#arquitectura-lógica)

[2.2 Arquitectura Física en producción
6](#arquitectura-física-en-producción)

[2.3 Arquitectura Física en Preproducción
9](#arquitectura-física-en-preproducción)

[2.4 Arquitectura Física en Desarrollo
10](#arquitectura-física-en-desarrollo)

INTRODUCCIÓN
============

El presente documento describe la arquitectura de sistemas del Backend
SGI del proyecto Hércules ASIO.

ARQUITECTURA DE SISTEMAS
========================

Este apartado describe la arquitectura de componentes (hardware y
sistemas) que necesitaría el Hércules Backend SGI, incluyendo una
estimación de requisitos.

Arquitectura lógica
-------------------

El diagrama lógico de la arquitectura es el siguiente:
![](.//media/image1.png)

Los componentes de la arquitectura son:

-   Frontales web para la generación dinámica de contenido Web.
    Contarían con un balanceo de red para garantizar la disponibilidad y
    dotar al Backend de una mayor capacidad de respuesta. Se encargarían
    de servir el contenido Web y de todos los servicios y APIs, con los
    niveles de seguridad de cada caso.

-   **Frontales** web para servir contenido estático y cache web**. Se
    encar**garían de las imágenes, ficheros Javascript y CSS de las
    páginas.

-   **S**ervidor para ejecución de servicios y tareas de procesado
    offline, que se encargarían de las tareas de carga y sincronización
    de datos entre los sistemas del backend y con los sistemas externos.

```{=html}
<!-- -->
```
-   Servicio de Base de Datos SQL, para configuración del portal
    semántico.

-   **S**ervidores balanceados para el sistema de gestión de base de
    datos RDF que aloje al Grafo de Conocimiento.

-   **S**ervidores balanceados para datos de cache, pregenerados por la
    plataforma para agilizar el servicio de páginas y datos hacia los
    usuarios del backend.

-   Servidor para gestión de colas de eventos, que usaremos en los
    procesos de actualización de datos.

-   Almacenamiento de disco en red, para alojamiento de archivos que
    deban ser accesibles por diferentes componentes del backend.

-   Servicio de balanceo de carga interno para los servidores de datos.

Arquitectura Física en producción
---------------------------------

Las siguientes características técnicas incluyen:

-   Un dimensionamiento estimado del hardware, según el volumen de datos
    estimado para el proyecto.

-   Un ejemplo de servicios de datos para los distintos modelos de
    gestión de base de datos: SQL, RDF Store, Cache y Colas de eventos.


: Sample grid table.

+---------------+---------------+--------------------+
| Fruit         | Price         | Advantages         |
+===============+===============+====================+
| Bananas       | $1.34         | - built-in wrapper |
|               |               | - bright color     |
+---------------+---------------+--------------------+
| Oranges       | $2.10         | - cures scurvy     |
|               |               | - tasty            |
+---------------+---------------+--------------------+


La lista de componentes es:

  ---------------------------------------------------------------------------------------------------------------------------
  Servidor                                             SO + Software                                      CPU   RAM   Disco
  ---------------------------------------------------- -------------------------------------------------- ----- ----- -------
  2 frontales web para contenido dinámico              CentOS 7\                                          4     8     50
                                                       Apache 2.4.33 con modcache y http2\                            
                                                       httpd24u-2.4.33-3.ius.centos7.x86\_64\                         
                                                       httpd24u-filesystem-2.4.33-3.ius.centos7.noarch\               
                                                       httpd24u-tools-2.4.33-3.ius.centos7.x86\_64\                   
                                                       httpd24u-mod\_ssl-2.4.33-3.ius.centos7.x86\_64\                
                                                       iotop-0.6-2.el7.noarch\                                        
                                                       net-tools-2.0-0.22.20131004git.el7.x86\_64\                    
                                                       htop-2.1.0-1.el7.x86\_64\                                      
                                                       mlocate-0.26-8.el7.x86\_64\                                    
                                                       lsof-4.87-5.el7.x86\_64\                                       
                                                       nano-2.3.1-10.el7.x86\_64                                      

  2 frontales web para estáticos y cache web           CentOS 7\                                          2     3     50
                                                       Apache 2.4.33 con modcache y http2 o SQUID\                    
                                                       httpd24u-2.4.33-3.ius.centos7.x86\_64\                         
                                                       httpd24u-filesystem-2.4.33-3.ius.centos7.noarch\               
                                                       httpd24u-tools-2.4.33-3.ius.centos7.x86\_64\                   
                                                       httpd24u-mod\_ssl-2.4.33-3.ius.centos7.x86\_64\                
                                                       iotop-0.6-2.el7.noarch\                                        
                                                       net-tools-2.0-0.22.20131004git.el7.x86\_64\                    
                                                       htop-2.1.0-1.el7.x86\_64\                                      
                                                       mlocate-0.26-8.el7.x86\_64\                                    
                                                       lsof-4.87-5.el7.x86\_64\                                       
                                                       nano-2.3.1-10.el7.x86\_64                                      

  1 servidor para tareas y servicios de Backoffice     CentOS 7\                                          2     6     50
                                                       Apache 2.4.33 con modcache y http2 o SQUID\                    
                                                       httpd24u-2.4.33-3.ius.centos7.x86\_64\                         
                                                       httpd24u-filesystem-2.4.33-3.ius.centos7.noarch\               
                                                       httpd24u-tools-2.4.33-3.ius.centos7.x86\_64\                   
                                                       httpd24u-mod\_ssl-2.4.33-3.ius.centos7.x86\_64\                
                                                       iotop-0.6-2.el7.noarch\                                        
                                                       net-tools-2.0-0.22.20131004git.el7.x86\_64\                    
                                                       htop-2.1.0-1.el7.x86\_64\                                      
                                                       mlocate-0.26-8.el7.x86\_64\                                    
                                                       lsof-4.87-5.el7.x86\_64\                                       
                                                       nano-2.3.1-10.el7.x86\_64                                      

  1 cluster de BBDD SQL                                PostgreSQL 11.6                                    4     8     50

  2 servidores para RDF Store en alta disponibilidad   CentOS Linux release 7.3.1611 (Core)\              4     14    50
                                                       Virtuoso 7.x Open Source Version\                              
                                                       ldd (GNU libc) 2.17\                                           
                                                       iotop-0.6-2.el7.noarch\                                        
                                                       net-tools-2.0-0.22.20131004git.el7.x86\_64\                    
                                                       htop-2.1.0-1.el7.x86\_64\                                      
                                                       mlocate-0.26-8.el7.x86\_64\                                    
                                                       lsof-4.87-5.el7.x86\_64\                                       
                                                       nano-2.3.1-10.el7.x86\_64                                      

  2 servidores para datos de cache                     CentOS Linux release 7.3.1611 (Core)\              2     4     25
                                                       Redis 5.x\                                                     
                                                       iotop-0.6-2.el7.noarch\                                        
                                                       net-tools-2.0-0.22.20131004git.el7.x86\_64\                    
                                                       htop-2.1.0-1.el7.x86\_64\                                      
                                                       mlocate-0.26-8.el7.x86\_64\                                    
                                                       lsof-4.87-5.el7.x86\_64\                                       
                                                       nano-2.3.1-10.el7.x86\_64                                      

  1 servidor para gestión de colas de eventos          CentOS Linux release 7.3.1611 (Core)\              2     2     25
                                                       RabbitMQ 3.8. rabbitmq-server\                                 
                                                       epel-release-7-11.noarch\                                      
                                                       erlang-R16B-03.18.el7.x86\_64.rpm\                             
                                                       socat-1.7.3.2-2.el7.x86\_64.rpm\                               
                                                       iotop-0.6-2.el7.noarch\                                        
                                                       net-tools-2.0-0.22.20131004git.el7.x86\_64\                    
                                                       htop-2.1.0-1.el7.x86\_64\                                      
                                                       mlocate-0.26-8.el7.x86\_64\                                    
                                                       lsof-4.87-5.el7.x86\_64\                                       
                                                       nano-2.3.1-10.el7.x86\_64                                      

  1 almacenamiento compartido                                                                                         50
  ---------------------------------------------------------------------------------------------------------------------------

Arquitectura Física en Preproducción
------------------------------------

La propuesta para los servidores de preproducción, con la misma
arquitectura lógica y software base que en producción, pero sin alta
disponibilidad (en principio), sería:

  Servidor                                           SO + Software                         CPU   RAM   Disco
  -------------------------------------------------- ------------------------------------- ----- ----- -------
  1 frontal web para contenido dinámico              CentOS 7, Apache, ...                 2     6     50
  1 frontal web para estáticos y cache web           CentOS 7, Apache, ...                 2     3     50
  1 servidor para tareas y servicios de Backoffice   PostgreSQL ...                        2     4     50
  1 servidor de BBDD SQL                             CentOS 7, Virtuoso Open Source, ...   3     8     50
  1 servidor para RDF Store                          CentOS 7, Redis, ...                  3     14    50
  1 servidor para datos de cache                     CentOS 7, RabbitMQ, ...               2     4     25
  1 servidor para gestión de colas de eventos        CentOS 7, Apache, ...                 2     1     25
  1 almacenamiento compartido                        NFS                                               50

Arquitectura Física en Desarrollo
---------------------------------

La propuesta para los servidores de desarrollo sería similar a la de
preproducción, pudiendo reutilizar componentes si se dimensionan
adecuadamente. En el caso de que fuesen servidores separados de los de
preproducción, necesitaríamos:

  Servidor                                           SO + Software                         CPU   RAM   Disco
  -------------------------------------------------- ------------------------------------- ----- ----- -------
  1 frontal web para contenido dinámico y estático   CentOS 7, Apache, ...                 2     6     50
  1 servidor para tareas y servicios de Backoffice   CentOS 7, Apache, ...                 2     3     50
  1 servidor de BBDD SQL                             PostgreSQL ...                        3     8     50
  1 servidor para RDF Store                          CentOS 7, Virtuoso Open Source, ...   3     14    50
  1 servidor para datos de cache                     CentOS 7, Redis, ...                  2     4     25
  1 servidor para gestión de colas de eventos        CentOS 7, RabbitMQ, ...               2     1     25
  1 almacenamiento compartido                        NFS                                               50
