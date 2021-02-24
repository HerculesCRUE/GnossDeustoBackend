![](.//media/CabeceraDocumentosMD.png)

# Hércules Backend ASIO. Evaluación de cumplimiento Linked Data Platform (LDP)


[INTRODUCCIÓN](#introducción)

[TIPOS DE RECURSOS SOPORTADOS](#tipos-de-recursos-soportados)

[RECUPERACIÓN Y CREACIÓN DE RECURSOS](#recuperación-y-creación-de-recursos)

[CONTENEDORES](#contenedores)

[CONCLUSIONES](#conclusiones)

INTRODUCCIÓN
============

Linked Data Platform (LDP) es una especificación de datos enlazados
(Linked Data) que define un conjunto de reglas para operaciones HTTP
sobre recursos web, expresados algunos de ellos en RDF (Resource
Description Framework <https://www.w3.org/RDF/>), que proporcionan una
arquitectura para leer y escribir datos enlazados en la web.

LDP 1.0 es una recomendación del W3C: <https://www.w3.org/TR/ldp/>

El presente documento evalúa el grado de cumplimiento de LDP que el
Servidor Linked Data de Hércules ASIO debería cubrir, para lo que se 
tienen en cuenta la extensión de uso de LDP en la comunidad Linked Data, 
su utilidad en ASIO, los posibles beneficios a largo plazo, la 
dificultad de implementación y los condicionantes de seguridad.

Cumplir con LDP garantiza un servidor que publica datos enlazados
(*linked data*) de acuerdo a los estándares y clarifica y extiende las
reglas de datos enlazados ([*Linked Data Design
Issues*](http://www.w3.org/DesignIssues/LinkedData.html)):

1.  Usar URIs como nombres de cosas.

2.  Usar URIs HTTP para que las personas puedan localizar esos nombres.

3.  Cuando alguien pide un URI, proporcionar información útil usando
    estándares (RDF, SPARQL).

4.  Incluir enlaces a otros URIs, para que se puedan descubrir más
    cosas.

Para cada sección del presente documento indicaremos el nivel de 
cumplimiento LDP que proponemos para el servidor Linked Data de ASIO:

-   Tipos de recursos soportados. El servidor Linked Data de ASIO 
    ofrecerá recursos representados en RDF.

-   Recuperación y creación de recursos. El servidor Linked Data de 
    ASIO responderá a los métodos HTTP de recuperación de
    información (GET, HEAD y OPTIONS) y NO a los de escritura (POST, 
    PUT, DELETE y PATCH).
    
-   Contenedores. El servidor Linked Data de ASIO dispondrá de
    un único contenedor de tipo *basic container*.

TIPOS DE RECURSOS SOPORTADOS
============================

En un servidor LDP los recursos (LDP Resources o LDPR) pueden ser de dos
tipos:

-   Representados en RDF (LDP-RS).

-   Representados en otros formatos (LDP-NR), como imágenes, HTML,
    documentos generados con editores de texto, hojas de cálculo, etc.

![](.//media/image2_LDP.png)

*Ejemplos de diferentes tipos de LDPRs - Fuente W3C*

Según la especificación, es obligatorio que un servidor LDP permita
la recuperación de recursos LDP-RS, pero no que ofrezca recursos LDP-NR.
Proponemos que el servidor LDP de Hércules ASIO sólo aloje y sirva 
recursos LDP-RS, mientras que los documentos LDP-NR de otros tipos, 
enlazados o descritos por los metadatos, serían accesibles mediante 
otras plataformas. Es decir, el servicio de recursos LDP-NR será 
responsabilidad de las plataformas que los alojen, particularmente de
Hércules SGI, y no del servidor Linked Data de ASIO.

RECUPERACIÓN Y CREACIÓN DE RECURSOS
===================================

La primera de las especificaciones generales de LDP que debe cumplirse
es que sea un servidor HTTP 1.1. Otras especificaciones obligatorias son
las habituales en este tipo de servidores y su cumplimiento no supone
dificultades ni problemas reseñables, por lo que incluirán. Se trata de:

-   Las respuestas deben incluir *entity tags* en una cabecera ETag,
    como mecanismo de validación de la cache web (ver
    <https://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.19>)

-   Las respuestas deben advertir que soportan LDP añadiendo una
    cabecera HTTP Link con una URI <http://www.w3.org/ns/ldp#Resource>;
    y otro Link de tipo *relation* (rel="type") con el tipo del LDPR.

-   Los servidores LDP deben publicar las posibles restricciones para
    crear o actualizar recursos (LDPRs), mediante una cabecera Link, una
    relación <http://www.w3.org/ns/ldp#constrainedBy> y un URI que
    defina el conjunto de restricciones. Por ejemplo y como veremos en
    nuestro caso, un servidor LDP podría rechazar la creación de
    recursos con PUT o POST, por lo que se devolvería esa cabecera Link al
    responder con un código 4xx.

En cuanto a los métodos HTTP de gestión de los recursos, la
especificación de LDP indica los siguientes métodos obligatorios 
(todos de lectura), a los que responderá el servidor Linked Data: 
GET, HEAD y OPTIONS.

Por el contrario, son métodos opcionales los que tienen que ver con la
actualización de recursos: POST, PUT, DELETE y PATCH.

Como ya hemos adelantado, consideramos que el servidor LDP de ASIO no
debería soportar las actualizaciones de recursos mediante los métodos
HTTP. Los métodos expuestos funcionan bien cuando la actualización
afecta sólo a un recurso concreto, sin implicaciones en recursos
relacionados. Cuando el modelo ontológico es complejo, como es el caso
de ASIO, actualizar una entidad (p.e. un proyecto) requiere
habitualmente de la actualización de datos entidades relacionadas (p.e.
las actividades). Hacer esto requeriría de varias peticiones (POST o
PUT) y de un control de la operación global con una especie de
transacción muy compleja de implementar mediante peticiones HTTP. 
Por otro lado, hay que considerar que la edición de recursos, 
y por tanto la modificación de sus datos (salvo determinados 
procesos como reconciliación de entidades y descubrimiento de
enlaces de Hércules ASIO, así como el enriquecimiento de datos 
previsto en Hércules ED), se realiza en el SGI de cada universidad, 
por lo que sería complicado gestionar problemas de inconsistencia 
de datos si se permitiera además hacer modificaciones mediante el
servidor LDP de ASIO.

En cuanto a las peticiones GET de recursos RDF (LDP-RS), lo que debe
cumplir un servidor LDP no tendría ninguna característica que no deba
tener un servidor Linked Data, por lo que también lo cumplirá. 
Se trata de:

-   Los recursos RDF deben ser recursos LDP-RS.

-   El servidor LDP debe proporcionar una representación RDF de los
    recursos LDP-RS.

-   Los clientes LDP asumen que un recurso LDP-RS puede tener más de un
    rdf:type y que estos pueden cambiar con el tiempo.

-   El servidor LDP no requiere que los clientes tengan inferencias
    implementadas.

CONTENEDORES
============

La implementación LDP del servidor Linked Data de ASIO tendría las 
siguientes características:

1.  El contenedor sólo devolverá un subconjunto de sus propiedades 
    (minimal-container triples) y en ningún caso todos los triples de 
    los recursos contenidos, ni siquiera de manera paginada. 
    
En el caso de ASIO, el volumen de recursos supondría una carga para
los sistemas que no se justifica. Si se desease proporcionar acceso
al dataset completo, sería más recomendable ofrecer una descarga
desde un servidor de ficheros.
    

2.  Sólo necesita un contenedor. 

Según el estándar LDP, la separación en containers tiene sentido 
cuando la información se organiza en conceptos que particionan 
la información. En el ejemplo del estándar hablan de blogs, 
páginas wiki o productos. Consideramos que no es necesario aplicar
en ASIO esta división en conceptos, ya que toda la información 
pertenece a la misma fuente, el Sistema de Gestión de la 
Investigación (SGI) de la universidad. Por tanto, proponemos que
no sea necesario que el servidor Linked Data disponga del 
mecanismo LDP previsto para obtener la información de un *recurso*
entre varios contenedores.

Además, las respuestas a las preguntas que, según el estándar LDP,
los contenedores deberían responder, indican que su utilidad en el
servidor Linked Data que proponemos sería muy limitado o inexistente:

*To which URLs can I POST to create new resources?*

Como hemos indicado antes, proponemos que el servidor Linked Data
no acepte actualizaciones de datos.

*Where can I GET a list of existing resources?*

Como hemos justificado antes por volumen de datos, proponemos que,
si es necesario un acceso al dataset completo, este se proporcione
mediante un volcado de datos descargable desde un servidor de
ficheros.

*How do I get information about the members along with the container?*

El servidor Linked Data proporcionará información básica acerca del 
tipo de miembros que contiene, particularmente su definición ontológica.

*How can I ensure the resource data is easy to query?*

El mecanismo de consulta de una entidad individual es el previsto
en un servidor Linked Data, que permite la navegación ilimitada entre
entidades a través de sus URI. Para consultas más complejas, el 
proyecto ASIO contará con un punto SPARQL.

*How is the order of the container entries expressed?*

Como hemos indicado, no vemos necesario implementar la paginación
como mecanismo de recuperación de entidades, por lo que tampoco
sería necesario gestionar un orden.


En resumen, el servidor Linked Data de ASIO se implementará como un servidor 
LDP con un único contenedor que sólo devolverá los triples de
la entidad solicitada ([LDP Basic container](https://www.w3.org/TR/ldp/#ldpbc)).


CONCLUSIONES
============

El servidor Linked Data que implementaremos en ASIO cumplirá el estándar
LDP para la recuperación de recursos RDF (LDP-RS), con un único contenedor 
y sólo con las funciones de lectura de datos.

Probablemente, el servidor Linked Data de ASIO reutilizará alguna
implementación de código abierto que soporte el estándar LDP. El 
nivel de cumplimiento aquí expresado no presupone una restricción
para estas implementaciones, sino que expresa el mínimo que deberían
ser capaces de cumplir. Por ejemplo, podría ser que la implementación 
seleccionada soportase el servicio de recursos no-RDF (LDP-NR), pero
no consideramos que sea un requisito, ya que en nuestra propuesta el
servidor Linked Data sólo servirá recursos RDF (LDP-RS).
