﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Autorización a hangfire dashboard
using Hangfire.Dashboard;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.Filters
{
    /// <summary>
    /// HangfireDashboardNoAuthorizationFilter.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HangfireDashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// Authorize.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
