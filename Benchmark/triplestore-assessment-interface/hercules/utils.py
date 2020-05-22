# -*- coding: utf-8 -*-
"""Helper utilities and decorators."""
from SPARQLWrapper import SPARQLWrapper, JSON
from flask import flash

from .settings import SPARQL_SETTINGS


def flash_errors(form, category="warning"):
    """Flash all errors for a form."""
    for field, errors in form.errors.items():
        for error in errors:
            flash("{0} - {1}".format(getattr(form, field).label.text, error), category)


def sparql_query(query):
    """
    Run a sparql query and get JSON values back

    Arguments:
        query {string} -- the query to run

    Returns:
        json -- returns json results of query
    """
    endpoint = SPARQL_SETTINGS["data-sources"][0]
    if endpoint["protocol"] == "sparql":
        sparql = SPARQLWrapper(endpoint["url"])
        sparql.setReturnFormat(JSON)
        sparql.setQuery(query)
        return sparql.query().convert()
    return None

