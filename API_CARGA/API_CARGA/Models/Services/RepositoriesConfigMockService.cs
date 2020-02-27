using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class RepositoriesConfigMockService : IRepositoriesConfigService
    {
        private List<RepositoryConfig> _configRepositories;
        public RepositoriesConfigMockService()
        {
            _configRepositories = new List<RepositoryConfig>();
            _configRepositories.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_1",
                Url = "config\\repository"
            });
            _configRepositories.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_2",
                Url = "config\\repository"
            });
            _configRepositories.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_3",
                Url = "config\\repository"
            });
            _configRepositories.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_4",
                Url = "config\\repository"
            });
            _configRepositories.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_5",
                Url = "config\\repository"
            });
        }

        public List<RepositoryConfig> GetRepositoryConfigs()
        {
            return _configRepositories;
        }

        public RepositoryConfig GetRepositoryConfigByName(string name)
        {
            return _configRepositories.FirstOrDefault(repository => repository.Name.Equals(name));
        }

        public RepositoryConfig GetRepositoryConfigById(Guid id)
        {
            return _configRepositories.FirstOrDefault(repository => repository.RepositoryConfigID.Equals(id));
        }

        public bool RemoveRepositoryConfig(Guid identifier)
        {
            try
            {
                RepositoryConfig repositoryConfig = GetRepositoryConfigById(identifier);
                if (repositoryConfig != null)
                {
                    _configRepositories.Remove(repositoryConfig);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddRepositoryConfig(RepositoryConfig repositoryConfig)
        {
            bool added = false;
            if (GetRepositoryConfigByName(repositoryConfig.Name) == null)
            {
                _configRepositories.Add(repositoryConfig);
                added = true;
            }
            return added;
        }

        public bool ModifyRepositoryConfig(RepositoryConfig repositoryConfig)
        {
            bool modified = false;
            RepositoryConfig repositoryConfigOriginal = GetRepositoryConfigById(repositoryConfig.RepositoryConfigID);
            if (repositoryConfigOriginal != null)
            {
                //CheckDataExceptions(repositoryConfigOriginal, repositoryConfig);
                repositoryConfigOriginal.Name = repositoryConfig.Name;
                repositoryConfigOriginal.Url = repositoryConfig.Url;
                //repositoryConfigOriginal.InitialDate = 
                modified = true;

            }
            return modified;
        }
    }
}
