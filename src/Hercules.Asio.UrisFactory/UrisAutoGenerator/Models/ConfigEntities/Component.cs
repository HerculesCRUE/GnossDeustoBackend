// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Objeto que representa a un objeto que hay dentro de la estructura Components del fichero json de configuración
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
