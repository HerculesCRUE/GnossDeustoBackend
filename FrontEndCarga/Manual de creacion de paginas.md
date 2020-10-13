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
Este tipo páginas contienen únicamente contenido html estático, se puede encontrar un ejemplo en: 
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

En este tipo de páginas vamos a distinguir dos tipos de llamadas, la obtención de datos con llamadas a los apis o la obtención de datos mediante consultas a virtuoso. Puede haber varias llamadas de estos tipos 
en las páginas creadas.
Estas vistas comparten el mismo módelo de datos de página, este modelo es CmsDataViewModel que contiene una propiedad Results, la cual es una lista de string donde se almacena los resultados de la llamadas al api indicado o la 
consulta a virtuoso. Para incluir este módelo en las páginas hay que incluir en la primera línea:

    @model ApiCargaWebInterface.ViewModels.CmsDataViewModel
	
## Llamadas al Api
Para este ejemplo vamos a partir de que solo hay una única llamada del tipo api.
Mediante la etiqueta `@*<% api https://localhost:44359/Job?type=0&count=100&from=0 /%>*@` indicamos que es una llamada al api, 
**importante respetar los espacios en la etiqueta tanto al principio `@*<%` como al final `/%>*@`.** con directiva api dentro de la etiqueta indicamos que es una llamada al api, la llamada se especifica después 
de la directiva api separada por un espacio.

Se puede encontrar un ejemplo en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/FrontEndCarga/paginas/api.cshtml

En está pagina se añade un using a Newtonsoft Json `@using Newtonsoft.Json` para poder deserializar los datos que están en el primer elemento de la lista que se encuentran
 en el modelo en la propiedad Results ya que se encuentran en Json y poder trabajar con la clase obtenida posteriormente.
Al final del documento está declarada la calse a deserializar mediante la directiva de razor 

    @functions{
    
    }
	
## Consultas a virtuoso
Para este ejemplo vamos a partir de que solo hay una única llamada del tipo sparql.
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

En está página se han añadido varios using para poder deserializar el contenido que hay en el primer elemento de la propiedad Results del modelo, el cual está en formato csv, y posteriormente trabjar con los datos obtenidos al deserializar.
A continación se indican los using importados en las vistas para el tratamiento de los datos llegados en csv.

    @using CsvHelper
	@using System.Globalization
	@using System.IO
	@using System.Text
	
Al final del documento está declarada la calse a deserializar mediante la directiva de razor 

    @functions{
    
    }
	
## Consultas mixtas
Se puede encontrar este ejemplo en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/FrontEndCarga/paginas/directivas.cshtml

En este ejemplo se han puesto todos los using vistos en este documento para tratar información tanto en json como csv y se han usado las dos directivas vistas, para hacer una llamada a un api 
y hacer una consulta a virtuoso. En este caso en la propiedad Results del modelo tenemos una lista de dos objetos, el primer elemento de la lista corresponde al resultado en json de la llamada api,
ya que la primera directiva es `@*<% api https://localhost:44359/Job?type=0&count=100&from=0 /%>*@` y en el segundo elemento de la lista el resultado en csv de la consulta a virtuoso, ya que la segunda llamada usada es del tipo sparql

    @*<% sparql 
    select ?type count(?s) as ?count 
    where
    {
    ?s rdf:type ?type
    }
    group by ?type
    /%>*@
	
En el caso de que se que quieran hacer más llamadas el orden de recogida de los datos equivale al orden en el que se uasn esas en llamadas en la vista.
