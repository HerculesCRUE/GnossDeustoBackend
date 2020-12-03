// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Implementación de ISetRepository
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OaiPmhNet.Models.OAIPMH
{
    /// <summary>
    /// Implementación de ISetRepository
    /// </summary>
    public class SetRepository : ISetRepository
    {
        private readonly IOaiConfiguration _configuration;
        private readonly IList<Set> _sets;

        /// <summary>
        /// Cinstructor
        /// </summary>
        /// <param name="configuration">Configuración OAI-PMH</param>
        public SetRepository(IOaiConfiguration configuration)
        {
            _configuration = configuration;
            _sets = new List<Set>();

            foreach (string dir in Directory.GetDirectories("XML"))
            {   
                string setName = dir.Split(Path.DirectorySeparatorChar)[1];
                Set set = new Set();
                set.Spec = setName;
                set.Name = setName;
                set.Description = setName;
                _sets.Add(set);
            }
           
        }

        /// <summary>
        /// Obtiene los sets del repositorio en función de los argumentos pasados
        /// </summary>
        /// <param name="arguments">Parámetros de la consulta</param>        
        /// <param name="resumptionToken">Token de reanudación</param>
        /// <returns></returns>
        public SetContainer GetSets(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            SetContainer container = new SetContainer();
            IQueryable<Set> sets = _sets.AsQueryable().OrderBy(s => s.Name);
            int totalCount = sets.Count();
            container.Sets = sets.Take(_configuration.PageSize);
            return container;
        }
    }
}
