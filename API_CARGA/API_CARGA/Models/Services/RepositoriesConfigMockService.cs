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
                Url = "config\\repository",
                OauthToken = "12weq1"
            });
            _configRepositories.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_2",
                Url = "config\\repository",
                OauthToken = "11389"
            });
            _configRepositories.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_3",
                Url = "config\\repository",
                OauthToken = "1238912"
            });
            _configRepositories.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_4",
                Url = "config\\repository",
                OauthToken = "asf46s"
            });
            _configRepositories.Add(new RepositoryConfig
            {
                RepositoryConfigID = new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"),
                Name = "ConfigRepository_5",
                Url = "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH",
                OauthToken = "87f9"
            }) ;
            _configRepositories.Add(new RepositoryConfig
            {
                RepositoryConfigID = new Guid("11111111-1111-1111-1111-111111111111"),
                Name = "CVN_OAI_PMH",
                Url = "https://localhost:44353/OAI_PMH",
                OauthToken = "87f9j"
            }); 
        }

        public List<RepositoryConfig> GetRepositoryConfigs()
        {
            return _configRepositories.OrderBy(repository => repository.Name).ToList();
        }

        //public RepositoryConfig GetRepositoryConfigByName(string name)
        //{
        //    return _configRepositories.FirstOrDefault(repository => repository.Name.Equals(name));
        //}

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

        public Guid AddRepositoryConfig(RepositoryConfig repositoryConfig)
        {
            Guid repositoryConfigID = Guid.Empty;
            //if (GetRepositoryConfigByName(repositoryConfig.Name) == null)
            //{
            repositoryConfigID = Guid.NewGuid();
            repositoryConfig.RepositoryConfigID = repositoryConfigID;
            _configRepositories.Add(repositoryConfig);
            //}
            return repositoryConfigID;
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
                repositoryConfigOriginal.OauthToken = repositoryConfig.OauthToken; 
                modified = true;

            }
            return modified;
        }
    }
}
