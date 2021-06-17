// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Enumeración
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Enumeracion
{
    /// <summary>
    /// JobType
    /// </summary>
    public enum JobType
    {
        /// <summary>
        /// All
        /// </summary>
        All = 0,
        /// <summary>
        /// Failed
        /// </summary>
        Failed = 1,
        /// <summary>
        /// Succeeded
        /// </summary>
        Succeeded = 2,
        /// <summary>
        /// Processing
        /// </summary>
        Processing = 3
    }
}
