![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 8/2/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Configuración del mapeo CVN-ROH| 
|Descripción|Instrucciones de configuración del mapeo CVN-ROH|
|Versión|1.1|
|Módulo|API Carga|
|Tipo|Manual|
|Cambios de la Versión|Reordenación de la documentación y cambio de rutas|


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


# Generación de entidades

Las entidades se declaran en el archivo `entities.toml`. Se definen en un array llamado `entities`:

```
[[entities]]
(configuración entidad 1)
[[entities]]
(configuración entidad 2)
```

## Configuración

### Código

- `code`: un string, representa el código del bean en el CVN. En las subentidades no es necesario. 

```
[[entities]]
code = "000.010.000.000"
[...]
```

### Displayname

Es la manera de representar qué clase y ontología vamos a usar. Es un string: la ontología, luego ":" y finalmente el nombre de la clase.

Por ejemplo: `foaf:Person` o `vivo:dateTime`.

```
[[entities]]
code = "000.010.000.000"
displayname = "foaf:Person"
[...]
```

Para permitir tener *backwards compatibility* con versiones antiguas de este programa, es posible declarar el displayname por separado (ontology y classname), aunque está *deprecated* y no debe hacerse así más.

### Entidad primaria

Una entidad puede ser marcada como primaria poniendo `primary` a `true`. Debe haber siempre una entidad primaria.
No es necesario poner el ajuste si el valor va a ser `false`. La entidad primaria se genera antes que las demás y es posible hacer relaciones directas a ella sin necesidad de crear subentidades.

```
[[entities]]
code = "000.010.000.000"
displayname = "foaf:Person"
primary = true
[...]
```

### Generación de URIs

Para la generación se usa el servicio UrisFactory.

- `id.resource`: Un string. El nombre del recurso en la URI (suele coincidir con el nombre de la clase). Si una entidad no tiene `id.resource` se generará como un blank node, sin URI.

- `id.format`: el identificador de la entidad. Un string. Si se deja vacío, se generará una ID única (UUID4). Se puede especificar el formato de la ID generada. Podemos incluir una o varias propiedades en lugar de una UUID. Para ello, ponemos el displayname de la propiedad entre corchetes. Ejemplo: `id.format = "{vivo:doi}`

```
[[entities]]
code = "060.010.010.000"
displayname = "bibo:AcademicArticle"
id.resource = "Article"
id.format = "{vivo:doi}"
```

### Caché

Debido a la cantidad de datos duplicados e independientes que existen en un CVN, existe la posibilidad de evitar generar dos entidades que tengan el mismo valor para una propiedad. De esta manera podemos guardar bajo la misma instancia varias personas con el mismo nombre. Ejemplo:

```
[[entities.subentities.subentities]]
displayname = "foaf:Person"
id.resource = "Person"
cache = "foaf:name"
```

## Propiedades

Una entidad puede tener propiedades (el equivalente a *data properties*).

Las propiedades se declaran en un subarray dentro de las entidades. Las propiedades, a su vez, tienen fuentes de datos, permitiendo formatear el resultado y mezclar datos de diferentes sitios dentro del Item del CVN.

Por ejemplo:

```
[[entities.properties]]
    displayname = "foaf:name"
    hidden = true
    format = "{name} {familyName} {secondFamilyName}"

        [[entities.properties.sources]]
        code = "000.010.000.020"
        name = "name"

        [[entities.properties.sources]]
        code = "000.010.000.010"
        name = "familyName"
        bean = "FirstFamilyName"

        [[entities.properties.sources]]
        code = "000.010.000.010"
        name = "secondFamilyName"
        bean = "SecondFamilyName"
```

En este ejemplo declaramos la propiedad `foaf:name` y lo formateamos: Nombre Apellido Segundoapellido. Ahora, sacamos esos valores de ciertos códigos en el Item CVN. Cada *Source* puede tener un código y bean diferente.

## Relaciones

Las entidades pueden tener dos tipos de relaciones: con su clase padre (si son subclases) o con la entidad primaria. Cada relación tiene a su vez dos campos: la relación directa (esta clase *x* la otra) e inversa (otra clase *x* esta). Ambas son opcionales: puede declararse solo directa, solo inversa o directa e inversa. Las relaciones equivalen a *object properties*. Ejemplos:

De una relación con la entidad primaria:

```
[[entities]]
code = "060.010.010.000"
ontology = "bibo"
classname = "AcademicArticle"
id.resource = "Article"
id.format = "{vivo:doi}"

    [[entities.relationships]]
    ontology = "roh"
    name = "correspondingAuthor"
    inverse_ontology = "roh"
    inverse_name = "correspondingAuthorOf"
    link_to_cvn_person = true
```
Aquí creamos un *AcademicArticle* y lo enlazamos con la entidad primaria (normalmente será la persona del CVN)

En el siguiente ejemplo hay dos relaciones con las clases padres: un *DateTimeInterval* se enlaza con la clase superior, y a su vez, sus subclases, con ella:

```
[[entities.subentities]]
    ontology = "vivo"
    classname = "DateTimeInterval"

        [[entities.subentities.relationships]]
        inverse_ontology = "vivo"
        inverse_name = "dateTimeInterval"

        [[entities.subentities.subentities]]
        ontology = "vivo"
        classname = "DateTimeValue"

            [[entities.subentities.subentities.relationships]]
            inverse_ontology = "vivo"
            inverse_name = "start"
```


## Subentidades

Las entidades pueden tener subentidades (con sus propiedades, condiciones, relaciones), y éstas pueden tener, a su vez, más subentidades (y subsubentidades, etc.)

Se declaran en el subarray `subentities`:

```
[[entities]]
code = "030.010.000.000"
displayname = "vivo:TeacherRole"

    [[entities.relationships]]
    direct = "oboro:RO_000052"
    inverse = "oboro:RO_000053"
    link_to_cvn_person = true

    [[entities.subentities]]
    displayname = "vivo:Course"

        [[entities.subentities.relationships]]
        direct = "obobfo:BFO_0000055"
        inverse = "obobfo:BFO_0000054"

        [[entities.subentities.properties]]
        displayname = "roh:title"
        format = "{title}"

            [[entities.subentities.properties.sources]]
            code = "030.010.000.160"
            name = "title"
```
En este ejemplo creamos una entidad padre *vivo:TeacherRole*, y una subentidad *vivo:Course* y las relacionamos entre sí. La subentidad tiene, a su vez, una propiedad.
