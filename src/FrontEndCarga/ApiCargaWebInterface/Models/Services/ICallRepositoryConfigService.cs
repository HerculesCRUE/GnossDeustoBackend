// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para hacer llamadas al api de repositorios OAIPMH
using ApiCargaWebInterface.ViewModels;
using System;
using System.Collections.Generic;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Interfaz para hacer llamadas al api de repositorios OAIPMH
    /// </summary>
    public interface ICallRepositoryConfigService
    {
        /// <summary>
        /// Obtiene todos los repositorios OAIPMH
        /// </summary>
        /// <returns>Lista de repositorios OAIPMH</returns>
        public List<RepositoryConfigViewModel> GetRepositoryConfigs();
        /// <summary>
        /// Obtiene un repositorio OAIPMH
        /// </summary>
        /// <param name="id">Identificador del repositorio</param>
        /// <returns>Repositorio OAIPMH</returns>
        public RepositoryConfigViewModel GetRepositoryConfig(Guid id);
        /// <summary>
        /// Elimina un repositorio OAIPMH
        /// </summary>
        /// <param name="id">Identificador del repositorio OAIPMH</param>
        /// <returns>Exito</returns>
        public bool DeleteRepositoryConfig(Guid id);
        /// <summary>
        /// Crea un repositorio OAIPMH
        /// </summary>
        /// <param name="newRepositoryConfigView">Repositorio a crear</param>
        /// <returns>Repositorio creado</returns>
        public RepositoryConfigViewModel CreateRepositoryConfigView(RepositoryConfigViewModel newRepositoryConfigView);
        /// <summary>
        /// Modifica un repositorio OAIPMH
        /// </summary>
        /// <param name="repositoryConfigView">Repositorio a modificar</param>
        public void ModifyRepositoryConfig(RepositoryConfigViewModel repositoryConfigView);
    }
}
