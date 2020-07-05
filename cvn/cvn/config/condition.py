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

import cvn.config.entity as cvn_entity
import cvn.config.property as cvn_property
import cvn.config.relationship as cvn_relationship
from cvn.utils import xmltree


def init_condition_from_serialized_toml(config, parent):
    """Genera una Condition a partir de un array

    Dentro de la config puede haber los siguientes argumentos:
    (obligatorios:)
    - type (str): ver documentación de Condition para una lista de posibles valores y sus efectos
    - code (str): el código CVN del bean sobre el que realizar la comprobación
    (opcionales:)
    - negated (bool): la condición se cumple si
    - bean (str)
    - value: en las

    Ejemplo de TOML para representar una condición que comprueba que el bean con código "030.010.000.210" tiene el valor
     "010":

        [[entities.subentities.properties.conditions]]
            type = "bean_value_equals"
            code = "030.010.000.210"
            bean = "Value"
            value = "010"

    :param config: el array correspondiente a la condición en la configuración
    :param parent: la clase (entidad, propiedad...) a la que se aplicará la condición
    :return: Condition
    """
    if "type" not in config:
        raise KeyError("type not present in Condition declaration: " + config)

    if "code" not in config:
        raise KeyError("code not present in Condition declaration: " + config)

    negated = False
    if "negated" in config:
        negated = config["negated"]
    bean = "Value"

    if "bean" in config:
        bean = config["bean"]

    value = None
    if "value" in config:
        value = config["value"]

    return Condition(
        condition_type=config["type"],
        code=config["code"],
        value=value,
        parent=parent,
        bean=bean,
        negated=negated,
    )


class Condition:
    """Representa una condición que puede darse o no para decidir si generar las tripletas de una clase (Entity, Property, Relationship)

    Tipos: (condition_type)
    - bean_present: la propiedad tiene un bean con ese código (da igual el valor)
    - bean_value_equals: la propiedad tiene un bean (bean) con un código específico (code) y con el valor (value) configurado

    Dentro de la config puede haber los siguientes argumentos:
    - type (str): obligatorio, ver documentación de Condition para una lista de posibles valores y sus efectos
    - code (str): obligatorio,

    """

    ALLOWED_TYPES = [
        "bean_value_equals",
        "bean_present",
    ]  # TODO comprobar a la hora de crear si está en este array

    def __init__(
        self, condition_type, code, parent, value=None, bean="Value", negated=False
    ):
        self.condition_type = condition_type
        self.code = code
        self.value = value
        self.bean = bean
        self.parent = parent
        self.negated = negated

    def is_met(self):
        """
        Realiza las comprobaciones para determinar si se cumple o no la condición en el XML de la clase padre

        :return: bool, si se cumple o no la condición
        """
        if self.parent is None:
            return not self.negated

        # Primero tenemos que obtener el árbol XML para el item
        xml_item = None
        if isinstance(self.parent, cvn_entity.Entity):
            if self.parent.parent is not None:
                xml_item = self.parent.parent.xml_item
            else:
                xml_item = self.parent.xml_item
        if isinstance(self.parent, cvn_property.Property) or isinstance(
            self.parent, cvn_relationship.Relationship
        ):
            xml_item = self.parent.parent.xml_item

        # Luego ya actuamos en consecuencia
        if self.condition_type == "bean_value_equals":
            for element in xmltree.get_all_nodes_by_code(xml_item, self.code):
                result = element.find("{http://codes.cvn.fecyt.es/beans}" + self.bean)
                if (result is not None) and (result.text == self.value):
                    return not self.negated
            return self.negated

        if self.condition_type == "bean_present":
            for element in xmltree.get_all_nodes_by_code(xml_item, self.code):
                result = element.find("{http://codes.cvn.fecyt.es/beans}" + self.bean)
                if result is not None:
                    return not self.negated
            return self.negated

        return not self.negated  # ante la duda por defecto
