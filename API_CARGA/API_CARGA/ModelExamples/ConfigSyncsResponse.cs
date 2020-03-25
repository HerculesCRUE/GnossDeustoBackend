using API_CARGA.Models.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;

namespace API_CARGA.ModelExamples
{
    public class ConfigSyncsResponse : IExamplesProvider<List<SyncConfig>>
    {
        public List<SyncConfig> GetExamples()
        {
            List<SyncConfig> syncConfigs = new List<SyncConfig>();
            syncConfigs.Add(new SyncConfig
            {
                SyncConfigID = Guid.NewGuid(),
                Name = "ConfigSync_1",
                StartHour = "00:00",
                UpdateFrequency = 3600,
                RepositoryIdentifier = Guid.NewGuid(),
                RepositorySetIdentifiers = new List<string>() { "cvn" }
            });
            syncConfigs.Add(new SyncConfig
            {
                SyncConfigID = Guid.NewGuid(),
                Name = "ConfigSync_2",
                StartHour = "00:00",
                UpdateFrequency = 3600,
                RepositoryIdentifier = Guid.NewGuid(),
                RepositorySetIdentifiers = new List<string>() { "project" }
            });
            syncConfigs.Add(new SyncConfig
            {
                SyncConfigID = Guid.NewGuid(),
                Name = "ConfigSync_3",
                StartHour = "00:00",
                UpdateFrequency = 3600,
                RepositoryIdentifier = Guid.NewGuid(),
                RepositorySetIdentifiers = new List<string>() { "researcher" }
            });
            return syncConfigs;
        }
    }
}
