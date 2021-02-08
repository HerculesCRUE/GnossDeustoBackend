from rdflib import Literal, Namespace, OWL, RDF, RDFS, URIRef, XSD
from urllib.parse import quote

from common.namespace import ASIO

prefix_main = 'http://data.datascienceinstitute.ie/'

def triplestore(graph, name, uid_only = False):
	if not (name): raise ValueError("I need at least a name for the software.")
	uid = quote(name.strip().lower().replace(' ','_'))
	uid = reconcile(uid)
	o = URIRef( prefix_main + "software/" + uid )
	graph.add( ( o, RDF.type, ASIO.TripleStore ) )
	graph.add( ( o, RDFS.label, Literal(name,lang='en')) )
	return uid if uid_only else o

def reconcile(uid):
	import configparser
	config = configparser.RawConfigParser()
	config.read('static/mappings.txt')
	try:
		return config.get('entities', uid)
	except configparser.NoOptionError:
		return uid

