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
[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/dashboard?id=API_CARGA)

![](https://github.com/HerculesCRUE/GnossDeustoBackend/workflows/Build%20and%20test%20API_CARGA/badge.svg)
[![codecov](https://codecov.io/gh/HerculesCRUE/GnossDeustoBackend/branch/master/graph/badge.svg?token=4SONQMD1TI&flag=carga)](https://codecov.io/gh/HerculesCRUE/GnossDeustoBackend)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=API_CARGA&metric=bugs)](https://sonarcloud.io/dashboard?id=API_CARGA)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=API_CARGA&metric=security_rating)](https://sonarcloud.io/dashboard?id=API_CARGA)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=API_CARGA&metric=ncloc)](https://sonarcloud.io/dashboard?id=API_CARGA)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=API_CARGA&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=API_CARGA)

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
 
## Configuración en los ficheros TOML
    
	[[entities]] # [[Objeto1]]
	rdftype = "http://purl.org/roh/mirror/foaf#Person" # Propiedad del Objeto1 = "Asignación"
	id = "ID" 
	source = "Persona" 

		[[entities.properties]] # [[Objeto1.Objeto2]]
		property = "http://purl.org/roh/mirror/foaf#name" # Propiedad del Objeto2 = "Asignación"
		source = "nombre"
    
Un fichero TOML (.toml) es un tipo de fichero de configuración que tiene como función mapear datos de forma sencilla.
Los ficheros TOML utilizados en la aplicación siguen una estructura similar al ejemplo de arriba.

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
	- pXmlFile: Seleccionaremos un XML perteneciente a ASIO. [XMLs de ASIO](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/src/Hercules.Asio.Api.Discover/xml_descubrimiento.zip)
- En el caso se quiera realizar la prueba con "oai_cerif_openaire", hay que escoger un XML de: **TODO**
- Una vez introducidos ambos parametros correctamente, pulsamos "Execute".
- Como resultado nos mostrará el RDF generado. Para descargarlo, pulsamos sobre el botón de "Download" situado en la parte inferior derecha.

## Dependencias

- **dotNetRDF**: versión 2.6.0
- **Nett**: versión 0.15.0
- **Swashbuckle.AspNetCore.Swagger**: versión 5.6.3
- **Swashbuckle.AspNetCore.SwaggerGen**: versión 5.6.3
- **Swashbuckle.AspNetCore.SwaggerUI**: versión 5.6.3
