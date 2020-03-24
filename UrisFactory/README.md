# Sobre UrisFactory

Este Api contiene dos controladores

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