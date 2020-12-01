using System;
using System.Collections.Generic;
using System.Text;

namespace API_DISCOVER.Models.Entities.ExternalAPIs
{
    interface I_ExternalAPI
    {
        string Name { get; }
        string HomePage { get; }
        string Id { get; }
    }
}
