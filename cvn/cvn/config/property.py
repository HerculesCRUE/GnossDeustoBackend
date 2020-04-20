from cvn.utils.printable import Printable
import cvn.config.source as cvn_source


def init_property_from_serialized_toml(config):
    if 'ontology' not in config:
        raise KeyError('ontology not specified for Property')
        # TODO comprobar que está definida

    if 'name' not in config:
        raise KeyError('name not specified for Property')

    if 'format' not in config:
        raise KeyError('format not specified for Property')

    generated_property = Property(config['ontology'], config['name'], config['format'])

    if 'sources' not in config:
        raise KeyError('no sources defined for Property')
    for source in config['sources']:
        generated_source = cvn_source.init_source_from_serialized_toml(source)
        generated_property.add_source(generated_source)

    return generated_property


class Property(Printable):
    def __init__(self, ontology, name, format_string):
        self.ontology = ontology
        self.name = name
        self.format = format_string
        self.sources = []

    def add_source(self, source):
        self.sources.append(source)
        return self
