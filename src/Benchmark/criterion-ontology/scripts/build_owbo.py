import os, sys
from rdflib import Graph
from rdflib.namespace import SKOS

g = Graph()
g.parse("../src/schema.ttl", format="turtle")
g.parse("../src/criteria.ttl", format="turtle")
g.parse("../src/owbo_layout.ttl", format="turtle")
name = 'criteria_owbo'

g.namespace_manager.bind('skos', SKOS)

dir = '../out'
if not os.path.exists(dir):
	os.makedirs(dir)
# Note: it will overwrite the existing Turtle file!
path = os.path.join(dir, name + '.ttl')
try:
	g.serialize(destination=path, format='turtle')
	print('DONE. ' + str(len(g)) + ' triples written to ' + path)
except Exception as e:
	print(e)
	sys.exit('Serialization of "' + name + '" failed.')