// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para gestionar las operaciones en memoria de los repositorios 
using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Clase para gestionar las operaciones en memoria de los repositorios 
    ///</summary>
    public class RepositoriesConfigMockService : IRepositoriesConfigService
    {
        readonly private List<RepositoryConfig> _configRepositories;

        ///<summary>
        ///Inicializa la lista de repositorios
        ///</summary>
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
                //Url = "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH",
                Url = "https://localhost:44308/OAI_PMH",
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

        ///<summary>
        ///Obtiene el listado de repositorios
        ///</summary>
        public List<RepositoryConfig> GetRepositoryConfigs()
        {
            return _configRepositories.OrderBy(repository => repository.Name).ToList();
        }

        ///<summary>
        ///Obtiene un repositorio
        ///</summary>
        ///<param name="id">Identificador del repositorio</param>
        public RepositoryConfig GetRepositoryConfigById(Guid id)
        {
            return _configRepositories.FirstOrDefault(repository => repository.RepositoryConfigID.Equals(id));
        }

        ///<summary>
        ///Elimina un repositorio
        ///</summary>
        ///<param name="identifier">Identificador del repositorio</param>
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
            catch (Exception)
            {
                return false;
            }
        }

        ///<summary>
        ///Añade un repositorio
        ///</summary>
        ///<param name="repositoryConfig">Repositorio a añadir</param>
        public Guid AddRepositoryConfig(RepositoryConfig repositoryConfig)
        {
            Guid repositoryConfigID = Guid.NewGuid();
            repositoryConfig.RepositoryConfigID = repositoryConfigID;
            _configRepositories.Add(repositoryConfig);
            return repositoryConfigID;
        }

        ///<summary>
        ///Modifica un repositorio
        ///</summary>
        ///<param name="repositoryConfig">Repositorio a modificar con los datos nuevos</param>
        public bool ModifyRepositoryConfig(RepositoryConfig repositoryConfig)
        {
            bool modified = false;
            RepositoryConfig repositoryConfigOriginal = GetRepositoryConfigById(repositoryConfig.RepositoryConfigID);
            if (repositoryConfigOriginal != null)
            {                
                repositoryConfigOriginal.Name = repositoryConfig.Name;
                repositoryConfigOriginal.Url = repositoryConfig.Url;
                repositoryConfigOriginal.OauthToken = repositoryConfig.OauthToken; 
                modified = true;
            }
            return modified;
        }
    }
}
