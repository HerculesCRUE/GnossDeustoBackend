// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System.Diagnostics.CodeAnalysis;

namespace API_DISCOVER.Models.Entities
{
    [ExcludeFromCodeCoverage]
    public class RabbitMQInfo
    {
        public string UsernameRabbitMq { get; set; }

        public string PasswordRabbitMq { get; set; }

        public string VirtualHostRabbitMq { get; set; }

        public string HostNameRabbitMq { get; set; }

        public string UriRabbitMq { get; set; }
    }
}
