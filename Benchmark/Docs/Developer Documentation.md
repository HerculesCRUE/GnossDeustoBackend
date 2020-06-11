![](.//media/CabeceraDocumentosMD.png)

# Hercules ASIO Benchmark - Developer documentation

This document gives a brief overview of the code associated with the Hercules ASIO Triple Store Benchmark tool and the underlying data. This code is organised in four different components, available as four subrepositories of the codebase for the project. Each section below covers each of those components.

## Components

### [The Criterion Ontology](/Benchmark/criterion-ontology)

This component is the backbone of the tool, as it is used to model and structure all of the data associated with the assessment of triple stores systems. It provides a dedicated [RDFS ontology](/Benchmark/criterion-ontology/src/schema.ttl) for the representation of criteria, systems and assessements, as well as a [dataset](/Benchmark/criterion-ontology/src/criteria.ttl) representing, according to this ontology, the set of criteria used to assess triple stores in the systems with their descriptions, weights, categories, etc.

The two files linked above are the main components of this repository. Other files and scripts included were support for creating those files and are kept mostly for transparancy reasons. We would expect those files to become stable over time. However, the creation of a new criterion or the modification of the default weights for criteria, if needed, should be achieved through editing those files directly (especially the [criteria.ttl](/Benchmark/criterion-ontology/src/criteria.ttl))

### [The Triple Store Dataset](/Benchmark/triplestore-dataset)

The repository includes scripts and processes by which base data about the triple stores to be assessed are produced and modelled in RDF according to the [criterion ontology]((#the-criterion-ontology)). The code in this repository is only expected to run once, or periodically if one wants to stay up to date on new and upcoming triple stores. Mode details about its functions and structure will be provided at a later stage.

### [Triple Store Assessments](/Benchmark/triplestore-assessments)

This repository contains the base data for producing assessment information for the triple stores following the criterion ontology. It mostly consist in [files](/Benchmark/triplestore-assessments/data) that include assessment information and their provenance, [scripts](/Benchmark/triplestore-assessments/scripts) that transform the content of those files into RDF, and the results of running those scripts.

Editing this repository should mostly have the purpose of adding new assessments or edit existing ones. This can be done by either adding new RDF data directly in a file to be uploaded onto the backend of the system, or by editing the [source.bib](/Benchmark/triplestore-assessments/data/sources.bib) file. This file is in [BibTeX format](http://www.bibtex.org/Format/), so to represent the references which might be used to evidence assessments, with assessment information included between bibliographic references. For example,

```
== allegrograph ==

...

F1_1::Loads RDF in various serialisations::5::allegrograph_load_doc

@misc{allegrograph_load_doc,
title={AllegroGraph Documentation on Importing Data},
url={https://franz.com/agraph/support/documentation/current/agload.html}
}
```

indicates that the triple store allegrograph is given a score of 5 on criterion F1.1 (handling of RDF) with the justification that it "Loads RDF in various serialisations" as evidenced in the document described through a bibtex entry with the key "allegrograph_load_doc".

With this file edited, the script [bib2ttl.py](/Benchmark/triplestore-assessments/scripts/bib2ttl.py) with the above describe file as parameter will generate RDF (turtle) data from the bibliographic references, and the script [ast2ttl.py](/Benchmark/triplestore-assessments/scripts/ast2ttl.py) (also with sources.bib in parameter) will generate RDF (according to the criterion ontology) representing the assessment of the triple stores.

### [Triple Store Assessment Interface](/Benchmark/triplestore-assessment-interface)

This component is the frontend of the tool. It is developed as a Python [Flask](https://flask.palletsprojects.com/) application. Its intallation procedure is described in the [README file](/Benchmark/triplestore-assessment-interface/README.md) of the repository. The application itself relies on a SPARQL endpoint being deployed and available with the data from the component described above, all loaded onto a single dataset (either on one single RDF graph, or on multiple graphs as long as the default graph is their union). The address of this endpoint must be configured in the [hercules/settings.py](/Benchmark/triplestore-assessment-interface/hercules/settings.py) file. Other than this, it is structured as a typical Flask application.

Basic [guidelines](/Benchmark/Docs/UserGuide.md) for using the interface once deployed are also available.

## HOWTO

The [Triple Store Assessment Interface](/Benchmark/triplestore-assessment-interface) is completely data-driven: Neither the criteria nor their categories are hardcoded, and the application will attempt to display whatever is contained in the associated [dataset](#the-triple-store-dataset).

In the following, we will assume that `asio:` is the prefix for namespace `<http://datascienceinstitute.ie/asio/>`. Note however that __namespaces are currently unstable and subject to change__.

### Add a criterion

To add a new criterion to your assessment framework, simply add its RDF description to the dataset that the Web app is accessing via SPARQL.

In order for the criterion to be visualized, its data must include the following:
1. be typed as an `asio:Criterion`;
2. a category for the criterion, established as a `dc:subject` whose object is of type `asio:CriterionCategory`;
3. a more general super-category, as the `skos:broader` of the one at (2);
4. weights for the subjects of (1) and (2);
5. an `rdfs:label` for each of the above.

It is also advisable to add human-readable descriptions in the form of `dc:description` or `rdfs:comment` literals.

For example, here is a snippet in Turtle format of a criterion that addresses support for the [Linked Data Platform](https://www.w3.org/TR/ldp/) specification, with an added category:
  
    @prefix     : <http://example.org/triplestoreassessment/> .
    @prefix asio: <http://datascienceinstitute.ie/asio/> .
    @prefix   dc: <http://purl.org/dc/terms/> .
    @prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .
    @prefix skos: <http://www.w3.org/2004/02/skos/core#> .
    @prefix  xsd: <http://www.w3.org/2001/XMLSchema#> .
     
    :ldp_support    a asio:Criterion
	    ; rdfs:label "Linked Data Platform support"@en
	    ; dc:description "Compliance with the HTTP protocol bindings established by the W3C Linked Data Platform specification, https://www.w3.org/TR/ldp/"@en
	    ; dc:subject :F4
	    ; asio:weight "4.0"^^xsd:decimal
	.
     
	:functionalities_standards    a asio:CriterionCategory
	    ; rdfs:label "Web service standard compliance"@en
	    ; rdfs:comment "Besides being able to handle RDF, a triplestore should be able to serve it with respect to standards and conventions."@en
	    ; skos:broader <http://datascienceinstitute.ie/asio/criteria/F>
	    ; asio:weight "5.0"^^xsd:decimal
    .

Here, `<http://datascienceinstitute.ie/asio/criteria/F>` is a broad category for functionalities that is provided in the default dataset.

### Add an assessment

Here, an Assessment is a binding between an `asio:Criterion` and something else that can be a database system in general, or a specific version of it. It must have a score, which currently is on a 0-5 scale.

The system being assessed will be a `dc:subject` and the justification is given as a `dc:description`. It is also advisable that this assessment be sourced, through an appropriate `prov:primarySource` provenance statement.

The following Turtle snippet is an example of how to describe an assessment for "My triplestore" on the criterion defined above:

    @prefix      : <http://example.org/triplestoreassessment/> .
    @prefix asios: <http://datascienceinstitute.ie/asio/schema#> .
    @prefix    dc: <http://purl.org/dc/terms/> .
    @prefix  prov: <http://www.w3.org/ns/prov#> .
     
    :mytriplestore-ldp_support a asios:CriterionAssessment
        ; asios:criterion :ldp_support
        ; dc:subject :mytriplestore
        ; asios:value 3
        ; dc:description "Does not fully respect the convention for POST parameters." 
        ; prov:primarySource :mytriplestore-ldp_support-evidence 
    .

where `:mytriplestore` is of type `asio:TripleStore` or  `asio:TripleStoreRelease`, and `:mytriplestore-ldp_support-evidence` can be anything, such as `foaf:Document` or a `bibo:Article`.
