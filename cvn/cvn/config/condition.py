# Tipos:
# property_present: la propiedad tiene un valor válido
# property_not_present: la propiedad no tiene un valor válido
# bean_value_equals -- la propiedad tiene un valor específico único o array (múltiples permitidos)
# property_regex: (PRÓXIMAMENTE) la propiedad está presente y su valor cumple el formato de una expresión regular
# entity_code
# property_code
import cvn.config.entity as cvn_entity
import cvn.config.property as cvn_property
import cvn.config.relationship as cvn_relationship
from cvn.utils import xmltree


def init_condition_from_serialized_toml(config, parent):
    if 'type' not in config:
        raise KeyError('type not present in Condition declaration: ' + config)
    # if 'type' not in Condition.ALLOWED_TYPES:
    #    raise ValueError('Condition type not supported: ' + config['type'])
    if 'code' not in config:
        raise KeyError('code not present in Condition declaration: ' + config)
    bean = "Value"
    if 'bean' in config:
        bean = config['bean']
    return Condition(condition_type=config['type'], code=config['code'], value=config['value'],
                     parent=parent, bean=bean)


class Condition:
    ALLOWED_TYPES = ['bean_value_equals']

    def __init__(self, condition_type, code, parent, value=None, bean="Value"):
        self.condition_type = condition_type
        self.code = code
        self.value = value
        self.bean = bean
        self.parent = parent

    def is_met(self):
        if self.parent is None:
            return True

        # Primero tenemos que obtener el árbol XML para el item
        xml_item = None
        if isinstance(self.parent, cvn_entity.Entity):
            xml_item = self.parent.xml_item
        if isinstance(self.parent, cvn_property.Property) or isinstance(self.parent, cvn_relationship.Relationship):
            xml_item = self.parent.parent.xml_item

        # Luego ya actuamos en consecuencia
        if self.condition_type == "bean_value_equals":
            for element in xmltree.get_all_nodes_by_code(xml_item, self.code):
                result = element.find("{http://codes.cvn.fecyt.es/beans}" + self.bean)
                if (result is not None) and (result.text == self.value):
                    return True
            return False

        return True  # ante la duda Sí
