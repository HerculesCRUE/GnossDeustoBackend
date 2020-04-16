# coding: utf-8

from __future__ import absolute_import
import unittest

from flask import json
from six import BytesIO

from openapi_server.test import BaseTestCase


class TestDefaultController(BaseTestCase):
    """DefaultController integration test stubs"""

    def test_collections_get(self):
        """Test case for collections_get

        Obtener una lista de las colecciones
        """
        headers = { 
            'Accept': 'application/json',
        }
        response = self.client.open(
            '/v1/collections',
            method='GET',
            headers=headers)
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_collections_id_evaluate_post(self):
        """Test case for collections_id_evaluate_post

        Inicia un nuevo test a partir de una colección
        """
        query_string = [('resource', 'resource_example'),
                        ('executor', 'executor_example'),
                        ('title', 'title_example')]
        headers = { 
            'Accept': 'application/json',
        }
        response = self.client.open(
            '/v1/collections/{id}/evaluate'.format(id=2),
            method='POST',
            headers=headers,
            query_string=query_string)
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_evaluations_get(self):
        """Test case for evaluations_get

        Obtiene una lista de evaluaciones relevantes
        """
        headers = { 
        }
        response = self.client.open(
            '/v1/evaluations',
            method='GET',
            headers=headers)
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_evaluations_id_result_get(self):
        """Test case for evaluations_id_result_get

        Obtiene detalles del resultado de una evaluación
        """
        headers = { 
        }
        response = self.client.open(
            '/v1/evaluations/{id}/result'.format(id=2),
            method='GET',
            headers=headers)
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))


if __name__ == '__main__':
    unittest.main()
