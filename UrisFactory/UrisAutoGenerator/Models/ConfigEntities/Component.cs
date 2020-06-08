
namespace UrisFactory.Models.ConfigEntities
{
    ///<summary>
    ///Objeto que representa a un objeto que hay dentro de la estructura Components del fichero json de configuración
    ///</summary>
    public class Component
    {
        public string UriComponent { get; set; }
        public string UriComponentValue { get; set; }
        public int UriComponentOrder { get; set; }
        public bool Mandatory { get; set; }
        public string FinalCharacter { get; set; }
    }
}
