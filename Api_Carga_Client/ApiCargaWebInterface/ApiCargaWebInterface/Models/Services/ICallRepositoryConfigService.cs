using ApiCargaWebInterface.ViewModels;
using System;
using System.Collections.Generic;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallRepositoryConfigService
    {
        public List<RepositoryConfigView> GetRepositoryConfigs();
        public RepositoryConfigView GetRepositoryConfig(Guid id);
        public bool DeleteRepositoryConfig(Guid id);
        public RepositoryConfigView CreateRepositoryConfigView(RepositoryConfigView newRepositoryConfigView);
        public void ModifyRepositoryConfig(RepositoryConfigView repositoryConfigView);
    }
}
