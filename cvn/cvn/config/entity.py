from ..utils import code as cvn_code
import cvn.config.property as cvn_property
from rdflib.namespace import RDF
from rdflib import Literal, URIRef, BNode
import uuid
from cvn.config.relationship import Relationship
import requests
import re
import cvn.config.condition as cvn_condition
import cvn.config.entitycache as cvn_entity_cache
import urllib.parse
import cvn.utils.xmltree as xmltree
import cvn.webserver as web_server

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
    return "http://data.um.es/class/" + resource_class + "/" + identifier
    if web_server.debug:
        return "http://data.um.es/class/" + resource_class + "/" + identifier

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
        raise ValueError('code does not match expected format: ' + config['code'])

    ontology = "owl"
    classname = "Thing"

    if 'displayname' in config:
        display_name_format = re.compile("^[a-zA-Z]+:\w+$")
        if re.match(display_name_format, config['displayname']):
            split = config['displayname'].split(":")
            ontology = split[0]
            classname = split[1]
        else:
            raise ValueError('displayname has invalid format: ' + config['displayname'])
    else:
        if 'ontology' not in config:
            raise KeyError('ontology not specified for Entity')
            # TODO comprobar que está definida

        if 'classname' not in config:
            raise KeyError('classname not specified for Entity')

        ontology = config['ontology']
        classname = config['classname']

    # ID
    config_id_format = None
    config_id_resource = None
    if 'id' in config:
        if 'format' in config['id']:
            config_id_format = config['id']['format'].replace(":", "_")
        if 'resource' in config['id']:
            config_id_resource = config['id']['resource']

    primary = False
    if 'primary' in config:
        primary = config['primary']

    cache_property = None
    if 'cache' in config:
        cache_property = config['cache']

    sub_code = False
    if 'subcode' in config:
        sub_code = config['subcode']

    entity = Entity(code=code, ontology=ontology, classname=classname, parent=parent,
                    identifier_config_resource=config_id_resource, identifier_config_format=config_id_format,
                    primary=primary, property_cache=cache_property, sub_code=sub_code)

    # Populate properties
    if 'properties' in config:
        for property_config in config['properties']:
            property_generated = cvn_property.init_property_from_serialized_toml(property_config, entity)
            entity.add_property(property_generated)
            property_generated.parent = entity

    # Relationships
    if 'relationships' in config:
        for relationship in config['relationships']:

            ontology = None
            name = None

            if 'direct' in relationship:
                display_name_format = re.compile("^[a-zA-Z]+:\w+$")
                if re.match(display_name_format, relationship['direct']):
                    split = relationship['direct'].split(":")
                    ontology = split[0]
                    name = split[1]
                else:
                    raise ValueError('direct in relationship has invalid format')
            else:
                if 'name' in relationship:
                    if 'ontology' not in relationship:
                        raise KeyError('Relationship name was specified but no ontology for it')
                        # TODO comprobar que está definida
                    name = relationship['name']
                    ontology = relationship['ontology']

            inverse_name = None
            inverse_ontology = None
            if 'inverse' in relationship:
                display_name_format = re.compile("^[a-zA-Z]+:\w+$")
                if re.match(display_name_format, relationship['inverse']):
                    split = relationship['inverse'].split(":")
                    inverse_ontology = split[0]
                    inverse_name = split[1]
                else:
                    raise ValueError('inverse in relationship has invalid format')
            else:
                if 'inverse_name' in relationship:
                    if 'inverse_ontology' not in relationship:
                        raise KeyError('inverse Relationship name was specified but no ontology for it')
                        # TODO comprobar que está definida
                    inverse_name = relationship['inverse_name']
                    inverse_ontology = relationship['inverse_ontology']

            link_to_cvn_person = False
            if 'link_to_cvn_person' in relationship:
                link_to_cvn_person = relationship['link_to_cvn_person']

            if ((name is not None) and (ontology is not None)) \
                    or ((inverse_name is not None) and (inverse_ontology is not None)):
                generated_relationship = Relationship(ontology, name, inverse_ontology,
                                                      inverse_name, link_to_cvn_person, parent=entity)
                entity.add_relationship(generated_relationship)

    # Conditions
    if 'conditions' in config:
        for condition in config['conditions']:
            entity.add_condition(cvn_condition.init_condition_from_serialized_toml(condition, entity))

    # Subentities, recursive (optional)
    if 'subentities' in config:
        for subentity in config['subentities']:
            entity.add_subentity(init_entity_from_serialized_toml(subentity, entity))

    return entity


