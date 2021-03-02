using System.Diagnostics.CodeAnalysis;

namespace Hercules.Asio.XML_RDF_Conversor.Models.Entities
{
    [ExcludeFromCodeCoverage]
    public class TokenBearer
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
}
