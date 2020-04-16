# -*- coding: utf-8 -*-
"""Public section, including homepage and signup."""
import json
import requests
from flask import (
    Blueprint,
    flash,
    redirect,
    render_template,
    request,
    url_for,
    jsonify,
)
from rdflib import Namespace, RDFS, URIRef
from rdflib.namespace import DC, DCTERMS, DOAP, FOAF, SKOS

from ..utils import sparql_query

blueprint = Blueprint("public", __name__, static_folder="../static")

ASIO = Namespace('http://datascienceinstitute.ie/asio/')

PREFIXES = f"""
  PREFIX asio: <{ASIO}>
  PREFIX asios: <http://datascienceinstitute.ie/asio/schema#>
  PREFIX dc: <http://purl.org/dc/elements/1.1/>
  PREFIX dct: <http://purl.org/dc/terms/>
  PREFIX doap: <http://usefulinc.com/ns/doap#>
  PREFIX owl: <http://www.w3.org/2002/07/owl#>
  PREFIX provo: <http://www.w3.org/ns/prov#>
  PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
  PREFIX sc: <http://purl.org/science/owl/sciencecommons/>
  PREFIX skos: <http://www.w3.org/2004/02/skos/core#>
"""

typetable = """
	( asio:TripleStore "store" )
	( asio:TripleStoreAssessment   "assessment" )
	( asio:Criterion   "criterion" )
	( asio:CriterionCategory   "criterioncategory" )
"""

def guess_type(uri):
    query = f"""
{PREFIXES}
SELECT DISTINCT ?typestring
WHERE {{
	VALUES(?type ?typestring) {{
		{typetable}
	}}
	 <{uri}> a ?type
}}
"""
    res = sparql_query(query)
    if res['results']['bindings']:
        return res['results']['bindings'][0]['typestring']['value']
    return None

def bgp_isstore(var):
	return f"""
	 {{ ?{var} a asio:TripleStore }}
	 UNION
	 {{ ?{var} a doap:Project ; doap:release [ a asio:TripleStoreRelease ] }}
"""

@blueprint.route("/", methods=["GET", "POST"])
def home():
    """Home page."""
    
    query = f"""
{PREFIXES}
SELECT DISTINCT ?criterion
WHERE {{
	?criterion a asio:Criterion
}}
"""
    res = sparql_query(query)
    ncriteria = len(res['results']['bindings'])
    query = f"""
{PREFIXES}
SELECT DISTINCT ?store
WHERE {{
	 {{ ?store a asio:TripleStore }}
	 UNION
	 {{ ?store a doap:Project ; doap:release [a asio:TripleStoreRelease] }}
}}
"""
    res = sparql_query(query)
    nstores = len(res['results']['bindings'])
    
    return render_template("public/home.html", numbers={ "criterion" : ncriteria, "store" : nstores})

@blueprint.route("/criterion", methods=["GET"])   
def criteria():
	benchmark = []
	query = f"""
{PREFIXES}
DESCRIBE ?category ?group ?criterion
WHERE {{  
 ?criterion a asio:Criterion
     ; dct:subject ?group
 . ?group skos:broader ?category
 . ?category a asio:CriterionCategory
}}
"""
	data = sparql_query(query)
	qcats = data.query(f"""
{PREFIXES}
SELECT DISTINCT ?category WHERE {{
  [] a asio:CriterionCategory ; skos:broader ?category
  . ?category a asio:CriterionCategory
}} ORDER BY ?category
	""")
	for row in qcats:
		cat = row.category
		catdesc = {
			'id' : str(cat),
			'name' : str(data.value(cat, RDFS.label)),
			'description' : str(data.value(cat, RDFS.comment)),
			'groups' : []
		}
		for group in data.subjects(SKOS.broader, cat):
			gr = {
				'id' : str(group),
				'name' : str(data.value(group, RDFS.label)),
				'description' : str(data.value(group, RDFS.comment)),
				'criteria' : []
			}
			# Just to ensure they come in ordered
			qcrits = data.query(f"""
{PREFIXES}
SELECT DISTINCT ?criterion WHERE {{
  ?criterion a asio:Criterion ; dct:subject <{group}>
}} ORDER BY ?criterion
	""")
			for rowc in qcrits:
				crit = rowc.criterion
				cr = {
					'id' : str(crit),
					'name' : str(data.value(crit, RDFS.label)),
					'description' : str(data.value(crit, DCTERMS.description)),
				}
				gr['criteria'].append(cr)
			catdesc['groups'].append(gr)		
		benchmark.append(catdesc)
	return render_template("public/benchmark.html", benchmark=benchmark) 


