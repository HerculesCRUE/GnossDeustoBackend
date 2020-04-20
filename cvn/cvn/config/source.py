from ..utils import code as cvn_code
from cvn.utils.printable import Printable


def init_source_from_serialized_toml(config):
    # code and name are required attributes

    if 'code' not in config:
        raise KeyError('code not specified for Source')
    if not cvn_code.is_cvn_code_valid(config['code']):
        raise ValueError('code does not match expected format')

    if 'name' not in config:
        raise KeyError('name not specified for Source')

    # Default value for bean
    bean = "Value"
    if 'bean' in config:
        bean = config['bean']

    # format is optional
    format_string = None
    if 'format' in config:
        format_string = config['format']

    return Source(config['code'], config['name'], bean, format_string)


class Source(Printable):
    def __init__(self, code, name, bean, format_string):
        self.code = code
        self.name = name
        self.bean = bean
        self.format = format_string

    def format(self, value):
        """
        Formats the input value following the source format
        :param value: the value of the source
        :return: the formatted value
        """
        if self.format is None:
            return value
        return self.format.format(value=value)

    def __str__(self):
        return str(self.__class__) + ": " + str(self.__dict__)