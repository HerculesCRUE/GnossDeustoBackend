# Hércules Backend ASIO. EF2.1-7. Métricas FAIR I- software y documentación de métricas

Contents {#contents .TOC-Heading}
========

[**1.** **Introducción** 2](#introducción)

[**1.1.** **Aplicación de los principios FAIR en HERCULES-ASIO**
2](#aplicación-de-los-principios-fair-en-hercules-asio)

[**2.** **Metodología para la implementación de las métricas FAIR**
5](#metodología-para-la-implementación-de-las-métricas-fair)

[**2.1.** **Especificación del módulo de métricas FAIR para evaluación
automatizada de los recursos (ontologías o datos)**
5](#especificación-del-módulo-de-métricas-fair-para-evaluación-automatizada-de-los-recursos-ontologías-o-datos)

[**2.2.** **Estado del proyecto FAIRmetrics y análisis de su
repositorio**
7](#estado-del-proyecto-fairmetrics-y-análisis-de-su-repositorio)

[**2.3.** **Decisión de la estrategia de implementación y evaluación de
políticas FAIR**
9](#decisión-de-la-estrategia-de-implementación-y-evaluación-de-políticas-fair)

[**3.** **Implementación del Bridge API RESTful**
11](#implementación-del-bridge-api-restful)

[**3.1.** **Implementación del puente entre ASIO y FAIRMetrics**
11](#implementación-del-puente-entre-asio-y-fairmetrics)

[**3.2.** **Instalación y configuración del puente ASIO-FAIRmetrics**
14](#instalación-y-configuración-del-puente-asio-fairmetrics)

[**4.** **Conclusión** 1](#conclusión)

[**Bibliografía** 2](#bibliografía)

[**Apéndice 1. Métricas FAIR en HERCULES-ASIO**
3](#apéndice-1.-métricas-fair-en-hercules-asio)

**Introducción** 
================

El objetivo de este documento es "Definir y desarrollar un módulo de
métricas FAIR para evaluación automatizada de los recursos (ontologías o
datos)".

Recordemos que los principios FAIR\[1\] (Findable, Accesible,
Interoperable, Reusable) proporcionan directrices para la publicación de
recursos digitales tales como conjuntos de datos, códigos, flujos de
trabajo y objetos de investigación, de manera que sean localizables,
accesibles, interoperables y reutilizables (FAIR). Tales principios se
refieren a tres tipos de entidades: a) *datos* (o cualquier objeto
digital), b) *metadatos* (información sobre ese objeto digital) e c)
*infraestructura*. Por ejemplo, el principio "F4 -- (Meta)data are
registered or indexed in a searchable resource" define que tanto los
metadatos como los datos se registren o indexen en un recurso
encontrable (el componente de infraestructura).

En el entregable "EF2.1-6. DOCUMENTO DE ANÁLISIS DE MÉTODOS FAIR" se
realizó un análisis exhaustivo de las métricas FAIR y su aplicabilidad
al proyecto. El Apéndice 1 incluye el listado de las 14 métricas
descritas FAIR y su interpretación dentro de HERCULES-ASIO.

En las siguientes subsecciones ofrecemos, primero, el contexto en el que
se ubica este entregable y, en segundo lugar, exponemos lo que se ha
planteado exactamente dentro del módulo de métricas FAIR.

**Aplicación de los principios FAIR en HERCULES-ASIO**
------------------------------------------------------

El objetivo principal del PT1 de este proyecto es crear la
infraestructura ontológica que describa los datos que almacenará el SGI
-- Sistema de Gestión de Investigación -- y que se concretará en la 'Red
de Ontologías Hércules' (ROH). Este PT tiene también la tarea de sentar
las bases y establecer los procedimientos, métricas y evaluación para
alinear el proyecto con los principios FAIR de cara tanto a la
publicación de las ontologías como a la publicación de datos.

Como resultado, este entregable describe un **sistema de comprobación
automatizado del nivel FAIR cumplido por los recursos publicados durante
el desarrollo del proyecto, que devuelve el nivel FAIR alcanzado,
asociado a la versión del proyecto y la fecha en la que se ejecutó la
comprobación**. La solución HERCULES-ASIO resultante debe ofrecer las
siguientes propiedades.

1.  Ofrecerá datos que sean *Findable* (Encontrables) a través de un
    identificador persistente e incluyendo metadatos

2.  *Accessible* (Accesible) a través del protocolo universal HTTP

3.  Interoperable usando vocabularios ampliamente adoptados y

4.  Reusable, se publican usando licencias de uso que promocionen la
    reusabilidad, como por ejemplo Creative Commons 4.0 BY-SA.

Hoy en día, la mejor manera de publicar datos FAIR es hacerlo mediante
Linked Data, teniendo especial cuidado de generar datos y metadatos de
alta calidad, mejorando así la reusabilidad de los datos para máquinas
(y, como consecuencia y en última instancia, para humanos).

El **Sistema de Comprobación FAIRness Automatizado en HERCULES-ASIO**,
descrito en este entregable, está diseñado para ser administrable desde
el propio back-end a través de una interfaz de web privada. Al igual que
el resto de componentes que desarrollaremos, tendrá una arquitectura SOA
(Service Oriented Architecture) que permitirá su reutilización desde
otros sistemas mediante llamadas a las funciones del API. También sería
reutilizable en otros proyectos adaptando el código publicado.

Este entregable es el segundo de los cuatro que abordan la alineación de
la solución propuesta con los principios FAIR:

1.  Análisis de métodos FAIR (Entregable: documento de análisis). Este
    es el documento presente. Entregado en diciembre 2019.

> **Entregable [EF2-1.6]{.underline}**: documento de análisis de métodos
> FAIR

2.  Definir y desarrollar un módulo de métricas FAIR para evaluación
    automatizada de los recursos (ontologías o datos). Se trata del
    presente entegable.

> **Entregable [EF2-1.7]{.underline}**: ***Métricas FAIR I- software y
> documentación de métricas***

3.  Publicar los resultados de ejecutar las métricas FAIR

> **Entregable [EF2-1.8a]{.underline}**: Métricas FAIR II - Resultados
> publicados en la Web de la evaluación con métricas FAIR, versión 1
> (primer ciclo de desarrollo realizado - 80%). Fecha por determinar,
> desde Mayo 2020.
>
> **Entregable [EF2-1.8b]{.underline}**: Métricas FAIR II - Resultados
> publicados en la Web de la evaluación con métricas FAIR, versión 2.
> Fecha por determinar, desde Mayo 2020.

Este entregable describe la creación de un *sistema automatizado de
comprobación de métricas FAIR que obtendrá información acerca del
cumplimiento de los principios FAIR*. La definición precisa de las
métricas se ha realizado en base a las métricas definidas en el proyecto
FAIR Metrics \[2\], \[3\] y en métricas adicionales definidas por la UTE
(pendientes de consideración).

Además de la obtención de las métricas FAIR descritas en el proyecto
FAIR Metrics \[2\], \[3\] proponemos que el sistema de comprobación
automatizado genere otros indicadores o métricas acerca del cumplimiento
de los principios FAIR, que serán especificadas al dominio SGI tras
haber primero abordado y garantizado que se ofrecen las 14 métricas
formuladas por FAIRmetrics.

**Metodología para la implementación de las métricas FAIR**
===========================================================

Esta sección describe el proceso seguido en la implementación del módulo
de verificación de las métricas FAIR para el proyecto HERCULES-ASIO.

La metodología aplicada para dar lugar a la primera versión de las
métricas FAIR para HERCULES-ASIO ha consistido en 3 pasos:

1.  Especificación del módulo de métricas teniendo en cuenta la
    especificación definida en el entregable EF2-1.6

2.  Evaluación del estado del proyecto FAIRmetrics y análisis de su
    repositorio de código fuente con la implementación de las métricas
    de referencia FAIR: <https://github.com/FAIRMetrics>

3.  Decisión de estrategia de implementación y evaluación de políticas
    FAIR tanto sobre la Red de Ontologías Hércules (ROH) como a los
    recursos modelados a partir de ROH.

    1.  **Especificación del módulo de métricas FAIR para evaluación automatizada de los recursos (ontologías o datos)**
        ----------------------------------------------------------------------------------------------------------------

El resultado final será la generación de un test-suite que evalúe las 14
métricas descritas en el entregable "EF2-1.6: documento de análisis de
métodos FAIR", así como otras adicionales, asociadas a los sistemas de
gestión de investigación, que surjan como resultado de plasmar este
modelo teórico de métricas en realidad y efectuar la validación del
grado de cumplimiento FAIR tanto de la red de ontologías como de los
recursos modelados sobre ella.

En el despliegue de la solución HERCULES-ASIO se ofrecerá un front-end
(interfaz de usuario) que **facilite el lanzamiento del test suite y la
generación de reportes que informen sobre el grado de cumplimiento de
las métricas FAIR**. Nótese que este front-end web no es parte del
entregable actual. Será proporcionando junto con el back-end de ASIO.
Previsiblemente en verano de 2020.

Es importante remarcar que la entrega actual aporta una interfaz
programática y accesible vía web siguiendo el estándar OpenAPI\[4\].
Además, tales reportes **son archivados (históricos) para poder analizar
el compromiso a lo largo del tiempo de ROH con los principios FAIR**.

A continuación, se enumeran algunas propiedades cualitativas del sistema
de medición de FAIR diseñado:

-   Las métricas deben abordar la multidimensionalidad de los principios
    de FAIR y abarcar todos los tipos de objetos digitales.

-   Las métricas universales pueden complementarse con métricas
    adicionales específicas de los recursos que reflejen las
    expectativas de comunidades particulares. En este caso el dominio de
    las universidades y la gestión de la investigación.

-   Las métricas en sí mismas, y cualquier resultado derivado de su
    aplicación, deben ser FAIR. Por esa razón, siguiendo lo marcado por
    FAIRmetrics hemos adoptado la definición semántica de métricas que
    ya han efectuado los creadores de las métricas FAIR. Por ejemplo,
    <https://purl.org/fair-metrics/FM_R1.3>

-   La evaluación del cumplimiento de métricas será ejecutado sobre
    muestras significativas del ROH para así validar que los principios
    FAIR se siguen cumpliendo en el tiempo. El juego de ensayo elegido
    tomará una muestra significativa de cada una de las entidades
    modeladas en ROH para ejecutar los tests. Tal juego de ensayo será
    descrito y justificado en el entregable futuro EF2-1.8a.

-   Las evaluaciones de imparcialidad deben mantenerse actualizadas, y
    todas las evaluaciones deben ser versionadas, tener un sello de
    tiempo y ser accesibles al público. En la implementación actual de
    FAIRmetrics ya se mantiene un registro histórico de tests. Esta
    funcionalidad será extendida para permitir filtros más avanzados
    para visualizar resultados de evaluación de métricas FAIR. Tales
    extensiones en el módulo de medición de métricas FAIR serán
    entregados junto con EF2-1.8a.

-   Las evaluaciones de imparcialidad, presentadas como una simple
    visualización, serán una poderosa modalidad para informar a los
    usuarios y guiar el trabajo de los productores de recursos
    digitales. Se ofrecerá la capacidad en la solución HERCULES-ASIO
    para visualizar el histórico de cumplimiento de métricas FAIR
    fácilmente visualizable a través de una tabla donde las filas serán
    las fechas en las que ejecutaron los tests y las columnas el grado
    de cumplimiento de cada métrica. Se usarán códigos de colores
    siguiendo la metáfora del semáforo. Actualmente la API provista ya
    ofrece los datos para poder realizar esta visualizión web, que será
    parte de la entrega del back-end de HERCULES-ASIO.

-   El proceso de evaluación, y la evaluación de la imparcialidad
    resultante, deben diseñarse y difundirse de manera que incentiven
    positivamente a los proveedores de recursos digitales; es decir,
    deben considerar que el proceso es justo e imparcial y, además,
    deben beneficiarse de estas evaluaciones y utilizarlas como una
    oportunidad para identificar áreas de mejora.

-   La gobernanza de las métricas, y los mecanismos para evaluarlas,
    serán necesarios para permitir su evolución cuidadosa y abordar los
    desacuerdos válidos.

    1.  **Estado del proyecto FAIRmetrics y análisis de su repositorio**
        ----------------------------------------------------------------

El repositorio de código en GitHub \[5\] para FAIRmetrics[^1] ofrece una
implementación de referencia para las métricas descritas en el artículo
seminal al respecto titulado "*A design framework and exemplar metrics
for FAIRness*" \[6\]. En tal repositorio, se ofrecen implementaciones
tanto en lenguaje Perl como en Ruby, siendo esta última la
implementación que está siendo mantenida por los creadores del
repositorio. La reciente (en el último mes) introducción de
modificaciones en la implementación de las métricas disponibles en Ruby
en el siguiente repositorio
(<https://github.com/FAIRMetrics/Metrics/tree/master/MetricsEvaluatorCode/Ruby/metrictests>)
demuestra que el proyecto sigue estando activo.

En el análisis del código disponible sobre las métricas se han seguido
los siguientes pasos:

1.  *Revisión del código fuente de las 14 métricas implementadas para
    entender su funcionamiento*:
    <https://github.com/FAIRMetrics/Metrics/tree/master/MetricsEvaluatorCode/Ruby/metrictests>.
    Tras este análisis se concluyó que las métricas están
    suficientemente bien implementadas para poder utilizarse, aunque es
    claro que habría que extenderlas en algunos apartados. Por ejemplo,
    actualmente hay soporte específico para algunos formatos de URIs
    ampliamente utilizados como DOI, pero no para algunos más concretos
    al ámbito científico como ORCID. En el apartado de estrategia se
    explica cómo se plantea realizar estas modificaciones.

2.  *Instalación del back-end en servidor donde se alojará el back-end
    de ASIO-HERCULES*. Trabajo todavía en curso. Se está instalando
    distribución disponible en:
    <https://github.com/FAIRMetrics/Metrics/tree/master/MetricsEvaluatorCode/Ruby/fairmetrics>.
    Se trata de una aplicación en Ruby on Rails que ofrece un back-end
    RESTful para bien programáticamente o a través de interfaz web
    acoplada a tal API RESTful, se puedan lanzar la evaluación de las
    métricas FAIR contra recursos.

3.  Uso a través de cliente programático y cliente de línea de comandos
    (CURL\[7\]) de la API ya desplegada de FAIRmetrics en Google App
    Engine \[7\]:
    [https://ejp-evaluator.appspot.com/FAIR\_Evaluator//](https://ejp-evaluator.appspot.com/FAIR_Evaluator/).
    Este paso nos ha permitido demostrar que la funcionalidad ofrecida
    por FAIRmetrics es correcta, aunque refleja el hecho esperable que
    la ejecución de las colecciones de test suites FAIR es costosa en
    tiempo, a menudo requiriendo varios minutos para su compleción. Esto
    se debe al hecho de que por cada recursos explorada es necesario
    varios accesos via HTTP a recursos enlazados.

4.  Uso del front-end web ya disponible en
    <https://fairsharing.github.io/FAIR-Evaluator-FrontEnd/>. Se ha
    contrastado cómo ya es posible ejecutar la validación de principios
    FAIR contra la API RESTful indicada en el anterior paso. Se ha
    evaluado el grado de cumplimiento de los principios FAIR sobre la
    red de ontologías Hércules (ROH). Además, también se ha validado
    contra recursos DOI. Por ejemplo, el resultado de realizar la
    evaluación contra ROH es disponible en:
    <https://fairsharing.github.io/FAIR-Evaluator-FrontEnd/#!/evaluations/3241>

5.  Se ha contactado con el equipo responsable del desarrollo de
    métricas FAIR, concretamente con Mark Wilkinson
    (<mark.wilkinson@upm.es>) para informar que queremos utilizar la
    solución Open Source FAIRmetrics. Hemos propuesto su validación,
    extensión y mejora adoptando los dos canales que la herramienta
    GitHub pone a disposición de los desarrolladores, esto es, a)
    emisión de issues y b) creación de pull requests.

6.  En nuestra conversación con el director del proyecto FAIRmetrics, se
    ha aclarado que es además posible crear nuevas métricas específicas
    al dominio de gestión de datos de la investigación a través de la
    compleción de FAIR Maturity Indicator template[^2]. Se pueden
    realizar extensiones a través de la provisión de un endpoint para
    tales nuevas métricas usando SmartAPI\[8\], extensión semántica de
    OpenAPI. Se ha acordado dialogar con autores de FAIRmetrics en caso
    de detectarse necesidad de ofrecer nuevas métricas, específicas a la
    gestión de información de investigación.

    1.  **Decisión de la estrategia de implementación y evaluación de políticas FAIR**
        ------------------------------------------------------------------------------

Se ha decidido cresar un proxy/bridge API usando OpenAPI, que será
utilizada desde back-end HERCULES-ASIO para realizar validación de
recursos (resources) y de la red de ontologías como tal.

Tal API será ofrecida en primera instancia conectándose con API pública
ya desplegada por FAIRsharing, disponible en
<https://ejp-evaluator.appspot.com/FAIR_Evaluator//>.

La implementación de la API del módulo de métricas FAIR para la
evaluación automática de recursos, se ha realizado de tal modo que es
agnóstica al despliegue del back-end. La decisión del proyecto ha sido
reutilizar la implementación actual e incluso hacer uso del despliegue
de la misma. Sin embargo, es claro que al proyecto no le conviene tener
que depender de terceros para la ejecución de las métricas FAIR. Por
consiguiente, tan pronto esté disponible el entorno de despliegue del
back-end ASIO, será desplegada en la misma la actual implementación de
las métricas FAIR.

Es reseñable que existe ya un front-end web implementado, desplegado y
totalmente funcional en (<https://w3id.org/AmIFAIR>), así como
disponible su código fuente desde
<https://github.com/FAIRsharing/FAIR-Evaluator-FrontEnd>. Este código
será el punto de partida para la interfaz que desde el back-end ASIO se
realizará para simplificar el lanzamiento de la batería de tests de
recursos de muestra y la propia ontología ROH. Tal implementación usará
el código de ejemplo cliente ya disponible que se menciona.

En resumen, las decisiones adoptadas relativas a la implementación del
módulo de validación de métricas FAIR para HERCULES-ASIO han sido:

1.  Crear un reverse proxy o puente RESTful API que permite la
    comunicación entre clientes y la implementación desplegada de las
    métricas FAIR

2.  Desplegar el back-end de FAIR en servidor de desarrollo de
    HERCULES-ASIO y adaptar el reverse proxy RESTful API para que
    propague invocaciones a tal servidor.

3.  Realizar extensiones o corregir errores en la implementación actual
    de FAIRmetrics trasmitiendo las solicitudes de cambios bien a través
    de *issues* de modo documental o con modificaciones concretas de
    código a través de *pull requests*.

4.  Preparar una batería de pruebas incluyendo un conjunto de recursos
    modelados acorde con todas las entidades de ROH y también evaluar la
    propia ontología.

5.  Ejecutar de modo periódico la batería de pruebas y mantener un
    histórico de las ejecuciones y sus resultados asociados, permitiendo
    filtrar y visualizar los resultados históricos.

En el entregable actual se documenta el resultado del paso 1), ver
sección 3. Obsérvese que también se ha abordado parcialmente 5) ya que
se mantiene un registro de las evaluaciones realizadas en el tiempo. Los
pasos 2), 3) y 4) se completarán junto con la entrega del back-end
HERCULES-ASIO.

Un efecto lateral importante, como resultado de realizar la evaluación
FAIR de recursos en ROH será la generación de un conjunto
recomendaciones para hacer ROH y recursos asociados (y por ende otras
ontologías y recursos) compatible con políticas FAIR. Se poblará de este
modo el repositorio
<https://github.com/FAIRMetrics/Your_Path_To_FAIRness/tree/master/MI_Test_Tutorials>
del proyecto FAIRmetrics, así ayudando a otras organizaciones en la
provisión de recursos online siguiendo las directrices FAIR.

**Implementación del Bridge API RESTful** 
=========================================

Tal como se ha mencionado en el punto 2, se ha decidido crear un bridge
o puente entre la API RESTful de FAIRmetrics ya desplegada en la nube
por los creadores de FAIRmetrics y el código cliente que se desarrollará
como parte del back-end de HERCULES-ASIO.

**Implementación del puente entre ASIO y FAIRMetrics**
------------------------------------------------------

La Figura 1 muestra la arquitectura del evaluador de métricas FAIR
diseñado para el proyecto HERCULES-ASIO. Tal como puede apreciarse, el
lado cliente, en la figura el front-end web de ASIO-HERCULES, a través
de comandos HTTP realiza llamadas sobre la API del bridge
ASIO-FAIRmetrics. Tales invocaciones son delegadas bien al despliegue
actual de FAIRmetrics en Google App Engine o al back-end de
ASIO-HERCULES. Tal como puede apreciarse, el back-end de ASIO-HERCULES
incluirá como uno de sus módulos el despliegue de la implementación de
FAIRmetrics.

![](.//media/image2_MetricasFAIR.png)

**Figura** **1**. Interfaz RESTful del bridge ROH-FAIR Metrics.

Las ventajas de crear este bridge son las siguientes:

1.  La API generada ofrece una interfaz sencilla consistente en 4
    métodos que permiten realizar las operaciones para poder automatizar
    las verificación de los principios FAIR dentro de ROH:

    a.  Recuperar las colecciones existentes de test suites FAIR a
        ejecutar

    b.  Ejecutar la colección de test suite seleccionada

    c.  Recuperar el histórico de evaluaciones ejecutadas por un
        investigador (ORCID)

    d.  Recuperar el resultado en detalle de la ejecución anterior de
        una colección de tests

2.  Es muy sencillo cambiar el back-end al que van redirigidas las
    peticiones asociadas a la ejecución de métricas FAIR, a través de un
    simple parámetro. De ese modo, este cambio se efectuará tan pronto
    sea desplegado el back-end de HERCULES-ASIO en la infraestructura de
    la Universidad de Murcia.

En la Figura 2 se muestra el front-end swagger del bridge creado, donde
pueden verse los cuatro métodos ofrecidos por la API:

![](.//media/image3_MetricasFAIR.png)

**Figura** **2**. Interfaz RESTful del bridge ROH-FAIR Metrics.

Los siguientes comandos ilustran cómo desde línea de comando
(herramienta CURL) o desde otras herramientas como Postman y por
supuesto, código fuente, se podría invocar tal API:

-   Recuperar las colecciones existentes de test suites FAIR a ejecutar:

curl -X GET \"http://localhost:8080/v1/collections\" -H \"accept:
application/json\"

-   Ejecutar la colección de test suite seleccionada:

curl -X POST
\"http://localhost:8080/v1/collections/5/evaluate?resource=10.1109%2FACCESS.2019.2952321&orcid=0000-0001-8055-6823&title=prueba\"
-H \"accept: application/json\"

-   Recuperar el histórico de evaluaciones ejecutadas por un
    > investigador (ORCID):

curl -X GET \"http://localhost:8080/v1/evaluations\" -H \"accept:
\*/\*\"

-   Recuperar el resultado en detalle de la ejecución anterior de una
    > colección de tests:

curl -X GET \"http://localhost:8080/v1/evaluations/3263/result\" -H
\"accept: \*/\*\"

Finalmente, la Figura 3 muestra el resultado de ejecutar el método GET
/collections que retorna una colección JSON con todas las colecciones de
test suites registradas en FAIRmetrics.

![](.//media/image4_MetricasFAIR.png)

**Figura** **3**. Ejecución del método GET
[/collections](http://localhost:8080/v1/ui/#/operations/default/collections_get)
a través de la interfaz Swagger.

**Instalación y configuración del puente ASIO-FAIRmetrics**
-----------------------------------------------------------

Antes de instalar y ejecutar el puente ASIO-FAIRmetrics es primero
necesario definir las colecciones de tests de métricas FAIR que quieren
ser lanzadas. Tales colecciones de tests pueden ser configuradas usando
el front-end provisto por la implementación de FAIRmetrics, accesible a
través de <https://fairsharing.github.io/FAIR-Evaluator-FrontEnd/#!/>.
Tal implementación del front-end está actualmente conectado al end-point
desplegado en
[https://ejp-evaluator.appspot.com/FAIR\_Evaluator//](https://ejp-evaluator.appspot.com/FAIR_Evaluator/).
Esto significa que una vez cambiado el endpoint al servidor de
desarrollo de HERCULES-ASIO, la URL del servidor referenciado deberá
modificarse.

La Figura 4 muestra la interfaz web disponible para la configuración de
una batería de tests. Obsérvese que la caja de selección "Select
Maturity Indicators" permite la selección de cada uno de los tests
individuales que configurarán la batería de pruebas. La Figura 5 muestra
algunos de los posibles valores de métricas que puedan ser elegidas.

![](.//media/image5_MetricasFAIR.png)

**Figura** **4**. Interfaz para la definición de una colección de tests
FAIR.

![](.//media/image6_MetricasFAIR.png)

**Figura** **5**. Selección de métricas a incluir en batería de pruebas.

Una vez creada la colección de tests a ejecutar o bien identificado el
ID de aquella colección de tests a ejecutar se puede proceder a la
instalación del puente ASIO-FAIRmetrics. Obsérvese que por ejemplo la
métrica "6 -- All Maturity Indicator Tests as of May 8, 2019"
(<https://fairsharing.github.io/FAIR-Evaluator-FrontEnd/#!/collections/6>),
see Figura 6, incluye todos los tests definidos por el proyecto
FAIRmetrics.

El proceso a seguir para instalar y ejecutar el bridge sería el
siguiente:

1.  Descargar el código de
    <https://github.com/deustohercules/roh/tree/master/fair/bridge>

2.  Ejecutar los siguientes tres comandos para lanzar el puente:

    a.  pipenv install

    b.  pipenv Shell

    c.  python app.py

3.  Seguir las indicaciones de la sección 3.1 para ver ejemplos de uso
    de la API.

Para generar el front-end swagger hacer lo siguiente:

1.  Ejecutar comando: openapi-generator generate -i openapi.yaml -g
    python-flask -o codegen\_server/

2.  Copiar los contenidos del fichero
    codegen\_server/openapi\_server/controllers/default\_controller\_asio.py
    al fichero automáticamente generado por openapi-generator
    codegen\_server/openapi\_server/controllers/default\_controller.py

3.  Ejecutar en directorio codegen\_server/ el comando: python -m
    openapi\_server

4.  Acceder en el navegador a la dirección
    <http://localhost:8080/v1/ui/>

5.  Probar la API a través de la interfaz Swagger generada

El fichero bridge/openapi.yaml contiene la especificación de la API del
bridge ASIO-FAIRmetrics acorde con OpenAPI 3.0

![](.//media/image7_MetricasFAIR.png)

**Figura** **6**. Detalle de métricas a evaluar dentro de colección de
tests 6 -- All Maturity Indicator Tests as of May 8, 2019.

La Figura 7 muestra el resultado de ejecutar las métricas FAIR definidas
en la colección 6 a un recurso representando una publicación (a través
de su DOI). Los detalles del resultado de haber ejecutado tal métrica
puede accederse en:
<https://fairsharing.github.io/FAIR-Evaluator-FrontEnd/#!/evaluations/3264>

![](.//media/image8_MetricasFAIR.png)

**Figura** **7**. Resultado de ejecutar la colección de tests 6 al
recurso DOI
[10.1109/ACCESS.2019.2952321](http://dx.doi.org/10.1109/ACCESS.2019.2952321)
lanzado por investigador 0000-0001-8055-6823.

**Conclusión**
==============

Este documento ha descrito la implementación inicial de las métricas
FAIR, mediante la provisión de un puente o bridge entre el código del
proyecto HERCULES-ASIO y las métricas desarrolladas por el proyecto
FAIRmetrics. Esta primera implementación del evaluador de métricas FAIR
hace uso del código fuente open source de la implementación por
referencia de las métricas FAIR en el repositorio FAIRmetrics. El
objetivo es que durante el desarrollo del proyecto HERCULES-ASIO se
consiga lo siguiente:

-   Incrementar el grado de cumplimiento de las métricas FAIR tanto para
    la red de ontologías ROH como para los recursos modelado con ROH

-   Realizar extensiones de las métricas de FAIRmetrics para el dominio
    de investigación

-   Realizar contribuciones para la depuración de las métricas
    existentes, de ese modo, contribuyendo de vuelta al proyecto
    FAIRmetrics

-   Garantizar un despliegue autónomo de las métricas para asegurar la
    independencia del código de terceros.

Como trabajo futuro deberemos realizar lo siguiente:

-   Crear un cliente sobre interfaz web de HERCULES-ASIO

-   Ejecutar periódicamente tests FAIR y mantener histórico de tales
    ejecuciones.

**Bibliografía**
================

\[1\] M. D. Wilkinson *et al.*, «The FAIR Guiding Principles for
scientific data management and stewardship», *Scientific Data*, vol. 3,
n.^o^ 1, pp. 1-9, mar. 2016, doi: 10.1038/sdata.2016.18.\[2\] Mark
Wilkinson, Erik Schultes, Luiz Olavo Bonino, Susanna-Assunta Sansone,
Peter Doorn, y Michel Dumontier, *FAIRMetrics/Metrics: FAIR Metrics,
Evaluation results, and initial release of automated evaluator code*.
Zenodo, 2018.\[3\] P. O. of the E. Union, «Turning FAIR into reality :
final report and action plan from the European Commission expert group
on FAIR data.», 26-nov-2018. \[En línea\]. Disponible en:
https://op.europa.eu:443/en/publication-detail/-/publication/7769a148-f1f6-11e8-9982-01aa75ed71a1/language-en/format-PDF.
\[Accedido: 23-nov-2019\].\[4\] «OpenAPI Specification \| Swagger». \[En
línea\]. Disponible en: https://swagger.io/specification/. \[Accedido:
14-ene-2020\].\[5\] «GitHub». \[En línea\]. Disponible en:
https://github.com/. \[Accedido: 14-ene-2020\].\[6\] M. D. Wilkinson,
S.-A. Sansone, E. Schultes, P. Doorn, L. O. Bonino da Silva Santos, y M.
Dumontier, «A design framework and exemplar metrics for FAIRness»,
*Scientific Data*, vol. 5, n.^o^ 1, pp. 1-4, jun. 2018, doi:
10.1038/sdata.2018.118.\[7\] «curl». \[En línea\]. Disponible en:
https://curl.haxx.se/. \[Accedido: 14-ene-2020\].\[8\] «SmartAPI». \[En
línea\]. Disponible en: https://smart-api.info/. \[Accedido:
14-ene-2020\].\[9\] «FAIRsharing». \[En línea\]. Disponible en:
https://fairsharing.org/. \[Accedido: 24-nov-2019\].\[10\] «Home -
schema.org». \[En línea\]. Disponible en: https://schema.org/.
\[Accedido: 24-nov-2019\].

**Apéndice 1. Métricas FAIR en HERCULES-ASIO**
==============================================

+----------------+----------------+----------------+----------------+
| **Principio    | **Métrica      | **Es           | **             |
| FAIR**         | /Explicación** | pecificación** | Verificación** |
+================+================+================+================+
| **Findable**   | **FM-F1A.      | An URL to a    | URL to a       |
|                | Identifier     | registered     | registered     |
| F1. (Meta)data | Uniqueness**   | identifier     | identifier     |
| are assigned a |                | scheme must be | scheme must be |
| globally       | The uniqueness | specified.     | present:       |
| unique and     | of an          |                |                |
| persistent     | identifier is  | An identifier  | -   A first    |
| identifier     | a necessary    | scheme is      |     > version  |
|                | condition to   | valid if and   |     > of this  |
|                | unambiguously  | only if it is  |     > metric   |
|                | refer that     | described in a |     > would    |
|                | resource, and  | repository     |     > focus on |
|                | that resource  | that can       |     > just     |
|                | alone.         | register and   |     > checking |
|                | Otherwise, an  | present such   |     > a URL    |
|                | identifier     | identifier     |     > that     |
|                | shared by      | schemes (e.g.  |     > resolves |
|                | multiple       | f              |     > to a     |
|                | resources will | airsharing.org |                |
|                | confound       | -- a curated,  |    > document. |
|                | efforts to     | informative    |                |
|                | describe that  | and            | -   A second   |
|                | resource, or   | educational    |     > version  |
|                | to use the     | resource on    |     > would    |
|                | ident to       | data and       |     > indicate |
|                | retrieve it.   | metadata       |     > how to   |
|                |                | standards,     |                |
|                |                | inter-related  |    > structure |
|                |                | to databases   |     > the data |
|                |                | and data       |     > policy   |
|                |                | policies       |     > document |
|                |                | \[9\]).        |     > with a   |
|                |                |                |     > section  |
|                |                |                |     > (similar |
|                |                |                |     > to how   |
|                |                |                |     > the CC   |
|                |                |                |     > licenses |
|                |                |                |     > now have |
|                |                |                |     > a formal |
|                |                |                |                |
|                |                |                |    > structure |
|                |                |                |     > in RDF). |
|                |                |                |                |
|                |                |                | -   A third    |
|                |                |                |     > version  |
|                |                |                |     > would    |
|                |                |                |     > insist   |
|                |                |                |     > that     |
|                |                |                |     > that     |
|                |                |                |     > document |
|                |                |                |     > and      |
|                |                |                |     > section  |
|                |                |                |     > is       |
|                |                |                |     > signed   |
|                |                |                |     > by an    |
|                |                |                |     > approved |
|                |                |                |                |
|                |                |                | > organization |
|                |                |                |     > and made |
|                |                |                |                |
|                |                |                |    > available |
|                |                |                |     > in an    |
|                |                |                |                |
|                |                |                |  > appropriate |
|                |                |                |                |
|                |                |                |  > repository. |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | Tal como se    |                |                |
| abordada en    | reflejó en     |                |                |
| HE             | análisis de    |                |                |
| RCULES-ASIO?** | entidades      |                |                |
|                | efectuado en   |                |                |
|                | el "ANEXO I de |                |                |
|                | la FASE I --   |                |                |
|                | Estudio de     |                |                |
|                | Viabilidad",   |                |                |
|                | para cada      |                |                |
|                | entidad se ha  |                |                |
|                | explorado el   |                |                |
|                | formato del    |                |                |
|                | identificador  |                |                |
|                | único a usar,  |                |                |
|                | e              |                |                |
|                | stableciéndose |                |                |
|                | mapeos entre   |                |                |
|                | el esquema de  |                |                |
|                | URIs uniforme  |                |                |
|                | y unívoco      |                |                |
|                | propuesto para |                |                |
|                | Hércules e     |                |                |
|                | i              |                |                |
|                | dentificadores |                |                |
|                | estándar que   |                |                |
|                | ya existen     |                |                |
|                | (ORCID,        |                |                |
|                | DOI). La       |                |                |
|                | factoría de    |                |                |
|                | URIs en        |                |                |
|                | HERCULES       |                |                |
|                | también jugará |                |                |
|                | un importante  |                |                |
|                | rol aquí,      |                |                |
|                | asegurando que |                |                |
|                | las IDs        |                |                |
|                | generados sean |                |                |
|                | únicos y       |                |                |
|                | también        |                |                |
|                | persistentes.  |                |                |
|                | Esta métrica   |                |                |
|                | garantizará    |                |                |
|                | que las URIs   |                |                |
|                | de un recurso  |                |                |
|                | y sus          |                |                |
|                | metadatos      |                |                |
|                | puedan ser     |                |                |
|                | resueltas      |                |                |
|                | correctamente. |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **Findable**   | **FM-F1B.      | Providers of   | Use an HTTP    |
|                | Identifier     | digital        | GET on URL     |
| F1. (Meta)data | persistence**  | resources must | provided.      |
| are assigned a |                | ensure that    | Present (a     |
| globally       | Whether there  | they have a    | 200,202,203 or |
| unique and     | is a policy    | policy to      | 206 HTTP       |
| persistent     | that describes | manage changes | response after |
| identifier     | what the       | in their       | resolving all  |
|                | provider will  | identifier     | and any prior  |
|                | do in the      | scheme, with a | redirects.     |
|                | event an       | special        | e.g. 301 -\>   |
|                | identifier     | emphasis on    | 302 -\> 200    |
|                | scheme becomes | maintaini      | OK) or Absent  |
|                | deprecated.    | ng/redirecting | (any other     |
|                |                | previously     | HTTP code). A  |
|                |                | generated      | first version  |
|                |                | identifiers.   | of this metric |
|                |                |                | would focus on |
|                |                | They must      | just checking  |
|                |                | provide a URL  | a URL that     |
|                |                | that resolves  | resolves to a  |
|                |                | to a document  | document.      |
|                |                | containing the |                |
|                |                | relevant       |                |
|                |                | policy.        |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | Muy            |                |                |
| abordada en    | relacionado    |                |                |
| ASIO?**        | con métrica    |                |                |
|                | FM-F1A. El ID  |                |                |
|                | asignado no    |                |                |
|                | sólo tiene que |                |                |
|                | ser único,     |                |                |
|                | sino que       |                |                |
|                | también apunte |                |                |
|                | de manera      |                |                |
|                | permanente a   |                |                |
|                | un recurso     |                |                |
|                | incluso cuando |                |                |
|                | cambie el      |                |                |
|                | identificador. |                |                |
|                | Este es un     |                |                |
|                | requisito      |                |                |
|                | importante que |                |                |
|                | la Factoría de |                |                |
|                | URIs y también |                |                |
|                | el backend del |                |                |
|                | SGI deben      |                |                |
|                | cumplir. La    |                |                |
|                | implementación |                |                |
|                | inicial        |                |                |
|                | probará que la |                |                |
|                | solución       |                |                |
|                | HERCULES-ASIO  |                |                |
|                | soporta el     |                |                |
|                | cambio de IDs  |                |                |
|                | de una entidad |                |                |
|                | y que todavía  |                |                |
|                | se mantienen   |                |                |
|                | mapeos usando  |                |                |
|                | esquemas de    |                |                |
|                | URIs           |                |                |
|                | anteriormente  |                |                |
|                | puestos en     |                |                |
|                | marcha.        |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **Findable**   | **FM-F2.       | A URL to a     | HTTP GET on    |
|                | Machi          | document that  | the metadata   |
| F2. Data are   | ne-readability | contains       | URL. A         |
| described with | of metadata**  | ma             | response of    |
| rich metadata  |                | chine-readable | \[a            |
|                | The            | metadata for   | 200,202,203 or |
|                | availability   | the digital    | 206 HTTP       |
|                | of             | resource must  | response after |
|                | ma             | be provided.   | resolving all  |
|                | chine-readable | Furthermore,   | and any prior  |
|                | metadata that  | the\           | redirects.     |
|                | describes a    | file format    | e.g. 301 -\>   |
|                | digital        | must be        | 302 -\> 200    |
|                | resource. This | specified.     | OK\] indicates |
|                | metric is      |                | that there is  |
|                | intended to    |                | indeed a       |
|                | test the       |                | document. The  |
|                | format of the  |                | second URL     |
|                | metadata since |                | should resolve |
|                | machine        |                | to the record  |
|                | readability of |                | of a           |
|                | metadata makes |                | registered fi\ |
|                | it possible to |                | le format      |
|                | optimize       |                | (e.g. DCAT,    |
|                | discovery. For |                | DICOM,         |
|                | instance, Web  |                | schema.org     |
|                | search engines |                | etc.) in a     |
|                | suggest the    |                | registry like  |
|                | use of         |                | FAIRsharing.   |
|                | structured     |                | Possible valid |
|                | metadata       |                | results will   |
|                | elements to    |                | be: a)         |
|                | optimize       |                | Ma             |
|                | search. Thus,  |                | chine-readable |
|                | the            |                | or b)          |
|                | machi          |                | Machin         |
|                | ne-readability |                | e-not-readable |
|                | aspect can     |                |                |
|                | help people    |                |                |
|                | and machines   |                |                |
|                | and a digital  |                |                |
|                | resource of    |                |                |
|                | interest.      |                |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | Para           |                |                |
| abordada en    | asegurarnos    |                |                |
| ASIO?**        | que ROH cumple |                |                |
|                | está métrica   |                |                |
|                | es             |                |                |
|                | imprescindible |                |                |
|                | que cada       |                |                |
|                | concepto       |                |                |
|                | representado   |                |                |
|                | ofrezca una    |                |                |
|                | URL apuntando  |                |                |
|                | a sus          |                |                |
|                | metadatos y    |                |                |
|                | que éstos      |                |                |
|                | estén en       |                |                |
|                | formatos       |                |                |
|                | estándar como  |                |                |
|                | DCAT -- Data   |                |                |
|                | Catalogue      |                |                |
|                | Vocabulary[^6] |                |                |
|                | o Schema.org   |                |                |
|                | \[10\]. ROH    |                |                |
|                | será un        |                |                |
|                | catálogo de    |                |                |
|                | datasets de    |                |                |
|                | investigación, |                |                |
|                | donde tanto el |                |                |
|                | grafo de       |                |                |
|                | conocimiento   |                |                |
|                | per sé         |                |                |
|                | (dcat:Catalog) |                |                |
|                | como cada      |                |                |
|                | dataset        |                |                |
|                | (dcat:Dataset) |                |                |
|                | en particular  |                |                |
|                | serán          |                |                |
|                | descritos a    |                |                |
|                | través del     |                |                |
|                | vocabulario    |                |                |
|                | DCAT. Cada     |                |                |
|                | concepto       |                |                |
|                | definido en    |                |                |
|                | ROH debe       |                |                |
|                | proporcionar   |                |                |
|                | una propiedad  |                |                |
|                | rdf:type que   |                |                |
|                | vincule a cada |                |                |
|                | entidad con    |                |                |
|                | una            |                |                |
|                | rdfs:Class.    |                |                |
|                | Además, la     |                |                |
|                | procedencia de |                |                |
|                | los elementos  |                |                |
|                | será declarada |                |                |
|                | explícitamente |                |                |
|                | a través de    |                |                |
|                | propiedades de |                |                |
|                | la ontología   |                |                |
|                | PROV-O.        |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **Findable**   | **FM-F3.       | The GUID[^7]   | Parsing the    |
|                | Resource       | of the         | metadata for   |
| F3. Metadata   | Identifier in  | metadata and   | the given      |
| clearly and    | Metadata**     | the GUID of    | digital        |
| explicitly     |                | the digital    | resource GUID. |
| include the    | The discovery  | resource it    | If the         |
| identifier of  | of a digital   | describes,     | GUID/UUID of   |
| the data they  | object should  | must be        | the referred   |
| describe       | be possible    | provided. For  | digital        |
|                | from its       | this to        | resource is    |
|                | metadata. This | happen, the    | present or     |
|                | metric checks  | metadata must  | absent.        |
|                | whether the    | explicitly     |                |
|                | metadata       | contain the    | In addition,   |
|                | document       | identifier for | since many     |
|                | contains the   | the digital    | digital        |
|                | globally       | resource it    | objects cannot |
|                | unique and     | describes, and | be arbitrarily |
|                | persistent     | this should be | extended to    |
|                | identifier for | present in the | include        |
|                | the digital    | form of a      | references to  |
|                | resource.      | qualified      | their          |
|                |                | reference,     | metadata, in   |
|                |                | indicating     | many cases,    |
|                |                | some manner of | the only means |
|                |                | \"about\"      | to discover    |
|                |                | relationship,  | the metadata   |
|                |                | to             | related to a   |
|                |                |                | digital object |
|                |                | distinguish    | will be to     |
|                |                | this           | search based   |
|                |                | identifier     | on the GUID of |
|                |                | from the       | the digital    |
|                |                | numerous       | object itself. |
|                |                | others that    |                |
|                |                | will be        |                |
|                |                | present in the |                |
|                |                | metadata.      |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | Íntimamente    |                |                |
| abordada en    | ligada a       |                |                |
| HE             | FM-F2,         |                |                |
| RCULES-ASIO?** | requiere que   |                |                |
|                | las conexiones |                |                |
|                | entre recursos |                |                |
|                | digitales y    |                |                |
|                | metadatos sean |                |                |
|                | bi             |                |                |
|                | direccionales. |                |                |
|                | En este caso,  |                |                |
|                | FM-F3 evalúa   |                |                |
|                | si desde los   |                |                |
|                | metadatos se   |                |                |
|                | puede llegar a |                |                |
|                | los datos del  |                |                |
|                | recurso        |                |                |
|                | digital        |                |                |
|                | modelado. Por  |                |                |
|                | lo tanto, en   |                |                |
|                | ROH los        |                |                |
|                | metadatos de   |                |                |
|                | una entidad    |                |                |
|                | tienen que     |                |                |
|                | apuntar de     |                |                |
|                | vuelta         |                |                |
|                | necesariamente |                |                |
|                | al recurso     |                |                |
|                | descrito       |                |                |
|                | incluyendo     |                |                |
|                | como valor la  |                |                |
|                | URI única y    |                |                |
|                | persistente    |                |                |
|                | del recurso    |                |                |
|                | digital.       |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **Findable**   | **FM-F4.       | The ability to | We perform an  |
|                | Indexed in a   | discover a     | HTTP GET on    |
| F4. (Meta)data | searchable     | resource       | the URLs       |
| are registered | resource**     | should be      | provided and   |
| or indexed in  |                | tested using   | attempt to     |
| a searchable   | The degree to  | a) its         | find the       |
| resource       | which the      | identifier, b) | persistent     |
|                | digital        | other          | identifier in  |
|                | resource can   | text-based     | the page that  |
|                | be found using | metadata.      | is returned. A |
|                | web-based      |                | second step    |
|                | search         | Taking as      | might include  |
|                | engines. Most  | input the      | following each |
|                | people use a   | persistent     | of the top N   |
|                | search engine  | identifier of  | hits and       |
|                | to initiate a  | the resource,  | examine the    |
|                | search for a   | perform search | resulting      |
|                | particular     | in the web and | documents for  |
|                | digital        | verify that    | presence of    |
|                | resource of    | pages pointing | the            |
|                | interest. If   | to the         | identifier.    |
|                | the resource   | resource are   | The result     |
|                | or its         | returned.      | will be true   |
|                | metadata are   |                | when the       |
|                | not indexed by |                | persistent     |
|                | web search     |                | identifier was |
|                | engines, then  |                | found in the   |
|                | this would     |                | search         |
|                | substantially  |                | results.       |
|                | diminish an    |                |                |
|                | individual\'s  |                |                |
|                | ability to\    |                |                |
|                | find and reuse |                |                |
|                | it.            |                |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | Dada la URI de |                |                |
| abordada en    | un recurso     |                |                |
| HE             | digital en     |                |                |
| RCULES-ASIO?** | ROH, deberemos |                |                |
|                | extraer las    |                |                |
|                | URIs que       |                |                |
|                | apuntan a sus  |                |                |
|                | metadatos y    |                |                |
|                | realizar       |                |                |
|                | búsquedas con  |                |                |
|                | buscadores de  |                |                |
|                | Internet para  |                |                |
|                | explorar sus   |                |                |
|                | primeros       |                |                |
|                | resultados y   |                |                |
|                | determinar si  |                |                |
|                | se devuelven   |                |                |
|                | enlaces al     |                |                |
|                | GUID del       |                |                |
|                | recurso        |                |                |
|                | buscado y sus  |                |                |
|                | metadatos.     |                |                |
|                | Esta misma     |                |                |
|                | funcionalidad  |                |                |
|                | de búsqueda    |                |                |
|                | debería ser    |                |                |
|                | disponible a   |                |                |
|                | través de las  |                |                |
|                | herramientas   |                |                |
|                | de búsqueda de |                |                |
|                | HERCULES-ASIO. |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **Accessible** | **FM-A1.1.     | This metric    | Do an HTTP get |
|                | Access         | should supply: | on the URL to  |
| **A1.          | Protocol**     |                | see if it      |
| (Meta)data are |                | -   A URL to   | returns a      |
| retrievable by | The nature and |     > the      | valid          |
| their          | use            |                | document.      |
| identifier     | limitations of |  > description | Ideally, we    |
| using a        | the access     |     > of the   | would have a   |
| standardised   | protocol.      |     > protocol | universal      |
| communications | Access to a    |                | database of    |
| protocol**     | resource may   | -   true/false | communication  |
|                | be limited by  |     > as to    | protocols from |
| *A1.1 - the    | the specified  |     > whether  | which we can   |
| protocol is    | communication  |     > the      | check this     |
| open, free,    | protocol. In   |     > protocol | URL. The HTTP  |
| and            | particular, we |     > is open  | GET on the URL |
| universally    | are worried    |     > source   | should return  |
| implementable* | about access   |                | a 200,202,203  |
|                | to technical   | -   true/false | or 206 HTTP    |
|                | specifications |     > as to    | response after |
|                | and any costs  |     > whether  | resolving all  |
|                | associated     |     > the      | and any prior  |
|                | with           |     > protocol | redirects.     |
|                | implementing   |     > is       | e.g. 301 - 302 |
|                | the protocol.  |                | - 200 OK. The  |
|                | Protocols that |    > (royalty) | other two      |
|                | are closed     |     > free     | should return  |
|                | source or that |                | true/false     |
|                | have royalties |                | (\"true\" is   |
|                | associated     |                | success)       |
|                | with them      |                |                |
|                | could prevent  |                |                |
|                | users from     |                |                |
|                | being able to  |                |                |
|                | obtain the     |                |                |
|                | resource.      |                |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | En ASIO, los   |                |                |
| abordada en    | i              |                |                |
| HE             | dentificadores |                |                |
| RCULES-ASIO?** | utilizados     |                |                |
|                | serán          |                |                |
|                | resolubles a   |                |                |
|                | través de      |                |                |
|                | HTTP/s. Es     |                |                |
|                | decir, apuntan |                |                |
|                | a recursos     |                |                |
|                | cuyos          |                |                |
|                | metadatos son  |                |                |
|                | devueltos a    |                |                |
|                | través de      |                |                |
|                | comandos HTTP  |                |                |
|                | (GET) en       |                |                |
|                | diferentes     |                |                |
|                | formatos       |                |                |
|                | atendiendo a   |                |                |
|                | la capacidad   |                |                |
|                | de negociación |                |                |
|                | de contenidos  |                |                |
|                | del paradigma  |                |                |
|                | REST, que se   |                |                |
|                | basa en la     |                |                |
|                | cabecera       |                |                |
|                | Content-Type,  |                |                |
|                | pudiendo ser   |                |                |
|                | servidor en    |                |                |
|                | formatos       |                |                |
|                | estándar como  |                |                |
|                | RDF/XML,       |                |                |
|                | Turtle, RDFa o |                |                |
|                | JSON-LD,       |                |                |
|                | dependiendo de |                |                |
|                | la cabecera    |                |                |
|                | Accept de la   |                |                |
|                | petición HTTP. |                |                |
|                | Uno de los     |                |                |
|                | componentes de |                |                |
|                | ASIO es un     |                |                |
|                | servidor       |                |                |
|                | Linked Data    |                |                |
|                | que            |                |                |
|                | contestaría    |                |                |
|                | con datos en   |                |                |
|                | respuesta a    |                |                |
|                | una URI, en el |                |                |
|                | formato        |                |                |
|                | indicado en la |                |                |
|                | petición.      |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **Accessible** | **FM-A1.2.     | The outcomes   | Computational  |
|                | Access         | of this metric | validation of  |
| **A1.          | a              | should be:     | the data       |
| (Meta)data are | uthorization** |                | provided. A    |
| retrievable by |                | -   true/false | valid answer   |
| their          | Specification  |     concerning | contains a     |
| identifier     | of a protocol  |     whether    | true or false  |
| using a        | to access      |                | for the first  |
| standardised   | restricted     |  authorization | question.      |
| communications | content. Not   |     is needed  |                |
| protocol**     | all content    |                | If true, an    |
|                | can be made    | -   a URL that | HTTP GET on    |
| *A1.2 - the    | available      |     resolves   | the URL        |
| protocol       | without        |     to a       | provided       |
| allows for an  | restriction.   |                | should return  |
| authentication | For instance,  |    description | a 200, 202,    |
| and            | access and     |     of the     | 203, or 206    |
| authorization  | distribution   |     process to | HTTP Response  |
| procedure,     | of personal    |     obtain     | after          |
| where          | health data    |     access to  | resolving all  |
| necessary*     | may be         |     restricted | redirects.     |
|                | restricted by  |     content.   |                |
|                | law or by      |                |                |
|                | organizational |                |                |
|                | policy. In     |                |                |
|                |                |                |                |
|                | such cases, it |                |                |
|                | is important   |                |                |
|                | that the       |                |                |
|                | protocol by    |                |                |
|                | which such     |                |                |
|                | content can be |                |                |
|                | accessed is    |                |                |
|                | fully          |                |                |
|                | specified.     |                |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | En             |                |                |
| abordada en    | HERCULES-ASIO, |                |                |
| HE             | se usarán      |                |                |
| RCULES-ASIO?** | HTTPS para     |                |                |
|                | incrementar la |                |                |
|                | seguridad en   |                |                |
|                | la             |                |                |
|                | comunicación,  |                |                |
|                | adoptándose    |                |                |
|                | OAuth para la  |                |                |
|                | autenticación  |                |                |
|                | y autorización |                |                |
|                | de acceso a    |                |                |
|                | recursos. Esta |                |                |
|                | métrica        |                |                |
|                | evaluará que   |                |                |
|                | sólo usuarios  |                |                |
|                | autorizados    |                |                |
|                | podrán acceder |                |                |
|                | a los recursos |                |                |
|                | digitales que  |                |                |
|                | requieren      |                |                |
|                | autorización.  |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **Accessible** | **FM-A2.       | This metric    | Resolve the    |
|                | Metadata       | must verify    | URL checking:  |
| **A2 -         | Longevity**    | the existence  |                |
| metadata are   |                | of an URL to a | -   Successful |
| accessible,    | The existence  | formal         |                |
| even when the  | of metadata    | metadata       |   > resolution |
| data are no    | even in the    | longevity      |                |
| longer         | a              | plan.          | -   Returns a  |
| available**    | bsence/removal |                |     > document |
|                | of Data.       |                |     > that     |
|                | Cr             |                |                |
|                | oss-references |                |   > represents |
|                | to data from   |                |     > a plan   |
|                | third-party\'s |                |     > or       |
|                | FAIR data and  |                |     > policy   |
|                | metadata will  |                |     > of some  |
|                | naturally      |                |     > kind     |
|                | degrade over   |                |                |
|                | time and       |                | -   Preferably |
|                | become "stale  |                |                |
|                | links\". In    |                |    > certified |
|                | such cases, it |                |     > (e.g.    |
|                | is important   |                |     > DSA)     |
|                | for FAIR       |                |                |
|                | providers to   |                |                |
|                | continue to    |                |                |
|                | provide        |                |                |
|                | descriptors of |                |                |
|                | what the data  |                |                |
|                | was to assist  |                |                |
|                | in the         |                |                |
|                | continued      |                |                |
|                | interpretation |                |                |
|                | of those       |                |                |
|                | third-party    |                |                |
|                | data. As per   |                |                |
|                | FAIR Principle |                |                |
|                | F3, this       |                |                |
|                | meta-data      |                |                |
|                | remains        |                |                |
|                | discoverable,  |                |                |
|                | even in the    |                |                |
|                | absence of the |                |                |
|                | data, because  |                |                |
|                | it contains an |                |                |
|                | explicit       |                |                |
|                | reference to   |                |                |
|                | the IRI of the |                |                |
|                | data.          |                |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | En el back-end |                |                |
| abordada en    | de             |                |                |
| HE             | HERCULES-ASIO  |                |                |
| RCULES-ASIO?** | verificaremos  |                |                |
|                | que, aunque    |                |                |
|                | algunos        |                |                |
|                | recursos       |                |                |
|                | digitales      |                |                |
|                | dejen de       |                |                |
|                | existir,       |                |                |
|                | todavía sus    |                |                |
|                | metadatos sean |                |                |
|                | recuperables.  |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **I            | **FM-I1. Use a | URL to the     | BNF (or        |
| nteroperable** | Knowledge      | specification  | other?) found, |
|                | Representation | of the         | Media-type of  |
| **I1 -         | Language**     | language       | the document   |
| (meta)data use |                |                | is             |
| a formal,      | Use of a       | -   The        |                |
| accessible,    | formal,        |     language   | registered in  |
| shared, and    | accessible,    |     must have  | FAIRSharing.   |
| broadly        | shared, and    |     a BNF (or  |                |
| applicable     | broadly        |     other      | Future:        |
| language for   | applicable     |                | FAIRSharing    |
| knowledge      | language for   |  specification | has tags to    |
| re             | knowledge      |     language)  | indicate       |
| presentation** | r              |                | constrained    |
|                | epresentation. | -   The URL    | vs. extendable |
|                |                |     resolves   | languages?     |
|                | The            |                |                |
|                | unambiguous    |   (accessible) |                |
|                | communication  |                |                |
|                | of knowledge   | -   The        |                |
|                | and meaning    |     document   |                |
|                | (what symbols  |     has an     |                |
|                | are, and how   |     IANA       |                |
|                | they relate to |                |                |
|                | one another)   | media-type[^8] |                |
|                | necessitates   |     (i.e. it   |                |
|                | the use of     |     is         |                |
|                | languages that |                |                |
|                | are capable of |   sufficiently |                |
|                | representing   |     widely     |                |
|                | these concepts |     accepted   |                |
|                | in a           |     and shared |                |
|                | ma             |     that it    |                |
|                | chine-readable |     has been   |                |
|                | manner.        |                |                |
|                |                |    registered) |                |
|                |                |                |                |
|                |                | -   The        |                |
|                |                |     language   |                |
|                |                |     can be     |                |
|                |                |                |                |
|                |                |    arbitrarily |                |
|                |                |     extended   |                |
|                |                |     (e.g.      |                |
|                |                |     PDBml can  |                |
|                |                |     be used to |                |
|                |                |     represent  |                |
|                |                |     knowledge, |                |
|                |                |     but only   |                |
|                |                |     about      |                |
|                |                |     proteins)  |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | En             |                |                |
| abordada en    | HERCULES-ASIO, |                |                |
| HE             | los metadatos  |                |                |
| RCULES-ASIO?** | se expresarán  |                |                |
|                | según el       |                |                |
|                | lenguaje       |                |                |
|                | estándar de la |                |                |
|                | web semántica  |                |                |
|                | Ontology Web   |                |                |
|                | Language, OWL. |                |                |
|                | Se hará un uso |                |                |
|                | extensivo de   |                |                |
|                | Ontologías     |                |                |
|                | ampliamente    |                |                |
|                | utilizadas por |                |                |
|                | terceros (como |                |                |
|                | por ejemplo    |                |                |
|                | FOAF, Dublin   |                |                |
|                | Core o         |                |                |
|                | schema.org) lo |                |                |
|                | que            |                |                |
|                | garantizará la |                |                |
|                | int            |                |                |
|                | eroperabilidad |                |                |
|                | entre nuestro  |                |                |
|                | grafo de       |                |                |
|                | conocimiento   |                |                |
|                | ROH y otros    |                |                |
|                | externos.      |                |                |
|                | Además, los    |                |                |
|                | metadatos      |                |                |
|                | serán          |                |                |
|                | exportados en  |                |                |
|                | diferentes     |                |                |
|                | s              |                |                |
|                | erializaciones |                |                |
|                | de RDF como    |                |                |
|                | RDF/XML,       |                |                |
|                | JSON-LD o      |                |                |
|                | Turtle.        |                |                |
|                | Revisaremos    |                |                |
|                | las ontologías |                |                |
|                | reutilizadas a |                |                |
|                | lo largo del   |                |                |
|                | tiempo y       |                |                |
|                | contrastaremos |                |                |
|                | que ROH en una |                |                |
|                | gran mayoría   |                |                |
|                | (\>80%) está   |                |                |
|                | basado en      |                |                |
|                | ontologías     |                |                |
|                | conocidas y    |                |                |
|                | ampliamente    |                |                |
|                | probadas.      |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **I            | **FM-I2. Use   | IRIs           | Resolve IRIs,  |
| nteroperable** | FAIR           | representing   | check FAIRness |
|                | Vocabularies** | the            | of the         |
| **I2 -         |                | vocabularies   | returned       |
| (meta)data use | The metadata   | used for       | document(s).   |
| vocabularies   | values and     | (meta)data     |                |
| that follow    | qualified      | must be        | Successful     |
| FAIR           | relations      | provided.      | resolution;    |
| principles**   | should         |                | document is    |
|                | themselves be  |                | amenable to    |
|                | FAIR, for      |                | m              |
|                | example, terms |                | achine-parsing |
|                | from open,     |                | and            |
|                | comm           |                | identification |
|                | unity-accepted |                | of terms       |
|                | vocabularies   |                | within it. It  |
|                | published in   |                | may be         |
|                | an appropriate |                | possible to    |
|                | know           |                | use            |
|                | ledge-exchange |                | FAIRSharing to |
|                | format.        |                | validate these |
|                |                |                | vocabularies.  |
|                | Data, and the  |                |                |
|                | provenance     |                |                |
|                | descriptors of |                |                |
|                | the data,      |                |                |
|                | should (where  |                |                |
|                | reasonable)    |                |                |
|                | use            |                |                |
|                | vocabularies   |                |                |
|                | and            |                |                |
|                | terminologies  |                |                |
|                | that are,      |                |                |
|                | themselves,    |                |                |
|                | FAIR.          |                |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | En             |                |                |
| abordada en    | HERCULES-ASIO, |                |                |
| HE             | se va a crear  |                |                |
| RCULES-ASIO?** | una red de     |                |                |
|                | ontologías     |                |                |
|                | (ROH) basada   |                |                |
|                | en ontologías  |                |                |
|                | ampliamente    |                |                |
|                | aceptadas.     |                |                |
|                | Tales          |                |                |
|                | ontologías van |                |                |
|                | a ser          |                |                |
|                | enlazadas      |                |                |
|                | entre ellas    |                |                |
|                | para asegurar  |                |                |
|                | su             |                |                |
|                | inte           |                |                |
|                | roperabilidad. |                |                |
|                | Se respetarán  |                |                |
|                | las licencias  |                |                |
|                | de uso y       |                |                |
|                | reutilización  |                |                |
|                | de cada una de |                |                |
|                | ellas,         |                |                |
|                | reconociendo   |                |                |
|                | la autoría     |                |                |
|                | inicial de las |                |                |
|                | mismas a       |                |                |
|                | través de      |                |                |
|                | relaciones     |                |                |
|                | basadas en la  |                |                |
|                | ontología      |                |                |
|                | PROV-O. Esta   |                |                |
|                | métrica deberá |                |                |
|                | verificar que  |                |                |
|                | los            |                |                |
|                | vocabularios   |                |                |
|                | adoptados para |                |                |
|                | modelar        |                |                |
|                | conocimiento   |                |                |
|                | son FAIR por   |                |                |
|                | sí mismos.     |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **I            | **FM-I3. Use   | Linksets (in   | The linksets   |
| nteroperable** | Qualified      | the formal     | must have      |
|                | References**   | sense)         | qualified      |
| **I3 -         |                | representing   | references. At |
| (meta)data     | Relationships  | part or all of | least one of   |
| include        | within         | your resource  | the links must |
| qualified      | (meta)data,    | must be        | be in a        |
| references to  | and between    | provided.      | different Web  |
| other**        | local and      |                | domain         |
|                | third-party    |                |                |
| **(meta)data** | data, have     |                | (or the        |
|                | explicit and   |                | equivalent of  |
|                | \'useful\'     |                | a different    |
|                | semantic       |                | namespace for  |
|                | meaning.       |                | non-URI        |
|                |                |                | identifiers)   |
|                | For            |                |                |
|                | Int            |                | -   References |
|                | eroperability, |                |     > are      |
|                | the            |                |                |
|                | relationships  |                |    > qualified |
|                | within and     |                |                |
|                | between data   |                | -   Qualities  |
|                | must be more   |                |     > are      |
|                | semantically   |                |     > beyond   |
|                | rich than "is  |                |     > "ref\"   |
|                | (somehow)      |                |     > or "is   |
|                | related to\".  |                |     > related  |
|                |                |                |     > to"      |
|                | Numerous       |                |                |
|                | ontologies     |                | -   One of the |
|                | include richer |                |     > cr       |
|                | relationships  |                | oss-references |
|                | that can be    |                |     > points   |
|                | used for this  |                |     > outwards |
|                | purpose, at    |                |     > to a     |
|                | various levels |                |     > distinct |
|                | of             |                |                |
|                | domai          |                |    > Namespace |
|                | n-specificity. |                |                |
|                | For example,   |                |                |
|                | the use of     |                |                |
|                | SKOS for       |                |                |
|                | terminologies  |                |                |
|                | (e.g. exact    |                |                |
|                | matches)       |                |                |
|                | refere         |                |                |
|                | nces/relations |                |                |
|                | point outwards |                |                |
|                | to other       |                |                |
|                | resources,     |                |                |
|                | owned by       |                |                |
|                | third-parties; |                |                |
|                | this is one of |                |                |
|                | the            |                |                |
|                | requirements   |                |                |
|                | for 5 star     |                |                |
|                | linked data.   |                |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | En             |                |                |
| abordada en    | HERCULES-ASIO, |                |                |
| HE             | siguiendo el   |                |                |
| RCULES-ASIO?** | enfoque de     |                |                |
|                | Linked Data    |                |                |
|                | nuestras       |                |                |
|                | ontologías y   |                |                |
|                | las instancias |                |                |
|                | de las mismas  |                |                |
|                | estarán        |                |                |
|                | enlazadas a    |                |                |
|                | otras          |                |                |
|                | ontologías y   |                |                |
|                | recursos de    |                |                |
|                | grafos         |                |                |
|                | externos       |                |                |
|                | (Wikidata,     |                |                |
|                | DBpedia),      |                |                |
|                | re             |                |                |
|                | spectivamente. |                |                |
|                | Esta métrica   |                |                |
|                | evaluará que   |                |                |
|                | la mayoría de  |                |                |
|                | las relaciones |                |                |
|                | sean           |                |                |
|                | específicas al |                |                |
|                | dominio y no   |                |                |
|                | genéricas (ej. |                |                |
|                | related\_to    |                |                |
|                | frente a       |                |                |
|                | child\_of),    |                |                |
|                | cuando enlacen |                |                |
|                | las diferentes |                |                |
|                | entidades      |                |                |
|                | modeladas en   |                |                |
|                | el grafo       |                |                |
|                | resultante.    |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **Reusable**   | **FM-R1.1.     | The IRI of the | Resolve the    |
|                | Accessible     | license (e.g.  | IRI(s) using   |
| **R1.          | Usage          | its URL) for   | its associated |
| Meta(data) are | License**      | the data       | resolution     |
| richly         |                | license and    | protocol.      |
| described with | The existence  | for the        |                |
| a plurality of | of a license   | metadata       | Valid result:  |
| accurate and   | document, for  | license must   | A document     |
| relevant       | BOTH           | be provided.   | containing the |
| attributes**   | (              |                | license        |
|                | independently) |                | information    |
| *R1.1 -        | the data and   |                |                |
| (meta)data are | its associated |                |                |
| released with  | metadata, and  |                |                |
| a clear and    | the ability to |                |                |
| accessible*    | retrieve those |                |                |
|                | documents. A   |                |                |
| *data usage    | core aspect of |                |                |
| license*       | data           |                |                |
|                | reusability is |                |                |
|                | the ability to |                |                |
|                | determine,     |                |                |
|                | unambiguously  |                |                |
|                | and with       |                |                |
|                | relative ease, |                |                |
|                | the conditions |                |                |
|                | under which    |                |                |
|                | you are        |                |                |
|                | allowed to     |                |                |
|                | reuse the      |                |                |
|                | (meta)data.    |                |                |
|                | Thus, FAIR     |                |                |
|                | data providers |                |                |
|                | must make      |                |                |
|                | these terms    |                |                |
|                | openly         |                |                |
|                | available.     |                |                |
|                | This applies   |                |                |
|                | both to data   |                |                |
|                | (e.g. for the  |                |                |
|                | purpose of     |                |                |
|                | third-party    |                |                |
|                | integration    |                |                |
|                | with other     |                |                |
|                | data) and for  |                |                |
|                | metadata (e.g. |                |                |
|                | for the        |                |                |
|                | purpose of     |                |                |
|                | third-party    |                |                |
|                | indexing or    |                |                |
|                | other          |                |                |
|                | administrative |                |                |
|                | metrics)       |                |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | En ASIO, la    |                |                |
| abordada en    | red de         |                |                |
| HE             | ontologías     |                |                |
| RCULES-ASIO?** | Hércules (ROH) |                |                |
|                | resultante     |                |                |
|                | tendrá una     |                |                |
|                | licencia       |                |                |
|                | Creative       |                |                |
|                | Commons 4.0    |                |                |
|                | BY-SA. Se      |                |                |
|                | verificará que |                |                |
|                | las ontologías |                |                |
|                | conectadas en  |                |                |
|                | esta red       |                |                |
|                | dispongan      |                |                |
|                | todas ellas de |                |                |
|                | una licencia   |                |                |
|                | compatible.    |                |                |
|                | Además, las    |                |                |
|                | ontologías     |                |                |
|                | importadas     |                |                |
|                | serán          |                |                |
|                | enriquecidas   |                |                |
|                | con            |                |                |
|                | descripciones  |                |                |
|                | semánticas, a  |                |                |
|                | través de las  |                |                |
|                | propiedades    |                |                |
|                | dc:rights y    |                |                |
|                | dc:license,    |                |                |
|                | del            |                |                |
|                | vocabulario    |                |                |
|                | Dublin Cored.  |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **Reusable**   | **FM-R1.2.     | Several IRIs - | We resolve the |
|                | Detailed       | at least one   | IRI according  |
| **R1.          | Provenance**   | of these       | to their       |
| Meta(data) are |                | points to one  | associated     |
| richly         | That there is  | of the         | protocols.     |
| described with | provenance     | vocabularies   |                |
| a plurality of | information    | used to        | In the future, |
| accurate and   | associated     | describe       | we may be able |
| relevant       | with the data, | citational     | to             |
| attributes**   | covering at    | provenance     | c              |
|                | least two      | (e.g.          | ross-reference |
| *R1.2 -        | primary types  |                | these with     |
| (meta)data are | of provenance  | dublin core).  | FAIRSharing to |
| associated     | information:   | At least one   | confirm that   |
| with detailed  |                | points to one  | they are       |
| provenance*    | -              | of the         | \"standard\",  |
|                |  Who/what/When | vocabularies   | and perhaps    |
|                |     produced   | (likely        | even           |
|                |     the data   | do             | distinguish    |
|                |     (i.e. for  | main-specific) | citation vs.   |
|                |     citation)  | that is used   | domain         |
|                |                | to describe    | specific       |
|                | -   Why/How    | contextual     |                |
|                |     was the    | provenance     | IRI 1 should   |
|                |     data       | (e.g. EDAM)    | resolve to a   |
|                |     produced   |                | recognized     |
|                |     (i.e. to   |                | citation       |
|                |     understand |                | provenance     |
|                |     context    |                | standard such  |
|                |     and        |                | as Dublin      |
|                |     relevance  |                | Core.          |
|                |     of the     |                |                |
|                |     data).     |                | IRI 2 should   |
|                |                |                | resolve to     |
|                |    Reusability |                | some           |
|                |     is not     |                | vocabulary     |
|                |     only a     |                | that itself    |
|                |     technical  |                | passes basic   |
|                |     issue;     |                | tests of       |
|                |     data can   |                | FAIRness       |
|                |     be         |                |                |
|                |                |                |                |
|                |    discovered, |                |                |
|                |     retrieved, |                |                |
|                |     and even   |                |                |
|                |     be         |                |                |
|                |     mac        |                |                |
|                | hine-readable, |                |                |
|                |     but still  |                |                |
|                |     not be     |                |                |
|                |     reusable   |                |                |
|                |     in any     |                |                |
|                |     rational   |                |                |
|                |     way.       |                |                |
|                |                |                |                |
|                |    Reusability |                |                |
|                |     goes       |                |                |
|                |     beyond     |                |                |
|                |     "can I     |                |                |
|                |     reuse this |                |                |
|                |     data?\" to |                |                |
|                |     other      |                |                |
|                |     important  |                |                |
|                |     questions  |                |                |
|                |     such as    |                |                |
|                |     "may I     |                |                |
|                |     reuse this |                |                |
|                |     data?\",   |                |                |
|                |     "should I  |                |                |
|                |     reuse this |                |                |
|                |     data\",    |                |                |
|                |     and "who   |                |                |
|                |     should I   |                |                |
|                |     credit if  |                |                |
|                |     I decide   |                |                |
|                |     to use     |                |                |
|                |     it?\"      |                |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | En             |                |                |
| abordada en    | HERCULES-ASIO, |                |                |
| HE             | se hará uso de |                |                |
| RCULES-ASIO?** | la ontología   |                |                |
|                | PROV-O         |                |                |
|                | (h             |                |                |
|                | ttps://www.w3. |                |                |
|                | org/TR/prov-o) |                |                |
|                | para declarar  |                |                |
|                | explícitamente |                |                |
|                | la procedencia |                |                |
|                | de las         |                |                |
|                | ontologías y   |                |                |
|                | sus atributos  |                |                |
|                | y datos de     |                |                |
|                | instancia      |                |                |
|                | reutilizados.  |                |                |
|                | Esta métrica   |                |                |
|                | comprobará que |                |                |
|                | se ha          |                |                |
|                | declarado      |                |                |
|                | provenance de  |                |                |
|                | cada           |                |                |
|                | vocabulario    |                |                |
|                | incorporado en |                |                |
|                | ROH.           |                |                |
|                |                |                |                |
|                | Se reutilizará |                |                |
|                | y extenderán   |                |                |
|                | \[2\] scripts  |                |                |
|                | disponibles en |                |                |
|                | Ruby en        |                |                |
|                | <https:        |                |                |
|                | //github.com/F |                |                |
|                | AIRMetrics/Met |                |                |
|                | rics/tree/mast |                |                |
|                | er/MetricsEval |                |                |
|                | uatorCode/Ruby |                |                |
|                | /metrictests>. |                |                |
+----------------+----------------+----------------+----------------+
| **Reusable**   | **FM-R1.3.     | A              | Validate the   |
|                | Meets          | certification  | electronic     |
| **R1.          | Community      | saying that    | signature as   |
| Meta(data) are | Standards**    | the resource   | coming from a  |
| richly         |                | is compliant   | community      |
| described with | Certification, | should be      | authority      |
| a plurality of | from a         | provided.      | (e.g. a        |
| accurate and   | recognized     |                | Verisign       |
| relevant       | body, of the   |                | signature).    |
| attributes**   | resource       |                | Successful     |
|                | meeting        |                | signature      |
| *R1.3 -        | community      |                | validation     |
| (meta)data     | standards.     |                |                |
| meet           | Various        |                | Such           |
| d              | communities    |                | certification  |
| omain-relevant | have           |                | services may   |
| community      | recognized     |                | not exist, but |
| standards*     | that           |                | this principle |
|                | maximizing the |                | serves to      |
|                | usability of   |                | encourage the  |
|                | their data     |                | community to   |
|                | requires them  |                | create both    |
|                | to adopt a set |                | the            |
|                | of guidelines  |                | standard(s)    |
|                | for metadata   |                | and the        |
|                | (often in the  |                | verification   |
|                | form of        |                | services for   |
|                | "minimal       |                | those          |
|                | information    |                | standards.     |
|                | about. . . \"  |                |                |
|                | models).       |                |                |
|                | Non-compliance |                |                |
|                | with these     |                |                |
|                | standards will |                |                |
|                | often render a |                |                |
|                | dataset        |                |                |
|                | \`reuseless\'  |                |                |
|                | because        |                |                |
|                | critical       |                |                |
|                | information    |                |                |
|                | about its      |                |                |
|                | context or     |                |                |
|                | provenance is  |                |                |
|                | missing.       |                |                |
|                |                |                |                |
|                | However,       |                |                |
|                | adherence to   |                |                |
|                | community      |                |                |
|                | standards does |                |                |
|                | more than just |                |                |
|                | improve        |                |                |
|                | reusability of |                |                |
|                | the data. The  |                |                |
|                | software used  |                |                |
|                | by the         |                |                |
|                | community for  |                |                |
|                | analysis and   |                |                |
|                | visualization  |                |                |
|                | often depends  |                |                |
|                | on the         |                |                |
|                | (meta)data     |                |                |
|                | having certain |                |                |
|                | fields; thus,  |                |                |
|                | non-compliance |                |                |
|                | with standards |                |                |
|                | may result in  |                |                |
|                | the data being |                |                |
|                | unreadable by  |                |                |
|                | its associated |                |                |
|                | tools. As      |                |                |
|                | such, data     |                |                |
|                | should be      |                |                |
|                | (individually) |                |                |
|                | certified as   |                |                |
|                | being          |                |                |
|                | compliant,     |                |                |
|                | likely through |                |                |
|                | some automated |                |                |
|                | process (e.g.  |                |                |
|                | submitting the |                |                |
|                | data to the    |                |                |
|                | community\'s   |                |                |
|                | online         |                |                |
|                | validation     |                |                |
|                | service)       |                |                |
+----------------+----------------+----------------+----------------+
| **¿Cómo es     | En             |                |                |
| abordada en    | HERCULES-ASIO, |                |                |
| HE             | esta métrica   |                |                |
| RCULES-ASIO?** | es de difícil  |                |                |
|                | verificación   |                |                |
|                | automática,    |                |                |
|                | será evaluada  |                |                |
|                | por la         |                |                |
|                | revisión       |                |                |
|                | manual de la   |                |                |
|                | ontología      |                |                |
|                | resultante por |                |                |
|                | expertos y la  |                |                |
|                | evaluación de  |                |                |
|                | la usabilidad  |                |                |
|                | de la misma    |                |                |
|                | por los        |                |                |
|                | usuarios de    |                |                |
|                | ROH. Se        |                |                |
|                | prepararán     |                |                |
|                | cuestionarios  |                |                |
|                | que evalúen    |                |                |
|                | que ROH cumple |                |                |
|                | las            |                |                |
|                | convenciones   |                |                |
|                | del dominio de |                |                |
|                | los sistemas   |                |                |
|                | de gestión de  |                |                |
|                | investigación, |                |                |
|                | sigue          |                |                |
|                | estándares y   |                |                |
|                | además         |                |                |
|                | corresponde    |                |                |
|                | con una        |                |                |
|                | ontología      |                |                |
|                | fácilmente     |                |                |
|                | interoperable  |                |                |
|                | y usable.      |                |                |
+----------------+----------------+----------------+----------------+

[^1]: <https://github.com/FAIRMetrics/Metrics>

[^2]: <https://github.com/FAIRMetrics/Metrics/blob/master/MaturityIndicators/MaturityIndicatorTemplate.md>

[^3]: <https://www.w3.org/TR/vocab-dcat-2/>

[^4]: <http://guid.one/guid>

[^5]: <https://www.iana.org/assignments/media-types/media-types.xhtml>

[^6]: <https://www.w3.org/TR/vocab-dcat-2/>

[^7]: <http://guid.one/guid>

[^8]: <https://www.iana.org/assignments/media-types/media-types.xhtml>