@blueprint.route("/criterion/<slug>", methods=["GET"])
def criterion(slug):
	crid = URIRef('http://datascienceinstitute.ie/asio/criteria/'+slug)
	query = f"""
{PREFIXES}
DESCRIBE <{crid}> ?group
WHERE {{
    <{crid}> dct:subject ?group
}}
"""
	data = sparql_query(query)
	grid = data.value(crid, DCTERMS.subject)
	criterion = {
		"id"   : str(crid),
		"name" : str(data.value(crid, RDFS.label)),
		"description" : str(data.value(crid, DCTERMS.description)),
		"weight" : str(data.value(crid, ASIO.weight)),
		"group" : {
			"id" : str(grid),
			"name" : str(data.value(grid, RDFS.label))
		}
	}
	
	query = f"""
{PREFIXES}
SELECT DISTINCT ?store ?storename ?name ?value ?why WHERE {{
	?assess a asios:CriterionAssessment
	      ; asios:criterion <{crid}>
	      ; asios:value ?value
	      ; dc:description ?why
	      ; dc:subject ?store
	. ?store rdfs:label ?storename
}} ORDER BY DESC(?value)
"""
	res = sparql_query(query)
	ranking = []
	last = None
	for bind in res['results']['bindings']:
		stid = bind['store']['value']
		if last is None or stid!=last:
			ranking.append({
				"id"     : str(stid),
				"name"   : str(bind['storename']['value']),
				"score"  : int(bind['value']['value']),
				"reason" : str(bind['why']['value']),
			})
			last = stid
	
	return render_template("public/criteria.html", criterion=criterion, ranking=ranking) 	


@blueprint.route("/entity", methods=["GET"])
def entity(): 
    what = request.args.get('uri')
    type = guess_type(what)
    slug = what.rsplit('/', 1)[-1]
    return redirect(url_for('.'+type, slug=slug))

@blueprint.route("/store", methods=["GET"])   
def ranking():
    ranking = []
    unranked = []
    ists = bgp_isstore('store')
    query = f"""
{PREFIXES}
SELECT DISTINCT ?store ?namesO (SUM(?bigweight * ?bigscore)/SUM(?bigweight) as ?score) WHERE {{
SELECT DISTINCT ?store ?namesO ?bigcrit ?bigweight ((SUM(?weight * ?val)/SUM(?weight)) as ?bigscore)
WHERE {{
  {{
    SELECT DISTINCT ?store (GROUP_CONCAT(?name; separator='", "') as ?names ) WHERE {{
	 {ists}
    	?store rdfs:label ?name
    }} GROUP BY ?store
  }}
  OPTIONAL {{
  ?assess dc:subject|dct:subject ?store
  		; asios:value ?val
  		; asios:criterion ?crit . 
  ?crit rdfs:label ?criterion  
      ; dct:subject ?bigcrit
      ; asio:weight ?weight .
  ?bigcrit skos:broader ?cat
      ; asio:weight ?bigweight .
  }}
  BIND ( CONCAT('["', ?names, '"]') AS ?namesO)
}} GROUP BY ?store ?namesO ?bigcrit ?bigweight
}} GROUP BY ?store ?namesO 
ORDER BY DESC(?score) ?store
"""
    res = sparql_query(query)
    for bind in res['results']['bindings']:
    	score = 'score' in bind
    	names = json.loads(bind["namesO"]["value"])
    	where = ranking if score else unranked
    	where.append({ 
    		"id" : bind['store']['value'],
    		"name" : names[0],
    		"score" : round(float(bind['score']['value']), 2) if score else None
    	})
    	
    return render_template("public/ranking.html", ranking=ranking, unranked=unranked)

@blueprint.route("/ranking", methods=["POST"])   
def reranking():
	nucrit = request.form.get('criterion')
	nuweight = float(request.form.get('weight'))
	rerank = {
		"criterion"	: nucrit,
		"weight"	: nuweight,
		"ranking"	: []
	}
	query = f"""
{PREFIXES}
SELECT DISTINCT * WHERE {{
	?criterion a asio:Criterion
	      ; asio:weight ?weight
}} 
"""
	res = sparql_query(query)
	spvalues = ''
	for bind in res['results']['bindings']:
		crit = str(bind['criterion']['value'])
		weight = nuweight if crit == nucrit else float(bind['weight']['value'])
		spvalues += f"""( <{crit}> {weight} )"""
	ists = bgp_isstore('store')
	query = f"""
{PREFIXES}
SELECT DISTINCT ?store (SUM(?weighted*?groupweight)/SUM(?groupweight) AS ?score) WHERE {{
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
       ; asio:weight ?groupweight .
}} GROUP BY ?store
ORDER BY DESC(?score)
"""
	res = sparql_query(query)
	scoremap = {}
	describe = "DESCRIBE"
	for bind in res['results']['bindings']:
		describe += ' <' + bind['store']['value'] + '>'
		scoremap[bind['store']['value']] = bind['score']['value']
	data = sparql_query(describe)	
	for bind in res['results']['bindings']:
		stor = bind['store']['value']
		rerank['ranking'].append({
			'id'   : str(stor),
			'name' : str(data.value(URIRef(stor), RDFS.label)),
			'score': round(float(scoremap[stor]),2)
		})
	return jsonify(rerank)
    
