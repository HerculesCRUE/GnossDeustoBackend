![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 19/05/2021                                                  |
| ------------- | ------------------------------------------------------------ |
|Titulo|Carga Dataset Murcia| 
|Descripción|Manual del servicio API CARGA|
|Versión|1|
|Módulo|Carga Dataset Murcia|
|Tipo|Manual|
|Cambios en la versión|Creación|

## Sobre Carga Dataset Murcia

[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)


Carga Dataset Murcia es una aplicación de consola encargada de la carga de los Dataset de Murcia a su RDF store

## Configuración en el appsettings.json

    { 
		"UrlUrisFactory": "https://localhost:44340/factory",
		"SparqlASIO_Graph": "http://xxxxxxx.um.es/graph/sgi",
		"SparqlASIO_1": {
			"Endpoint": "http://127.0.0.1:8890/sparql-auth/",
			"Username": "",
			"Password": ""
		},
		"SparqlASIO_2": {
			"Endpoint": "http://127.0.0.1:8890/sparql-auth/",
			"Username": "",
			"Password": ""
		}
    }
 - UrlUrisFactory: Url del controlador 'factory' dentro del servicio UrisFactory
 - SparqlASIO_Graph: Url del grafo en el que hay que realizar la carga
 - SparqlASIO_1: Configuración del Sparql endpoint 1
 - SparqlASIO_1.Endpoint: Url del Sparql endpoint
 - SparqlASIO_1.Username: Usuario del Sparql endpoint
 - SparqlASIO_1.Password: Password del Sparql endpoint
 - SparqlASIO_2: Configuración del Sparql endpoint 2 (En caso de que haya 2 sparql endpoint, no es obligatorio)
 - SparqlASIO_2.Endpoint: Url del Sparql endpoint
 - SparqlASIO_2.Username: Usuario del Sparql endpoint
 - SparqlASIO_2.Password: Password del Sparql endpoint
 
 Funcionamiento:
 
 1º- Se realizan las configuraciones correspondientes en fichero appsettings
 
 2º- Se añaden dentro de la carpeta 'Dataset' los ficheros corresponfientes a la exportación de los datos de Murcia, tiene que contener los siguientes ficheros:
- Articulos.xml
- Autores articulos.xml
- Autores congresos.xml
- Autores exposiciones.xml
- Centros.xml
- Congresos.xml
- Departamentos.xml
- Equipos proyectos.xml
- Exposiciones.xml
- Fechas equipos proyectos.xml
- Fechas proyectos.xml
- Personas.xml
- Proyectos.xml 

3º Se ejecuta la aplicación

4º La aplicación leera los ficheros dentro de la carpeta 'Dataset' y generará una nueva carpeta llamada 'RDF' con los RDF correspondientes de ese Dataset y finalmente volcará los datos a las BBDD rdf
 
 
