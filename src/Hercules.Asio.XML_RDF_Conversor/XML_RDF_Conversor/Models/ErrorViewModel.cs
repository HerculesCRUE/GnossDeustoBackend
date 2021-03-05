using System;

namespace Hercules.Asio.XML_RDF_Conversor.Models
{
    /// <summary>
    /// Error.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Id.
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// Permite visualizar el id.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
