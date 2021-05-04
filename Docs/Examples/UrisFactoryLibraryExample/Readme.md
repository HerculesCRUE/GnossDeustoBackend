![](../../../Docs/media/CabeceraDocumentosMD.png)

# Ejemplo de uso de la libreria UrisFactory

Este proyecto es un ejemplo de uso de la librería uris factory. Es una aplicación web sencilla que crea una estructura de URIs nueva y obtiene a partir de esa estructura de URIs la URI para una entidad: 

> UriStructureGeneral structureGeneral = new UriStructureGeneral() { Base = "http://graph.um.es" };
> ...
> string uriSchemaJson = JsonConvert.SerializeObject(structureGeneral);

> ConfigJsonHandler config = new ConfigJsonHandler(uriSchemaJson);

> FactoryController factoryController = new FactoryController(config);
> return factoryController.GenerateUri("Test", "1234");

La referencia a la librería UrisFactory se encuentra en el directorio Libraries/UrisFactory.dll. 
