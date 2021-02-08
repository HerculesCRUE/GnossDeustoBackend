Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH.

http://herc-as-front-desa.atica.um.es:8080/v1/ui/#/default/convert_post

# Instalación

Requisitos:

- Python 3
- Pipenv

Instalamos con Pipenv:

```bash
$ pipenv install
```

# Ejecución del servicio

Ejecutar un servidor en el puerto por defecto (`5000`) y con el modo `debug` desactivado:

```bash
$ pipenv run python3 -m cvn.webserver
```

## Configuración

Es posible configurar el puerto de ejecución con el parámetro `-p`.  Si no se especifica nada, el puerto por defecto es `5000`. 
En el siguiente ejemplo, se ejecutará el servidor en el puerto `80`.

```bash
$ pipenv run python3 -m cvn.webserver -p 80
```

Si se incluye el argumento `-h`, se mostrará la ayuda:

```text
usage: webserver.py [-h] [-p] [--host HOST] [--debug]

Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH

optional arguments:
  -h, --help    show this help message and exit
  -p , --port   El puerto en el que se ejecutará el servidor HTTP (por defecto
                5000)
  --host HOST   El host donde se bindeará el servidor HTTP (por defecto
                127.0.0.1)
  --debug       DEBUG: activar modo debug (aumenta tiempo de ejecución)
```

Si queremos que el servidor escuche en `0.0.0.0`, añadimos el argumento `--host 0.0.0.0`.

# Endpoints

## `/v1/convert`

- Método: `POST`

Parámetros:

| key | ¿Obligatorio? | descripción |
|:--|:--|:--|
| `orcid` | Sí | La ORCID de la persona de la que es el CVN.<br>Ejemplo: `0000-0001-8055-6823`
| `format` | No | Valor por defecto: `xml`<br>Posibles formatos: <ul><li>`xml`</li><li>`n3`</li><li>`turtle`</li><li>`nt`</li><li>`pretty-xml`</li><li>`trix`</li><li>`trig`</li><li>`nquads`</li></ul>

El contenido del archivo que queremos debe enviarse como el cuerpo, en codificación binaria (ver ejemplos más abajo) y estando en UTF-8.

### Posibles errores

- `An xml file is required as binary body data.` \
    No se está enviando bien el archivo, ver ejemplos más abajo. Debe ser el cuerpo de la solicitud.

- `The orcid parameter is required.` \
    El parámetro ORCID debe ser indicado como un parámetro HTTP más, y es obligatorio.

- `The orcid field has an invalid format.` \
    El ORCID indicado no sigue el formato. Ejemplo de ORCID: `0000-0001-8055-6823`. 
    El RegEx que usa el programa es [`0000-000(1-[5-9]|2-[0-9]|3-[0-4])\d{3}-\d{3}[\dX]`](https://regex101.com/r/w6sa8Q/2)

- `The format field has an unsupported format.` \
	El parámetro format tiene un valor fuera de los permitidos. Ver tabla más arriba para una lista completa de los formatos soportados.

- `Error while parsing the XML.` \
    El XML no es válido o no ha sido posible procesarlo (ej.: extremadamente grande)

### Ejemplos de consultas

#### curl

Suponiendo que el servidor está en `127.0.0.1:5000`, la siguiente llamada usando la utilidad de consola `curl` convertirá
el archivo en `examples/cvn_202033-Diego.xml`

```bash
curl --location --request POST 'http://127.0.0.1:5000/v1/convert?orcid=0000-0001-8055-6823' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-binary '@examples/cvn_202033-Diego.xml'
```

> Recomendable usar en Windows `"` en vez de `'`, el comando sería:
> `curl --location --request POST "http://127.0.0.1:5000/v1/convert?orcid=0000-0001-8055-6823" --header "Content-Type: application/x-www-form-urlencoded" --data-binary "@examples/cvn_202033-Diego.xml"`

#### C# - RestSharp

Similar a la de `curl`.  
(sustituyendo `<contenido del archivo aquí>` por el contenido del archivo que queremos convertir)

```cs
var client = new RestClient("http://127.0.0.1:5000/v1/convert?orcid=0000-0001-8055-6823");
client.Timeout = -1;
var request = new RestRequest(Method.POST);
request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
request.AddParameter("application/x-www-form-urlencoded", "<contenido del archivo aquí>", ParameterType.RequestBody);
IRestResponse response = client.Execute(request);
Console.WriteLine(response.Content);
```

# Ejecución de la documentación de la API

Existe un servidor de OpenAPI que va incluido en los archivos. Tiene los mismos requisitos que la API. Podemos ejecutarlo de la siguiente manera:

```bash
$ cd docs/api/flask
$ pipenv install
$ pipenv run python3 -m openapi_server
```

Se puede acceder desde la siguiente URL:

http://localhost:8080/v1/ui

Se puede ejecutar a la vez que la API abriendo otra terminal nueva (en Windows, una nueva ventana)
