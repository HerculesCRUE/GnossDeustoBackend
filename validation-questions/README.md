# ASIO Validation Questions

In this project different SPARQL queries are executed in order to test the ROH ontology. Those validations questions are gathered from: https://github.com/HerculesCRUE/GnossDeustoOnto/raw/master/Documentation/CoverturaPreguntasCompetencia.pdf

* [Documentation](https://deustohercules.github.io/validation-questions/testapidocs/index.html)
* [Test Results](https://deustohercules.github.io/validation-questions/surefire-report.html)


# USAGE

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
$ docker run --rm -ti -v </path/to/GnossDeustoBackend/validation-questions>:/source maven:3-jdk-8 /bin/bash
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


