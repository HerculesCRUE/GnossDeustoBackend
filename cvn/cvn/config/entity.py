from ..utils import code as cvn_code
import cvn.config.property as cvn_property
from rdflib.namespace import RDF
from rdflib import Literal, URIRef
import uuid
from cvn.config.relationship import Relationship


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

    entity = Entity(code, config['ontology'], config['classname'], parent=parent)

    # Populate properties
    if 'properties' not in config:
        raise KeyError('no properties defined for Entity: ' + entity.classname)
    for property_config in config['properties']:
        property_generated = cvn_property.init_property_from_serialized_toml(property_config)
        entity.add_property(property_generated)

    # Relationships
    if 'relationships' in config:
        for relationship in config['relationships']:

            if 'ontology' not in relationship:
                raise KeyError('ontology not specified for Relationship')
                # TODO comprobar que está definida
            if 'name' not in relationship:
                raise KeyError('name not specified for Relationship')

            inverse_name = None
            inverse_ontology = None
            if 'inverse_name' in relationship:
                if 'inverse_ontology' not in relationship:
                    raise KeyError('inverse Relationship name was specified but no ontology for it')
                    # TODO comprobar que está definida
                inverse_name = relationship['inverse_name']
                inverse_ontology = relationship['inverse_ontology']

            link_to_cvn_person = False
            if 'link_to_cvn_person' in relationship:
                link_to_cvn_person = relationship['link_to_cvn_person']

            generated_relationship = Relationship(relationship['ontology'], relationship['name'], inverse_ontology,
                                                  inverse_name, link_to_cvn_person)
            entity.add_relationship(generated_relationship)

    # Subentities, recursive (optional)
    if 'subentities' in config:
        for subentity in config['subentities']:
            entity.add_subentity(init_entity_from_serialized_toml(subentity, entity))

    return entity


class Entity:
    # TODO todo el tema de la id y la URI
    def __init__(self, code, ontology, classname, parent=None):
        self.code = code
        self.ontology = ontology
        self.classname = classname
        self.subentities = []
        self.properties = []
        self.relationships = []
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

    def add_relationship(self, relationship):
        self.relationships.append(relationship)

    def add_subentity(self, subentity):
        self.subentities.append(subentity)
        return self

    def get_property_values_from_node(self, item_node):
        for property_item in self.properties:
            property_item.get_value_from_node(item_node)
        for subentities in self.subentities:
            subentities.get_property_values_from_node(item_node)

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

    def generate_property_triples(self, ontology_config):
        triples = []
        for property_item in self.properties:
            if property_item.formatted_value is not None:
                triple = self.get_uri(), \
                         ontology_config.get_ontology(property_item.ontology).term(property_item.name), \
                         Literal(str(property_item.formatted_value))
                triples.append(triple)
        return triples

    def generate_relationship_triples(self, ontology_config):
        triples = []
        for relationship in self.relationships:

            if relationship.link_to_cvn_person:
                other = ontology_config.cvn_person
            else:
                if self.parent is None:
                    continue  # Si es una relación con el padre, pero no tiene... nos la saltamos
                other = self.parent.get_uri()

            # Relación directa
            direct_triple = self.get_uri(), ontology_config.get_ontology(relationship.ontology).term(relationship.name), other
            triples.append(direct_triple)

            # Relación inversa
            if (relationship.inverse_name is not None) and \
                (relationship.inverse_ontology is not None):
                inverse_triple = other, ontology_config.get_ontology(relationship.inverse_ontology)\
                    .term(relationship.inverse_name), self.get_uri()
                triples.append(inverse_triple)

        return triples

        # TODO comprobar que person no sea None

    #   if 'relations' in entity:
    #     for relation in entity['relations']:
    #         if 'link_to_cvn_person' in relation and relation['link_to_cvn_person']:
    #             g.add((person, ontologies[relation['ontology']].term(relation['name']), current_entity))
    #         if 'inverse_name' in relation:
    #             inverse_ontology = relation['ontology']
    #             if 'inverse_ontology' in relation:
    #                 inverse_ontology = relation['inverse_ontology']
    #             g.add((current_entity, ontologies[inverse_ontology].term(relation['inverse_name']), person))

    def add_entity_to_ontology(self, ontology_config):
        ontology_config.graph.add(self.generate_entity_triple(ontology_config))

        # Propiedades
        for triple in self.generate_property_triples(ontology_config):
            ontology_config.graph.add(triple)

        # Relaciones
        relationship_triples = self.generate_relationship_triples(ontology_config)
        for triple in relationship_triples:
            ontology_config.graph.add(triple)

        # Subentidades
        for subentity in self.subentities:
            subentity.add_entity_to_ontology(ontology_config)
