// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para gestionar las operaciones en base de datos de los repositorios 
using API_CARGA.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Clase para gestionar las operaciones en base de datos de los repositorios 
    ///</summary>
    [ExcludeFromCodeCoverage]
    public class RepositoriesConfigBDService : IRepositoriesConfigService
    {
        private readonly EntityContext _context;
        public RepositoriesConfigBDService(EntityContext context)
        {
            _context = context;
        }

        ///<summary>
        ///Obtiene el listado de repositorios
        ///</summary>
        public List<RepositoryConfig> GetRepositoryConfigs()
        {
            return _context.RepositoryConfig.Include(item => item.ShapeConfig).OrderBy(repository => repository.Name).ToList();
        }

        ///<summary>
        ///Obtiene un repositorio
        ///</summary>
        ///<param name="id">Identificador del repositorio</param>
        public RepositoryConfig GetRepositoryConfigById(Guid id)
        {
            return _context.RepositoryConfig.Include(item => item.ShapeConfig).Include(item => item.RepositoryConfigSet).FirstOrDefault(repository => repository.RepositoryConfigID.Equals(id));
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
            _context.RepositoryConfig.Add(repositoryConfig);
            _context.SaveChanges();
            
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
                _context.SaveChanges();
                modified = true;                
            }
            return modified;
        }
    }
}
