using API_CARGA.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class RepositoriesConfigBDService : IRepositoriesConfigService
    {
        private readonly EntityContext _context;
        public RepositoriesConfigBDService(EntityContext context)
        {
            _context = context;
        }

        public List<RepositoryConfig> GetRepositoryConfigs()
        {
            return _context.RepositoryConfig.Include(item => item.ShapeConfig).OrderBy(repository => repository.Name).ToList();
        }

        //public RepositoryConfig GetRepositoryConfigByName(string name)
        //{
        //    return _configRepositories.FirstOrDefault(repository => repository.Name.Equals(name));
        //}

        public RepositoryConfig GetRepositoryConfigById(Guid id)
        {
            return _context.RepositoryConfig.Include(item => item.ShapeConfig).FirstOrDefault(repository => repository.RepositoryConfigID.Equals(id));
        }

        public bool RemoveRepositoryConfig(Guid identifier)
        {
            try
            {
                RepositoryConfig repositoryConfig = GetRepositoryConfigById(identifier);
               // List<ShapeConfig> shapes = 
                if (repositoryConfig != null)
                {
                    if (repositoryConfig.ShapeConfig != null)
                    {
                        foreach (var shape in repositoryConfig.ShapeConfig)
                        {
                            _context.Entry(shape).State = EntityState.Deleted;
                        }
                    }
                    
                    _context.Entry(repositoryConfig).State = EntityState.Deleted;
                    _context.SaveChanges();
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
            _context.RepositoryConfig.Add(repositoryConfig);
            _context.SaveChanges();
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
                _context.SaveChanges();
                modified = true;
                
            }
            return modified;
        }
    }
}
