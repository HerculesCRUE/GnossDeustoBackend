#!/usr/bin/env python3

from flask import Flask, jsonify, abort, request, make_response
from flask_sqlalchemy import SQLAlchemy
import requests
import re
import argparse

app = Flask(__name__)

app.config.from_pyfile('config.py')

server_url = None


@app.route('/v1/collections/', methods=['GET'])
def v1_collections():
    """Obtiene la lista de colecciones disponibles para realizar tests"""
    api_url = server_url + '/collections'
    result = requests.get(api_url, headers={'Accept': 'application/json'})  # TODO cachear

    # Si nos
    if result.status_code != requests.codes.ok:
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}),
                             requests.codes.unprocessable)

    return jsonify(result.json())


@app.route('/v1/collections/<collection_id>/evaluate', methods=['POST'])
def v1_new_metric(collection_id):
    """Crea un nuevo test"""

    # Comprobamos que los parámetros resource, orcid y title estén presentes y sean válidos
    if request.args.get('resource') is None:
        return make_response(jsonify({'error': 'The resource field is required.'}), requests.codes.unprocessable)

    if request.args.get('orcid') is None:
        return make_response(jsonify({'error': 'The orcid field is required.'}), requests.codes.unprocessable)

    if request.args.get('title') is None:
        return make_response(jsonify({'error': 'The resource field is required.'}), requests.codes.unprocessable)

    post_data = {
        'resource': request.args.get('resource'),
        'executor': request.args.get('orcid'),
        'title': request.args.get('title')
    }

    api_url = server_url + '/collections/' + collection_id + '/evaluate'
    result = requests.post(api_url, post_data, headers={'Accept': 'application/json'})

    print(result.text)

    if result.status_code == requests.codes.not_found:
        return make_response(jsonify({'error': 'Collection not found.'}), requests.codes.not_found)
    elif result.status_code != requests.codes.ok:
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}),
                             requests.codes.server_error)

    return jsonify(result.json())


@app.route('/v1/evaluations', methods=['GET'])
def v1_evaluations():
    """Devuelve una lista con todas las evaluaciones que se hayan ejecutado con el orcid_id indicado más abajo"""

    api_url = server_url + '/evaluations'
    result = requests.get(api_url, headers={'Accept': 'application/json'})  # TODO cachear

    # Si nos
    if result.status_code != requests.codes.ok:
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}),
                             requests.codes.unprocessable)

    all_evaluations = result.json()
    evaluations = []

    # Ahora mismo filtramos los resultados para que solo las pruebas hechas desde este ORCID iD sean mostradas
    # TODO dejar de hardcodear
    orcid_id = 'https://orcid.org/0000-0001-8055-6823'

    for evaluation in all_evaluations:
        print(str(evaluation))
        if evaluation['creator'] == orcid_id:
            evaluations.append(evaluation)

    return jsonify(evaluations)


@app.route('/v1/evaluations/<evaluation_id>/result', methods=['GET'])
def v1_evaluation_result(evaluation_id):
    """Devuelve los resultados por métrica de una evaluación específica"""

    api_url = server_url + '/evaluations/' + evaluation_id + '/result'
    result = requests.get(api_url, headers={'Accept': 'application/json'})  # TODO cachear

    if result.status_code != requests.codes.ok:
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}),
                             requests.codes.unprocessable)

    return jsonify(result.json())


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description="Servidor HTTP que ofrece una API simple para interactuar con" +
                                                 " un servicio de métricas FAIR")
    parser.add_argument("-p", "--port", type=int, default=5200, choices=range(0, 65536),
                        help="El puerto en el que se ejecutará el servidor HTTP (por defecto 5200)", metavar="")
    parser.add_argument("--host", default="127.0.0.1",
                        help="El host donde se bindeará el servidor HTTP (por defecto 127.0.0.1)")
    parser.add_argument("--debug", action="store_true", help="DEBUG: activar modo debug (aumenta tiempo de ejecución)")
    parser.add_argument("--server-url", type=str, default="https://fair-evaluator.semanticscience.org/FAIR_Evaluator",
                        help="URL del backend de FAIR (no incluir / final)")
    args = parser.parse_args()
    server_url = args.server_url
    app.run(debug=args.debug, port=args.port, host=args.host)
