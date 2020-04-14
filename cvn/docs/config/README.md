# Configuración del mapeo CVN-ROH

Para que la herramienta no se quede atrás, y debido al gran número de mejoras que la ontología recibe constantemente, se ha visto necesario que la lógica que sigue este programa sea fácilmente configurable, de modo que en el futuro se pueda adaptar para nuevas versiones tanto de la especificación CVN como de la ontología.

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
# Generación de entidades 
