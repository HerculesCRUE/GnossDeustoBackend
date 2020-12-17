using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Entities
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
