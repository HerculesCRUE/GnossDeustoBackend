// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para las operaciones con el fichero de configuración
using Microsoft.AspNetCore.Http;

namespace UrisFactory.Models.Services
{
    ///<summary>
    ///Interfaz para las operaciones con el fichero de configuración
    ///</summary>
    public interface ISchemaConfigOperations
    {
        ///<summary>
        ///Obtiene el content type del fichero
        ///</summary>
        public abstract string GetContentType();
        ///<summary>
        ///Obtiene los bytes con el fichero de configuracion
        ///</summary>
        public abstract byte[] GetFileSchemaData();
        ///<summary>
        ///Guarda el fichero de configuracion
        ///</summary>
        ///<param name="formFile">cadena de texto emulando el json del fichero de configuracion</param>
        public abstract bool SaveConfigFile(IFormFile formFile);
        ///<summary>
        ///Guarda el fichero de configuracion
        ///</summary>
        public abstract bool SaveConfigJson();
    }
}
