// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para hacer llamadas al api de Shapes
using ApiCargaWebInterface.ViewModels;
using System;
using System.Collections.Generic;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Interfaz para hacer llamadas al api de Shapes
    /// </summary>
    public interface ICallShapeConfigService
    {
        /// <summary>
        /// Obtiene todos las configuraciones de validación
        /// </summary>
        /// <returns>Lista de configuraciones</returns>
        public List<ShapeConfigViewModel> GetShapeConfigs();
        /// <summary>
        /// Obtiene una configuración de validación
        /// </summary>
        /// <param name="id">Identificador del shape</param>
        /// <returns>configuración de validación</returns>
        public ShapeConfigViewModel GetShapeConfig(Guid id);
        /// <summary>
        /// Elimina una configuración de validación
        /// </summary>
        /// <param name="id">Identificador del shape</param>
        /// <returns>Si se ha completado con éxito</returns>
        public bool DeleteShapeConfig(Guid id);
        /// <summary>
        /// Añade una configuración de validación mediante un shape SHACL.
        /// </summary>
        /// <param name="newRepositoryConfigView">Configuración de validación a añadir</param>
        /// <returns>Configuración creada</returns>
        public ShapeConfigViewModel CreateShapeConfig(ShapeConfigCreateModel newRepositoryConfigView);
        /// <summary>
        /// Modifica una validación de configuración
        /// </summary>
        /// <param name="repositoryConfigView">Configuración de validación a modificar</param>
        public void ModifyShapeConfig(ShapeConfigEditModel repositoryConfigView);
    }
}
