using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargaDataSetMurcia.Model.XML
{
    public class Articulo
    {
        public string CODIGO{ get; set; }
        public string TITULO { get; set; }
        public string ANO { get; set; }
        public string PAIS_CODIGO { get; set; }
        public string REIS_ISSN { get; set; }
        public string CATALOGO { get; set; }
        public string AREA { get; set; }
        public string NOMBRE_REVISTA { get; set; }
        public string CUARTIL_REVISTA { get; set; }
        public string URL_REVISTA { get; set; }
        public string VOLUMEN { get; set; }
        public string NUMERO { get; set; }
        public string PAGDESDE { get; set; }
        public string PAGHASTA { get; set; }
        public string NUMPAG { get; set; }
        public string COAUT_EXT { get; set; }
        public string ARTI_DOI { get; set; }
    }
}
