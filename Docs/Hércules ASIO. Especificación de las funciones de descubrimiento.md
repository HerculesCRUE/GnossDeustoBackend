![](.//media/CabeceraDocumentosMD.png)

# Hércules Backend ASIO. Especificación de las funciones de descubrimiento

[Introducción](#introducción)

[Reconciliación, Descubrimiento y Detección de equivalencias](#reconciliación-descubrimiento-y-detección-de-equivalencias)

Introducción
============
El API Descubrimiento ofrece unas funciones que son parte del proceso de carga. Estas funciones se dividen en 3 grupos:
-	Reconciliación de entidades, que evita la duplicación de entidades y puede añadir datos desde otros nodos ASIO, a través de las entidades incorporadas en el nodo Unidata.
-	Descubrimiento de enlaces, que genera enlaces hacia datasets externos (incluidos los de otros datasets ASIO a través del nodo Unidata), puede incorporar datos en ASIO y ofrece información de ayuda en la reconciliación de entidades.
-	Detección de equivalencias, que genera equivalencias semánticas.

Los 3 grupos de funciones se desarrollarán al mismo tiempo, ya que todos ellos actúan en el proceso de descubrimiento para todos los datos a cargar en ASIO.

Reconciliación, Descubrimiento y Detección de equivalencias
===================
Para completarse, el proceso de reconciliación de entidades necesita del descubrimiento de enlaces. A su vez, las equivalencias se detectan durante el proceso de descubrimiento.
El proceso de reconciliación de entidades actuará sobre un conjunto de triples correspondientes a una entidad que va a ser cargada en ASIO (por ejemplo, un investigador), utilizando los datos ya cargados en el propio nodo ASIO, los datos existentes en el nodo Unidata y los datos existentes en datasets externos. En este sentido, el proceso de reconciliación de entidades necesita los resultados del proceso de descubrimiento de enlaces, donde se detectarán las equivalencias.
La reconciliación actuará frecuentemente sobre datos de entidades que son, de alguna manera, ajenas al SGI de la universidad. Por ejemplo, un investigador ha podido declarar una publicación en la que otros investigadores sólo están citados con su nombre. Esto sucederá especialmente cuando se incorporen a ASIO datos provenientes de fuentes no-SGI, como podría ser el CVN.

Reconciliación y carga
---------------------
El proceso de reconciliación opera sobre un RDF proveniente de la transformación en formato ROH de unos datos originados en un servicio externo OAI-PMH. En la implementación actual, el servicio OAI-PMH disponible ofrece información de investigadores en formato CVN.
La o las entidades principales estarán identificadas en el RDF y estarán completamente definidas. Si la reconciliación es positiva, los datos de estas entidades se eliminarán del grafo y será sustituido por la nueva información, en el caso de que la reconciliación termine con éxito.
En detalle, el proceso de reconciliación y carga es el siguiente:
1.	Se lee el RDF y se obtienen todas las entidades que NO sean blank nodes para intentar realizar la reconciliación.
2.	Para cada una de las entidades se hace una consulta al grafo RDF para obtener posibles candidatos para la reconciliación y se obtienen todas las propiedades relevantes de esos elementos para posteriormente poder realizar los cálculos de descubrimiento oportunos y así realizar la reconciliación.
3.	Una vez obtenidos todos los candidatos se aplican las reglas de cálculo de descubrimiento para obtener las entidades finales ya cargadas.
4.	En función del resultado obtenido se realiza una de las siguientes acciones:
    1. Si para alguna entidad hay más de un candidato que supere el umbral máximo o hay alguna entidad que supere el umbral mínimo, pero no alcance el máximo se agregará el RDF a una BBDD junto con todos los datos necesarios para que el administrador decida como proceder.
    2. Si no estamos en el punto anterior, es decir no hay dudas en cuanto a la reconciliación:
       1.	Se obtienen las entidades principales del RDF y se eliminan todos los triples que haya en el grafo RDF en los que aparezcan como sujeto u objeto (nota: sólo tendrían que estar marcadas como 'principales' aquellas entidades que lleguen con la información completa, por ejemplo: una conferencia con todos sus papers ).
       2.	Se eliminan todos los triples de las entidades secundarias cuyo sujeto y predicado estén en el RDF a cargar y estén marcados como monovaluados, a excepción de las propiedades foaf:name y roh:title que no se eliminan y TAMPOCO se insertan.
       3.	Se vuelcan los triples al grafo RDF, a excepción de las propiedades foaf:name y roh:title de las entidades secundarias que no se hayan eliminado en el paso anterior. En el caso de las entidades secundarias nuevas sí que hay que cargar sus títulos.

Reglas de cálculo de descubrimiento
-----------------------
El flujo de acciones de descubrimiento, que comienza con la reconciliación de entidades, tiene estos pasos y reglas:
1.	Si la entidad cuenta con algún identificador (p.e. ORCID), reconciliar la entidad a través de alguno de sus identificadores. Si existe alguna entidad cargada en el nodo ASIO que comparta identificador se considera la misma entidad.
2.	Si no se ha encontrado la entidad, se buscan similitudes con entidades ya cargadas en ASIO. Para cada tipo de entidad (rdf:type) se definirá: 
    1.	Atributos y relaciones para la búsqueda de similitudes. Por ejemplo, el nombre del investigador y el paper con el que este investigador se incorporaría en ASIO.
    2.	Umbral mínimo para considerar la similitud suficiente para que sea una entidad candidata a reconciliar. Si se alcanza, la valoración obtenida se podría reforzar en los siguientes pasos de descubrimiento.
    3.	Umbral máximo para considerar que es la misma entidad.
3.	En el caso de que no se haya encontrado la entidad o sólo se hayan encontrado entidades candidatas que alcanzan el umbral mínimo, se utilizará el descubrimiento de enlaces para intentar la reconciliación. 
    1.	Nodo Unidata. La entidad podría estar cargada en Unidata procedente de otra universidad.
        1.	La entidad cuenta con algún identificador (p.e. ORCID) y no había una entidad candidata. Consideramos que es la misma entidad y procedemos a enriquecer los datos del nodo de ASIO con lo que hubiera en Unidata.
        2. La entidad cuenta con algún identificador (p.e. ORCID) pero había una entidad candidata. En este caso, tenemos que considerar que podría no ser la misma entidad. Por ejemplo, supongamos que la entidad a reconciliar es el investigador Luis Pérez con el ORCID XXX. Podría ser que en el nodo ASIO ya tuviéramos un Luis Pérez, pero sin código ORCID. No se puede considerar que sea la misma persona. Sin embargo, sí consideraríamos que sería la única entidad candidata a serlo de las existentes en Unidata.
        3. Se buscan similitudes con entidades ya cargadas en Unidata, en función del tipo de entidad, del mismo modo que se ha hecho antes en el nodo ASIO. Por ejemplo, el paper con el que llega este investigador podría estar ya cargado en Unidata. El resultado sería una nueva propuesta de entidades con su valoración de la similitud, a sumar al obtenido de ASIO (si lo hubiera).
    2.	Datasets externos. La entidad estará, probablemente, en alguna de las fuentes externas configuradas en el descubrimiento de enlaces. 
        1. La entidad cuenta con algún identificador (p.e. ORCID) y no había una entidad candidata. Consideramos que es la misma entidad y procedemos a enriquecer y enlazar los datos del nodo de ASIO con lo que hubiera en el dataset externo.
        2. La entidad cuenta con algún identificador (p.e. ORCID) pero había una entidad candidata. De la misma forma que hemos indicado para el nodo Unidata, no podemos considerar que sea la misma entidad, aunque sí sería la única candidata de los datasets externos.
        3. Mediante los APIs de descubrimiento se intentarían recuperar datos adicionales de la(s) entidad(es) candidata(s) con los que intentaría la reconciliación y se obtendría una nueva valoración que añadir, en su caso, a la obtenida de pasos anteriores.
4.	Terminados los pasos anteriores, podrían darse los siguientes casos:
    1.	Una entidad ha superado el umbral máximo. Se considera que es la misma entidad y se continúa con la carga con los datos adicionales obtenidos.
    2.	Más de una entidad ha superado el umbral máximo. Se incluye en la lista de entidades sobre las que tendría que decidir un administrador.
    3.	Una o más entidades han superado el umbral mínimo. Se incluyen en la lista de entidades sobre las que tendría que decidir un administrador.
    4.	Ninguna entidad ha superado el umbral mínimo. La entidad se considera nueva, y se continúa con el proceso de carga con los datos adicionales obtenidos en los procesos anteriores.

Configuración por tipo de entidad
-------------------
Para la reconciliación de entidades se realiza una configuración por cada tipo de entidad de las propiedades que deben coincidir para considerar que se trata de la misma entidad. Estas propiedades pueden ser directas o inversas y pueden tener N saltos (ver formato en (Configuración en el reconciliationConfig.json)[https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/API_DISCOVER/README.md#configuraci%C3%B3n-en-el-reconciliationconfigjson]
Esta concidencia puede ser de 4 tipos:
1.	Equals: El valor de la propiedad es exactamente el mismo.
2.	IgnoreCaseSensitive: El valor de la propiedad es exactamente el mismo (ignorando mayúsculas y minúsculas).
3.	Name: Utilizado para nombres de personas, tiene en cuenta abreviaturas, más o menos apellidos.... (pendiente de definir al algoritmo)
4.	Title: Utilizado para nombres/títulos en los que el valor debe ser el mismo ignorando caracteres especiales, mayúsculas, minúsculas y acentos.
    1.	En este caso, además hay que establecer el nº de palabras para considerar que la similitud es del 100%.

Algoritmos de similitud. Nombres y nombres propios
--------------
Se han evaluado las siguientes aproximaciones:
* Distancia de edición (distancia Levenshtein). Es utilizado comúnmente para ver las similitudes entre dos textos, pero consideramos que no daría un buen resultado en los nombres y apellidos. Es útil para encontrar textos similares por si se cometen faltas de ortografía, pero en este caso las faltas de ortografía no son tan frecuentes y podría dar lugar a muchos falsos positivos (Mario - María, Fernando – Fernández).
* Medida Jaro-Winkler. Esta variación de la medida Jaro tiene en cuenta la existencia de prefijos comunes, lo que resulta de utilidad en nombres escritos con iniciales, como “E. García López”. Funciona bien con nombres y apellidos, pero ofrece una escala de resultados con la que no es fácil establecer umbrales de aceptación. Por ejemplo, la similitud de “Álvaro” y “Alvar” sería de 0,94 y la de “Álvaro” y “A.” sería de 0,78.
* Finalmente se ha optado por una medida basada en conjuntos de caracteres, usando n-gramas (inicialmente de longitud 2) y obteniendo el coeficiente de Jaccard. Los aspectos a considerar son:
  * Reordenar la cadena de nombre + apellidos si aparece una coma. Por ejemplo, “Pérez Lara, Ángel” a “Ángel Pérez Lara”.
  * Dividir el nombre y apellidos en sus palabras, retirando stop words (de, del, la) y guiones.
  * Considerar la puntuación de las palabras con un coeficiente de Jaccard por encima de 0,5. Si no se supera, el índice resultante sería 0.
  * Otorgar un peso fijo de 0,5 al reconocimiento de una inicial (“Eduardo” y “E.”).
  * La puntuación de una palabra será 0 si no aparece en el orden adecuado.

Por ejemplo, para “Ángel Pérez Lara” podríamos obtener los siguientes candidatos, de los que consideraríamos los que superasen 0,5:
* Ángela Pérez Lara: 0,90
* A. Pérez Lara: 0,83
* Miguel Pérez Lara: 0,67.
* Ángel Pérez Rodríguez: 0,67.
* Miguel Ángel Pérez Lara: 0,625
* Ángel Pedro Pérez Laras: 0,58
* Corte
  * Ángel Pedro Pérez Talavera: 0,46
  * Ángel Pedro Pérez Calatayud: 0,46
  * Ángel Yoset Lara Pérez: 0,42

TODO: Revisar si se penaliza la no exactitud. Por ejemplo: “Luis Miguel Pérez García” con “Luis Miguel Aguirre Gómez”.
 
