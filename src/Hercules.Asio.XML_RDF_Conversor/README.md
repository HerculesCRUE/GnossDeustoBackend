![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 19/05/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|XML RDF Conversor| 
|Descripción|Manual del servicio XML RDF Conversor|
|Versión|2|
|Módulo|XML RDF Conversor|
|Tipo|Manual|
|Cambios de la Versión|Creación|

## Sobre XML RDF Conversor
[<img align="right" width="100px" src="https://dotnetfoundation.org/img/logo_big.svg" />](https://dotnetfoundation.org/projects?searchquery=IdentityServer&type=project)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=XML_RDF_Conversor&metric=bugs)](https://sonarcloud.io/dashboard?id=XML_RDF_Conversor)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=XML_RDF_Conversor&metric=security_rating)](https://sonarcloud.io/dashboard?id=GestorDocumentacion)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=XML_RDF_Conversor&metric=ncloc)](https://sonarcloud.io/dashboard?id=XML_RDF_Conversor)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=XML_RDF_Conversor&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=GestorDocumentacion)

Accesible en el entorno de desarrollo en esta dirección a través de swagger: http://herc-as-front-desa.atica.um.es/conversor_xml_rdf/swagger/index.html.

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
 - UrlUrisFactory: URL dónde está lanzada la aplicación UrisFactory.
 
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

- Entities: Lista cada una de las entidades que tienen mapeo
	- rdftype: IRI del Tipo de la ontología a cargar (debe estar especificado éste o 'rdftypeproperty' para realizar el mapeo hacia el rdftype).
	- rdftypeproperty: Elemento del XML que se utilizará para mapear el rdftype, si está especificado también debe estar especificado 'Mappingrdftype'.
	- id: Elemento del XML del que se cogerá el identificador del elemento.
	- nameSpace: Espacio de nombres del nodo.
	- source: Elemento del XML que se considerará para crear la entidad.
	- property: Propiedad la cual hay que acceder y no se encuentra en el nodo.
	- datatype: Tipo de dato de la propiedad que hay que crear.
	- transform: Transformación que hay que realizar del texto obtenido del XML
- Properties: Listado de propiedades de la entidad (de tipo literal)
	- property: IRI de la propiedad en la ontología.
	- source: Nombre del nodo del que obtener la propiedad.
	- datatype: Tipo de la propiedad. String por defecto.
	- transform: Transformación que hay que realizar del texto obtenido del XML
- Subentities: Listado de propiedades de la entidad que apuntan a otras entidades (no tipo literal)
	- property: IRI de la propiedad de la subentidad a la que apunta.
	- inverseProperty: Propiedad que indica si es inversa. Si no aparece, es que es directa.
	- entities: Listado de subentidades de tipo Entity.
- Mappingrdftype: Mapeo para obtener el rdftype de la entidad
	- nameSpace: Espacio de nombre del nodo.
	- source: Contenido del XML al que se le aplicará el mapeo.
	- target: IRI del tipo de la entidad a la que apunta.

Se puede ver un ejemplo completo con la configuración para la Universidad de Radboud [aquí](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/src/Hercules.Asio.XML_RDF_Conversor/XML_RDF_Conversor/Config/oai_cerif_openaire.json)

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
