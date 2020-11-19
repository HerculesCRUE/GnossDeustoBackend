![](.//media/CabeceraDocumentosMD.png)

| Fecha         | 19/11/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Fuentes externas de información para las funciones de descubrimiento| 
|Descripción|El documento indica que fuentes se usan y justifica cuáles se han descartado|
|Versión|0.2|
|Módulo|API DISCOVER|
|Tipo|Documento|
|Cambios de la Versión|Cambios en Recolecta, Europeana, BNE y Teseo|

# Hércules Backend ASIO. Fuentes externas de información para las funciones de descubrimiento

[Criterios de utilización](introduccion-y-criterios-de-utilización#)

[Fuentes utilizadas](#fuentes-utilizadas)

[Fuentes descartadas](#fuentes-descartadas)

[Fuentes dudosas](#fuentes-dudosas)

Introducción y criterios de utilización
=======================================
El proceso de descubrimiento usa fuentes externas de información para reconciliar, 
descubrir y detectar equivalencias (ver [Hércules Backend ASIO. Especificación de 
las funciones de descubrimiento](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/H%C3%A9rcules%20ASIO.%20Especificaci%C3%B3n%20de%20las%20funciones%20de%20descubrimiento.md)).

La información proporcionada por estas fuentes permite identificar y desambiguar a los 
investigadores y sus resultados de investigación, y obtener información adicional, 
como los códigos ORCID (investigadores) y DOI (publicaciones), por ejemplo.

Consideramos como fuentes utilizables las que cumplen con las siguientes características:
- Disponen de un API de uso gratuito que permite realizar búsquedas y recuperar la información de los resultados.
- No comprometen un coste adicional para las universidades.
- Contienen información de objetos de investigación y de los autores.
- Contienen información en español y en inglés.

Fuentes utilizadas
==================
**Scopus**. Utilizable mediante un API a la que se tiene acceso desde las universidades españolas, 
con un token de seguridad en cada petición, que será distinto en cada universidad. Contiene
publicaciones de áreas tecnológicas y también de otras áreas, como sociología o
economía.

**Web of Science (WoS)**. Utilizable mediante un API a la que se tiene acceso desde las universidades
españolas, con un token de seguridad en cada petición que será distinto en cada universidad. 
Contiene publicaciones, libros, _proceedings_, patentes y _data sets_; de ciencias, ciencias sociales, 
arte y humanidades.

**PubMed**. Utilizable mediante un API de uso público. Contiene publicaciones de áreas bio y salud.
No codifica los autores, lo que puede dificultar su desambiguación, ya que sólo se dispone del nombre
y los apellidos.

**DBLP**. Utilizable mediante un API de uso público. Contiene publicaciones de ámbito internacional, 
preferentemente en inglés, del área de ciencia de computadores.

**CrossRef**. Utilizable mediante un API de uso público o de pago, con mejor rendimiento. Contiene
publicaciones, artículos, libros, capítulos, _proceedings_, _data sets_, etc.; de áreas tematicas
diversas. Permite recuperar datos de obras y autores.

**Recolecta** ([FECYT https://recolecta.fecyt.es/](https://recolecta.fecyt.es/)). Se accede mediante _scraping_ sobre el contenido de la web,
cuyas condiciones de uso no lo impiden. Contiene publicaciones extraídas de 141 repositorios 
institucionales de España.

**Europeana** (incluye la antigua European Library). Dispone de un API ([https://pro.europeana.eu/page/api-rest-console](https://pro.europeana.eu/page/api-rest-console)), 
pero no siempre proporciona todos  los autores. Los recursos de algunas fuentes sólo incluyen 
uno de los investigadores como _contributor_. Usaremos los recursos que declaren a los autores 
como _creator_ y enlazaremos con el recurso identificado en el portal Linked Open Data de 
Europeana ([https://pro.europeana.eu/page/linked-open-data](https://pro.europeana.eu/page/linked-open-data)).

Fuentes descartadas
===================
**Google Scholar**. No dispone de API.

**Microsoft Academic**. No es tan completo con los títulos en español. Si bien el uso del API (MAKES)
es gratuito, necesita una suscripción de Azure y tendría el coste de una cuenta de almacenamiento.

**ResearchGate**. De momento no dispone de API.

**Dimensions**. Contiene información de libros y publicaciones, pero el API es de pago.

**Dialnet**. No dispone de un API. Se ha contactado con la fundación, pero de momento no
es posible usarlo como fuente.

**Global Register of Publishers** ([https://grp.isbn-international.org/](https://grp.isbn-international.org/)). Contiene libros internacionales, 
con poco contenido en español. No tiene API.

Fuentes dudosas
===============
Se trata de fuentes que no tienen API, pero se podría hacer _scraping_ en sus webs, para obtener 
la información. Estos procesos tienen problemas de mantenibilidad, ya que pueden dejar de funcionar 
ante cambios menores en el HTML.

**Libros editados en España** ([http://www.mcu.es/webISBN/tituloSimpleFilter.do?cache=init&layout=busquedaisbn&language=es](http://www.mcu.es/webISBN/tituloSimpleFilter.do?cache=init&layout=busquedaisbn&language=es)). Las condiciones
de uso no impiden la realización de _scraping_ sobre el contenido de la web.

**DOAB** ([https://www.doabooks.org/doab?uiLanguage=en](https://www.doabooks.org/doab?uiLanguage=en) Es un directorio de libros con acceso abierto. Las condiciones de uso
no impiden la realización de _scraping_ sobre el contenido de la web.

**Biblioteca Nacional** ([http://catalogo.bne.es/uhtbin/webcat](http://catalogo.bne.es/uhtbin/webcat)). Es un buscador de sus colecciones de libros, 
sobre el que se podría hacer _scraping_, que no está prohibido por las condiciones de uso. 
También disponen de un sitio _open_ _linked_ _data_ en [datos.bne.es](https://datos.bne.es). Por lo que 
parece, la web de datos enlazados tiene menos libros que el catálogo web y también menos datos
 en algunos libros. Por ejemplo:
1.	Libro que está en las dos webs pero con menos datos en datos.bne.es
Si buscamos el título "La economía solidaria y su inserción en la formación universitaria" en
 http://datos.bne.es/obras, podemos acceder a sus triples en http://datos.bne.es/obra/XX4129538.ttl. 
 Sin embargo, observamos que aparecen muy pocos datos.

2.	Libro que sólo está en el catálogo web.
Si buscamos el título "Activismo de datos y cambio social: alianzas, mapas, plataformas y acción para 
un mundo mejor" no aparece en http://datos.bne.es/obras. En cambio, sí que lo encuentra en 
http://catalogo.bne.es/uhtbin/webcat.

3.	Libro que están en las dos webs con datos completos en datos.bne.es
Por último, dos títulos que aparecen en ambos sitios web (Datos y Catálogo), "El Consejo de Derechos 
Humanos: oportunidades y desafíos" y "El derecho al ambiente como derecho de participación". Ambos 
documentos aparecen con mucha más información en sus RDFs (http://datos.bne.es/edicion/Mimo0002220308.ttl y 
http://datos.bne.es/edicion/bimo0000394982.ttl).

En resumen, parece que el catálogo web de la BNE contiene datos que podríamos usar pero no todos  
están en el proyecto de datos enlazados ni con el mismo detalle. Por lo tanto, si quisiéramos usarlos  
nos tendríamos que plantear el _scraping_ sobre el catálogo web, que podría tener problemas técnicos.

Pendiente:
> Verificar si el _scraping_ es posible técnicamente, ya que los resultados se muestran tras una
petición POST en la que se envían cookies de sesión.
> Contactar con los propietarios del portal.

**Teseo** ([https://www.educacion.gob.es/teseo/irBusquedaAvanzada.do](https://www.educacion.gob.es/teseo/irBusquedaAvanzada.do)). Es un buscador de
tesis publicadas en España. No dispone de API y el _scraping_, aunque permitido por las condiciones 
de uso, no sería posible por un problema técnico, ya que la lista de resultados que se obtiene, 
incluso si sólo se devuelve uno, no proporciona un enlace hacia cada tesis devuelta, sino que 
se accede mediante un javascript que usa un identificador que no tiene relación con el que luego 
tiene cada tesis en su acceso directo. 

Pendiente:
> Contactar con los propietarios del portal.
