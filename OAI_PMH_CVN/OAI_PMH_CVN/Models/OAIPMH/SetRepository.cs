using OaiPmhNet.Converters;
using OaiPmhNet.Models;
using System.Collections.Generic;
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
        /// <param name="sets">Lista de sets disponibles</param>
        public SetRepository(IOaiConfiguration configuration, IList<Set> sets)
        {
            _configuration = configuration;
            _sets = sets;
        }

        /// <summary>
        /// Obtiene los sets del repositorio en función de los argumentos pasados
        /// </summary>
        /// <param name="arguments">Parámetros de la consulta</param>        /// 
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
