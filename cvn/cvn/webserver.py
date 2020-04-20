#
# webserver.py
# Parte de HERCULES-ASIO
# ----------------------
# Crea una API HTTP que permite la conversión de XML CVN a tripletas ROH.
#
# ** EN DESARROLLO **
#
import argparse
import xml.etree.ElementTree as ET
from rdflib import Graph, Namespace, Literal, URIRef
from rdflib.namespace import RDF, NamespaceManager
from flask import Flask, request, make_response, jsonify
import re
import requests
import toml
import uuid

app = Flask(__name__)
app.config['MAX_CONTENT_LENGTH'] = 16 * 1024 * 1024  # 16MB máx.

# Formatos de salida permitidos
ALLOWED_FORMATS = ["xml", "n3", "turtle", "nt", "pretty-xml", "trix", "trig", "nquads"]

# Caché para los nombres de códigos
# TODO estudiar caché más elaborado, también para URIs generadas
code_name = {}


# TODO testing
# TODO modularizar, quitar spaghetti code
# TODO crear clases para config y units tests correspondientes
# TODO documentación


@app.route('/v1/convert', methods=['POST'])
def v1_convert():
    # ---
    # Validación de la solicitud
    # ---

    # Validar la solicitud
    # Comprobar el archivo que nos llega
    # Comprobar los argumentos

    input_xml = request.get_data().decode()

    if request.get_data() is None:
        return make_validation_error("An xml file is required as binary body data.")

    if request.args.get('orcid') is None:
        return make_validation_error("The orcid parameter is required.")

    # Validar si es una ID de ORCID real
    pattern = re.compile('0000-000(1-[5-9]|2-[0-9]|3-[0-4])\d{3}-\d{3}[\dX]')
    if not pattern.match(request.args.get('orcid')):
        return make_validation_error("The orcid field has an invalid format.")

    # Guardar algunos parámetros en el diccionario para luego generar bien el resultado
    params = {'orcid': request.args.get('orcid'), 'format': "xml"}

    # Si se especifica el formato, solo permitir ciertos valores
    if request.args.get('format') is not None:
        if request.args.get('format') not in ALLOWED_FORMATS:
            return make_validation_error("The format field has an unsupported format.")
        params['format'] = request.args.get('format')

    try:
        root = ET.fromstring(input_xml)
    except Exception as e:
        return make_error_response("Error while parsing the XML.")

    # ---
    # Grafo y ontologías
    # ---

    # Lista de ontologías para acceder a ellas desde la config
    # TODO mover a una clase con funciones de utilidad
    ontologies = {}
    ontology_primary = None  # TODO comprobar que se ha definido una ontología primaria

    with open("mappings/cvn/1.4.2_sp1/cvn-to-roh/ontologies.toml") as f:
        config_ontologies = toml.loads(f.read())
        # TODO error handling config ontologías error lectura

    # El NamespaceManager se encarga de gestionar las ontologías enlazadas
    namespace_manager = NamespaceManager(Graph())

    # Recorrer la config de ontologías y añadirlas al NamespaceManager
    for ontology in config_ontologies['ontologies']:
        ontologies[ontology['shortname']] = Namespace(ontology['uri_base'])
        namespace_manager.bind(ontology['shortname'], ontologies[ontology['shortname']])
        if ('primary' in ontology) and ontology['primary']:
            ontology_primary = ontologies[ontology['shortname']]

    # Iniciar grafo principal con el NamespaceManager rellenado de ontologías
    g = Graph()
    g.namespace_manager = namespace_manager

    person = URIRef(generate_uri('Researcher', params['orcid']))

    # ----- INICIO PROCESO CVN

    # 1. Datos personales
    with open("mappings/cvn/1.4.2_sp1/cvn-to-roh/1-personal-data.toml") as f:  # TODO des-hardcodificar
        config = toml.loads(f.read())
        # TODO error handling si archivo no se puede abrir
        # TODO validar TOML

    # Lógica a seguir
    #
    # Recorrer instances. En cada uno:
    #   Instanciar en la ontología la clase, aunque esté vacía
    #       Para cada propiedad:
    #           Recorrer todos los sources:
    #               Obtener valor
    #               Darle formato
    #               Guardarlos en un diccionario para que el source pueda acceder a ellos
    #           Aplicar formato
    #           Guardar en el grafo

    person = URIRef(generate_uri(config['instance']['classname'], params['orcid']))
    g.add((person, RDF.type, ontology_primary.term(config['instance']['classname'])))

    # Representa el único nodo que contiene todos los datos personales del CVN
    info_node = get_first_node_by_code(root, config['code'])

    for data_property in config['properties']:
        # Formateamos
        formatted_value = data_property['format'].format_map(get_sources_from_property(data_property, info_node))

        g.add((person, ontologies[data_property['ontology']].term(data_property['name']),
               Literal(formatted_value)))

        # Sources procesadas, formateamos texto

    # 2. Generar entidades

    # Cargar config
    with open("mappings/cvn/1.4.2_sp1/cvn-to-roh/2-entities.toml") as f:  # TODO des-hardcodificar
        entities_config = toml.loads(f.read())

    for entity in entities_config['entities']:
        # Para cada tipo de entidad buscamos en el árbol las que tengan el código
        for entity_result_node in get_all_nodes_by_code(root, entity['code']):
            properties = get_properties_from_node(entity, entity_result_node)

            # Generación de la URI
            identifier = uuid.uuid4()  # por defecto UUIDv4
            resource_class = entity['classname']  # por defecto el nombre de la clase de la entidad
            if 'id' in entity:
                if 'resource' in entity['id']:
                    resource_class = entity['id']['resource']
                if 'format' in entity['id']:
                    if has_all_formatting_fields(entity['id']['format'], properties):
                        identifier = entity['id']['format'].format_map(properties).strip()

            # Generamos la tripleta de la entidad como tal
            current_entity = URIRef(generate_uri(resource_class, str(identifier)))
            g.add((current_entity, RDF.type, ontologies[entity['ontology']].term(entity['classname'])))

            # La rellenamos con las propiedades
            for class_property in entity['properties']:
                sources = get_sources_from_property(class_property, entity_result_node)
                if has_all_formatting_fields(class_property['format'], sources):
                    formatted = class_property['format'].format_map(sources)
                    # TODO validar que se tienen todos los parámetros necesarios
                    # TODO no mostrar si es "None"
                    g.add((current_entity, ontologies[class_property['ontology']].term(class_property['name']),
                           Literal(formatted)))

            # Relaciones (modo sencillo)
            # TODO cambiar spanglish relations por relationship
            if 'relations' in entity:
                for relation in entity['relations']:
                    if 'link_to_cvn_person' in relation and relation['link_to_cvn_person']:
                        g.add((person, ontologies[relation['ontology']].term(relation['name']), current_entity))
                    if 'inverse_name' in relation:
                        inverse_ontology = relation['ontology']
                        if 'inverse_ontology' in relation:
                            inverse_ontology = relation['inverse_ontology']
                        g.add((current_entity, ontologies[inverse_ontology].term(relation['inverse_name']), person))

    return make_response(g.serialize(format=params['format']), 200)  # TODO Quitar, DEBUG

    # 3. Enlazar

    # TODO limpiar comentarios
    # ----- FIN PROCESO CVN
    # Vamos a ir recorriendo el árbol XML del documento CVN e iremos sacando, cuando sea necesario, información, para
    # luego insertarla en tripletas generadas al vuelo con rdflib
    # Serializar y guardar en un archivo con el formato que queramos


