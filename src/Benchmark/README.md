![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 26/4/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Benchmark RDF Store| 
|Descripción|Desarrollo del módulo de benchmark de RDF Store|
|Versión|1.3|
|Módulo|Documentación|
|Tipo|Manual|
|Cambios de la Versión|Reordenación de la documentación y cambio de ruta|

## Benchmark RDF Store

El informe sobre el Benchmark se encuentra en la [carpeta Docs](../../Docs) general del Backend:

[Hércules Backend ASIO. TripleStore Benchmark deliverable report](../../Docs/Hercules-TripleStore-Benchmark-deliverable-report.md)

En la carpeta [Docs del Benchmark](../../Docs/Benchmark/) se pueden consultar los siguientes documentos:

[Guía del usuario](../../Docs/Benchmark/UserGuide.md) (en inglés).

[Documentación del desarrollador](../../Docs/Benchmark/Developer-Documentation.md) (en inglés). Este documento explica como añadir, editar o eliminar criterios.

La aplicación de Benchmark está desplegada en:

https://herc-as-front-desa.atica.um.es/benchmark

This folder includes the code, ontologies and data for the triple store assessement tool, including:

- [criteria ontology](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/src/Benchmark/criterion-ontology): base ontology to represent projects, criteria and assessements, as well as representation of the assessements
- [triplestore dataset](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/src/Benchmark/triplestore-dataset): base dataset of information about the triple stores assessed.
- [triplestore assessments](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/src/Benchmark/triplestore-assessments): representation and data of the assessement of triplestores according to criteria ontology
- [triplestore assessment interface](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/src/Benchmark/triplestore-assessment-interface): interface connecting to the data to display and explore the assessments

Data from the first three project should be loaded onto a triplestore providing a sparql interface (Fuseki was used during the development of the project). Instruction are provided to run the tool using that SPARQL endpoint.
