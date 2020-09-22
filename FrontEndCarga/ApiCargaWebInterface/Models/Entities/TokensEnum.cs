// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Enumeración con los diferentes apis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Entities
{
    public enum TokensEnum
    {
        TokenCarga = 0,
        TokenUrisFactory = 1,
        TokenCron = 2,
        TokenOAIPMH = 3,
        TokenDocumentacion= 4
    }
}
