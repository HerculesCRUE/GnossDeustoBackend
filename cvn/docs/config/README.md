# Configuración del mapeo CVN-ROH

Para que la herramienta no se quede atrás, y debido al gran número de mejoras que la ontología recibe constantemente, se ha visto necesario que la lógica que sigue este programa sea fácilmente configurable, de modo que en el futuro se pueda adaptar para nuevas versiones tanto de la especificación CVN como de la ontología.

# Sobre el formato CVN

- Versión actual: `1.4.2_sp1`

En este caso, cada currículum se contiene en un archivo XML lleno de elementos que cuelgan todos de uno de tipo `CVN`. En su interior, son todo elementos de tipo `CvnItemBean`, cada uno con un código identificativo único previamente definido en la especificación de la versión por medio de un elemento hijo de tipo `Code`.

Los `CvnItemBean` tienen a su vez clases anidadas, las cuales son las que guardan los datos como tal. Según el tipo de dato que guardan, el tipo del elemento XML será diferente: `CvnString`, `CvnPhoneBean`, etc. Para el caso de estos subelementos, también se indica el código de la misma manera: a través de un elemento de tipo `Code`. 

Un ejemplo (sin datos específicos) sobre cómo sería una estructura típica de un CVN. (comentarios con `#`):

```
CVN
\-- CvnItemBean
    \-- Code # 000.010.000.000
    \-- CvnString
    	\-- Code # 000.010.000.020
    	\-- Value # Roberto
\-- CvnItemBean
    \-- Code 
    \-- CvnPhoneBean
    	\-- Code # 000.010.000.240
    	\-- Number # 612345689
```

Para ver las equivalencias de los códigos se puede consultar el archivo `mappings/cvn/(versión CVN)/XSD/SpecificationManual.xml`. Basta con hacer una simple búsqueda dentro del documento. Por ejemplo:

- `000.010.000.000`: Identificación CVN
- `000.010.000.020`: Nombre
- `000.010.000.240`: Teléfono móvil

Los códigos indican también el nivel de profundidad en el que estamos. Cuando acaba en tres/seis ceros consecutivos, se entiende que se trata de un código de categoría.

# Resumen del funcionamiento

Para la transformación de los archivos, se han definido una serie de ficheros de configuración para cada versión del CVN 

> Actualmente, el programa solo funciona para la versión `1.4.2_sp1` de CVN. En el futuro se implementará la detección de versiones y la posibilidad de definir mappings para cada una de ellas.

# Estructura de archivos

Todos los archivos de configuración están ubicados en la carpeta `mappings/(versión actual cvn)/cvn-to-roh`. En su interior encontramos los siguientes archivos:

- `1-personal-data.toml` \
	Sección especial, en la que se definen todos los datos personales que deben extraerse del CVN y enlazarlos a la persona dueña del CVN.

- `2-entities.toml`
	Generación de entidades, subentidades y relaciones a partir de los datos disponibles en el CVN.

- `ontologies.toml`
	Definición de las ontologías de las que hace uso el mapeo.

# TOML

Para los archivos de configuración se ha optado por el lenguaje [TOML](https://github.com/toml-lang/toml). No es complicado de aprender, aunque recomiendo ver algunos ejemplos ([1](https://github.com/toml-lang/toml#user-content-example), [2](https://learnxinyminutes.com/docs/toml/)) antes de entrar a editarlos.

# Configuración de las ontologías

`ontologies.toml`

En los diferentes archivos de configuración se hace referencia a ontologías: cuando se indica que debe algo debe convertirse en una clase o propiedad, se indica a qué ontología pertenece por medio de un código corto o indicador. Por ejemplo, el indicador de la ontología *Friend of a Friend* es `foaf`.

Si se va a usar una clase o propiedad (de dato u objeto) en la configuración, es necesario que haya sido declarada previamente en este archivo.

Para definir, por ejemplo, el uso de la ontología `foaf`, se haría añadiendo lo siguiente al archivo:

```toml
[[ontologies]]
shortname = "foaf"
uri_base = "http://xmlns.com/foaf/0.1/"
```

Parámetros:

- `shortname`: el indicador corto. Se usará para referenciar la ontología dentro de la configuración.
- `uri_base`: el prefijo de la URI a usar para la ontología. El nombre de la clase o propiedad se insertará inmediatamente después. \
    Por ejemplo: para la clase `foaf:Person`, y con la URI declarada en el ejemplo, el resultado será `http://xmlns.com/foaf/0.1/Person`.
- `primary`: indica si es la ontología primaria (ROH). Solo debe haber una ontología con este valor como `true`. No es necesario ponerlo para las demás.

Otro ejemplo de configuración completo:

```toml
[[ontologies]]
shortname = "roh"
uri_base = "https://purl.org/roh/"
primary = true

[[ontologies]]
shortname = "bibo"
uri_base = "http://purl.org/roh/mirror/bibo/"

[[ontologies]]
shortname = "vivo"
uri_base = "http://purl.org/roh/mirror/vivo#"

[[ontologies]]
shortname = "foaf"
uri_base = "http://xmlns.com/foaf/0.1/"
```

Observa cómo se repite el encabezado `[[ontologies]]` en cada ontología, indicando que se trata de un elemento de un *array*.


> En proceso de redacción

# Generación de datos personales

`1-personal-data.toml`

En esta sección se extraen ciertos datos simples de una sección especial en el CVN dedicada exclusivamente a los datos personales de la persona del currículum. Dichos datos se guardan específicamente en un *Item* dentro del árbol XML. Como posteriormente se va a referenciar a la persona del CVN en muchos sitios de la conversión se ha visto necesario hacer este paso uno aparte, de manera que sea lo primero que tengamos listo.

El archivo está dividido en dos partes:

- Metadatos de la generación de datos personales \
	Para indicar de dónde sacar los datos y a qué clase convertirlos
- Parámetros a extraer \
	Códigos y formatos de los parámetros y a qué propiedades convertirlos

## Metadatos de la generación de datos personales

En las primeras líneas de código que tiene este archivo es donde se indica el código del CVN del elemento XML donde se guardan los datos personales, y la clase que se debe generar.

```
code = "000.010.000.000"
instance.ontology = "roh"
instance.class = "Person"
```

- `code`: el código del `CvnItemBean` de donde vamos a sacar la información
- `instance`
	- `instance.ontology`: el nombre corto de ontología (previamente definida) de la clase que se va a crear
	- `instance.class`: el nombre de la clase que queremos crear

## Extracción de propiedades

> En proceso de redacción

# Generación de entidades 

`2-entities.toml`

> En proceso de redacción