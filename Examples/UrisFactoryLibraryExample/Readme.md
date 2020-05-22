# Ejemplo de uso de la libreria UrisFactory

En este proyecto se ha creado un ejemplo de uso de la librería del uris factory, esta libreria la podemos encontrar dentro del proyecto 
UrisFactoryLibraryExample en Libraries/UrisFactory.dll
Este ejemplo contiene un controlador con un método Get, cuya funcionalidad es inicializar una estructura de un fichero de configuración de uris para su posterior manejo como se realiza en la siguiente parte del código, que corresponde a la última parte del ejemplo:

    return schemaController.GetUriStructureInfo("test");
Esta línea de código accede al esquema generado y obtiene la estructura de uris con el nombre test
