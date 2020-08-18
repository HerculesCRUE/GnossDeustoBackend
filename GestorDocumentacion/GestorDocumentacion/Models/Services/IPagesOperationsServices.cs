using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    public interface IPagesOperationsServices
    {
        public string GetPage(string name);
        public bool LoadPage();
        public List<string> GetPages();
        public bool DeletePage(string name);
    }
}
