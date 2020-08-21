using GestorDocumentacion.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    public interface IPagesOperationsServices
    {
        public Page GetPage(string name);
        public Page GetPage(Guid pageID);
        public bool LoadPage(Page page, bool isNew);
        public List<Page> GetPages();
        public bool DeletePage(Guid pageID);
    }
}
