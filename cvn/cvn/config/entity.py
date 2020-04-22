from ..utils import code as cvn_code
import cvn.config.property as cvn_property
from cvn.utils.printable import Printable
from rdflib.namespace import RDF
from rdflib import Literal, URIRef
import uuid

def init_entity_from_serialized_toml(config, parent=None):
    # code and name are required attributes

    code = None
    if 'code' not in config:
        # if code is not set but we have a parent, set it to the parent's code
        if (parent is not None) and (parent.code is not None):
            code = parent.code
        else:
            raise KeyError('code not specified for Entity')
    else:
        code = config['code']

    if not cvn_code.is_cvn_code_valid(code):
        raise ValueError('code does not match expected format')

    if 'ontology' not in config:
        raise KeyError('ontology not specified for Entity')
        # TODO comprobar que está definida

    if 'classname' not in config:
        raise KeyError('classname not specified for Entity')

    entity = Entity(code, config['ontology'], config['classname'])

    # Populate properties
    if 'properties' not in config:
        raise KeyError('no properties defined for Entity: ' + entity.classname)
    for property_config in config['properties']:
        property_generated = cvn_property.init_property_from_serialized_toml(property_config)
        entity.add_property(property_generated)

    # Subentities, recursive (optional)
    if 'subentities' in config:
        for subentity in config['subentities']:
            entity.add_subentity(init_entity_from_serialized_toml(subentity, entity))

    return entity


class Entity(Printable):
    # TODO todo el tema de la id y la URI
    def __init__(self, code, ontology, classname, parent=None):
        self.code = code
        self.ontology = ontology
        self.classname = classname
        self.subentities = []
        self.properties = []
        self.relations = []
        self.parent = parent
        self.triplets = []
        self.identifier = None

    def add_property(self, entity_property):
        """
        Adds a property to the Entity
        :param entity_property: the Property to add
        :return: the resulting Entity
        """
        self.properties.append(entity_property)
        return self

    def add_subentity(self, subentity):
        self.subentities.append(subentity)
        return self

    def get_property_values_from_node(self, item_node):
        for property_item in self.properties:
            property_item.get_value_from_node(item_node)

    def clear_values(self):
        self.identifier = None
        for property_item in self.properties:
            property_item.clear_values()
        for subentity in self.subentities:
            subentity.clear_values()
        return self

    def get_identifier(self):
        if self.identifier is None:
            self.identifier = str(uuid.uuid4())
        return self.identifier

    def get_uri(self):
        return URIRef("http://data.um.es/class/" + self.classname + "/" + self.get_identifier())
        # TODO re-integrar UriFactory

    def generate_entity_triple(self, ontology_config):
        return self.get_uri(), RDF.type, ontology_config.get_ontology(self.ontology).term(self.classname)

    def generate_property_triples(self):
        triples = []
        for property_item in self.properties:
            triples.append(property_item.generate_triple())

    def add_entity_to_ontology(self, ontology_config):
        ontology_config.graph.add(self.generate_entity_triple(ontology_config))