class Entity:
    # TODO todo el tema de la id y la URI
    def __init__(self, code, ontology, classname, parent=None, identifier_config_resource=None,
                 identifier_config_format=None, primary=False, property_cache=None, sub_code=False):
        self.code = code
        self.sub_code = sub_code
        self.ontology = ontology
        self.classname = classname
        self.subentities = []
        self.generated_subentities = []
        self.properties = []
        self.relationships = []
        self.parent = parent
        self.triplets = []
        self.generated_identifier = None
        self.identifier_config_resource = identifier_config_resource
        self.identifier_config_format = identifier_config_format
        self.node = None
        self.xml_item = None
        self.primary = primary
        self.conditions = []
        self.property_cache = property_cache

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

    def add_condition(self, condition):
        self.conditions.append(condition)
        return self

    def generate_and_add_to_ontology(self, ontology_config, xml_tree, skip_subentities_with_subcode=True, do_loop=True):
        if do_loop:
            for entity_result_node in xmltree.get_all_nodes_by_code(xml_tree, self.code):
                print("generating w/ loop: " + self.classname)
                self.get_property_values_from_node(entity_result_node, skip_subentities_with_subcode)
                self.add_entity_to_ontology(ontology_config, skip_subentities_with_subcode)

                for sub_entity in self.subentities:
                    loop = False
                    node = entity_result_node
                    if sub_entity.sub_code:
                        print("subcode loop")
                        loop = True
                    sub_entity.generate_and_add_to_ontology(ontology_config, node,
                                                            skip_subentities_with_subcode=False, do_loop=loop)
                self.clear_values()
        else:
            print("generating " + self.classname)
            self.get_property_values_from_node(xml_tree, skip_subentities_with_subcode)
            self.add_entity_to_ontology(ontology_config, skip_subentities_with_subcode)

            for sub_entity in self.subentities:
                loop = False
                if sub_entity.sub_code:
                    print("subcode no loop " + str(xml_tree))
                    loop = True
                    sub_entity.generate_and_add_to_ontology(ontology_config, xml_tree,
                                                            skip_subentities_with_subcode=False, do_loop=loop)
                else:
                    sub_entity.generate_and_add_to_ontology(ontology_config, xml_tree,
                                                            skip_subentities_with_subcode=False, do_loop=loop)
            self.clear_values()

    def get_property_values_from_node(self, item_node, skip_subentities_with_subcode):
        node = item_node
        for property_item in self.properties:
            property_item.get_value_from_node(item_node)
        self.xml_item = item_node

    def clear_values(self, include_sub_entities_with_sub_code=False):
        self.generated_identifier = None
        for property_item in self.properties:
            property_item.clear_values()
        self.node = None
        self.xml_item = None
        for subentity in self.subentities:
            if include_sub_entities_with_sub_code or not subentity.sub_code:
                subentity.clear_values()
        return self

    def is_blank_node(self):
        return self.identifier_config_resource is None

    def get_identifier(self):
        if self.generated_identifier is None:

            resource = self.classname
            if self.identifier_config_resource is not None:
                resource = self.identifier_config_resource

            identifier = str(uuid.uuid4())
            if self.identifier_config_format is not None:
                property_dict = self.get_property_dict(format_safe=True)
                if has_all_formatting_fields(self.identifier_config_format, property_dict):
                    identifier = urllib.parse.quote_plus(self.identifier_config_format.format_map(property_dict))

            self.generated_identifier = generate_uri(resource, identifier)

        return self.generated_identifier

    def get_uri(self):
        if self.is_blank_node():
            if self.node is None:
                self.node = BNode()
            return self.node
        if self.should_cache():
            if cvn_entity_cache.get_current_entity_cache().in_cache(self.get_cache_id()):
                return cvn_entity_cache.get_current_entity_cache().get(self.get_cache_id())
            else:
                uri = URIRef(self.get_identifier())
                cvn_entity_cache.get_current_entity_cache().add_to_cache(self.get_cache_id(), uri)
                return uri
        return URIRef(self.get_identifier())

    def generate_entity_triple(self, ontology_config):
        return self.get_uri(), RDF.type, ontology_config.get_ontology(self.ontology).term(self.classname)

    def generate_property_triples(self, ontology_config):
        triples = []
        default_type = ontology_config.get_default_data_type()
        for property_item in self.properties:
            if property_item.should_generate():

                # Valores por defecto: el tipo de datos definido como default y la propiedad como string simplón
                literal_type = ontology_config.get_ontology(default_type.ontology).term(default_type.name)
                property_value = str(property_item.formatted_value)

                # ¿Tiene la propiedad un tipo de dato específico? Si no, nos quedamos con el default
                if property_item.data_type is not None:
                    # El tipo de dato definido para la propiedad, ¿existe? - si no, el default
                    data_type = ontology_config.get_data_type(property_item.data_type)
                    if data_type is not None:
                        # Intentamos convertir el string en su tipo de dato correspondiente
                        try:
                            property_value = (data_type.get_python_type())(property_value)
                            literal_type = ontology_config.get_ontology(data_type.ontology).term(data_type.name)
                        except TypeError:
                            pass

                        if data_type.force:
                            literal_type = ontology_config.get_ontology(data_type.ontology).term(data_type.name)

                        # print("Generando tripleta de tipo " + str(type(property_value)) + " con valor "
                        #      + str(property_value))
                        # print("Generando tipo " + str(type(literal_type)) + " con valor " + str(literal_type))

                triple = self.get_uri(), \
                         ontology_config.get_ontology(property_item.ontology).term(property_item.name), \
                         Literal(property_value, datatype=literal_type)

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
            if (relationship.name is not None) and (relationship.ontology is not None):
                direct_triple = self.get_uri(), \
                                ontology_config.get_ontology(relationship.ontology).term(relationship.name), \
                                other
                triples.append(direct_triple)

            # Relación inversa
            if (relationship.inverse_name is not None) and \
                    (relationship.inverse_ontology is not None):
                inverse_triple = other, ontology_config.get_ontology(relationship.inverse_ontology) \
                    .term(relationship.inverse_name), self.get_uri()
                triples.append(inverse_triple)

        return triples

    def add_entity_to_ontology(self, ontology_config, skip_subentities_with_subcode):
        if not self.should_generate():
            return

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
            if skip_subentities_with_subcode and subentity.sub_code:
                subentity.add_entity_to_ontology(ontology_config, skip_subentities_with_subcode)
            else:
                subentity.add_entity_to_ontology(ontology_config, skip_subentities_with_subcode)

    def get_property_dict(self, format_safe=False):
        properties = {}
        for property_item in self.properties:
            if property_item.formatted_value is not None:
                if format_safe:
                    properties[property_item.get_format_safe_identifier()] = property_item.formatted_value
                else:
                    properties[property_item.get_identifier()] = property_item.formatted_value
        return properties

    def should_generate(self):
        if (len(self.properties) > 0) and self.are_properties_empty():
            return False
        for condition in self.conditions:
            if not condition.is_met():
                return False
        return True

    def are_properties_empty(self):
        for property_item in self.properties:
            if property_item.formatted_value is not None:
                return False
        for subentity in self.subentities:
            if not subentity.are_properties_empty():
                return False
        return True

    def should_cache(self):
        if self.property_cache is None:
            return False
        for property_item in self.properties:
            if property_item.get_identifier() == self.property_cache:
                if property_item.formatted_value is not None:
                    return True
        return False

    def get_cache_id(self):
        if self.property_cache is None:
            return None
        cached_property = None
        for property_item in self.properties:
            if property_item.get_identifier() == self.property_cache:
                cached_property = property_item.formatted_value
                break
        return self.ontology + ":" + self.classname + ":" + cached_property


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
