from rdflib import Namespace
from pydoc import locate


class DataType:
    def __init__(self, ontology, name, python_type, default, force):
        self.ontology = ontology
        self.name = name
        self.python_type = python_type
        self.default = default
        self.force = force

    def get_python_type(self):
        return locate(self.python_type)

    def get_identifier(self):
        return self.ontology + ":" + self.name


class OntologyConfig:
    def __init__(self):
        self.ontologies = {}
        self.graph = None
        self.cvn_person = None
        self.data_types = []

    def add_data_type(self, data_type):
        self.data_types.append(data_type)
        return self

    def get_default_data_type(self):
        for data_type in self.data_types:
            if data_type.default:
                return data_type
        return None

    def get_data_type(self, identifier):
        for data_type in self.data_types:
            if data_type.get_identifier() == identifier:
                return data_type
        return None

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
