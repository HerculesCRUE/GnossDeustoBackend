![](.//media/CabeceraDocumentosMD.png)

# Adding Criteira to the Benchmark Framework

The process to add criteria to the framework is based on editing the data sources that is used in by the tool to display scores and ranke them mostly in two places: the criteria description and the triple srore assessments. 

## Adding criteria descriptions

The description of criteria, following the criterion ontology established for the framework, is contained in the [criteria.ttl](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/criterion-ontology/src/criteria.ttl) file, which is in the RDF turtle format.

Each criteria belongs to a category, has a title, a description and a weight. Categories too have titles, descriptions and weights, as well as, potentially, an upper category. For example, adding the category to represents aspects of the Linked Data platform is done by adding:

```
:F4 a asio:CriterionCategory
	; rdfs:label "Linked Data Platform (LDP)"@en
	; rdfs:comment "Linked Data Platform (LDP) defines a set of rules for HTTP operations on web resources, some based on RDF, to provide an architecture for read-write Linked Data on the web. A score of 5 would include full compliance with the recommendations at https://www.w3.org/TR/ldp/"@en
	; skos:broader :F
	; asio:weight "5.0"^^xsd:decimal
.
```

Which describes it as a sub-category of the functionality category (F) with a weight of 5.

In order to add a criterion in this category, one might add to the file the follow, as an example of a criterion related to the management of RDF resources with a weight of 5:

```
:F4_1 a asio:Criterion
	; rdfs:label "Support for the managment of LDP RDF resources"@en
	; dc:description "The system support the management of Linked Data Platform RDF resources, according to the specification. This gets a score of 5 for full compliance with the specification. "@en
	; dc:subject :F4
	; asio:weight "4.0"^^xsd:decimal
.
```

## Adding criteria assessements

Criteria assessments are included in the file [sources.bib](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/triplestore-assessments/data/sources.bib) which contains both the assessments themselves in a specific "short-hand" format, and the sources containing information related to those assessment, in the bibtex format. The extract from this file below shows for example the reference to a part of the documentation for the virtuoso triplestore regarding linked data platform implementation, and its use in the assessement of criteria F4_1 described above, with a score of 5.

```
@misc{virtuoso_ldp,
title= {Virtuoso support for Linked Data Platform},
url = {http://vos.openlinksw.com/owiki/wiki/VOS/VirtLDP}
}

F4_1::Implements support for managing RDF resources::5::virtuoso_ldp
```

To generate the RDF turtle files that can be used by the tool, two scripts are available: [bib2ttl.py](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/triplestore-assessments/scripts/bib2ttl.py) which generates a rdf data from the bibtex descriptions, and [ast2ttl.py](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/triplestore-assessments/scripts/ast2ttl.py) which generates RDF representing assessments of the defined criteria according to the criterion ontology. More concretely, running:

```
python scripts/bib2ttl.py data/sources.bib > bib.ttl
```

generates the bib.ttl file containings bibliographical references, and running:

```
python scripts/ast2ttl.py data/sources.bib > assessments.ttl
```

generates the assessments.ttl file conraining description of the criterion assessments.

## Loading the data

The criteria.ttl, bib.ttl and assessments.ttl file can be loaded directly into the the triplestore used as backend of the tool, in the relevant graph.
