![](.//media/CabeceraDocumentosMD.png)

# Hércules Backend ASIO. Modelo de Multilingüismo

[1 INTRODUCCIÓN](#introducción)

[2 DATOS MULTILINGÜES](#datos-multilingües)

[2.1 Nombrado (naming)](#nombrado-naming)

[2.2 Desreferenciado (dereferencing)](#desreferenciado-dereferencing)

[2.3 Etiquetado (labeling)](#etiquetado-labeling)

[2.4 Descripciones más largas (longer description)](#descripciones-más-largas-longer-description)

[2.5 Enlazado (linking)](#enlazado-linking)

[2.6 Reutilización (reuse)](#reutilización-reuse)

[3 RESUMEN DE PATRONES DEL MODELO](#resumen-de-patrones-del-modelo)

INTRODUCCIÓN
============

El desarrollo de la Red de Ontologías Hércules (ROH) en el proyecto Hércules ASIO
debe contemplar el uso de diversos idiomas. Por una parte, tiene que poder albergar
información de las universidades de las comunidades autónomas españolas
con sus propios idiomas oficiales. Por otra, el proyecto tiene vocación
de ser conocido y potencialmente usado más allá de España.

El presente documento describe un Modelo de multilingüismo que
aplicaremos en el desarrollo y posterior uso de la infraestructura
ontológica, es decir, en la Red de Ontologías Hércules y en su
documentación.

Como base de este modelo usamos las propuestas del siguiente artículo,
publicado en Semantic Web Journal:

[Multilingual Linked Data
Patterns](http://www.semantic-web-journal.net/content/multilingual-linked-data-patterns)

Jose Emilio Labra Gayo, Dimitris Kontokostas, Sören Auer

DATOS MULTILINGÜES
==================

En el artículo "Multilingual Linked Data Patterns" se describen un
conjunto de patrones y buenas prácticas para la definición y uso de una
plataforma de datos enlazados. El Modelo de multilingüismo propone la
aplicación de algunas de estos patrones en el desarrollo de la
Infraestructura Ontológica de Hércules ASIO y, particularmente, en la
Red de Ontologías Hércules (ROH).

La agrupación de patrones propuesta en el artículo anteriormente citado
es:

-   Nombrado (*naming*). Diseño de URI y descripción del dataset.

-   Desreferenciado (*dereferencing*). Gestión de la información
    devuelta en un entorno multilingüe.

-   Etiquetado (*labeling*). Provisión de etiquetas en varios idiomas.

-   Descripciones más largas (*longer descriptions*). Los recursos
    tienen información textual que no son sólo etiquetas y que pueden
    contener información en varios idiomas.

-   Enlazado (*linking*). Podrían existir recursos que representasen a
    la misma entidad en idiomas distintos. ¿Cómo se enlazan?

-   Reutilización (*reuse*). Los datos enlazados tienen que ver con la
    reutilización de los datos, que están unidos a los vocabularios que
    los expresan. ¿Es mejor que sean multilingües o traducirlos?

Indicamos a continuación que patrones de multilingüismo proponemos usar
en el desarrollo y posterior uso de la infraestructura ontológica de
ASIO. No repetiremos la descripción y argumentos completos de cada
patrón, que puede consultarse en el artículo de referencia.

Nombrado (naming)
-----------------

En una plataforma de datos enlazados los URIs no deben cambiar y no
deben depender de implementaciones técnicas o extensiones de ficheros.
Contamos con un manual de buenas prácticas que, como veremos a
continuación, tiene en cuenta alguno de los siguientes patrones patrones
en cuanto al nombrado:

1.  URIs descriptivos. Utilizar URIs descriptivos con caracteres ASCII
    codificados con caracteres extendidos %.

2.  URIs opacos. Utilizar URIs no interpretables por humanos.

3.  IRIS completas. Utilizar IRIs con caracteres Unicode.

4.  Nombres locales internacionalizados. Utilizar caracteres Unicode
    solo para los nombres locales.

5.  Incluir la información del idioma en los URIs.

Proponemos usar **URIs opacos** en cuanto a los identificadores de las
entidades (este patrón entra en contradicción con 1, 3 y 4). Es decir,
parte del URI describe la entidad que representa (un proyecto o un
investigador), pero el dato que lo identifica es un código no
interpretable por humanos, si bien podría el tipo de código puede ser
reconocido por algunos (un ORCID o un DNI). En cuanto a la parte
descriptiva del URI (http://data.um.es/res/**project**/{código}), el
proyecto ASIO cuenta con unas buenas prácticas y un Esquema de URIs (ver
[20191220 Hércules ASIO Especificación Esquema de URIs y Buenas
prácticas
URIs](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/UrisFactory/docs/20191220%20H%C3%A9rcules%20ASIO%20Especificaci%C3%B3n%20Esquema%20de%20URIs%20y%20Buenas%20pr%C3%A1cticas%20URIs.md)
y [20191211 Esquema de
URIs.json](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/UrisFactory/docs/20191211%20Esquema%20de%20URIs.json))
que propone URIs descriptivos en un único idioma con caracteres ASCII no
extendidos.

No vamos a incluir información del idioma en el URI. Como veremos a
continuación, no vamos a desreferenciar teniendo en cuenta el idioma
solicitado.

Desreferenciado (dereferencing)
-------------------------------

El desreferenciado de un URI consiste en la recuperación de los datos
que representan al recurso identificado por esa URI. Los patrones de
multilingüismo para el desreferenciado son:

1.  Datos independientes del idioma. Devolver los mismos triples con
    independencia del idioma.

2.  Negociación del contenido por idioma. Devolver triples diferentes
    dependiendo de las preferencias del solicitante (*user agent*).

Las peticiones HTTP de datos enlazados podrían incluir una cabecera
accept-language con las preferencias del agente que los solicita, lo que
se podría usarse para devolver sólo los datos del idioma requerido.

Nuestra propuesta es la devolución de todos los **datos independientes
del idioma**, sin tener en cuenta las preferencias del solicitante. Como
veremos a continuación, se indica el idioma de cada dato o atributo
devuelto, lo que permite al solicitante utilizar o presentar los datos
en el idioma de su conveniencia, una vez recuperados.

Etiquetado (labeling)
---------------------

Para exponer los datos a los usuarios, las aplicaciones de datos
enlazados asocian etiquetas (*labels*) a los recursos. Estas etiquetas
se muestran al usuario final junto con los datos, para facilitar su
interpretación. La propiedad más usada para mostrar etiquetas es
rdfs:label, aunque también hay otras posibilidades como skos:prefLabel o
dc:title.

Los patrones de etiquetado son:

1.  Etiquetar todo. Definir etiquetas para todos los recursos.

2.  Etiquetas multilingües. Añadir *tags* de idiomas a las etiquetas.

3.  Etiquetas sin *tags* de idioma. Añadir etiquetas en un idioma por
    defecto sin tags de idiomas.

Proponemos **etiquetar todos** los recursos (individuos, conceptos y
propiedades) y no sólo las entidades principales, para facilitar el uso
de los datos por parte de humanos, especialmente cuando la
representación de los datos se genera de manera automática. El
etiquetado debe ser consistente en el uso de mayúsculas, espacios,
delimitadores, etc.

Además, proponemos que las **etiquetas sean multilingües**, es decir,
que tengan un tag asociado que permita identificar el idioma apropiado
para cada aplicación. Por ejemplo:

:maria :country \"Spain\"\@en .

:maria :country \"España\"\@es .

La definición de ROH incluirá etiquetas en castellano (es) e inglés
(en). Inicialmente, las etiquetas de los individuos se generarán sólo en
castellano, si bien el sistema admitirá que se carguen en otros idiomas
oficiales o en inglés.

No proponemos la generación de etiquetas sin tag de idioma.

Descripciones más largas (longer description)
---------------------------------------------

Las etiquetas son útiles en las aplicaciones dirigidas a humanos, pero
no son suficientes cuando se necesita mostrar una descripción más larga.
Para estas descripciones existen propiedades comunes (como
dcterms:description o rdfs:comment).

Los patrones para gestionar el multilingüismo en descripciones más
largas son:

1.  Dividir descripciones largas. Sustituir descripciones largas por
    recursos adicionales con etiquetas.

2.  Información léxica. Añadir información léxica a las descripciones
    largas.

3.  Literales estructurados. Usar HTML/XML en los literales de
    descripciones largas.

En el desarrollo de ROH estamos evitando la existencia de descripciones
que puedan expresarse mediante la **división de descripciones largas**.
Es decir, no tenemos un triple que diga:

:maria :address :"Calle Mayor 2. 26001 Logroño. España"\@es

Este tipo de informaciones siempre se dividirían en ROH. Para el ejemplo
anterior:

:maria :street g:street1

g:street1 rdfs:label :"Calle Mayor 2"\@es

:maria :postalcode :"26001"

:maria :city g:city1

g:city1 rdfs:label :"Logroño"\@es

:maria :country g:country1

g:country1 rdfs:label "España"\@es

No proponemos la inclusión de información léxica.

En cuanto al uso de literales estructurados, no proponemos su uso en las
descripciones largas de los individuos, porque dificulta el
funcionamiento de soluciones de búsqueda por texto libre, ya que los
marcados interrumpen las cadenas de texto; pero sí incluiríamos
**literales estructurados en entidades principales, conceptos y
propiedades**, cuando fuera necesario para la generación automática de
una documentación enriquecida de ROH.

Enlazado (linking)
------------------

Hay algunos aspectos del enlazado que pueden ser tenidos en
consideración en las plataformas de datos enlazados multi-idioma.

Los patrones de enlazado para gestionar el multilingüismo son:

1.  Enlaces de identidad. Utilizar owl:sameAs o predicados similares
    para enlazar dos recursos que se refieren a la misma entidad en
    idiomas distintos.

2.  Enlaces *soft*. Utilizar predicados con semántica más "blanda" (como
    rdf:seeAlso) para declarar que dos recursos están relacionados entre
    idiomas.

3.  Metadatos lingüísticos. Añadir metadatos lingüísticos acerca de los
    términos del dataset.

Proponemos no aplicar ninguno de estos patrones y usar el etiquetado
multilingüe de todo (ver [Etiquetado](#etiquetado-labeling)).

Reutilización (reuse)
---------------------

La reutilización es uno de los aspectos fundamentales de los datos
enlazados y una de las mejores formas de conseguirlo es proporcionar
enlaces a vocabularios existentes, preferentemente con aquellos que sean
muy conocidos de la comunidad y permitan integrar diferentes fuentes de
datos.

Los patrones de reutilización para el multilingüismo son:

1.  Vocabularios monolingües. Utilizar vocabularios con etiquetas en un
    único idioma, como FOAF o Dublin Core.

2.  Vocabularios multilingües. Preferir vocabularios multi-idioma, con
    las etiquetas traducidas a diferentes idiomas.

3.  Traducir vocabularios existentes. Traducir etiquetas para enriquecer
    vocabularios existentes.

4.  Crear nuevos vocabularios traducidos. Los nuevos vocabularios se
    enlazarían con los existentes, mediante propiedades como owl:sameAs,
    owl:equivalentProperty o ow:equivalentClass.

En proponemos usar **vocabularios multilingües** y **traducir
vocabularios existentes** (en inglés) añadiendo las etiquetas en español
y, potencialmente, en otros idiomas.

RESUMEN DE PATRONES DEL MODELO
==============================

Por tanto, los patrones de multilingüismo que componen el modelo de ASIO
son:

-   Nombrado (*naming*).

    -   URIs opacos. Utilizar URIs no interpretables por humanos, pero
        con una parte descriptiva legible por humanos.

-   Desreferenciado (*dereferencing*).

    -   Datos independientes del idioma. Devolver los mismos triples con
        independencia del idioma.

-   Etiquetado (*labeling*).

    -   Etiquetar todo. Definir etiquetas para todos los recursos.

    -   Etiquetas multilingües. Añadir *tags* de idiomas a las
        etiquetas.

-   Descripciones más largas (*longer description*).

    -   Dividir descripciones largas. Sustituir descripciones largas por
        recursos adicionales con etiquetas.

    -   Literales estructurados. Usar HTML/XML en los literales de
        descripciones largas para entidades principales, conceptos y
        propiedades; pero no para individuos.

-   Enlazado (*linking*). No aplicable.

-   Reutilización (*reuse*).

    -   Vocabularios multilingües. Preferir vocabularios multi-idioma,
        con las etiquetas traducidas a diferentes idiomas.

    -   Traducir vocabularios existentes. Traducir etiquetas para
        enriquecer vocabularios existentes.
