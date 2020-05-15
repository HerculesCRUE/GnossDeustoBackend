# Hércules Backend ASIO. Ejemplos de consultas sparql

[1 GRAFO DE DATOS DE CVN 3](#grafo-de-datos-de-cvn)

[2 GRAFO DE DATOS DE SISTEMAS DE LA UM
4](#grafo-de-datos-de-sistemas-de-la-um)

GRAFO DE DATOS DE CVN
=====================

El grafo de los datos cargados desde CVN es:

http://graph.um.es/graph/um/cvn

**Consulta que devuelve los URIs de los investigadores cargados**.

	select * from <http://graph.um.es/graph/um_cvn>
	where {?s a <http://purl.org/roh/mirror/foaf#Person>}

**Consulta que devuelve todos los triples de un investigador**.

	select * from <http://graph.um.es/graph/um_cvn>
	where { <http://data.um.es/res/person/fb4cab6c-1e0f-4010-9e59-ae2af1ac23f3> ?p ?o}

**Consulta que devuelve todas las entidades de las que el investigador
es objeto (**con los datos cargados actualmente, sólo devuelve
artículos**)**.

	select * from <http://graph.um.es/graph/um_cvn>
	where {
		?s ?p <http://data.um.es/res/person/fb4cab6c-1e0f-4010-9e59-ae2af1ac23f3>.
		?s a ?o.
	}

**Consulta que devuelve todos los triples un artículo.**

	select * from <http://graph.um.es/graph/um_cvn>
	where {
		<http://data.um.es/res/article/e449793d-fdaa-4c0e-9999-53f47d7aa437> ?p ?o.
	}

GRAFO DE DATOS DE SISTEMAS DE LA UM
===================================

El grafo de los datos importados desde los sistemas de la UM es:

http://graph.um.es/graph/um_sgi_3

**Consulta que devuelve los URIs de todos los proyectos**.

	select * from <http://graph.um.es/graph/um_sgi>
	where { ?s a <http://vivoweb.org/ontology/core#Project>}

**Consulta que cuenta los proyectos cargados**.

	select count(*) from <http://graph.um.es/graph/um_sgi>
	where { ?s a <http://vivoweb.org/ontology/core#Project>}

**Consulta que devuelve los datos de un proyecto** (teniendo en cuenta
que podrían faltar algunos datos).

	select * from <http://graph.um.es/graph/um_sgi>
	where {
	?s ?p ?o.
	OPTIONAL{?o ?p2 ?o2 OPTIONAL{?o2 ?p3 ?o3}}
	FILTER(?s=<http://graph.um.es/res/project/10031>)
	}
	order by asc(?s) asc(?p) asc(?o) asc(?p2) asc(?o2) asc(?p3) asc(?o3)
