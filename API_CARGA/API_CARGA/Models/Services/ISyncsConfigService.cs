using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;

namespace API_CARGA.Models.Services
{
    public interface ISyncsConfigService
    {
        public List<SyncConfig> GetSyncConfigs();

        public SyncConfig GetSyncConfigById(Guid id);
        public bool RemoveSyncConfig(Guid identifier);
        public Guid AddSyncConfig(SyncConfig repositoryConfig);
        public bool ModifySyncConfig(SyncConfig repositoryConfig);
    }
}
