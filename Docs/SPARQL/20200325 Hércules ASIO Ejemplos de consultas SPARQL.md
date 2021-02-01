![](..//media/CabeceraDocumentosMD.png)

# Hércules Backend ASIO. Ejemplos de consultas sparql

[INTRODUCCIÓN](#introduccion)

[GRAFOS DE ASIO](#grafos-de-asio)

[GRAFO DE DATOS DE CVN](#grafo-de-datos-de-cvn)

[GRAFO DE DATOS DE SISTEMAS DE LA UM](#grafo-de-datos-de-sistemas-de-la-um)

Introducción
============

En este documento se incluyen y describen un conjunto de consultas SPARQL sobre el 
conjunto de datos disponible en el proyecto Hércules ASIO.

El documento no es exhaustivo, sino que contiene ejemplos de consultas de 
recuperación de la información, e irá evolucionando según el sistema vaya incorporando
más datos.

La URL del SPARQL endpoint es http://155.54.239.204:8890/sparql

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
- http://graph.um.es/graph/um_cvn: Grafo con los catod cargados de CVN de la UM.
- http://graph.um.es/graph/um_sgi: Grafo de los datos importados desde los sistemas de la UM:

Con la siguiente instrucción: 

	rdfs_rule_set ('rohontology', 'http://graph.um.es/graph/research/roh') ;
	
Se han cargado las reglas de inferencia de la ontología, para utilizarlas en el SPARQL endpoint hay que incluir al inicio de la query la instrucción:	

	define input:inference "rohontology"

GRAFO DE DATOS DE CVN
=====================

El grafo de los datos cargados desde CVN es:

http://graph.um.es/graph/um_cvn

**Consultas por rdf:type**

1. Consulta que devuelve los URIs de los investigadores cargados.

		select * from <http://graph.um.es/graph/um_cvn>
		where 
		{
			?s a <http://purl.org/roh/mirror/foaf#Person>
		}
		
2. Consulta que devuelve los URIs de los artículos académicos cargados.

		select * from <http://graph.um.es/graph/um_cvn>
		where 
		{
			?s <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://purl.org/roh/mirror/bibo#AcademicArticle>
		}
		
3. Consulta que devuelve los URIs de todos los documentos (utilizando inferencia).

		define input:inference "rohontology"
		select * from <http://graph.um.es/graph/um_cvn>
		where 
		{
			?s a  <http://purl.org/roh/mirror/bibo#Document>
		}


**Consultas por sujeto**

1. Consulta que devuelve todos los triples directos de una persona.

		select * from <http://graph.um.es/graph/um_cvn>
		where 
		{ 
			<http://graph.um.es/res/person/1949f7bb-70d9-4e2b-94a4-a54b0df96312> ?p ?o
		}
	
2. Consulta que devuelve todos los triples directos de un artículo.

		select * from <http://graph.um.es/graph/um_cvn>
		where 
		{
			<http://graph.um.es/res/article/01fbc549-2173-4078-b51f-55311ecc5df8> ?p ?o.
		}	
	
3. Consulta que devuelve todos los autores de un artículo.	
	
		select distinct * from <http://graph.um.es/graph/um_cvn>
		where 
		{
			<http://graph.um.es/res/article/01fbc549-2173-4078-b51f-55311ecc5df8> <http://purl.org/roh/mirror/bibo#authorList> ?lista.
			?lista ?propLista ?persona.
			?persona <http://purl.org/roh/mirror/foaf#name> ?nombrePersona.	
		}	
	
**Consultas por objeto**

1. Consulta que devuelve todas las entidades de las que el investigador es objeto.

		select * from <http://graph.um.es/graph/um_cvn>
		where 
		{ 
			?s ?p <http://graph.um.es/res/person/1949f7bb-70d9-4e2b-94a4-a54b0df96312>
		}

2. Consulta que devuelve todas las entidades de las que el artículo es objeto.

		select * from <http://graph.um.es/graph/um_cvn>
		where {
			?s ?p <http://graph.um.es/res/article/01fbc549-2173-4078-b51f-55311ecc5df8>.
		}

GRAFO DE DATOS DE SISTEMAS DE LA UM
===================================

El grafo de los datos importados desde los sistemas de la UM es:

http://graph.um.es/graph/um_sgi

**Consulta que devuelve los URIs de todos los proyectos**.

	select * from <http://graph.um.es/graph/um_sgi>
	where { ?s a <http://purl.org/roh/mirror/vivo#Project>}

**Consulta que cuenta los proyectos cargados**.

	select count(*) from <http://graph.um.es/graph/um_sgi>
	where { ?s a <http://purl.org/roh/mirror/vivo#Project>}

**Consulta que devuelve los datos de un proyecto (hasta 3 niveles)** 

	select * from <http://graph.um.es/graph/um_sgi>
	where {
		?s ?p ?o.
		OPTIONAL{?o ?p2 ?o2 OPTIONAL{?o2 ?p3 ?o3}}
		FILTER(?s=<http://graph.um.es/res/project/12307>)
	}
	order by asc(?s) asc(?p) asc(?o) asc(?p2) asc(?o2) asc(?p3) asc(?o3)
	
**Consulta que devuelve los URIs de todas las conferencias**.

	select * from <http://graph.um.es/graph/um_sgi>
	where { ?s a <http://purl.org/roh/mirror/bibo#Conference>}

**Consulta que cuenta las conferencias cargadas**.

	select count(*) from <http://graph.um.es/graph/um_sgi>
	where { ?s a <http://purl.org/roh/mirror/bibo#Conference>}

**Consulta que devuelve los datos de una conferencia (hasta 3 niveles)**

	select * from <http://graph.um.es/graph/um_sgi>
	where {
		?s ?p ?o.
		OPTIONAL{?o ?p2 ?o2 OPTIONAL{?o2 ?p3 ?o3}}
		FILTER(?s=<http://graph.um.es/res/conference/12670>)
	}
	order by asc(?s) asc(?p) asc(?o) asc(?p2) asc(?o2) asc(?p3) asc(?o3)
	
**Consulta que devuelve los URIs de todos los artículos**.

	select * from <http://graph.um.es/graph/um_sgi>
	where { ?s a <http://purl.org/roh/mirror/bibo#Article>}

**Consulta que cuenta los artículos cargados**.

	select count(*) from <http://graph.um.es/graph/um_sgi>
	where { ?s a <http://purl.org/roh/mirror/bibo#Article>}

**Consulta que devuelve los datos de un artículo (hasta 3 niveles)** 

	select * from <http://graph.um.es/graph/um_sgi>
	where {
		?s ?p ?o.
		OPTIONAL{?o ?p2 ?o2 OPTIONAL{?o2 ?p3 ?o3}}
		FILTER(?s=<http://graph.um.es/res/article/44357>)
	}
	order by asc(?s) asc(?p) asc(?o) asc(?p2) asc(?o2) asc(?p3) asc(?o3)	
