#
# main.py
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
import secrets  # temporal, para generar las ID de ciertas entidades
import urllib.parse
from flask import Flask, request, make_response, jsonify
import re
import requests
import toml

app = Flask(__name__)
app.config['MAX_CONTENT_LENGTH'] = 16 * 1024 * 1024  # 16MB máx.
# app.debug = True

# Formatos
ALLOWED_FORMATS = ["xml", "n3", "turtle", "nt", "pretty-xml", "trix", "trig", "nquads"]

# Caché para los nombres de códigos
code_name = {}


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
    ontologies = {}
    ontology_primary = None # TODO comprobar que se ha definido una ontología primaria

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

    person = URIRef(generate_uri(config['instance']['class'], params['orcid']))
    g.add((person, RDF.type, ontology_primary.term(config['instance']['class'])))

    info_node = get_node_by_code(root, config['code'])

    for property in config['properties']:
        # Declaramos un dict. para que podamos guardar los valores de los sources
        sources = {}
        for source in property['sources']:
            source_node = get_node_by_code(info_node, source['code'])
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

        # Formateamos
        g.add((person, ontologies[property['ontology']].term(property['name']),
               Literal(property['format'].format(**sources))))


        # Sources procesadas, formateamos texto

    return make_response(g.serialize(format=params['format']), 200)  # TODO Quitar, DEBUG

    # 2. Generar entidades
    # 3. Enlazar

    # ----- FIN PROCESO CVN

    # Vamos a ir recorriendo el árbol XML del documento CVN e iremos sacando, cuando sea necesario, información, para
    # luego insertarla en tripletas generadas al vuelo con rdflib
    for child in root:
        code = node_get_code(child)

        if app.debug:  # Debug
            print("> " + code + ": " + code_get_name(code, "spa"))

        # Publicaciones, documentos científicos y técnicos (ResearchObject)
        if code == "060.010.010.000":
            title, journal, volume, page_start, page_end, doi, issn = None, None, None, None, None, None, None  # TODO sustituir por diccionarios

            for subchild in child:
                subcode = node_get_code(subchild)

                if app.debug:  # Debug
                    print(">> " + subcode + ": " + code_get_name(subcode))

                # TODO hacer switch

                # Título
                if node_get_code(subchild) == "060.010.010.030":
                    title = subchild.find("{http://codes.cvn.fecyt.es/beans}Value").text
                    if app.debug:  # Debug
                        print(">>> TÍTULO: " + str(title))

                # Journal
                if node_get_code(subchild) == "060.010.010.210":
                    try:
                        journal = subchild.find("{http://codes.cvn.fecyt.es/beans}Value").text
                    except AttributeError as e:
                        if app.debug:  # Debug
                            print(">>> JOURNAL: error")
                    if app.debug:  # Debug
                        print(">>> JOURNAL: " + str(journal))

                # Volume
                if node_get_code(subchild) == "060.010.010.080":
                    try:
                        volume = subchild.find("{http://codes.cvn.fecyt.es/beans}Volume").text
                    except AttributeError as e:
                        if app.debug:  # Debug
                            print(">>> VOLUMEN: error")
                    if app.debug:  # Debug
                        print(">>> VOLUMEN: " + str(volume))

                # Páginas inicio y fin
                if node_get_code(subchild) == "060.010.010.090":
                    try:
                        page_start = subchild.find("{http://codes.cvn.fecyt.es/beans}InitialPage").text
                        page_end = subchild.find("{http://codes.cvn.fecyt.es/beans}FinalPage").text
                    except AttributeError as e:
                        if app.debug:  # Debug
                            print(">>> PÁGINAS: error")

                    if app.debug:  # Debug
                        print(">>> PÁGINAS: " + str(page_start) + "-" + str(page_end))

                # ISSN Journal
                if node_get_code(subchild) == "060.010.010.160":
                    try:
                        issn = subchild.find("{http://codes.cvn.fecyt.es/beans}Value").text
                    except AttributeError as e:
                        if app.debug:  # Debug
                            print(">>> ISSN JOURNAL: error")
                    if app.debug:  # Debug
                        print(">>> ISSN JOURNAL: " + str(issn))

                # DOI
                if node_get_code(subchild) == "060.010.010.400":
                    try:
                        if subchild.find(
                                "{http://codes.cvn.fecyt.es/beans}Type").text == "040":  # SOLO DOI, número mágico :S
                            doi = subchild.find("{http://codes.cvn.fecyt.es/beans}Value").text
                    except AttributeError as e:
                        if app.debug:  # Debug
                            print(">>> DOI: error")
                    if app.debug:  # Debug
                        print(">>> DOI: " + str(doi))

            if doi is None:

                publication = URIRef(generate_uri('Article', secrets.token_hex(6)))
            else:
                publication = URIRef(generate_uri('Article', urllib.parse.quote_plus(str(doi))))
            g.add((publication, RDF.type, bibo.AcademicArticle))
            g.add((publication, roh.title, Literal(str(title))))
            # Journal object
            if journal is not None:
                # Generar a URI
                if issn is None:  # Si no hemos detectado el ISSN, generamos un número al azar
                    journal_object = URIRef(generate_uri("Journal", urllib.parse.quote_plus(str(secrets.token_hex(6)))))
                else:

                    journal_object = URIRef(generate_uri("Journal", urllib.parse.quote_plus(str(issn))))
                g.add((journal_object, RDF.type, bibo.Journal))
                g.add((journal_object, roh.title, Literal(str(journal))))
                g.add((journal_object, vivo.publicationVenueFor, publication))
                if issn is not None:
                    g.add((journal_object, bibo.issn, Literal(str(issn))))
            if volume is not None:
                g.add((publication, bibo.volume, Literal(str(volume))))
            if (page_start is not None) and (page_end is not None):
                g.add((publication, bibo.start, Literal(str(page_start))))
                g.add((publication, bibo.end, Literal(str(page_end))))
            if doi is not None:
                g.add((publication, bibo.doi, Literal(str(doi))))
            # roh:correspondingAuthor
            g.add((publication, roh.correspondingAuthor, person))

            # Crear rol TODO personalizar el nombre del rol según el tipo
            # role_uri = URIRef("http://purl.obolibrary.org/obo/BFO_0000023")
            # TODO roles — lo tengo que consultar con Mikel y Diego

    # Serializar y guardar en un archivo con el formato que queramos
    return make_response(g.serialize(format=params['format']), 200)


def get_node_by_code(tree, code):
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

def get_nodes_by_code(tree, code):
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
    return None

def node_get_code(node):
    """
    Obtener el código CVN de un nodo XML.
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
    """
    if code in code_name:
        return code_name[code]
    tree = ET.parse('mappings/cvn/1.4.2_sp1/XSD/SpecificationManual.xml')  # hardcoded for now
    result = tree.getroot().find("./*Item[@code='" + code + "']/Name/NameDetail[@lang='" + lang + "']/Name").text
    code_name[code] = result
    return result


def make_validation_error(message):
    return make_response(jsonify({'error': message}), 422)  # 422 = Unprocessable Entity


def make_error_response(message):
    return make_response(jsonify({'error': message}), 500)  # 422 = Unprocessable Entity


# Caché de URIs generadas
# TODO mover a un servicio externo
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
    })

    # Si falla:
    if api_response.status_code != 200:
        return "http://data.um.es/class/" + resource_class + "/" + identifier

    # TODO comprobar que lo que devuelve es de hecho una URL bien formateada

    # Guardamos en caché si sale bien
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