@blueprint.route("/store/<slug>", methods=["GET"])
def store(slug):
	uri = URIRef('http://data.datascienceinstitute.ie/software/' + slug)
	query = f"""
DESCRIBE <{uri}>
"""
	data = sparql_query(query)
	name = data.value( uri, RDFS.label )
	home = data.value( uri, DOAP.homepage )
	pages = data.objects( uri, FOAF.page )

	bgp = f"""
 ?assess a asios:CriterionAssessment 
     ; dc:subject <{uri}>
     ; asios:value ?val
     ; asios:criterion ?criterion
  . ?criterion asio:weight ?weight
     ; dct:subject ?bigcrit
  . ?bigcrit asio:weight ?bigweight
     ; skos:broader ?category
  . ?category a asio:CriterionCategory
"""
	
	query = f"""
{PREFIXES}
SELECT DISTINCT ?criterion ?critname ?val ?why ?weight ?bigcrit ?bigcritname ?bigweight ?category ?catname ?catdesc ?source ?sourcetitle
WHERE {{
  {bgp}
  . ?assess dc:description ?why
  . OPTIONAL {{ 
      ?assess provo:primarySource ?src 
    . ?src dc:source ?source ;  dc:title ?sourcetitle
  }}
  . ?criterion rdfs:label ?critname
  . ?bigcrit rdfs:label ?bigcritname
  . ?category rdfs:label ?catname ; rdfs:comment ?catdesc
  
}}
ORDER BY ?category ?bigcrit
"""
	assess = {}
	res = sparql_query(query)
	for bind in res['results']['bindings']:
		cat = bind['category']['value']
		group = bind['bigcrit']['value']
		crit = bind['criterion']['value']
		if cat not in assess : 
			assess[cat] = {
				'name' : bind['catname']['value'],
				'description' : bind['catdesc']['value'],
				'score' : 0,
				'groups' : {}
			}
		if group not in assess[cat]['groups'] :
			assess[cat]['groups'][group] = {
				'name' : bind['bigcritname']['value'],
				'score' : 0,
				'weight' : bind['bigweight']['value'],
				'criteria' : {}
			}
		assess[cat]['groups'][group]['criteria'][crit] = {
			'id'     : bind['criterion']['value'],
			'name'   : bind['critname']['value'],
			'score'  : float(bind['val']['value']),
			'weight' : float(bind['weight']['value']),
			'reason' : bind['why']['value'],
		}
		if 'source' in bind:
			assess[cat]['groups'][group]['criteria'][crit]['source'] = {
				'id' : bind['source']['value'],
				'title' : bind['sourcetitle']['value']
			}
		#ac = assess[cat]['groups'][group]['criteria'][crit]
		#assess[cat]['groups'][group]['score'] += ac['score'] * ac['weight']
		
	query = f"""
{PREFIXES}
SELECT DISTINCT ?bigcrit ?category (SUM(?val * ?weight)/SUM(?weight) as ?bigscore)
WHERE {{
  {bgp}
}}
GROUP BY ?bigcrit ?category ORDER BY ?bigcrit
"""
	res = sparql_query(query)
	for bind in res['results']['bindings']:
		cat = bind['category']['value']
		group = bind['bigcrit']['value']
		assess[cat]['groups'][group]['score'] = round(float(bind['bigscore']['value']),2)

	query = f"""
{PREFIXES}
SELECT DISTINCT ?category (SUM(?bigscore * ?bigweight)/SUM(?bigweight) AS ?score) WHERE {{
SELECT DISTINCT ?category ?bigweight (SUM(?val * ?weight)/SUM(?weight) as ?bigscore)
WHERE {{
  {bgp}
}}
GROUP BY ?bigweight ?category 
  }} GROUP BY ?category
"""
	res = sparql_query(query)
	for bind in res['results']['bindings']:
		cat = bind['category']['value']
		assess[cat]['score'] = round(float(bind['score']['value']),2)
	return render_template("public/store.html", store={
		'uri'  : uri,
		'name' : name, 
		'home' : home,
		'score': 0,
		'pages': data.objects( uri, FOAF.page )
	}, assessment=assess)