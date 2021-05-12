![](../media/CabeceraDocumentosMD.png)

| Fecha         | 27/4/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Especificación de Esquema de URIs| 
|Descripción|Especificación del Esquema de URIs para el proyecto Hércules ASIO|
|Versión|1.4|
|Módulo|Documentación|
|Tipo|Especificación|
|Cambios de la Versión|Cambios en las consultas|


# Hércules Backend ASIO. Ejemplos de consultas sparql

[INTRODUCCIÓN](#introduccion)

[GRAFOS DE ASIO](#grafos-de-asio)

[GRAFO DE DATOS DE SISTEMAS DE LA UM](#grafo-de-datos-de-sistemas-de-la-um)

Introducción
============

En este documento se incluyen y describen un conjunto de consultas SPARQL sobre el 
conjunto de datos disponible en el proyecto Hércules ASIO.

El documento no es exhaustivo, sino que contiene ejemplos de consultas de 
recuperación de la información, e irá evolucionando según el sistema vaya incorporando
más datos.

La URL del SPARQL endpoint es http://155.54.239.221:8890/sparql

En la siguiente URL: https://docs.data.world/tutorials/sparql/introduction.html se encuentra un tutorial sobre SPARQL

GRAFOS DE ASIO
=====================

Con la siguiente query se listan los diferentes grafos cargados:

	select distinct ?g 
	where {
		graph ?g
		{?s ?p ?o}
	}

Los grafos relevantes para las queries descritas en los siguientes ejemplos son los siguientes:
- http://graph.um.es/graph/research/roh: Grafo con la ontología
- http://linkeddata2test.um.es/graph/sgi: Grafo de los datos importados desde los sistemas de la UM:

Con la siguiente instrucción: 

	rdfs_rule_set ('rohontology', 'http://graph.um.es/graph/research/roh') ;
	
Se han cargado las reglas de inferencia de la ontología, para utilizarlas en el SPARQL endpoint hay que incluir al inicio de la query la instrucción:	

	define input:inference "rohontology"
		

GRAFO DE DATOS DE SISTEMAS DE LA UM
===================================

El grafo de los datos importados desde los sistemas de la UM es:

http://linkeddata2test.um.es/graph/sgi

**Consultas por rdf:type**

1. Consulta que devuelve los URIs de los investigadores cargados.

		select * from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{
			?s a <http://purl.org/roh/mirror/foaf#Person>
		}
		
2. Consulta que devuelve los URIs de los artículos académicos cargados.

		select * from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{
			?s <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://purl.org/roh/mirror/bibo#AcademicArticle>
		}
		
3. Consulta que devuelve los URIs de todas las organizaciones (utilizando inferencia).

		define input:inference "rohontology"
		select * from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{
			?s a  <http://purl.org/roh/mirror/foaf#Organization>
		}


**Consultas por sujeto**

1. Consulta que devuelve todos los triples directos de una persona.

		select * from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{ 
			<http://linkeddata2test.um.es/res/person/1602> ?p ?o
		}
	
2. Consulta que devuelve todos los triples directos de un artículo.

		select * from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{ 
			<http://linkeddata2test.um.es/res/academic-article/50209> ?p ?o
		}	
	
3. Consulta que devuelve todos los autores de un artículo.	
	
		select distinct * from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{
			<http://linkeddata2test.um.es/res/academic-article/50209> <http://purl.org/roh/mirror/bibo#authorList> ?lista.
			?lista ?propLista ?persona.
			?persona <http://purl.org/roh/mirror/foaf#name> ?nombrePersona.	
		}	
	
**Consultas por objeto**

1. Consulta que devuelve todas las entidades de las que el investigador es objeto.

		select * from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{ 
			?s ?p <http://linkeddata2test.um.es/res/person/1602>
		}
		
2. Consulta que devuelve todas las entidades de las que el investigador forma parte de su lista de autores.

		select distinct ?s from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{ 
			?s <http://purl.org/roh/mirror/bibo#authorList> ?lista.
			?lista ?item <http://linkeddata2test.um.es/res/person/1602>
		}
		
**Consultas con agrupación**

1. Consulta que obtiene todas las personas junto con el número de entidades de las que es parte como autor
		
		select ?persona ?nombrePersona count(distinct ?doc) as ?numDoc from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{
			?persona a <http://purl.org/roh/mirror/foaf#Person>.
			?doc <http://purl.org/roh/mirror/bibo#authorList> ?lista.
		     	?lista ?item ?persona.
                      	?persona <http://purl.org/roh/mirror/foaf#name> ?nombrePersona.				
		}group by (?persona and ?nombrePersona ) order by desc (?numDoc)
		
2.Consulta que obtiene el número de entidades por cada rdf:type		
		
		select ?tipo count(distinct ?entidad ) as ?numEntidades from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{
			?entidad a ?tipo.	
		}group by (?tipo ) order by desc (?numEntidades)
		
**Consultas con 'FILTER'**	

1. Consulta que devuelve todas las entidades de las que dos investigadores forman parte de su lista de autores (OR).

		select distinct ?s from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{ 
			?s <http://purl.org/roh/mirror/bibo#authorList> ?lista.
			?lista ?item ?autor.
			FILTER(?autor in (<http://linkeddata2test.um.es/res/person/1602>,<http://linkeddata2test.um.es/res/person/3906>))
			
		}
		
2. Consulta que devuelve todas las entidades de las que dos investigadores forman parte de su lista de autores (AND).

		select distinct ?s from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{ 
			?s <http://purl.org/roh/mirror/bibo#authorList> ?lista.
			?lista ?item ?autor.
			FILTER(?autor =<http://linkeddata2test.um.es/res/person/1602>)
                    	?lista ?item2 ?autor2.			
			FILTER(?autor2 =<http://linkeddata2test.um.es/res/person/3906>)			
		}

**Consultas con LIKE**	

1. Consulta que devuelve todas las personas que contiene la palabra 'juan' y 'antonio' en el nombre
		
		select * from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{
			?persona a <http://purl.org/roh/mirror/foaf#Person>.
                      	?persona <http://purl.org/roh/mirror/foaf#name> ?nombrePersona.	
			FILTER (lcase(?nombrePersona) like'%juan%' AND lcase(?nombrePersona) like'%antonio%')			
		}
		
**Consultas con OPTIONAL**	

1. Consulta que devuelve todos los triples de primer nivel y opcionalmente los autores de un artículo
		
		select * from <http://linkeddata2test.um.es/graph/sgi>
		where 
		{
			?s ?p ?o.
		      	OPTIONAL
			{
				?s ?p ?lista.
				?lista ?porpLista ?autor.
				?autor <http://purl.org/roh/mirror/foaf#name> ?nombreAutor
				FILTER(?p=<http://purl.org/roh/mirror/bibo#authorList>)
			}
			FILTER(?s =<http://linkeddata2test.um.es/res/academic-article/50209>)

		}

**Consultas con 'UNION'**	

1. Consulta que devuelve todas las entidades de las que dos investigadores forman parte de su lista de autores (OR).

		select distinct ?s from  <http://linkeddata2test.um.es/graph/sgi>
		where 
		{ 
			{
				?s <http://purl.org/roh/mirror/bibo#authorList> ?lista.
				?lista ?item <http://linkeddata2test.um.es/res/person/1602>.
			}
			UNION
			{
				?s <http://purl.org/roh/mirror/bibo#authorList> ?lista.
				?lista ?item <http://linkeddata2test.um.es/res/person/3906>.
			}
		}


**Consulta que devuelve los URIs de todos los proyectos**.

	select * from <http://linkeddata2test.um.es/graph/sgi>
	where { ?s a <http://purl.org/roh/mirror/vivo#Project>}

**Consulta que cuenta los proyectos cargados**.

	select count(*) from <http://linkeddata2test.um.es/graph/sgi>
	where { ?s a <http://purl.org/roh/mirror/vivo#Project>}

**Consulta que devuelve los datos de un proyecto (hasta 3 niveles)** 

	select * from <http://linkeddata2test.um.es/graph/sgi>
	where {
		?s ?p ?o.
		OPTIONAL{?o ?p2 ?o2 OPTIONAL{?o2 ?p3 ?o3}}
		FILTER(?s=<http://linkeddata2test.um.es/res/project/12532>)
	}
	order by asc(?s) asc(?p) asc(?o) asc(?p2) asc(?o2) asc(?p3) asc(?o3)
	
