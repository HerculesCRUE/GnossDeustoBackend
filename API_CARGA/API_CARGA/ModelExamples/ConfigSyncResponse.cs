using API_CARGA.Models.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.ModelExamples
{
    public class ConfigSyncResponse : IExamplesProvider<SyncConfig>
    {
        public SyncConfig GetExamples()
        {
            return new SyncConfig
            {
                SyncConfigID = Guid.NewGuid(),
                Name = "ConfigSync_1",
                StartHour = "00:00",
                UpdateFrequency = 3600,
                RepositoryIdentifier = Guid.NewGuid(),
                RepositorySetIdentifiers = new List<string>() { "cvn" }
            };
        }
    }
}
