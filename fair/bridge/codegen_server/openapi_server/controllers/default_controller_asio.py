import connexion
import six

from openapi_server import util
from flask import Flask, jsonify, abort, request, make_response
import requests
import re


def collections_get():  # noqa: E501
    '''Obtiene la lista de colecciones disponibles para realizar tests'''
    api_url = 'https://ejp-evaluator.appspot.com/FAIR_Evaluator/collections'
    result = requests.get(api_url, headers={'Accept': 'application/json'}) # TODO cachear

    # Si nos
    if(result.status_code != requests.codes.ok):
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}), requests.codes.unprocessable)

    return jsonify(result.json())


def collections_id_evaluate_post(id_collection, resource, orcid, title):  # noqa: E501
    '''Crea un nuevo test'''

    # Comprobamos que los parámetros resource, orcid y title estén presentes y sean válidos
    if(request.args.get('resource') is None):
        return make_response(jsonify({'error': 'The resource field is required.'}), requests.codes.unprocessable)

    if(request.args.get('orcid') is None):
        return make_response(jsonify({'error': 'The orcid field is required.'}), requests.codes.unprocessable)

    # if not re.match(r" 0000-000(1-[5-9]|2-[0-9]|3-[0-4])\d{3}-\d{3}[\dX]", request.args.get('orcid')):
    #     return make_response(jsonify({'error': 'The ORCID iD provided in the orcid field is invalid.'}), requests.codes.unprocessable)
   
    if(request.args.get('title') is None):
        return make_response(jsonify({'error': 'The resource field is required.'}), requests.codes.unprocessable)

    post_data = {
        'resource': request.args.get('resource'),
        'executor': request.args.get('orcid'),
        'title': request.args.get('title')
    }

    api_url = 'https://ejp-evaluator.appspot.com/FAIR_Evaluator/collections/' + str(id_collection) + '/evaluate'
    result = requests.post(api_url, post_data, headers={'Accept': 'application/json'})

    print(result.text)

    if(result.status_code == requests.codes.not_found):
        return make_response(jsonify({'error': 'Collection not found.'}), requests.codes.not_found)
    elif(result.status_code != requests.codes.ok):
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}), requests.codes.server_error)

    return jsonify(result.json())


def evaluations_get():  # noqa: E501
    '''Devuelve una lista con todas las evaluaciones que se hayan ejecutado con el orcid_id indicado más abajo'''

    api_url = 'https://ejp-evaluator.appspot.com/FAIR_Evaluator/evaluations'
    result = requests.get(api_url, headers={'Accept': 'application/json'}) # TODO cachear

    # Si nos
    if(result.status_code != requests.codes.ok):
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}), requests.codes.unprocessable)

    all_evaluations = result.json()
    evaluations = []

    # Ahora mismo filtramos los resultados para que solo las pruebas hechas desde este ORCID iD sean mostradas
    # TODO dejar de hardcodear
    orcid_id = 'https://orcid.org/0000-0001-8055-6823'

    for evaluation in all_evaluations:
        print(str(evaluation))
        if(evaluation['creator'] == orcid_id):
            evaluations.append(evaluation)

    return jsonify(evaluations)


def evaluations_id_result_get(id_evaluation):  # noqa: E501
    '''Devuelve los resultados por métrica de una evaluación específica'''

    api_url = 'https://ejp-evaluator.appspot.com/FAIR_Evaluator/evaluations/' + str(id_evaluation) + '/result'
    result = requests.get(api_url, headers={'Accept': 'application/json'}) # TODO cachear

    if(result.status_code != requests.codes.ok):
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}), requests.codes.unprocessable)

    return jsonify(result.json())
