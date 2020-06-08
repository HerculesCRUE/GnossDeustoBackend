using Newtonsoft.Json;
using System.IO;
using System.Text;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    ///<summary>
    ///Transforma el texto en formato Json del fichero en un objeto de estrcutura de uris general, utilizable por el programa
    ///</summary>
    public class ReaderConfigJson
    {
        ///<summary>
        ///Lee el contenido del fichero de configuración y lo transforma en un objeto de tipo UriStructureGeneral
        ///</summary>
        public static UriStructureGeneral Read()
        {
            string texto = File.ReadAllText("Config/UrisConfig.json");
            return Read(texto);
        }

        ///<summary>
        ///Lee el contenido de una cadena de texto en formato Json y lo transforma en un objeto de tipo UriStructureGeneral
        ///</summary>
        ///<param name="texto">cadena de texto en formato Json</param>
        public static UriStructureGeneral Read(string texto)
        {
            UriStructureGeneral uriStructure = JsonConvert.DeserializeObject<UriStructureGeneral>(texto);
            return uriStructure;
        }
    }
}
