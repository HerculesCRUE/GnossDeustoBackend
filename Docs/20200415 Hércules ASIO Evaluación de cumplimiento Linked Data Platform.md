![](.//media/CabeceraDocumentosMD.png)

# Hércules Backend ASIO. Evaluación de cumplimiento Linked Data Platform (LDP)


[1 INTRODUCCIÓN](#introducción)

[2 TIPOS DE RECURSOS SOPORTADOS](#tipos-de-recursos-soportados)

[3 RECUPERACIÓN Y CREACIÓN DE RECURSOS](#recuperación-y-creación-de-recursos)

[4 CONTENEDORES](#contenedores)

[5 CONCLUSIONES](#conclusiones)

INTRODUCCIÓN
============

Linked Data Platform (LDP) es una especificación de datos enlazados
(Linked Data) que define un conjunto de reglas para operaciones HTTP
sobre recursos web, expresados algunos de ellos en RDF (Resource
Description Framework <https://www.w3.org/RDF/>), que proporcionan una
arquitectura para leer y escribir datos enlazados en la web.

LDP 1.0 es una recomendación del W3C: <https://www.w3.org/TR/ldp/>

El presente documento evalúa el grado de cumplimiento de LDP que el
Servidor Linked Data de Hércules ASIO debe cumplir, para lo que se tiene en
cuenta la extensión de uso en la comunidad Linked Data, su utilidad en
ASIO, los posibles beneficios a largo plazo, la dificultad de
implementación y los condicionantes de seguridad.

TIPOS DE RECURSOS SOPORTADOS
============================

En un servidor LDP los recursos (LDP Resources o LDPR) pueden ser de dos
tipos:

-   Representados en RDF (LDP-RS).

-   Representados en otros formatos (LDP-NR), como imágenes, HTML,
    documentos generados con editores de texto, hojas de cálculo, etc.

![](.//media/image2_LDP.png)
*Ejemplos de diferentes tipos de LDPRs - Fuente W3C*

No es necesario que un servidor LDP permita la recuperación de recursos
LDP-NR. En Hércules ASIO sólo tendremos recursos LDP-RS, que podrían
contener metadatos que describan y enlacen con documentos de otros tipos
y que permitan su obtención desde otras plataformas. Es decir, el
servicio de otros tipos de recursos será responsabilidad de las
plataformas que los alojen, particularmente Hércules SGI.

RECUPERACIÓN Y CREACIÓN DE RECURSOS
===================================

La primera de las especificaciones generales de LDP que debe cumplirse
es que sea un servidor HTTP 1.1. Otras especificaciones obligatorias son
las habituales en este tipo de servidores y su cumplimiento no supone
dificultades ni problemas reseñables. Se trata de:

-   Las respuestas deben incluir *entity tags* en una cabecera ETag,
    como mecanismo de validación de la cache web (ver
    <https://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.19>)

-   Las respuestas deben advertir que soportan LDP añadiendo una
    cabecera HTTP Link con una URI <http://www.w3.org/ns/ldp#Resource>;
    y otro Link de tipo *relation* (rel="type") con el tipo del LDPR.

-   Los servidores LDP deben publicar las posibles restricciones para
    crear o actualizar recursos (LDPRs), mediante una cabecera Link, una
    relación <http://www.w3.org/ns/ldp#constrainedBy> y una URI que
    defina el conjunto de restricciones. Por ejemplo y como veremos en
    nuestro caso, un servidor LPD podría rechazar la creación de
    recursos con PUT o POST, y se devolvería esa cabecera Link al
    responder con un código 4xx.

En cuanto a los métodos HTTP de gestión de los recursos, la
especificación de LDP indica lo siguientes métodos obligatorios (todos
de lectura): GET, HEAD y OPTIONS.

Por el contrario, son métodos opcionales los que tienen que ver con la
actualización de recursos: POST, PUT, DELETE y PATCH.

Como ya hemos adelantado, consideramos que el servidor LDP de ASIO no
debería soportar las actualizaciones de recursos mediante los métodos
HTTP. Los métodos expuestos funcionan bien cuando la actualización
implica sólo a un recurso concreto, sin implicaciones en recursos
relacionados. Cuando el modelo ontológico es complejo, como es el caso
de ASIO, actualizar una entidad (p.e. un proyecto) requiere
habitualmente de la actualización de datos entidades relacionadas (p.e.
las actividades). Hacer esto requeriría de varias peticiones (POST o
PUT) y de un control de la operación global con una especie de
transacción muy complejo de implementar mediante peticiones HTTP.

En cuanto a las peticiones GET de recursos RDF (LDP-RS), lo que debe
cumplir un servidor LDP no tendría ninguna característica que no deba
tener un servidor Linked Data. Se trata de:

-   Los recursos RDF deben ser recursos LPDR.

-   El servidor LDP debe proporcionar una representación RDF de los
    recursos LDP-RS.

-   Los clientes LDP asumen que un recurso LDP-RS puede tener más de un
    rdf:type y que estos pueden cambiar con el tiempo.

-   El servidor LDP no requiere que los clientes tengan inferencias
    implementadas.

CONTENEDORES
============

La implementación LDP del servidor Linked Data de ASIO sólo tendría un
contenedor (*basic* *container*) y devolvería sólo un subconjunto de las
propiedades del contenedor (*minimal-container triples*), sin devolver
los triples de los recursos. En el caso de ASIO, el volumen de recursos
y su complejidad no permite que la recuperación de los triples se
realice mediante el mecanismo previsto en los contenedores.

CONCLUSIONES
============

El servidor Linked Data que implementaremos en ASIO cumplirá el estándar
LDP para la recuperación de recursos RDF, con un único contenedor y sin
las funciones de actualización de datos.

Cumplir con LDP garantiza un servidor que publica datos enlazados
(*linked data*) de acuerdo a los estándares y clarifica y extiende las
reglas de datos enlazados ( [*Linked Data Design
Issues*](http://www.w3.org/DesignIssues/LinkedData.html)):

1.  Usar URIs como nombres de cosas.

2.  Usar URIs HTTP para que las personas puedan localizar esos nombres.

3.  Cuando alguien pide una URI, proporcionar información útil usando
    estándares (RDF, SPARQL).

4.  Incluir enlaces a otros URIs, para que se puedan descubrir más
    cosas.
