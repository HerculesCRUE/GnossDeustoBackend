from cvn.utils.printable import Printable
import cvn.config.source as cvn_source
import cvn.config.condition as cvn_condition
from cvn.utils import xmltree
import re


def init_property_from_serialized_toml(config, entity_parent):
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
        ontology = config['ontology']

        if 'name' not in config:
            raise KeyError('name not specified for Property')
        name = config['name']

    format_string = None
    if 'format' in config:
        format_string = config['format']

    hidden = False
    if 'hidden' in config:
        hidden = config['hidden']

    generated_property = Property(ontology=ontology, name=name, format_string=format_string,
                                  hidden=hidden, parent=entity_parent)

    if 'sources' not in config:
        raise KeyError('no sources defined for Property')
    for source in config['sources']:
        generated_source = cvn_source.init_source_from_serialized_toml(source)
        generated_property.add_source(generated_source)

    # Conditions
    if 'conditions' in config:
        for condition in config['conditions']:
            generated_property.add_condition(cvn_condition.init_condition_from_serialized_toml(condition,
                                                                                               generated_property))

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


class Default(dict):
    # solución rápida para cuando no hay un valor de un format
    #  https://stackoverflow.com/a/19800610
    def __missing__(self, key):
        return ''


class Property(Printable):
    def __init__(self, ontology, name, parent, format_string=None, hidden=False):
        self.ontology = ontology
        self.name = name
        self.format = format_string
        self.sources = []
        self.conditions = []
        self.formatted_value = None
        self.hidden = hidden
        self.parent = parent

    def add_source(self, source):
        self.sources.append(source)
        return self

    def add_condition(self, condition):
        self.conditions.append(condition)
        return self

    def get_source_array(self):
        sources = []
        for source in self.sources:
            if source.formatted_value is not None:
                sources.append(source.formatted_value)
        return sources

    def get_source_dict(self):
        sources = {}
        for source in self.sources:
            if source.formatted_value is not None:
                sources[source.name] = source.formatted_value
        return sources

    def formatted(self):
        if self.format is None:
            result = ""
            for source in self.get_source_array():
                result = result + str(source)
            if result == "":
                return None
            self.formatted_value = result
            return result
        else:
            try:
                formatted = self.format.format_map(Default(self.get_source_dict())).strip()
                if formatted != "":
                    self.formatted_value = formatted
                    return formatted
                return None
            except KeyError:
                pass  # TODO error handling
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

    def should_generate(self):
        if self.hidden or self.formatted_value is None:
            return False
        for condition in self.conditions:
            if not condition.is_met():
                return False
        return True

    def get_identifier(self):
        return str(self.ontology) + ":" + str(self.name)