def generate_class_from_(entity, parent=None):
    return None  # TODO


def get_properties_from_node(entity_config, node):
    """
    Recorre un nodo y genera un diccionario con propiedades y sus valores formateados
    :param entity_config: el objeto de configuración
    :param node: el nodo XML del CVN de donde queremos sacar las propiedades
    :return:
    """
    properties = {}
    for property_config in entity_config['properties']:
        sources = get_sources_from_property(property_config, node)
        if 'format' in property_config and has_all_formatting_fields(property_config['format'], sources):
            properties[property_config['name']] = property_config['format'].format_map(sources)
    return properties
    # hay algún source que no se ha generado
    # TODO validar que se tienen todos los parámetros necesarios
    # Posible problema aquí: ¿qué pasa si hay dos propiedades con el mismo nombre de distintas ontologías?


def get_sources_from_property(current_property, node):
    # Declaramos un dict. para que podamos guardar los valores de los sources
    sources = {}
    for source in current_property['sources']:
        source_node = get_first_node_by_code(node, source['code'])
        if source_node is not None:
            result = source_node.find("{http://codes.cvn.fecyt.es/beans}" + source['bean'])
            if result is not None:
                # Formateamos source
                if 'format' in source:
                    sources[source['name']] = source['format'].format(value=result.text)
                else:
                    sources[source['name']] = result.text
            else:
                sources[source['name']] = None
    return sources


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


def get_first_node_by_code(tree, code):
    """
    Devuelve el primer elemento del árbol con el código especificado
    :param tree: el árbol de nodos donde buscar
    :param code: el código que buscamos
    :return: el primer elemento del árbol con el código, si no, None
    """
    for child in tree:
        if node_get_code(child) == code:
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
    for child in tree:
        if node_get_code(child) == code:
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

    find_result = node.find('{http://codes.cvn.fecyt.es/beans}Code')

    if find_result is None:
        for child in node:
            if child.tag == "{http://codes.cvn.fecyt.es/beans}Code":
                return child.text

    return find_result.text


def code_get_name(code, lang='spa'):
    """
    Obtener el nombre de un código CVN a partir de los mapeos XML.
    OJO: muy mal rendimiento, usar solo para debug.
    :param lang: idioma en el que queremos obtener los nombres
    :param code: el código del que queremos obtener el nombre
    :return: string el nombre del código
    """
    if code in code_name:
        return code_name[code]
    tree = ET.parse('mappings/cvn/1.4.2_sp1/XSD/SpecificationManual.xml')  # hardcoded for now
    result = tree.getroot().find("./*Item[@code='" + code + "']/Name/NameDetail[@lang='" + lang + "']/Name").text
    code_name[code] = result
    return result


def make_validation_error(message):
    return make_response(jsonify({'error': message}), 422)  # 422 = Unprocessable Entity
    # TODO juntar esta función y make_error_response


def make_error_response(message):
    return make_response(jsonify({'error': message}), 500)  # 422 = Unprocessable Entity


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


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Servidor HTTP que ofrece una API para convertir XML CVN a tripletas "
                                                 "ROH")
    parser.add_argument("-p", "--port", type=int, default=5000, choices=range(0, 65536),
                        help="El puerto en el que se ejecutará el servidor HTTP (por defecto 5000)", metavar="")
    parser.add_argument("--debug", action="store_true", help="DEBUG: activar modo debug (aumenta tiempo de ejecución)")
    args = parser.parse_args()

    app.run(debug=args.debug, port=args.port)

# 19/03/2020 Iñigo — lo que he podido en este poco tiempo, lo siento por el código desastroso :^(
# 20/03/2020 Iñigo — he cambiado el script para que sea una API HTTP en vez de una utilidad de consola. Lo he hecho
#                    rápido, por lo que no es muy configurable por ahora.
