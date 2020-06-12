// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve para mostrar los datos de una tarea

namespace CronConfigure.ViewModels
{
    ///<summary>
    ///Clase que sirve para mostrar los datos de una tarea
    ///</summary>
    public class JobViewModel
    {
        public string Job { get; set; }
        public string State { get; set; }
        public string Id { get; set; }

        public string ExceptionDetails { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }
}
