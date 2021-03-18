from rdflib import Namespace, OWL, RDFS
from rdflib.namespace import DC, DCTERMS, DOAP, SKOS

ASIO = Namespace('http://datascienceinstitute.ie/asio/')

PREFIXES = f"""
PREFIX asio: <{ASIO}>
PREFIX asios: <http://datascienceinstitute.ie/asio/schema#>
PREFIX dc: <{DC}>
PREFIX dct: <{DCTERMS}>
PREFIX doap: <{DOAP}>
PREFIX owl: <{OWL}>
PREFIX provo: <http://www.w3.org/ns/prov#>
PREFIX rdfs: <{RDFS}>
PREFIX sc: <http://purl.org/science/owl/sciencecommons/>
PREFIX skos: <{SKOS}>
"""


def bgp_isstore(var, prefixed=True):
    a_TS = 'asio:TripleStore' if prefixed else '<{ASIO}TripleStore>'
    a_TSR = 'asio:TripleStoreRelease' if prefixed else '<{ASIO}TripleStoreRelease>'
    d_Proj = 'doap:Project' if prefixed else '<{DOAP}Project>'
    d_rel = 'doap:release' if prefixed else '<{DOAP}release>'
    return f"""
     {{ ?{var} a {a_TS} }}
     UNION
     {{ ?{var} a {d_Proj} ; {d_rel} [ a {a_TSR} ] }}
"""
