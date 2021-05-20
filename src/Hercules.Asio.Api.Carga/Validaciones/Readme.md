![](../../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 20/05/20201                                                  |
| ------------- | ------------------------------------------------------------ |
|Titulo|API CARGA Gestión de Validaciones| 
|Descripción|Manual de la gestión de validaciones del API de Carga|
|Versión|1.2|
|Módulo|API CARGA|
|Tipo|Manual|
|Cambios de la Versión|Actualización de las validaciones|


# Gestión de validaciones

Las validaciones se efectuan mediante [Shapes SHACL](https://www.w3.org/TR/shacl/).	

1.- Las validaciones van ligadas al repositorio de sincronización, es decir, se podrán efectuar diferentes validaciones en función del repositorio ya que puede que algún repositorio sea más completo que otro.

2.- Desde el 'API_CARGA' http://herc-as-front-desa.atica.um.es/carga/swagger/index.html se pueden dar altas, bajas, y modificaciones de los Shapes

	GET	/etl-config/validation 				Obtiene la configuración de los shape SHACL de validación	
	POST 	/etl-config/validation				Añade una configuración de validación mediante un shape SHACL
	PUT	/etl-config/validation				Modifica la configuración de validación mediante un shape SHACL
	GET	/etl-config/validation/{identifier}		Obtiene la configuración del shape SHACL pasado por parámetro
	DELETE	/etl-config/validation/{identifier}		Elimina la configuración una configuración de validación

3.- Las configuraciones de validaciones cuentan con los siguientes campos:

	1.- ID del shape (se genera automáticamente al dar de alta u Shape)
	2.- Nombre del Shape
	3.- ID del repositorio
	4.- Fichero con el Shape
	
4.- Desde el 'API_CARGA' http://herc-as-front-desa.atica.um.es/carga/swagger/index.html se efectuan las validaciones

	POST	/etl/data-validate	Valida un RDF mediante el shape SHACL configurado

5.- Las validaciones cuentan con los siguientes campos

	1.- ID del repositorio
	2.- Fichero en formato RDF, TTL o NTriples para validar
	
6.- El resultado de la validación devuelve un objeto JSON con el siguiente formato:

	{
		"severity": "string", //Nivel de severidad de la validación mas restrictiva que se ha incumplido (INFO, WARNING o VIOLATION)
		"conforms": true, //Indica si se han cumplido todas las validaciones
		"results": [	//Resultado de todas las validaciones que se han incumplido
			{
				"severity": "string",		//Nivel de severidad de la validación (INFO, WARNING o VIOLATION)
				"focusNode": "string",		//Nodo en el que se ha incumplido la validación
				"resultValue": "string",	//Valor del campo que ha incumplido la validación
				"message": "string",		//Mensaje explicativo de la validación que se ha incumplido
				"resultPath": "string",		//Path de la validación que se ha incumplido
				"shapeName": "string",		//Nombre de la validación que se ha incumplido
				"shapeID": "string"			//Identificador de la validación que se ha incumplido
			}
		]
	}
	
7.- En esta carpeta se encuentran todas las validaciones extraídas de la ontología a fecha 20/5/2021.

	domain.ttl: Contiene todas las validaciones de restricciones de dominio extraídas del OWL de la ontología ROH.
	rangeDataType.ttl: Contiene todas las validaciones de restricciones de rango de dataTypes extraídas del OWL de la ontología ROH.
	rangeObject.ttl: Contiene todas las validaciones de restricciones de rango de objetos extraídas del OWL de la ontología ROH.
	Class_validations: Carpeta con todas las validaciones propias de cada una de las clases del OWL de la ontología ROH.
	backup: Carpeta con un backup de las validaciones extraídas de la ontología a fecha 15/5/2020.
	Radboud: Carpeta con validaciones para los datos de la Universidad de Radboud.
	
8.- Las validaciones cargadas en las configuraciones se corresponden con las validaciones del repositorio OAI_PMH_CVN, cuyo identificador es 5efac0ad-ec4e-467d-bbf5-ce3f64edb46a.
Se pueden consultar con esta petición:

http://herc-as-front-desa.atica.um.es/carga/etl-config/Validation 

O mediante el interfaz swagger en: 

http://herc-as-front-desa.atica.um.es/carga/swagger/index.html

	
