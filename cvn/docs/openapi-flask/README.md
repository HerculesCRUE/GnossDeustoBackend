# Servidor de Swagger 

## Requisitos
Python 3.5.2+

## Ejecución
Para lanzar el servidor, ejecutar desde esta carpeta:

```
pip3 install -r requirements.txt
python3 -m swagger_server
```

y luego acceder a la web desde:

```
http://localhost:8080/v1/ui/
```

El archivo swagger está en:

```
http://localhost:8080/v1/swagger.json
```

## Docker

```bash
docker build -t swagger_server .
docker run -p 8080:8080 swagger_server
```
