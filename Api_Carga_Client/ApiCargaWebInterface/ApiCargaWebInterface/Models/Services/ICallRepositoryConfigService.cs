using ApiCargaWebInterface.ViewModels;
using System;
using System.Collections.Generic;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallRepositoryConfigService
    {
        public List<RepositoryConfigViewModel> GetRepositoryConfigs();
        public RepositoryConfigViewModel GetRepositoryConfig(Guid id);
        public bool DeleteRepositoryConfig(Guid id);
        public RepositoryConfigViewModel CreateRepositoryConfigView(RepositoryConfigViewModel newRepositoryConfigView);
        public void ModifyRepositoryConfig(RepositoryConfigViewModel repositoryConfigView);
    }
}
