using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class ExceptionDetails
    {
        public string type { get; set; }
        public string title { get; set; }
        public string status { get; set; }
        public string detail { get; set; }
        public string traceId { get; set; }
    }
}
