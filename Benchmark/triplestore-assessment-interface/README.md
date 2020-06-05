![](../../Docs/media/CabeceraDocumentosMD.png)

# Hercules triple store assessment interface

This Web application is a frontend for triple store assessment datasets. It provides a point of reference for the evaluation of RDF stores or other database systems that support RDF in some form.

At the moment, the Hercules app is provided as a [Flask](https://flask.palletsprojects.com) application on a standalone WSGI server. Alternative ways to deploy it will be provided soon.

## Requirements

* Python 3.6.6 or above
* A SPARQL endpoint Fuseki that publishes a dataset using the ASIO Criterion ontology.

## Build

1. Clone this repo
2. `python setup.py install`

## Run

1. Set the server port in [config.json](config.json) (defaults to `5000`)
2. Set the SPARQL endpoint in [hercules/settings.py](hercules/settings.py).
3. `python server.py`
4. Go to `localhost:{the_port}`

## Rights

### License

This software is distributed under the [GNU General Public License v3.0](https://www.gnu.org/licenses/gpl-3.0.en.html). See the [COPYING](COPYING) file for license details.

### Acknowledgement

This work is funded by the Hercules project, a collaboration between [GNOSS](https://www.gnoss.com) (RIAM INTELEARNING LAB S.L.), [Universidad de Murcia](https://www.um.es) and the [National University of Ireland Galway](https://nuigalway.ie).
