#
# webserver.py
# Parte de HERCULES-ASIO
# ----------------------
# Crea una API HTTP que permite la conversión de XML CVN a tripletas ROH.
#
# ** EN DESARROLLO **
#
import argparse
from functools import partial

import rdflib
from werkzeug.local import LocalProxy
from werkzeug.local import LocalStack
import xml.etree.ElementTree as ET
from rdflib import Graph, Literal, URIRef
from rdflib.namespace import RDF, NamespaceManager
from flask import Flask, request, make_response, jsonify
import re
import toml
import cvn.config.entity as config_entity
#from cvn.config import entity as config_entity
from cvn.config.ontology import OntologyConfig, Ontology, DataType
import cvn.utils.xmltree as xmltree
import logging
import cvn.config.entitycache as cvn_entity_cache
#import requests

app = Flask(__name__)
app.config['MAX_CONTENT_LENGTH'] = 16 * 1024 * 1024  # 16MB máx.

debug = False

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

    # Guardar algunos parámetros en el diccionario para luego generar bien el resultado
    params = {'format': "xml"}

    # Si se especifica el formato, solo permitir ciertos valores
    if request.args.get('format') is not None:
        if request.args.get('format') not in ALLOWED_FORMATS:
            return make_validation_error("The format field has an unsupported format.")
        else:
            params={'format': request.args.get('format')}
    try:
        #root=ET.ElementTree().parse(source='C:/Users/mpuer/Downloads/CVN/cv7.xml')
        root = ET.fromstring(input_xml)
    except Exception as e:
        return make_error_response("Error while parsing the XML.")
    # Lipiamos la cache
    cvn_entity_cache.get_current_entity_cache().__init__()
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
                                               python_type=data_type_config['python_type'], default=default,
                                               force=force))
    # 2. Generar entidades

    # Cargar config
    with open("mappings/cvn/1.4.2_sp1/cvn-to-roh/entities.toml") as f:
        # TODO deshardcodificar
        try:
            a=f.read()
            entities_config = toml.loads(a)
        except Exception as e:
            print(e)
            return make_error_response("Error while parsing the .toml.")

    # Generar instancias Entity
    entities = []
    primary_entity = None
    for entity_config in entities_config['entities']:
        generated_entity = config_entity.init_entity_from_serialized_toml(entity_config)
        if generated_entity.primary:  # La entidad primaria (la de la persona del CVN la guardamos por separado)
            primary_entity = generated_entity
        else:
            entities.append(generated_entity)
    if primary_entity is None:
        raise ValueError("Config error: there is no primary entity")

    # Procesar entidad primaria
    primary_entity.generate_and_add_to_ontology(ontology_config, root)

    for entity in entities:
        # Para cada tipo de entidad buscamos en el árbol las que tengan el código
        entity.generate_and_add_to_ontology(ontology_config, root)
    numdeleted=1
    while numdeleted > 0:
        numdeleted = 0
        # Query para obtener las entidades que NO tengan rdftype.
        numdeleted += remove_entities_without_rdftype(g)
        # Query para obtener las entidades que únicamente tengan rdftype.
        numdeleted += remove_empty_entities(g)
    g=change(g)
    g=eliminar_localizacion(g)
    return make_response(g.serialize(format=params['format']), 200)


def remove_entities_without_rdftype(grafo):
    dicentities={}
    totalentities = []
    entitieswithrdftype = []
    numdeleted=0
    for triple in grafo:
        sujeto = str(triple[0])
        if sujeto not in totalentities:
            totalentities.append(sujeto)
            dicentities[sujeto] = triple[0]

    resultadoQuery = grafo.query("SELECT DISTINCT ?entity WHERE {?entity a ?rdftype}")
    for fila in resultadoQuery:
        sujeto = str(fila[0])
        if sujeto not in entitieswithrdftype:
            entitieswithrdftype.append(sujeto)
    for entity in totalentities:
        if entity not in entitieswithrdftype:
            numdeleted = numdeleted + 1
            grafo.remove((dicentities[entity], None, None))
            grafo.remove((None, None, dicentities[entity]))
    return numdeleted


