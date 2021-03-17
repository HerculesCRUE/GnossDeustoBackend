# -*- coding: utf-8 -*-
"""Application configuration.

Most configuration is set via environment variables.

For local development, use a .env file to set
environment variables.
"""
import os
from environs import Env

env = Env()
env.read_env()

ENV = env.str("FLASK_ENV", default="development") # or "production"
DEBUG = ENV == "development"

APP_SETTINGS = {
	"url-prefix" : "/"
}

SPARQL_SETTINGS = {
    "data-sources" : [{
    	# Example local endpoint
        "protocol" : "sparql",
             "url" : "http://155.54.239.221:8890/sparql"
    }]
}
