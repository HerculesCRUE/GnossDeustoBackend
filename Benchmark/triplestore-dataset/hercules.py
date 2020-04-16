#!/usr/bin/env python

import argparse
import importlib, importlib.util
import logging
import os, sys
from rdflib import OWL, URIRef
from rdflib.namespace import DCTERMS, DOAP, SKOS

from common.namespace import ASIO

FORMAT = '[%(levelname)s] %(asctime)s %(message)s'
logging.basicConfig(format=FORMAT)
logger = logging.getLogger('hercules')
logger.setLevel(logging.DEBUG)

dir = 'extractors'

def write_graph(graph, name):
	# Print the graph in Turtle format to screen (with nice prefixes)
	graph.namespace_manager.bind('asio', ASIO)
	graph.namespace_manager.bind('dct', DCTERMS)
	graph.namespace_manager.bind('doap', DOAP)
	graph.namespace_manager.bind('owl', OWL)
	graph.namespace_manager.bind('skos', SKOS)
	# ... to a file 'out/[name].ttl' (will create the 'out' directory if missing)
	dir = 'out'
	if not os.path.exists(dir):
		os.makedirs(dir)
	# Note: it will overwrite the existing Turtle file!
	path = os.path.join(dir, name + '.ttl')
	try:
		graph.serialize(destination=path, format='turtle')
		print('DONE. ' + str(len(graph)) + ' triples written to ' + path)
	except Exception as e:
		print(e)
		sys.exit('Serialization of "' + name + '" failed.')

parser = argparse.ArgumentParser(description='Run one or more linked data extractors.')
parser.add_argument('sources', metavar='S', nargs='+',
                   help='the name of a data source (there must be a Python module with that name in the \'' + dir + '\' package)')

extractors = []                   
args = parser.parse_args()
if args.sources:
	print('Looking for the following extractors to run: ' + str(args.sources))
	for src in vars(args)['sources']:
		xm = dir + '.' + src
		spec = importlib.util.find_spec(xm)
		if spec is None : 
			logger.error("module '%s' not found, did you forget to drop one inside the '%s' package?", src, dir)
		else :
			print("Found '" + xm + "'")
			extractors.append(src)	

for src in extractors:		
	mod = importlib.import_module(dir + '.' + src)
	if hasattr(mod, 'extract_graph'):
		g = mod.extract_graph()
		if g : write_graph(g, src)
