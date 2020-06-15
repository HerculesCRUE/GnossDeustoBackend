![](../Docs/media/CabeceraDocumentosMD.png)

# ASIO Validation Questions

In this project different SPARQL queries are executed in order to test the ROH ontology. Those validations questions are gathered from: https://github.com/HerculesCRUE/GnossDeustoOnto/blob/master/Documentation/2-%20CoberturaPreguntasCompetencia.pdf

* [Documentation](https://deustohercules.github.io/validation-questions/testapidocs/index.html)
* [Test Results](https://deustohercules.github.io/validation-questions/surefire-report.html)

You can find more details about the questions and run them interactively at `sparql-query` directory.


# USAGE

Asumptions: it requires to have installed the following tools:
- git client
- mvn tool
- jdk 8.0

If you do not want to install neither Java or Maven in your system, please go to section entitled Docker, where how to lauch a container including these tools is explained. 

Download the repository and cd to validation-questions path:

```
$ git clone https://github.com/HerculesCRUE/GnossDeustoBackend
$ cd GnossDeustoBackend/validation-questions
```

Add required submodules, GnossDeustoOnto (ontology) and pellet (reasoner):

```
$ git submodule add https://github.com/HerculesCRUE/GnossDeustoOnto
$ git submodule add https://github.com/stardog-union/pellet/
$ git submodule init
$ git submodule update
```

Install pellet reasoner:

```
$ cd pellet
$ mvn install
```

Go back and execute tests:

```
$ cd ..
$ mvn compile
$ mvn test -DontFile=GnossDeustoOnto/roh-v2.owl -DdataFile=GnossDeustoOnto/examples/data.ttl  -Duneskos=GnossDeustoOnto/unesco-individuals.rdf
```

# Docker

If you want to create your Java environment at Docker, you could follow next steps:

Download the repository and cd to validation-questions path:

```
$ git clone https://github.com/HerculesCRUE/GnossDeustoBackend
$ cd GnossDeustoBackend/validation-questions
```

Add required submodules, GnossDeustoOnto (ontology) and pellet (reasoner):

```
$ git submodule add https://github.com/HerculesCRUE/GnossDeustoOnto
$ git submodule add https://github.com/stardog-union/pellet/
$ git submodule init
$ git submodule update
```

Create a Docker container:

```
cd </path/to/GnossDeustoBackend/validation-questions>
$ docker run --rm -ti -v ${PWD}:/source maven:3-jdk-8 /bin/bash
```

In Windows, you may use the following command instead:

```
$ docker run --rm -ti -v %cd%:/source maven:3-jdk-8 /bin/bash
```

From the container, execute the following commands to install pellet:

```
$ cd /source/pellet
$ mvn install
```

Go back and execute tests:

```
$ cd ..
$ mvn compile
$ mvn test -DontFile=GnossDeustoOnto/roh-v2.owl -DdataFile=GnossDeustoOnto/examples/data.ttl  -Duneskos=GnossDeustoOnto/unesco-individuals.rdf
```


