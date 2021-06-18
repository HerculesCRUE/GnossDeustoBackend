using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.XML_RDF_Conversor.Models.Services
{
    public class CallUrisFactoryApiMockService : ICallUrisFactoryApiService
    {
        public string GetUri(string resourceClass, string identifier)
        {
            if (identifier.Equals("100070"))
            {
                return "http://graph.um.es/res/academic-article/100070";
            }
            else if (identifier.Equals("1024346"))
            {
                return "http://graph.um.es/res/person/1024346";
            }
            else if (identifier.Equals("1003410"))
            {
                return "http://graph.um.es/res/person/1003410";
            }
            else if (identifier.Equals("1024377"))
            {
                return "http://graph.um.es/res/person/1024377";
            }
            else if (identifier.Equals("RU+SCI+IMAPP+MP"))
            {
                return "http://graph.um.es/res/project/RU+SCI+IMAPP+MP";
            }
            else if (identifier.Equals("J102909"))
            {
                return "http://graph.um.es/res/journal/J102909";
            }
            else { return ""; }
        }
    }
}
