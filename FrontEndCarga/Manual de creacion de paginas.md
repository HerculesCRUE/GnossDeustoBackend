![](../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 01/10/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|versión inicial| 
|Descripción|Documentación de creación de páginas por parte del usuario|
|Versión|0.1|
|Módulo|FrontEndCarga|
|Tipo|Manual|
|Cambios de la Versión|Creación|

# Manual de creación de páginas

Creación de páginas sin contenido dinámico
----------------
Estas páginas contienen únicamente contenido html estático, se puede encontrar un ejemplo en: 
https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/FrontEndCarga/paginas/gnossdeustobackend.cshtml

Como curiosidad esta página contiene el código: 

    @{
    Layout = "_Layout";
    ViewData["BodyClass"] = "fichaRecurso";
    }
	
 - Layout = "_Layout": Con este código indicamos que coja el Layout compartida con todas las páginas del hércules para tener las páginas de forma homogénea.
 - ViewData["BodyClass"] = "fichaRecurso": Añade la clase fichaRecurso al body de la página (él cuál está en el Layaout) para poder usar los estilos de la clase de la web.
 
 Creación de páginas con contenido dinámico
----------------

En este tipo de páginas vamos a distinguir dos tipos de casos, la obtención de datos con llamadas a los apis o la obtención de datos mediante consultas a virtuoso.
Estas vistas comparten el mismo módelo de datos de página, este modelo es CmsDataViewModel que contiene una propiedad Results donde se almacena el resultado de la llamada al api indicado o la 
consulta a virtuoso
## Llamadas al Api
Mediante la etiqueta `@*<% api https://localhost:44359/Job?type=0&count=100&from=0 /%>*@` indicamos que es una llamada al api, 
**importante respetar los espacios en la etiqueta tanto al principio `@*<%` como al final `/%>*@`.** con directiva api dentro de la etiqueta indicamos que es una llamada al api, la llamada se especifica después 
de la directiva api separada por un espacio.

Se puede encontrar un ejemplo en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/FrontEndCarga/paginas/api.cshtml

En está pagina se añade un using a Newtonsoft Json `@using Newtonsoft.Json` para poder deserializar los datos que están el modelo en Results ya que se encuentran en Json y poder trabajar con la clase obtenida
posteriormente.
Al final del documento está declarada la calse a deserializar mediante la directiva de razor 

    @functions{
    
    }
	
## Consultas a virtuoso
Mediante la etiqueta 

    @*<% sparql 
    select ?type count(?s) as ?count 
    where
    {
    ?s rdf:type ?type
    }
    group by ?type
    /%>*@

indicamos que es una llamada a virtoso, **importante respetar los espacios en la etiqueta tanto al principio `@*<%` como al final `/%>*@`.** con directiva sparql dentro de la etiqueta indicamos que es una llamada a virtuoso, la consulta se especifica después separada por un espacio.
Se puede encontrar un ejemplo en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/FrontEndCarga/paginas/sparql.cshtml
En está página se han añadido varios using para poder deserializar el contenido que hay en la propiedad Results del modelo, el cual está en formato csv, y posteriormente trabjar con los datos obtenidos al deserializar.
Al final del documento está declarada la calse a deserializar mediante la directiva de razor 

    @functions{
    
    }