![](..//media/CabeceraDocumentosMD.png)

# Hércules Backend ASIO. Ejemplos de consultas sparql

[1 INTRODUCCIÓN](#introduccion)

[2 GRAFO DE DATOS DE CVN](#grafo-de-datos-de-cvn)

[3 GRAFO DE DATOS DE SISTEMAS DE LA UM](#grafo-de-datos-de-sistemas-de-la-um)

Introducción
============

En este documento se incluyen y describen un conjunto de consultas SPARQL sobre el 
conjunto de datos disponible en el proyecto Hércules ASIO.

El documento no es exhaustivo, sino que contiene ejemplos de consultas de 
recuperación de la información, e irá evolucionando según el sistema vaya incorporando
más datos.

GRAFO DE DATOS DE CVN
=====================

El grafo de los datos cargados desde CVN es:

http://graph.um.es/graph/um/cvn

**Consulta que devuelve los URIs de los investigadores cargados**.

	select * from <http://graph.um.es/graph/um_cvn>
	where {?s a <http://purl.org/roh/mirror/foaf#Person>}

**Consulta que devuelve todos los triples de un investigador**.

	select * from <http://graph.um.es/graph/um_cvn>
	where { <http://graph.um.es/res/person/d7aad123-55b9-4400-b108-185139408f7f> ?p ?o}

**Consulta que devuelve todas las entidades de las que el investigador
es objeto**.

	select * from <http://graph.um.es/graph/um_cvn>
	where {
		?s ?p <http://graph.um.es/res/person/d7aad123-55b9-4400-b108-185139408f7f>.
		?s a ?o.
	}

**Consulta que devuelve todos los triples un artículo.**

	select * from <http://graph.um.es/graph/um_cvn>
	where {
		<http://graph.um.es/res/article/4167c433-5af4-4071-8daa-df71d9c18fc5> ?p ?o.
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
