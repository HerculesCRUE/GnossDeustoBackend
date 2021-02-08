#!/bin/sh
 
echo "Ejecutando en modo debug..."
pipenv run python3 -m cvn.webserver -p 5000 --debug
