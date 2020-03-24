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
from rdflib import Graph, Namespace, Literal, URIRef, BNode
from rdflib.namespace import RDF, FOAF, NamespaceManager
import secrets  # temporal, para generar las ID de ciertas entidades
import urllib.parse
from flask import Flask, request, make_response, jsonify
import re

app = Flask(__name__)
app.config['MAX_CONTENT_LENGTH'] = 16 * 1024 * 1024  # 16MB máx.
# app.debug = True

# Formatos
ALLOWED_FORMATS = ["xml", "n3", "turtle", "nt", "pretty-xml", "trix", "trig", "nquads"]

# Caché para los nombres de códigos
code_name = {}


@app.route('/v1/convert', methods=['POST'])
def v1_convert():

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


    # Crear el grafo, lo iremos rellenando más abajo
    # Le inyectamos el término corto de la ontología roh

    namespace_manager = NamespaceManager(Graph())
    roh = Namespace("https://purl.org/roh/")
    namespace_manager.bind('roh', roh, override=False)
    bibo = Namespace("http://purl.org/ontology/bibo/")
    namespace_manager.bind('bibo', bibo, override=False)
    vivo = Namespace("http://vivoweb.org/ontology/core#")
    namespace_manager.bind('vivo', vivo, override=False)
    g = Graph()
    g.namespace_manager = namespace_manager

    person = URIRef("https://purl.org/roh/researcher/" + str(params['orcid']))

    # Vamos a ir recorriendo el árbol XML del documento CVN e iremos sacando, cuando sea necesario, información, para
    # luego insertarla en tripletas generadas al vuelo con rdflib
    for child in root:
        code = node_get_code(child)

        if app.debug:  # Debug
            print("> " + code + ": " + code_get_name(code, "spa"))

        # TODO hacer switch

        # Identificación CVN
        if code == "000.010.000.000":
            first_name, first_family_name, second_family_name, email = None, None, None, None
            for subchild in child:
                subcode = node_get_code(subchild)

                if app.debug:  # Debug
                    print(">> " + subcode + ": " + code_get_name(subcode))

                # TODO hacer switch

                # Apellidos
                if node_get_code(subchild) == "000.010.000.010":
                    first_family_name = subchild.find("{http://codes.cvn.fecyt.es/beans}FirstFamilyName").text
                    second_family_name = subchild.find("{http://codes.cvn.fecyt.es/beans}SecondFamilyName").text

                # Nombre
                if node_get_code(subchild) == "000.010.000.020":
                    first_name = subchild.find("{http://codes.cvn.fecyt.es/beans}Value").text

                # Email
                if node_get_code(subchild) == "000.010.000.230":
                    email = subchild.find("{http://codes.cvn.fecyt.es/beans}Value").text

            # Person
            g.add((person, RDF.type, roh.Researcher))
            # Person > name
            full_name = first_name + " " + first_family_name + " " + second_family_name
            g.add((person, FOAF.name, Literal(full_name)))
            # Person > email
            g.add((person, FOAF.mbox, Literal(email)))

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
                publication = URIRef("https://purl.org/roh/article/" + str(
                    secrets.token_hex(6)))  # TODO integrar backend generador URIs desarrollado por GNOSS
            else:
                publication = URIRef("https://purl.org/roh/article/" + urllib.parse.quote_plus(str(doi)))
            g.add((publication, RDF.type, bibo.AcademicArticle))
            g.add((publication, roh.title, Literal(str(title))))
            # Journal object
            if journal is not None:
                # Generar a URI
                if issn is None:  # Si no hemos detectado el ISSN, generamos un número al azar
                    journal_object = URIRef(
                        "https://purl.org/roh/journal/" + urllib.parse.quote_plus(str(secrets.token_hex(6))))
                else:
                    journal_object = URIRef("https://purl.org/roh/journal/" + urllib.parse.quote_plus(str(issn)))
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


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Servidor HTTP que ofrece una API para convertir XML CVN a tripletas "
                                                 "ROH")
    parser.add_argument("-p", "--port", type=int, default=5000, choices=range(0,65536),
                        help="El puerto en el que se ejecutará el servidor HTTP (por defecto 5000)", metavar="")
    parser.add_argument("--debug", action="store_true", help="DEBUG: activar modo debug (aumenta tiempo de ejecución)")
    args = parser.parse_args()

    app.run(debug=args.debug, port=args.port)

# 19/03/2020 Iñigo — lo que he podido en este poco tiempo, lo siento por el código desastroso :^(
# 20/03/2020 Iñigo — he cambiado el script para que sea una API HTTP en vez de una utilidad de consola. Lo he hecho
#                    rápido, por lo que no es muy configurable por ahora.
