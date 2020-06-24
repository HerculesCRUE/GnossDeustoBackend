![](./media/CabeceraDocumentosMD.png)

# Plantilla de documento de adhesión a las recomendaciones de buenas prácticas de URIs.md

La *Universidad X* ha implantado el backend del proyecto Hércules Arquitectura
Semántica e Infraestructura Ontológica (ASIO en adelante) para hacer accesible, 
mediante los estándares de la web semántica, la información de su Sistema de 
Gestión de la Investigación (SGI), lo que incluye datos de sus investigadores,
sus proyectos y sus resultados de investigación. 

La iniciativa Hércules es parte de la Comisión Sectorial de Tecnologías de la
Información y las Comunicaciones de la Conferencia de Rectores de las Universidades
Españolas (CRUE-TIC). Su objetivo es crear un Sistema de Gestión de Investigación (SGI)
basado en datos abiertos semánticos que ofrezca una visión global de los datos
de investigación del Sistema Universitario Español (SUE), para mejorar la gestión,
el análisis y las posibles sinergias entre universidades y el gran público.

El proyecto Hércules ASIO es uno de los componentes de la iniciativa Hércules,
que es parte de la Comisión Sectorial de Tecnologías de la Información y las
Comunicaciones de la Conferencia de Rectores de las Universidades Españolas (CRUE-TIC). 

ASIO está compuesto de:

-   La Infraestructura Ontológica de la información del SUE comprende una red de
    ontologías que pueda ser usada para describir con fidelidad y alta granularidad
    los datos del dominio de la Gestión de la Investigación.
    
-   La Arquitectura Semántica de Datos del SUE consiste en una plataforma eficiente
    para almacenar, gestionar y publicar los datos del SGI, basándose en la
    Infraestructura Ontológica, con la capacidad de sincronizar instancias
    instaladas en diferentes Universidades.
    
Para el cumplimiento de los objetivos propuestos, es importante que los **URI 
(Uniform Resource Identifier)**, usados para referenciar la información del SGI
mediante Hércules ASIO, **compartan un esquema común, apliquen unas reglas
de normalización similares y, especialmente, persistan según lo identificado**,
es decir, que cumplan el principio de persistencia para que la información
sea accesible en el futuro y se mantenga la integridad de la información.

Por tanto, la *Universidad X* se compromete, en la medida de lo posible,
a cumplir el documento de Buenas prácticas de URIs de Hércules (anexado a 
continuación), lo que implica que los recursos que aloja en su instancia
de Hércules ASIO cumplen con las siguientes condiciones:

-   Los URIs que identifican a los recursos no variarán sin justificación 
    y proporcionarán acceso a la información que referencian, mediante 
    mecanismos HTTP estándares.
    
-   En caso de variación, el URI proporcionará mecanismos estándar, mediante
    códigos de estado HTTP, para redirigir al nuevo URI o para informar de
    que el recurso ha sido eliminado.
    
-   Los URIs se construirán siguiendo la [Especificación del Esquema de URIs
    de Hércules](https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/UrisFactory/docs/Especificaci%C3%B3n%20Esquema%20de%20URIs.md) y las recomendaciones de normalización de las 
    Buenas prácticas de URIs de Hércules.
    
-   Los recursos persistentes continuarán estando disponibles durante la
    existencia de la *Universidad X*.
    
-   Si el backend dejase de existir en la *Universidad X* otras entidades
    del ámbito de la universidad española podrían publicar la información
    con nuevos URIs con un dominio distinto, y aplicando la misma política
    de persistencia de datos y cumplimiento de las las Buenas prácticas de
    URIs de Hércules. La *Universidad X* proporcionará durante *N* meses
    mecanismos de redirección hacia el nuevo dominio, mediante códigos de
    estado HTTP 3xx.


