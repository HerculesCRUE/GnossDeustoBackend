![](../../Docs/media/CabeceraDocumentosMD.png)

# Introducción

Este documento da acceso a la implementación y ejecución de las preguntas de competencia que han sido desplegadas sobre un Triple Store [Apache Jena Fuseki](https://jena.apache.org/documentation/fuseki2/) sobre el que se ha habilitado el [razonador Pellet Openllet](https://github.com/Galigator/openllet/). Además, también documenta las adaptaciones que han sido realizadas para permitir la ejecución de las consultas sobre un Triple Store RDF que no soporte algunas características avanzadas de razonamiento, como sucede en este caso con [Openlink Virtuoso](https://virtuoso.openlinksw.com/). El dataset con los datos de prueba es [data.ttl](https://github.com/HerculesCRUE/GnossDeustoOnto/blob/master/examples/data.ttl)

# Carga de datos en Virtuoso

Dadas las [limitaciones de razonamiento de la versión open source de Virtuoso](http://docs.openlinksw.com/virtuoso/virtuosofaq6/), se han tenido que realizar algunas variaciones sobre las consultas SPARQL. Estas consultas se encuentran en el directorio `virtuoso-sparql`. 

En primer lugar, hay que cargar en Virtuoso el dataset previamente generado que contiene todas las tripletas inferidas. Para este ejemplo se ha utilizado el grafo `http://graph.um.es/validation-questions`. Además, se carga la instancia de Geonames necesaria para ejecutar los ejemplos, para poder realizar las consultas solamente con SPARQL, y sin intervención de otros lenguajes de programación:

```
ld_dir ('/ruta/del/dataset/inferido/', '*.owl', 'http://graph.um.es/validation-questions');
rdf_loader_run();
sparql load <https://sws.geonames.org/3128026/> into <http://graph.um.es/validation-questions>;
```

A la hora de ejecutar las consultas, en el punto de SPARQL de Virtuoso (http://155.54.239.204:8890/sparql) hay que deshabilitar la opción `Strict checking of void variables` para que las consultas funcionen correctamente.

# Preguntas de competencia

A continuación, se exponen las preguntas de competencia y su ejecución online:

* Q1 - Centros de  investigación que trabajan en un área/disciplina específica: https://tinyurl.com/yc5pvrn5
* Q2 - Listado de los investigadores de un centro/estructura de investigación de un área/disciplina específica y su posición: https://tinyurl.com/y9w5gykd
* Q3 - Sin implementar.
* Q4 - Centros/estructuras de investigación que posean sellos de calidad asociados: https://tinyurl.com/yc3zlhoy
* Q5 - Listado de los centros/estructuras de investigación que hayan realizado proyectos y su respectiva convocatoria: https://tinyurl.com/y97q9b6s
* Q6 - Listado de la producción científica en un determinado rango de fechas de un centro/estructura de investigación en un área/disciplina: https://tinyurl.com/ycszunzv
* Q7 - Artículos publicados en revistas, según las comunidades autónomas: https://tinyurl.com/y9ejsa88
* Q8 - Producción científica (ResearchObjects) de un grupo de investigación agrupados por tipo, es decir, publicaciones, patentes: https://tinyurl.com/ydcqxmpu
* Q9 - Listado de patentes, diseños industriales, etc. de un centro/estructura de investigación en un área/disciplina: https://tinyurl.com/yd2yf8xp
* Q10 - Listado de los proyectos adjudicados/desarrollados, de un centro/estructura de investigación, de un área/disciplina, en un determinado año de búsqueda: https://tinyurl.com/y77cut74
* Q11A - Encontrar el research object más antiguo organización: https://tinyurl.com/ydx4tt9c
* Q11B - Encontrar el research object más reciente de una organización: https://tinyurl.com/y9hen48l
* Q12 - Listar proyectos agrupados por ámbito geográfico: https://tinyurl.com/y9onbpqa
* Q13 - Dado un proyecto listar los documentos de su dossier: https://tinyurl.com/y9onbpqa
* Q14 - Diferentes consultas sobre proyectos: Implementada por Q5, Q10 y Q12.
* Q15 - Listar proyectos con el mismo subject area o con subject areas relacionadas por parentesco, mirando en el árbol UNESKOS: https://tinyurl.com/y7al28b5
* Q16A - Dada una persona listar proyectos en los que ha intervenido filtrados por periodo y/o organización: https://tinyurl.com/yaxswxce
* Q16B - Dada una persona listar research objects a los que ha contribuido, filtrados por periodo y/o organización: https://tinyurl.com/y7dlllfu
* Q17 - Dada una organización, en un periodo de tiempo, listar research projects o proyectos: https://tinyurl.com/y72gcv2q
* Q18 - Contar las publicaciones con índice de impacto de un usuario en un periodo de tiempo: https://tinyurl.com/y8sq37ss
* Q19 - Sin implementar.
* Q20 - Listar las áreas de conocimiento más abordadas por una organización en proyectos o publicaciones en un periodo de tiempo: https://tinyurl.com/yd2amby5
* Q21 - Contar research objects de diferentes tipos o proyectos a nivel persona, línea, área de conocimiento u organización: implementada por Q6, Q8, Q11A, Q11B, Q16B.
* Q22 - Sin implementar.
* Q23 - Ordenar por contador de pubicaciones de impacto o proyectos europeos línea de investigación asociados a diferentes líneas o áreas de conocimiento: implementada por Q20.
* Q24 - Dada una empresa encontrar los proyectos en los que ha colaborado con los grupos de una universidad: https://tinyurl.com/yczu3n73
* Q25 - Obtener el listado de las tesis doctorales que he dirigido: https://tinyurl.com/y9bwy6s9
* Q26 - Obtener el listado de congresos/workshops y eventos de divulgación científica en los que haya participado indicando el rol que he tenido: organizador, expositor, etc.: https://tinyurl.com/yaby664q
* Q27 - Obtener el listado de patentes, diseños industriales, etc. que haya registrado como titular o cotitular X o Y persona, Z o K institución: https://tinyurl.com/y7xhgwpv
* Q28 - Obtener el listado de proyectos en los que he participado incluyendo el rol que he desempeñado, por ejemplo, investigador principal: https://tinyurl.com/y8534q4e
* Q29 - Obtener el listado de mi producción científica: https://tinyurl.com/ydgclj5a
* Q30 - Sin implementar.
* Q31 - Obtener los indicadores de mi producción científica como, por ejemplo, total de citas, h-index, etc.: https://tinyurl.com/y9pvvwam
* Q32 - Listar proyectos en los que una persona ha participado en un periodo de tiempo: implementada por Q16A.
* Q33 - Dado un periodo de 6 años devuelveme el número de academic articles con factor de impacto y determina si es mayor 5, pudiendo filtrar por cuartil, considerando de Q3 para arriba: https://tinyurl.com/yckep25z
* Q34 - Proyecto en estado PROPOSAL_SUBMITTED dirigidas a una empresa e incluso detalles económicos de la misma, el Funding propuesto y los Funding Amounts associados: https://tinyurl.com/y9tus8k4
* Q35A - Listar los documentos de justificación asociados a un proyecto: https://tinyurl.com/y8ogyj7l
* Q35B - Listar las fechas de justificación de un proyecto: https://tinyurl.com/yahgfgpt
* Q35C - Listar los gastos de un proyecto: https://tinyurl.com/y6u46ew4
* Q36 - Listar los grupos ordenados por financiación recibida: https://tinyurl.com/yc57ukou
* Q37 - Listar número de proyectos y/o publicaciones compartidas entre 2 o más organizaciones: https://tinyurl.com/y9cantuf
* Q38 - Sin implementar.
* Q39 - Financiación atraída en unos años por todos los investigadores del área de conocimiento X: https://tinyurl.com/yat4p2ce
* Q40 - Sin implementar.
* Q41 - Sin implementar.
* Q42 - Investigadores que tienen ERCs, Marie Curie, etc.: https://tinyurl.com/y7zkl9j2
* Q43 - Identificar qué universidades de la red cuentan con la distinción de excelencia Severo Ochoa, María de Maeztu o las equivalentes: implementada por Q4.
* Q44 - Cuantificar los proyectos en convocatorias competitivas de un grupo de investigación en un rango de años con grupos de investigación de otras Universidades: https://tinyurl.com/y9obcvre
* Q45 - Obtener el total de publicaciones de impacto por mujeres y hombres: https://tinyurl.com/yd7w3c6n
* Q46 - Sin implementar.
* Q47 - Sin implementar.
* Q48 - Obtener los indicadores de la producción científica como, por ejemplo, total de citas, h-index, JCR, etc de un grupo de investigación o instituto de investigación: https://tinyurl.com/ybnbsw2l
* Q49 - Obtener las publicaciones de un investigador o grupo de investigación en una revista científica indicada: https://tinyurl.com/y7vahkfs
* Q50 - Dados unos años listar los JRCs en los mismos para poder visualizar evolución: https://tinyurl.com/yanngc57
* Q51 - Sin implementar.
* Q52 - Buscar las publicaciones de un grupo o instituto de investigación, o una universidad en las que aparezca en el título de la publicación los tokens que indique como entrada a la consulta: https://tinyurl.com/y93zyj3b
* Q53 - Listar la revistas científicas con un JCR Q2, en un área determinada, donde ha publicado un grupo o instituto de investigación: https://tinyurl.com/ybhgbjpt
* Q54 - Listar los impactos JCR de los investigadores de un grupo en un año: https://tinyurl.com/y886xrzt
* Q55 - Listar los proyectos privados obtenidos por un grupo de investigación contratados por una organización privada: https://tinyurl.com/ycvuqzay
* Q56 - Listar las tesis de una universidad en las que aparezca en el título los tokens que indique como entrada a la consulta: https://tinyurl.com/yc6vh6ny
* Q57 - Indicar el porcentaje de mujeres frente a hombres como autores de publicaciones con impacto de un grupo de investigación: implementada por Q45.
