![](.//media/CabeceraDocumentosMD.png)

| Fecha         | 01/10/2020                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Fuentes externas de información para las funciones de descubrimiento| 
|Descripción|El documento indica que fuentes se usan y justifica cuáles se han descartado|
|Versión|0.1|
|Módulo|API DISCOVER|
|Tipo|Documento|
|Cambios de la Versión|Creación|

# Hércules Backend ASIO. Fuentes externas de información para las funciones de descubrimiento

[Criterios de utilización](introduccion-y-criterios-de-utilización#)

[Fuentes utilizadas](#fuentes-utilizadas)

[Fuentes descartadas](#fuentes descartadas)

[Fuentes dudosas](#fuentes dudosas)

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

Fuentes descartadas
===================
**Google Scholar**. No dispone de API.

**Microsoft Academic**. No es tan completo con los títulos en español. Si bien el uso del API (MAKES)
es gratuito, necesita una suscripción de Azure y tendría el coste de una cuenta de almacenamiento.

**ResearchGate**. De momento no dispone de API.

**Dimensions**. Contiene información de libros y publicaciones, pero el API es de pago.

**Europeana** (incluye la antigua European Library). Dispone de un API, pero no proporciona todos 
los autores. Habitualmente sólo figura uno de los investigadores como _contributor_.

**Dialnet**. No dispone de un API. Se ha contactado con la fundación, pero de momento no
es posible usarlo como fuente.

**Global Register of Publishers** ([https://grp.isbn-international.org/](https://grp.isbn-international.org/)). Contiene libros internacionales, 
con poco contenido en español. Mo tiene API.

Fuentes dudosas
===============
Se trata de fuentes que no tienen API, pero se podría hacer _scraping_ en sus webs, para obtener 
la información. Estos procesos tienen problemas de mantenibilidad, ya que pueden dejar de funcionar 
ante cambios menores en el HTML.

**Recolecta** ([FECYT https://recolecta.fecyt.es/](https://recolecta.fecyt.es/)). Las condiciones de uso no impiden la realización 
de _scraping_ sobre el contenido de la web.

**Libros editados en España** [http://www.mcu.es/webISBN/tituloSimpleDispatch.do](http://www.mcu.es/webISBN/tituloSimpleDispatch.do)). Las condiciones
de uso no impiden la realización de _scraping_ sobre el contenido de la web.

**DOAB** ([https://www.doabooks.org/doab?uiLanguage=en](https://www.doabooks.org/doab?uiLanguage=en) Directorio de libros de acceso abierto. Las condiciones de uso
no impiden la realización de _scraping_ sobre el contenido de la web.


