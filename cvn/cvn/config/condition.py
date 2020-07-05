# Tipos:
# bean_present: la propiedad tiene un bean con ese código
# bean_value_equals -- la propiedad tiene un valor específico único o array (múltiples permitidos)
import cvn.config.entity as cvn_entity
import cvn.config.property as cvn_property
import cvn.config.relationship as cvn_relationship
from cvn.utils import xmltree


def init_condition_from_serialized_toml(config, parent):
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
    ALLOWED_TYPES = ["bean_value_equals", "bean_present"]

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
