![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 27/05/2021                                                   |
| ------------- | ------------------------------------------------------------ |
|Titulo|Descripción del servicio SAML| 
|Descripción|Manual del servicio de background de SAML|
|Versión|1|
|Módulo|SAML|
|Tipo|Documentación|
|Cambios de la Versión|Creación del documento|


# Acerca de SAML

![](https://github.com/HerculesCRUE/GnossDeustoBackend/workflows/Build%20Hercules.Asio.SPARQLReplication/badge.svg)

SAML es un tipo de servicio SSO (Single Sign On) capaz de intercambiar información del usuario mediante un identity provider y un service provider. 
La función principal es de dar seguridad y permisos a los distintos usuarios que naveguen por la web sin comprometer sus datos.
Cuando un usuario intenta acceder a una página privada, se le redigirá al SSO de Hércules ASIO al cual tendrá que introducir sus credenciales para acceder.
Dependiendo del tipo de usuario que sea, le dará unos privilegios u otros. Tras loguearse, volverá a la página a la cual ha intentado acceder.

## Configuración en el appsettings.json
    {
	  "ConnectionStrings": {
		"PostgreConnection": "",
		"PostgreConnectionmigration": ""
	  },
	  "Logging": {
		"LogLevel": {
		  "Default": "Information",
		  "Microsoft": "Warning",
		  "Microsoft.Hosting.Lifetime": "Information"
		}
	  },
	  "Saml2": {
		"IdPMetadata": "",
		"Issuer": "",
		"SignatureAlgorithm": "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
		"CertificateValidationMode": "ChainTrust",
		"RevocationMode": "NoCheck",
	  },
	  "Proxy": "/login",
	  "Claim": "",
	  "Value": "",
	  "AllowedHosts": "*",
	  "Urls": "http://0.0.0.0:5115"
    }
  
- ConnectionStrings.PostgreConnection: PostgreConnectionmigration: Cadena de conexión a la base de datos PostgreSQL.
- ConnectionStrings.PostgreConnectionmigration: PostgreConnectionmigration: Cadena de conexión a la base de datos PostgreSQL.
- LogLevel.Default: Nivel de error por defecto.
- LogLevel.Microsoft: Nivel de error para los errores propios de Microsoft.
- LogLevel.Microsoft.Hosting.Lifetime: Nivel de error para los errores de host.
- Saml2.IdPMetadata: URL del metadata proporcionado por el Identity Provier.
- Saml2.Issuer: Identificador del proveedor.
- Saml2.SignatureAlgorithm: Algoritmo de encriptación.
- Saml2.CertificateValidationMode: Especifica el modo de validación del certificado.
- Saml2.RevocationMode: Establece el modo de revocación que especifica si se produce una comprobación de revocación.
- Proxy: Directorio virtual que se ha configurado para el proxy inverso, en caso de que no se haya configurado dejar vacío.
- Claim: Nombre del Claim a leer.
- Value: Valor del Claim a leer.
- AllowedHosts: Hosts aceptados.
- Urls: Url en la que se va a lanzar la aplicación.

## Dependencias

- **ITfoxtec.Identity.Saml2**: versión 4.6.3
- **ITfoxtec.Identity.Saml2.MvcCore**: versión 4.6.3
- **Microsoft.EntityFrameworkCore**: versión 5.0.6
- **Npgsql.EntityFrameworkCore.PostgreSQL**: versión 5.0.6
