![](../Docs/media/CabeceraDocumentosMD.png)

# Sobre api gestor de documentación

Este módulo se usa para cargar páginas HTML y servirlas a través de la web a modo de un gestor de contenidos, en el que los usuarios pueden subir páginas html con una ruta
determinada y luego acceder a esas páginas a través de la web (https://herc-as-front-desa.atica.um.es/carga-web), estas operaciones están disponibles en el controlador Page
  - Controlador Page: contiene 4 métodos para realizar las operaciones con las páginas
	 - *Get:* /Page: Que devuelve una página dada su ruta.
	 - *Get:* /Page/list: Que devuelve la lista de páginas sin el html.
	 - *Post:* /Page/loadpage: Carga una nueva página o modifica un página existente
	 - *Delete:* /Page/Delete: Elimina una página dando su identificador

Hay una versión disponible en el entorno de pruebas de la Universidad de Murcia, en esta dirección: 

http://herc-as-front-desa.atica.um.es/documentacion.
