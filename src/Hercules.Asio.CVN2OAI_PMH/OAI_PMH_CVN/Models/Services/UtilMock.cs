using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.CVN2OAI_PMH.Models.Services
{
    public class UtilMock: IUtil
    {
        /// <summary>
        /// Obtiene los IDs de los curriculums desde una fecha de inicio
        /// </summary>
        /// <param name="pInicio">Fecha de inicio</param>
        /// <param name="pXML_CVN_Repository">Ruta del repositorio de CVN</param>
        /// <returns>Identificadores de os curriculums</returns>
        public HashSet<string> GetCurriculumsIDs(DateTime pInicio, string pXML_CVN_Repository)
        {
            HashSet<string> list = new HashSet<string>();
            list.Add("1");
            list.Add("2");
            return list;
        }
    }
}
