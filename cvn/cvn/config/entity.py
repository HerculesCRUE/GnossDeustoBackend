from ..utils import code as cvn_code
import cvn.config.property as cvn_property
from rdflib.namespace import RDF
from rdflib import Literal, URIRef
import uuid
from cvn.config.relationship import Relationship
import requests
import re

# Caché de URIs generadas
# TODO mover a un servicio externo, o hacer algo más elaborado
# Problema: memoria...? múltiples ejecuciones = se borra
cached_uris = {}


def generate_uri(resource_class, identifier):
    """
    Generar, usando la API HTTP, las URIs correspondientes a cada entidad.
    :param resource_class:
    :param identifier:
    :return: la URI
    """
    # Antes de intentar obtener la URI, comprobar a ver si la tenemos ya en caché
    cache_id = resource_class + "." + identifier
    if cache_id in cached_uris:
        return cached_uris[cache_id]

    api_response = requests.get("http://herc-as-front-desa.atica.um.es/uris/Factory", params={
        'resource_class': resource_class,
        'identifier': identifier
    })  # TODO comprobar que lo que devuelve es de hecho una URL bien formateada

    # Si falla, nos la jugamos y nos inventamos una URI que podría ser:
    if api_response.status_code != 200:
        return "http://data.um.es/class/" + resource_class + "/" + identifier

    # Guardamos en caché si ha salido bien
    result = api_response.text
    cached_uris[cache_id] = result
    return result


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
    if 'properties' in config:
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
    def __init__(self, code, ontology, classname, parent=None, identifier_config_resource=None,
                 identifier_config_format=None):
        self.code = code
        self.ontology = ontology
        self.classname = classname
        self.subentities = []
        self.properties = []
        self.relationships = []
        self.parent = parent
        self.triplets = []
        self.generated_identifier = None
        self.identifier_config_resource = identifier_config_resource
        self.identifier_config_format = identifier_config_format

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
        self.generated_identifier = None
        for property_item in self.properties:
            property_item.clear_values()
        for subentity in self.subentities:
            subentity.clear_values()
        return self

    def get_identifier(self):
        if self.generated_identifier is None:

            resource = self.classname
            if self.identifier_config_resource is not None:
                resource = self.identifier_config_resource

            identifier = str(uuid.uuid4())
            if self.identifier_config_format is not None:
                property_dict = self.get_property_dict()
                if has_all_formatting_fields(self.identifier_config_format, property_dict):
                    identifier = self.identifier_config_format.format_map(property_dict)

            self.generated_identifier = generate_uri(resource, identifier)

        return self.generated_identifier

    def get_uri(self):
        return URIRef(self.get_identifier())

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
                # TODO comprobar que person no sea None
            else:
                if self.parent is None:
                    continue  # Si es una relación con el padre, pero no tiene... nos la saltamos
                other = self.parent.get_uri()

            # Relación directa
            direct_triple = self.get_uri(), ontology_config.get_ontology(relationship.ontology).term(
                relationship.name), other
            triples.append(direct_triple)

            # Relación inversa
            if (relationship.inverse_name is not None) and \
                    (relationship.inverse_ontology is not None):
                inverse_triple = other, ontology_config.get_ontology(relationship.inverse_ontology) \
                    .term(relationship.inverse_name), self.get_uri()
                triples.append(inverse_triple)

        return triples

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

    def get_property_dict(self):
        properties = {}
        for property_item in self.properties:
            if property_item.formatted_value is not None:
                properties[property_item.get_identifier()] = property_item.formatted_value
        return properties


def has_all_formatting_fields(format_string, fields):
    """
    Comprueba que todos los campos de formateo estén definidos en el diccionario
    :param format_string: el texto que se le pasa al Formatter
    :param fields: los campos con los valores que se usan para rellenar
    :return: bool ¿están todos los campos de formateo cubiertos por el diccionario?
    """
    # Busca los valores entre {} y los devuelve en una lista
    format_fields = re.findall(r'{(.*?)}', format_string)

    for field in format_fields:
        if field not in fields:
            return False
    return True