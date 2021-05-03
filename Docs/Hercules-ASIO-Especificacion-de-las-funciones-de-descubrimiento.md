![](.//media/CabeceraDocumentosMD.png)

| Fecha         | 14/4/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Hércules ASIO. Especificación de las funciones de descubrimiento| 
|Descripción|Especificación de las funciones de descubrimiento|
|Versión|1.0|
|Módulo|API DISCOVER|
|Tipo|Especificación|
|Cambios de la Versión|Añadida justificación del método elegido|

# Hércules Backend ASIO. Especificación de las funciones de descubrimiento

[1 Introducción](#introducción)

[2 Arquitectura del proceso de descubrimiento](#arquitectura-del-proceso-de-descubrimiento)

[3 Reconciliación, Descubrimiento y Detección de equivalencias](#reconciliación-descubrimiento-y-detección-de-equivalencias)

[3.1 Reconciliación y carga](#reconciliación-y-carga)

[3.2 Reglas de cálculo de descubrimiento](#reglas-de-cálculo-de-descubrimiento)

[3.3 Configuración por tipo de entidad](#configuración-por-tipo-de-entidad)

[3.4 Algoritmos de similitud. Nombres y nombres propios](#algoritmos-de-similitud-nombres-y-nombres-propios)

[3.5 Tipos de Entidades en las que aplicar descubrimiento](#tipos-de-entidades-en-las-que-aplicar-descubrimiento)

[3.6 Detalle del proceso para "http://purl.org/roh/mirror/foaf#Person". Investigadores de un CV](#detalle-del-proceso-para-httppurlorgrohmirrorfoafPerson-investigadores-de-un-cv)

[4 Justificación del método elegido](#justificación-del-método-elegido)

[4.1 Resultados obtenidos](#resultados-obtenidos)

[4.2 Comparación con resultados obtenidos contra benchmarks](#comparación-con-resultados-obtenidos-contra-benchmarks)

[4.3 Conclusiones](#conclusiones)

[5 Detalle de los APIs utilizados para el descubrimiento de enlaces](#detalle-de-los-apis-utilizados-para-el-descubrimiento-de-enlaces)

[5.1 Crossref](#user-content-crossref-httpswwwcrossreforg)

[5.2 DBLP Computer Science Bibliography](#user-content-dblp-computer-science-bibliography-httpsdblporg)

[5.3 DOAJ](#user-content-doaj-httpsdoajorg)

[5.4 ORCID](#user-content-orcid-httpsorcidorg)

[5.5 PubMed](#user-content-pubmed-httpspubmedncbinlmnihgov)

[5.6 Recolecta](#user-content-recolecta-httpsrecolectafecytes)

[5.7 Scopus](#user-content-scopus-httpswwwscopuscom)

[5.8 Web of Science](#user-content-web-of-science-httpwosfecytes)

Introducción
============
El API Descubrimiento ofrece unas funciones que son parte del proceso de carga. Estas funciones se dividen en 3 grupos:
-	Reconciliación de entidades, que evita la duplicación de entidades y puede añadir datos desde otros nodos ASIO, a través de las entidades incorporadas en el nodo Unidata.
-	Descubrimiento de enlaces, que genera enlaces hacia datasets externos (incluidos los de otros datasets ASIO a través del nodo Unidata), puede incorporar datos en ASIO y ofrece información de ayuda en la reconciliación de entidades.
-	Detección de equivalencias, que genera equivalencias semánticas.

Los 3 grupos de funciones se desarrollarán al mismo tiempo, ya que todos ellos actúan en el proceso de descubrimiento para todos los datos a cargar en ASIO.

Arquitectura del proceso de descubrimiento
=====================================

Los procesos de carga de datos desde el SGI hacia el Backend ASIO
responden al siguiente esquema de arquitectura:

![](.//media/image2_FuncionesCarga.png)

Dentro de esta estructura, el API de descubrimiento reconcilia, descubre enlaces y detecta equivalencias;
y se encarga de enviar los triples definitivos hacia el RDF Store. Para ello, lee de una cola de Rabbit MQ los RDF pendientes de procesar, que han sido introducidos a través del método POST etl​/data-publish del API CARGA.

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
       2.	Se eliminan todos los triples de las entidades secundarias cuyo sujeto y predicado estén en el RDF a cargar y estén marcados como monovaluados según la especificación de la ontología. En la próxima implementación se añadirá una excepción para las propiedades foaf:name y roh:title que no se eliminarán y TAMPOCO se insertarán.
       3.	Se vuelcan los triples al grafo RDF. En la próxima implementación se contemplará la excepción de las propiedades foaf:name y roh:title de las entidades secundarias que no se hayan eliminado en el paso anterior. En el caso de las entidades secundarias nuevas sí que hay que cargar sus títulos.

Reglas de cálculo de descubrimiento
-----------------------
El flujo de acciones de descubrimiento, que comienza con la reconciliación de entidades, tiene estos pasos y reglas:
1.	Si la entidad cuenta con algún identificador (p.e. ORCID), reconciliar la entidad a través de alguno de sus identificadores. Si existe alguna entidad cargada en el nodo ASIO que comparta identificador se considera la misma entidad.
2.	Si no se ha encontrado la entidad, se buscan similitudes con entidades ya cargadas en ASIO. Para cada tipo de entidad (rdf:type) se definirán atributos y relaciones para la búsqueda de similitudes. Por ejemplo, el nombre del investigador y el paper con el que este investigador se incorporaría en ASIO. Esta configración está definida más adelante en el punto [Configuración por tipo de entidad](#configuración-por-tipo-de-entidad).
3.  Umbrales generales para todas las entidades:
    1.	Umbral mínimo para considerar la similitud suficiente para que sea una entidad candidata a reconciliar. Si se alcanza, la valoración obtenida se podría reforzar en los siguientes pasos de descubrimiento.
    2.	Umbral máximo para considerar que es la misma entidad.
4.	En el caso de que no se haya encontrado la entidad o sólo se hayan encontrado entidades candidatas que alcanzan el umbral mínimo, se utilizará el descubrimiento de enlaces para intentar la reconciliación. 
    1.	(Próxima implementación) Nodo Unidata. La entidad podría estar cargada en Unidata procedente de otra universidad.
        1.	La entidad cuenta con algún identificador (p.e. ORCID) y no había una entidad candidata. Consideramos que es la misma entidad y procedemos a enriquecer los datos del nodo de ASIO con lo que hubiera en Unidata.
        2. La entidad cuenta con algún identificador (p.e. ORCID) pero había una entidad candidata. En este caso, tenemos que considerar que podría no ser la misma entidad. Por ejemplo, supongamos que la entidad a reconciliar es el investigador Luis Pérez con el ORCID XXX. Podría ser que en el nodo ASIO ya tuviéramos un Luis Pérez, pero sin código ORCID. No se puede considerar que sea la misma persona. Sin embargo, sí consideraríamos que sería la única entidad candidata a serlo de las existentes en Unidata.
        3. Se buscan similitudes con entidades ya cargadas en Unidata, en función del tipo de entidad, del mismo modo que se ha hecho antes en el nodo ASIO. Por ejemplo, el paper con el que llega este investigador podría estar ya cargado en Unidata. El resultado sería una nueva propuesta de entidades con su valoración de la similitud, a sumar al obtenido de ASIO (si lo hubiera).
    2.	Datasets externos. La entidad estará, probablemente, en alguna de las fuentes externas configuradas en el descubrimiento de enlaces. 
        1. La entidad cuenta con algún identificador (p.e. ORCID) y no había una entidad candidata. Consideramos que es la misma entidad y procedemos a enriquecer y enlazar los datos del nodo de ASIO con lo que hubiera en el dataset externo.
        2. La entidad cuenta con algún identificador (p.e. ORCID) pero había una entidad candidata. De la misma forma que hemos indicado para el nodo Unidata, no podemos considerar que sea la misma entidad, aunque sí sería la única candidata de los datasets externos.
        3. Mediante los APIs de descubrimiento se intentarían recuperar datos adicionales de la(s) entidad(es) candidata(s) con los que intentaría la reconciliación y se obtendría una nueva valoración que añadir, en su caso, a la obtenida de pasos anteriores.
5.	Además de intentar reconciliar las entidades con los datos cargados en ASIO y en Unidata se enriquecen los datos usando datasets Externos.
	1.	ORCID: Se enriquecerá el RDF de carga con el API de ORCID (https://pub.orcid.org/v3.0/) añadiendo identificadores a las personas del RDF (http://purl.org/roh#ORCID, http://purl.org/roh/mirror/vivo#researcherId y http://purl.org/roh/mirror/vivo#scopusId). Para identificar en ORCID a las personas se aplicarán las mismas reglas de similitud configuradas para el descubrimiento con los datos procedentes de ORCID (nombres de personas y nombres de publicaciones).
6.	Terminados los pasos anteriores, podrían darse los siguientes casos:
    1.	Una entidad ha superado el umbral máximo. Se considera que es la misma entidad y se continúa con la carga con los datos adicionales obtenidos.
    2.	Más de una entidad ha superado el umbral máximo. Se incluye en la lista de entidades sobre las que tendría que decidir un administrador.
    3.	Una o más entidades han superado el umbral mínimo. Se incluyen en la lista de entidades sobre las que tendría que decidir un administrador.
    4.	Ninguna entidad ha superado el umbral mínimo. La entidad se considera nueva, y se continúa con el proceso de carga con los datos adicionales obtenidos en los procesos anteriores.

Configuración por tipo de entidad
-------------------
Para la reconciliación de entidades se realiza una configuración por cada tipo de entidad de las propiedades que deben coincidir para considerar que se trata de la misma entidad. Estas propiedades pueden ser directas o inversas y pueden tener N saltos (ver formato de configuración para entidad de tipo persona en [Formato de configuración en el fichero reconciliationConfig.json](../src/Hercules.Asio.Api.Discover/README.md#formato-de-configuraci%C3%B3n-en-el-fichero-reconciliationconfigjson)).
Esta concidencia puede ser de 4 tipos:
1.	Equals (0): El valor de la propiedad es exactamente el mismo. Sirve para cualquier tipo de valor de propiedad: textos, fechas, otras entidades...

Ejemplo:
	
	{
		"rdfType": "http://purl.org/roh/mirror/foaf#Person",
		"properties": [
		{
			"property": "http://purl.org/roh/mirror/bibo#authorList@@@?",
			"mandatory": false,
			"inverse": true,
			"type": 0,
			"maxNumWordsTitle": null,
			"scorePositive": 0.5,
			"scoreNegative": null
		}
	}
Está configuración sería para comparar los documentos de los que una persona es autor.	
	
2.	IgnoreCaseSensitive (1): El valor de la propiedad es exactamente el mismo (ignorando mayúsculas y minúsculas).

Ejemplo:
	
	{
		"rdfType": "http://purl.org/roh/mirror/foaf#Person",
		"properties": [
		{
			"property": "http://purl.org/roh/mirror/foaf#name",
			"mandatory": true,
			"inverse": false,
			"type": 1,
			"maxNumWordsTitle": null,
			"scorePositive": 0.89,
			"scoreNegative": null
		}
	}
Está configuración sería para comparar los nombres de las personas ignorando las mayúsculas y minúsculas.

3.	Name (2): Utilizado para nombres de personas, tiene en cuenta abreviaturas, más o menos apellidos.... El detalle del algoritmo está en el apartado [Algoritmos de similitud. Nombres y nombres propios](#algoritmos-de-similitud-nombres-y-nombres-propios)

Ejemplo:
	
	{
		"rdfType": "http://purl.org/roh/mirror/foaf#Person",
		"properties": [
		{
			"property": "http://purl.org/roh/mirror/foaf#name",
			"mandatory": true,
			"inverse": false,
			"type": 2,
			"maxNumWordsTitle": null,
			"scorePositive": 0.89,
			"scoreNegative": null
		}
	}
	
Está configuración sería para comparar los nombres de las personas utilizando al algoritmo especificado en el apartado [3.4 Algoritmos de similitud. Nombres y nombres propios](#algoritmos-de-similitud-nombres-y-nombres-propios).
	
4.	Title (3): Utilizado para nombres/títulos en los que el valor debe ser el mismo ignorando caracteres especiales, mayúsculas, minúsculas y acentos.
    1.	En este caso, además hay que establecer el nº de palabras para considerar que la similitud es del 100%.
   
Ejemplo:
	
	{
		"rdfType": "http://purl.org/roh/mirror/bibo#Document",
		"properties": [
		{
			"property": "http://purl.org/roh#title",
			"mandatory": true,
			"inverse": false,
			"type": 3,
			"maxNumWordsTitle": 10,
			"scorePositive": 0.9,
			"scoreNegative": null
	      	}
	}

Está configuración sería para comparar los títulos de los documetnos ignorando caracteres especiales, mayúsculas, minúsculas y acentos.

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

Próxima implementación: Decidir si se penaliza la no exactitud. Por ejemplo: “Luis Miguel Pérez García” con “Luis Miguel Aguirre Gómez”.

Tipos de Entidades en las que aplicar descubrimiento
--------------------
Los casos de uso identificados se corresponden con las entidades principales de la ontología, que son:

    http://purl.org/roh#Accreditation (Acreditación)
        http://purl.org/roh#LanguageCertificate (Certificado de Idioma)
    http://purl.org/roh#Activity (Actividad)
        http://purl.org/roh/mirror/vivo#Course (Curso)
    http://purl.org/roh/mirror/foaf#Agent (Agente)
        http://purl.org/roh/mirror/foaf#Organization (Organización)
        http://purl.org/roh/mirror/foaf#Person (Persona)
    http://purl.org/roh/mirror/geonames#Feature (Característica)
    http://purl.org/roh#CurriculumVitae (Curriculum Vitae)
    http://purl.org/roh/mirror/foaf#Document (Documento)
    http://purl.org/roh/mirror/obo/iao#IAO_0000030 (Entidad de Contenido de Información)
    http://purl.org/roh#Funding (Financiación)
    http://purl.org/roh#FundingSource (Fuente de financiación)
    http://purl.org/roh#Expense (Gasto)
    http://purl.org/roh/mirror/vivo#AcademicDegree (Grado académico)
    http://purl.org/roh#Infraestructure (Infraestructura)
    http://purl.org/roh#FundingAmount (Monto de financiación)
    http://purl.org/roh#Metric (Métrica)
    http://purl.org/roh#ResearchObject (Objeto de financiación)
    http://purl.org/roh#FundingProgram (Programa de financiación)
    http://purl.org/roh/mirror/vivo#Project (Proyecto)
    http://purl.org/roh/mirror/obo/bfo#BFO_0000008 (Región Temporal)
    http://purl.org/roh/mirror/vivo#Relationship (Relación)
        http://purl.org/roh/mirror/vivo#Contract (Contrato)
        http://purl.org/roh/mirror/vivo#Position (Posición)
        http://purl.org/roh/mirror/vivo#AdvisingRelationship (Relación de asesoramiento)
        http://purl.org/roh#AuditingRelationship (Relación de auditoría)
        http://purl.org/roh#SupervisingRelationship (Relación de supervisión)
        http://purl.org/roh/mirror/vivo#AwardedDegree (Titulación Universitaria Concedida)
    http://purl.org/roh/mirror/obo/bfo#BFO_0000023 (Rol)
    http://purl.org/roh/mirror/obo/ero#ERO_0000005 (Servicio)
    http://purl.org/roh/mirror/vcard#Kind (Tipo)

1/10/2020: En este momento, se realiza el descubrimiento sobre las entidades "http://purl.org/roh/mirror/foaf#Person", "http://purl.org/roh/mirror/foaf#Organization", "http://purl.org/roh/mirror/vivo#Project", "http://purl.org/roh/mirror/bibo#Document", "http://purl.org/roh#Activity", "http://purl.org/roh/mirror/vivo#AcademicDegree", obtenidas de un curriculum en formato CVN.


Detalle del proceso para "http://purl.org/roh/mirror/foaf#Person". Investigadores de un CV
--------------------
Describimos aquí el proceso para un investigador que nos llega en un CV del que sólo tenemos el nombre y un paper que tiene en común con el dueño del CV. 
Entre otras configuraciones, en este caso, para el caso de los investigadores (http://purl.org/roh/mirror/foaf#Person) se utilizarán dos configuraciones para la similitud: 
1. Similitud por nombre:

	{
		"property": "http://purl.org/roh/mirror/foaf#name",
		"mandatory": true,
		"inverse": false,
		"type": 2,
		"maxNumWordsTitle": null,
		"scorePositive": 0.89,
		"scoreNegative": null
	}    
    
A través de un algoritmo, [Algoritmos de similitud. Nombres y nombres propios](#algoritmos-de-similitud-nombres-y-nombres-propios), se obtendrá la similitud (un valor entre 0% y 100%) entre el nombre del investigador a cargar y el de otros investigadores cargados. Estará establecido un valor mínimo (p.e. 70%) que indicará que es probable que se trate de la misma entidad pero que habrá que reforzar con el descubrimiento para poder llegar a la conclusión de que se trata de la misma entidad y un valor máximo (p.e. 90%) que implicará que se trata de la misma entidad. Dado que esta propiedad está configurada como "mandatory" (obligatoria), es requisito imprescindible que los investigadores candidatos cumplan esta similitud.

2. Similitud por publicaciones: 

	{
        	"property": "http://purl.org/roh#correspondingAuthorOf",
        	"mandatory": false,
 		"inverse": false,
        	"type": 0,
        	"maxNumWordsTitle": null,
        	"scorePositive": 0.5,
        	"scoreNegative": null
	}      
      
Obtendrá las publicaciones que tienen en común el investigador del RDF a cargar con los investigadores candidatos cargados en el Triple Store. 

Una vez explicadas las configuraciones de similitud que actuarán para este caso de uso en particular, este sería el flujo de reconciliación utilizado:

1. En primer lugar, se intentaría reconciliar la entidad a través de alguno de sus identificadores, en este caso no lo tenemos, por lo tanto, en este punto no se consigue la reconciliación. 
2. En segundo lugar, se intenta la reconciliación en base a similitudes entre la entidad a cargar y las entidades cargadas en ASIO. Para ello se buscan candidatos dentro del grafo de ASIO de la universidad que cumplan la primera restricción 'similitud por nombre' (http://purl.org/roh/mirror/foaf#name) y de todos ellos obtenemos sus publicaciones (http://purl.org/roh#correspondingAuthorOf). Una vez obtenidos todos los candidatos obenemos la probabilidad de que se trate de la misma entidad sobre cada uno de ellos.
    1. En caso de encontrar sólo un investigador cuya similitud supere el umbral máximo de similitud (0.9) se considera que es la misma entidad por lo que terminamos la reconciliación. 
    2. En caso de encontrar más de investigador que supere el umbral máximo de similitud (0.9) o algún investigador que supere el umbral mínimo de similitud (0.7) pero no alcance el umbral máximo de similitud (0.9) no podemos reconciliar la entidad automáticamente. 
3. En tercer lugar, se intenta la reconciliación en base al descubrimiento de enlaces, para ello se busca información en Unidata (Pendiente de implementar) y en los APIs de descubrimiento de enlaces (Crossref, DBLP Computer Science Bibliography, DOAJ, ORCID, PubMed, Recolecta, Scopus y Web of Science) del investigador a cargar y de los investigadores cargados (en caso de que se haya encontrado algún investigador cargado que supere el umbral mínimo de similitud).  
    1. En caso de que a través de Unidata o de los APIs de descubrimiento de enlaces se encuentren identificadores para el investigador se vuelve a proceder con el punto 1 de la reconciliación y si se encuentra algún investigador cargado en ASIO que comparta identificador (ORCID) se considera que es la misma entidad por lo que terminamos la reconciliación. 
    2. En caso de que a través del descubrimiento se encuentre al investigador y papers publicados por el mismo, se vuelve a intentar la reconciliación del punto 2. En este punto podríamos tener que en el grafo cargado había un investigador cuyo nombre alcanzaba el umbral mínimo de similitud con el investigador a cargar, pero no tuviesen ningún paper cargado en común con el paper que se está cargando, sin embargo, tras obtener la información adicional del descubrimiento podríamos obtener varios papers del investigador a cargar y alguno de ellos coincidiese con el paper que tiene la entidad que ya estaba cargada en el grafo de la universidad. Por lo tanto, se considera que es la misma entidad por lo que terminamos la reconciliación. 
4. Si tras todos estos intentos de reconciliación no se ha conseguido encontrar el investigador se procede del siguiente modo: 
    1. En caso de que no se haya encontrado el umbral máximo de similitud del investigador, pero sí que se haya alcanzado el umbral mínimo se pondrá la carga de RDF en el que estaba la entidad en ‘cuarentena’ a la espera de que un administrador decida si se trata de la misma entidad o no. 
    2. En caso de que se haya encontrado más de una entidad que supere el umbral máximo se pondrá la carga de RDF en el que estaba la entidad en ‘cuarentena’ a la espera de que un administrador decida cuál de las entidades es la equivalente. 
    3. En el caso de que ni siquiera se haya alcanzado el umbral mínimo de similitud, se considera que se trata de un investigador nuevo en el sistema por lo que se procede a su carga como si se tratase de una entidad nueva, además se cargará en el sistema con información adicional obtenida en el descubrimiento (de Unidata o de los APIs) en el caso de que se haya encontrado (ORCID, áreas temáticas, etc.).


Justificación del método elegido
===================

De los tres procesos de descubrimiento, la reconciliación podría haberse afrontado mediante algún desarrollo que utilizase técnicas de _machine learning_ autónomo o supervisado. Sin embargo y como se ha explicado en el apartado anterior, se ha optado por un método que utiliza un conjunto de reglas predefinidas sobre las que un administrador o desarrollador podría actuar cambiando ciertos parámetros o mediante reprogramación de algunos comportamientos.

Consideramos que el problema que hay que solucionar en este caso tiene unas características específicas que lo hacen adecuado para un sistema de reglas que, en cualquier caso, si hace uso de la naturaleza de grafo de los datos para obtener sus resultados. Estas características son:

- La información provendrá, en la mayoría de los casos, de un SGI (preferentemente Hércules SGI), por lo que podemos esperar un conformado de los datos correcto.
- Disponemos de sistemas externos con los que obtener información adicional para el proceso de reconciliación (descubrimiento de enlaces), en las que los usuarios y sistemas publican con un nivel de calidad de datos alto.
- La información incorpora, frecuentemente, datos de códigos únicos y ampliamente aceptados para investigadores (ORCID y otros) y documentos (DOI).
- En el peor de los casos, los datos vendrán de la autoedición de su CV por parte de los investigadores (formato CVN). En principio, esto supone un interés alto por parte del usuario en la generación de información precisa y, por tanto, una alta corrección en los datos.

Es decir, no se trata de desarrollar un sistema "ciego" de reconciliación en el que la calidad de los datos es incierta, sino que se trata de información altamente formalizada y susceptible de ser reconciliada mediante reglas que tienen en cuenta su tipología y las relaciones entre las entidades y sus datos.

Resultados obtenidos
--------------

El método elegido se ha puesto a prueba con los curriculum proporcionados por la UM en formato CVN. Este sería el caso más complicado de resolución, ya que en un mismo CV nos encontraremos con muchas publicaciones, ROs y, especialmente, investigadores para los que, generalmente, solo se dispone de nombre y apellidos.

En el caso del CVN, la reconciliación se hace en dos pasos. En primer lugar, dentro del propio curriculum, ya que se pueden repetir entidades, particularmente personas. En segundo, con las entidades ya existentes en el grafo. Los resultados han sido:

|Reconciliación de artículos dentro del CV| |
|:----|:----|
|Artículos diferentes en el CV|934|
|Artículos diferentes en el CV (reconciliación en el CV)|876|
|Artículos reconciliados dentro del CV|58|
|Artículos reconciliados dentro del CV que no eran correctos (fallos)|0|
|Artículos NO reconciliados dentro del CV (fallos)|7|
|Precisión|1|
|Cobertura|0,9920|

|Reconciliación de artículos con existentes en el grafo| |
|:----|:----|
|Artículos reconciliados con la BBDD|33|
|Artículos reconciliados dentro de la BBDD que no eran correctos (fallos)|0|
|Articulos NO reconciliados con la BBDD (propuestos para reconciliar manualmente)|0|
|Precisión|1|
|Cobertura|1|

|Reconciliación de personas dentro del CV| |
|:----|:----|
|Personas diferentes en el CV|868|
|Personas diferentes en el CV (reconciliación RDF)|831|
|Personas reconciliadas dentro del CV|37|
|Personas reconciliados dentro del CV que no eran correctos (fallos)|2|
|Personas NO reconciliadas dentro del CV (fallos)|41|
|Precisión|0,9977|
|Cobertura|0,9507|

|Reconciliación de personas con existentes en el grafo| |
|:----|:----|
|Personas reconciliadas con la BBDD|47|
|Personas reconciliadas dentro de la BBDD que no eran correctas (fallos)|0|
|Personas NO reconciliadas con la BBDD (propuestas para reconciliar manualmente)|12|
|Precisión|1|
|Cobertura|0,7966|


La explicación de los fallos es:
- Cobertura de artículos reconciliados en el CV (0,9920). Se trata de errores ortográficos ya que dentro del curriculum los títulos tienen que ser iguales para considerarse el mismo.
- Precisión de personas reconciliadas en el CV (0,9977). Los investigadores comparten parte del nombre y tienen coautores en común.
- Cobertura de personas reconciliadas en el CV (0,9507). Las personas tienen el mismo nombre, pero no tienen resultados de investigación en común. Con esta información, tampoco una persona en un proceso manual podría asegurar de que se trata de la misma persona.
- Cobertura de personas reconciliadas con personas existentes en el grafo (0,7966). El sistema no las ha reconocido con suficiente fiabilidad, pero las propone para una resolución manual.

Comparación con resultados obtenidos contra benchmarks
--------------------

Hemos tomado como referencia los resultados de [Ontology Alignment Evaluation Initiative (OAEI)](http://oaei.ontologymatching.org/), en relación con el problema de _instance matching_ para el track SPIMBENCH en los años [2019](https://project-hobbit.eu/challenges/om2019/) y [2020](https://hobbit-project.github.io/OAEI_2020.html#spimbench-1).

Como se puede ver, los resultados de 2020 son algo mejores, oscilando los resultados de precisión entre 0,8349 y 0,9383 en 2019; y 0,8349 y 1 en 2020. En cuanto a cobertura, va de 0,7625 a 1 en 2019; y de 0,7095 a 1 en 2020.

Como se puede observar, los resultados obtenidos mediante reglas son comparables, en este caso, a los que se obtienen contra el benchmark SPIMBENCH de referencia en OAEI.

Conclusiones
-----------

Los algoritmos y aproximaciones utilizados contra SPIMBENCH, de propósito general, necesitarían adaptarse a algunas peculiaridades de los datos. Por ejemplo, se puede dar el caso de dos entidades con el mismo o muy parecido nombre sean distintas (publicaciones y autores con nombres muy parecidos); y que dos entidades sean muy parecidas en nombre y compartan autores, pero sean distintas en realidad (p.e. publicaciones con un nombre en el que sólo varía el año, siendo el resto del título igual y compartiendo autores).

También cabe citar que los algoritmos utilizados contra SPIMBENCH necesitarían una arquitectura más compleja para obtener un resultado dudosamente mejor que con la solución que proponemos.

La solución propuesta, basada en reglas, sería configurable para resolver otros problemas, pero está especialmente adaptada al caso que nos ocupa, lo que ofrece un resultado muy similar al que ofrecería una revisión humana. Además, los parámetros del sistema son configurables, según lo indicado anteriormente, lo que ofrecería la posibilidad de generar un funcionamiento más o menos desatendido. Por ejemplo, se podría reducir el umbral de decisión para que la reconciliación con los investigadores ya cargados en el grafo fuera más automática.


    
Detalle de los APIs utilizados para el descubrimiento de enlaces
===================
El funcionamiento es similar en el uso de todos los APIs, se obtienen del RDF a cargar los documentos junto con sus autores y se buscan en los diferente APIs las coincidencias de documentos+autores, no se buscan los nombres de las personas ni los nombres de los autores si no existe una relación entre ambos ya que la coincidencia de un nombre o un título por sí solos no es suficiente para considerarlos la misma entidad.

Las pequeñas diferencias en los usos de los APIs están derivadas de la naturaleza propia de cada uno de ellos (en alguno se puede buscar por documentos, en otros por personas...).

Los datos incorporados desde fuentes externas se cargan junto con triples que indican su procedencia, según lo especificado en el documento [Hércules Backend ASIO. Procedencia de datos - Provenance](Hercules-ASIO-Procedencia-de-datos-externos-Provenance.md).

A continuación se detallan los diferentes APIs utilizados junto con su URL, una breve descripción y el uso que hacemos dentro del servicio de descubrimiento:

Crossref (https://www.crossref.org/)	
--------------------
Crossref makes research outputs easy to find, cite, link, assess, and reuse. A not-for-profit membership organization that exists to make scholarly communications better.

Dentro de discover se hacen llamadas al método del API 'https://api.crossref.org/works?query.author={nombre_autor}&rows=200' y se obtienen las publicaciones que tienen como autor el parámetro pasado.

DBLP Computer Science Bibliography (https://dblp.org/)	
--------------------
The dblp computer science bibliography is the on-line reference for bibliographic information on major computer science publications. It has evolved from an early small experimental web server to a popular open-data service for the whole computer science community.

Dentro de discover se hacen llamadas al método del API 'https://dblp.org/search/author/api?q={nombre_autor}&h=5' y se obtienen los autores junto con sus publicaciones, a continuación se llama al método 'https://dblp.org/pid/{id_autor}' con los identificadores de DBLP de los autores para obtener más metadatos.

DOAJ (https://doaj.org/)	
--------------------
The DOAJ (Directory of Open Access Journals) was launched in 2003 with 300 open access journals. Today, this independent database contains over 15 000 peer-reviewed open access journals covering all areas of science, technology, medicine, social sciences, arts and humanities. Open access journals from all countries and in all languages are welcome to apply for inclusion.

Dentro de discover se hacen llamadas a los métodos del API 'https://doaj.org/api/v2/search/articles/title:{nombre_documento}' y 'https://doaj.org/api/v2/search/journals/title:{nombre_documento}' y se obtienen las publicaciones junto con sus autores.

ORCID (https://orcid.org/)	
--------------------
ORCID is a nonprofit organization helping create a world in which all who participate in research, scholarship and innovation are uniquely identified and connected to their contributions and affiliations, across disciplines, borders, and time.

Dentro de discover se hacen llamadas al método del API 'https://pub.orcid.org/v3.0/expanded-search?q={nombre_autor}&rows=5' para obtener los identificadores de los autores y posteriormente se hacen llamadas a los métodos del API 'https://pub.orcid.org/v3.0/{id_autor}/person' y 'https://pub.orcid.org/v3.0/{id_autor}/works para obtener metadatos de los autores y sus obras respectivamente.

PubMed (https://pubmed.ncbi.nlm.nih.gov/)	
--------------------
PubMed® comprises more than 30 million citations for biomedical literature from MEDLINE, life science journals, and online books.

Dentro de discover se hacen llamadas al método del API 'https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?term={nombre_documento}&field=title&sort=relevance&retmax=10' con el que obtenemo los IDs de los documentos y posteriormente se hacen llamadas al método del API 'https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id={id_documento}&retmode=xml' con los identificadores de los documentos y obtenemos los documetnos junto con sus autores.

Recolecta (https://recolecta.fecyt.es/)	
--------------------
RECOLECTA, o Recolector de Ciencia Abierta, es el agregador nacional de repositorios de acceso abierto. En esta plataforma se agrupan a todas las infraestructuras digitales españolas en las que se publican y/o depositan resultados de investigación en acceso abierto. 

Dentro de discover se hacen llamadas al método del API 'https://buscador.recolecta.fecyt.es/buscador-recolecta?search_api_fulltext={nombre_documento}' y se obtienen las publicaciones junto con sus autores.

Scopus (https://www.scopus.com/)	
--------------------
Scopus is the largest abstract and citation database of peer-reviewed literature: scientific journals, books and conference proceedings. Delivering a comprehensive overview of the world's research output in the fields of science, technology, medicine, social sciences, and arts and humanities, Scopus features smart tools to track, analyze and visualize research.

Dentro de discover se hacen llamadas al método del API 'https://api.elsevier.com/content/search/scopus?query=TITLE({nombre_documento})&view=COMPLETE&apiKey={ScopusApiKey}&httpAccept=application/xml' y se obtienen las publicaciones junto con sus autores, posteriormente se llama al método del API 'https://api.elsevier.com/content/author/author_id/{id_autor}?apiKey={ScopusApiKey} para obtener metadatos de los autores.

Web of Science (http://wos.fecyt.es/)	
--------------------
FECYT provides access to Web of Science, the world’s largest publisher-neutral citation index and research intelligence platform

Dentro de discover se hacen llamadas al método del API 'http://search.webofknowledge.com/esti/wokmws/ws/WokSearch' con los nombres de las publicaciones y se obtienen las publicaciones junto con sus autores.

