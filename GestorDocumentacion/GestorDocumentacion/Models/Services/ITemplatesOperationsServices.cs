using GestorDocumentacion.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    public interface ITemplatesOperationsServices
    {
        public Template GetTemplate(string name);
        public Template GetTemplate(Guid templateID);
        public bool LoadTemplate(Template template, bool isNew);
        public List<Template> GetTemplates();
        public bool DeleteTemplate(Guid templateID);
    }
}
