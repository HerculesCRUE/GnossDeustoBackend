El documento se encuentra en la [carpeta Docs](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Docs):

[Hércules Backend ASIO. TripleStore Benchmark deliverable report](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/20200325%20Hercules%20TripleStore%20Benchmark%20deliverable%20report.md)

This folder includes the code, ontologies and data for the triple store assessement tool, including:

- criteria ontology: base ontology to represent projects, criteria and assessements, as well as representation of the assessements
- triplestore dataset: base dataset of information about the triple stores assessed.
- triplestore assessments: representation and data of the assessement of triplestores according to criteria ontology
- triplestore assessment interface: interface connecting to the data to display and explore the assessments

Data from the first three project should be loaded onto a triplestore providing a sparql interface (Fuseki was used during the development of the project). Instruction are provided to run the tool using that SPARQL endpoint.
