using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class SyncsConfigMockService : ISyncsConfigService
    {
        private List<SyncConfig> _configSyncs;
        public SyncsConfigMockService()
        {
            _configSyncs = new List<SyncConfig>();
            _configSyncs.Add(new SyncConfig
            {
                SyncConfigID = Guid.NewGuid(),
                Name = "ConfigSync_1",
                StartHour = "00:00",
                UpdateFrequency = 3600,
                RepositoryIdentifier = Guid.NewGuid(),
                RepositorySetIdentifiers = new List<string>() { "cvn" }
            });
            _configSyncs.Add(new SyncConfig
            {
                SyncConfigID = Guid.NewGuid(),
                Name = "ConfigSync_2",
                StartHour = "00:00",
                UpdateFrequency = 3600,
                RepositoryIdentifier = Guid.NewGuid(),
                RepositorySetIdentifiers = new List<string>() { "cvn" }
            });
            _configSyncs.Add(new SyncConfig
            {
                SyncConfigID = Guid.NewGuid(),
                Name = "ConfigSync_3",
                StartHour = "00:00",
                UpdateFrequency = 3600,
                RepositoryIdentifier = Guid.NewGuid(),
                RepositorySetIdentifiers = new List<string>() { "cvn" }
            });
            _configSyncs.Add(new SyncConfig
            {
                SyncConfigID = Guid.NewGuid(),
                Name = "ConfigSync_4",
                StartHour = "00:00",
                UpdateFrequency = 3600,
                RepositoryIdentifier = Guid.NewGuid(),
                RepositorySetIdentifiers = new List<string>() { "cvn" }
            });
            _configSyncs.Add(new SyncConfig
            {
                SyncConfigID = Guid.NewGuid(),
                Name = "ConfigSync_5",
                StartHour = "00:00",
                UpdateFrequency = 3600,
                RepositoryIdentifier = Guid.NewGuid(),
                RepositorySetIdentifiers = new List<string>() { "cvn" }
            });
            _configSyncs.Add(new SyncConfig
            {
                SyncConfigID = Guid.NewGuid(),
                Name = "ConfigSync_6",
                StartHour = "00:00",
                UpdateFrequency = 3600,
                RepositoryIdentifier = Guid.NewGuid(),
                RepositorySetIdentifiers = new List<string>() { "cvn" }
            }); ;
        }

        public List<SyncConfig> GetSyncConfigs()
        {
            return _configSyncs.OrderBy(sync => sync.Name).ToList();
        }

        //public RepositoryConfig GetRepositoryConfigByName(string name)
        //{
        //    return _configRepositories.FirstOrDefault(repository => repository.Name.Equals(name));
        //}

        public SyncConfig GetSyncConfigById(Guid id)
        {
            return _configSyncs.FirstOrDefault(sync => sync.SyncConfigID.Equals(id));
        }

        public bool RemoveSyncConfig(Guid identifier)
        {
            try
            {
                SyncConfig syncConfig = GetSyncConfigById(identifier);
                if (syncConfig != null)
                {
                    _configSyncs.Remove(syncConfig);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Guid AddSyncConfig(SyncConfig syncConfig)
        {
            Guid syncConfigID = Guid.Empty;
            //if (GetRepositoryConfigByName(repositoryConfig.Name) == null)
            //{
            syncConfigID = Guid.NewGuid();
            syncConfig.SyncConfigID = syncConfigID;
            _configSyncs.Add(syncConfig);
            //}
            return syncConfigID;
        }

        public bool ModifySyncConfig(SyncConfig syncConfig)
        {
            bool modified = false;
            SyncConfig syncConfigOriginal = GetSyncConfigById(syncConfig.SyncConfigID);
            if (syncConfigOriginal != null)
            {
                //CheckDataExceptions(repositoryConfigOriginal, repositoryConfig);
                syncConfigOriginal.Name = syncConfig.Name;
                syncConfigOriginal.StartHour = syncConfig.StartHour;
                syncConfigOriginal.UpdateFrequency = syncConfig.UpdateFrequency;
                syncConfigOriginal.RepositoryIdentifier = syncConfig.RepositoryIdentifier;
                syncConfigOriginal.RepositorySetIdentifiers = syncConfig.RepositorySetIdentifiers;
                modified = true;
            }
            return modified;
        }
    }
}
