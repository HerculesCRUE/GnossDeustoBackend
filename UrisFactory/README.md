# Sobre UrisFactory

Accesible en pruebas en esta dirección a través de swagger: http://herc-as-front-desa.atica.um.es/uris/swagger/index.html.

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
	`{
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
    "labelResourceClass": "example",
    "resourceURI": "uriExampleStructure"
  }
}`
	 
