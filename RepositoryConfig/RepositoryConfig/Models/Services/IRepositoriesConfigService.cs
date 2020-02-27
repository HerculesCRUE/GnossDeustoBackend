using RepositoryConfigSolution.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoryConfigSolution.Models.Services
{
    public interface IRepositoriesConfigService
    {
        public List<RepositoryConfig> GetRepositoryConfigs();
        public RepositoryConfig GetRepositoryConfigByName(string name);
        public RepositoryConfig GetRepositoryConfigById(Guid id);
        public bool RemoveRepositoryConfig(string name);
        public bool AddRepositoryConfig(RepositoryConfig repositoryConfig);
        public bool ModifyRepositoryConfig(RepositoryConfig repositoryConfig);
    }
}

