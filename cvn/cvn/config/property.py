from cvn.utils.printable import Printable
import cvn.config.source as cvn_source
from cvn.utils import xmltree
import re


def init_property_from_serialized_toml(config):

    ontology = "owl"
    name = "topDataProperty"
    if 'displayname' in config:
        display_name_format = re.compile("^[a-zA-Z]+:\w+$")
        if re.match(display_name_format, config['displayname']):
            split = config['displayname'].split(":")
            ontology = split[0]
            name = split[1]
        else:
            raise ValueError('displayname has invalid format')
    else:
        if 'ontology' not in config:
            raise KeyError('ontology not specified for Property')
            # TODO comprobar que está definida

        if 'name' not in config:
            raise KeyError('name not specified for Property')

    if 'format' not in config:
        raise KeyError('format not specified for Property')

    generated_property = Property(ontology, name, config['format'])

    if 'sources' not in config:
        raise KeyError('no sources defined for Property')
    for source in config['sources']:
        generated_source = cvn_source.init_source_from_serialized_toml(source)
        generated_property.add_source(generated_source)

    return generated_property


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


class Property(Printable):
    def __init__(self, ontology, name, format_string):
        self.ontology = ontology
        self.name = name
        self.format = format_string
        self.sources = []
        self.formatted_value = None

    def add_source(self, source):
        self.sources.append(source)
        return self

    def get_source_dict(self):
        sources = {}
        for source in self.sources:
            if source.formatted_value is not None:
                sources[source.name] = source.formatted_value
        return sources

    def formatted(self):
        try:
            formatted = self.format.format_map(self.get_source_dict())
            self.formatted_value = formatted
            return formatted
        except KeyError:
            pass # TODO error handling
        return None

    def get_value_from_node(self, item_node):
        sources = {}
        # Rellenar todas las sources
        for source in self.sources:
            source_node = xmltree.get_first_node_by_code(item_node, source.code)
            if source_node is not None:
                source.get_value_from_node(source_node)
        # Formatear
        return self.formatted()

    def clear_values(self):
        self.formatted_value = None
        for source in self.sources:
            source.clear_value()

    def generate_triple(self):
        print("tripleta property (entidad, " + str(self.ontology) + ":" + str(self.name) + ", "
              + str(self.formatted_value) + ")")

    def get_identifier(self):
        return str(self.ontology) + ":" + str(self.name)
