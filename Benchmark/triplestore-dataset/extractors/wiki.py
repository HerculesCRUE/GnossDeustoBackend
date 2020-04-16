import os
from rdflib import Graph, OWL, URIRef
from rdflib.namespace import DCTERMS, DOAP
from SPARQLWrapper import SPARQLWrapper, JSON

import common.rdf_factories as maker

def extract_graph(targetGraph=None):
	print('Querying Wikidata for triple stores...')
	graph = targetGraph if targetGraph else Graph()
	cache = {}
	sparql = SPARQLWrapper('https://query.wikidata.org/sparql')
	sparql.setReturnFormat(JSON)
	q = f"""
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX wdp: <http://www.wikidata.org/prop/direct/>
SELECT DISTINCT * WHERE {{ 
	?x wdp:P31 <http://www.wikidata.org/entity/Q3539533> 
	 ; rdfs:label ?name FILTER(LANG(?name)='en')
	. OPTIONAL {{ ?x wdp:P856 ?www }}
}}
"""
	sparql.setQuery(q)
	results = sparql.query().convert()
	for bind in results['results']['bindings'] :
		w = bind['x']['value']
		if w not in cache:
			sw = maker.triplestore(graph, bind['name']['value'])
			graph.add( (sw, OWL.sameAs, URIRef(w)) )
			cache[w] = sw
		else: sw = cache[w]
		if 'www' in bind : graph.add( (sw, DOAP.homepage, URIRef(bind['www']['value'])) )

	return graph