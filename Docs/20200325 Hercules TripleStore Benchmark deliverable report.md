# Hércules Backend ASIO. TripleStore Benchmark deliverable report

We report here on the creation of a TripleStore assessment framework and
on the results obtained from applying this framework to several modern
TripleStore. We consider a TripleStore any data storage and querying
system which is capable of handling RDF[^1] data, either natively or
not, and to query the data using the SPARQL query language and
protocol[^2]. A number of benchmark studies have been conducted in the
past to assess the relative performance of TripleStores. Here however,
while we rely on the results of those studies, we extend the assessment
to a number of other aspects that might affect the choice of a given
triple store in a production scenario. We define around 60 criteria that
are assessed on a 0 to 5 scale. As the primary objective of this
framework is to provide a decision support tool for setting up linked
data for universities, we establish weights that reflect the importance
of those criteria in that specific context. For example, considering
that it is unlikely that, in this scenario, very large scales will be
achieved, we give less importance to performance when dealing with
billions of triples, as compared with millions.

The criteria defined, their assessment for each of the TripleStore
systems, as well as the sources of the information leading to such
assessment, for a network of entities which, while not large, need to be
managed and structured so to be explorable, as they form the basis of
the decision to adopt a given system. For this reason, and to be
consistent with the technology assessed, we build a framework based on
RDF and SPARQL to represent criteria, weights, assessments and
bibliographical sources, query them to obtain a ranking of TripleStores,
and publish them as an open dataset. The end result, besides the data
itself, is a simple web application to display the ranking, explore
assessments and customised weights.

The rest of the report describes the different components defined and
the results obtained. We start by providing an updated version of the
Criteria defined for assessment, and a short description of the triple
stores assessed. We then describe briefly the general architecture of
the framework before presenting the results.

Table of contents
=================

