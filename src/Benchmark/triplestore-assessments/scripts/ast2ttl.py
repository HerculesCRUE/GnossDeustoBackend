import sys
from rdflib import Graph, Literal, RDF, URIRef
from rdflib.namespace import DC, FOAF

if len(sys.argv) != 2:
    print("provide name of bib with assessment file to process")
    quit()

g = Graph()
    
base      = 'http://data.datascienceinstitute.ie/asio/assessments/'
crit_base = 'http://datascienceinstitute.ie/asio/criteria/'
bib_base  = 'http://data.datascienceinstitute.ie/asio/bib/'
ts_base   = 'http://data.datascienceinstitute.ie/software/'
onto_base = 'http://datascienceinstitute.ie/asio/schema#' 

filename = sys.argv[1]

lines = []
with open(filename) as fp:
   lines = fp.readlines()

for line in lines:
    if "==" in line:
        ts = line.replace("==", "").strip()
        tsuri = URIRef(ts_base+ts)
    if "::" in line:
        la = line.split("::")        
        if len(la)==4: # ignoring homepages for now
            curi = URIRef(crit_base+la[0])
            comment = la[1]
            value = int(la[2])
            buri = URIRef(bib_base+la[3].strip())
            as_uri = URIRef(base+ts+"_"+la[0])
            g.add((as_uri, RDF.type, URIRef(onto_base+"CriterionAssessment")))
            g.add((as_uri, URIRef(onto_base+"criterion"), curi))
            g.add((as_uri, DC.subject, tsuri))
            g.add((as_uri, DC.description, Literal(comment)))
            g.add((as_uri, URIRef(onto_base+"value"), Literal(value)))
            g.add((as_uri, URIRef('http://www.w3.org/ns/prov#primarySource'), buri))

print( g.serialize(format='ttl').decode('utf-8') )

