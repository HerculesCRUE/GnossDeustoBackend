from rdflib import Namespace


class OntologyConfig:
    def __init__(self):
        self.ontologies = {}
        self.graph = None
        self.cvn_person = None

    def get_ontology(self, short_name):
        if short_name not in self.ontologies:
            raise KeyError("Ontology " + short_name + " not defined")
        return self.ontologies.get(short_name)

    def add_ontology(self, ontology):
        # if self.ontology_exists(ontology.short_name):
        # TODO error handling verificación de si existe ontología
        self.ontologies[ontology.short_name] = ontology
        return self

    def ontology_exists(self, short_name):
        return short_name in self.ontologies


class Ontology:
    def __init__(self, short_name, uri_base):
        self.short_name = short_name
        self.uri_base = uri_base
        self.namespace = Namespace(self.uri_base)

    def term(self, name):
        return self.namespace.term(name)
