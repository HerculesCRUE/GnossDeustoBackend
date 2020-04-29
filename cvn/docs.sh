#!/bin/sh
cd docs/api/flask
pipenv install
pipenv run python3 -m openapi_server