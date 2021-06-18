using System.Diagnostics.CodeAnalysis;

namespace Hercules.Asio.XML_RDF_Conversor.Models.Entities
{
    /// <summary>
    /// Token Bearer de seguridad.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TokenBearer
    {
        /// <summary>
        /// Token id.
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// Tiempo de expiración.
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// Tipo de token.
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// Scope.
        /// </summary>
        public string scope { get; set; }
    }
}
