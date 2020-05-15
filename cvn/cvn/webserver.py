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
from cvn.config.ontology import OntologyConfig, Ontology, DataType
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

    # Tipos de datos
    with open("mappings/cvn/1.4.2_sp1/cvn-to-roh/data-types.toml") as f:
        config_data_types = toml.loads(f.read())

    # TODO mover a su clase
    for data_type_config in config_data_types['datatypes']:
        default = False
        if 'default' in data_type_config:
            default = data_type_config['default']
        force = False
        if 'force' in data_type_config:
            force = data_type_config['force']

        ontology_config.add_data_type(DataType(ontology=data_type_config['ontology'], name=data_type_config['name'],
                                      python_type=data_type_config['python_type'], default=default, force=force))

    # 2. Generar entidades

    # Cargar config
    with open("mappings/cvn/1.4.2_sp1/cvn-to-roh/entities.toml") as f:  # TODO deshardcodificar
        entities_config = toml.loads(f.read())

    # Generar instancias Entity
    entities = []
    primary_entity = None
    for entity_config in entities_config['entities']:
        generated_entity = config_entity.init_entity_from_serialized_toml(entity_config)
        if generated_entity.primary:  # La entidad primnaria (la de la persona del CVN la guardamos por separado)
            primary_entity = generated_entity
        else:
            entities.append(generated_entity)
    if primary_entity is None:
        raise ValueError("Config error: there is no primary entity")

    # Procesar entidad primaria
    primary_node = xmltree.get_first_node_by_code(root, primary_entity.code)
    if primary_node is None:
        raise ValueError("This CVN doesn't have a primary node with personal data")
    primary_entity.get_property_values_from_node(primary_node)
    primary_entity.add_entity_to_ontology(ontology_config)
    ontology_config.cvn_person = primary_entity.get_uri()


    for entity in entities:
        # Para cada tipo de entidad buscamos en el árbol las que tengan el código
        for entity_result_node in xmltree.get_all_nodes_by_code(root, entity.code):
            entity.get_property_values_from_node(entity_result_node)
            entity.add_entity_to_ontology(ontology_config)
            entity.clear_values()

    return make_response(g.serialize(format=params['format']), 200)  # TODO Quitar, DEBUG


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
    parser.add_argument("--host", default="127.0.0.1",
                        help="El host donde se bindeará el servidor HTTP (por defecto 127.0.0.1)")
    parser.add_argument("--debug", action="store_true", help="DEBUG: activar modo debug (aumenta tiempo de ejecución)")
    args = parser.parse_args()

    app.run(debug=args.debug, port=args.port, host=args.host)

# 19/03/2020 Iñigo — lo que he podido en este poco tiempo, lo siento por el código desastroso :^(
# 20/03/2020 Iñigo — he cambiado el script para que sea una API HTTP en vez de una utilidad de consola. Lo he hecho
#                    rápido, por lo que no es muy configurable por ahora.
