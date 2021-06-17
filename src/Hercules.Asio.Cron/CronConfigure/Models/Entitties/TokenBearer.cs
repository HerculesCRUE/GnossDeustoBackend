using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Entitties
{
    /// <summary>
    /// TokenBearer.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TokenBearer
    {
        /// <summary>
        /// access_token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// expires_in
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// token_type
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// scope
        /// </summary>
        public string scope { get; set; }
    }
}
