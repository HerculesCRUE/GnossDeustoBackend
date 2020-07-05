import xml.etree.ElementTree as ET

# Caché para los nombres de códigos
# TODO estudiar caché más elaborado, también para URIs generadas
code_name = {}


def get_first_node_by_code(tree, code):
    """
    Devuelve el primer elemento del árbol con el código especificado
    :param tree: el árbol de nodos donde buscar
    :param code: el código que buscamos
    :return: el primer elemento del árbol con el código, si no, None
    """
    for child in tree:
        if (
            node_get_code(child) == code
            and child.tag != "{http://codes.cvn.fecyt.es/beans}Code"
        ):
            return child
    return None


def get_all_nodes_by_code(tree, code):
    """
    Recorre un árbol y devuelve todos los elementos inmediatamente debajo que tienen el código
    :param tree: el árbol de nodos donde buscar
    :param code: el código que buscamos
    :return: array con los nodos que buscamos, si no, None
    """
    nodes = []
    if tree is None:
        return nodes
    for child in tree:
        if (
            node_get_code(child) == code
            and child.tag != "{http://codes.cvn.fecyt.es/beans}Code"
        ):
            nodes.append(child)
    return nodes


def node_get_code(node):
    """
    Obtener el código CVN de un nodo XML.
    :param node: el nodo XML
    :return: string con el código del nodo
    """

    if node.tag == "{http://codes.cvn.fecyt.es/beans}Code":
        return node.text

    find_result = node.find("{http://codes.cvn.fecyt.es/beans}Code")

    if find_result is None:
        for child in node:
            if child.tag == "{http://codes.cvn.fecyt.es/beans}Code":
                return child.text

    if find_result is None:
        return None

    return find_result.text


def code_get_name(code, lang="spa"):
    """
    Obtener el nombre de un código CVN a partir de los mapeos XML.
    OJO: muy mal rendimiento, usar solo para debug.
    :param lang: idioma en el que queremos obtener los nombres
    :param code: el código del que queremos obtener el nombre
    :return: string el nombre del código
    """
    if code in code_name:
        return code_name[code]
    tree = ET.parse(
        "mappings/cvn/1.4.2_sp1/XSD/SpecificationManual.xml"
    )  # TODO des-hardcodificar
    result = (
        tree.getroot()
        .find(
            "./*Item[@code='" + code + "']/Name/NameDetail[@lang='" + lang + "']/Name"
        )
        .text
    )
    code_name[code] = result
    return result
