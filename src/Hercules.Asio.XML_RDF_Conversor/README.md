![](..//Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 12/02/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|XML RDF Conversor readme| 
|Descripción|Manual del servicio XML RDF Conversor|
|Versión|0.1|
|Módulo|XML RDF Conversor|
|Tipo|Manual|
|Cambios de la Versión|Primera subida|

## Sobre XML RDF Conversor
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

Accesible en el entorno de desarrollo en esta dirección a través de swagger: http://herc-as-front-desa.atica.um.es/uris/swagger/index.html.

XML RDF Conversor es un servicio web que contiene únicamente un controlador, cuyo propósito es:
 - ConversorController: Permite obtener un un archivo RDF mediante un XML dado.

## Configuración en el appsettings.json

    { 
		"Logging": {
		    "LogLevel": {
		      "Default": "Information",
		      "Microsoft": "Warning",
		      "Microsoft.Hosting.Lifetime": "Information"
		    }
		  },
		  "AllowedHosts": "*",
		  "UrlUrisFactory": "http://herc-as-front-desa.atica.um.es/uris/"
    }
    
 - LogLevel.Default: Nivel de error por defecto.
 - LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft.
 - LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host.
 - UrlUrisFactory: URL dónde está lanzada la aplicación.
 
## Configuración en los ficheros JSON
    
	{
		"entities": [{
			"rdftype": "rdftype",
			"rdftypeproperty": "rdftype",
			"id": "id",
			"nameSpace": "nameSpace",
			"source": "source",
			"property": "property",
			"datatype": "datatype",
			"properties": [{
					"property": "property",
					"source": "source",
					"datatype": "datatype"
				}
			],
			"subentities": [{
					"property": "property",
					"inverseProperty": "inverseProperty",
					"entities": [{}]
				}
			],
			"mappingrdftype": [{
					"nameSpace": "nameSpace",
					"source": "source",
					"target": "target"
				}
			]
		}]		
	}

- Entities
	- rdftype: Tipo de la ontología.
	- rdftypeproperty: Tipo del rdf del cual habrá que obtener del mapa.
	- id: Propiedad id del nodo.
	- nameSpace: Espacio de nombres del nodo.
	- source: Nombre del nodo.
	- property: Propiedad la cual hay que acceder y no se encuentra en el nodo.
	- datatype: Tipo de dato de la propiedad que hay que acceder.
- Properties
	- property: Tipo de la propiedad en la ontología.
	- source: Nombre del nodo.
	- datatype: Tipo de la propiedad. String por defecto.
- Subentities
	- property: Propiedad de la subentidad a la que apunta.
	- inverseProperty: Propiedad que indica si es inversa. Si no aparece, es que es directa.
	- entities: Listado de subentidades de tipo Entity.
- Mappingrdftype
	- nameSpace: Espacio de nombre del nodo.
	- source: Contenido de la etiqueta en la cual se le ha de aplicar el target.
	- target: IRI del tipo de la entidad a la que apunta.

## Comprobaciones y pruebas

Realizaremos las pruebas accediendo a dos métodos:
- /Conversor/ConfigurationFilesList: Nos muestra un listado de todos los archivos de configuración que existen.
- /Conversor/Convert: Permite convertir archivos XML en RDF.

## /Conversor/ConfigurationFilesList    

- Pulsamos en el método /Conversor/ConfigurationFilesList.
- Seleccionamos "Try it out".
- Pulsamos al botón "Execute".
- Al ejecutar el método, se realizará lo siguiente:
	- Mostrará una lista con los nombres de los diferentes archivos de configuración que hay.
	
		[
		"oai_cerif_openaire", 
		"XML_ASIO"
		]
		
- Según el tipo de ejemplo que queramos realizar, escogeremos uno u otro. En este caso, se realizará con "XML_ASIO".

## /Conversor/Convert

- Pulsamos en el método /Conversor/Conversor/Convert.
- Seleccionamos "Try it out".
- Hay que completar los siguientes campos:
	- pType: Indicamos el fichero de configuración que se quiera utilizar. En nuestro caso, se le indicará "XML_ASIO".
	- pXmlFile: Seleccionaremos un XML perteneciente a ASIO. 
- Una vez introducidos ambos parametros correctamente, pulsamos "Execute".
- Como resultado nos mostrará el RDF generado. Para descargarlo, pulsamos sobre el botón de "Download" situado en la parte inferior derecha.

## Dependencias

- **dotNetRDF**: versión 2.6.0
- **Swashbuckle.AspNetCore.Swagger**: versión 5.6.3
- **Swashbuckle.AspNetCore.SwaggerGen**: versión 5.6.3
- **Swashbuckle.AspNetCore.SwaggerUI**: versión 5.6.3
