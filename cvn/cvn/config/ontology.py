from rdflib import Namespace


class OntologyConfig:
    def __init__(self):
        self.ontologies = {}
        self.graph = None

    def get_ontology(self, short_name):
        try:
            return self.ontologies.get(short_name)
        except KeyError:
            return None

    def add_ontology(self, ontology):
        # if self.ontology_exists(ontology.short_name):
        # TODO error handling verificación de si existe ontología
        self.ontologies[ontology.short_name] = ontology
        return self

    def ontology_exists(self, short_name):
        return short_name in self.ontologies

    def get_primary(self):
        """
        Obtener la ontología primaria
        :return: la primera ontología que tenga primary como True
        """
        for ontology in self.ontologies.values():
            if ontology.primary:
                return ontology
        return None


class Ontology:
    def __init__(self, short_name, uri_base, primary=False):
        self.short_name = short_name
        self.uri_base = uri_base
        self.primary = primary
        self.namespace = Namespace(self.uri_base)

    def term(self, name):
        return self.namespace.term(name)
