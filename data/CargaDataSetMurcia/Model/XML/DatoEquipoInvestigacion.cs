using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargaDataSetMurcia.Model.XML
{
    public class DatoEquipoInvestigacion
    {
        public string IDGRUPOINVESTIGACION { get; set; }
        public string NUMERO { get; set; }
        public string IDPERSONA { get; set; }
        public string CODTIPOPARTICIPACION { get; set; }
        public string RESPONSABLE { get; set; }
        public string EDP { get; set; }
        public string FECHAINICIOPERIODO { get; set; }
        public string FECHAFINPERIODO { get; set; }
    }
}
