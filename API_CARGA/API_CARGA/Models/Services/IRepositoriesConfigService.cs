using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Interfaz para gestionar las operaciones de los repositorios 
    ///</summary>
    public interface IRepositoriesConfigService
    {
        ///<summary>
        ///Obtiene el listado de repositorios
        ///</summary>
        public List<RepositoryConfig> GetRepositoryConfigs();
        ///<summary>
        ///Obtiene un repositorio
        ///</summary>
        ///<param name="id">Identificador del repositorio</param>
        public RepositoryConfig GetRepositoryConfigById(Guid id);
        ///<summary>
        ///Elimina un repositorio
        ///</summary>
        ///<param name="identifier">Identificador del repositorio</param>
        public bool RemoveRepositoryConfig(Guid identifier);
        ///<summary>
        ///Añade un repositorio
        ///</summary>
        ///<param name="repositoryConfig">Repositorio a añadir</param>
        public Guid AddRepositoryConfig(RepositoryConfig repositoryConfig);
        ///<summary>
        ///Modifica un repositorio
        ///</summary>
        ///<param name="repositoryConfig">Repositorio a modificar con los datos nuevos</param>
        public bool ModifyRepositoryConfig(RepositoryConfig repositoryConfig);
    }
}
