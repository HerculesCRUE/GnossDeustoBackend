![](.//media/CabeceraDocumentosMD.png)

| Fecha         | 01/10/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Hércules ASIO. Data confidentiality proposal| 
|Descripción|Proposal for the management of data confidentiality|
|Versión|1.1|
|Módulo|Data Load and SPARQL Enpoint|
|Tipo|Specification|
|Cambios de la Versión|Added a section to define the management of private attributes and entities|


# Hércules Backend ASIO. Data confidentiality proposal

[INTRODUCTION](#introduction)

[Requirements](#requirements)

[Data Confidentiality and privacy](#data-confidentiality-and-privacy)

[Identified Roles](#identified-roles)

[RDF Stores secutiry options](#rdf-stores-security-options)

[Data confidentiality management](#data-confidentiality-management)

[Confidential entities](#confidential-entities)

[Confidential attributes](#confidential-attributes)

[Confidential Attributes and Entities](confidential-attributes_and-entities)

INTRODUCTION
============

This document contains two confidentiality proposal for the Backend SGI
data of the Hercules ASIO project. On the one hand, we consider the functional 
requirements and theexpected roles, on the other, the security and privacy
limitations of the RDF stores.

The chosen proposal is the one based on the definition of [confidential entities](#confidential-entities).

This proposal will be applied later in the configuration and deployment of the
data retrieval interfaces, such as de Linked Data Server and the SPARQL Endpoint.

Requirements
============

Hercules Backend SGI will have two systems to access the data through
the public website:

-   A Linked Data Server (including the searching functionality), that
    would allow the access to the graph entities and the navigation
    trough their attributes.

-   An SPARQL Endpoint, that would allow the execution of arbitrary read
    queries over the graph triples.

Data Confidentiality and privacy
--------------------------------

The University should be able to define the data confidentiality and
privacy:

-   Each university could set the confidential fields from the dataset
    using metadata, that is, its input format will be predefined: SGI
    Hércules, Murcia university, CVN, etc.

-   The ETL interface would allow to mark the confidential data and
    assign permissions by role.

We must consider that the confidential data will be used for aggregated
queries so the restrictions should be applied just for entity
visualization.

Identified Roles 
-----------------

We have these roles:

-   Ministry, national agencies.

-   Regional governments.

-   Hércules Backend managers.

-   Universities

    -   System administrator for the initial data load.

    -   Data Manager for queries, who would have access to confidential
        data.

    -   Researcher, who would have access to her or his confidential
        data.

-   Public user.

We suppose these permissions:

|                              | Aggregated confidential data | Confidential data |
|------------------------------|------------------------------|-------------------|
| Ministry, national agencies  | Yes                          |                   |
| Regional government          | Yes                          |                   |
| Backend Manager              | Yes                          | Yes               |
| Universities                 |                              |                   |
|    System administrators     | Yes                          | Yes               |
|    Data manager              | Yes                          | Yes               |
|    Researcher                | Yes                          | Yes (own data)    |
| Public user                  | Yes                          |                   |

RDF Stores security options
===========================

These are the security options of the most popular RDF Store:

|             | Store security | Graph security | Triple security |
|-------------|----------------|----------------|-----------------|
| Marklogic   | X              | X              |                 |
| Virtuoso    | X              | X              |                 |
| Neptune     | X              |                |                 |
| GraphDB     | X              | X              |                 |
| AllegroGraf | X              | X              | X               |
| Stardog     | X              | X              |                 |

That is, only AllegroGraf would allow the confidentiality definition at
attribute level. According its documentation, we should consider that a
complex security schema would have some performance loss.

The coverage of complex security requirements has never been a priority
of the RDF Stores that are designed to support the fast load of big
amounts of data and to offer a good performance in complex read queries.

Data confidentiality management
===============================

It seems that it's not possible to fulfill the previous requirements.
Even with a triple level security (AllegroGraf) is not possible to allow
aggregated SPARQL queries that use a confidential data (e.g. the project
budget) and, at the same time, avoid that the use of the confidential
data in a query that retrieves only the triples of an entity: this could
be accomplished with an aggregated query that only retrieves a specific
entity.

In addition, the triple level security would restrict the RDF Stores to
just one system, AllegroGraf, that has commercial license.

We propose two solutions that meet part of the requirements, each with
its drawbacks:

-   Confidential entities, managing access to data access through a
    confidentiality schema (software).

-   Confidential attributes, managing access to data through a
    confidentiality contract (management and confidence).

Confidential entities 
----------------------

In this case the entity is entirely confidential but has some data (e.g.
the project budget) that could be used in arbitrary aggregated queries.
Suppose these data:

![](.//media/image2_DataConfidentiality.png)

The SPARQL Endpoint would always exclude the private graph for an
external data manager if the query asks for an attribute that allows a
human to identify the entity. In the example, this attribute would be
the project name. So, **this require a confidentiality schema that
defines which attributes would allow a person to know about a
confidential entity**.

Suppose the case of the external data manager of a national agency
running arbitrary queries:

-   If the query retrieves the sum of the budgets by month, the SPARQL
    Endpoint would include the private graph. The query would return the
    sum of P1, P2 and P3 budgets.

-   If the query retrieves the projects ordered by name, the SPARQL
    Endpoint would detect the use of the "Name" attribute and would
    exclude the private graph. In this case the query would only return
    the names of P1 and P2. The SPARQL Endpoint should inform the user
    that there are some private data that that cannot be showed.

So, in this case we would have these roles and permissions regarding the
use of aggregated queries in the SPARQL Endpoint:

-   System administrator. No limits.

-   External data manager. Public data and aggregated confidential data.

-   Public user. Public data.

The pros and cons of this proposal:

| Pros                                                                                                                                                   | Cons                                                                                                                                                                                                                  |
|--------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **SPARQL Endpoint**. The external data manager can run arbitrary aggregated queries over the public and private data without breaking the confidentiality. | **Complex confidentiality schema**. It’s not enough to define which attribute is confidential, the schema definition should contain the attributes that avoid the use of the private graph if they are used in the query. |
| **Confidentiality management**. It’s not necessary to manage the transfer of the confidential data to external data managers.                              | **Linked Data Server**. The public linked data server would not show data from the private graph, not even the non-confidential attributes.                                                                               |
|                                                                                                                                                        | **SPARQL Endpoint**. It isn’t possible to retrieve any data from an entity that has a confidential attribute except for aggregated queries.                                                                               |

Confidential attributes 
------------------------

If an entity has a confidential data (e.g. the project budget) then the
confidential attribute is confidential. Suppose these data:

![](.//media/image3_DataConfidentiality.png)

The SPARQL Endpoint would always include the private graph if the
external data manager has permission to query aggregated confidential
data.

In this case we would have these roles and permissions regarding the use
of aggregated queries in the SPARQL Endpoint:

-   System administrator. No limits.

-   External data manager. No limits.

-   Public user. No access to data in the private graph, neither for
    aggregate nor for normal queries. The SPARQL Endpoint should inform
    the user that there are some private data that she or he can't see.

The pros and cons of this proposal:

| Pros                                                                                                                                           | Cons                                                                                                                                               |
|------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------|
| **Linked Data Server**. The public linked data server would show the non-confidential attributes of the entities with some confidential attribute. | **Confidentiality management**. It’s necessary to manage the transfer of the confidential data to external data managers.                              |
| **Simple confidentiality schema**. It’s enough to define which attribute is confidential.                                                          | **SPARQL Endpoint**. The external data manager can run arbitrary aggregated queries over the public and private data but breaking the confidentiality. |
| **SPARQL Endpoint**. It’s possible to retrieve the non-confidential data from any entity.                                                          |                                                                                                                                                    |

Confidential Attributes and Entities 
--------------------
Finally, there are confidential entities but also confidential data. E.g. the budget of a project is a confidential attribute that can only be retrieved for aggregated queries. Suppose these data for project entities:

**Public Graph**
|ID|Name|Date|
|--|--|--|
|ID1|Name1|Date1|
|ID2|Name2|Date2|

**Private Data Graph**
| ID|Budget|
|--|--|
|ID1|Budget1|
|ID2|Budget2|
|ID3|Budget3|

**Private Entities Graph**
|ID|Name|Date|
|--|--|--|
|ID3|Name3|Date3|

The SPARQL Endpoint would answer this way for the External Data Manager:

 - If the query contains attributes marked as identifiers of the entity (as ID or Name for project), it doesn't retrieve data from the Private Data Graph.
 - If the query contains attributes marked as identifiers of the entity, it doesn't retrieve data from the Private Entities Graph.

So, in the example, an External Data Manaher could ask for the aggregated budget by month but not for the budget of ID2 project nor any data from ID3.

The permissions could be:
||Public|Private Data|Private entities|
|--|--|--|--|
|Administrator|X|X|X|
|External Data Manager|X|Aggregated|Aggregated |
|Public user|X| | |


