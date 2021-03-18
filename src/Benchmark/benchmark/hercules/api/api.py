# -*- coding: utf-8 -*-
"""Public Services API"""

from SPARQLWrapper import TURTLE
from flask import (
    Blueprint,
    jsonify,
    request,
    Response,
)
from rdflib import RDFS, URIRef

from ..common.rdf import PREFIXES, bgp_isstore
from ..utils  import sparql_query as sparql

blueprint = Blueprint("api", __name__)


def get_overrides(req):
    overrides = {}
    query = f"""
{PREFIXES}
SELECT DISTINCT * WHERE {{
    ?criterion a asio:Criterion
          ; asio:weight ?weight
}} 
"""
    res = sparql(query)
    spvalues = ''
    for bind in res['results']['bindings']:
        crit = str(bind['criterion']['value'])
        if crit in req:
            weight = req[crit]
            overrides[crit] = weight
        else:
            weight = float(bind['weight']['value'])
        spvalues += f"""( <{crit}> {weight} )"""

    # Again for groups
    query = f"""
{PREFIXES}
SELECT DISTINCT * WHERE {{
    [] a asio:Criterion
       ; dct:subject ?criterion .
    ?criterion asio:weight ?weight
}} 
"""
    res = sparql(query)
    gpvalues = ''
    for bind in res['results']['bindings']:
        crit = str(bind['criterion']['value'])
        if crit in req:
            weight = req[crit]
            overrides[crit] = weight
        else:
            weight = float(bind['weight']['value'])
        gpvalues += f"""( <{crit}> {weight} )"""
    return overrides, spvalues, gpvalues


@blueprint.route("/assessment", methods=["GET", "POST"]) 
def reassess():
    reqdata = request.json if request.is_json else request.form.to_dict()
    overrides, spvalues, gpvalues = get_overrides(reqdata)
    ists = bgp_isstore('store')
    query = f"""
{PREFIXES}
CONSTRUCT {{
  ?assess dct:subject ?store
        ; a ?at
        ; asios:value ?val
        ; dct:description ?why
        ; asios:criterion ?crit . 
  ?crit dct:subject ?group 
        ; a ?ct ; rdfs:label ?cl 
        ; dct:description ?ctdesc
        ; asio:weight ?weight .
  ?group skos:broader ?cat
		; a ?gt ; rdfs:label ?gl 
        ; dct:description ?gdesc
        ; asio:weight ?groupweight .
  ?cat a ?catt 
     ; rdfs:label ?catl
     ; dct:description ?catdesc
}}
WHERE {{
  {ists}
  VALUES(?crit ?weight) {{
    {spvalues}
  }}
  ?assess dc:subject|dct:subject ?store
        ; a ?at
  		; asios:value ?val
  		; asios:criterion ?crit
  . OPTIONAL {{ ?assess dc:description ?why }}
  ?crit dct:subject ?group
        ; a ?ct ; rdfs:label ?cl
  . OPTIONAL {{ ?crit dct:description ?ctdesc }}
  VALUES(?group ?groupweight) {{
    {gpvalues}
  }}
  ?group skos:broader ?cat
  	   ; a ?gt ; rdfs:label ?gl
  . OPTIONAL {{ ?group rdfs:comment ?gdesc }}
  ?cat a ?catt 
     ; rdfs:label ?catl
  . OPTIONAL {{ ?cat rdfs:comment ?catdesc }}
}}
"""
    graph = sparql(query)
    graph.namespace_manager.bind('rdfs', RDFS)
    ttl = graph.serialize(format=TURTLE)
    return Response(ttl, mimetype='text/turtle')


@blueprint.route("/ranking", methods=["POST"])   
def reranking():
    reqdata = request.json if request.is_json else request.form.to_dict()
    overrides, spvalues, gpvalues = get_overrides(reqdata)
    rerank = {
        "overrides"	: overrides,
        "ranking"	: []
    }
    ists = bgp_isstore('store')
    query = f"""
{PREFIXES}
SELECT DISTINCT ?store (SUM(?weighted*?groupweight)/SUM(?groupweight) AS ?score) WHERE {{
  VALUES(?group ?groupweight) {{
    {gpvalues}
  }}
{{
SELECT DISTINCT ?store ?group (SUM(?val*?weight)/SUM(?weight) AS ?weighted) WHERE {{
  {ists}
  VALUES(?crit ?weight) {{
    {spvalues}
  }}
  ?assess dc:subject|dct:subject ?store
  		; asios:value ?val
  		; asios:criterion ?crit . 
  ?crit dct:subject ?group .
  }} GROUP BY ?store ?group 
}} 
  ?group skos:broader ?cat
#       ; asio:weight ?groupweight
}} GROUP BY ?store
 ORDER BY DESC(?score)
"""
    res = sparql(query)
    scoremap = {}
    describe = "DESCRIBE"
    for bind in res['results']['bindings']:
        describe += ' <' + bind['store']['value'] + '>'
        scoremap[bind['store']['value']] = bind['score']['value']
    data = sparql(describe)	
    for bind in res['results']['bindings']:
        stor = bind['store']['value']
        rerank['ranking'].append({
            'id'   : str(stor),
            'name' : str(data.value(URIRef(stor), RDFS.label)),
            'score': round(float(scoremap[stor]), 2)
        })
    return jsonify(rerank)