**[Table of contents](#table-of-contents) 1**

**[Criteria](#criteria) 3**

> [Functionalities (F)](#functionalities-f) 3
>
> [Performance and scalability](#performance-and-scalability) 5
>
> [Management and maintenance](#management-and-maintenance) 7

**[TripleStore Assessed](#triplestore-assessed) 9**

**[Architecture and data](#architecture-and-data) 10**

**[Results](#results) 10**

**[Appendix 1: Criteria in RDF](#appendix-1-criteria-in-rdf) 11**

**[Appendix 2: Criterion Ontology](#appendix-2-criterion-ontology) 12**

Criteria
========

In this section, we describe the criteria used to assess TripleStores.
We divide those criteria into several categories (functionalities,
performance/scalability and management/maintenance), and for each of the
criteria, briefly describe how it will be evaluated, and what weight we
apply by default. The criteria are defined as ways to evaluate a
TripleStores to be evaluated on a 0 to 5 scale. Each category has a
weight (out of 5) indicating its importance, and each criterion has a
weight which is used to aggregate the criteria within the category. The
representation of those criteria in RDF/TTL is provided in [[Appendix
1]{.underline}](#7cmwyuryxkse).

Functionalities (F)
-------------------

Functionalities relate to the capabilities of the TripleStore. Most
weights in this section reflect the fact that some functions are seen as
necessary, while others are considered good to have, but optional.

**Base handling of RDF and SPARQL (F1, Weight: 5)**

Basic functions of a triple store are considered necessary, i.e.,
failing in this category would rule the triplestore/graphstore out. The
score for each criterion here is based on: 1- whether the triple store
addresses the functionality, 2- whether the function is complete and
compliant with standards, and 3- whether the implementation of the
function is effective.

| Function                      | Weight | Description                                                                                                                                                                                                                              |
|-------------------------------|--------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Ability to load and serve RDF | 5      | Can load RDF in its standard serialisations, using standard interfaces, efficiently. Will provide RDF as results of queries in standard serialisations effectively.                                                                      |
| SPARQL 1.1 compliance         | 5      | Provide SPARQL endpoints using the SPARQL protocol and a complete implementation of the SPARQL 1.1 query language, including support for multiple languages and character encodings.                                                     |
| SPARQL update compliance      | 3      | Provide SPARQL Update endpoints compliant with the  SPARQL Update protocol. Assuming that, in the absence of SPARQL update, other approaches can be used to update the data, this is seen as less important than the previous functions. |


**Extensions (F2, Weight: 3)**

Extensions are functionalities that are not considered core to the
handling of RDF/SPARQL linked data, but would provide useful additions.

| Function                                                          | Weight | Description                                                                                                                                                                                                                                                                                                                   |
|-------------------------------------------------------------------|--------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Full-text search                                                  | 5      | Provides a full-text index of the data, and can make full-text queries (e.g. as part of SPARQL queries). As this is a common requirement in many scenarios, the weight of this function is high.(e.g. through Lucene, Solr, ElasticSearch)                                                                                    |
| GeoSPARQL                                                         | 2      | Compliant with the GeoSPARQL specification, providing SPARQL construct to query data based on geographical locations. As this is a less common requirement, which can be addressed through dedicated SPARQL queries, this is seen as less critical.                                                                           |
| SPARQL query federation (SERVICE Clause)                          | 2      | Compliant with SPARQL query federation through providing the SERVICE clause. As implementations of this tend to be inefficient, and often not needed if data distribution can be achieved in another way, this is seen as a weak requirement.                                                                                 |
| SPARQL query federation (automatic handling of data distribution) | 2      | The SERVICE clause described above requires knowledge of where the data is distributed. A system able to automatically handle data distribution and federate queries over distributed stores transparently would be an advantage without being absolutely required.                                                           |
| Reasoning (RDFS entailment)                                       | 5      | Being able to enable basic taxonomy-based reasoning over RDFS ontologies. This is the simplest level of reasoning, which is often sufficient in simple use cases.                                                                                                                                                             |
| Reasoning (“Basic” OWL profiles)                                  | 4      | The simplest OWL profiles (EL, QL) are made to represent classes of language that enable efficient reasoning. In cases where reasoning is useful, adhering to those profile gives some assurance that performance will not be too degraded. Those can therefore be useful in a broader, but slightly more complex use cases.  |
| Reasoning (DL-based OWL profiles)                                 | 2      | The more advanced profiles of OWL require reasoners based on description logics, which can be very complex and resource-consuming, especially for large knowledge bases. This addresses a small number of more advanced use cases.                                                                                            |
| Custom inference rules                                            | 3      | Ability to implement custom inference rules, including Jena-like rules, SWRL, or others.                                                                                                                                                                                                                                      |
| Method for RDF validation and close world assumption              | 3      | Provide support for, for example, SHACL.                                                                                                                                                                                                                                                                                      |
| Programmatic access through other RDF standard APIs               | 2      | Other programming interfaces exist for RDF data (e.g. RDF4J) and might be available to the system.                                                                                                                                                                                                                            |
| Programmatic access through other database access methods         | 1      | The system provides access to the data through standard database access methods (e.g. JDBC, ODBC).                                                                                                                                                                                                                            |
| Named Entity recognition                                          | 3      | Availability to recognise entities from text.                                                                                                                                                                                                                                                                                 |
| Handling of property graphs                                       | 2      | The system might include the ability to handle property graphs through specific functions.                                                                                                                                                                                                                                    |
| Other data formats on ingest                                      | 1      | The system might provide facilities to load and transform into RDF data from other formats (e.g. CSV/TSV, XML, JSON, RDBs through “virtual graph” system).                                                                                                                                                                    |

**Security (F3, Weight: 4)**

The handling of security for any information system is critical.
TripleStores might include their own security mechanisms, or those might
have to be handled separately.
| Function                       | Weight | Description                                                                                                                                                                                                                                              |
|--------------------------------|--------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Data encryption                | 2      | This relates to whether the system provides options to encrypt the data in the store, and the strength of the encryption.                                                                                                                                |
| Role-based access control      | 5      | This relates to the ability to restrict access to the data based on the role of the user. The level at which access control is provided (graph, entities, individual triples, custom) affects the score for this criterion.                              |
| Attribute-based access control | 3      | This relates to the ability to restrict access to the data based on custom policies related to attributes of the user. The level at which access control is provided (graph, entities, individual triples, custom) affects the score for this criterion. |
| Usage quotas                   | 2      | The system might provide a way for the administrator to impose quotas on the use of the system (e.g. amount of triples read in a given period of time), to avoid overload.                                                                               |


Performance and scalability
---------------------------

Performance relates to the amount of time and computational resources
required to run and use the system. Scalability relates to the ability
of the system to handle increasingly large amounts of data.

**Query response time (P1, Weight: 5)**

Query response time corresponds to the time between when a query is
submitted to the system and when the results are obtained. Scoring here
is based on comparing average results over a large variety of queries on
several sizes of datasets (i.e. the system with the smallest average
query response time will be scored 5). In the weights here, there is an
assumption that TripleStores might perform very differently depending on
whether they are optimised for large or small datasets, and that most
scenarios will require significant, but not very large amounts of data.

| Data size                        | Weight | Description                                                                        |
|----------------------------------|--------|------------------------------------------------------------------------------------|
| Tens of thousands of triples     | 5      | Average query response time on datasets with tens of thousands of RDF triples.     |
| Hundreds of thousands of triples | 5      | Average query response time on datasets with hundreds of thousands of RDF triples. |
| Millions of triples              | 4      | Average query response time on datasets with millions of RDF triples.              |
| Tens of millions of triples      | 3      | Average query response time on datasets with tens of millions of RDF triples.      |
| Hundreds of millions of triples  | 2      | Average query response time on datasets with hundreds of millions of RDF triples.  |
| Billions of triples              | 1      | Average query response time on datasets with billions of RDF triples.              |

**Loading and update time (P2, Weight: 2)**

Loading and update time, since it does not affect the end user directly,
is less prominent than query response time. However, it might affect the
performance of the system overall. Loading and update times are
evaluated based on calculating the time required to make changes to
datasets, using different size and complexity of changes.

| Operation     | Weight | Description                                                                                                                   |
|---------------|--------|-------------------------------------------------------------------------------------------------------------------------------|
| Batch loading | 5      | Batch loading is the insertion of large amounts of triples into the system, as a background process.                          |
| Insert        | 4      | Insert corresponds to adding triples on the basis of specific conditions (using SPARQL update).                               |
| Clear         | 2      | Clear corresponding to dropping a whole graph of the whole of the data in the store (possibly to overnight it with new data). |
| Delete        | 3      | Delete corresponds to removing specific triples based on specific conditions (using SPARQL update).                           |

**Memory footprint (P3, Weight: 4)**

The amount of memory a system requires to run is critical as it not only
represents an indicator of the amount of computing resources required to
run the system, but also because it affects the overall performance of
the system. Memory footprint is measured while the queries used to
measure query response times are executed, taking the peak memory use of
the system.

| Data size                        | Weight | Description                                                                                            |
|----------------------------------|--------|--------------------------------------------------------------------------------------------------------|
| Tens of thousands of triples     | 5      | Amount of memory allocated to the system when querying datasets with tens of thousands of triples.     |
| Hundreds of thousands of triples | 5      | Amount of memory allocated to the system when querying datasets with hundreds of thousands of triples. |
| Millions of triples              | 4      | Amount of memory allocated to the system when querying datasets with millions of triples.              |
| Tens of millions of triples      | 3      | Amount of memory allocated to the system when querying datasets with tens of millions of triples.      |
| Hundreds of millions of triples  | 2      | Amount of memory allocated to the system when querying datasets with hundreds of millions of triples.  |
| Billions of triples              | 1      | Amount of memory allocated to the system when querying datasets with billions of triples.              |

**Robustness and Scaling (Weight: 4)**

Besides pure runtime performance, reliability and the ability to scale
the system up are critical. Crashes, downtimes, and performance
decreases can have a very negative effect on the end user.

| Criterion                           | Weight | Description                                                                                                                                                                     |
|-------------------------------------|--------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Robustness / reliability            | 5      | Robustness and reliability are calculated by running complex, notoriously challenging queries with large result sets, stress-testing the system.                                |
| Transaction and rollback mechanism  | 3      | The system provides a journal of update transactions, which can be used to rollback changes.                                                                                    |
| Vertical scalability                | 2      | Vertical scalability is the ability of the system to maximally utilise the resources available to it, and to expand to new resources made available.                            |
| Horizontal scalability (clustering) | 3      | Horizontal scalability is the ability of the system to be distributed over multiple instances (e.g. in a cluster), with new instances being added to enable increases in scale. |

Management and maintenance
--------------------------

Management and maintenance relate to the complexity of getting and
keeping the system running from the point of view of the administrator.
While the initial cost of obtaining the system and deploying are
important, the effort to update and maintain the system represents a
large part of the cost of running the system.

**Costs and rights (M1, Weight: 5)**

The cost of obtaining the system is an obvious criteria for the choice
of the system, with many systems being part of a commercial offering,
others being free and open source, with hybrid models. The assessment of
the cost is based on the most likely option depending on the scenario.
For open source systems, there might be constraints and specific clauses
that apply, which might restrict some aspects of the system.

| Criterion           | Weight | Description                                                                                                                                                                                       |
|---------------------|--------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Cost                | 5      | Cost is calculated based on the price of the software, considering the options that are most likely to be required. Free and open source system score 5.                                          |
| Open source licence | 2      | Open source licences can be more or less permissive, or include specific aspects that could restrict the use or distribution of the system. A system under commercial, non-free licences score 0. |

**Ease of deployment (M2, Weight: 2)**

The effort required in deployment is important, especially as it might
have to be realised by administrators with limited experience in
TripleStores.

| Criterion                    | Weight | Description                                                                                                                                                                                                                              |
|------------------------------|--------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Automatic installer          | 3      | This criterion looks at whether an automatic installer is available to get the system up and running.                                                                                                                                    |
| Complexity of configuration  | 5      | This criterion measures the amount of effort required to configure the system, including whether default configurations are available, how valid they are, the number of changes required to be made, and how complex those changes are. |
| Quality of the documentation | 4      | This measures how much the system can be setup in common scenarios following the documentation, and how much the documentation addresses possible problems in installation.                                                              |
| Dependencies                 | 3      | The system might require other libraries or systems to be installed in order to run, which might increase the cost and make deployment more complicated.                                                                                 |
| Released as a container      | 2      | The system is available as a (Docker) container, ready to be deployed.                                                                                                                                                                   |

**Administration, updates and maintenance (Weight: 5)**

The effort required in keeping the system running is a key part of the
cost of the system. There are many factors that affect the amount of
efforts required to keep the system updated and maintained, which are
evaluated differently.

| Criterion                                   | Weight | Description                                                                                                                                                                                                                     |
|---------------------------------------------|--------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Quality of the documentation                | 4      | As for the same criterion in relation to deployment, the quality of the documentation here relates to how complete and clear the documentation is in relation to realising basic maintenance operations and resolving problems. |
| Support services                            | 4      | This relates to whether the cost of the system includes customer support services from the system provider.                                                                                                                     |
| Active development                          | 5      | This relates to how much the system is being developed, whether by the company providing it or by the open source community.                                                                                                    |
| Update frequency                            | 3      | This relates to how often the system is being updated by the provider, whether for additional features, bug fixes, etc.                                                                                                         |
| Automatic update                            | 3      | This relates to whether update are automatic, or automatically deployed.                                                                                                                                                        |
| Downtime required                           | 4      | This relates to whether downtime from the system is required for updates.                                                                                                                                                       |
| Backup mechanism                            | 3      | A preferably automatic mechanism is available to make regular backups of the data.                                                                                                                                              |
| Monitoring                                  | 2      | Interfaces to monitor the state (health, data) of the system are available.                                                                                                                                                     |
| Availability of a strong community of users | 2      | A large community of users might help with easy of use and administration, by providing a knowledge base of problems and solutions. This can be evaluated in part by using the db-engines ranking.                              |
| Available as a cloud service                | 1      | There is an option to use the system as a cloud service (DBaaS), rather than deploying it locally.                                                                                                                              |

TripleStore Assessed 
====================

Below is a list, with a short description, of TripleStores that have
already been assessed through our framework. Others are also described
in the Framework, but have not been assessed at this stage. Those are
selected in priority because:

-   They provide compliant RDF and SPARQL 1.1 interfaces to the data

-   They have been actively developed and significantly used recently

**AllegroGraph:** AllegroGraph is a graph database provided commercially
by FranzInc. It scales to billions of triples and supports SPARQL,
RDFS++, and Prolog reasoning.

**Blazegraph:** Blazegraph is the predecessor of Amazon Neptune, and
provide similar functionalities (and some that were removed from
Neptune). It focuses on large scale graph databases. Despite having been
taken over by Amazon, the github project for Blazegraph is still being
updated.

**Corese:** Corese is a software platform developed by INRIA for
supporting Semantic Web and Linked Data applications. It implements RDF,
RDFS, SPARQL 1.1 Query and Update, OWL RL and SHACL.

**Fuseki (Jena TDB):** Apache Jena Fuseki is a SPARQL server based on
the Jena TDB store. It provides SPARQL 1.1 compliant query and update
endpoints, as well as security (using Apache Shiro), monitoring and
monitoring and administration.

**Neo4J:** Neo4J is a graph database and platform with support for RDF
ingesting and SPARQL querying.

**Stardog:** Stardog is a knowledge graph platform with built-in,
scalable virtual graph querying, reasoning and machine learning
capabilities.

**Virtuoso:** Provided by the company OpenLinks, Virtuoso is Data Server
enabling multi-model data management with a focus on large scale RDF
TripleStores and SPARQL querying.

Architecture and data
=====================

As mentioned above, to keep track of the different elements involved in
the assessment of TripleStores, we built the framework as an
RDF/SPARQL-based application, where criteria, weights, assessments and
sources are represented in RDF, and accessed in the ranking interface
through SPARQL (see figure below).

**Figure 1:** Overview of the architecture of the framework.

All of the information at the basis of the ranking of TripleStores is
represented using specially created the criteria ontology (see figure
below and [[Appendix 2]{.underline}](#b5aic4awbkqq)). This ontology is
inspired from the Criterion Ontology Design Pattern[^3], and uses
DOAP[^4] and ProvO[^5]. Based on this, all the criteria described in the
[[Criteria Section]{.underline}](#wnjjjeniodpu) are given an RDF
representations, and the assessed TripleStores are also described. The
Assessments of each criterion for each TripleStore System is also
inputted, together with reference to the documents that contain the
evidence for the considered assessment.

With all the data entered and loaded into the TripleStore at the centre
of the framework, the role of the application server is simply to run
the SPARQL queries that enable obtaining the ranking of TripleStore, as
well as to explore specific assessments for TripleStores. It is
interesting in particular that all of the ranking (using a score
corresponding to the weighted average over the criteria/categories) is
computed entirely through one SPARQL query.

![](.//media/image2_BenchmarkReport.png)

**Figure 2:** Diagram of the Criterion Ontology (see [[Appendix
2]{.underline}](#b5aic4awbkqq)).

![](.//media/image3_BenchmarkReport.png)

**Figure 3:** Screenshot of the ranking interface.[^6]

Results
=======

As can be seen in the screenshot above, the framework's application
provides a ranking which is based on the weighted average of each
criterion category, which score is itself based on a weighted average of
the included criterion. It also allows to further explore the
computation of scores and to customize weights based on specific needs.
The table below summarises the results on the assessed TripleStores.

**Table 1:** Results of scoring for assessed TripleStores.


|                | Functionalities | Performance | Management | Total |
|----------------|-----------------|-------------|------------|-------|
| AllegroGraph   | 2.87            | 3.68        | 3.67       | 3.27  |
| Amazon Neptune | 2.86            | 2.95        | 3.0        | 2.89  |
| Blazegraph     | 2.58            | 3.32        | 3.0        | 2.97  |
| Corese         | 2.09            | 2.2         | **4.87**       | 3.14  |
| Jena TDB       | 3.53            | 3.2         | **4.87**       | **3.82**  |
| Neo4J          | 2.12            | 3.44        | 3.27       | 2.83  |
| Stardog        | **3.75**            | **4.41**        | 2.47       | 3.55  |
| Virtuoso       | 3.16            | 3.89        | 3.27       | 3.4   |

Those results are obtained from carrying out:

-   243 assessments, based on

-   141 references

Appendix 1: Criteria in RDF
===========================

\@prefix : \<http://datascienceinstitute.ie/asio/criteria/\> .

\@prefix asio: \<http://datascienceinstitute.ie/asio/\> .

\@prefix dc: \<http://purl.org/dc/terms/\> .

\@prefix rdfs: \<http://www.w3.org/2000/01/rdf-schema\#\> .

\@prefix skos: \<http://www.w3.org/2004/02/skos/core\#\> .

\@prefix xsd: \<http://www.w3.org/2001/XMLSchema\#\> .

\@base \<http://datascienceinstitute.ie/asio/criteria/\> .

\#\# High-level criterion categories

:F a asio:CriterionCategory

; rdfs:label \"Functionalities\"\@en

; rdfs:comment \"Criteria concerning the satisfaction of typical
functional requirements of triple stores\"\@en

.

:P a asio:CriterionCategory

; rdfs:label \"Performance and scalability\"\@en

; rdfs:comment \"Criteria concerning the satisfaction of typical
performance requirements of triple stores\"\@en

; asio:weight \<http://datascienceinstitute.ie/asio/criteria/P/weight\>

.

:M a asio:CriterionCategory

; rdfs:label \"Management and maintenance\"\@en

; rdfs:comment \"Criteria concerning the manageability of triple stores
over time\"\@en

; asio:weight \<http://datascienceinstitute.ie/asio/criteria/M/weight\>

.

\#\# Criterion subcategories

:F1 a asio:CriterionCategory

; rdfs:label \"Base handling of RDF and SPARQL\"\@en

; rdfs:comment \"Basic functions of a triple store are considered
necessary, i.e., failing in this category would rule the
triplestore/graphstore out. A score of 5 conresponds to a full
implementation of the SPARQL 1.1 query language and protocol and CRUD
handling of RDF in the most common serialisations. A score of 0
correspond to no support of SPARQL or RDF. Scores in between correspond
to incomplete implementations of the SPARQL specifications.\"\@en

; skos:broader :F

; asio:weight \"5.0\"\^\^xsd:decimal

.

:F2 a asio:CriterionCategory

; rdfs:label \"Extensions\"\@en

; rdfs:comment \"Extensions are functionalities that are not considered
core to the handling of RDF/SPARQL linked data, but would provide useful
addition. The score depends on the number of extension relevant to the
scenario being considered.\"\@en

; skos:broader :F

; asio:weight \"3.0\"\^\^xsd:decimal

.

:F3 a asio:CriterionCategory

; rdfs:label \"Security\"\@en

; rdfs:comment \"The handling of security for any information system is
critical. Triplestore/graphstores might include their own security
mechanisms, or those might have to be handled separately. A score of 0
means that no security mechanism is provided and a score of 5 would be
given to a triple store handling fine-grained, customisable
authorisation and authentication, as well as secured communication
protocols.\"\@en

; skos:broader :F

; asio:weight \"4.0\"\^\^xsd:decimal

.

:P1 a asio:CriterionCategory

; rdfs:label \"Query response time\"\@en

; rdfs:comment \"Query response time corresponds to the time between
when a query is submitted to the system and when the results are
obtained. Scoring here is based on comparing average results over a
large variety of queries on several sizes of datasets.\"\@en

; skos:broader :P

; asio:weight \"5.0\"\^\^xsd:decimal

.

:P2 a asio:CriterionCategory

; rdfs:label \"Loading and update time\"\@en

; rdfs:comment \"Loading and update time, since it does not affect the
end user directly, is less prominent than query response time. However,
it might affect the performance of the system overall. Scoring base on
comparing averages times over a number of datasets of varying
sizes.\"\@en

; skos:broader :P

; asio:weight \"2.0\"\^\^xsd:decimal

.

:P3 a asio:CriterionCategory

; rdfs:label \"Memory footprint\"\@en

; rdfs:comment \"The amount of memory a system requires to run is
critical as it not only represents an indicator of the amount of
computing resources required to run the system, but also because it
affects the overall performance of the system. Scoring here is based on
measuring memory consumption on an idle system loaded with various
numbers of datasets of variying sizes.\"\@en

; skos:broader :P

; asio:weight \"4.0\"\^\^xsd:decimal

.

:P4 a asio:CriterionCategory

; rdfs:label \"Robustness and Scaling\"\@en

; rdfs:comment \"Besides pure runtime performance, reliability and the
ability to scale the system up are critical. Crashes, downtimes, and
performance decreases can have a very negative effect on the end user.
Scoring here is based on assessing publicly available forums for
reported issues.\"\@en

; skos:broader :P

; asio:weight \"4.0\"\^\^xsd:decimal

.

:M1 a asio:CriterionCategory

; rdfs:label \"Costs and rights\"\@en

; rdfs:comment \"The cost of obtaining the system is an obvious criteria
for the choice of the system, with many systems being part of a
commercial offering, others being free and open source, with hybrid
models. A score of 0 would be assigned to an expensive system with no
option but to acquire it. A score of 5 is given to a system with with
only a fully free option.\"\@en

; skos:broader :M

; asio:weight \"5.0\"\^\^xsd:decimal

.

:M2 a asio:CriterionCategory

; rdfs:label \"Ease of deployment\"\@en

; rdfs:comment \"The effort required in deployment is important,
especially has it might have to be realised by administrators with
limited experience in triplestores / graphstores. A score of 5 would be
given to a triplestore that can be installed in a few minutes, without
requiring skills beyond the use of a PC. A score of 0 is given to a
triplestore requiring a complex procedure, specialised equipment and
specialised skills.\"\@en

; skos:broader :M

; asio:weight \"2.0\"\^\^xsd:decimal

.

:M3 a asio:CriterionCategory

; rdfs:label \"Administration, updates and maintenance\"\@en

; rdfs:comment \"The effort required in keeping the system running is a
key part of the cost of the system. There are many factors that affect
the amount of efforts required to keep the system updated and
maintained, which are evaluated differently. A score of 5 would be given
to a system requiring no attention at all, while a score of 0 would be
given to a system requiring daily maintenance tasks.\"\@en

; skos:broader :M

; asio:weight \"5.0\"\^\^xsd:decimal

.

\#\# Criteria

:F1\_1 a asio:Criterion

; rdfs:label \"Ability to load and serve RDF\"\@en

; dc:description \"Can load RDF in its standard serialisations, using
standard interfaces, efficiently. Will provide RDF as results of queries
in standard serialisations effectively. Scoring depends on how much the
triple store can deal with various serialisations of RDF.\"\@en

; dc:subject :F1

; asio:weight \"5.0\"\^\^xsd:decimal

.

:F1\_2 a asio:Criterion

; rdfs:label \"SPARQL 1.1 compliance\"\@en

; dc:description \"Provide SPARQL endpoints using the SPARQL protocol
and a complete implementation of the SPARQL 1.1 query language,
including support for multiple languages and character encodings. Scores
depends on SPARQL 1.1. level of SPARQL 1.1 compliance.\"\@en

; dc:subject :F1

; asio:weight \"5.0\"\^\^xsd:decimal

.

:F1\_3 a asio:Criterion

; rdfs:label \"SPARQL update compliance\"\@en

; dc:description \"Provide SPARQL Update endpoints compliant with the
SPARQL Update protocol. Assuming that, in the absence of SPARQL update,
other approaches can be used to update the data, this is seen as less
important than the previous functions. A score of 0 means no support for
SPARQL update. A score of 5 means a fully compliant support for SPARQL
update query and protocol.\"\@en

; dc:subject :F1

; asio:weight \"3.0\"\^\^xsd:decimal

.

:F2\_1 a asio:Criterion

; rdfs:label \"Full-text search\"\@en

; dc:description \"Provides a full-text index of the data, and can make
full-text queries (e.g. as part of SPARQL queries). As this is a common
requirement in many scenarios, the weight of this function is high. A
score of 0 means no support for full-test search, and a score of 5 full,
and efficient support for full-text search integrated with SPARQL.\"\@en

; dc:subject :F2

; asio:weight \"5.0\"\^\^xsd:decimal

.

:F2\_2 a asio:Criterion

; rdfs:label \"GeoSPARQL\"\@en

; dc:description \"Compliant with the GeoSPARQL specification, providing
SPARQL construct to query data based on geographical locations. As this
is a less common requirements, which can be addressed through dedicated
SPARQL queries, this is seen as less critical. A score of 5 would mean a
fully compliant implementation of GeoSparql.\"\@en

; dc:subject :F2

; asio:weight \"2.0\"\^\^xsd:decimal

.

:F2\_3 a asio:Criterion

; rdfs:label \"SPARQL query federation (SERVICE clause)\"\@en

; dc:description \"Compliant with SPARQL query federation through
providing the SERVICE clause. As implementations of this tend to be
inefficient, and often not needed if data distribution can be achieved
in another way, this is seen as a weak requirement. A score of 5 means a
fully-compliant support for the SERVICE clause in SPARQL.\"\@en

; dc:subject :F2

; asio:weight \"2.0\"\^\^xsd:decimal

.

:F2\_4 a asio:Criterion

; rdfs:label \"SPARQL query federation (automatic handling of data
distribution)\"\@en

; dc:description \"The SERVICE clause described above requires knowledge
of where the data is distributed. A system able to automatically handle
data distribution and federate queries over distributed stores
transparently would be an advantage without being absolutely required. A
score of 5 would mean ability to transparently federate multiple
stores.\"\@en

; dc:subject :F2

; asio:weight \"2.0\"\^\^xsd:decimal

.

:F2\_5 a asio:Criterion

; rdfs:label \"Reasoning (RDFS entailment)\"\@en

; dc:description \"Being able to enable basic taxonomy-based reasoning
over RDFS ontologies. This is the simplest level of reasoning, which is
often sufficient in simple use cases. A Score of 0 means no support for
RDF reasoning can be enabled, while a score of 5 means native support
for RDF reasoning.\"\@en

; dc:subject :F2

; asio:weight \"5.0\"\^\^xsd:decimal

.

:F2\_6 a asio:Criterion

; rdfs:label \"Reasoning (\'Basic\' OWL profiles)\"\@en

; dc:description \"The simplest OWL profiles (EL, QL) are made to
represent classes of language that enable efficient reasoning. In cases
where reasoning is useful, adhering to those profile gives some
assurance that performance will not be too degraded. Those can therefore
be useful in a broader, but slightly more complex use cases. A score of
0 implies no support for OWL reasoning, while a score of 5 means native
support for OWL reasoning.\"\@en

; dc:subject :F2

; asio:weight \"4.0\"\^\^xsd:decimal

.

:F2\_7 a asio:Criterion

; rdfs:label \"Reasoning (DL-based OWL profiles)\"\@en

; dc:description \"The more advanced profiles of OWL require reasoners
based on description logics, which can be very complex and
resource-consuming, especially for large knowledge bases. They address a
small number of more advanced use cases. A score of 5 is obtained if a
description logic reasoner is embedded in the triple-store. A score of 0
is obtained if one cannot be integrated.\"\@en

; dc:subject :F2

; asio:weight \"2.0\"\^\^xsd:decimal

.

:F2\_8 a asio:Criterion

; rdfs:label \"Custom inference rules\"\@en

; dc:description \"Ability to implement custom inference rules,
including Jena-like rules, SWRL, or others. A score of 5 means that
there is an integrated way to define rules that apply on the datasets of
the triplestore.\"\@en

; dc:subject :F2

; asio:weight \"3.0\"\^\^xsd:decimal

.

:F2\_9 a asio:Criterion

; rdfs:label \"Method for RDF validation and close world
assumption\"\@en

; dc:description \"Provide support for, for example, SHACL. A score of 5
would mean full support for SHACL or other similar approaches to
validation. Lower score mean no or limited/adhoc approaches to
validation.\"\@en

; dc:subject :F2

; asio:weight \"3.0\"\^\^xsd:decimal

.

\# MDA: I believe this one should be removed or made more precise
depending on requirements.

:F2\_10 a asio:Criterion

; rdfs:label \"Programmatic access through other RDF standard APIs\"\@en

; dc:description \"Other programming interfaces exist for RDF data (e.g.
RDF4J) and might be available to the system.\"\@en

; dc:subject :F2

; asio:weight \"2.0\"\^\^xsd:decimal

.

\# MDA: As above.

:F2\_11 a asio:Criterion

; rdfs:label \"Programmatic access through other database access
methods\"\@en

; dc:description \"The system provides access to the data through
standard database access method (e.g. JDBC, ODBC).\"\@en

; dc:subject :F2

; asio:weight \"1.0\"\^\^xsd:decimal

.

:F2\_12 a asio:Criterion

; rdfs:label \"Named Entity recognition\"\@en

; dc:description \"Ability to recognise entities from text. Since there
is currently not standard for this, a score of 5 means an integrated
approach to perform named entity recognition in whatever form it
appears.\"\@en

; dc:subject :F2

; asio:weight \"3.0\"\^\^xsd:decimal

.

\# MDA: I\'m not very clear on this one\...

:F2\_13 a asio:Criterion

; rdfs:label \"Handling of property graphs\"\@en

; dc:description \"The system might include the ability to handle
property graphs through specific functions.\"\@en

; dc:subject :F2

; asio:weight \"2.0\"\^\^xsd:decimal

.

:F2\_14 a asio:Criterion

; rdfs:label \"Other data formats on ingest\"\@en

; dc:description \"The system might provide facilities to load and
transform into RDF data from other formats (e.g. CSV/TSV, XML, JSON,
RDBs through "virtual graph" system). A score of 5 means the ability to
load data from a large variety of common formats.\"\@en

; dc:subject :F2

; asio:weight \"1.0\"\^\^xsd:decimal

.

:F3\_1 a asio:Criterion

; rdfs:label \"Data encryption\"\@en

; dc:description \"This relates to whether the system provides options
to encrypt the data in the store, and the strength of the encryption.
Score is 0 if data is kept in a clear format and not encrypted at
all.\"\@en

; dc:subject :F3

; asio:weight \"2.0\"\^\^xsd:decimal

.

:F3\_2 a asio:Criterion

; rdfs:label \"Role-based access control\"\@en

; dc:description \"This relates to the ability to restrict access to the
data based on the role of the user. The level at which access control is
provided (graph, entities, individual triples, custom) affects the score
for this criterion. A score of 0 means that role-nased data access
control is enable with fine-grained permissions.\"\@en

; dc:subject :F3

; asio:weight \"5.0\"\^\^xsd:decimal

.

:F3\_3 a asio:Criterion

; rdfs:label \"Attribute-based access control\"\@en

; dc:description \"This relates to the ability to restrict access to the
data based on custom policies related to attributes of the user. The
level at which access control is provided (graph, entities, individual
triples, custom) affects the score for this criterion.\"\@en

; dc:subject :F3

; asio:weight \"3.0\"\^\^xsd:decimal

.

:F3\_4 a asio:Criterion

; rdfs:label \"Usage quotas\"\@en

; dc:description \"The system might provide a way for the administrator
to impose quotas on the use of the system (e.g. amount of triples read
in a given period of time), to avoid overload. This is scored 5 if
quotas can be highly customised.\"\@en

; dc:subject :F3

; asio:weight \"2.0\"\^\^xsd:decimal

.

:P1\_1 a asio:Criterion

; rdfs:label \"Tens of thousands of triples\"\@en

; dc:description \"Average query response time on datasets with tens of
thousands of RDF triples.\"\@en

; dc:subject :P1

; asio:weight \"5.0\"\^\^xsd:decimal

.

:P1\_2 a asio:Criterion

; rdfs:label \"Hundreds of thousands of triples\"\@en

; dc:description \"Average query response time on datasets with hundreds
of thousands of RDF triples.\"\@en

; dc:subject :P1

; asio:weight \"5.0\"\^\^xsd:decimal

.

:P1\_3 a asio:Criterion

; rdfs:label \"Millions of triples\"\@en

; dc:description \"Average query response time on datasets with millions
of RDF triples.\"\@en

; dc:subject :P1

; asio:weight \"4.0\"\^\^xsd:decimal

.

:P1\_4 a asio:Criterion

; rdfs:label \"Tens of millions of triples\"\@en

; dc:description \"Average query response time on datasets with tens of
millions of RDF triples.\"\@en

; dc:subject :P1

; asio:weight \"3.0\"\^\^xsd:decimal

.

:P1\_5 a asio:Criterion

; rdfs:label \"Hundreds of millions of triples\"\@en

; dc:description \"Average query response time on datasets with hundreds
of millions of RDF triples.\"\@en

; dc:subject :P1

; asio:weight \"2.0\"\^\^xsd:decimal

.

:P1\_6 a asio:Criterion

; rdfs:label \"Billions of triples\"\@en

; dc:description \"Average query response time on datasets with billions
of RDF triples.\"\@en

; dc:subject :P1

; asio:weight \"1.0\"\^\^xsd:decimal

.

:P2\_1 a asio:Criterion

; rdfs:label \"Batch loading\"\@en

; dc:description \"Batch loading is the insertion of large amounts of
triples into the system, as a background process. This is scored based
on the average time of loading for datasets of various times.\"\@en

; dc:subject :P2

; asio:weight \"5.0\"\^\^xsd:decimal

.

:P2\_2 a asio:Criterion

; rdfs:label \"Insert\"\@en

; dc:description \"Insert corresponds to adding triples on the basis of
specific conditions (using SPARQL update). Scored based on the average
number of triples inserted per minutes.\"\@en

; dc:subject :P2

; asio:weight \"4.0\"\^\^xsd:decimal

.

:P2\_3 a asio:Criterion

; rdfs:label \"Clear\"\@en

; dc:description \"Clear corresponding to dropping a whole graph of the
whole of the data in the store (possibly to overnight it with new data).
Average time of clearing datasets of different sizes\"\@en

; dc:subject :P2

; asio:weight \"3.0\"\^\^xsd:decimal

.

:P2\_4 a asio:Criterion

; rdfs:label \"Delete\"\@en

; dc:description \"Delete corresponds to removing specific triples based
on specific conditions (using SPARQL update). This is scored based on
the average number of triples deleted per minutes.\"\@en

; dc:subject :P2

; asio:weight \"2.0\"\^\^xsd:decimal

.

:P3\_1 a asio:Criterion

; rdfs:label \"Tens of thousands of triples\"\@en

; dc:description \"Amount of memory allocated to the system when
querying datasets with tens of thousands of RDF triples.\"\@en

; dc:subject :P3

; asio:weight \"5.0\"\^\^xsd:decimal

.

:P3\_2 a asio:Criterion

; rdfs:label \"Hundreds of thousands of triples\"\@en

; dc:description \"Amount of memory allocated to the system when
querying datasets with hundreds of thousands of RDF triples.\"\@en

; dc:subject :P3

; asio:weight \"5.0\"\^\^xsd:decimal

.

:P3\_3 a asio:Criterion

; rdfs:label \"Millions of triples\"\@en

; dc:description \"Amount of memory allocated to the system when
querying datasets with millions of RDF triples.\"\@en

; dc:subject :P3

; asio:weight \"4.0\"\^\^xsd:decimal

.

:P3\_4 a asio:Criterion

; rdfs:label \"Tens of millions of triples\"\@en

; dc:description \"Amount of memory allocated to the system when
querying datasets with tens of millions of RDF triples.\"\@en

; dc:subject :P3

; asio:weight \"3.0\"\^\^xsd:decimal

.

:P3\_5 a asio:Criterion

; rdfs:label \"Hundreds of millions of triples\"\@en

; dc:description \"Amount of memory allocated to the system when
querying datasets with hundreds of millions of RDF triples.\"\@en

; dc:subject :P3

; asio:weight \"2.0\"\^\^xsd:decimal

.

:P3\_6 a asio:Criterion

; rdfs:label \"Billions of triples\"\@en

; dc:description \"Amount of memory allocated to the system when
querying datasets with billions of RDF triples.\"\@en

; dc:subject :P3

; asio:weight \"1.0\"\^\^xsd:decimal

.

:P4\_1 a asio:Criterion

; rdfs:label \"Robustness / reliability\"\@en

; dc:description \"Robustness and reliability are calculated by running
complex, notoriously challenging queries with large result sets,
stress-testing the system. This scored based on assessment of requests
and issue reports on public forums.\"\@en

; dc:subject :P4

; asio:weight \"5.0\"\^\^xsd:decimal

.

:P4\_2 a asio:Criterion

; rdfs:label \"Transaction and rollback mechanism\"\@en

; dc:description \"The system provides a journal of update transactions,
which can be used to rollback changes. This scores 0 if a full journal
mechanism with ACID transactions is provided.\"\@en

; dc:subject :P4

; asio:weight \"3.0\"\^\^xsd:decimal

.

:P4\_3 a asio:Criterion

; rdfs:label \"Vertical scalability\"\@en

; dc:description \"Vertical scalability is the ability of the system to
maximally utilise the resources available to it, and to expand to new
resources made available. Scored 0 if the system works at fixed scale.
\"\@en

; dc:subject :P4

; asio:weight \"2.0\"\^\^xsd:decimal

.

:P4\_4 a asio:Criterion

; rdfs:label \"Horizontal scalability (clustering)\"\@en

; dc:description \"Horizontal scalability is the ability of the system
to be distributed over multiple instances (e.g. in a cluster), with new
instances being added to enable increases in scale. This is scored 5 if
new instances can be added at run-time, preferably using standard
distribution architectures.\"\@en

; dc:subject :P4

; asio:weight \"3.0\"\^\^xsd:decimal

.

:M1\_1 a asio:Criterion

; rdfs:label \"Cost\"\@en

; dc:description \"Cost is calculated based on the price of the
software, considering the options that are most likely to be required.
Free and open source systems score 5.\"\@en

; dc:subject :M1

; asio:weight \"5.0\"\^\^xsd:decimal

.

:M1\_2 a asio:Criterion

; rdfs:label \"Open source licence\"\@en

; dc:description \"Open source licences can be more or less permissive,
include specific aspects that could restrict the use or distribution of
the system. A systems under commercial, non-free licence scores 0.\"\@en

; dc:subject :M1

; asio:weight \"2.0\"\^\^xsd:decimal

.

:M2\_1 a asio:Criterion

; rdfs:label \"Automatic installer\"\@en

; dc:description \"This criterion looks at whether an automatic
installer is available to get the system up and running.\"\@en

; dc:subject :M2

; asio:weight \"3.0\"\^\^xsd:decimal

.

:M2\_2 a asio:Criterion

; rdfs:label \"Complexity of configuration\"\@en

; dc:description \"This criterion measures the amount of effort required
to configure the system, including whether default configurations are
available, how valid they are, the number of changes required to be
made, and how complex those changes are.\"\@en

; dc:subject :M2

; asio:weight \"5.0\"\^\^xsd:decimal

.

:M2\_3 a asio:Criterion

; rdfs:label \"Quality of the documentation\"\@en

; dc:description \"This measures how much the system can be setup in
common scenarios following the documentation, and how much the
documentation address possible problems in installation.\"\@en

; dc:subject :M2

; asio:weight \"4.0\"\^\^xsd:decimal

.

:M2\_4 a asio:Criterion

; rdfs:label \"Dependencies\"\@en

; dc:description \"The system might require other libraries or systems
to be installed in order to run, mich might increase the cost and make
deployment more complicated.\"\@en

; dc:subject :M2

; asio:weight \"3.0\"\^\^xsd:decimal

.

:M2\_5 a asio:Criterion

; rdfs:label \"Released as a container\"\@en

; dc:description \"The system is available as a (Docker) container,
ready to be deployed.\"\@en

; dc:subject :M2

; asio:weight \"2.0\"\^\^xsd:decimal

.

:M3\_1 a asio:Criterion

; rdfs:label \"Quality of the documentation\"\@en

; dc:description \"As for the same criterion in relation to deployment,
the quality of the documentation here relates to how complete and clear
the documentation is in relation to realising basic maintenance
operations and resolving problems.\"\@en

; dc:subject :M3

; asio:weight \"4.0\"\^\^xsd:decimal

.

:M3\_2 a asio:Criterion

; rdfs:label \"Support services\"\@en

; dc:description \"This relates to whether the cost of the system
includes customer support services from the system provider.\"\@en

; dc:subject :M3

; asio:weight \"4.0\"\^\^xsd:decimal

.

\# redundant with M3\_4??

:M3\_3 a asio:Criterion

; rdfs:label \"Active development\"\@en

; dc:description \"This relates to how much the system is being
developed, whether by the company providing it or by the open source
community. A score of 0 means a system no longer supported.\"\@en

; dc:subject :M3

; asio:weight \"5.0\"\^\^xsd:decimal

.

:M3\_4 a asio:Criterion

; rdfs:label \"Update frequency\"\@en

; dc:description \"This relates to how often the system is being updated
by the provider, whether for additional features, bug fixes, etc.\"\@en

; dc:subject :M3

; asio:weight \"3.0\"\^\^xsd:decimal

.

:M3\_5 a asio:Criterion

; rdfs:label \"Automatic update\"\@en

; dc:description \"This relates to whether update are automatic, or
automatically deployed.\"\@en

; dc:subject :M3

; asio:weight \"3.0\"\^\^xsd:decimal

.

:M3\_6 a asio:Criterion

; rdfs:label \"Downtime required\"\@en

; dc:description \"This relates to whether downtime from the system is
required for updates.\"\@en

; dc:subject :M3

; asio:weight \"4.0\"\^\^xsd:decimal

.

:M3\_7 a asio:Criterion

; rdfs:label \"Backup mechanism\"\@en

; dc:description \"A preferably automatic mechanism is available to make
regular backups of the data.\"\@en

; dc:subject :M3

; asio:weight \"3.0\"\^\^xsd:decimal

.

:M3\_7 a asio:Criterion

; rdfs:label \"Monitoring\"\@en

; dc:description \"Interfaces to monitor the state (health, data) of the
system are available.\"\@en

; dc:subject :M3

; asio:weight \"2.0\"\^\^xsd:decimal

.

:M3\_8 a asio:Criterion

; rdfs:label \"Availability of a strong community of users\"\@en

; dc:description \"A large community of users might help with easy of
use and administration, by providing a knowledge base of problems and
solutions. This can be evaluated in part by using the db-engines
ranking. This is scored based on an assessment of amount of activity in
public forums.\"\@en

; dc:subject :M3

; asio:weight \"2.0\"\^\^xsd:decimal

.

:M3\_9 a asio:Criterion

; rdfs:label \"Available as a cloud service\"\@en

; dc:description \"There is an option to use the system as a cloud
service (DBaaS), rather than deploying it locally. This scores 5 if a
directly-subscribable service is available based on this system.\"\@en

; dc:subject :M3

; asio:weight \"1.0\"\^\^xsd:decimal

.

 

Appendix 2: Criterion Ontology
==============================

\@prefix : \<http://datascienceinstitute.ie/asio/schema\#\> .

\@prefix dc: \<http://purl.org/dc/terms/\> .

\@prefix owl: \<http://www.w3.org/2002/07/owl\#\> .

\@prefix rdf: \<http://www.w3.org/1999/02/22-rdf-syntax-ns\#\> .

\@prefix xml: \<http://www.w3.org/XML/1998/namespace\> .

\@prefix xsd: \<http://www.w3.org/2001/XMLSchema\#\> .

\@prefix asio: \<http://datascienceinstitute.ie/asio/\> .

\@prefix doap: \<http://usefulinc.com/ns/doap\#\> .

\@prefix foaf: \<http://xmlns.com/foaf/0.1/\> .

\@prefix rdfs: \<http://www.w3.org/2000/01/rdf-schema\#\> .

\@prefix skos: \<http://www.w3.org/2004/02/skos/core\#\> .

\@prefix provo: \<http://www.w3.org/ns/prov\#\> .

\@base \<http://datascienceinstitute.ie/asio/schema\> .

\<http://datascienceinstitute.ie/asio/schema\> rdf:type owl:Ontology ;

rdfs:comment \"Ontological schema for defining the benchmarking criteria
of RDF and graph stores.\"\@en .

\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#

\# Object Properties

\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#

\#\#\# http://datascienceinstitute.ie/asio/benchmakedIn

asio:benchmakedIn rdf:type owl:ObjectProperty ;

rdfs:domain asio:TripleStoreRelease ;

rdfs:range provo:Entity ;

rdfs:label \"benchmarked in\"\@en .

\#\#\# http://datascienceinstitute.ie/asio/criterion

asio:criterion rdf:type owl:ObjectProperty ;

rdfs:domain asio:CriterionAssessment ;

rdfs:range asio:Criterion ;

rdfs:label \"criterion\"\@en .

\#\#\# http://datascienceinstitute.ie/asio/isAssessedWith

asio:isAssessedWith rdf:type owl:ObjectProperty ;

rdfs:domain doap:Version ;

rdfs:range asio:CriterionAssessment ;

rdfs:label \"is assessed with\"\@en .

\#\#\# http://purl.org/dc/terms/subject

dc:subject rdf:type owl:ObjectProperty .

\#\#\# http://usefulinc.com/ns/doap\#release

doap:release rdf:type owl:ObjectProperty .

\#\#\# http://www.w3.org/ns/prov\#primarySource

provo:primarySource rdf:type owl:ObjectProperty .

\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#

\# Data properties

\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#

\#\#\# http://datascienceinstitute.ie/asio/value

asio:value rdf:type owl:DatatypeProperty ;

rdfs:domain asio:CriterionAssessment ;

rdfs:range xsd:integer ;

rdfs:comment \"value\"\@en .

\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#

\# Classes

\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#\#

\#\#\# http://datascienceinstitute.ie/asio/Criterion

asio:Criterion rdf:type owl:Class ;

rdfs:subClassOf \[ rdf:type owl:Restriction ;

owl:onProperty dc:subject ;

owl:someValuesFrom asio:CriterionCategory

\] ;

rdfs:label \"criterion\"\@en .

\#\#\# http://datascienceinstitute.ie/asio/CriterionAssessment

asio:CriterionAssessment rdf:type owl:Class ;

rdfs:subClassOf \[ rdf:type owl:Restriction ;

owl:onProperty provo:primarySource ;

owl:someValuesFrom provo:Entity

\] ;

rdfs:label \"criterion assessment\"\@en .

\#\#\# http://datascienceinstitute.ie/asio/CriterionCategory

asio:CriterionCategory rdf:type owl:Class ;

rdfs:subClassOf skos:Concept .

\#\#\# http://datascienceinstitute.ie/asio/TripleStore

asio:TripleStore rdf:type owl:Class ;

rdfs:subClassOf doap:Project ,

\[ rdf:type owl:Restriction ;

owl:onProperty doap:release ;

owl:allValuesFrom asio:TripleStoreRelease

\] ;

rdfs:comment \"\"\"A software project that provides RDF triple/quad
storage capabilities.

At the moment this notion is strict: a software system is a triple store
iff it was born a triple store, i.e. all its releases are triple store
releases. This excludes databases (e.g. Oracle DB since release 18c)
that grew to incorporate support for RDF.\"\"\"\@en ;

rdfs:label \"triple store\"\@en .

\#\#\# http://datascienceinstitute.ie/asio/TripleStoreRelease

asio:TripleStoreRelease rdf:type owl:Class ;

rdfs:subClassOf doap:Version ;

rdfs:label \"triple store release\"\@en .

\#\#\# http://usefulinc.com/ns/doap\#Project

doap:Project rdf:type owl:Class .

\#\#\# http://usefulinc.com/ns/doap\#Version

doap:Version rdf:type owl:Class .

\#\#\# http://www.w3.org/2004/02/skos/core\#Concept

skos:Concept rdf:type owl:Class .

\#\#\# http://www.w3.org/ns/prov\#Entity

provo:Entity rdf:type owl:Class .

\#\#\# Generated by the OWL API (version 4.5.9.2019-02-01T07:24:44Z)
https://github.com/owlcs/owlapi

[^1]: Rdf spec

[^2]: Sparql spec

[^3]: Criterion Design Pattern

[^4]: DOAP

[^5]: ProvO

[^6]: Available at
    [[http://tsa.datascienceinstitute.ie]{.underline}](http://tsa.datascienceinstitute.ie)
