// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio

using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.Filters
{
    /// <summary>
    /// ProlongExpirationTimeAttribute.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProlongExpirationTimeAttribute : JobFilterAttribute, IApplyStateFilter
    {
        /// <summary>
        /// OnStateUnapplied.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="transaction"></param>
        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(60);
        }

        /// <summary>
        /// OnStateApplied.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="transaction"></param>
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(60);
        }
    }
}
