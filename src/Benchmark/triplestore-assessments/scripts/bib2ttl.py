from pybtex.database.input import bibtex
from pybtex.database import BibliographyData
import sys
from rdflib import Graph, Literal, RDF, URIRef
from rdflib.namespace import DC, FOAF

if len(sys.argv) != 2:
    print("provide name of bib file to process")
    quit()

g = Graph()
base = 'http://data.datascienceinstitute.ie/asio/bib/'
    
filename = sys.argv[1]

parser = bibtex.Parser()
bib_data = parser.parse_file(filename)

for key in bib_data.entries:
    uri = URIRef(base+key)
    g.add((uri, RDF.type, DC.BibliographicResource))
    fields = bib_data.entries[key].fields
    if "title" in fields:
        g.add((uri, DC.title, Literal(fields["title"])))
    else:
        sys.stderr.write("error: no title in "+key+"\n")
        quit()
    if "url" in fields:
        g.add((uri, DC.source, URIRef(fields["url"])))
    else:
        sys.stderr.write("warning: no url in "+key+"\n")

print( g.serialize(format='ttl') )


