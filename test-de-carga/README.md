![](../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 30/7/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Test de carga de Hércules ASIO| 
|Descripción|Configuración y ficheros de JMeter con los que reproducir el test de carga y documentos con los resultados|
|Versión|1.2|
|Módulo|Documentación|
|Tipo|Manual|
|Cambios de la Versión|Añadidos datos de rendimiento de sistemas|

## TEST DE CARGA DE HÉRCULES ASIO

Para ejecutar los tests de carga es necesario descargar [JMeter en su versión 5.4.1](https://jmeter.apache.org/download_jmeter.cgi).

Los datos brutos de rendimiento de los sistemas están disponibles en [20210724 Hércules ASIO Pruebas de rendimiento Uso de servidores.xlsx](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/20210724%20H%C3%A9rcules%20ASIO%20Pruebas%20de%20rendimiento%20Uso%20de%20servidores.xlsx)

Disponemos de los siguientes tests:
- [De 5 a 10 hilos](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/test-de-carga/hercules-5-10-hilos), con peticiones HTML de la web pública y del Linked Data Server.
  - El archivo para repetir las pruebas es [hercules-5-10-hilos.jmx](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-5-10-hilos/hercules-5-10-hilos.jmx)
- [De 10 a 20 hilos](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/test-de-carga/hercules-10-20-hilos), con peticiones HTML de la web pública y del Linked Data Server.
  - El archivo para repetir las pruebas es [hercules-10-20-hilos.jmx](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos/hercules-10-20-hilos.jmx)
  - Los gráficas de rendimiento de los sistemas se pueden consultar en:
    - [Frontal 1 10-20 hilos CPU.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos/Frontal%201%2010-20%20hilos%20CPU.png)
    - [Frontal 1 10-20 hilos Mem.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos/Frontal%201%2010-20%20hilos%20Mem.png)
    - [Frontal 2 10-20 hilos CPU.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos/Frontal%202%2010-20%20hilos%20CPU.png)
    - [Frontal 2 10-20 hilos Mem.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos/Frontal%202%2010-20%20hilos%20Mem.png)
    - [RDF Store 1 10-20 hilos CPU.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos/RDF%20Store%201%2010-20%20hilos%20CPU.png)
    - [RDF Store 1 10-20 hilos Mem.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos/RDF%20Store%201%2010-20%20hilos%20Mem.png)
    - [RDF Store 2 10-20 hilos CPU.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos/RDF%20Store%202%2010-20%20hilos%20CPU.png)
    - [RDF Store 2 10-20 hilos Mem.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos/RDF%20Store%202%2010-20%20hilos%20Mem.png)
- [De 10 a 20 hilos](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos-linkeddata), con peticiones de datos RDF del Linked Data Server.
  - El archivo para repetir las pruebas es [hercules-10-20-hilos-linkeddata.jmx](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos-linkeddata/hercules-10-20-hilos-linkeddata.jmx)
  - Los gráficas de rendimiento de los sistemas se pueden consultar en:
    - [Frontal 1 10-20 hilos Linked Data CPU.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos-linkeddata/Frontal%201%2010-20%20hilos%20Linked%20Data%20CPU.png)
    - [Frontal 1 10-20 hilos Linked Data Mem.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos-linkeddata/Frontal%201%2010-20%20hilos%20Linked%20Data%20Mem.png)
    - [Frontal 2 10-20 hilos Linked Data CPU.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos-linkeddata/Frontal%202%2010-20%20hilos%20Linked%20Data%20CPU.png)
    - [Frontal 2 10-20 hilos Linked Data Mem.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos-linkeddata/Frontal%202%2010-20%20hilos%20Linked%20Data%20Mem.png)
    - [RDF Store 1 10-20 hilos Linked Data CPU.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos-linkeddata/RDF%20Store%201%2010-20%20hilos%20Linked%20Data%20CPU.png)
    - [RDF Store 1 10-20 hilos Linked Data Mem.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos-linkeddata/RDF%20Store%201%2010-20%20hilos%20Linked%20Data%20Mem.png)
    - [RDF Store 2 10-20 hilos Linked Data CPU.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos-linkeddata/RDF%20Store%202%2010-20%20hilos%20Linked%20Data%20CPU.png)
    - [RDF Store 2 10-20 hilos Linked Data Mem.png](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-10-20-hilos-linkeddata/RDF%20Store%202%2010-20%20hilos%20Linked%20Data%20Mem.png)
- [De 13 a 25 hilos](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-13-25-hilos-linkeddata), con peticiones de datos RDF del Linked Data Server.
  - El archivo para repetir las pruebas es [hercules-13-25-hilos-linkeddata.jmx](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/test-de-carga/hercules-13-25-hilos-linkeddata/hercules-13-25-hilos-linkeddata.jmx)
