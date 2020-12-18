![](.//docs/media/CabeceraDocumentosMD.png)

# Acerca de UrisFactory
![](https://github.com/HerculesCRUE/GnossDeustoBackend/workflows/Build%20and%20test%20UrisFactory/badge.svg)
[![codecov](https://codecov.io/gh/HerculesCRUE/GnossDeustoBackend/branch/master/graph/badge.svg?token=4SONQMD1TI&flag=uris)](https://codecov.io/gh/HerculesCRUE/GnossDeustoBackend)

La documentación que explica la Especificación de URIs Hércules y las Buenas prácticas de URis está en:

https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/UrisFactory/docs

La librería se utiliza como un servicio web, accesible en pruebas en esta dirección a través de swagger: 

http://herc-as-front-desa.atica.um.es/uris/swagger/index.html.

La documentación de la librería está disponible en: 

http://herc-as-front-desa.atica.um.es/uris-factory/library/api/UrisFactory.Controllers.html

Los resultados de las pruebas unitarias se pueden consultar en [ResultsTest](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/UrisFactory/ResultsTest).

La librería compilada se encuentra en la carpeta [librerías](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/libraries).

Este Api contiene dos controladores.


 - **Factory**: contiene el siguiente método para la generación de uris: 
	 - **get /Factory** : Genera una uri de forma dinámica pasandole dos parámetros: 
		 - **Resource_Class**: Nombre del esquema de configuración de las Uris.
		 - **identifier:** Identificador.
 - **Schema:** ofrece métodos para administrar la configuración de Uris:
	 - **get /Schema:** Para obtener todos los esquemas de configuración.
	 - **post /Schema:** Remplaza la configuración de esquemas por otra.
	 - **Delete /Schema:** Elimina un esquema de uris.
	 - **Put /Schema:** Añade un nuevo esquema de uris.
	 - **Get /Schema{name}:** Obtiene un esquema de uris
	 >Los métodos correspondientes a Schema reciben o devuelven un objeto de esquema de uris como puede por ejemplo:
	
	    [
		    {
			    "uriStructure": {
				    "name": "uriExampleStructure",  
				    "components": [
					    {
						    "uriComponent": "base",
						    "uriComponentValue": "base",
						    "uriComponentOrder": 1,
						    "mandatory": true,
						    "finalCharacter": "/"
					    },
					    {
						    "uriComponent": "character",
						    "uriComponentValue": "character@RESOURCE",
						    "uriComponentOrder": 2,
						    "mandatory": true,
						    "finalCharacter": "/"
					    },
					    { 
						    "uriComponent": "resourceClass",
						    "uriComponentValue": "resourceClass@RESOURCECLASS",
						    "uriComponentOrder": 3,
						    "mandatory": true,
						    "finalCharacter": "/"
					    },
					    { 
						    "uriComponent": "identifier",  
						    "uriComponentValue": "@ID",
						    "uriComponentOrder": 4,
						    "mandatory": true,
						    "finalCharacter": ""
					    }
				    ]
			    },  
			    "resourcesClass": {  
				    "resourceClass": "Example",
					"RdfType": "ExampleRdfType",
					"labelResourceClass": "example",
				    "resourceURI": "uriExampleStructure"
			    }
		    }
	    ]

*Obtención del Token*
-------------------------
Este api esta protegida mediante tokens, por ello para poder usar la interfaz swagger hay que obtener un token, el cual se puede obtener desde https://herc-as-front-desa.atica.um.es/carga-web/Token
	
## Configuración en el appsettings.json	

    {
      "Logging": {
          "LogLevel": {
                "Default": "Information",
                 "Microsoft": "Warning",
                  "Microsoft.Hosting.Lifetime": "Information"
                    }
              },
        "Authority": "http://localhost:56306",
         "Scope": "apiUrisFactory",
          "AllowedHosts": "*"
    }
- Scope: Limitación de acceso al api de urisFactory

 Se puede encontrar un el appsettings usado para este servicio sin datos sensibles en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/UrisFactory/UrisAutoGenerator/appsettings.json

## Dependencias

 - **IdentityServer4**: versión 3.1.2
 -  **IdentityServer4.AccessTokenValidation**: versión 3.0.1
 - **Microsoft.EntityFrameworkCore.Tools**: versión 3.0.0
 - **Microsoft.Extensions.Logging**: versión 3.1.1
 - **Microsoft.Extensions.Logging.Debug**: versión 3.0.0
 - **Microsoft.VisualStudio.Web.CodeGeneration.Design**: versión 3.0.0
 - **Newtonsoft.Json**: versión 12.0.3
 - **Serilog.AspNetCore**: versión 3.2.0
 - **Swashbuckle.AspNetCore**: versión 5.0.0
 - **Swashbuckle.AspNetCore.Annotations**: versión 5.0.0
 - **Swashbuckle.AspNetCore.Filters**: versión 5.0.0
 - **Swashbuckle.AspNetCore.Swagger**: versión 5.0.0
 - **Swashbuckle.AspNetCore.SwaggerGen**: versión 5.0.0
 - **Swashbuckle.AspNetCore.SwaggerUI**: versión 5.0.0
 - **SSwashbuckle.Core**: versión 5.6.0	
	 
# Hércules backend asio. ESPECIFICACIÓN DE ESQUEMA DE URIS

## Proyecto Hércules

 1. [Introducción](#1-introducción)
 2. [Características del esquema de Uris](#2-características-del-esquema-de-uris)
 3. [Estructura del esquema de Uris](#3-estructura-del-esquema-de-uris)
	 1. [Base](#31-base)
	 2. [Carácter de la información](#32-carácter-de-la-información)
	 3. [Sector o ámbito](#33-sector-o-ámbito)
	 4. [Dominio o temática](#34-dominio-o-temática)
	 5. [Conceptos específicos](#35-conceptos-específicos)
 4. [Tipos de uris](#4-tipos-de-uris)
	 1. [Uri para identificar vocabularios](#41-uri-para-identificar-vocabularios)
	 2. [Uri para identificar esquemas de conceptos](#42-uri-para-identificar-esquemas-de-conceptos)
	 3. [Uri para identificar a cualquier instancia física o conceptual](#43-uri-para-identificar-a-cualquier-instancia-física-o-conceptual)
 5. [Normalización de los componentes de los uris](#5-normalización-de-los-componentes-de-los-uris)
 6. [Prácticas relativas a la gestión de recursos semánticos a través de uri](#6-prácticas-relativas-a-la-gestión-de-recursos-semánticos-a-través-de-uri)
 7. [Definición del esquema de uris para la factoría de uris](#7-definición-del-esquema-de-uris)

## 1 Introducción

El presente documento describe la Especificación del Esquema de URIs de Hércules. En su elaboración tenemos en cuenta las recomendaciones de la [Norma Técnica de Interoperabilidad de Reutilización de recursos de la Información](https://www.boe.es/boe/dias/2013/03/04/pdfs/BOE-A-2013-2380.pdf) (NTI), de la Secretaría de Estado de Administraciones Públicas; que se basan, a su vez, en las iniciativas de datos abiertos y las recomendaciones procedentes del mundo de la Web Semántica.

Como veremos, estas recomendaciones serán adaptadas al ámbito de un sistema de investigación universitaria; y tendrán presente la resolución y conexión de las entidades identificadas por la URIs.

## 2 Características del esquema de Uris

Este documento de diseño del Esquema de URIs Hércules tendrá unos requisitos genéricos similares a los utilizados en la citada NTI. Estos requisitos serán:

 1. Utilizar el protocolo HTTP, de forma que se garantice la resolución de cualquier URI en la web.
 2. Usar una estructura de composición de URI consistente, extensible y persistente. Las normas de construcción de los URI seguirán unos patrones determinados que ofrezcan coherencia en la uniformidad, los cuales podrán ser ampliados o adaptados en caso de necesidad.
 3. Seguir una estructura de composición de URIs comprensible y relevante. Esto significa que el propio identificador debe ofrecer información semántica autocontenida, lo que permitirá a cualquier agente reutilizador disponer de información sobre el propio recurso, así como su procedencia.
 4. No exponer información sobre la implementación técnica de los recursos que representan los URIs. En la medida de lo posible se omitirá información específica sobre la tecnología subyacente del recurso representado; por ejemplo, no se incluirán las extensiones correspondientes a tecnologías con las que se generan los recursos web como .php, .jsp, etc.
 5. Cumplir el principio de persistencia de los URIs, lo que significa que los que ya han sido creados previamente nunca deberían variar, y que el contenido al que hacen referencia debería ser accesible. En el caso de que sea necesario cambiar o eliminar el recurso al que apunta un identificador, se deberá establecer un mecanismo de información sobre el estado del recurso usando los códigos de estado de HTTP. En el caso de poder ofrecer una redirección a la nueva ubicación del recurso, se utilizarán los códigos de estado HTTP 3XX, mientras que para indicar que un recurso ha desaparecido permanentemente se utilizará el código de estado HTTP 410.

## 3 Estructura del esquema de uris

Los URIs generados tendrán una estructura uniforme que proporcionará coherencia al esquema de URIs de Hércules ASIO como sistema de representación de los recursos, de acuerdo con los requisitos genéricos previamente descritos y proporcionará información intuitiva sobre la procedencia y el tipo de información identificada.

Además de la base, los elementos que compondrán la ruta del URI serán: carácter de la información y, opcionalmente, dominio, concepto y extensión/formato. El orden de los elementos es el siguiente (se señalan entre corchetes los elementos opcionales):

http://{base}/{carácter}[/{dominio}][/{concepto}][.{ext}]

Definimos a continuación cada uno de los elementos de la ruta del URI.

### 3.1 Base

Es un componente obligatorio que define el espacio y dominio dedicado por cada universidad al albergue de la plataforma de datos abiertos y reutilizables. Recomendamos el uso de un subdominio, por ejemplo:

http://datos.um.es

http://datos.crue.org

http://sgi-data.deusto.es

### 3.2 Carácter de la información 

Es un componente obligatorio, que puede tomar una de las siguientes formas:

 - catalogo o cat. En principio, el proyecto HERCULES ASIO no contempla el alojamiento de datasets como medio de publicación de datos abiertos, sino que todo el portal será un sistema Linked Data interrogable mediante un API y un punto SPARQL. No parece necesario que existan declaraciones de datasets.
 - def. Indica que el recurso identificado es un vocabulario u ontología definido por OWL.
 - kos. Indica que se trate de un sistema de organización del conocimiento (Knowledge Organization System) en el dominio de SGI: tesauros, taxonomías, etc.
 - res. Indica que se trata de una entidad del dominio.

### 3.3 Sector o ámbito

Es un componente opcional de posible aplicación en URIs de organización de conocimiento.


### 3.4 Dominio o temática

Es un componente opcional de posible aplicación en URIs de organización de conocimiento o en entidades que puedan tener subclases. Por ejemplo, podría servir para distinguir el tema de una publicación.

### 3.5 Conceptos específicos

Es un componente opcional del URI, pero funcionalmente obligatorio si se trata de declarar entidades del ámbito de SGI, como: investigador, publicación, proyecto, etc.

## 4 Tipos de URIs

A continuación, se especifican los tipos de URI específicos para recursos semánticos de una iniciativa basada en _Linked Data_.

### 4.1 Uri para identificar vocabularios

Cualquier vocabulario u ontología seguirá el esquema: http://{base}/def/{sector}/{dominio}

### 4.2 URI para identificar esquemas de conceptos

Cualquier sistema de organización del conocimiento –taxonomías, diccionarios, tesauros, etc.– sobre un dominio concreto será identificado mediante un esquema de URI basado en la estructura: http://{base}/kos/{sector}/{dominio}

### 4.3 URI para identificar a cualquier instancia física o conceptual

Estos recursos son las representaciones atómicas de los documentos y recursos de información. A su vez suelen ser instancias de las clases que se definen en los vocabularios. Estos recursos se identifican mediante el esquema: http://{base}/res/{sector}[/{dominio}]/{clase}/{ID}

Por ejemplo: http://data.um.es/res/investigador/{id-investigador}

Las instancias físicas o conceptuales que se incluirán como fragmentos en las URIs se corresponderán con las entidades identificadas en la Red de Ontologías Hércules (ROH), como: researcher/investigador, project/proyecto, publication/publicación, etc.

## 5 Normalización de los componentes de los URIs

Para garantizar la coherencia y el mantenimiento posterior del esquema de URI se aplicarán las siguientes reglas para normalizar las distintas partes que componen los URI:

 1. Seleccionar identificadores alfanuméricos cortos únicos, que sean representativos, intuitivos y semánticos.
 2. Usar siempre minúsculas, salvo en los casos en los que se utilice el nombre de la clase o concepto. Habitualmente, los nombres de las clases se representan con el primer carácter de cada palabra en mayúsculas.
 3. Eliminar todos los acentos, diéresis y símbolos de puntuación. Como excepción puede usarse el guión (–).
 4. Eliminar conjunciones y artículos en los casos de que el concepto a representar contenga más de una palabra.
 5. Usar el guión (–) como separador entre palabras.
 6. Evitar en la medida de lo posible la abreviatura de palabras, salvo que la abreviatura sea intuitiva.
 7. Los términos que componen los URI deberán ser legibles e interpretables por el mayor número de personas posible, por lo que se utilizará el castellano, cualquiera de las lenguas oficiales de España o el inglés como lengua franca de la investigación.

## 6 Prácticas relativas a la gestión de recursos semánticos a través de URI

Las siguientes prácticas se desarrollarán como requisitos del servidor Linked Data de Hércules ASIO y se aplicarán para la gestión de recursos semánticos descritos en RDF:

 1. Siempre que sea posible, y existan versiones del recurso en formato legible para personas HTML o similar y RDF, el servidor que gestiona los URI realizará negociación del contenido en función de la cabecera del agente que realiza la petición. En el caso de que el cliente acepte un formato de representación RDF en cualquiera de sus notaciones (p.e., especificando en su cabecera que acepta el tipo MIME application/rdf+xml) se servirá el documento RDF a través del mecanismo de redirecciones alternativas mediante los códigos de estado HTTP 3XX.
 2. En el caso de que no se realice una negociación del contenido desde el servidor y, para favorecer el descubrimiento de contenido RDF desde los documentos HTML relacionados con las descripciones de los recursos, se incluirán enlaces a la representación alternativa en cualquiera de las representaciones en RDF desde los propios documentos HTML de la forma `<link rel=«alternate» type=«application/rdf+xml» href=«documento.rdf»>` o similar. En esa sentencia se incluye el tipo de formato MIME del documento (application/rdf+xml, text/n3, etc.).
 3. Cuando se establezcan enlaces entre distintos recursos de información, se procurará la generación de enlaces que conecten los recursos bidireccionales para facilitar la navegación sobre los recursos de información en ambos sentidos.

## 7 Definición del Esquema de URIs

El Esquema de URIs tiene que declararse en un formato informático que pueda ser interpretado por la Factoría de URIs para devolver el identificador único que precisa cada entidad cargada en ASIO. La propuesta se declara como un JSON y tiene la siguiente forma:

	
    [
    		{
    			"base": {url},
    			"characters": [
    				{
    					"character": {nombre del carácter},
    					"labelCharacter": {etiqueta del carácter}
    				}
    			]
    			"uriResourceStructure": [
    				{
    					"uriComponent": {identificador del componente},
    					"uriComponentValue": {origen del valor del componente},
    					"uriComponentOrder": {orden del componente en el URI},
    					"mandatory": {true or false},
    					"finalCharacter": {si lo tiene, será una barra “/”}
    				}
    			]
    			"resourcesClasses": [ 
    				{
    					"resourceClass": {se corresponde con una entidad de ROH},
						"RdfType": "Rdf type al que pertenece en la ontología",
						"labelResourceClass": {opcional, etiqueta de la entidad},
    					"resourceURI": {nombre de la estructura de URI que aplica}
    				}
    			]
    		}
    	]

Las partes del formato anterior son:

 - **base**. Contendrá el dominio en el que se encuentra el servidor que resolverá el URI.
 - **characters**. Podrá tener más de uno, pero al menos necesitará el que indica las entidades (res). Por cada carácter se indica:
	 - **character**. Identifica al tipo de carácter del URI.
	 - **labelCharacter**. Devuelve el fragmento del URI para el tipo de carácter.
 - **uriResourceStructure**. Podrá tener más de uno, pero al menos necesitará uno que indique como se compone el URI de las entidades de la ROH. La composición del URI se declara mediante elementos que tienen los siguientes atributos:
	 - **uriComponent**. Identifica el componente.
	 - **uriComponentValue**. Identifica como se obtiene el valor del componente. Las opciones pueden ser:
		 - {porción del JSON@IDENTIFICADOR}, cuando el valor está definido en el propio esquema.
		 - {@IDENTIFICADOR}, cuando el valor se suministra desde la aplicación que invoca a la factoría de URIs.
	 - **uriComponentOrder**. Define el orden del componente en el URI devuelto.
	 - **mandatory**. Indica si el componente del URI es obligatorio.
	 - **finalCharacter**. Indica si el componente lleva una barra para la composición de un URI correcto.
 - **resourceClasses**. Tendrá tantos elementos como entidades de ROH necesiten disponer de un URI a través de la Factoría de URIs. Los atributos por los que se declara como se compone el URI para cada entidad son:
	 - **resourceClass**. Identificador del tipo de entidad.
	 - **RdfType**. Rdf type al que pertenece en la ontología.
	 - **labelResourceClass**. Opcional, se declara si se desea que la URL tenga otro texto, habitualmente por requisitos de idioma.
	 - **resourceURI**. Identifica el elemento uriResourceStructure que se usará para componer el URI de la entidad.
	 

Dado el carácter de personalización completa gracias a este formato, la librería podría responder a cualquier otro Esquema de URIs, como el de la NTI (https://www.boe.es/boe/dias/2013/03/04/pdfs/BOE-A-2013-2380.pdf).


Se indica a continuación un ejemplo:

    [
	    {	    
		    "base": "http://datos.um.es",    
			    "characters": [    
				    {
					    "character": "resource", 
					    "labelCharacter": "res"	    
				    }
				    {
					    "character": "kos"
					    "labelCharacter": "kos"
				    }
			    ]	   
			    "uriResourceStructure": [ 
				    {
					    "uriComponent": "base",   
					    "uriComponentValue": "base",
					    "uriComponentOrder": 1,  
					    "mandatory": true,
					    "finalCharacter": "/"  
				    }
				    {
					    "uriComponent": "character",
					    "uriComponentValue": "character@RESOURCE",
					    "uriComponentOrder": 2,
					    "mandatory": true,
					    "finalCharacter": "/"
				    }
				    {
					    "uriComponent": "resourceClass",
					    "uriComponentValue": "resourceClass@RESOURCECLASS",
					    "uriComponentOrder": 3,
					    "mandatory": true,
					    "finalCharacter": "/"
				    }
				    {
					    "uriComponent": "identifier",
					    "uriComponentValue": "@ID",
					    "uriComponentOrder": 4,
					    "mandatory": true,
					    "finalCharacter": ""
				    }
			    ]
			    "uriPublicationStructure": [
				    {
					    "uriComponent": "base",
					    "uriComponentValue": "base",
					    "uriComponentOrder": 1,
					    "mandatory": true,
					    "finalCharacter": "/"
				    }
				    {
					    "uriComponent": "character",
					    "uriComponentValue": "character@RESOURCE",
					    "uriComponentOrder": 2,
					    "mandatory": true,
					    "finalCharacter": "/"
				    }
				    {
					    "uriComponent": "sector",
					    "uriComponentValue": "@SECTOR",
					    "uriComponentOrder": 3,
					    "mandatory": true,
					    "finalCharacter": "/"
				    }
				    {
					    "uriComponent": "resourceClass",
					    "uriComponentValue": "resourceClass@RESOURCECLASS",
					    "uriComponentOrder": 4,
					    "mandatory": true,
					    "finalCharacter": "/"
				    }
				    {
					    "uriComponent": "identifier",
					    "uriComponentValue": "@ID",
					    "uriComponentOrder": 5,  
					    "mandatory": true,
					    "finalCharacter": ""
				    }
			    ]
		    "resourcesClasses": [
			    {
				    "resourceClass": "researcher",
					"RdfType": "researcherType",
					"labelResourceClass": "investigador",
				    "resourceURI": "uriResourceStructure"
			    },
			    {
				    "resourceClass": "publication",
					"RdfType": "publicationType",
					"labelResourceClass": "publicacion",
				    "resourceURI": "uriPublicationStructure"
			    }
		    ]
	    }
    ]




