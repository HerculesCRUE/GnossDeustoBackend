# coding: utf-8

import sys
from setuptools import setup, find_packages

NAME = "openapi_server"
VERSION = "1.0.0"

# To install the library, run the following
#
# python setup.py install
#
# prerequisite: setuptools
# http://pypi.python.org/pypi/setuptools

REQUIRES = ["connexion>=2.0.2", "swagger-ui-bundle>=0.0.2", "python_dateutil>=2.6.0"]

setup(
    name=NAME,
    version=VERSION,
    description="Servicio conversión CVN-ROH",
    author_email="",
    url="",
    keywords=["OpenAPI", "Servicio conversión CVN-ROH"],
    install_requires=REQUIRES,
    packages=find_packages(),
    package_data={"": ["openapi/openapi.yaml"]},
    include_package_data=True,
    entry_points={"console_scripts": ["openapi_server=openapi_server.__main__:main"]},
    long_description="""\
    Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH.
    """,
)
