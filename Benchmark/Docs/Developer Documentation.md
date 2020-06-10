![](.//media/CabeceraDocumentosMD.png)

# Hercules ASIO Benchmark - Developer documentation

This document gives a brief overview of the code associated with the Hercules ASIO Triple Store Benchmark tool and the underlying data. This code is organised in four different components, available as four subrepositories of the codebase for the project. Each section below covers each of those components.

## [The Criterion Ontology](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/criterion-ontology)

This component is the backbone of the tool, as it is used to model and structure all of the data associated with the assessment of triple stores systems. It provides a dedicated [RDFS ontology](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/criterion-ontology/src/schema.ttl) for the representation of criteria, systems and assessements, as well as a [dataset](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/criterion-ontology/src/criteria.ttl) representing, according to this ontology, the set of criteria used to assess triple stores in the systems with their descriptions, weights, categories, etc.

The two file linked above are the main component of this repository. Other files and script included were support for creating those files and are kept mostly for transparancy reasons. We would expect those files to remain stable in time. However, the creation of a new criteria or the modification of the default weights for criteria if needed should be achieved through editing those files directly (especially the [criteria.ttl](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/criterion-ontology/src/criteria.ttl))

## [The triple store dataset](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/triplestore-dataset)

The repository includes scripts and processes by which base data about the triple stores to be assessed is produced and modelled in RDF according to the criterion ontology. The code in this repository is only expected to run once. Mode details about its functions and structure will be provided at a later stage.

## [Triple Store Assessments](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/triplestore-assessments)

This repository contains the base data for producing assessment information for the triple stores following the criterion ontology. It mostly consist in [files](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/triplestore-assessments/data) that include assessment information and their provenance, [scripts](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/triplestore-assessments/scripts) that transform the content of those files into RDF and the results of running those scripts.

Editing this repository should mostly have the purpose to add new assessments or edit existing ones. This can be done by either adding new RDF data directly in a file to be uploaded onto the backend of the system, or by editing the [source.bib](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/triplestore-assessments/data/sources.bib) file. This file is in the bibtex format, so to represent the references which might be used to evidence assessments, with assessment information included between bibliographic references. For example,

```
== allegrograph ==

...

F1_1::Loads RDF in various serialisations::5::allegrograph_load_doc

@misc{allegrograph_load_doc,
title={AllegroGraph Documentation on Imporing Datat},
url={https://franz.com/agraph/support/documentation/current/agload.html}
}
```

indicates that the triple store allegrograph is given a score of 5 on criterion F1.1 (handling of RDF) with the justification that it "Loads RDF in various serialisations" as evidenced in the document described through a bibtex entry with the key "allegrograph_load_doc".

With this file edited, the script [bib2ttl.py](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/triplestore-assessments/scripts/bib2ttl.py) with the above describe file as parameter will generate RDF (tutle) data from the bibliographic references, and the script [ast2ttl.py](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/triplestore-assessments/scripts/ast2ttl.py) (also with sources.bib in parameter) will generate RDF (according to the criterion ontology) representing the assessment of the triple stores.

## [Triple Store Assessment Interface](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Benchmark/triplestore-assessment-interface)

This component is the frontend of the tool. It is developped as a python flasck application which intallation is described in the [README file](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/triplestore-assessment-interface/README.md) of the repository. The application itself relies on a sparql endpoint being deployed and available with the data from the component described above loaded in one single graph. The address of this endpoint shoudl be configured in the [hercules/settings.py](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/triplestore-assessment-interface/hercules/settings.py) file. Other than this, it is structured as a typical flask application.

Basic [guidelines](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Benchmark/Docs/UserGuide.md) for using the interface once deployed are also available.
