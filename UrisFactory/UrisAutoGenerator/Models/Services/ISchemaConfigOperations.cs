using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
