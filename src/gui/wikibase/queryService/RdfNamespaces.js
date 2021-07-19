var wikibase = window.wikibase || {};
wikibase.queryService = wikibase.queryService || {};
wikibase.queryService.RdfNamespaces = {};

( function ( $, RdfNamespaces ) {
	'use strict';

	RdfNamespaces.NAMESPACE_SHORTCUTS = {
		Hércules: {
			roh: 'http://purl.org/roh#',
			bfo: 'http://purl.org/roh/mirror/obo/bfo#',
			bibo: 'http://purl.org/roh/mirror/bibo#',
			foaf: 'https://xmlns.com/foaf/0.1/',
			iao: 'http://purl.org/roh/mirror/obo/iao#',
			ns: 'http://www.w3.org/2003/06/sw-vocab-status/ns#',
			obo: 'http://purl.obolibrary.org/obo/',
			owl: 'http://www.w3.org/2002/07/owl#',
			rdf: 'http://www.w3.org/1999/02/22-rdf-syntax-ns#',
			rdfs: 'http://www.w3.org/2000/01/rdf-schema#',
			ro: 'http://purl.org/roh/mirror/obo/ro#',
			schema: 'https://schema.org/',
			skos: 'http://www.w3.org/2004/02/skos/core#',
			terms: 'http://purl.org/dc/terms/',
			uneskos: 'http://purl.org/umu/uneskos#',
			vitro: 'http://vitro.mannlib.cornell.edu/ns/vitro/0.7#',
			vivo: 'http://purl.org/roh/mirror/vivo#',
			xml: 'http://www.w3.org/XML/1998/namespace',
			xsd: 'http://www.w3.org/2001/XMLSchema#'
		},
	};

	RdfNamespaces.ENTITY_TYPES = {
		'http://www.wikidata.org/prop/direct/': 'property',
		'http://www.wikidata.org/prop/direct-normalized/': 'property',
		'http://www.wikidata.org/prop/': 'property',
		'http://www.wikidata.org/prop/novalue/': 'property',
		'http://www.wikidata.org/prop/statement/': 'property',
		'http://www.wikidata.org/prop/statement/value/': 'property',
		'http://www.wikidata.org/prop/statement/value-normalized/': 'property',
		'http://www.wikidata.org/prop/qualifier/': 'property',
		'http://www.wikidata.org/prop/qualifier/value/': 'property',
		'http://www.wikidata.org/prop/qualifier/value-normalized/': 'property',
		'http://www.wikidata.org/prop/reference/': 'property',
		'http://www.wikidata.org/prop/reference/value/': 'property',
		'http://www.wikidata.org/prop/reference/value-normalized/': 'property',
		'http://www.wikidata.org/wiki/Special:EntityData/': 'item',
		'http://www.wikidata.org/entity/': 'item'
	};

	RdfNamespaces.ALL_PREFIXES = $.map( RdfNamespaces.NAMESPACE_SHORTCUTS, function ( n ) {
		return n;
	} ).reduce( function ( p, v, i ) {
		return $.extend( p, v );
	}, {} );

	RdfNamespaces.STANDARD_PREFIXES = {
		wd: 'PREFIX wd: <http://www.wikidata.org/entity/>',
		wdt: 'PREFIX wdt: <http://www.wikidata.org/prop/direct/>',
		wikibase: 'PREFIX wikibase: <http://wikiba.se/ontology#>',
		p: 'PREFIX p: <http://www.wikidata.org/prop/>',
		ps: 'PREFIX ps: <http://www.wikidata.org/prop/statement/>',
		pq: 'PREFIX pq: <http://www.wikidata.org/prop/qualifier/>',
		rdfs: 'PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>',
		bd: 'PREFIX bd: <http://www.bigdata.com/rdf#>'
	};

	RdfNamespaces.addPrefixes = function( prefixes ) {
		$.extend( RdfNamespaces.ALL_PREFIXES, prefixes );
	};

	RdfNamespaces.getPrefixMap = function ( entityTypes ) {
		var prefixes = {};
		$.each( RdfNamespaces.ALL_PREFIXES, function ( prefix, url ) {
			if ( entityTypes[url] ) {
				prefixes[prefix] = entityTypes[url];
			}
		} );
		return prefixes;
	};

} )( jQuery, wikibase.queryService.RdfNamespaces );
