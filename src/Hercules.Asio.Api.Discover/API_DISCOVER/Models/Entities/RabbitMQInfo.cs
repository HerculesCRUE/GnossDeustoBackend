// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System.Diagnostics.CodeAnalysis;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// RabbitMQInfo
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RabbitMQInfo
    {
        /// <summary>
        /// UsernameRabbitMq
        /// </summary>
        public string UsernameRabbitMq { get; set; }
        /// <summary>
        /// PasswordRabbitMq
        /// </summary>
        public string PasswordRabbitMq { get; set; }
        /// <summary>
        /// VirtualHostRabbitMq
        /// </summary>
        public string VirtualHostRabbitMq { get; set; }
        /// <summary>
        /// HostNameRabbitMq
        /// </summary>
        public string HostNameRabbitMq { get; set; }
        /// <summary>
        /// UriRabbitMq
        /// </summary>
        public string UriRabbitMq { get; set; }
    }
}
