using Linked_Data_Server.Models.Entities;
using Linked_Data_Server.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.LinkedDataServer.Utility
{
    public interface ISparqlUtility
    {
        public SparqlObject SelectData(ConfigService pConfigService, string pGraph, string pConsulta, ref string pXAppServer);
    }
}
