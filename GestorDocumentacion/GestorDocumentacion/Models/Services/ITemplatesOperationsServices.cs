using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    public interface ITemplatesOperationsServices
    {
        public string GetTemplate(string name);
        public bool LoadTemploate();
        public List<string> GetTemplates();
        public bool DeleteTemplate(string name);
    }
}
