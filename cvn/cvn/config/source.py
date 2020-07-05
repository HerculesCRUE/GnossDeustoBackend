#  This file is part of Hércules ASIO.
#
#  Hércules ASIO is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#
#  Hércules ASIO is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#
#  You should have received a copy of the GNU General Public License
#  along with Hércules ASIO.  If not, see <https://www.gnu.org/licenses/>.
#

from ..utils import code as cvn_code
from cvn.utils.printable import Printable


def init_source_from_serialized_toml(config):
    # code and name are required attributes

    code = None
    if "code" in config:
        code = config["code"]
        if not cvn_code.is_cvn_code_valid(config["code"]):
            raise ValueError("code does not match expected format")

    if "name" not in config:
        raise KeyError("name not specified for Source")

    # Default value for bean
    bean = "Value"
    if "bean" in config:
        bean = config["bean"]

    # format is optional
    format_string = None
    if "format" in config:
        format_string = config["format"]

    return Source(code, config["name"], bean, format_string)


class Source(Printable):
    def __init__(self, code, name, bean, format_string):
        self.code = code
        self.name = name
        self.bean = bean
        self.format = format_string
        self.value = None
        self.formatted_value = None

    def set_value(self, value):
        self.value = value
        if value is None:
            self.formatted_value = None
        else:
            self.formatted_value = self.formatted()

    def formatted(self):
        """
        Formats the input value following the source format
        :return: the formatted value
        """
        if self.value is None:
            return None
        if self.format is None:
            return self.value
        return self.format.format(value=self.value)

    def get_value_from_node(self, item_node):
        result = item_node.find("{http://codes.cvn.fecyt.es/beans}" + self.bean)
        if result is not None:
            self.set_value(result.text)
        return self.formatted_value

    def clear_value(self):
        self.value = None
        self.formatted_value = None

    def __str__(self):
        return str(self.__class__) + ": " + str(self.__dict__)
