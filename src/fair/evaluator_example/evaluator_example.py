#!/usr/bin/env python3

import requests

print("Lanzando test programáticamente... (puede tardar unos minutos)")
r = requests.post('https://ejp-evaluator.appspot.com/FAIR_Evaluator/collections/5/evaluate/', {"resource": "https://purl.org/roh", "executor": "0000-0001-8055-6823", "title": "ROH API Test"})

print(r)
