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
from rdflib import Graph, Literal, URIRef
from rdflib.namespace import RDF, NamespaceManager
from flask import Flask, request, make_response, jsonify
import re
import toml
from cvn.config import entity as config_entity
from cvn.config.ontology import OntologyConfig, Ontology
from cvn.utils import xmltree

app = Flask(__name__)
app.config['MAX_CONTENT_LENGTH'] = 16 * 1024 * 1024  # 16MB máx.

# Formatos de salida permitidos
ALLOWED_FORMATS = ["xml", "n3", "turtle", "nt", "pretty-xml", "trix", "trig", "nquads"]


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

    with open("mappings/cvn/1.4.2_sp1/cvn-to-roh/ontologies.toml") as f:
        config_ontologies = toml.loads(f.read())
        # TODO error handling config ontologías error lectura

    # El NamespaceManager se encarga de gestionar las ontologías enlazadas
    namespace_manager = NamespaceManager(Graph())

    ontology_config = OntologyConfig()

    # Recorrer la config de ontologías y añadirlas al NamespaceManager
    for ontology in config_ontologies['ontologies']:
        ontology_instance = Ontology(short_name=ontology['shortname'], uri_base=ontology['uri_base'])
        ontology_config.add_ontology(ontology_instance)
        namespace_manager.bind(ontology_instance.short_name, ontology_instance.namespace)

    # Iniciar grafo principal con el NamespaceManager rellenado de ontologías
    g = Graph()
    g.namespace_manager = namespace_manager

    ontology_config.graph = g

    person = URIRef(config_entity.generate_uri('Researcher', params['orcid']))

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

    person = URIRef(config_entity.generate_uri(config['instance']['classname'], params['orcid']))
    ontology_config.cvn_person = person
    g.add((person, RDF.type, ontology_config.get_ontology(config['instance']['ontology']).term(config['instance']['classname'])))

    # Representa el único nodo que contiene todos los datos personales del CVN
    info_node = xmltree.get_first_node_by_code(root, config['code'])

    for data_property in config['properties']:
        # Formateamos
        formatted_value = data_property['format'].format_map(get_sources_from_property(data_property, info_node))

        g.add((person, ontology_config.get_ontology(data_property['ontology']).term(data_property['name']),
               Literal(formatted_value)))

        # Sources procesadas, formateamos texto

    # 2. Generar entidades

    # Cargar config
    with open("mappings/cvn/1.4.2_sp1/cvn-to-roh/2-entities.toml") as f:  # TODO deshardcodificar
        entities_config = toml.loads(f.read())

    # Generar instancias Entity
    entities = []
    for entity_config in entities_config['entities']:
        entities.append(config_entity.init_entity_from_serialized_toml(entity_config))

    for entity in entities:
        # Para cada tipo de entidad buscamos en el árbol las que tengan el código
        for entity_result_node in xmltree.get_all_nodes_by_code(root, entity.code):
            # properties = get_properties_from_node(entity, entity_result_node)

            entity.get_property_values_from_node(entity_result_node)
            # entity.generate_property_triples()
            # MAL MAL MAL no se puede pretender rellenar una entidad con la información de todas, hay que, de alguna
            # manera intentar

            # Generación de la URI
            # identifier = uuid.uuid4()  # por defecto UUIDv4
            # resource_class = entity['classname']  # por defecto el nombre de la clase de la entidad
            # if 'id' in entity:
            #     if 'resource' in entity['id']:
            #         resource_class = entity['id']['resource']
            #     if 'format' in entity['id']:
            #         if has_all_formatting_fields(entity['id']['format'], properties):
            #             identifier = entity['id']['format'].format_map(properties).strip()

            # Generamos la tripleta de la entidad como tal
            entity.add_entity_to_ontology(ontology_config)

            # # La rellenamos con las propiedades
            # for class_property in entity['properties']:
            #     sources = get_sources_from_property(class_property, entity_result_node)
            #     if has_all_formatting_fields(class_property['format'], sources):
            #         formatted = class_property['format'].format_map(sources)
            #         # TODO validar que se tienen todos los parámetros necesarios
            #         # TODO no mostrar si es "None"
            #         g.add((current_entity, ontologies[class_property['ontology']].term(class_property['name']),
            #                Literal(formatted)))

            # # Relaciones (modo sencillo)
            # # TODO cambiar spanglish relations por relationship
            # if 'relations' in entity:
            #     for relation in entity['relations']:
            #         if 'link_to_cvn_person' in relation and relation['link_to_cvn_person']:
            #             g.add((person, ontologies[relation['ontology']].term(relation['name']), current_entity))
            #         if 'inverse_name' in relation:
            #             inverse_ontology = relation['ontology']
            #             if 'inverse_ontology' in relation:
            #                 inverse_ontology = relation['inverse_ontology']
            #             g.add((current_entity, ontologies[inverse_ontology].term(relation['inverse_name']), person))

            # Limpiamos la entidad
            entity.clear_values()

    return make_response(g.serialize(format=params['format']), 200)  # TODO Quitar, DEBUG

    # 3. Enlazar
    # TODO limpiar comentarios
    # ----- FIN PROCESO CVN
    # Vamos a ir recorriendo el árbol XML del documento CVN e iremos sacando, cuando sea necesario, información, para
    # luego insertarla en tripletas generadas al vuelo con rdflib
    # Serializar y guardar en un archivo con el formato que queramos


# def get_properties_from_node(entity_config, node):
#     """
#     Recorre un nodo y genera un diccionario con propiedades y sus valores formateados
#     :param entity_config: el objeto de configuración
#     :param node: el nodo XML del CVN de donde queremos sacar las propiedades
#     :return:
#     """
#     properties = {}
#     for property_config in entity_config['properties']:
#         sources = get_sources_from_property(property_config, node)
#         if 'format' in property_config and has_all_formatting_fields(property_config['format'], sources):
#             properties[property_config['name']] = property_config['format'].format_map(sources)
#     return properties
#     # hay algún source que no se ha generado
#     # TODO validar que se tienen todos los parámetros necesarios
#     # Posible problema aquí: ¿qué pasa si hay dos propiedades con el mismo nombre de distintas ontologías?
#
#
def get_sources_from_property(current_property, node):
    # Declaramos un dict. para que podamos guardar los valores de los sources
    sources = {}
    for source in current_property['sources']:
        source_node = xmltree.get_first_node_by_code(node, source['code'])
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


def make_validation_error(message):
    return make_response(jsonify({'error': message}), 422)  # 422 = Unprocessable Entity
    # TODO juntar esta función y make_error_response


def make_error_response(message):
    return make_response(jsonify({'error': message}), 500)  # 422 = Unprocessable Entity


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
