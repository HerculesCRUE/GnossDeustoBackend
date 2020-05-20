![](.//media/CabeceraDocumentosMD.png)

# Hércules Backend ASIO. Buenas prácticas del esquema de URIs

[1 INTRODUCCIÓN](#introducción)

[1 Buenas prácticas de URIs](#buenas-prácticas-de-uris)

[1.1 Normalización de los componentes de los URI](#normalización-de-los-componentes-de-los-uri)

[1.2 Prácticas relativas a la gestión de recursos semánticos a través de URI](#prácticas-relativas-a-la-gestión-de-recursos-semánticos-a-través-de-uri)

INTRODUCCIÓN
============

El presente documento describe las buenas prácticas en la definición de los URI
para el proyecto Hércules ASIO y tiene en cuenta la [Especificación del Esquema de URIs de
Hércules](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/UrisFactory/docs/Especificaci%C3%B3n%20Esquema%20de%20URIs.md).

En su elaboración tenemos en cuenta las recomendaciones de la [Norma
Técnica de Interoperabilidad de Reutilización de recursos de la
Información](https://www.boe.es/boe/dias/2013/03/04/pdfs/BOE-A-2013-2380.pdf)
(NTI), de la Secretaría de Estado de Administraciones Públicas; que se
basan, a su vez, en las iniciativas de datos abiertos y las
recomendaciones procedentes del mundo de la Web Semántica.

Las recomendaciones de multilingüismo se incluyen y detallan en el documento
[20200427 Hércules ASIO Modelo de multilingüismo.md](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/20200427%20H%C3%A9rcules%20ASIO%20Modelo%20de%20multiling%C3%BCismo.md).

Buenas prácticas de URIs
========================

Agrupamos las Buenas prácticas en unas **Reglas de normalización** de
los componentes de los URI y en unas recomendaciones en la **Gestión de
recursos semánticos** a través de URI.

Normalización de los componentes de los URI
-------------------------------------------

Para garantizar la coherencia y el mantenimiento posterior del esquema
de URI se aplicarán las siguientes reglas para normalizar las distintas
partes que componen los URI:

a)  Seleccionar identificadores alfanuméricos cortos únicos, que sean
    representativos, intuitivos y semánticos.

b)  Usar siempre minúsculas, salvo en los casos en los que se utilice el
    nombre de la clase o concepto. Habitualmente, los nombres de las
    clases se representan con el primer carácter de cada palabra en
    mayúsculas.

c)  Eliminar todos los acentos, diéresis y símbolos de puntuación. Como
    excepción puede usarse el guion (--).

d)  Eliminar conjunciones y artículos en los casos de que el concepto a
    representar contenga más de una palabra.

e)  Usar el guion (--) como separador entre palabras.

f)  Evitar en la medida de lo posible la abreviatura de palabras, salvo
    que la abreviatura sea intuitiva.

g)  Los términos que componen los URI deberán ser legibles e
    interpretables por el mayor número de personas posible, por lo que
    se utilizará el castellano, cualquiera de las lenguas oficiales de
    España o el inglés como lengua franca de la investigación.

Prácticas relativas a la gestión de recursos semánticos a través de URI
-----------------------------------------------------------------------

Las siguientes prácticas se desarrollarán como requisitos del servidor
Linked Data de Hércules ASIO y se aplicarán para la gestión de recursos
semánticos descritos en RDF:

a)  Cumplir el principio de persistencia de los URIs, lo que significa
    que los que ya han sido creados previamente nunca deberían variar, y
    que el contenido al que hacen referencia debería ser accesible. En
    el caso de que sea necesario cambiar o eliminar el recurso al que
    apunta un identificador, se deberá establecer un mecanismo de
    información sobre el estado del recurso usando los códigos de estado
    de HTTP. En el caso de poder ofrecer una redirección a la nueva
    ubicación del recurso, se utilizarán los códigos de estado HTTP 3XX,
    mientras que para indicar que un recurso ha desaparecido
    permanentemente se utilizará el código de estado HTTP 410

b)  Siempre que sea posible, y existan versiones del recurso en formato
    legible para personas HTML o similar y RDF, el servidor que gestiona
    los URI realizará negociación del contenido en función de la
    cabecera del agente que realiza la petición. En el caso de que el
    cliente acepte un formato de representación RDF en cualquiera de sus
    notaciones (p.e., especificando en su cabecera que acepta el tipo
    MIME application/rdf+xml) se servirá el documento RDF a través del
    mecanismo de redirecciones alternativas mediante los códigos de
    estado HTTP 3XX.

c)  En el caso de que no se realice una negociación del contenido desde
    el servidor y, para favorecer el descubrimiento de contenido RDF
    desde los documentos HTML relacionados con las descripciones de los
    recursos, se incluirán enlaces a la representación alternativa en
    cualquiera de las representaciones en RDF desde los propios
    documentos HTML de la forma \<link rel=«alternate»
    type=«application/rdf+xml» href=«documento.rdf»\> o similar. En esa
    sentencia se incluye el tipo de formato MIME del documento
    (application/rdf+xml, text/n3, etc.).

d)  Cuando se establezcan enlaces entre distintos recursos de
    información, se procurará la generación de enlaces que conecten los
    recursos bidireccionales para facilitar la navegación sobre los
    recursos de información en ambos sentidos.

