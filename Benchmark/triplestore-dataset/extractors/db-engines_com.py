import feedparser, requests
from bs4 import BeautifulSoup as Soup
from bs4.element import Tag
import pandas as pd
from rdflib import Graph, Literal, URIRef, RDFS, XSD
from rdflib.namespace import FOAF

import common.rdf_factories as maker

def mine_html(graph):
	url = 'https://db-engines.com/en/ranking/rdf+store'
	page = requests.get(url)
	soup = Soup(page.content, 'html.parser')
	table = soup.find('table', attrs={ 'class' : 'dbi' })
	for a in table.select('tr a[href^="https://db-engines.com/en/system/"]'):
		for l in a.children:
			if isinstance(l, Tag):
				z = l.decompose()
		name = a.get_text().strip()
		sw = maker.triplestore(graph, a.get_text().strip())
		graph.add( (sw, FOAF.page, URIRef(a['href'])) )
	return graph

def mine_rss(graph):
	feed = feedparser.parse('https://db-engines.com/en/rss/ranking_rdf+store.xml')
	#r  = requests.get('https://db-engines.com/en/ranking/rdf+store')
	content = feed.entries[0].summary_detail.value # r.text
	soup = Soup(content, 'html.parser')

	table = soup.find('table', attrs={})
	l = []
	for tr in table.find_all('tr'):
	    td = tr.find_all('td')
	    row = [el.text for el in td if len(td) == 4]
	    if row : l.append(row)
	df = pd.DataFrame(l, columns=["rank", "DBMS", "score", "changes"])
	print(df)
	for index, row in df.iterrows():
	    print(row['DBMS'], row['score'])
	    
	return graph
    

def extract_graph(targetGraph=None):
	print('Running db-engines.com RSS feed and HTML extractor...')
	graph = targetGraph if targetGraph else Graph()
	mine_rss(graph)
	mine_html(graph)
	return graph