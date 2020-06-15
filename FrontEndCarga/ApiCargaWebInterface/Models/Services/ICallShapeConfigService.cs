// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using ApiCargaWebInterface.ViewModels;
using System;
using System.Collections.Generic;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallShapeConfigService
    {
        public List<ShapeConfigViewModel> GetShapeConfigs();
        public ShapeConfigViewModel GetShapeConfig(Guid id);
        public bool DeleteShapeConfig(Guid id);
        public ShapeConfigViewModel CreateShapeConfig(ShapeConfigCreateModel newRepositoryConfigView);
        public void ModifyShapeConfig(ShapeConfigEditModel repositoryConfigView);
    }
}
