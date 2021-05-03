![](../../Docs/media/CabeceraDocumentosMD.png)

# Acerca de UrisFactory

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/dashboard?id=UrisFactory)

![](https://github.com/HerculesCRUE/GnossDeustoBackend/workflows/Build%20and%20test%20UrisFactory/badge.svg)
[![codecov](https://codecov.io/gh/HerculesCRUE/GnossDeustoBackend/branch/master/graph/badge.svg?token=4SONQMD1TI&flag=uris)](https://codecov.io/gh/HerculesCRUE/GnossDeustoBackend)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=UrisFactory&metric=bugs)](https://sonarcloud.io/dashboard?id=UrisFactory)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=UrisFactory&metric=security_rating)](https://sonarcloud.io/dashboard?id=UrisFactory)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=UrisFactory&metric=ncloc)](https://sonarcloud.io/dashboard?id=UrisFactory)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=UrisFactory&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=UrisFactory)

La documentación que explica la Especificación de URIs Hércules y las Buenas prácticas de URis está en:

https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Docs/UrisFactory

La librería se utiliza como un servicio web, accesible en pruebas en esta dirección a través de swagger: 

http://herc-as-front-desa.atica.um.es/uris/swagger/index.html

La documentación de la librería está disponible en: 

http://herc-as-front-desa.atica.um.es/uris-factory/library/api/UrisFactory.Controllers.html

Los resultados de las pruebas unitarias se pueden consultar en [ResultsTest](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/Hercules.Asio.UrisFactory/ResultsTest).

La librería compilada se encuentra en la carpeta [librerías](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Builds/libraries).

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

 Se puede encontrar un el appsettings usado para este servicio sin datos sensibles en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/src/Hercules.Asio.UrisFactory/UrisAutoGenerator/appsettings.json

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

Ver la documentación en [Especificación de Esquema de URIs](../../Docs/UrisFactory/Especificacion-Esquema-de-URIs.md)

