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
    ViewData["Title"] = "Proyecto ASIO";
    ViewData["MetaData"] = "<meta name=\"description\" content=\"Web pública del Proyecto Hércules ASIO de la Universidad de Murcia\">";
    }
	
 - Layout = "_Layout": Con este código indicamos que coja el Layout compartida con todas las páginas del hércules para tener las páginas de forma homogénea.
 - ViewData["BodyClass"] = "fichaRecurso": Añade la clase fichaRecurso al body de la página (él cuál está en el Layout) para poder usar los estilos de la clase de la web.
 - ViewData["Title"]: Añade un título a la página definido por el usuario, este título es visible desde la pestaña del navegador.
 - ViewData["MetaData"]: Añade etiquetas de metadatos a la cabecera
 
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
Al final del documento está declarada la clase a deserializar mediante la directiva de razor 

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
	
Al final del documento está declarada la clase a deserializar mediante la directiva de razor 

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
	
En el caso de que se que quieran hacer más llamadas el orden de recogida de los datos equivale al orden en el que se usan esas llamadas en la vista.

En el siguiente fragmento de código tenemos dos bloques, el primer bloque donde se cogen los resultados de la posición 0 (FirstOrDefault) que pertenecen a una llamada del api, ya que la primera llamada del ejemplo es a un api, una vez cogido los datos en la variable resultApi que posteriormente se hace la deserialización de JSON ya que los apis mandan los resultados en este formato, para obtener nuestro objeto deseado. El segundo bloque pertenece a la obtención de datos de una consulta sparql donde cogemos el elemento que se encuentra en la posición 1 (Model.Results[1]) ya que la segunda llamada en el ejemplo se corresponde con una consulta sparql, una vez obtenidos los resultados en resultSparql, hacemos el tratamiento para leer los datos que vienen en formato CSV.

    string resultApi = Model.Results.FirstOrDefault();
    List<JobPage> resultObject = JsonConvert.DeserializeObject<List<JobPage>>(resultApi);
    var jobs = resultObject.Where(item => item.ExecutedAt.HasValue).OrderByDescending(item => item.ExecutedAt.Value).Take(2);
	
    string resultSparql = Model.Results[1];
    byte[] byteArray = Encoding.UTF8.GetBytes(resultSparql);
    MemoryStream stream = new MemoryStream(byteArray);
    var csvReader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
    var records = csvReader.GetRecords<PruebaSparql>();


## Varias consultas Sparql
Se puede encontrar este ejemplo en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/FrontEndCarga/paginas/Consultas-Sparql.cshtml

En este ejemplo se sigue el mismo proceso que en el ejemplo visto al crear una página con una consulta Sparql, con la salvedad de que en este caso hay que obtener todos los resultados para las consultas realizadas y tratarlos para poder consultar los resultados como se ve en el ejemplo,
en nuestro modelo de la página tendremos en la posición 0 la primera consulta realizada, en la posición 1 la segunda consulta y así sucesivamente. En el siguiente ejemplo tenemos dos bloques de código que los dos hacen referencia a consultas sparql, en el primer bloque se deserealizan los datos obtenidos de la primera consulta de la página, usando la clase deseada (PruebaSparql, que contiene una propiedad count como el nombre del campo de los datos que le hemos dado en la consulta) para leer los resultados que han venido en formato CSV. En el segundo bloque se sigue la misma dinámica para leer los datos obtenidos de la segunda consulta realizada en la página.


    string result = Model.Results.FirstOrDefault();
    byte[] byteArray = Encoding.UTF8.GetBytes(result);
    MemoryStream stream = new MemoryStream(byteArray);
    var csvReader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
    var records = csvReader.GetRecords<PruebaSparql>();
    
    string result2 = Model.Results[1];
    byteArray = Encoding.UTF8.GetBytes(result2);
    stream = new MemoryStream(byteArray);
    csvReader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
    var records2 = csvReader.GetRecords<investigadoresSparql>();
    
En el caso de que se quiera añadir más consultas basta con seguir el proceso de uno de estos bloques obteniendo los datos de la posicion x `string resultX = Model.Results[x];` y deserealizando usando el CSVReader con el stream de esos datos y una clase con los parámetros del select

## Varias consultas a APIs
Se puede encontrar este ejemplo en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/FrontEndCarga/paginas/llamadas-apis.cshtml

En este ejemplo se sigue el mismo proceso que el visto al crear una página con una llamada a un api, excepto que en este caso hay dos llamadas a un api, por lo que hay que obtener y tratar los resultados obtenidos al hacer
las dos llamadas al api, como en los ejemplos anterior estos resultados obtenidos se corresponderan con la llamada al api según el orden, es decir, en la posición 0 de la lista la primera llamada que aparece en el código de la página, en la posición 1 y así sucesivamente.

    string result = Model.Results.FirstOrDefault();
    List<PageList> resultObject = JsonConvert.DeserializeObject<List<PageList>>(result);
    
    result = Model.Results[1];
    Page resultPage = JsonConvert.DeserializeObject<Page>(result);

En el ejemplo anterior se muestran dos bloques de código donde se recogen datos para dos llamadas a un api, en el primer bloque se cogen los datos de la primera llamada y posteriormente se deserealizan con una clase que sigue el patrón con el que devuelven el api los datos, en el segundo bloque se reutiliza la variable result (debido a que los resultados anteriores los hemos guardado en la variable resultObject) para recoger los datos obtenidos al realizar la llamada al api que aparece en segundo lugar en la página, posteriormente se deserealizan en otra varibale


