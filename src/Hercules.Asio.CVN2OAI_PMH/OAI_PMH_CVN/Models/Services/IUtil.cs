using OAI_PMH_CVN.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.CVN2OAI_PMH.Models.Services
{
    public interface IUtil
    {
        public HashSet<string> GetCurriculumsIDs(DateTime pInicio, string pXML_CVN_Repository);
        public string GetCurriculum(string pId, bool pOnlyIDs, string pXML_CVN_Repository);
    }
}