def remove_empty_entities(grafo):
    resultadoQuery = grafo.query("SELECT ?entity WHERE {?entity ?p ?o.  } GROUP BY ?entity HAVING (COUNT(*) = 1)")
    resiult=len(resultadoQuery)
    for fila in resultadoQuery:
        if fila[0][:24] != "http://sws.geonames.org/": # Necesitamos recoger esta estepcion porque no estan
            # identificados como URIS, son un caso muy especifico y la uri viene dada pero no se define el tipo, es un caso especial.
            # En este caso no queremos eliminar el dato.
            grafo.remove((fila[0], None, None))
            grafo.remove((None, None, fila[0]))
        else:
            resiult = resiult -1
    return resiult

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

# lo unico que hace es cambiar los rdf:member por rdf:_n, ahora como el grafo esta terminado podemos saber le valor de
# n por lo que es mas facil.
def change(grafo):
    for triple in grafo:
        prificado=str(triple[1])
        if prificado == "http://www.w3.org/2000/01/rdf-schema#member":
            sujeto = str(triple[0])
            string="SELECT DISTINCT ?objeto WHERE { <"+  sujeto +">  <"+ prificado +"> ?objeto }"
            resultadoQuery = grafo.query(string)
            i=1
            for fila in enumerate(resultadoQuery):
                objecto = fila[1][0]
                grafo.remove(( rdflib.term.URIRef(sujeto),rdflib.term.URIRef( prificado), rdflib.term.URIRef(objecto)))
                grafo.add((  rdflib.term.URIRef(sujeto), rdflib.term.URIRef('http://www.w3.org/1999/02/22-rdf-syntax-ns#_'+str(i)), objecto))
                i=i+1
    return grafo

# esta funcion nos permite que un recurso no tenga mas de una localizacion. Por ello se queda con la localizacion mas larga,
# Lo que ocurria es que un mismo recurso tenia varias localizaciones: JAEN, JAÈN, JAEN ESPAÑA. Por tanto esta funcion se queda con una unicamente.
def eliminar_localizacion(grafo):
    for triple in grafo:
        prificado = str(triple[1])
        if prificado == "http://w3id.org/roh/mirror/vcard#locality":
            sujeto = str(triple[0])
            string = "SELECT DISTINCT ?objeto WHERE { <" + sujeto + ">  <" + prificado + "> ?objeto }"
            resultadoQuery = grafo.query(string)
            filas=len(resultadoQuery)
            if filas>1:
                longuitud_max=0
                localizacion=0
                for f in resultadoQuery:
                    objecto = f[0]
                    if len(objecto)>longuitud_max:
                        localizacion=objecto

                for f in resultadoQuery:
                    objecto = f[0]
                    if objecto != localizacion:
                        grafo.remove(
                            (rdflib.term.URIRef(sujeto), rdflib.term.URIRef(prificado), rdflib.term.Literal(f[0], datatype=rdflib.term.URIRef('http://www.w3.org/2001/XMLSchema#string'))))

    return grafo
def make_validation_error(message):
    return make_response(jsonify({'error': message}), 422)  # 422 = Unprocessable Entity
    # TODO juntar esta función y make_error_response


def make_error_response(message):
    return make_response(jsonify({'error': message}), 422)  # Parsin error


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Servidor HTTP que ofrece una API para convertir XML CVN a tripletas "
                                                 "ROH")
    parser.add_argument("-p", "--port", type=int, default=5000, choices=range(0, 65536),
                        help="El puerto en el que se ejecutará el servidor HTTP (por defecto 5000)", metavar="")
    parser.add_argument("--host", default="127.0.0.1",
                        help="El host donde se bindeará el servidor HTTP (por defecto 127.0.0.1)")
    parser.add_argument("--debug", action="store_true", help="DEBUG: activar modo debug (aumenta tiempo de ejecución)")
    args = parser.parse_args()

    debug = args.debug
    if debug:
        logging.basicConfig(level=logging.DEBUG)
    app.run(debug=args.debug, port=args.port, host=args.host)

# 19/03/2020 Iñigo — lo que he podido en este poco tiempo, lo siento por el código desastroso :^(
# 20/03/2020 Iñigo — he cambiado el script para que sea una API HTTP en vez de una utilidad de consola. Lo he hecho
#                    rápido, por lo que no es muy configurable por ahora.
# 07/06/2021 Maite - Se han arreglado ciertas cosas y actualizao los ficheros toml para que sean compatibles con la version
#                    de ROH en el momento actual.
