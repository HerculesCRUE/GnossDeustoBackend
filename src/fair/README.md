![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 4/5/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Métricas FAIR| 
|Descripción|Documentación y resultados|
|Versión|1.4|
|Módulo|FAIR Metrics|
|Tipo|Documentación|
|Cambios de la Versión|Revisión de enlaces|


# Métricas FAIR

Contamos con la siguiente documentación:

[Documento de Análisis de métodos FAIR](../../Docs/FAIR/Entregable-EF2-1.6-documento-de-analisis-de-metodos-FAIR.md)

[Métricas FAIR. Software y documentación](../../Docs/FAIR/EF2.1-7.-METRICAS-FAIR-I-SOFTWARE-Y-DOCUMENTACION-DE-METRICAS.md)

Hay una instancia disponible de la [aplicación y librería FAIR Metrics](https://github.com/FAIRMetrics/Metrics) 
instanciada en los servidores de la Universidad de Murcia:

http://herc-as-front-desa.atica.um.es/

Como hemos indicado en los documentos citados anteriormente, la intención es incorporar los tests
de evaluación de FAIR del proyecto Hércules dentro del proyecto [FAIR Sharing](https://fairsharing.github.io/FAIR-Evaluator-FrontEnd/#!/#%2F!).

Sin embargo, este interfaz experimenta algunas caídas y problemas de funcionamiento, por lo que se implementará un interfaz completo para las pruebas del Hito 2 del proyecto.

Primeros resultados de métricas FAIR
------------------------------------

Los primeros tests de la ROH están en:

https://fairsharing.github.io/FAIR-Evaluator-FrontEnd/#!/evaluations/4046

Tenemos también los primeros tests de los datos cargados desde la UM, si bien será tarea del hito 2 definir los tests concretos para los datos de ASIO, una vez que se vaya dotando de contenido. Tenemos el siguiente resultado en FAIR Sharing: 

https://fairsharing.github.io/FAIR-Evaluator-FrontEnd/#!/evaluations/4059

Y estos otros en el despliegue local de la librería FAIR Metrics:

http://herc-as-front-desa.atica.um.es/evaluations/9/result

http://herc-as-front-desa.atica.um.es/evaluations/10/result


Librería bridge
---------------

Por último, indicar que disponemos de una [librería "bridge"](https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/src/fair/bridge) que conecta con el API
del proyecto FAIR Sharing y permite ejecutar las métricas programáticamente.
El interfaz swagger de esta librería está accesible en:

http://herc-as-front-desa.atica.um.es/bridgeswagger/v1/ui/#/
