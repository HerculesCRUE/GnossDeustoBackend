# -*- coding: utf-8 -*-
"""The app module, containing the app factory function."""
import logging
import sys

from flask import Flask, render_template
from flask_bootstrap import Bootstrap

from hercules import api, public


def create_app(config_object="hercules.settings"):
    """Create application factory, as explained here: http://flask.pocoo.org/docs/patterns/appfactories/.

    :param config_object: The configuration object to use.
    """
    app = Flask(__name__.split(".")[0])
    app.config.from_object(config_object)
    register_blueprints(app)
    register_extensions(app)
    configure_logger(app)
    return app

def configure_logger(app):
    """Configure loggers."""
    handler = logging.StreamHandler(sys.stdout)
    if not app.logger.handlers:
        app.logger.addHandler(handler)

def register_blueprints(app):
    """Register Flask blueprints."""
    url_prefix=app.config['APP_SETTINGS']['url-prefix']
    app.register_blueprint(public.views.blueprint, url_prefix='/')
    app.register_blueprint(api.api.blueprint, url_prefix='/')
    return None

def register_extensions(app):
    """Register Flask extensions."""
    Bootstrap(app)
    return None

