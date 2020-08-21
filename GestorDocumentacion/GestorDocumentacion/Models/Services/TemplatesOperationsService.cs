using GestorDocumentacion.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    public class TemplatesOperationsService : ITemplatesOperationsServices
    {
        private readonly EntityContext _context;
        public TemplatesOperationsService(EntityContext context)
        {
            _context = context;
        }
        public bool DeleteTemplate(Guid templateID)
        {
            Template template = _context.Template.FirstOrDefault(template => template.TemplateID.Equals(templateID));
            if(template != null)
            {
                _context.Entry(template).State = EntityState.Deleted;
                _context.SaveChanges();
            }
            return true;
        }

        public Template GetTemplate(string name)
        {
            return _context.Template.FirstOrDefault(template => template.Name.Equals(name));
        }

        public Template GetTemplate(Guid templateID)
        {
           return _context.Template.FirstOrDefault(template => template.TemplateID.Equals(templateID));
        }

        public List<Template> GetTemplates()
        {
            return _context.Template.ToList();
        }

        public bool LoadTemplate(Template template, bool isNew)
        {
            if (isNew)
            {
                if (template != null && !string.IsNullOrEmpty(template.Content) && !string.IsNullOrEmpty(template.Name) && GetTemplate(template.Name) == null)
                {
                    template.TemplateID = Guid.NewGuid();
                    _context.Template.Add(template);
                    _context.SaveChanges();
                    return true;
                }
            }
            else
            {
                var templateModify = GetTemplate(template.TemplateID);
                if (!string.IsNullOrEmpty(template.Content) && template.Content != templateModify.Content)
                {
                    templateModify.Content = template.Content;
                }
                if (!string.IsNullOrEmpty(template.Name) && template.Content != templateModify.Name)
                {
                    if (GetTemplate(template.Name) != null)
                    {
                        templateModify.Name = template.Name;
                    }
                    else
                    {
                        return false;
                    }
                }
                _context.SaveChanges();
            }
            return false;
        }
    }
}
