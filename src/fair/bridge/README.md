# *bridge*

Enlace con el API del servidor público de FAIR Metrics, desplegado en:

http://herc-as-front-desa.atica.um.es/bridgeswagger/v1/ui/#/

## Despliegue

### Requisitos

- Python 3
- Pipenv

### Instalación y ejecución

``` bash
pipenv install
pipenv shell
./app.py ORCID
```

> Sustituir `ORCID` por el ORCID de la persona que ejecuta o supervisa la ejecución. El servidor de backend público
> lo requiere.

Se abrirá un servidor local:

http://localhost:5200

#### Configuración

Es posible configurar hasta cierto punto algunos parámetros de ejecución. Para ver las opciones disponibles, ejecutar el
servidor con la opción `-h`, para ver la ayuda:

```
usage: app.py [-h] [-p] [--host HOST] [--debug] [--server-url SERVER_URL]
              orcid

Servidor HTTP que ofrece una API simple para interactuar con un servicio de
métricas FAIR

positional arguments:
  orcid                 ORCID de la persona que ejecuta el test (requerido por
                        el backend)

optional arguments:
  -h, --help            show this help message and exit
  -p , --port           El puerto en el que se ejecutará el servidor HTTP (por
                        defecto 5200)
  --host HOST           El host donde se bindeará el servidor HTTP (por
                        defecto 127.0.0.1)
  --debug               DEBUG: activar modo debug (aumenta tiempo de
                        ejecución)
  --server-url SERVER_URL
                        URL del backend de FAIR (no incluir / final)
```

> :warning: Si se especifica una URL de un servidor, NO poner `/` al final de la URL.

### Ejemplos de ejecución

Para probar el servidor ejecuta el siguiente comando:
- Recuperar las colecciones existentes de test suites FAIR a ejecutar:
	curl -X GET "http://localhost:5200/v1/collections" -H "accept: application/json"
- Ejecutar la colección de test suite seleccionada:
	curl -X POST "http://localhost:5200/v1/collections/5/evaluate?resource=10.1109%2FACCESS.2019.2952321&orcid=0000-0001-8055-6823&title=prueba" -H "accept: application/json"
- Recuperar el histórico de evaluaciones ejecutadas por un investigador (ORCID):
	curl -X GET "http://localhost:5200/v1/evaluations" -H "accept: */*"
- Recuperar el resultado en detalle de la ejecución anterior de una colección de tests:
	curl -X GET "http://localhost:5200/v1/evaluations/3263/result" -H "accept: */*"
	

Para generar el front-end swagger hacer lo siguiente:
- Ejecutar comando: openapi-generator generate -i openapi.yaml -g python-flask -o codegen_server/
- Copiar los contenidos del fichero codegen_server/openapi_server/controllers/default_controller_asio.py al fichero automáticamente generado por openapi-generator codegen_server/openapi_server/controllers/default_controller.py
- Ejecutar en directorio codegen_server/ el comando: python -m openapi_server
- Acceder en el navegador a la dirección http://localhost:8080/v1/ui/
- Probar la API a través de la interfaz Swagger generada

El fichero bridge/openapi.yaml contiene la especificación de la API del bridge ASIO-FAIRmetrics acorde con OpenAPI 3.0


## Por hacer/mejoras

- Hacer configurable
  Guardar valores de configuración en un archivo aparte
- Mejoras en la filtración de evaluaciones
  - Cambiar el filtrado para que muestre solo los tests ejecutados con esta API
    propia
    - Persistencia de datos
- Cacheado de las peticiones poco propicias a cambiar
  - Listado de colecciones (cambia muy poco)
  - Listado de evaluaciones (tarda mucho en cargar)
  - Resultados test (no cambia nunca)
