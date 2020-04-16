#!/usr/bin/env python3

from flask import Flask, jsonify, abort, request, make_response
from flask_sqlalchemy import SQLAlchemy
import requests
import re

app = Flask(__name__)

app.config.from_pyfile('config.py')

# db = SQLAlchemy(app)

# class Test(db.Model):
#     id = db.Column(db.Integer, primary_key=True)
#     test_id = db.Column(db.Integer, unique=True, nullable=False)

@app.route('/v1/collections/', methods=['GET'])
def v1_collections():
    '''Obtiene la lista de colecciones disponibles para realizar tests'''
    api_url = 'https://ejp-evaluator.appspot.com/FAIR_Evaluator/collections'
    result = requests.get(api_url, headers={'Accept': 'application/json'}) # TODO cachear

    # Si nos
    if(result.status_code != requests.codes.ok):
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}), requests.codes.unprocessable)

    return jsonify(result.json())

@app.route('/v1/collections/<id>/evaluate', methods = ['POST'])
def v1_new_metric(id):
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

    api_url = 'https://ejp-evaluator.appspot.com/FAIR_Evaluator/collections/' + id + '/evaluate'
    result = requests.post(api_url, post_data, headers={'Accept': 'application/json'})

    print(result.text)

    if(result.status_code == requests.codes.not_found):
        return make_response(jsonify({'error': 'Collection not found.'}), requests.codes.not_found)
    elif(result.status_code != requests.codes.ok):
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}), requests.codes.server_error)

    return jsonify(result.json())

@app.route('/v1/evaluations', methods=['GET'])
def v1_evaluations():
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

@app.route('/v1/evaluations/<id>/result', methods=['GET'])
def v1_evaluation_result(id):
    '''Devuelve los resultados por métrica de una evaluación específica'''

    api_url = 'https://ejp-evaluator.appspot.com/FAIR_Evaluator/evaluations/' + id + '/result'
    result = requests.get(api_url, headers={'Accept': 'application/json'}) # TODO cachear

    if(result.status_code != requests.codes.ok):
        return make_response(jsonify({'error': 'Something went wrong when contacting the public FAIR Metrics API.'}), requests.codes.unprocessable)

    return jsonify(result.json())

if __name__ == '__main__':
    app.run()
