using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;

namespace API_CARGA.Models.Services
{
    public interface IRepositoriesConfigService
    {
        public List<RepositoryConfig> GetRepositoryConfigs();
        public RepositoryConfig GetRepositoryConfigByName(string name);
        public RepositoryConfig GetRepositoryConfigById(Guid id);
        public bool RemoveRepositoryConfig(Guid identifier);
        public Guid AddRepositoryConfig(RepositoryConfig repositoryConfig);
        public bool ModifyRepositoryConfig(RepositoryConfig repositoryConfig);
    }
}
