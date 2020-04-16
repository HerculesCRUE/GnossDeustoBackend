# *bridge*

Enlace entre la API del servidor público de FAIR Metrics y el futuro *backend*
de ROH.

## Despliegue

Requisitos

- Python(3)
- Pipenv

Instalación y ejecución

``` bash
pipenv install
pipenv shell
./app.py
```

Se abrirá un servidor local:

http://localhost:5000

Para probar el servidor ejecuta el siguiente comando:
- Recuperar las colecciones existentes de test suites FAIR a ejecutar:
	curl -X GET "http://localhost:5000/v1/collections" -H "accept: application/json"
- Ejecutar la colección de test suite seleccionada:
	curl -X POST "http://localhost:5000/v1/collections/5/evaluate?resource=10.1109%2FACCESS.2019.2952321&orcid=0000-0001-8055-6823&title=prueba" -H "accept: application/json"
- Recuperar el histórico de evaluaciones ejecutadas por un investigador (ORCID):
	curl -X GET "http://localhost:5000/v1/evaluations" -H "accept: */*"
- Recuperar el resultado en detalle de la ejecución anterior de una colección de tests:
	curl -X GET "http://localhost:5000/v1/evaluations/3263/result" -H "accept: */*"
	

Para generar el front-end swagger hacer lo siguiente:
- Ejecutar comando: openapi-generator generate -i openapi.yaml -g python-flask -o codegen_server/
- Copiar los contenidos del fichero codegen_server/openapi_server/controllers/default_controller_asio.py al fichero automáticamente generado por openapi-generator codegen_server/openapi_server/controllers/default_controller.py
- Ejecutar en directorio codegen_server/ el comando: python -m openapi_server
- Acceder en el navegador a la dirección http://localhost:8080/v1/ui/
- Probar la API a través de la interfaz Swagger generada

El fichero bridge/openapi.yaml contiene la especificación de la API del bridge ASIO-FAIRmetrics acorde con OpenAPI 3.0


## Por hacer/mejoras en el futuro

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
