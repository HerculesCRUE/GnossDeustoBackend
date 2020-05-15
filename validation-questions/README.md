# ASIO Validation Questions

In this project different SPARQL queries are executed in order to test the ROH ontology. Those validations questions are gathered from: https://docs.google.com/spreadsheets/d/17y4mIlhFw691-Cv3AqMJGUqXgFEX-A2WZ7CdmZ9wdlY/edit#gid=0

* [Documentation](https://deustohercules.github.io/validation-questions/testapidocs/index.html)
* [Test Results](https://deustohercules.github.io/validation-questions/surefire-report.html)


# USAGE

```
$ git submodule init
$ git submodule update
$ cd pellet
$ mvn install
$ cd ..
$ mvn compile
$ mvn test -DontFile=<path/to/roh-v2.owl> -DdataFile=<path/to/examples/data.ttl>  -Duneskos=<path/to/roh/unesco-individuals.rdf>
```



