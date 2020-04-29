# coding: utf-8

from __future__ import absolute_import
import unittest

from flask import json
from six import BytesIO

from openapi_server.test import BaseTestCase


class TestDefaultController(BaseTestCase):
    """DefaultController integration test stubs"""

    @unittest.skip("application/x-www-form-urlencoded not supported by Connexion")
    def test_convert_post(self):
        """Test case for convert_post

        Convierte CVN XML a tripletas ROH
        """
        query_string = [('orcid', 'orcid_example'),
                        ('format', 'xml')]
        headers = { 
            'Content-Type': 'application/x-www-form-urlencoded',
        }
        response = self.client.open(
            '/v1/convert',
            method='POST',
            headers=headers,
            content_type='application/x-www-form-urlencoded',
            query_string=query_string)
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))


if __name__ == '__main__':
    unittest.main()
